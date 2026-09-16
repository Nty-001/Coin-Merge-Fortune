using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
namespace CoinMerge.Recovery.Editor
{
    public static class FailDialogBindingRepair
    {
        // Explicit asset migration. Original FailDialog refs: aliveBtn=7, btnclose=14,
        // reAliveLabel component=36 (node 13). Node 37 is the visible close graphic.
        public static void Run()
        {
            const string path="Assets/Prefabs/Runtime/RecoveredMain.prefab";
            var root=PrefabUtility.LoadPrefabContents(path);
            try{Configure(root.GetComponent<RecoveredGameSession>());PrefabUtility.SaveAsPrefabAsset(root,path);}
            finally{PrefabUtility.UnloadPrefabContents(root);}
            var scene=EditorSceneManager.OpenScene("Assets/Scenes/RecoveredMain.unity");
            foreach(var item in scene.GetRootGameObjects())if(item.TryGetComponent<RecoveredGameSession>(out var session))Configure(session);
            EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();Debug.Log("FAIL_DIALOG_BINDINGS_REPAIRED");
        }
        static void Configure(RecoveredGameSession session)
        {
            var view=session.failView;var nodes=NativeGameplayBuilder.Nodes(view.gameObject);
            foreach(var graphic in view.GetComponentsInChildren<Graphic>(true))graphic.raycastTarget=false;
            view.revive=Bind(nodes[7]);view.close=Bind(nodes[37]);
            var labels=new List<LocalizedLabelBinding>(session.localizedLabels);
            int[] ids={13,12,22,26,28};string[] keys={"33","29","30","31","32"};
            var locale=new RecoveredLocalization("US");
            for(int i=0;i<ids.Length;i++)
            {
                var label=nodes[ids[i]].GetComponent<Text>();labels.RemoveAll(b=>b.label==label);
                labels.Add(new LocalizedLabelBinding {label=label,key=keys[i]});label.text=locale.Label(keys[i]);
            }
            session.localizedLabels=labels.ToArray();
        }
        static Button Bind(GameObject node)
        {
            var graphic=node.GetComponent<Graphic>();graphic.raycastTarget=true;
            var button=node.GetComponent<Button>()??node.AddComponent<Button>();button.targetGraphic=graphic;button.transition=Selectable.Transition.None;return button;
        }
    }
}
