using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace CoinMerge.Recovery.Editor
{
    // Asset validation only. Never invoked by runtime initialization.
    public static class RecoveredAssetValidation
    {
        [Serializable] public sealed class Entry
        {
            public string path;
            public int roots, nodes, missingScripts;
            public string error;
        }
        [Serializable] public sealed class Report
        {
            public string unityVersion;
            public string checkedAtUtc;
            public bool structurePassed;
            public bool gameplayParityVerified;
            public List<Entry> assets = new List<Entry>();
        }
        [MenuItem("Coin Merge/Validate recovered asset structure")]
        public static void Run()
        {
            var report=new Report {unityVersion=Application.unityVersion,checkedAtUtc=DateTime.UtcNow.ToString("O"),structurePassed=true};
            var prefabs=Directory.GetFiles("Assets/Prefabs","*.prefab",SearchOption.AllDirectories);
            Array.Sort(prefabs,StringComparer.Ordinal);
            foreach(var path in prefabs)
            {
                var item=new Entry {path=path.Replace('\\','/')};
                GameObject root=null;
                try {root=PrefabUtility.LoadPrefabContents(item.path);item.roots=1;Inspect(root,item);}
                catch(Exception ex) {item.error=ex.Message;}
                finally {if(root!=null)PrefabUtility.UnloadPrefabContents(root);}
                report.assets.Add(item);
                if(item.error!=null||item.missingScripts!=0)report.structurePassed=false;
            }
            foreach(var path in Directory.GetFiles("Assets/Scenes","*.unity"))
            {
                var item=new Entry {path=path.Replace('\\','/')};
                try
                {
                    var scene=EditorSceneManager.OpenScene(item.path,OpenSceneMode.Single);
                    foreach(var root in scene.GetRootGameObjects()){item.roots++;Inspect(root,item);}
                    int expectedRoots=item.path.EndsWith("MockFlow.unity",StringComparison.Ordinal)?2:1;
                    if(item.roots!=expectedRoots)item.error="Unexpected scene root count: "+item.roots;
                }
                catch(Exception ex){item.error=ex.Message;}
                report.assets.Add(item);
                if(item.error!=null||item.missingScripts!=0)report.structurePassed=false;
            }
            Directory.CreateDirectory("../07_Verification");
            File.WriteAllText("../07_Verification/unity_asset_validation.json",JsonUtility.ToJson(report,true));
            if(!report.structurePassed)throw new InvalidOperationException("Recovered assets failed Unity structure validation.");
            Debug.Log("RECOVERED_ASSET_STRUCTURE_PASSED "+report.assets.Count+" assets; gameplay parity remains separate.");
        }
        static void Inspect(GameObject root,Entry item)
        {
            foreach(var node in root.GetComponentsInChildren<Transform>(true))
            {
                item.nodes++;
                item.missingScripts+=GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(node.gameObject);
            }
        }
    }
}
