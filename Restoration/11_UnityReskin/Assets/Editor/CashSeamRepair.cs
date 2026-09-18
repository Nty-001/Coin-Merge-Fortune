using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace CoinMerge.Recovery.Editor
{
    public static class CashSeamRepair
    {
        static void Apply(GameObject root)
        {
            var page=root.GetComponent<RecoveredGameSession>().menus.pages[RecoveredMainMenus.Fake];
            var uuid=page.GetComponent<RecoveredNode>().sourceUuid;
            var nodes=new Dictionary<int,RectTransform>();
            foreach(var n in page.GetComponentsInChildren<RecoveredNode>(true))
                if(n.sourceUuid==uuid)nodes[n.sourceObjectId]=(RectTransform)n.transform;
            // Keep a single continuous full-page sky instead of abrupt, separately cropped strips.
            var hero=nodes[3].Find("CashHeroSky");
            if(hero)hero.GetComponent<Image>().enabled=false;
            nodes[24].GetComponent<Image>().enabled=false;
            var layout=page.GetComponent<RecoveredCashPageLayout>();
            var oldMask=layout.viewport.GetComponent<Mask>();if(oldMask)oldMask.enabled=false;
            var clip=layout.viewport.GetComponent<RectMask2D>();
            if(!clip)clip=layout.viewport.gameObject.AddComponent<RectMask2D>();
            clip.softness=new Vector2Int(0,12);
            var surface=layout.viewport.GetComponent<Image>();
            if(surface){surface.color=Color.clear;surface.raycastTarget=true;surface.enabled=true;}
            // The existing native button, geometry, text and code-bound event are untouched.
            if(!nodes[25].GetComponent<Button>())throw new Exception("Cash withdraw button missing");
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
        public static void Review(){Author();DeviceLayoutReview.Run();}
        public static void Build(){Author();DeviceSafeAreaChecks.Run();AndroidApkBuild.Run();}
    }
}
