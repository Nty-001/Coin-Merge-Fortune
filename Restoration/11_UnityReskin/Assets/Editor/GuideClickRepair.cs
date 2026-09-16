using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
namespace CoinMerge.Recovery.Editor
{
    public static class GuideClickRepair
    {
        static Button VisibleButton(GameObject node)
        {
            var graphic=node.GetComponent<Graphic>();
            if(!graphic||!graphic.enabled)throw new InvalidOperationException("Guide button has no visible Graphic: "+node.name);
            var button=node.GetComponent<Button>()??node.AddComponent<Button>();
            button.targetGraphic=graphic;button.transition=Selectable.Transition.None;graphic.raycastTarget=true;return button;
        }
        public static void Configure(RecoveredGuideView view)
        {
            var nodes=new Dictionary<int,GameObject>();
            foreach(var node in view.GetComponentsInChildren<RecoveredNode>(true))nodes.Add(node.sourceObjectId,node.gameObject);
            foreach(var graphic in view.GetComponentsInChildren<Graphic>(true))graphic.raycastTarget=false;
            // Source clickArea covers the viewport. The existing visible dimmer is the native Button surface.
            view.oneButton=VisibleButton(nodes[27]);view.threeButton=VisibleButton(nodes[32]);
            view.fourButton=VisibleButton(nodes[2]);view.backdropButton=VisibleButton(nodes[7]);
            foreach(int id in new[]{18,14})
            {
                var obsolete=nodes[id].GetComponent<Button>();if(obsolete)UnityEngine.Object.DestroyImmediate(obsolete);
            }
        }
        [MenuItem("Coin Merge/Repair guide pointer targets")]
        public static void Run()
        {
            const string path="Assets/Prefabs/Runtime/RecoveredMain.prefab";
            var root=PrefabUtility.LoadPrefabContents(path);
            try{Configure(root.GetComponentInChildren<RecoveredGuideView>(true));PrefabUtility.SaveAsPrefabAsset(root,path);}
            finally{PrefabUtility.UnloadPrefabContents(root);}
            var scene=EditorSceneManager.OpenScene("Assets/Scenes/RecoveredMain.unity");
            foreach(var item in scene.GetRootGameObjects())
            {
                var view=item.GetComponentInChildren<RecoveredGuideView>(true);if(view)Configure(view);
            }
            EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();
            Debug.Log("GUIDE_POINTER_TARGETS_REPAIRED");
        }
    }
}
