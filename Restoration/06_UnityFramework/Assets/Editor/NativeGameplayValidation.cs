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
    // Runs the authored scene in actual Play Mode and invokes standard Button.onClick events.
    [InitializeOnLoad]
    public static class NativeGameplayValidation
    {
        const string Key="CoinMerge.NativePlayValidation";
        const string StorePrefix="coinmerge.validation.disposable";
        static readonly List<string> checks=new List<string>();
        static RecoveredGameSession session;
        static RenderTexture pointerSurface;
        static double next;
        static int phase;
        static double beforeReward;
        [Serializable] sealed class Report {public bool passed;public string unityVersion,error;public string[] checks;public string scope;}
        static NativeGameplayValidation(){EditorApplication.playModeStateChanged+=State;}
        [MenuItem("Coin Merge/Validate native gameplay in Play Mode")]
        public static void Run()
        {
            RecoveredAssetValidation.Run();
            SessionState.SetBool(Key,true);SessionState.SetString(Key+".error","");
            var store=new PlayerStore(StorePrefix);store.ResetPlayer();store.ResetProfile();
            EditorSceneManager.OpenScene("Assets/Scenes/RecoveredMain.unity");
            var target=UnityEngine.Object.FindObjectOfType<RecoveredGameSession>();
            target.saveNamespace=StorePrefix;target.automaticInput=false;
            EditorApplication.EnterPlaymode();
        }
        static void State(PlayModeStateChange state)
        {
            if(!SessionState.GetBool(Key,false))return;
            if(state==PlayModeStateChange.EnteredPlayMode)
            {
                try
                {
                    session=UnityEngine.Object.FindObjectOfType<RecoveredGameSession>();
                    // Keep render and pointer projection in the original portrait viewport for the whole run.
                    pointerSurface=new RenderTexture(750,1624,24);session.worldCamera.targetTexture=pointerSurface;
                    session.automaticInput=false;session.Initialize(new PlayerStore(StorePrefix));
                    Application.logMessageReceived+=Log;
                    phase=0;next=EditorApplication.timeSinceStartup+1;EditorApplication.update+=Tick;
                }
                catch(Exception ex){Finish(ex.ToString());}
            }
            if(state==PlayModeStateChange.EnteredEditMode)
            {
                var store=new PlayerStore(StorePrefix);store.ResetPlayer();store.ResetProfile();
                SessionState.SetBool(Key,false);
                EditorApplication.Exit(string.IsNullOrEmpty(SessionState.GetString(Key+".error",""))?0:1);
            }
        }
        static void Log(string condition,string stack,LogType type)
        {if(type==LogType.Exception||type==LogType.Error)SessionState.SetString(Key+".error",condition+"\n"+stack);}
        static void ClickGraphic(Button button)
        {
            if(!button.gameObject.activeInHierarchy||!button.targetGraphic.enabled||!button.IsInteractable())
                throw new Exception("Pointer target is hidden or disabled: "+button.name);
            Canvas.ForceUpdateCanvases();
            // A dialog activated in this test tick has not rendered yet; UGUI ignores depth -1 graphics.
            session.worldCamera.Render();
            var rect=button.targetGraphic.rectTransform;
            var pointer=new PointerEventData(EventSystem.current) {button=PointerEventData.InputButton.Left,
                position=RectTransformUtility.WorldToScreenPoint(session.worldCamera,rect.TransformPoint(rect.rect.center))};
            var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(pointer,hits);
            if(hits.Count==0)
            {
                var canvas=button.targetGraphic.canvas;var raycaster=canvas.GetComponent<GraphicRaycaster>();
                throw new Exception("Pointer missed all graphics: "+button.name+" position="+pointer.position+" screen="+Screen.width+"x"+Screen.height+
                    " depth="+button.targetGraphic.depth+" culled="+button.targetGraphic.canvasRenderer.cull+" canvasMode="+canvas.renderMode+
                    " eventCamera="+(raycaster.eventCamera?raycaster.eventCamera.name:"null")+" graphicWorld="+rect.position+
                    " contains="+RectTransformUtility.RectangleContainsScreenPoint(rect,pointer.position,raycaster.eventCamera));
            }
            var target=ExecuteEvents.GetEventHandler<IPointerClickHandler>(hits[0].gameObject);
            if(target!=button.gameObject)throw new Exception("Pointer intercepted before "+button.name+" by "+hits[0].gameObject.name);
            ExecuteEvents.Execute(target,pointer,ExecuteEvents.pointerDownHandler);
            ExecuteEvents.Execute(target,pointer,ExecuteEvents.pointerUpHandler);
            ExecuteEvents.Execute(target,pointer,ExecuteEvents.pointerClickHandler);
        }
        static void Tick()
        {
            if(EditorApplication.timeSinceStartup<next)return;
            try
            {
                string error=SessionState.GetString(Key+".error","");if(error.Length>0)throw new Exception(error);
                var board=session.board;var player=session.Player;
                switch(phase++)
                {
                    case 0:
                        Require(board.Coins.Count==6,"Five original bottom coins plus a world-space preview");
                        Require(board.Preview.visual.sprite!=null&&board.Preview.IsPreview&&!board.Preview.body.simulated,"Preview sprite loaded with physics disabled");
                        Require(session.Profile.country=="US"&&session.Profile.cohort=="B"&&session.Profile.rewardedVariant,"Embedded default is US rewarded cohort B");
                        Capture("native_main_initial.png");
                        player.guideStep=9999;session.guideView.gameObject.SetActive(false);
                        player.hasSavedGameScene=true;player.savedCoins=new[]{new SavedCoin {value=1,x=0,y=-455.064f+27+10}};
                        player.savedCurrentCoinValue=1;player.savedNextCoinValue=1;board.Initialize(player);
                        next=EditorApplication.timeSinceStartup+.3;break;
                    case 1:
                        board.MovePreview(0);Require(board.RequestDrop(),"Drop accepted after preview tween");
                        Require(board.Preview==null,"Released preview leaves queue until delay elapses");
                        next=EditorApplication.timeSinceStartup+2;break;
                    case 2:
                        Require(player.roundScore==1&&player.gameTotalScore==1,"Real physics contact merges 1+1 into 2 and awards original score");
                        bool two=false;foreach(var c in board.Coins)if(c.value==2&&!c.IsPreview)two=true;
                        Require(two,"Merged world-space SpriteRenderer coin exists");
                        session.Save();var loaded=new PlayerStore(StorePrefix).LoadPlayer();
                        Require(loaded.gameTotalScore==1&&loaded.savedCoins.Length==1&&loaded.savedCoins[0].value==2,"Saved board reload preserves merged coin and cumulative score");
                        session.ChangeProfile("JP","A",false);session.ResetPlayerKeepingProfile();
                        Require(session.Profile.country=="JP"&&session.Profile.cohort=="A"&&!session.Profile.rewardedVariant,"Player reset retains separate GM profile");
                        session.ChangeProfile("US","B",true);
                        player=session.Player;player.guideStep=1;session.guideView.Show(1,player.fakeMoney);
                        ClickGraphic(session.guideView.oneButton);
                        Require(player.guideStep==2&&session.rewardView.gameObject.activeSelf&&session.rewardView.Kind==5,"Guide Button opens initial cash reward");
                        double guide=RecoveredGameRules.CashConfiguration(board.Config.rules,"US").guideMoney;
                        session.rewardView.guideClose.onClick.Invoke();
                        Require(player.fakeMoney==guide&&player.guideStep==3,"Guide reward settles real cached guideMoney once");
                        Capture("native_guide_step3.png");
                        ClickGraphic(session.guideView.threeButton);
                        Require(player.guideStep==4,"Visible guide card receives EventSystem pointer hit and advances step 3");
                        Capture("native_guide_step4.png");
                        ClickGraphic(session.guideView.backdropButton);
                        Require(player.guideStep==9999,"Visible dimmer receives pointer hit and advances step 4");
                        player.guideStep=3;session.guideView.Show(3,player.fakeMoney);ClickGraphic(session.guideView.backdropButton);
                        Require(player.guideStep==4,"Step 3 also accepts viewport backdrop pointer hit");
                        ClickGraphic(session.guideView.fourButton);
                        Require(player.guideStep==9999&&!session.guideView.gameObject.activeSelf,"Remaining guide Buttons reach original terminal step");
                        board.InputBlocked=false;board.TriggerFailure();next=EditorApplication.timeSinceStartup+1.4;break;
                    case 3:
                        Require(session.failView.gameObject.activeSelf&&board.GameOver,"Failure delay opens original fail prefab");
                        session.Sdk.NextAdOutcome=AdOutcome.Failed;session.failView.revive.onClick.Invoke();
                        Require(session.failView.gameObject.activeSelf&&session.failView.revive.interactable,"Failed revive ad leaves failure dialog usable");
                        beforeReward=player.fakeMoney;session.Sdk.NextAdOutcome=AdOutcome.Completed;session.failView.revive.onClick.Invoke();
                        Require(session.rewardView.Kind==1&&session.rewardView.gameObject.activeSelf,"Completed 3_A ad opens revive reward");
                        session.rewardView.mask.onClick.Invoke();
                        Require(!board.GameOver&&player.fakeMoney>beforeReward,"Revive reward closes, grants cash, resumes physics");
                        Require(board.Coins.Count==4,"Revive removes ceil(5/3) lower coins and restores one preview");
                        Require(session.Sdk.Trace.Contains("ad.request:3_A"),"Original revive placement crosses existing SDK mock facade");
                        Capture("native_main_after_revive.png");
                        player.gameTotalScore=151;player.roundScore=42;double balance=player.fakeMoney;
                        board.TriggerFailure();board.ResetAfterFailure();
                        Require(player.gameTotalScore==151&&player.roundScore==0&&player.fakeMoney==balance&&board.Coins.Count==1,"Failure restart retains draw score and cash; no initial coins recreated");
                        player.gameTotalScore=50;board.ResetAfterFailure();next=EditorApplication.timeSinceStartup+1;break;
                    case 4:
                        Require(session.wheelView.gameObject.activeSelf,"Score threshold automatically opens wheel after original popup delay");
                        session.wheelView.draw.onClick.Invoke();
                        Require(session.wheelView.IsSpinning&&player.gameTotalScore==50,"Wheel Button starts animation without spending score early");
                        Capture("native_wheel.png");next=EditorApplication.timeSinceStartup+10;break;
                    case 5:
                        Require(session.wheelRewardView.gameObject.activeSelf&&player.currentLotteryCount==1&&player.gameTotalScore==0,"Wheel animation completes before score spending and prize popup");
                        beforeReward=player.fakeMoney;int beforeCoins=player.coin1024Number;
                        session.Sdk.NextAdOutcome=AdOutcome.Unavailable;session.wheelRewardView.claim.onClick.Invoke();
                        Require(session.wheelRewardView.gameObject.activeSelf&&player.fakeMoney==beforeReward&&player.coin1024Number==beforeCoins,"Unavailable 2_A ad leaves prize unsettled");
                        double prize=session.wheelRewardView.Cash;int coinPrize=session.wheelRewardView.Coins;
                        session.Sdk.NextAdOutcome=AdOutcome.Failed;session.wheelRewardView.claim.onClick.Invoke();
                        Require(!session.wheelRewardView.gameObject.activeSelf&&player.fakeMoney==beforeReward+prize&&player.coin1024Number==beforeCoins+coinPrize,"Started-but-failed 2_A ad settles exactly as original error callback");
                        session.wheelRewardView.claim.onClick.Invoke();
                        Require(player.fakeMoney==beforeReward+prize&&player.coin1024Number==beforeCoins+coinPrize,"Repeated prize click is idempotent");
                        Require(session.Sdk.Trace.Contains("ad.request:2_A"),"Wheel reward crosses original SDK placement");
                        Finish(null);break;
                }
            }
            catch(Exception ex){Finish(ex.ToString());}
        }
        static void Require(bool value,string label){if(!value)throw new Exception(label);checks.Add(label);}
        static void Capture(string name)
        {
            var camera=session.worldCamera;var old=camera.targetTexture;
            var target=new RenderTexture(750,1624,24);camera.targetTexture=target;
            Canvas.ForceUpdateCanvases();camera.Render();RenderTexture previous=RenderTexture.active;RenderTexture.active=target;
            var texture=new Texture2D(750,1624,TextureFormat.RGB24,false);texture.ReadPixels(new Rect(0,0,750,1624),0,0);texture.Apply();
            File.WriteAllBytes("../07_Verification/"+name,texture.EncodeToPNG());
            RenderTexture.active=previous;camera.targetTexture=old;UnityEngine.Object.DestroyImmediate(texture);UnityEngine.Object.DestroyImmediate(target);
        }
        static void Finish(string error)
        {
            EditorApplication.update-=Tick;Application.logMessageReceived-=Log;
            if(session)session.worldCamera.targetTexture=null;
            if(pointerSurface)UnityEngine.Object.DestroyImmediate(pointerSurface);
            SessionState.SetString(Key+".error",error??"");
            var report=new Report {passed=error==null,unityVersion=Application.unityVersion,error=error,checks=checks.ToArray(),scope="Actual Unity Play Mode: physical merge, queue, save, guide cash, fail/revive mock branches. Does not certify visual/whole-lifecycle parity."};
            File.WriteAllText("../07_Verification/native_gameplay_validation.json",JsonUtility.ToJson(report,true));
            Debug.Log(error==null?"NATIVE_PLAY_MODE_VALIDATION_PASSED "+checks.Count:error);
            EditorApplication.ExitPlaymode();
        }
    }
}
