using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
namespace CoinMerge.Recovery.Editor
{
    [InitializeOnLoad] public static class ApprovedSevenReview
    {
        const string Key="ApprovedSevenReview",Prefix="coinmerge.skin.audit.disposable";
        static string Output=>Path.GetFullPath("ApprovedSevenReview");
        static RecoveredGameSession s;static Camera cam,bg;static RenderTexture rt,back;
        static int index;static double next,deadline;static readonly List<Entry> entries=new List<Entry>();
        [Serializable] sealed class Entry{public string name,scope;public string[] texts,resources;public string file;}
        [Serializable] sealed class Report{public string error;public Entry[] screens;}
        static ApprovedSevenReview(){EditorApplication.playModeStateChanged+=State;}
        public static void Run(){RemainingApprovedAuthor.Run();Start();}
        static void Start()
        {
            Directory.CreateDirectory(Output);SessionState.SetBool(Key,true);SessionState.SetString(Key+".error","");
            var store=new PlayerStore(Prefix);store.ResetPlayer();store.ResetProfile();store.Save(new PlayerProgress{guideStep=9999,fakeMoney=482.32,coin1024Number=503,open_bgm=true,open_music=true,open_vibrate=true});store.SaveProfile(new VersionProfile{country="US",cohort="B",rewardedVariant=true});
            
            EditorSceneManager.OpenScene("Assets/Scenes/RecoveredMain.unity");
            UnityEngine.Object.FindObjectOfType<RecoveredGameSession>().saveNamespace=Prefix;
            EditorApplication.EnterPlaymode();
        }
        static void State(PlayModeStateChange state)
        {
            if(!SessionState.GetBool(Key,false))return;
            if(state==PlayModeStateChange.EnteredPlayMode){index=-1;next=EditorApplication.timeSinceStartup+2;deadline=next+90;entries.Clear();Application.logMessageReceived+=Log;EditorApplication.update+=Tick;}
            if(state==PlayModeStateChange.EnteredEditMode){SessionState.SetBool(Key,false);var store=new PlayerStore(Prefix);store.ResetPlayer();store.ResetProfile();EditorApplication.Exit(SessionState.GetString(Key+".error","").Length==0?0:1);}
        }
        static void Log(string message,string trace,LogType type){if(type==LogType.Error||type==LogType.Exception)SessionState.SetString(Key+".error",message+"\n"+trace);}
        static void Tick()
        {
            if(EditorApplication.timeSinceStartup<next)return;next=EditorApplication.timeSinceStartup+.6;
            try
            {
                if(EditorApplication.timeSinceStartup>deadline)throw new TimeoutException();var error=SessionState.GetString(Key+".error","");if(error.Length>0)throw new Exception(error);

                if(index<0)
                {
                    s=UnityEngine.Object.FindObjectOfType<RecoveredGameSession>();if(!s||s.Player==null)return;s.automaticInput=false;s.rewardView.enabled=false;cam=s.worldCamera;
                    var viewport=cam.GetComponent<DeviceSafeViewport>();if(viewport)viewport.enabled=false;
                    var root=s.transform;var b=root.Find("SafeAreaBackdrop");if(b)bg=b.GetComponent<Camera>();
                    rt=new RenderTexture(1080,1920,24);cam.targetTexture=rt;cam.rect=new Rect(0,0,1,1);
                    if(bg){back=new RenderTexture(1080,1920,24);bg.targetTexture=back;}
                    if(viewport)foreach(var scaler in viewport.scalers){scaler.scaleFactor=1.44f;scaler.GetComponent<Canvas>().scaleFactor=1.44f;}
                    index=0;return;
                }
                Rewarded();
                index++;
            }catch(Exception e){Finish(e.ToString());}
        }
        static void Rewarded()
        {
            var m=s.menus;
            if(index>=12){if(index==13||index==16)Capture("Flow"+index,m.pages[m.CurrentPage]);if(index<26)return;if(m.CurrentPage!=9)throw new Exception("Withdrawal verification did not reach next stage: "+m.CurrentPage);Capture("Flow_complete",m.pages[9]);Finish(null);return;}
            m.CloseAll();s.rewardView.gameObject.SetActive(false);s.failView.gameObject.SetActive(false);s.rating.gameObject.SetActive(false);s.wheelView.gameObject.SetActive(false);s.wheelRewardView.gameObject.SetActive(false);s.guideView.gameObject.SetActive(false);
            if(index==0){m.Act(7);Capture("01_privacy",m.pages[2]);return;}
            if(index==1){m.Act(8);if(m.policyPlate.sprite.name!="TermsPanel")throw new Exception("Terms plate mismatch");Capture("02_terms",m.pages[2]);return;}
            if(index==2){m.Show(8);m.verification.enabled=false;m.verifyAmount.text="$500";m.verifyCommission.text="Fee: 0";m.verifyCredited.text="Received: $500";m.verification.continueGroup.SetActive(true);foreach(var t in m.verification.tips)t.localScale=Vector3.one;foreach(var o in m.verification.tipOpacity)o.alpha=1;int i=0;foreach(var p in new[]{m.verification.first,m.verification.second,m.verification.stamp}){p.transform.parent.gameObject.SetActive(true);var b=p.GetComponent<RecoveredVerificationBadge>();b.enabled=false;b.waiting.color=new Color(1,1,1,i==0?0:1);b.complete.color=new Color(1,1,1,i==0?1:0);i++;}Capture("03_withdraw_request",m.pages[8]);return;}
            if(index==3||index==9){if(index==9){rt.Release();rt.height=2340;rt.Create();if(back){back.Release();back.height=2340;back.Create();}}m.Show(9);m.stageAmount.text="$500";m.stageHint.text="Still 15 days left and merge 5 coins every day";m.stagePercent.text="0%";m.stageFill.fillAmount=0;Capture(index==3?"04_next_stage":"Tall_next_stage",m.pages[9]);return;}
            if(index==4||index==10||index==11){s.ChangeProfile("US","B",true);s.Player.raccountName="";m.Show(4);m.Act(10);if(m.CurrentPage!=5)m.Show(5);m.forms[0].account.text="";if(index==4){if(m.forms[0].confirm.sprite.name!="DisabledButton")throw new Exception("Empty email visual");Capture("05_email",m.pages[5]);}else if(index==10){m.forms[0].account.text="review@example.com";if(m.forms[0].confirm.sprite.name!="GreenButton")throw new Exception("Valid email visual");Capture("Email_enabled",m.pages[5]);}else{m.forms[0].account.text="review@example.com";m.verification.enabled=true;foreach(var p in new[]{m.verification.first,m.verification.second,m.verification.stamp})p.GetComponent<RecoveredVerificationBadge>().enabled=true;m.Act(11);if(m.CurrentPage!=8)throw new Exception("Valid email did not start verification");}return;}
            if(index==5){m.Show(10);Capture("08_limit",m.pages[10]);return;}
            if(index==6||index==7){if(index==7)s.ChangeProfile("JP","B",true);s.rewardView.Show(5,.54);if(index==7){var artwork=s.rewardView.guideGroup.GetComponentInChildren<RecoveredCurrencyArtwork>();if(!artwork||artwork.largeNotes[0].sprite.name.Contains("USD"))throw new Exception("Country cash missing");}Capture(index==6?"10_newbie":"Newbie_JP",s.rewardView.gameObject);var glow=s.rewardView.glows[0];foreach(var g in s.rewardView.glows)if(g.gameObject.activeInHierarchy){float before=g.localEulerAngles.z;s.rewardView.Tick(.2f);if(Mathf.Approximately(before,g.localEulerAngles.z))throw new Exception("Reward glow stopped");}return;}
            if(index==8){s.ChangeProfile("US","B",true);m.Act(7);var scroll=m.privacyScroll.GetComponent<ScrollRect>();Canvas.ForceUpdateCanvases();scroll.verticalNormalizedPosition=0;Capture("Privacy_scrolled",m.pages[2]);return;}
            return;
}
        static void Capture(string name,GameObject scope)
        {
            Canvas.ForceUpdateCanvases();foreach(var popup in UnityEngine.Object.FindObjectsOfType<RecoveredMenuPopup>())popup.Tick(1);Canvas.ForceUpdateCanvases();if(s)s.playfieldLayout.Refresh();
            if(bg){bg.Render();Graphics.Blit(back,rt);}cam.Render();var old=RenderTexture.active;RenderTexture.active=rt;var image=new Texture2D(rt.width,rt.height,TextureFormat.RGB24,false);image.ReadPixels(new Rect(0,0,rt.width,rt.height),0,0);image.Apply();File.WriteAllBytes(Path.Combine(Output,name+".png"),image.EncodeToPNG());RenderTexture.active=old;UnityEngine.Object.DestroyImmediate(image);
            var texts=new List<string>();foreach(var t in scope.GetComponentsInChildren<Text>())if(t.enabled&&!string.IsNullOrEmpty(t.text))texts.Add(t.text);
            var art=new HashSet<string>();foreach(var loader in UnityEngine.Object.FindObjectsOfType<RecoveredMenuArt>(true))if(loader.images!=null)foreach(var b in loader.images)if(b.image&&b.image.enabled&&b.image.gameObject.activeInHierarchy&&b.image.transform.IsChildOf(scope.transform))art.Add(b.resourcePath);
            entries.Add(new Entry{name=name,scope=scope.name,texts=texts.ToArray(),resources=new List<string>(art).ToArray(),file=name+".png"});
        }
        static void Finish(string error)
        {
            EditorApplication.update-=Tick;Application.logMessageReceived-=Log;SessionState.SetString(Key+".error",error??"");File.WriteAllText(Path.Combine(Output,"rewarded.json"),JsonUtility.ToJson(new Report{error=error,screens=entries.ToArray()},true));EditorApplication.ExitPlaymode();
        }
    }
}
