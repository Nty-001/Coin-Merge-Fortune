using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
namespace CoinMerge.Recovery.Editor
{
    // Disposable Play Mode verification through the same loading/session path as the player.
    [InitializeOnLoad] public static class ApprovedScreensReview
    {
        const string Key="ApprovedScreensReview",Prefix="coinmerge.approvedscreens.review.disposable";
        static string Output=>Path.GetFullPath("ApprovedScreensVerification");
        static RecoveredGameSession session;static Camera camera,background;static RenderTexture target,back;
        static int phase,height,chosen,completed,originalCoins;static double next,deadline,stageEnd,originalCash;
        static float peak;static readonly List<string> checks=new List<string>();
        [Serializable] sealed class Report{public bool passed;public string error,unityVersion;public string[] checks;}
        static ApprovedScreensReview(){EditorApplication.playModeStateChanged+=State;}
        public static void Run()
        {
            Directory.CreateDirectory(Output);
            var before=Geometry();ApprovedScreensAuthor.Run();var after=Geometry();
            if(before!=after)throw new InvalidOperationException("Modal or native Button geometry changed.");
            File.WriteAllText(Path.Combine(Output,"original-layout.txt"),after);
            VerifyApplied();
        }
        public static void VerifyApplied()
        {
            Directory.CreateDirectory(Output);SessionState.SetBool(Key,true);SessionState.SetString(Key+".error","");
            var store=new PlayerStore(Prefix);store.ResetPlayer();store.ResetProfile();
            store.Save(new PlayerProgress{guideStep=9999,fakeMoney=451.55,coin1024Number=50,roundScore=570,histroyMaxScore=612,open_bgm=true,open_music=true,open_vibrate=true});
            store.SaveProfile(new VersionProfile{country="US",cohort="B",rewardedVariant=true});
            EditorSceneManager.OpenScene("Assets/Scenes/RecoveredLoading.unity");UnityEngine.Object.FindObjectOfType<RecoveredStartup>().saveNamespace=Prefix;EditorApplication.EnterPlaymode();
        }
        static IEnumerable<Button> Buttons(RecoveredGameSession s)
        {
            yield return s.failView.revive;yield return s.failView.close;yield return s.failView.restart;
            yield return s.wheelView.draw;yield return s.wheelRewardView.claim;yield return s.rewardView.highestClose;
            yield return s.rating.close;yield return s.rating.confirm;foreach(var b in s.rating.stars)yield return b;
        }
        static string Geometry()
        {
            var root=PrefabUtility.LoadPrefabContents("Assets/Prefabs/Runtime/RecoveredMain.prefab");
            try
            {
                var s=root.GetComponent<RecoveredGameSession>();var lines=new List<string>();
                void Record(RectTransform t){lines.Add(t.name+" "+t.anchorMin+" "+t.anchorMax+" "+t.pivot+" "+t.anchoredPosition+" "+t.sizeDelta+" "+t.localScale+" "+t.localRotation);}
                foreach(var v in new[]{s.failView.transform,s.wheelView.transform,s.wheelRewardView.transform,s.rewardView.highestGroup.transform,s.rewardView.doubleGroup.transform,s.rating.transform})Record((RectTransform)v);
                foreach(var b in Buttons(s))if(b)Record((RectTransform)b.transform);
                return string.Join("\n",lines);
            }finally{PrefabUtility.UnloadPrefabContents(root);}
        }
        static void State(PlayModeStateChange state)
        {
            if(!SessionState.GetBool(Key,false))return;
            if(state==PlayModeStateChange.EnteredPlayMode)
            {checks.Clear();phase=completed=0;next=0;deadline=EditorApplication.timeSinceStartup+130;Application.logMessageReceived+=Log;EditorApplication.update+=Tick;}
            if(state==PlayModeStateChange.EnteredEditMode)
            {var store=new PlayerStore(Prefix);store.ResetPlayer();store.ResetProfile();SessionState.SetBool(Key,false);EditorApplication.Exit(SessionState.GetString(Key+".error","").Length==0?0:1);}
        }
        static void Log(string message,string trace,LogType type){if(type==LogType.Error||type==LogType.Exception)SessionState.SetString(Key+".error",message+"\n"+trace);}
        static void Require(bool value,string message){if(!value)throw new InvalidOperationException(message);if(!checks.Contains(message))checks.Add(message);}
        static void Completed(int index){completed++;Require(index==chosen,"Original draw result preserved");}
        static void Tick()
        {
            if(EditorApplication.timeSinceStartup<next)return;next=EditorApplication.timeSinceStartup+.4;
            try
            {
                if(EditorApplication.timeSinceStartup>deadline)throw new TimeoutException("Review timeout");
                var error=SessionState.GetString(Key+".error","");if(error.Length>0)throw new Exception(error);
                // The existing mock player intentionally waits for its normal close button after playback.
                if(session&&session.AdShowing)
                {
                    var ad=session.GetComponentInChildren<MockAdPlaybackView>(true);
                    if(ad&&ad.Ready)ad.finish.onClick.Invoke();
                }
                switch(phase)
                {
                    case 0:
                        session=UnityEngine.Object.FindObjectOfType<RecoveredGameSession>();if(!session||session.Player==null)return;
                        session.automaticInput=false;session.rewardView.enabled=false;camera=session.worldCamera;camera.GetComponent<DeviceSafeViewport>().enabled=false;background=session.transform.Find("SafeAreaBackdrop").GetComponent<Camera>();
                        Resize(1920);session.failView.Show(session.Player);phase++;break;
                    case 1:
                        Capture("01_game_over");Require(session.failView.scoreText.text==session.Player.roundScore.ToString(),"Game Over values remain dynamic");
                        session.failView.gameObject.SetActive(false);session.wheelView.Show(session.Player,session.Locale);phase++;break;
                    case 2:
                        Capture("02_lottery");session.wheelView.gameObject.SetActive(false);chosen=FindPrize(false);
                        session.wheelRewardView.Show(chosen,session.Player,session.board.Config,session.Locale,.5);phase++;break;
                    case 3:
                        Capture("03_lottery_reward");CheckCoin(session.wheelRewardView.coin.GetComponent<Image>());session.wheelRewardView.gameObject.SetActive(false);session.rewardView.Show(4,1);phase++;break;
                    case 4:Capture("04_highest_merge");session.rewardView.gameObject.SetActive(false);session.rewardView.Show(1,2.33);phase++;break;
                    case 5:Capture("05_revive_success");session.rewardView.gameObject.SetActive(false);session.rating.Show();session.rating.Select(4);phase++;break;
                    case 6:
                        Capture("06_rating");session.rating.gameObject.SetActive(false);
                        if(height==1920){Resize(2340);session.failView.Show(session.Player);phase=1;break;}
                        CheckAssetsAndButtons();session.rating.Show();session.rating.stars[1].onClick.Invoke();
                        Require(session.rating.SelectedIndex==1&&session.rating.stars[0].image.sprite==session.rating.filledStar&&session.rating.stars[2].image.sprite==session.rating.emptyStar,"Rating selection preserves filled and empty states");
                        session.rating.confirm.onClick.Invoke();Require(!session.rating.gameObject.activeSelf,"Rating native OK dismisses");
                        session.rating.Show();session.rating.close.onClick.Invoke();Require(!session.rating.gameObject.activeSelf,"Rating native close dismisses");
                        session.ShowReward(4);session.rewardView.highestClose.onClick.Invoke();Require(!session.rewardView.gameObject.activeSelf,"Highest Claim keeps original close handler");
                        session.failView.Show(session.Player);session.failView.close.onClick.Invoke();Require(!session.failView.gameObject.activeSelf&&!session.board.GameOver,"Game Over native close keeps restart handler");
                        session.failView.Show(session.Player);session.failView.revive.onClick.Invoke();stageEnd=EditorApplication.timeSinceStartup+15;phase++;break;
                    case 7:
                        if(!session.rewardView.gameObject.activeSelf){if(EditorApplication.timeSinceStartup>stageEnd)throw new TimeoutException("Revive ad callback");break;}
                        Require(session.rewardView.Kind==1&&!session.failView.gameObject.activeSelf,"Revive Button retains original mocked ad and reward branch");
                        originalCash=session.Player.fakeMoney;double amount=session.rewardView.Amount;session.rewardView.mask.onClick.Invoke();
                        Require(Math.Abs(session.Player.fakeMoney-originalCash-amount)<.000001,"Revive reward mask credits calculated amount once");
                        double paid=session.Player.fakeMoney;session.rewardView.Close();Require(session.Player.fakeMoney==paid,"Repeated reward close cannot pay twice");
                        stageEnd=EditorApplication.timeSinceStartup+5;phase++;break;
                    case 8:
                        if(session.board.Reviving){if(EditorApplication.timeSinceStartup>stageEnd)throw new TimeoutException("Revive completion");break;}
                        session.Player.gameTotalScore=0;session.wheelView.Show(session.Player,session.Locale);session.wheelView.draw.onClick.Invoke();Require(!session.wheelView.IsSpinning,"Insufficient score cannot start draw");
                        var lever=session.wheelView.GetComponentInChildren<RecoveredWheelLever>(true);var player=session.wheelView.backgroundAnimation;
                        player.EvaluateAt("idle",.6667f);lever.ApplyPose();Require(lever.PressedFraction>.99f,"Reskinned lever reaches source pressed pose");Capture("07_lever_pressed");
                        player.ResetPose("idle");lever.ApplyPose();Require(lever.PressedFraction<.001f&&Mathf.Abs(lever.knob.localScale.x-lever.knob.localScale.y)<.001f,"Lever returns and knob remains round");
                        session.wheelView.DrawCompleted+=Completed;BeginDraw(false);phase++;break;
                    case 9:
                        next=EditorApplication.timeSinceStartup+.02;peak=Mathf.Max(peak,session.wheelView.GetComponentInChildren<RecoveredWheelLever>(true).PressedFraction);
                        if(!session.wheelRewardView.gameObject.activeSelf){if(EditorApplication.timeSinceStartup>stageEnd)throw new TimeoutException("Coin draw completion");break;}
                        Require(peak>.95f&&completed==1,"Live lottery uses lever animation and completes once");
                        Require(session.wheelView.slots[chosen].selected.activeSelf,"Winning prize cell remains highlighted");
                        Capture("08_live_coin_reward");originalCoins=session.Player.coin1024Number;session.wheelRewardView.claim.onClick.Invoke();session.wheelRewardView.claim.onClick.Invoke();stageEnd=EditorApplication.timeSinceStartup+15;phase++;break;
                    case 10:
                        if(session.wheelRewardView.gameObject.activeSelf){if(EditorApplication.timeSinceStartup>stageEnd)throw new TimeoutException("Coin claim callback");break;}
                        Require(session.Player.coin1024Number-originalCoins==session.wheelRewardView.Coins,"Coin Claim credits once and keeps original reward quantity");BeginDraw(true);phase++;break;
                    case 11:
                        next=EditorApplication.timeSinceStartup+.02;
                        if(!session.wheelRewardView.gameObject.activeSelf){if(EditorApplication.timeSinceStartup>stageEnd)throw new TimeoutException("Cash draw completion");break;}
                        Require(completed==2&&session.wheelRewardView.Cash>0&&!session.wheelRewardView.coin.activeSelf,"Cash prize uses original localized cash branch");Capture("09_live_cash_reward");
                        originalCash=session.Player.fakeMoney;session.wheelRewardView.claim.onClick.Invoke();session.wheelRewardView.claim.onClick.Invoke();stageEnd=EditorApplication.timeSinceStartup+15;phase++;break;
                    case 12:
                        if(session.wheelRewardView.gameObject.activeSelf){if(EditorApplication.timeSinceStartup>stageEnd)throw new TimeoutException("Cash claim callback");break;}
                        Require(Math.Abs(session.Player.fakeMoney-originalCash-session.wheelRewardView.Cash)<.000001,"Cash Claim credits calculated amount once");Finish(null);break;
                }
            }catch(Exception e){Finish(e.ToString());}
        }
        static int FindPrize(bool money){for(int i=0;i<session.board.Config.rules.lotteryRewards.Length;i++)if((session.board.Config.rules.lotteryRewards[i].type=="money")==money)return i;throw new Exception("Missing prize type");}
        static void BeginDraw(bool money)
        {
            session.Player.gameTotalScore=RecoveredGameRules.RequiredScore(session.board.Config.rules.lotteryScores,session.Player.currentLotteryCount);
            chosen=FindPrize(money);session.GmSetNextWheel(chosen);session.wheelView.Show(session.Player,session.Locale);peak=0;
            session.wheelView.draw.onClick.Invoke();session.wheelView.draw.onClick.Invoke();Require(session.wheelView.IsSpinning,"Valid draw starts through original Button");stageEnd=EditorApplication.timeSinceStartup+20;
        }
        static void CheckCoin(Image image){Require(image.sprite&&image.sprite.texture.width==1254&&image.sprite.texture.format==TextureFormat.RGBA32,"2000 coins use original lossless 1254px sprite");}
        static void CheckAssetsAndButtons()
        {
            foreach(var name in new[]{"FailPanel","PrizePanel","RevivePanel","RatingPanel","GreenButton","MachineBody","LeverKnob","LeverShaft","PrizeTile"})
            {var sprite=Resources.Load<Sprite>("ApprovedScreens/"+name);Require(sprite&&sprite.texture.format==TextureFormat.RGBA32,"Lossless resource imported: "+name);}
            foreach(var b in Buttons(session))if(b){Require(b.onClick.GetPersistentEventCount()==0,"Native Buttons have no Inspector callbacks");Require(b.targetGraphic&&b.targetGraphic.gameObject==b.gameObject,"Native Buttons own their visible target graphic");}
            Require(session.rewardView.doubleRewardGroup&&session.rewardView.normalGroup,"Previously approved reward presentations remain configured");
        }
        static void Resize(int h)
        {
            height=h;camera.targetTexture=null;background.targetTexture=null;if(target)UnityEngine.Object.DestroyImmediate(target);if(back)UnityEngine.Object.DestroyImmediate(back);
            target=new RenderTexture(1080,h,24);back=new RenderTexture(1080,h,24);camera.targetTexture=target;background.targetTexture=back;camera.rect=new Rect(0,0,1,1);
            foreach(var scaler in camera.GetComponent<DeviceSafeViewport>().scalers){scaler.scaleFactor=1080f/750;scaler.GetComponent<Canvas>().scaleFactor=scaler.scaleFactor;}
        }
        static void Capture(string name)
        {
            Canvas.ForceUpdateCanvases();session.playfieldLayout.Refresh();background.Render();Graphics.Blit(back,target);camera.Render();var old=RenderTexture.active;RenderTexture.active=target;
            var image=new Texture2D(target.width,target.height,TextureFormat.RGB24,false);image.ReadPixels(new Rect(0,0,target.width,target.height),0,0);image.Apply();File.WriteAllBytes(Path.Combine(Output,name+"_1080x"+height+".png"),image.EncodeToPNG());RenderTexture.active=old;UnityEngine.Object.DestroyImmediate(image);
        }
        static void Finish(string error)
        {
            EditorApplication.update-=Tick;Application.logMessageReceived-=Log;if(session)session.wheelView.DrawCompleted-=Completed;
            if(camera)camera.targetTexture=null;if(background)background.targetTexture=null;if(target)UnityEngine.Object.DestroyImmediate(target);if(back)UnityEngine.Object.DestroyImmediate(back);
            SessionState.SetString(Key+".error",error??"");File.WriteAllText(Path.Combine(Output,"result.json"),JsonUtility.ToJson(new Report{passed=error==null,error=error,unityVersion=Application.unityVersion,checks=checks.ToArray()},true));EditorApplication.ExitPlaymode();
        }
    }
}
