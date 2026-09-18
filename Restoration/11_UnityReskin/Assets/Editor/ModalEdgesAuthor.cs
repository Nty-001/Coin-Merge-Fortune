using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace CoinMerge.Recovery.Editor
{
    public static class ModalEdgesAuthor
    {
        static void Apply(GameObject root)
        {
            var session=root.GetComponent<RecoveredGameSession>();
            var backdrop=root.transform.Find("SafeAreaBackdrop");
            var parent=backdrop.Find("BackgroundCanvas");
            var component=backdrop.GetComponent<FullScreenModalEdges>();
            if(!component)component=backdrop.gameObject.AddComponent<FullScreenModalEdges>();
            component.contentCamera=session.worldCamera;component.backdropCamera=backdrop.GetComponent<Camera>();
            var masks=new List<Image>();
            foreach(var image in root.GetComponentsInChildren<Image>(true))
            {
                string name=image.name;
                if(name!="mask"&&name!="maskNode"&&name!="sprite_nask_bg")continue;
                if(image.color.r!=0||image.color.g!=0||image.color.b!=0||image.color.a<=0)continue;
                masks.Add(image);
            }
            if(!masks.Contains(session.rewardView.mask.GetComponent<Image>()))throw new Exception("Reward dimmer missing");
            component.sourceMasks=masks.ToArray();
            Image Edge(string name)
            {
                var existing=parent.Find(name);
                var image=existing?existing.GetComponent<Image>():new GameObject(name,typeof(RectTransform),typeof(CanvasRenderer),typeof(Image)).GetComponent<Image>();
                image.transform.SetParent(parent,false);image.gameObject.layer=parent.gameObject.layer;
                image.color=Color.clear;image.raycastTarget=false;image.transform.SetAsLastSibling();
                return image;
            }
            component.top=Edge("ModalTopEdge");component.bottom=Edge("ModalBottomEdge");
            component.left=Edge("ModalLeftEdge");component.right=Edge("ModalRightEdge");
            Debug.Log("MODAL_EDGE_SOURCES "+masks.Count);
        }
        public static void Author()
        {
            const string path="Assets/Prefabs/Runtime/RecoveredMain.prefab";
            var root=PrefabUtility.LoadPrefabContents(path);
            try{Apply(root);PrefabUtility.SaveAsPrefabAsset(root,path);}finally{PrefabUtility.UnloadPrefabContents(root);}
            var scene=EditorSceneManager.OpenScene("Assets/Scenes/RecoveredMain.unity");
            foreach(var go in scene.GetRootGameObjects())if(go.GetComponent<RecoveredGameSession>())Apply(go);
            EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();
        }
        public static void Review(){Author();ModalEdgesReview.Run();}
    }
}
