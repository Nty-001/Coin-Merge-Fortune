using System;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
namespace CoinMerge.Recovery.Editor
{
    // User approved PURE_ART. The only scene edits are Image.sprite references.
    public static class HomeArtOnlyAuthor
    {
        static readonly Dictionary<int,string> Art=new Dictionary<int,string>{
            {22,"Sky"},{6,"Balance"},{7,"GreenButton"},{42,"GreenButton"},
            {16,"Notice"},{52,"Settings"},{53,"Rules"},{21,"NextTile"},
            {32,"Bottom"},{14,"Track"},{69,"Fill"},{15,"GreenButton"},{73,"GreenButton"}
        };
        [MenuItem("Coin Merge/Reskin/Apply art only (keep layout)")]
        public static void Run()
        {
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            const string p="Assets/Prefabs/Runtime/RecoveredMain.prefab";
            var root=PrefabUtility.LoadPrefabContents(p);
            try{Apply(root);PrefabUtility.SaveAsPrefabAsset(root,p);}finally{PrefabUtility.UnloadPrefabContents(root);}
            var scene=EditorSceneManager.OpenScene("Assets/Scenes/RecoveredMain.unity");
            foreach(var candidate in scene.GetRootGameObjects())if(candidate.GetComponent<RecoveredGameSession>())Apply(candidate);
            EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();
            Debug.Log("HOME_ART_ONLY_AUTHORED: original transforms/text/buttons/components retained; only 13 sprite bindings changed per root");
        }
        static void Apply(GameObject root)
        {
            var session=root.GetComponent<RecoveredGameSession>();
            var snapshots=new Dictionary<Component,string>();
            foreach(var component in root.GetComponentsInChildren<Component>(true))
                if(component&&!(component is Image))snapshots.Add(component,EditorJsonUtility.ToJson(component));
            int count=root.GetComponentsInChildren<Component>(true).Length;
            string uuid=session.moneyText.GetComponent<RecoveredNode>().sourceUuid;int replaced=0;
            foreach(var node in root.GetComponentsInChildren<RecoveredNode>(true))
                if(node.sourceUuid==uuid&&Art.TryGetValue(node.sourceObjectId,out string name))
                {
                    var image=node.GetComponent<Image>();if(!image)throw new Exception("Missing source Image "+node.sourceObjectId);
                    var sprite=AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Resources/HomeReskin/"+name+".png");
                    if(!sprite)throw new Exception("Missing pure-art sprite "+name);
                    image.sprite=sprite;replaced++;
                }
            if(replaced!=Art.Count)throw new Exception("Incomplete source mapping: "+replaced);
            if(count!=root.GetComponentsInChildren<Component>(true).Length)throw new Exception("Component structure changed");
            foreach(var saved in snapshots)if(EditorJsonUtility.ToJson(saved.Key)!=saved.Value)
                throw new Exception("Pure-art invariant changed: "+saved.Key.name+" / "+saved.Key.GetType().Name);
        }
    }
}
