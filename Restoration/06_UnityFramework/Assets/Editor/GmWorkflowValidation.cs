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
    [InitializeOnLoad] public static class GmWorkflowValidation
    {
        const string Key="CoinMerge.GmWorkflow",Prefix="coinmerge.gm.workflow.disposable";
        static RecoveredGameSession session;static GameplayGmPanel gm;static Camera camera;static RenderTexture target;
        static readonly List<string> checks=new List<string>();static int phase,task,outcome;static double next,deadline,offset,cash;static int watched,coins;
        [Serializable] sealed class Report{public bool passed;public string error;public string[] checks;}
        static GmWorkflowValidation(){EditorApplication.playModeStateChanged+=State;}
        public static void Run()
        {
            SessionState.SetBool(Key,true);SessionState.SetString(Key+".error","");
            var store=new PlayerStore(Prefix);store.ResetPlayer();store.ResetProfile();store.Save(new PlayerProgress {guideStep=9999});
            store.SaveProfile(new VersionProfile {country="US",cohort="B",rewardedVariant=true});
            EditorSceneManager.OpenScene("Assets/Scenes/RecoveredMain.unity");var game=UnityEngine.Object.FindObjectOfType<RecoveredGameSession>();game.saveNamespace=Prefix;game.automaticInput=false;
            EditorApplication.EnterPlaymode();
        }
        static void State(PlayModeStateChange state)
        {
            if(!SessionState.GetBool(Key,false))return;
            if(state==PlayModeStateChange.EnteredPlayMode){checks.Clear();phase=task=outcome=0;next=EditorApplication.timeSinceStartup+1;deadline=next+150;Application.logMessageReceived+=Log;EditorApplication.update+=Tick;}
            if(state==PlayModeStateChange.EnteredEditMode){var store=new PlayerStore(Prefix);store.ResetPlayer();store.ResetProfile();SessionState.SetBool(Key,false);EditorApplication.Exit(SessionState.GetString(Key+".error","").Length==0?0:1);}
        }
        static void Log(string value,string stack,LogType kind){if(kind==LogType.Exception||kind==LogType.Error)SessionState.SetString(Key+".error",value+"\n"+stack);}
        static void Require(bool value,string reason){if(!value)throw new Exception(reason);checks.Add(reason);}
        static void Click(Button button)
        {
            if(!button.gameObject.activeInHierarchy||!button.IsInteractable())throw new Exception("Inactive Button "+button.name);
            Canvas.ForceUpdateCanvases();camera.Render();var rect=button.targetGraphic.rectTransform;
            var pointer=new PointerEventData(EventSystem.current){button=PointerEventData.InputButton.Left,position=RectTransformUtility.WorldToScreenPoint(camera,rect.TransformPoint(rect.rect.center))};
            var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(pointer,hits);
            var actual=hits.Count>0?ExecuteEvents.GetEventHandler<IPointerClickHandler>(hits[0].gameObject):null;
            if(actual!=button.gameObject)throw new Exception("Pointer blocked: "+button.name+" by "+(hits.Count>0?hits[0].gameObject.name:"none"));
            ExecuteEvents.Execute(actual,pointer,ExecuteEvents.pointerClickHandler);
        }
        static void Gm(int code){foreach(var b in gm.actions)if(b.action==code){Click(b.button);return;}throw new Exception("Missing GM "+code);}
        static void Menu(int code){foreach(var b in session.menus.actions)if(b.action==code&&b.button.gameObject.activeInHierarchy&&b.button.transform.IsChildOf(session.menus.pages[session.menus.CurrentPage].transform)){Click(b.button);return;}throw new Exception("Missing menu "+code);}
        static void Open(int tab){Click(session.gm.open);Gm(tab);}
        static void Tick()
        {
            if(EditorApplication.timeSinceStartup<next)return;next=EditorApplication.timeSinceStartup+.15;
            try
            {
                string error=SessionState.GetString(Key+".error","");if(error.Length>0)throw new Exception(error);if(EditorApplication.timeSinceStartup>deadline)throw new Exception("GM workflow timeout at phase "+phase);
                switch(phase)
                {
                    case 0:
                        session=UnityEngine.Object.FindObjectOfType<RecoveredGameSession>();gm=session.gm.gameplay;camera=session.worldCamera;target=new RenderTexture(750,1624,24);camera.targetTexture=target;
                        Require(gm&&gm.actions.Length==40,"GM has 40 code-bound standard Button actions");
                        foreach(var b in gm.actions)if(b.button.onClick.GetPersistentEventCount()!=0)throw new Exception("Inspector event binding detected");
                        Open(1);Capture("gm_progress_tools.png");
                        Require(((RectTransform)session.gm.popup.transform.Find("Card")).sizeDelta.y<1624,"GM remains a centered popup smaller than the game screen");
                        session.Player.gmTimeOffsetSeconds=(DateTime.Today.AddDays(1).AddHours(1)-DateTime.Now).TotalSeconds;
                        gm.selectedTask=3;Gm(14);offset=session.Player.gmTimeOffsetSeconds;
                        Gm(16);Require(session.Player.loginDays==14,"First +12h does not grant an active day");
                        Gm(16);Require(session.Player.loginDays==14&&session.Player.today1024NumberCoin==0,"Crossing midnight still requires daily merges");
                        for(int i=0;i<4;i++)Gm(19);Require(session.Player.loginDays==14,"Four highest-coin merge events do not qualify the day");
                        Gm(19);Require(session.Player.loginDays==15,"Fifth merge qualifies exactly one active day");
                        Gm(20);Require(session.Player.loginDays==15,"Further merges on the same date do not double-count a day");
                        Gm(17);Gm(20);Require(session.Player.loginDays==16,"Next day plus five merge events adds one more active day");
                        var saved=new PlayerStore(Prefix).LoadPlayer();Require(Math.Abs(saved.gmTimeOffsetSeconds-offset-48*3600)<.01&&saved.loginDays==16,"Time offset and earned days persist through PlayerStore reload");
                        gm.number.text="NaN";double before=session.Player.fakeMoney;Gm(21);Require(session.Player.fakeMoney==before,"Invalid numeric input cannot corrupt player money");
                        session.gm.Hide();phase=1;break;
                    case 1:
                        Open(1);gm.selectedTask=task;Gm(14);Gm(25);Require(!session.menus.CoinConditionsMet,"Task "+(task+1)+" blocks withdrawal at threshold minus one");
                        Open(1);Gm(15);Gm(25);Require(session.menus.CoinConditionsMet,"Task "+(task+1)+" enables withdrawal exactly at threshold");
                        next=EditorApplication.timeSinceStartup+3.1;phase=2;break;
                    case 2:
                        Menu(10);
                        if(session.menus.CurrentPage==RecoveredMainMenus.Email){session.menus.forms[0].account.text="gm-fixture@example.test";Menu(11);}
                        Require(session.menus.CurrentPage==RecoveredMainMenus.Verify,"Task "+(task+1)+" enters the real account verification animation");phase=3;break;
                    case 3:
                        if(session.menus.CurrentPage==RecoveredMainMenus.Verify)break;
                        Require(session.Player.newFakeMoneyWithdraw[0]==task+1,"Task "+(task+1)+" advances through the normal animation callback");
                        Require(session.menus.CurrentPage==(task==4?RecoveredMainMenus.Active:RecoveredMainMenus.NextStage),"Task "+(task+1)+" opens the expected next result page");
                        if(task==4)Capture("gm_withdrawal_all_tasks.png");session.menus.CloseAll();task++;
                        phase=task<5?1:4;break;
                    case 4:
                        Open(2);Gm(30+outcome);Gm(36);cash=session.Player.fakeMoney;watched=session.Player.watch_video_count;
                        if(outcome==0)Capture("gm_event_tools.png");Gm(37);next=EditorApplication.timeSinceStartup+3;phase=5;break;
                    case 5:
                        Require(session.Player.windowsCointimes==0,"1_A outcome "+outcome+" resets the current Unity drop window");
                        Require(session.Player.watch_video_count==watched+(outcome==0?1:0),"1_A outcome "+outcome+" counts only completed ads");
                        Require(outcome==0?session.Player.fakeMoney>cash:session.Player.fakeMoney==cash,"1_A outcome "+outcome+" applies correct reward settlement");
                        outcome++;phase=outcome<4?4:6;break;
                    case 6:
                        Open(2);gm.selectedSlot=1;Gm(44);Require(session.Player.gameTotalScore==49,"Wheel fixture starts one point before first threshold");Gm(45);session.gm.Hide();phase=7;break;
                    case 7:
                        if(!session.wheelView.gameObject.activeSelf)break;Click(session.wheelView.draw);phase=8;break;
                    case 8:
                        if(!session.wheelRewardView.gameObject.activeSelf)break;
                        Require(session.wheelRewardView.Coins==10&&session.wheelRewardView.RequiresAd,"One-shot forced zero-weight slot still uses normal spin and ad gate");
                        coins=session.Player.coin1024Number;session.Sdk.NextAdOutcome=AdOutcome.Unavailable;Click(session.wheelRewardView.claim);
                        Require(!session.wheelRewardView.Settled&&session.Player.coin1024Number==coins,"Unavailable wheel ad keeps reward open without settlement");
                        session.Sdk.NextAdOutcome=AdOutcome.Cancelled;Click(session.wheelRewardView.claim);
                        Require(session.wheelRewardView.Settled&&session.Player.coin1024Number==coins+10,"Cancelled wheel callback settles once as the original error callback does");
                        Require(new PlayerStore(Prefix).LoadProfile(session.board.Config).rewardedVariant,"GM workflow keeps the chosen content version");
                        Open(2);coins=session.Player.coin1024Number;Gm(39);phase=9;break;
                    case 9:
                        if(!session.rewardView.gameObject.activeSelf)break;
                        Require(session.rewardView.Kind==4&&session.Player.coin1024Number==coins+1,"GM real 1000+1000 merge reaches highest-coin reward and counting event");
                        Click(session.rewardView.highestClose);Open(2);Gm(38);phase=10;break;
                    case 10:
                        if(!session.failView.gameObject.activeSelf)break;
                        Capture("gm_revive_binding.png");
                        session.Sdk.NextAdOutcome=AdOutcome.Failed;Click(session.failView.revive);
                        Require(session.failView.gameObject.activeSelf&&session.failView.revive.interactable,"GM failure entry preserves failed 3_A retry path");
                        session.Sdk.NextAdOutcome=AdOutcome.Completed;Click(session.failView.revive);
                        Require(session.rewardView.gameObject.activeSelf&&session.rewardView.Kind==1,"Successful 3_A enters revive reward through the normal callback");
                        Click(session.rewardView.mask);Require(!session.board.GameOver,"Closing revive reward resumes the board");
                        session.Player.gameTotalScore=RecoveredGameRules.RequiredScore(session.board.Config.rules.lotteryScores,session.Player.currentLotteryCount);session.GmRefresh();phase=11;break;
                    case 11:
                        if(!session.wheelView.gameObject.activeSelf)break;Click(session.wheelView.draw);phase=12;break;
                    case 12:
                        if(!session.wheelRewardView.gameObject.activeSelf)break;
                        Require(session.wheelRewardView.Coins!=10,"Forced zero-weight slot is consumed once; following spin returns to original probability table");
                        Finish(null);break;
                }
            }
            catch(Exception e){Finish(e.ToString());}
        }
        static void Capture(string name)
        {
            Canvas.ForceUpdateCanvases();camera.Render();var previous=RenderTexture.active;RenderTexture.active=target;
            var texture=new Texture2D(750,1624,TextureFormat.RGB24,false);texture.ReadPixels(new Rect(0,0,750,1624),0,0);texture.Apply();File.WriteAllBytes("../07_Verification/"+name,texture.EncodeToPNG());RenderTexture.active=previous;UnityEngine.Object.DestroyImmediate(texture);
        }
        static void Finish(string error)
        {
            EditorApplication.update-=Tick;Application.logMessageReceived-=Log;SessionState.SetString(Key+".error",error??"");
            File.WriteAllText("../07_Verification/gm_workflow_validation.json",JsonUtility.ToJson(new Report {passed=error==null,error=error,checks=checks.ToArray()},true));
            Debug.Log(error==null?"GM_WORKFLOW_VALIDATED "+checks.Count:error);if(camera)camera.targetTexture=null;if(target)UnityEngine.Object.DestroyImmediate(target);EditorApplication.ExitPlaymode();
        }
    }
}
