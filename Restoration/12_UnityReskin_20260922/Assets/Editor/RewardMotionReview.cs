using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
namespace CoinMerge.Recovery.Editor
{
    [InitializeOnLoad] public static class RewardMotionReview
    {
        const string Key="RewardMotionReview",Prefix="coinmerge.rewardmotion.disposable";
        static string Output=>Path.GetFullPath("RewardMotionVerification");
        static RecoveredGameSession session;static Camera camera,background;static RenderTexture target,back;
        static int phase,height;static double next,deadline;static List<string> checks=new List<string>();
        [Serializable] sealed class Report{public bool passed;public string error;public string[] checks;}
        static RewardMotionReview(){EditorApplication.playModeStateChanged+=State;}
        public static void Run(){Directory.CreateDirectory(Output);RewardMotionAuthor.Run();VerifyApplied();}
        public static void VerifyApplied()
        {
            Directory.CreateDirectory(Output);SessionState.SetBool(Key,true);SessionState.SetString(Key+".error","");
            var store=new PlayerStore(Prefix);store.ResetPlayer();store.ResetProfile();
            store.Save(new PlayerProgress{guideStep=9999,fakeMoney=415.97,coin1024Number=5,open_bgm=true,open_music=true,open_vibrate=true});
            store.SaveProfile(new VersionProfile{country="US",cohort="B",rewardedVariant=true});
            EditorSceneManager.OpenScene("Assets/Scenes/RecoveredLoading.unity");UnityEngine.Object.FindObjectOfType<RecoveredStartup>().saveNamespace=Prefix;EditorApplication.EnterPlaymode();
        }
        static void State(PlayModeStateChange s)
        {
            if(!SessionState.GetBool(Key,false))return;
            if(s==PlayModeStateChange.EnteredPlayMode){phase=0;next=0;checks.Clear();deadline=EditorApplication.timeSinceStartup+100;Application.logMessageReceived+=Log;EditorApplication.update+=Tick;}
            if(s==PlayModeStateChange.EnteredEditMode){var store=new PlayerStore(Prefix);store.ResetPlayer();store.ResetProfile();SessionState.SetBool(Key,false);EditorApplication.Exit(SessionState.GetString(Key+".error","").Length==0?0:1);}
        }
        static void Log(string m,string trace,LogType t){if(t==LogType.Error||t==LogType.Exception)SessionState.SetString(Key+".error",m+"\n"+trace);}
        static void Require(bool b,string message){if(!b)throw new Exception(message);if(!checks.Contains(message))checks.Add(message);}
        static void Tick()
        {
            if(EditorApplication.timeSinceStartup<next)return;next=EditorApplication.timeSinceStartup+.7;
            try
            {
                if(EditorApplication.timeSinceStartup>deadline)throw new TimeoutException("Motion review");
                var error=SessionState.GetString(Key+".error","");if(error.Length>0)throw new Exception(error);
                switch(phase)
                {
                    case 0:
                        session=UnityEngine.Object.FindObjectOfType<RecoveredGameSession>();if(!session||session.Player==null)return;
                        session.automaticInput=false;session.rewardView.enabled=false;camera=session.worldCamera;camera.GetComponent<DeviceSafeViewport>().enabled=false;background=session.transform.Find("SafeAreaBackdrop").GetComponent<Camera>();Resize(1920);phase++;break;
                    case 1:
                        var hint=session.bubble.GetComponent<RecoveredHintFloat>();Require(hint&&hint.enabled,"Hint has native motion component");hint.enabled=false;hint.gameObject.SetActive(false);hint.gameObject.SetActive(true);hint.enabled=true;hint.enabled=false;
                        var rest=hint.restPosition;hint.Tick(1);Require(Mathf.Abs(hint.Offset-10)<.001,"Hint rises 10 in one second");Capture("hint_peak");hint.Tick(1);Require(Mathf.Abs(hint.Offset)<.001,"Hint crosses rest at two seconds");hint.Tick(1);Require(Mathf.Abs(hint.Offset+10)<.001,"Hint reaches minus 10 at three seconds");Capture("hint_low");hint.Tick(1);Require(Mathf.Abs(hint.Offset)<.001,"Hint completes original four-second loop");
                        session.Player.fakeMoney=500;session.ChangeProfile("US","B",true);Require(!session.bubble.activeSelf,"Hint stays hidden at 500");
                        session.Player.fakeMoney=415.97;session.ChangeProfile("US","B",true);Require(session.bubble.activeSelf&&session.bubbleText.text.Contains("84.02"),"Hint returns below threshold with original amount formatting");
                        session.rewardView.Show(3,4.62);phase++;break;
                    case 2:ReviewReward("normal",true);session.rewardView.Show(2,.08);phase++;break;
                    case 3:ReviewReward("double",true);session.rewardView.Show(1,2.33);phase++;break;
                    case 4:ReviewReward("revive",false);session.rewardView.Show(4,1);phase++;break;
                    case 5:ReviewReward("highest",false);session.rewardView.gameObject.SetActive(false);session.wheelRewardView.Show(1,session.Player,session.board.Config,session.Locale,.5);phase++;break;
                    case 6:
                        var effect=session.wheelRewardView.GetComponentInChildren<RecoveredRewardLight>();Require(effect&&effect.sourceGlow==session.wheelRewardView.glow,"Lottery light follows original glow rotation source");
                        session.wheelRewardView.glow.localRotation=Quaternion.identity;effect.Apply();Capture("lottery_light_0");session.wheelRewardView.glow.localRotation=Quaternion.Euler(0,0,-16.2f);effect.Apply();Capture("lottery_light_018");session.wheelRewardView.gameObject.SetActive(false);
                        if(height==1920){Resize(2340);session.rewardView.Show(3,4.62);phase=2;break;}
                        session.rewardView.Show(3,4.62);session.rewardView.Tick(1.49f);Require(session.rewardView.gameObject.activeSelf,"Ordinary reward retains original 1.5s lifetime");session.rewardView.Tick(.02f);Require(!session.rewardView.gameObject.activeSelf,"Ordinary reward still closes and settles at original time");
                        ReviewCountries();ReviewLever();Finish(null);break;
                }
            }catch(Exception e){Finish(e.ToString());}
        }
        static void ReviewReward(string name,bool ratio)
        {
            var light=session.rewardView.GetComponentInChildren<RecoveredRewardLight>();Require(light,"Visible reward has animated light: "+name);
            var frame=light.GetComponent<Image>();Require(frame.material.shader.name=="CoinMerge/UI/RecoveredRewardLight"&&frame.material.shader.isSupported,"Native UI light shader is supported");
            Require(frame.material.GetTexture("_FlatTex")==Resources.Load<Texture2D>(light.flatResource),"Clean colour plate bound: "+name);
            var strength=light.strength;light.strength=0;light.Apply();Capture(name+"_flat");light.strength=strength;
            if(ratio){Require(Mathf.Abs(frame.rectTransform.rect.width-frame.rectTransform.rect.height)<.01&&frame.preserveAspect,"Reference reward uses original square image ratio: "+name);}
            session.rewardView.Show(session.rewardView.Kind,session.rewardView.Amount);light.Apply();Capture(name+"_light_0");
            session.rewardView.Tick(.18f);light.Apply();Require(Mathf.Abs(Mathf.DeltaAngle(light.Angle,-16.2f))<.01,"Reward light uses original clockwise 90 degrees per second: "+name);Capture(name+"_light_018");
        }
        static void ReviewCountries()
        {
            foreach(var country in new[]{"AR","AU","BD","BR","CA","CO","DE","EG","FR","GB","ID","IN","JP","KR","KZ","MX","MY","NG","PH","PK","RU","TH","TR","US","VN","ZA"})
            {
                session.ChangeProfile(country,"B",true);var sprite=session.Locale.Icon(3);Require(sprite,"Country banknote available: "+country);
                foreach(var kind in new[]{1,2,3})
                {
                    session.rewardView.Show(kind,.54);var cash=session.rewardView.GetComponentInChildren<RecoveredCurrencyArtwork>();
                    Require(cash&&cash.CurrentType==3&&cash.largePile.activeInHierarchy,"Visible native cash pile: "+country+"/"+kind);
                    foreach(var note in cash.largeNotes)Require(note.sprite==sprite&&note.enabled,"Correct country banknote in every pile note: "+country+"/"+kind);
                    if(country=="JP"||country=="US"||country=="BR")Capture("cash_"+country+"_"+kind);
                }
                session.rewardView.gameObject.SetActive(false);
                for(int i=0;i<session.board.Config.rules.lotteryRewards.Length;i++)
                {
                    if(session.board.Config.rules.lotteryRewards[i].type!="money")continue;
                    session.wheelRewardView.Show(i,session.Player,session.board.Config,session.Locale,.5);
                    var cash=session.wheelRewardView.GetComponentInChildren<RecoveredCurrencyArtwork>();
                    Require(cash&&cash.single.sprite==sprite,"Lottery reward banknote follows country: "+country);break;
                }
                session.wheelRewardView.gameObject.SetActive(false);
            }
        }
        static void ReviewLever()
        {
            session.ChangeProfile("US","B",true);session.wheelView.Show(session.Player,session.Locale);
            var lever=session.wheelView.GetComponentInChildren<RecoveredWheelLever>();
            foreach(var time in new[]{0f,.6667f,1.6667f})
            {
                session.wheelView.backgroundAnimation.EvaluateAt("idle",time);lever.ApplyPose();
                Require(Vector2.Distance(lever.shaft.anchoredPosition,lever.shaftRest)<.01,"Lever pivot stays on side axle at "+time);
                var tip=lever.shaft.TransformPoint(Vector2.Scale(new Vector2(.745f,.915f)-lever.shaft.pivot,lever.shaft.rect.size));
                Require(Vector3.Distance(tip,lever.knob.position)<.01,"Lever tip stays connected to round knob at "+time);
                Require(Mathf.Abs(lever.knob.localScale.x-lever.knob.localScale.y)<.001,"Lever ball keeps round proportions");
                Capture(time==0?"lever_rest":time<1?"lever_pressed":"lever_return");
                var corners=new Vector3[4];lever.knob.GetWorldCorners(corners);
                foreach(var corner in corners)Require(camera.WorldToViewportPoint(corner).x<=1,"Whole lever knob stays inside screen at "+time);
            }
            session.wheelView.gameObject.SetActive(false);
        }
        static void Resize(int h)
        {
            height=h;camera.targetTexture=null;background.targetTexture=null;if(target)UnityEngine.Object.DestroyImmediate(target);if(back)UnityEngine.Object.DestroyImmediate(back);target=new RenderTexture(1080,h,24);back=new RenderTexture(1080,h,24);camera.targetTexture=target;background.targetTexture=back;camera.rect=new Rect(0,0,1,1);
            foreach(var scaler in camera.GetComponent<DeviceSafeViewport>().scalers){scaler.scaleFactor=1080f/750;scaler.GetComponent<Canvas>().scaleFactor=scaler.scaleFactor;}
        }
        static void Capture(string name)
        {
            Canvas.ForceUpdateCanvases();foreach(var popup in session.GetComponentsInChildren<RecoveredMenuPopup>())popup.Tick(1);Canvas.ForceUpdateCanvases();session.playfieldLayout.Refresh();background.Render();Graphics.Blit(back,target);camera.Render();var old=RenderTexture.active;RenderTexture.active=target;var image=new Texture2D(1080,height,TextureFormat.RGB24,false);image.ReadPixels(new Rect(0,0,1080,height),0,0);image.Apply();File.WriteAllBytes(Path.Combine(Output,name+"_"+height+".png"),image.EncodeToPNG());RenderTexture.active=old;UnityEngine.Object.DestroyImmediate(image);
        }
        static void Finish(string error)
        {
            EditorApplication.update-=Tick;Application.logMessageReceived-=Log;if(camera)camera.targetTexture=null;if(background)background.targetTexture=null;if(target)UnityEngine.Object.DestroyImmediate(target);if(back)UnityEngine.Object.DestroyImmediate(back);
            SessionState.SetString(Key+".error",error??"");File.WriteAllText(Path.Combine(Output,"result.json"),JsonUtility.ToJson(new Report{passed=error==null,error=error,checks=checks.ToArray()},true));EditorApplication.ExitPlaymode();
        }
    }
}
