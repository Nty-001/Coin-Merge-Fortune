using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace CoinMerge.Recovery.Editor
{
    public static class WidthFitAuthor
    {
        static void Apply(GameObject root)
        {
            var menus=root.GetComponent<RecoveredGameSession>().menus;
            // Only fixed modal panels need an additional height bound. Main and both
            // full-screen withdrawal pages retain the original fixed-width layout.
            foreach(int index in new[]{2,6,8})
            {
                var popup=menus.pages[index].GetComponentInChildren<RecoveredMenuPopup>(true);
                if(!popup)
                {
                    popup=menus.pages[index].AddComponent<RecoveredMenuPopup>();
                    popup.content=menus.pages[index].transform.Find("content");
                    popup.startScale=1;popup.duration=.001f;
                }
                var content=(RectTransform)popup.content;
                Vector3[] corners=new Vector3[4];Vector2 extent=Vector2.zero;
                foreach(var graphic in content.GetComponentsInChildren<Graphic>(true))
                {
                    if(!graphic.enabled||graphic.GetComponentInParent<RectMask2D>()||graphic.GetComponentInParent<Mask>())continue;
                    bool visible=true;
                    for(Transform t=graphic.transform;t!=content;t=t.parent)if(!t.gameObject.activeSelf)visible=false;
                    if(!visible)continue;
                    graphic.rectTransform.GetLocalCorners(corners);
                    var matrix=Matrix4x4.identity;
                    for(Transform t=graphic.transform;t!=content;t=t.parent)
                        matrix=Matrix4x4.TRS(t.localPosition,t.localRotation,t.localScale)*matrix;
                    foreach(var point in corners)
                    {
                        var local=matrix.MultiplyPoint3x4(point);
                        extent.x=Mathf.Max(extent.x,Mathf.Abs(local.x));
                        extent.y=Mathf.Max(extent.y,Mathf.Abs(local.y));
                    }
                }
                popup.availableFrame=(RectTransform)menus.pages[index].transform;
                popup.fittedVisualSize=extent*2;popup.fitMargin=20;
                Debug.Log("MODAL_VISUAL_ENVELOPE "+index+" "+popup.fittedVisualSize);
            }
        }
        public static void Author()
        {
            const string path="Assets/Prefabs/Runtime/RecoveredMain.prefab";
            var root=PrefabUtility.LoadPrefabContents(path);
            try{Apply(root);PrefabUtility.SaveAsPrefabAsset(root,path);}
            finally{PrefabUtility.UnloadPrefabContents(root);}
            var scene=EditorSceneManager.OpenScene("Assets/Scenes/RecoveredMain.unity");
            foreach(var go in scene.GetRootGameObjects())if(go.GetComponent<RecoveredGameSession>())Apply(go);
            EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();
        }
        public static void Review(){Author();DeviceSafeAreaChecks.Run();DeviceLayoutReview.Run();}
    }
}
