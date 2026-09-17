using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace CoinMerge.Recovery.Editor
{
    public static class SimpleTestGuideAuthor
    {
        public static void Author()
        {
            const string path="Assets/Prefabs/Runtime/RecoveredMain.prefab";
            var root=PrefabUtility.LoadPrefabContents(path);
            try { Apply(root);PrefabUtility.SaveAsPrefabAsset(root,path); }
            finally { PrefabUtility.UnloadPrefabContents(root); }
            var scene=EditorSceneManager.OpenScene("Assets/Scenes/RecoveredMain.unity");
            foreach(var go in scene.GetRootGameObjects())if(go.GetComponent<RecoveredGameSession>())Apply(go);
            EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();
        }
        public static void Apply(GameObject root)
        {
            var g=root.GetComponent<RecoveredGameSession>().gm.gameplay;
            var bindings=new List<MenuActionBinding>(g.actions);
            var title=g.progress.transform.parent.Find("Title").GetComponent<Text>();
            title.rectTransform.anchorMin=title.rectTransform.anchorMax=title.rectTransform.pivot=Vector2.one*.5f;
            title.rectTransform.anchoredPosition=new Vector2(-30,495);title.rectTransform.sizeDelta=new Vector2(490,60);
            title.GetComponent<RecoveredTextFit>().maximumFontSize=30;
            Add(g,bindings,49,g.progress.transform,"切换提现路线",0,365,590);
            g.routeLabel=g.progress.transform.Find("TestAction_49/Label").GetComponent<Text>();
            g.progressStatus.rectTransform.anchoredPosition=new Vector2(0,270);
            g.progressStatus.rectTransform.sizeDelta=new Vector2(590,125);
            Add(g,bindings,50,g.progress.transform,"今天差 1 枚（合成 4/5）",0,-425,590);
            Add(g,bindings,51,g.events.transform,"准备首次第 10 次投币",0,-365,590);
            Add(g,bindings,52,g.events.transform,"首次合成 500 → 看评分",0,-425,590);
            g.actions=bindings.ToArray();
            Rename(g,19,"2000 筹码 +1");Rename(g,20,"今天合成 5 枚");Rename(g,22,"设置筹码总数");
            Rename(g,25,"打开筹码提现");Rename(g,26,"看完广告计数 +1");
            Rename(g,30,"广告：看完了");Rename(g,31,"广告：中途取消");Rename(g,32,"广告：没有可播");Rename(g,33,"广告：播放失败");
            Rename(g,39,"合成 2000 筹码动画");Rename(g,43,"换下一个奖品");Rename(g,44,"抽奖还差 1 分");Rename(g,45,"抽奖分数 +1");
            Rename(g,34,"普通奖前 1 次");Rename(g,35,"第 16 次前 1 次");Rename(g,36,"广告奖前 1 次");
            Help(g.progress.transform,"先选路线、档位、任务，再准备差 1。准备会覆盖相关测试进度；打开提现页查看变化。");
            Help(g.events.transform,"预览弹窗会发奖，但不检查触发条件。测广告请用投币、抽奖领奖或复活。结果一直有效到再次切换。");
            foreach(var text in g.GetComponentsInChildren<Text>(true))
            {
                var fit=text.GetComponent<RecoveredTextFit>();if(!fit)continue;
                if(text==g.progressStatus||text==g.eventStatus){fit.maximumFontSize=21;fit.maximumLines=0;fit.singleLine=false;}
            }
            EditorUtility.SetDirty(g);
        }
        static void Help(Transform parent,string value)
        {
            var t=parent.Find("Help").GetComponent<Text>();t.text=value;t.rectTransform.anchoredPosition=new Vector2(0,-490);t.rectTransform.sizeDelta=new Vector2(590,65);
        }
        static void Rename(GameplayGmPanel g,int code,string label)
        {foreach(var b in g.actions)if(b.action==code)b.button.GetComponentInChildren<Text>(true).text=label;}
        static void Add(GameplayGmPanel g,List<MenuActionBinding> bindings,int code,Transform parent,string label,float x,float y,float width)
        {
            Button b=null;foreach(var item in bindings)if(item.action==code)b=item.button;
            if(!b){b=Object.Instantiate(g.actions[0].button,parent);b.name="TestAction_"+code;bindings.Add(new MenuActionBinding{action=code,button=b});}
            var rect=(RectTransform)b.transform;rect.anchoredPosition=new Vector2(x,y);rect.sizeDelta=new Vector2(width,48);
            var text=b.GetComponentInChildren<Text>(true);text.name="Label";text.text=label;text.rectTransform.sizeDelta=new Vector2(width-16,44);
        }
    }
}
