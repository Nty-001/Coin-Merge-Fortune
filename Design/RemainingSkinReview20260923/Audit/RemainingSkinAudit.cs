using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
namespace CoinMerge.Recovery.Editor
{
    [InitializeOnLoad] public static class RemainingSkinAudit
    {
        const string Key="RemainingSkinAudit",Prefix="coinmerge.skin.audit.disposable";
        static string Output=>Path.GetFullPath("RemainingSkinAudit");
        static RecoveredGameSession s;static PackagedGameSession p;static Camera cam,bg;static RenderTexture rt,back;
        static int index;static double next,deadline;static readonly List<Entry> entries=new List<Entry>();
        [Serializable] sealed class Entry{public string name,scope;public string[] texts,resources;public string file;}
        [Serializable] sealed class Report{public string error;public Entry[] screens;}
        static RemainingSkinAudit(){EditorApplication.playModeStateChanged+=State;}
        public static void Run(){Start(false);}
        public static void RunPackaged(){Start(true);}
        static void Start(bool packaged)
        {
            Directory.CreateDirectory(Output);SessionState.SetBool(Key,true);SessionState.SetBool(Key+".packaged",packaged);SessionState.SetString(Key+".error","");
            var store=new PlayerStore(Prefix);store.ResetPlayer();store.ResetProfile();store.Save(new PlayerProgress{guideStep=9999,fakeMoney=482.32,coin1024Number=503,open_bgm=true,open_music=true,open_vibrate=true});store.SaveProfile(new VersionProfile{country="US",cohort="B",rewardedVariant=true});
            PlayerPrefs.DeleteKey(Prefix+".packaged.player");
            EditorSceneManager.OpenScene(packaged?"Assets/Scenes/RecoveredPackaged.unity":"Assets/Scenes/RecoveredMain.unity");
            if(packaged){store.SaveProfile(new VersionProfile{country="US",cohort="A",rewardedVariant=false,contentMode=1,cohortMode=1});UnityEngine.Object.FindObjectOfType<PackagedGameSession>().saveNamespace=Prefix;}else UnityEngine.Object.FindObjectOfType<RecoveredGameSession>().saveNamespace=Prefix;
            EditorApplication.EnterPlaymode();
        }
        static void State(PlayModeStateChange state)
        {
            if(!SessionState.GetBool(Key,false))return;
            if(state==PlayModeStateChange.EnteredPlayMode){index=-1;next=EditorApplication.timeSinceStartup+2;deadline=next+90;entries.Clear();Application.logMessageReceived+=Log;EditorApplication.update+=Tick;}
            if(state==PlayModeStateChange.EnteredEditMode){SessionState.SetBool(Key,false);var store=new PlayerStore(Prefix);store.ResetPlayer();store.ResetProfile();PlayerPrefs.DeleteKey(Prefix+".packaged.player");EditorApplication.Exit(SessionState.GetString(Key+".error","").Length==0?0:1);}
        }
        static void Log(string message,string trace,LogType type){if(type==LogType.Error||type==LogType.Exception)SessionState.SetString(Key+".error",message+"\n"+trace);}
        static void Tick()
        {
            if(EditorApplication.timeSinceStartup<next)return;next=EditorApplication.timeSinceStartup+.6;
            try
            {
                if(EditorApplication.timeSinceStartup>deadline)throw new TimeoutException();var error=SessionState.GetString(Key+".error","");if(error.Length>0)throw new Exception(error);
                bool packaged=SessionState.GetBool(Key+".packaged",false);
                if(index<0)
                {
                    if(packaged){p=UnityEngine.Object.FindObjectOfType<PackagedGameSession>();if(!p||p.Player==null)return;p.automaticInput=false;p.Home();cam=p.worldCamera;}
                    else{s=UnityEngine.Object.FindObjectOfType<RecoveredGameSession>();if(!s||s.Player==null)return;s.automaticInput=false;s.rewardView.enabled=false;cam=s.worldCamera;}
                    var viewport=cam.GetComponent<DeviceSafeViewport>();if(viewport)viewport.enabled=false;
                    var root=packaged?p.transform:s.transform;var b=root.Find("SafeAreaBackdrop");if(b)bg=b.GetComponent<Camera>();
                    rt=new RenderTexture(1080,1920,24);cam.targetTexture=rt;cam.rect=new Rect(0,0,1,1);
                    if(bg){back=new RenderTexture(1080,1920,24);bg.targetTexture=back;}
                    if(viewport)foreach(var scaler in viewport.scalers){scaler.scaleFactor=1.44f;scaler.GetComponent<Canvas>().scaleFactor=1.44f;}
                    index=0;return;
                }
                if(packaged)Packaged();else Rewarded();
                index++;
            }catch(Exception e){Finish(e.ToString());}
        }
        static void Rewarded()
        {
            var m=s.menus;m.CloseAll();s.rewardView.gameObject.SetActive(false);s.failView.gameObject.SetActive(false);s.rating.gameObject.SetActive(false);s.wheelView.gameObject.SetActive(false);s.wheelRewardView.gameObject.SetActive(false);s.guideView.gameObject.SetActive(false);
            if(index==0){Capture("B00_home",s.gameObject);return;}
            if(index<=11)
            {
                int page=index-1;
                if(page>=5&&page<=7){s.ChangeProfile(page==6?"BR":page==7?"ID":"US","B",true);s.Player.raccountName="";s.Player.rfullName="";s.Player.rdocumentId="";m.Show(4);m.Act(10);if(m.CurrentPage!=page)m.Show(page);}
                else{ s.ChangeProfile("US","B",true);m.Show(page);}
                if(page==2){m.privacyScroll.SetActive(true);m.termsScroll.SetActive(false);m.policyTitle.text=s.Locale.Label("7");}
                if(page==8){m.verification.enabled=false;m.verifyAmount.text="$500";m.verifyCommission.text="Fee: 0";m.verifyCredited.text="Received: $500";foreach(var t in m.verification.tips)t.localScale=Vector3.one;foreach(var o in m.verification.tipOpacity)o.alpha=1;}
                if(page==9){m.stageAmount.text="$500";m.stageHint.text="Still 15 days left and merge 5 coins every day";m.stagePercent.text="0%";m.stageFill.fillAmount=0;}
                Capture("B"+index.ToString("00")+"_"+new[]{"settings","rules","privacy","balance","coin","email","brazil","phone","verification","next_stage","active"}[page],m.pages[page]);return;
            }
            if(index==12){m.Show(2);m.privacyScroll.SetActive(false);m.termsScroll.SetActive(true);m.policyTitle.text=s.Locale.Label("6");Capture("B12_terms",m.pages[2]);return;}
            if(index<=17){int kind=index-12;s.rewardView.Show(kind,.54);Capture("B"+index+"_reward_"+kind,s.rewardView.gameObject);return;}
            if(index==18){s.failView.Show(s.Player);Capture("B18_fail",s.failView.gameObject);return;}
            if(index==19){s.wheelView.Show(s.Player,s.Locale);Capture("B19_lottery",s.wheelView.gameObject);return;}
            if(index==20){s.wheelRewardView.Show(0,s.Player,s.board.Config,s.Locale,.5);Capture("B20_lottery_reward",s.wheelRewardView.gameObject);return;}
            if(index==21){s.rating.Show();Capture("B21_rating",s.rating.gameObject);return;}
            if(index<=26){s.guideView.Show(index-22,s.Player.fakeMoney);Capture("B"+index+"_tutorial_"+(index-22),s.guideView.gameObject);return;}
            if(index==27){m.ShowToast("Not enough points to spin");Capture("B27_toast",m.toast);return;}
            Finish(null);
        }
        static void Packaged()
        {
            while(p.Dialog>=0)p.Close();
            if(index==0){p.Home();Capture("A00_home",p.gameObject);return;}
            if(index<=7){p.Show(index-1);if(index==7){p.Act(12);}Capture("A0"+index+"_"+new[]{"guide","settings","honor","info","heart","failure","privacy"}[index-1],p.dialogs[index-1]);return;}
            if(index==8){p.Act(13);Capture("A08_terms",p.dialogs[6]);return;}
            if(index==9){p.ChooseMode(0);Capture("A09_gameplay",p.gameObject);return;}
            Finish(null);
        }
        static void Capture(string name,GameObject scope)
        {
            Canvas.ForceUpdateCanvases();foreach(var popup in UnityEngine.Object.FindObjectsOfType<RecoveredMenuPopup>())popup.Tick(1);Canvas.ForceUpdateCanvases();if(s)s.playfieldLayout.Refresh();
            if(bg){bg.Render();Graphics.Blit(back,rt);}cam.Render();var old=RenderTexture.active;RenderTexture.active=rt;var image=new Texture2D(1080,1920,TextureFormat.RGB24,false);image.ReadPixels(new Rect(0,0,1080,1920),0,0);image.Apply();File.WriteAllBytes(Path.Combine(Output,name+".png"),image.EncodeToPNG());RenderTexture.active=old;UnityEngine.Object.DestroyImmediate(image);
            var texts=new List<string>();foreach(var t in scope.GetComponentsInChildren<Text>())if(t.enabled&&!string.IsNullOrEmpty(t.text))texts.Add(t.text);
            var art=new HashSet<string>();foreach(var loader in UnityEngine.Object.FindObjectsOfType<RecoveredMenuArt>(true))if(loader.images!=null)foreach(var b in loader.images)if(b.image&&b.image.enabled&&b.image.gameObject.activeInHierarchy&&b.image.transform.IsChildOf(scope.transform))art.Add(b.resourcePath);
            entries.Add(new Entry{name=name,scope=scope.name,texts=texts.ToArray(),resources=new List<string>(art).ToArray(),file=name+".png"});
        }
        static void Finish(string error)
        {
            EditorApplication.update-=Tick;Application.logMessageReceived-=Log;SessionState.SetString(Key+".error",error??"");File.WriteAllText(Path.Combine(Output,SessionState.GetBool(Key+".packaged",false)?"packaged.json":"rewarded.json"),JsonUtility.ToJson(new Report{error=error,screens=entries.ToArray()},true));EditorApplication.ExitPlaymode();
        }
    }
}
