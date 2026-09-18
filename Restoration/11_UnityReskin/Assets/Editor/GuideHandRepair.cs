using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace CoinMerge.Recovery.Editor
{
    public static class GuideHandRepair
    {
        public static void Apply(GameObject root)
        {
            var session=root.GetComponent<RecoveredGameSession>();
            var guide=session.guideView;
            foreach(var binding in session.menus.actions)
                if(binding.action==3&&binding.button.transform.IsChildOf(session.playfieldLayout.upper))
                    guide.cashTarget=(RectTransform)binding.button.transform;
            if(!guide.cashTarget)throw new InvalidOperationException("Cash HUD button not found");
            var content=guide.stepThree.transform.Find("content");
            guide.cashHand=(RectTransform)content.Find("hand");
            guide.cashPanel=(RectTransform)content.Find("bg");
            guide.cashHand.pivot=new Vector2(.12f,.9f); // Fingertip of the supplied 139x141 sprite.
            guide.cashHand.GetComponent<Image>().preserveAspect=true;
            guide.cashTargetPoint=new Vector2(.65f,.45f);
            guide.cashPanelGap=16;
            // The old tutorial copy was drawn over the live responsive balance and button.
            content.Find("money").gameObject.SetActive(false);
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
        public static void Build(){Author();AndroidApkBuild.Run();}
    }
}
