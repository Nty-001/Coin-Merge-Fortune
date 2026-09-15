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
    [InitializeOnLoad] public static class LifecycleParityValidation
    {
        const string Key="CoinMerge.LifecycleCheck",Prefix="coinmerge.lifecycle.disposable";
        static readonly List<string> checks=new List<string>();
        static RecoveredGameSession s;static RecoveredLifecycleFeedback f;static RenderTexture texture;static double next,deadline;static int phase;
        [Serializable] sealed class Report{public bool passed;public string error;public string[] checks;}
        static LifecycleParityValidation(){EditorApplication.playModeStateChanged+=State;}
        public static void Run()
        {
            var store=new PlayerStore(Prefix);store.ResetPlayer();store.ResetProfile();store.Save(new PlayerProgress{guideStep=9999,hasSavedGameScene=true,currentLotteryCount=100});store.SaveProfile(new VersionProfile{country="US",rewardedVariant=true,cohort="B"});
            EditorSceneManager.OpenScene("Assets/Scenes/RecoveredMain.unity");var game=UnityEngine.Object.FindObjectOfType<RecoveredGameSession>();game.saveNamespace=Prefix;game.automaticInput=false;
            SessionState.SetBool(Key,true);SessionState.SetString(Key+".error","");EditorApplication.EnterPlaymode();
        }
        static void State(PlayModeStateChange state)
        {
            if(!SessionState.GetBool(Key,false))return;
            if(state==PlayModeStateChange.EnteredPlayMode){checks.Clear();phase=0;next=EditorApplication.timeSinceStartup+1;deadline=next+70;EditorApplication.update+=Tick;Application.logMessageReceived+=Log;}
            if(state==PlayModeStateChange.EnteredEditMode){var store=new PlayerStore(Prefix);store.ResetPlayer();store.ResetProfile();SessionState.SetBool(Key,false);EditorApplication.Exit(SessionState.GetString(Key+".error","").Length==0?0:1);}
        }
        static void Log(string text,string stack,LogType type){if(type==LogType.Error||type==LogType.Exception)SessionState.SetString(Key+".error",text+"\n"+stack);}
        static void Require(bool yes,string why){if(!yes)throw new Exception(why);checks.Add(why);}
        static void Surface(int w,int h)
        {if(texture){s.worldCamera.targetTexture=null;texture.Release();UnityEngine.Object.DestroyImmediate(texture);}texture=new RenderTexture(w,h,24);s.worldCamera.targetTexture=texture;s.worldCamera.Render();Canvas.ForceUpdateCanvases();}
        static void Click(Button button)
        {
            Canvas.ForceUpdateCanvases();s.worldCamera.Render();var rect=button.targetGraphic.rectTransform;
            var p=new PointerEventData(EventSystem.current){button=PointerEventData.InputButton.Left,position=RectTransformUtility.WorldToScreenPoint(s.worldCamera,rect.TransformPoint(rect.rect.center))};
            var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(p,hits);var actual=hits.Count>0?ExecuteEvents.GetEventHandler<IPointerClickHandler>(hits[0].gameObject):null;
            if(actual!=button.gameObject)throw new Exception("Pointer missed "+button.name+": "+(hits.Count>0?hits[0].gameObject.name:"none"));ExecuteEvents.Execute(actual,p,ExecuteEvents.pointerClickHandler);
        }
        static bool Visible(RectTransform rect)
        {var corners=new Vector3[4];rect.GetWorldCorners(corners);foreach(var c in corners){var p=s.worldCamera.WorldToViewportPoint(c);if(p.x<-.01f||p.x>1.01f||p.y<-.01f||p.y>1.01f)return false;}return true;}
        static void Fixture()
        {s.rating.Close();s.menus.CloseAll();s.rewardView.gameObject.SetActive(false);s.Player.hasSavedGameScene=true;s.Player.savedCoins=Array.Empty<SavedCoin>();s.Player.guideStep=9999;s.Player.gameTotalScore=0;s.Player.currentLotteryCount=100;s.board.Initialize(s.Player);s.board.Tick(.4f);f.ResetVisuals();s.mergeFeedback.ResetScore();}
        static NativeMergeCoin Merge(int value)
        {
            foreach(var c in s.board.Coins)c.body.simulated=false;float r=RecoveredGameRules.Coin(s.board.Config.rules,value).radiusPixels/s.board.Units;
            var point=new Vector3(0,s.board.ground.position.y+450/s.board.Units);var a=s.board.Spawn(value,point-Vector3.right*r*.75f);var b=s.board.Spawn(value,point+Vector3.right*r*.75f);a.body.gravityScale=b.body.gravityScale=0;
            Physics2D.SyncTransforms();Physics2D.Simulate(.02f);s.board.Tick(.21f);foreach(var c in s.board.Coins)c.body.simulated=false;
            foreach(var c in s.board.Coins)if(!c.IsPreview&&c.value==RecoveredGameRules.Coin(s.board.Config.rules,value).upgrade)return c;throw new Exception("Real physical merge missing");
        }
        static void Tick()
        {
            if(EditorApplication.timeSinceStartup<next)return;
            try
            {
                if(EditorApplication.timeSinceStartup>deadline)throw new Exception("Lifecycle validation timeout phase "+phase);
                var error=SessionState.GetString(Key+".error","");if(error.Length>0)throw new Exception(error);
                if(phase==0)
                {
                    s=UnityEngine.Object.FindObjectOfType<RecoveredGameSession>();f=s.lifecycle;s.enabled=f.enabled=false;Time.timeScale=0;Surface(750,1624);Fixture();
                    Require(s.board.visualConfig&&s.rating&&f.highestEffect,"Delivered scene binds lifecycle config, original highest effect and rating popup");
                    double before=s.Player.fakeMoney;s.ShowReward(3);Click(s.rewardView.mask);Require(s.rewardView.gameObject.activeSelf&&s.Player.fakeMoney==before,"Normal reward mask cannot bypass the original 1.5-second timer");
                    Require(s.rewardView.normalTitle.text==s.Locale.Label("110")&&s.rewardView.normalAmount.text.StartsWith("+"),"Normal reward uses original title key and plus-prefixed amount");
                    s.rewardView.Tick(1.49f);Require(s.rewardView.gameObject.activeSelf,"Normal reward remains open at 1.49 seconds");s.rewardView.Tick(.02f);
                    Require(!s.rewardView.gameObject.activeSelf&&s.Player.fakeMoney>before&&f.ActiveMoneyCount==3,"Timed close settles cash once and emits three original money flights");
                    before=s.Player.fakeMoney;s.rewardView.Close();Require(s.Player.fakeMoney==before,"Repeated reward close cannot double-settle money");
                    f.Tick(.8f);Capture("parity_cash_flight.png");Require(!f.plus.gameObject.activeSelf,"Top plus-amount waits until all three banknotes arrive");
                    f.Tick(1);Require(f.ActiveMoneyCount==0&&f.plus.gameObject.activeSelf&&f.CompletedMoneyBatches==1,"Final arriving banknote pulses cash icon and starts top amount text once");Capture("parity_cash_plus.png");f.Tick(1.81f);Require(!f.plus.gameObject.activeSelf,"Top plus-amount rises and disappears after 1.8 seconds");
                    s.ShowReward(1);Require(s.rewardView.doubleTitle.text==s.Locale.Label("34"),"Revive title differs from Double Rewards");s.rewardView.Close();
                    s.ShowReward(2);Require(s.rewardView.doubleTitle.text==s.Locale.Label("35"),"Double reward uses original title key 35");Click(s.rewardView.mask);Require(s.rewardView.gameObject.activeSelf,"Double reward also ignores an early mask click");s.rewardView.Tick(1.51f);f.ResetVisuals();
                    s.rating.Show();Click(s.rating.stars[1]);Require(s.rating.SelectedIndex==1&&s.rating.stars[1].image.sprite==s.rating.filledStar&&s.rating.stars[2].image.sprite==s.rating.emptyStar,"Rating uses one visible standard Button per empty/filled star");
                    int traces=s.Sdk.Trace.Count;Click(s.rating.confirm);Require(s.Sdk.Trace.Count==traces,"Two-star submission closes without a market request");
                    s.rating.Show();Click(s.rating.stars[3]);s.rating.GetComponent<RecoveredMenuPopup>().Tick(.36f);Capture("parity_rating.png");Click(s.rating.confirm);Require(s.Player.gameRateScore==3&&s.Sdk.Trace[s.Sdk.Trace.Count-1]=="market.request:mock","Four-star rating stores original zero-based index and uses market mock");
                    Fixture();s.Player.firstMergeIcon500=false;s.Player.gameRateScore=0;Merge(200);Require(s.rating.gameObject.activeSelf&&s.Player.firstMergeIcon500,"First physical 200+200 merge opens original 500-coin rating trigger");s.rating.Close();Merge(200);Require(!s.rating.gameObject.activeSelf,"Later 500-coin merges do not repeat first-merge rating");
                    Fixture();s.Player.dropCointimes=s.Player.windowsCointimes=s.board.Config.rules.flow.firstRewardDrop-1;s.board.Tick(.4f);s.board.RequestDrop();s.Tick(.61f);Require(s.rewardView.gameObject.activeSelf&&s.rewardView.Kind==3,"First original drop threshold enters ordinary reward");s.rewardView.Tick(1.51f);s.Tick(1.49f);Require(!s.rating.gameObject.activeSelf,"First reward rating waits another 1.5 seconds after settlement");s.Tick(.02f);Require(s.rating.gameObject.activeSelf&&s.Player.gameRateTimes==1,"First reward opens rating and persists gameRateTimes at delay boundary");
                    Fixture();int count=s.Player.coin1024Number;var highest=Merge(1000);Require(s.board.HighestFlowActive&&s.Player.coin1024Number==count+1&&!s.rewardView.gameObject.activeSelf,"Physical 1000+1000 starts flight and increments real counters without an invented reward dialog");
                    Require(f.DisplayedHighest==count,"Highest-coin badge waits for arrival");f.Tick(.22f);Require(highest.transform.localScale.x>1.07f,"Highest world-space coin grows during original 0.22-second rise");Capture("parity_highest_rise.png");
                    f.Tick(.281f);Require(f.HighestPhase==2&&!highest.IsAlive&&f.highestEffect.gameObject.activeSelf,"After 0.5 seconds the coin enters original GuangH_TX idle1 animation");
                    Time.timeScale=1;phase=1;next=EditorApplication.timeSinceStartup+.35;return;
                }
                if(phase==1)
                {
                    Capture("parity_highest_effect.png");phase=2;next=EditorApplication.timeSinceStartup+.1;return;
                }
                if(phase==2)
                {
                    if(f.HighestPhase!=3)return;Time.timeScale=0;f.Tick(.28f);Capture("parity_highest_flight.png");f.Tick(.28f);
                    Require(!s.board.HighestFlowActive&&f.DisplayedHighest==s.Player.coin1024Number&&!f.highestEffect.gameObject.activeSelf,"Native animation completion drives 0.55-second flight, badge update and preview resume");
                    s.board.Tick(.31f);Require(s.board.Preview&&!s.rewardView.gameObject.activeSelf,"Normal preview queue resumes without a reward popup after highest coin arrival");
                    Fixture();var a=s.board.Spawn(50,new Vector3(-2,s.board.deadLine.position.y+.1f));var b=s.board.Spawn(10,new Vector3(2,s.board.ground.position.y+2));a.body.simulated=b.body.simulated=false;
                    f.Tick(.21f);Require(f.warning.gameObject.activeSelf,"Settled high pile shows original danger line");s.board.CheckGameOver();Require(s.board.GameOver&&a.visual.color==f.config.failureTint&&b.visual.color==Color.white,"Only the actual failing coin is tinted red");
                    Require(!s.failView.gameObject.activeSelf,"Fail dialog waits for board animation completion");f.Tick(.14f);Require(a.visual.transform.localScale.x>1.15f&&a.visual.transform.localScale.y<.81f,"Failure uses original first squash keyframe");Capture("parity_failure_squash.png");
                    float duration=f.config.FailureDuration(2);s.board.Tick(duration-.01f);Require(!s.failView.gameObject.activeSelf,"Fail popup remains hidden before the staggered animation ends");s.board.Tick(.02f);f.Tick(2);Require(s.failView.gameObject.activeSelf&&a.visual.transform.localScale==Vector3.one,"Last squash completion restores scale and opens fail dialog");
                    s.failView.gameObject.SetActive(false);s.board.Revive();Require(a.IsAlive&&a.visual.color==Color.white&&a.visual.transform.localScale==Vector3.one,"Revive restores surviving coin color and visual scale");
                    Fixture();Surface(750,1334);Require(Mathf.Abs(s.board.previewLine.localPosition.y*s.board.Units-(558.466f+(1334-1624)*.4f))<.02f,"Short screen uses original 40-percent preview/deadline shift");
                    Require(Mathf.Abs(s.board.ground.position.y-s.playfieldLayout.bottom.TransformPoint(new Vector3(0,s.playfieldLayout.bottom.rect.yMax)).y)<.001f,"World floor matches actual bottom bar top");
                    Require(Mathf.Abs(s.worldCamera.orthographicSize*2*s.worldCamera.aspect*s.board.Units-750)<.02f,"World gameplay preserves original 750-pixel width at short aspect ratio");Capture("parity_main_short.png");
                    Require(Mathf.Abs(s.board.Preview.transform.position.y-s.board.previewLine.position.y)<.001f,"Non-simulated preview visual follows the resized preview line immediately");
                    Require(Visible(s.moneyText.rectTransform)&&Visible(s.wheelButton.targetGraphic.rectTransform)&&Visible(s.remainingText.rectTransform),"Cash header and bottom wheel controls remain fully visible on a short screen");
                    Require(Visible(s.playfieldLayout.gmButton),"GM button remains fully inside the short-screen viewport");
                    Surface(750,1800);Require(Mathf.Abs(s.board.previewLine.localPosition.y*s.board.Units-558.466f)<.02f,"Tall screen does not apply short-screen shift");Capture("parity_main_tall.png");
                    Require(Visible(s.moneyText.rectTransform)&&Visible(s.wheelButton.targetGraphic.rectTransform),"Cash header and wheel remain visible on a tall screen");
                    Require(Visible(s.playfieldLayout.gmButton),"GM button remains fully inside the tall-screen viewport");
                    Surface(750,1624);Fixture();s.menus.Act(1);Require(s.menus.IsOpen,"Settings remains operable after lifecycle changes");s.menus.CloseAll();
                    Finish("");
                }
            }
            catch(Exception ex){Finish(ex.ToString());}
        }
        static void Capture(string name)
        {Canvas.ForceUpdateCanvases();s.worldCamera.Render();var previous=RenderTexture.active;RenderTexture.active=texture;var image=new Texture2D(texture.width,texture.height,TextureFormat.RGB24,false);image.ReadPixels(new Rect(0,0,texture.width,texture.height),0,0);image.Apply();File.WriteAllBytes("../07_Verification/"+name,image.EncodeToPNG());UnityEngine.Object.DestroyImmediate(image);RenderTexture.active=previous;}
        static void Finish(string error)
        {
            EditorApplication.update-=Tick;Application.logMessageReceived-=Log;Time.timeScale=1;
            if(error.Length==0)error=SessionState.GetString(Key+".error","");SessionState.SetString(Key+".error",error);
            File.WriteAllText("../07_Verification/lifecycle_parity_validation.json",JsonUtility.ToJson(new Report{passed=error.Length==0,error=error,checks=checks.ToArray()},true));
            if(texture){s.worldCamera.targetTexture=null;texture.Release();UnityEngine.Object.DestroyImmediate(texture);}Debug.Log(error.Length==0?"LIFECYCLE_PARITY_VALIDATED "+checks.Count:error);EditorApplication.ExitPlaymode();
        }
    }
}
