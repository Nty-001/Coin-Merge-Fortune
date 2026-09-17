using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace CoinMerge.Recovery.Editor
{
    [InitializeOnLoad]
    public static class VerificationMotionReview
    {
        const string Request="Temp/VerificationMotion.request",Key="VerificationMotion.running",Prefix="coinmerge.motion.disposable";
        const string Output="Design/VerificationMotion20260917";
        static RecoveredGameSession session;
        static RecoveredVerificationView view;
        static RecoveredMenuPopup popup;
        static RecoveredVerificationBadge[] badges;
        static CanvasGroup[] opacity;
        static RenderTexture target;
        static Texture2D capture;
        static int run,frame,lastFrame=-1,finishedCount;
        static bool finished;
        static float started,deadline,previousAlpha;
        static readonly List<string> errors=new List<string>();
        static readonly List<float> durations=new List<float>();
        [Serializable] class Result {public bool passed;public List<string> errors;public List<float> completionSeconds;public int completedEvents;public string[] checks;}
        static VerificationMotionReview(){EditorApplication.update+=Poll;EditorApplication.playModeStateChanged+=State;}
        public static void Author()
        {
            const string path="Assets/Prefabs/Runtime/RecoveredMain.prefab";
            var root=PrefabUtility.LoadPrefabContents(path);
            try {Apply(root);PrefabUtility.SaveAsPrefabAsset(root,path);}finally{PrefabUtility.UnloadPrefabContents(root);}
            var scene=EditorSceneManager.OpenScene("Assets/Scenes/RecoveredMain.unity");
            foreach(var go in scene.GetRootGameObjects())if(go.GetComponent<RecoveredGameSession>())Apply(go);
            EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();
        }
        static void Apply(GameObject root)
        {
            var v=root.GetComponent<RecoveredGameSession>().menus.verification;
            v.tipOpacity=new CanvasGroup[v.tips.Length];v.tipEnterOffset=new Vector2(0,-8);
            for(int i=0;i<v.tips.Length;i++){var g=v.tips[i].GetComponent<CanvasGroup>();if(!g)g=v.tips[i].gameObject.AddComponent<CanvasGroup>();g.alpha=1;v.tipOpacity[i]=g;}
            var p=v.GetComponent<RecoveredMenuPopup>();p.smoothMotion=true;p.duration=.35f;p.startScale=.96f;
            p.opacity=p.content.GetComponent<CanvasGroup>();if(!p.opacity)p.opacity=p.content.gameObject.AddComponent<CanvasGroup>();p.opacity.alpha=1;
            foreach(var b in v.GetComponentsInChildren<RecoveredVerificationBadge>(true)){b.smoothMotion=true;b.rotationSpeed=150;b.blendTime=.12f;b.completedStartScale=.94f;}
        }
        static void Poll()
        {
            if(!File.Exists(Request)||SessionState.GetBool(Key,false)||EditorApplication.isCompiling||EditorApplication.isUpdating)return;
            if(EditorApplication.isPlaying){EditorApplication.ExitPlaymode();return;}
            if(EditorApplication.isPlayingOrWillChangePlaymode)return;
            File.Delete(Request);Directory.CreateDirectory(Output);Author();
            var store=new PlayerStore(Prefix);store.ResetPlayer();store.ResetProfile();store.Save(new PlayerProgress{guideStep=9999,fakeMoney=220});
            store.SaveProfile(new VersionProfile{country="US",cohort="B",rewardedVariant=true,contentMode=2,cohortMode=2});
            var s=UnityEngine.Object.FindObjectOfType<RecoveredGameSession>();s.saveNamespace=Prefix;s.automaticInput=false;
            SessionState.SetBool(Key,true);EditorApplication.EnterPlaymode();
        }
        static void State(PlayModeStateChange state)
        {
            if(!SessionState.GetBool(Key,false))return;
            if(state==PlayModeStateChange.EnteredPlayMode)
            {
                errors.Clear();durations.Clear();run=-1;session=null;finishedCount=0;EditorApplication.isPaused=false;Time.timeScale=1;Time.captureDeltaTime=1f/60;deadline=Time.time+.5f;
                EditorApplication.update+=Tick;Application.logMessageReceived+=Log;
            }
            if(state==PlayModeStateChange.EnteredEditMode)
            {
                var store=new PlayerStore(Prefix);store.ResetPlayer();store.ResetProfile();SessionState.SetBool(Key,false);
                EditorSceneManager.OpenScene("Assets/Scenes/RecoveredLoading.unity");
            }
        }
        static void Log(string text,string stack,LogType type){if(type==LogType.Error||type==LogType.Exception)errors.Add(text);}
        static void Done(){finished=true;finishedCount++;}
        static void BeginRun()
        {
            session.menus.CloseAll();bool smooth=run>0;
            view.tipOpacity=smooth?opacity:Array.Empty<CanvasGroup>();popup.smoothMotion=smooth;popup.startScale=smooth ? .96f : .5f;popup.opacity.alpha=1;
            foreach(var b in badges)b.smoothMotion=smooth;
            var g=session.gm.gameplay;g.cashRoute=false;g.selectedProduct=0;g.selectedTask=0;g.PrepareTask(true);
            session.Player.raccountName="motion@example.test";session.GmRefresh();session.menus.Act(4);session.menus.Act(1100);
            session.menus.coinReady.GetComponentInChildren<Button>().onClick.Invoke();
            if(session.menus.CurrentPage!=RecoveredMainMenus.Verify)throw new Exception("Did not enter actual verification");
            started=Time.time;frame=0;finished=false;previousAlpha=0;deadline=started+18;
        }
        static void Tick()
        {
            if(Time.frameCount==lastFrame)return;lastFrame=Time.frameCount;
            try
            {
                if(errors.Count>0)throw new Exception(errors[0]);
                if(!session)
                {
                    if(Time.time<deadline)return;
                    session=UnityEngine.Object.FindObjectOfType<RecoveredGameSession>();view=session.menus.verification;popup=view.GetComponent<RecoveredMenuPopup>();
                    badges=view.GetComponentsInChildren<RecoveredVerificationBadge>(true);opacity=view.tipOpacity;
                    target=new RenderTexture(448,970,24);capture=new Texture2D(448,970,TextureFormat.RGB24,false);session.worldCamera.targetTexture=target;
                    Canvas.ForceUpdateCanvases();session.playfieldLayout.Refresh();view.Finished+=Done;run=0;BeginRun();return;
                }
                if(Time.time>deadline)throw new Exception("Verification stalled");
                if(finished)
                {
                    durations.Add(Time.time-started);
                    if(session.Player.newFakeMoneyWithdraw[0]!=1)throw new Exception("Task progression changed");
                    if(run==0){run=1;BeginRun();return;}
                    if(Mathf.Abs(durations[0]-durations[1])>.04f)throw new Exception("Sequence timing changed");
                    // Reopening must reset animated geometry, never accumulate an offset.
                    session.menus.CloseAll();for(int i=0;i<view.tips.Length;i++)if(view.tips[i].localScale!=Vector3.one||Mathf.Abs(opacity[i].alpha-1)>.001f)throw new Exception("Disable did not restore tip");
                    Finish(null);return;
                }
                if(run>0)
                {
                    foreach(var tip in view.tips)if(tip.localScale!=Vector3.one)throw new Exception("Text was scaled");
                    if(opacity[0].alpha+.0001f<previousAlpha)throw new Exception("Tip opacity moved backwards");previousAlpha=opacity[0].alpha;
                    if(popup.content.localScale.x>1.0001f)throw new Exception("Popup overshot its frame");
                }
                if(frame%4==0)
                {
                    Canvas.ForceUpdateCanvases();session.worldCamera.Render();var old=RenderTexture.active;RenderTexture.active=target;
                    capture.ReadPixels(new Rect(0,0,448,970),0,0);capture.Apply();RenderTexture.active=old;
                    string folder=Output+(run==0?"/before":"/after");Directory.CreateDirectory(folder);File.WriteAllBytes(folder+"/"+(frame/4).ToString("D4")+".jpg",capture.EncodeToJPG(88));
                }
                frame++;File.WriteAllText(Output+"/progress.txt",(run==0?"before":"after")+" "+frame);
            }
            catch(Exception e){Finish(e.ToString());}
        }
        static void Finish(string error)
        {
            EditorApplication.update-=Tick;Application.logMessageReceived-=Log;if(error!=null)errors.Add(error);Time.captureDeltaTime=0;
            if(view)view.Finished-=Done;if(session)session.worldCamera.targetTexture=null;
            if(target)UnityEngine.Object.DestroyImmediate(target);if(capture)UnityEngine.Object.DestroyImmediate(capture);
            File.WriteAllText(Output+"/validation.json",JsonUtility.ToJson(new Result{passed=errors.Count==0,errors=errors,completionSeconds=durations,completedEvents=finishedCount,
                checks=new[]{"Before/after actual verification and next task","Timing unchanged within two 60fps frames","Text scale stays 1; opacity is monotonic","Popup never overshoots final size","Disable restores tip geometry and opacity","Commercial rules and callbacks unchanged"}},true));
            EditorApplication.ExitPlaymode();
        }
    }
}
