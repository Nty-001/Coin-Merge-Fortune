using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace CoinMerge.Recovery.Editor
{
    // Observe the real rendered startup frames. Never inject progress into the loading view.
    [InitializeOnLoad] public static class StartupProgressValidation
    {
        const string Key="CoinMerge.StartupProgress.Validation",Prefix="coinmerge.startup.progress.disposable";
        static RecoveredStartup startup;static readonly List<Frame> frames=new List<Frame>();static double deadline;static int lastFrame;static bool capturedZero,capturedMiddle,capturedFull;
        [Serializable] public sealed class Frame{public int frame;public float seconds,progress;public string label;}
        [Serializable] public sealed class Scenario{public string name;public bool passed;public string error;public float firstProgress,zeroVisibleSeconds,maximumJump,totalSeconds;public Frame[] frames;}
        [Serializable] public sealed class Report{public bool passed;public string unityVersion;public Scenario[] runs;}
        static string Output=>Path.GetFullPath("Design/LoadingR1/Verification");
        static StartupProgressValidation(){EditorApplication.update+=Poll;EditorApplication.playModeStateChanged+=State;}
        static void Poll()
        {
            string file=File.Exists("Temp/StartupProgressBaseline.request")?"Temp/StartupProgressBaseline.request":"Temp/StartupProgressValidate.request";
            if(!File.Exists(file)||SessionState.GetBool(Key,false)||EditorApplication.isCompiling||EditorApplication.isUpdating)return;
            if(EditorApplication.isPlaying){EditorApplication.ExitPlaymode();return;}if(EditorApplication.isPlayingOrWillChangePlaymode)return;
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);if(EditorApplication.isCompiling||EditorApplication.isUpdating)return;
            File.Delete(file);Run(file.Contains("Baseline"));
        }
        [MenuItem("Coin Merge/Reskin/Validate visible startup progress")]
        public static void Run(){Run(false);}
        static void Run(bool baseline)
        {
            Directory.CreateDirectory(Output);SessionState.SetBool(Key,true);SessionState.SetBool(Key+".baseline",baseline);SessionState.SetInt(Key+".index",0);SessionState.SetString(Key+".report",JsonUtility.ToJson(new Report{passed=true,unityVersion=Application.unityVersion,runs=Array.Empty<Scenario>()}));Begin();
        }
        static void Begin()
        {
            bool packaged=SessionState.GetInt(Key+".index",0)==2;var store=new PlayerStore(Prefix);store.ResetPlayer();store.ResetProfile();store.Save(new PlayerProgress{guideStep=5,fakeMoney=220});store.SaveProfile(new VersionProfile{country="US",cohort=packaged?"A":"B",rewardedVariant=!packaged});
            EditorSceneManager.OpenScene("Assets/Scenes/RecoveredLoading.unity");UnityEngine.Object.FindObjectOfType<RecoveredStartup>().saveNamespace=Prefix;EditorApplication.EnterPlaymode();
        }
        static void State(PlayModeStateChange state)
        {
            if(!SessionState.GetBool(Key,false))return;
            if(state==PlayModeStateChange.EnteredPlayMode)
            {
                frames.Clear();lastFrame=-1;capturedZero=capturedMiddle=capturedFull=false;startup=UnityEngine.Object.FindObjectOfType<RecoveredStartup>();deadline=EditorApplication.timeSinceStartup+40;Time.timeScale=1;EditorApplication.isPaused=false;
                Canvas.willRenderCanvases+=Rendered;EditorApplication.update+=Tick;
            }
            if(state==PlayModeStateChange.EnteredEditMode)
            {
                var store=new PlayerStore(Prefix);store.ResetPlayer();store.ResetProfile();PlayerPrefs.DeleteKey(Prefix+".packaged.player");int index=SessionState.GetInt(Key+".index",0)+1;SessionState.SetInt(Key+".index",index);
                var report=JsonUtility.FromJson<Report>(SessionState.GetString(Key+".report",""));
                if(report.passed&&!SessionState.GetBool(Key+".baseline",false)&&index<3){EditorApplication.delayCall+=Begin;return;}
                SessionState.SetBool(Key,false);EditorSceneManager.OpenScene("Assets/Scenes/RecoveredLoading.unity");
            }
        }
        static void Rendered()
        {
            if(!startup||!startup.View||!startup.View.gameObject.activeInHierarchy||!startup.View.fill.sprite||lastFrame==Time.frameCount)return;
            lastFrame=Time.frameCount;float value=startup.View.fill.fillAmount;frames.Add(new Frame{frame=lastFrame,seconds=Time.unscaledTime,progress=value,label=startup.View.percentage.text});
            string prefix=(SessionState.GetBool(Key+".baseline",false)?"Before":"After")+"_startup_"+SessionState.GetInt(Key+".index",0)+"_";
            if(!capturedZero&&value==0){ScreenCapture.CaptureScreenshot(Path.Combine(Output,prefix+"zero.png"));capturedZero=true;}
            if(!capturedMiddle&&value>=.4f&&value<.7f){ScreenCapture.CaptureScreenshot(Path.Combine(Output,prefix+"middle.png"));capturedMiddle=true;}
            if(!capturedFull&&value==1){ScreenCapture.CaptureScreenshot(Path.Combine(Output,prefix+"full.png"));capturedFull=true;}
        }
        static void Tick()
        {
            if(EditorApplication.timeSinceStartup>deadline){Finish("Real startup timed out.");return;}
            string expected=SessionState.GetInt(Key+".index",0)==2?"RecoveredPackaged":"RecoveredMain";
            if(SceneManager.GetActiveScene().name==expected&&!UnityEngine.Object.FindObjectOfType<RecoveredStartup>())Finish(null);
        }
        static void Finish(string error)
        {
            EditorApplication.update-=Tick;Canvas.willRenderCanvases-=Rendered;int index=SessionState.GetInt(Key+".index",0);
            var result=new Scenario{name=index==2?"Packaged Play":"Rewarded Play "+(index+1),error=error??"",frames=frames.ToArray()};
            if(frames.Count==0)result.error="No rendered loading frame observed.";
            else
            {
                float start=frames[0].seconds;result.firstProgress=frames[0].progress;result.totalSeconds=frames[frames.Count-1].seconds-start;float previous=0;bool positive=false;
                foreach(var frame in frames)
                {
                    if(!positive&&frame.progress>0){result.zeroVisibleSeconds=frame.seconds-start;positive=true;}
                    result.maximumJump=Mathf.Max(result.maximumJump,frame.progress-previous);if(frame.progress+.001f<previous)result.error+=" Progress regressed.";
                    if(frame.label!=Mathf.FloorToInt(frame.progress*100+.5f)+"%")result.error+=" Percentage differs from fill.";previous=frame.progress;frame.seconds-=start;
                }
                if(result.firstProgress!=0)result.error+=" First rendered progress was not zero.";
                if(result.zeroVisibleSeconds<.15f)result.error+=" Zero was visible for less than 0.15 seconds.";
                if(result.maximumJump>.06f)result.error+=" A rendered frame jumped more than 6 percentage points.";
                if(frames[frames.Count-1].progress!=1)result.error+=" No rendered 100% frame before gameplay.";
                if(!capturedMiddle)result.error+=" Startup skipped the middle progress range.";
            }
            result.passed=result.error.Length==0;var report=JsonUtility.FromJson<Report>(SessionState.GetString(Key+".report",""));var runs=new List<Scenario>(report.runs){result};report.runs=runs.ToArray();report.passed&=result.passed;
            string json=JsonUtility.ToJson(report,true);SessionState.SetString(Key+".report",json);File.WriteAllText(Path.Combine(Output,SessionState.GetBool(Key+".baseline",false)?"startup_before.json":"startup_after.json"),json);
            Debug.Log("STARTUP_PROGRESS_OBSERVED: "+result.name+" passed="+result.passed+" first="+result.firstProgress+" zero="+result.zeroVisibleSeconds+" jump="+result.maximumJump);EditorApplication.ExitPlaymode();
        }
    }
}
