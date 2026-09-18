using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CoinMerge.Recovery.Editor
{
    [InitializeOnLoad]
    public static class SimpleTestGuideValidation
    {
        const string Request="Temp/SimpleTestGuide.request",Key="SimpleTestGuide.running",Prefix="coinmerge.simpleguide.disposable";
        const string Output="Design/SimpleTestGuide20260917";
        static RecoveredGameSession s;static GameplayGmPanel g;static RenderTexture target;
        static IEnumerator<float> routine;static double next;static int boundaries,clicks;
        static readonly List<string> checks=new List<string>(),errors=new List<string>();
        [Serializable] class Report {public bool passed;public int preparationCases,nativeButtonClicks;public string[] checks,errors;public string unityVersion;}
        [Serializable] class ConfigExport {public RecoveredRulesData rules;public RecoveredWheelTimings wheel;public float rewardAutoClose,ratingDelay;}
        static SimpleTestGuideValidation(){EditorApplication.update+=Poll;EditorApplication.playModeStateChanged+=State;}
        static void Poll()
        {
            if(!File.Exists(Request)||SessionState.GetBool(Key,false)||EditorApplication.isCompiling||EditorApplication.isUpdating)return;
            if(EditorApplication.isPlaying){EditorApplication.ExitPlaymode();return;}
            if(EditorApplication.isPlayingOrWillChangePlaymode)return;
            SessionState.SetBool(Key+".layout",File.ReadAllText(Request).Trim()=="layout");File.Delete(Request);Directory.CreateDirectory(Output);
            try
            {
                SimpleTestGuideAuthor.Author();
                var store=new PlayerStore(Prefix);store.ResetPlayer();store.ResetProfile();
                store.Save(new PlayerProgress{guideStep=9999,fakeMoney=220});store.SaveProfile(new VersionProfile{country="US",cohort="B",rewardedVariant=true,contentMode=2,cohortMode=2});
                var session=UnityEngine.Object.FindObjectOfType<RecoveredGameSession>();session.saveNamespace=Prefix;session.automaticInput=false;
                SessionState.SetBool(Key,true);EditorApplication.EnterPlaymode();
            }
            catch(Exception e){File.WriteAllText(Output+"/author-error.txt",e.ToString());Debug.LogException(e);}
        }
        static void State(PlayModeStateChange state)
        {
            if(!SessionState.GetBool(Key,false))return;
            if(state==PlayModeStateChange.EnteredPlayMode){checks.Clear();errors.Clear();boundaries=clicks=0;s=null;next=EditorApplication.timeSinceStartup+.5;routine=Run();EditorApplication.update+=Tick;Application.logMessageReceived+=Log;}
            if(state==PlayModeStateChange.EnteredEditMode){var store=new PlayerStore(Prefix);store.ResetPlayer();store.ResetProfile();SessionState.SetBool(Key,false);EditorSceneManager.OpenScene("Assets/Scenes/RecoveredLoading.unity");}
        }
        static void Log(string text,string stack,LogType type){if(type==LogType.Error||type==LogType.Exception)errors.Add(text);}
        static void Tick()
        {
            if(EditorApplication.timeSinceStartup<next)return;
            try{if(errors.Count>0)throw new Exception(errors[0]);if(!routine.MoveNext()){Finish(null);return;}next=EditorApplication.timeSinceStartup+routine.Current;}
            catch(Exception e){Finish(e.ToString());}
        }
        static void Assert(bool condition,string label){if(!condition)throw new Exception(label);}
        static void Gm(int code){foreach(var b in g.actions)if(b.action==code){Click(b.button);return;}throw new Exception("Missing GM action "+code);}
        static void Click(Button b)
        {
            Render();Assert(b&&b.IsInteractable()&&b.gameObject.activeInHierarchy,"Button inactive");
            var rect=b.targetGraphic.rectTransform;var point=RectTransformUtility.WorldToScreenPoint(s.worldCamera,rect.TransformPoint(rect.rect.center));
            var pointer=new PointerEventData(EventSystem.current){button=PointerEventData.InputButton.Left,position=point};var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(pointer,hits);
            Assert(hits.Count>0&&ExecuteEvents.GetEventHandler<IPointerClickHandler>(hits[0].gameObject)==b.gameObject,"Button blocked: "+b.name);
            ExecuteEvents.Execute(b.gameObject,pointer,ExecuteEvents.pointerClickHandler);clicks++;
        }
        static void Clean()
        {
            s.gm.Hide();s.menus.CloseAll();s.rewardView.gameObject.SetActive(false);s.wheelView.gameObject.SetActive(false);s.wheelRewardView.gameObject.SetActive(false);s.failView.gameObject.SetActive(false);s.rating.Close();s.guideView.gameObject.SetActive(false);
            s.Player.guideStep=9999;s.Player.gameTotalScore=0;s.Player.currentLotteryCount=0;s.board.InputBlocked=false;s.GmRefresh();
        }
        static void OpenGm(int tab){s.gm.Show();Gm(tab);}
        static IEnumerator<float> Run()
        {
            s=UnityEngine.Object.FindObjectOfType<RecoveredGameSession>();g=s.gm.gameplay;s.automaticInput=false;Time.timeScale=1;EditorApplication.isPaused=false;
            target=new RenderTexture(941,1672,24);s.worldCamera.targetTexture=target;Canvas.ForceUpdateCanvases();s.playfieldLayout.Refresh();yield return .4f;
            if(SessionState.GetBool(Key+".layout",false))
            {
                Clean();OpenGm(0);Capture("gm-version.png");g.cashRoute=false;g.selectedProduct=0;g.selectedTask=0;Gm(1);Gm(14);Capture("gm-withdraw.png");
                g.selectedTask=3;Gm(14);Capture("time-before.png");Gm(16);Capture("time-12h.png");Gm(16);Gm(50);Capture("time-4of5.png");Gm(19);Capture("time-complete.png");
                Gm(2);g.selectedSlot=7;Gm(44);Capture("gm-events.png");checks.Add("GM title and controls separately authored; new buttons receive native pointer events");yield break;
            }
            File.WriteAllText(Output+"/rules.json",JsonUtility.ToJson(new ConfigExport{rules=s.board.Config.rules,wheel=s.board.Config.wheel,rewardAutoClose=s.lifecycle.config.rewardAutoClose,ratingDelay=s.lifecycle.config.ratingDelay},true));
            Clean();Capture("home.png");OpenGm(0);Capture("gm-version.png");
            // Every regional tier and both route orders, independently prepared just below / at completion.
            foreach(var country in s.board.Config.rules.supportedCountries)
            {
                s.ChangeProfile(country,"B",true);
                foreach(bool cash in new[]{false,true})for(int tier=0;tier<6;tier++)for(int step=0;step<(cash?4:5);step++)foreach(bool ready in new[]{false,true})
                {
                    g.cashRoute=cash;g.selectedProduct=tier;g.selectedTask=step;g.PrepareTask(ready);
                    var group=RecoveredGameRules.CashConfiguration(s.board.Config.rules,country);var p=cash?group.real_products[tier]:group.new_Fake_products[tier];
                    if(!cash){Assert(RecoveredWithdrawalRules.ConditionMet(s.Player,p,step)==ready,"Coin preparation "+country+"/"+tier+"/"+step);Assert(s.Player.newFakeMoneyWithdraw[tier]==step,"Preparation advanced task");}
                    else {int stage=RecoveredGameRules.FakeWithdrawConditionStage(s.Player,p);Assert(stage==(ready?Math.Min(3,step+1):step),"Cash preparation "+country+"/"+tier+"/"+step);Assert((s.Player.watch_video_count>=p.condition_video)==(ready&&step==3),"Cash final count");}
                    boundaries++;
                }
            }
            checks.Add("26 countries x 6 tiers x (5 coin + 4 cash) x below/at target: "+boundaries+" cases");
            s.ChangeProfile("US","B",true);Clean();g.cashRoute=false;g.selectedProduct=0;g.selectedTask=0;
            OpenGm(1);Gm(14);yield return .2f;Capture("gm-withdraw.png");Gm(49);Assert(g.cashRoute,"Route selector");Gm(14);Gm(24);yield return .2f;Capture("cash-gap.png");Assert(s.menus.SelectedCash==0,"Cash selected tier");
            // Cash route: finishing each condition immediately reveals the next unmet one.
            for(int step=0;step<4;step++)
            {
                Clean();g.cashRoute=true;g.selectedTask=step;OpenGm(1);Gm(14);Gm(24);yield return .2f;Capture("cash-task"+(step+1)+".png");
                Assert(s.menus.fakeRows[0].progress<1,"Cash gap progress");s.gm.Show();g.Execute(1);Gm(15);Gm(24);yield return .1f;
                if(step==3){s.menus.Act(9);Assert(s.menus.CurrentPage==RecoveredMainMenus.Active,"Cash final state");Capture("cash-final.png");}
            }
            checks.Add("Cash route: all 4 conditions and final status screen");
            // Coin route: native request, real account form, timed verification and next task, never an artificial advance.
            for(int step=0;step<5;step++)
            {
                Clean();g.cashRoute=false;g.selectedTask=step;OpenGm(1);Gm(14);Gm(25);yield return .2f;Capture("coin-task"+(step+1)+"-gap.png");Assert(!s.menus.CoinConditionsMet,"Coin button must be blocked");
                OpenGm(1);Gm(15);if(step==0)Gm(48);Gm(25);yield return 3.1f;Capture("coin-task"+(step+1)+"-ready.png");
                Click(s.menus.coinReady.GetComponentInChildren<Button>());
                if(step==0)
                {
                    Assert(s.menus.CurrentPage==RecoveredMainMenus.Email,"Account form missing");s.menus.forms[0].account.text="bad-address";Click(s.menus.forms[0].confirm.GetComponent<Button>());Assert(s.menus.CurrentPage==RecoveredMainMenus.Email,"Invalid account accepted");
                    s.menus.forms[0].account.text="tester@example.test";yield return .25f;Capture("account.png");Click(s.menus.forms[0].confirm.GetComponent<Button>());
                }
                Assert(s.menus.CurrentPage==RecoveredMainMenus.Verify,"No verification");double deadline=EditorApplication.timeSinceStartup+20;bool captured=false;
                while(s.menus.CurrentPage==RecoveredMainMenus.Verify){Assert(EditorApplication.timeSinceStartup<deadline,"Verification timeout");yield return .25f;if(!captured&&s.menus.verification.tips[2].gameObject.activeInHierarchy){Capture("verify-task"+(step+1)+".png");captured=true;}}
                Assert(s.Player.newFakeMoneyWithdraw[0]==step+1,"Wrong next task "+step);yield return .2f;Capture("coin-after-task"+(step+1)+".png");
            }
            checks.Add("Coin route: 5 real verification sequences, rejected invalid account, cached account reuse, final status");
            Clean();g.cashRoute=false;g.selectedTask=3;OpenGm(1);Gm(14);int days=s.Player.loginDays;Capture("time-before.png");Gm(16);Assert(s.Player.loginDays==days,"12h granted active day");Capture("time-12h.png");Gm(16);Assert(s.Player.loginDays==days,"24h granted active day");
            Gm(50);Assert(s.Player.today1024NumberCoin==4&&s.Player.loginDays==days,"Prepare 4/5");Capture("time-4of5.png");Gm(19);Assert(s.Player.loginDays==days+1,"5th merge did not qualify");Capture("time-complete.png");Gm(20);Assert(s.Player.loginDays==days+1,"Same day counted twice");
            Gm(18);Assert(s.Player.gmTimeOffsetSeconds==0,"Time offset restore");checks.Add("+12h twice does not grant a day; 4/5 -> 5/5 grants one; same date never counts twice; clock restore");
            // Compare both ad callback statistics and rewards through the actual UI.
            for(int outcome=0;outcome<4;outcome++)
            {
                Clean();OpenGm(2);Gm(30+outcome);Gm(36);s.Player.fakeMoney=220;s.Player.watch_video_count=0;s.Player.savedCurrentCoinValue=1;yield return .7f;Gm(37);
                double deadline=EditorApplication.timeSinceStartup+4;while(s.Player.windowsCointimes==21){Assert(EditorApplication.timeSinceStartup<deadline,"Drop not accepted");yield return .1f;}
                yield return .8f;Assert(s.Player.watch_video_count==(outcome==0?1:0),"Drop ad statistic "+outcome);Assert(s.Player.windowsCointimes==(outcome==2?22:0),"Drop counter reset "+outcome);
                if(outcome==0){Assert(s.rewardView.gameObject.activeSelf&&s.rewardView.Kind==2,"Double reward missing");Capture("double-reward.png");yield return 1.7f;Assert(s.Player.fakeMoney>220,"Double cash not credited");}else Assert(!s.rewardView.gameObject.activeSelf&&s.Player.fakeMoney==220,"Failed drop ad awarded cash");
            }
            checks.Add("22nd-drop ad: all four outcomes, exact counter reset and balance behavior");
            Clean();OpenGm(2);Gm(30);Gm(51);yield return .7f;Gm(37);yield return .85f;Assert(s.rewardView.Kind==3&&s.rewardView.gameObject.activeSelf,"First ordinary reward missing");Capture("normal-reward.png");yield return 3.4f;Assert(s.rating.gameObject.activeSelf,"First reward rating missing");Capture("rating.png");Click(s.rating.close);
            // Actual draw once, then retry no-ad and verify a single prize is settled.
            Clean();OpenGm(2);g.selectedSlot=7;g.Refresh();Gm(44);Capture("gm-events.png");Gm(45);s.gm.Hide();yield return 1;Assert(s.wheelView.gameObject.activeSelf,"Automatic wheel not opened");Capture("machine.png");Click(s.wheelView.draw);
            double wheelDeadline=EditorApplication.timeSinceStartup+20;
            while(!s.wheelRewardView.gameObject.activeSelf){Assert(EditorApplication.timeSinceStartup<wheelDeadline,"Wheel timeout");yield return .2f;}
            Capture("wheel-prize.png");int coins=s.Player.coin1024Number;int beforeAds=s.Player.watch_video_count;int today=s.Player.today1024NumberCoin;
            s.Sdk.NextAdOutcome=AdOutcome.Unavailable;Click(s.wheelRewardView.claim);Assert(s.wheelRewardView.gameObject.activeSelf&&s.Player.coin1024Number==coins,"No-ad wheel should remain pending");
            s.Sdk.NextAdOutcome=AdOutcome.Completed;Click(s.wheelRewardView.claim);Assert(s.Player.coin1024Number==coins+1&&s.Player.watch_video_count==beforeAds+1&&s.Player.today1024NumberCoin==today,"Wheel settlement counters");
            s.wheelRewardView.claim.onClick.Invoke();Assert(s.Player.coin1024Number==coins+1,"Duplicate claim awarded twice");
            foreach(var outcome in new[]{AdOutcome.Cancelled,AdOutcome.Failed}){s.wheelRewardView.Show(7,s.Player,s.board.Config,s.Locale,.5);s.Sdk.NextAdOutcome=outcome;coins=s.Player.coin1024Number;beforeAds=s.Player.watch_video_count;Click(s.wheelRewardView.claim);Assert(s.Player.coin1024Number==coins+1&&s.Player.watch_video_count==beforeAds,"Wheel error callback settlement");}
            checks.Add("Native machine spin + claim: no-ad retry, success, cancel, failure, no double award, lottery coins do not qualify an active day");
            Clean();OpenGm(2);Gm(38);yield return 4;Assert(s.failView.gameObject.activeSelf,"Failure popup missing");Capture("failure.png");
            foreach(var outcome in new[]{AdOutcome.Cancelled,AdOutcome.Unavailable,AdOutcome.Failed}){s.Sdk.NextAdOutcome=outcome;Click(s.failView.revive);Assert(s.failView.gameObject.activeSelf&&s.failView.revive.interactable,"Failed revive not retryable");}
            s.Sdk.NextAdOutcome=AdOutcome.Completed;Click(s.failView.revive);Assert(s.rewardView.Kind==1&&s.rewardView.gameObject.activeSelf,"Revive reward missing");Capture("revive.png");Click(s.rewardView.mask);Assert(!s.board.GameOver,"Revive did not resume board");
            checks.Add("Failure/revive: all four ad outcomes, retry button, reward and game resumes");
            Clean();OpenGm(2);Gm(52);yield return 1;Assert(s.rating.gameObject.activeSelf,"500 merge rating missing");Click(s.rating.close);
            Clean();OpenGm(2);Gm(39);yield return .6f;Capture("highest-animation.png");yield return 4;
            checks.Add("New first-reward, first-500 and 2000-merge GM buttons execute real gameplay paths");
            Clean();OpenGm(1);g.cashRoute=false;g.selectedTask=0;Gm(14);Gm(25);yield return .2f;Capture("coin-entry.png");
            File.WriteAllText(Output+"/progress.txt","Complete");
        }
        static void Render(){foreach(var p in s.GetComponentsInChildren<RecoveredMenuPopup>())p.Tick(1);Canvas.ForceUpdateCanvases();s.worldCamera.Render();}
        static void Capture(string name)
        {
            Render();var old=RenderTexture.active;RenderTexture.active=target;var texture=new Texture2D(target.width,target.height,TextureFormat.RGB24,false);texture.ReadPixels(new Rect(0,0,target.width,target.height),0,0);texture.Apply();File.WriteAllBytes(Output+"/"+name,texture.EncodeToPNG());RenderTexture.active=old;UnityEngine.Object.DestroyImmediate(texture);File.WriteAllText(Output+"/progress.txt",name);
        }
        static void Finish(string error)
        {
            EditorApplication.update-=Tick;Application.logMessageReceived-=Log;if(error!=null)errors.Add(error);
            if(s)s.worldCamera.targetTexture=null;if(target)UnityEngine.Object.DestroyImmediate(target);
            File.WriteAllText(Output+(SessionState.GetBool(Key+".layout",false)?"/layout-validation.json":"/validation.json"),JsonUtility.ToJson(new Report{passed=errors.Count==0,preparationCases=boundaries,nativeButtonClicks=clicks,checks=checks.ToArray(),errors=errors.ToArray(),unityVersion=Application.unityVersion},true));EditorApplication.ExitPlaymode();
        }
    }
}

