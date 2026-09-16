using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
namespace CoinMerge.Recovery.Editor {
[InitializeOnLoad] public static class RemainingReskinCapture {
 const string Key="CoinMerge.RemainingCapture",Prefix="coinmerge.remaining.capture.disposable";
 static RecoveredGameSession s;static RenderTexture rt;static int phase;static double next;static GameObject loading;
 static readonly List<string> checks=new List<string>();
 [Serializable] class Report {public bool passed;public string error;public string[] checks;}
 static RemainingReskinCapture(){EditorApplication.playModeStateChanged+=State;}
 public static void Run(){
  checks.Clear();
  var store=new PlayerStore(Prefix);store.ResetPlayer();store.ResetProfile();store.Save(new PlayerProgress{guideStep=9999,fakeMoney=446.43,gameTotalScore=128,currentLotteryCount=3});store.SaveProfile(new VersionProfile{country="US",cohort="B",rewardedVariant=true});
  SessionState.SetBool(Key,true);SessionState.SetString(Key+"error","");EditorSceneManager.OpenScene("Assets/Scenes/RecoveredMain.unity");var game=UnityEngine.Object.FindObjectOfType<RecoveredGameSession>();game.saveNamespace=Prefix;game.automaticInput=false;EditorApplication.EnterPlaymode();
 }
 static void State(PlayModeStateChange state){
  if(!SessionState.GetBool(Key,false))return;
  if(state==PlayModeStateChange.EnteredPlayMode){phase=0;next=EditorApplication.timeSinceStartup+1;EditorApplication.update+=Tick;Application.logMessageReceived+=Log;}
  if(state==PlayModeStateChange.EnteredEditMode){SessionState.SetBool(Key,false);new PlayerStore(Prefix).ResetPlayer();new PlayerStore(Prefix).ResetProfile();EditorApplication.Exit(SessionState.GetString(Key+"error","").Length==0?0:1);}
 }
 static void Log(string m,string stack,LogType type){if(type==LogType.Error||type==LogType.Exception)SessionState.SetString(Key+"error",m+"\n"+stack);}
 static void Capture(string name){
  foreach(var pop in s.GetComponentsInChildren<RecoveredMenuPopup>())pop.Tick(.4f);
  Canvas.ForceUpdateCanvases();s.worldCamera.Render();var old=RenderTexture.active;RenderTexture.active=rt;var t=new Texture2D(rt.width,rt.height,TextureFormat.RGB24,false);t.ReadPixels(new Rect(0,0,rt.width,rt.height),0,0);t.Apply();Directory.CreateDirectory("../07_Verification/RemainingRuntime");File.WriteAllBytes("../07_Verification/RemainingRuntime/"+name+".png",t.EncodeToPNG());UnityEngine.Object.DestroyImmediate(t);RenderTexture.active=old;
 }
 static void Clear(){s.menus.CloseAll();s.menus.toast.SetActive(false);s.rewardView.gameObject.SetActive(false);s.failView.gameObject.SetActive(false);s.rating.gameObject.SetActive(false);s.guideView.gameObject.SetActive(false);s.wheelRewardView.gameObject.SetActive(false);s.wheelView.gameObject.SetActive(false);}
 static void Reward(int kind){Clear();s.rewardView.Show(kind,kind==4?20:kind==5?10:kind==3?2:4);foreach(var anim in s.rewardView.GetComponentsInChildren<NativeSkeletonPlayer>())if(anim.defaultAnimation!="")anim.EvaluateAt(anim.defaultAnimation,.7f);}
 static void Tick(){
  if(EditorApplication.timeSinceStartup<next)return;next=EditorApplication.timeSinceStartup+.45;
  try{
   var error=SessionState.GetString(Key+"error","");if(error.Length>0)throw new Exception(error);
   switch(phase++){
    case 0:s=UnityEngine.Object.FindObjectOfType<RecoveredGameSession>();rt=new RenderTexture(750,1624,24);s.worldCamera.targetTexture=rt;s.Player.hasSavedGameScene=true;s.Player.savedCoins=new[]{new SavedCoin{value=500,x=-120,y=-400},new SavedCoin{value=100,x=180,y=-380},new SavedCoin{value=20,x=270,y=-500},new SavedCoin{value=5,x=-300,y=-500}};s.Player.savedCurrentCoinValue=5;s.Player.savedNextCoinValue=10;s.board.Initialize(s.Player);next+=2;break;
    case 1:foreach(var c in s.board.Coins)c.Freeze();Time.timeScale=0;s.enabled=false;Clear();s.menus.Act(4);break;
    case 2:Capture("01_coin_locked");s.Player.coin1024Number=50;s.menus.Refresh();break;
    case 3:Capture("01_coin_ready");s.menus.Act(10);break;
    case 4:Capture("02_account_us");Clear();s.menus.Show(8);s.menus.verifyAmount.text="$500";s.menus.verifyCommission.text="Fee: 0";s.menus.verifyCredited.text="Received: $500";s.menus.verification.Begin();Time.timeScale=1;next+=1.8;break;
    case 5:Time.timeScale=0;Capture("05_verification");Clear();s.menus.Show(9);s.menus.stageAmount.text="$500";s.menus.stageHint.text="Watch 50 more ads";s.menus.stagePercent.text="0%";s.menus.stageFill.fillAmount=0;break;
    case 6:Capture("05_next_condition");Clear();s.menus.Show(10);break;
    case 7:Capture("05_daily_limit");Clear();s.menus.Act(1);s.menus.Act(8);break;
    case 8:Capture("06_terms");s.menus.Close();s.menus.Act(7);break;
    case 9:Capture("06_privacy");Clear();s.Player.roundScore=1624;s.Player.histroyMaxScore=2400;s.failView.Show(s.Player);break;
    case 10:Capture("08_game_over");Clear();s.rating.Show();s.rating.Select(3);break;
    case 11:Capture("09_rating");Reward(4);break;
    case 12:Capture("10_highest_reward");Reward(5);break;
    case 13:Capture("10_newbie_reward");Reward(3);break;
    case 14:Capture("10_normal_reward");Reward(2);break;
    case 15:Capture("10_double_reward");Reward(1);break;
    case 16:Capture("14_resurrected");Clear();s.guideView.Show(0,446.43);break;
    case 17:Capture("12_guide_move");s.guideView.Show(1,446.43);break;
    case 18:Capture("12_guide_merge");s.guideView.Show(3,446.43);break;
    case 19:Capture("12_guide_cash");s.guideView.Show(4,446.43);break;
    case 20:Capture("12_guide_coin");Clear();s.menus.ShowToast("Still $53.56 remaining to withdraw");break;
    case 21:Capture("13_toast");Clear();loading=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Startup/RewardedLoading"));foreach(var c in loading.GetComponentsInChildren<Canvas>()){c.renderMode=RenderMode.ScreenSpaceCamera;c.worldCamera=s.worldCamera;c.planeDistance=1;}loading.GetComponent<RecoveredLoadingView>().SetProgress(.4f);break;
    case 22:Capture("07_loading");UnityEngine.Object.DestroyImmediate(loading);Clear();s.failView.Show(s.Player);break;
    case 23:CheckRestart();Clear();s.ChangeProfile("BR","B",true);s.Player.coin1024Number=50;s.Player.raccountName="";s.menus.Act(4);Time.timeScale=1;next+=3.2;break;
    case 24:s.menus.Act(10);break;
    case 25:Time.timeScale=0;s.menus.toast.SetActive(false);Capture("03_account_br");Clear();s.ChangeProfile("ID","B",true);s.Player.coin1024Number=50;s.Player.raccountName="";s.menus.Act(4);Time.timeScale=1;next+=3.2;break;
    case 26:s.menus.Act(10);break;
    case 27:Time.timeScale=0;s.menus.toast.SetActive(false);Capture("04_account_id");Clear();s.ChangeProfile("US","B",true);s.Player.gameTotalScore=128;s.Player.currentLotteryCount=3;WheelReward(true);break;
    case 28:Capture("11_wheel_cash");Clear();WheelReward(false);break;
    case 29:Capture("11_wheel_coin");Finish(null);break;
   }
  }catch(Exception ex){Finish(ex.ToString());}
 }
 static void Check(bool condition,string reason){if(!condition)throw new Exception(reason);checks.Add(reason);}
 static void WheelReward(bool cash){var rewards=s.board.Config.rules.lotteryRewards;for(int i=0;i<rewards.Length;i++)if((rewards[i].type=="money")==cash){s.wheelRewardView.Show(i,s.Player,s.board.Config,s.Locale,.5);return;}throw new Exception("Missing wheel prize kind");}
 static void CheckRestart(){
  var button=s.failView.restart;foreach(var pop in s.failView.GetComponentsInChildren<RecoveredMenuPopup>())pop.Tick(.4f);Canvas.ForceUpdateCanvases();s.worldCamera.Render();
  Check(button.gameObject.activeInHierarchy,"Approved Restart control is visible in the active failure layout");
  Check(button.onClick.GetPersistentEventCount()==0,"Restart retains its existing code-bound Button event");
  var rect=button.targetGraphic.rectTransform;var pointer=new PointerEventData(EventSystem.current){button=PointerEventData.InputButton.Left,position=RectTransformUtility.WorldToScreenPoint(s.worldCamera,rect.TransformPoint(rect.rect.center))};
  var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(pointer,hits);var actual=hits.Count>0?ExecuteEvents.GetEventHandler<IPointerClickHandler>(hits[0].gameObject):null;
  Check(actual==button.gameObject,"Restart receives a real EventSystem pointer through its visible bounds");
  double money=s.Player.fakeMoney;ExecuteEvents.Execute(actual,pointer,ExecuteEvents.pointerClickHandler);
  Check(!s.failView.gameObject.activeSelf&&s.Player.roundScore==0,"Restart closes the failure view and resets the original round state");
  Check(s.Player.fakeMoney==money,"Restart retains the player's cash balance");
 }
 static void Finish(string error){EditorApplication.update-=Tick;Application.logMessageReceived-=Log;Time.timeScale=1;if(s)s.worldCamera.targetTexture=null;if(rt)UnityEngine.Object.DestroyImmediate(rt);SessionState.SetString(Key+"error",error??"");Directory.CreateDirectory("../07_Verification/RemainingRuntime");File.WriteAllText("../07_Verification/RemainingRuntime/capture_status.txt",error??"PASS: actual Unity PlayMode visual-state captures; interaction validation reported separately.");File.WriteAllText("../07_Verification/RemainingRuntime/checks.json",JsonUtility.ToJson(new Report{passed=error==null,error=error,checks=checks.ToArray()},true));Debug.Log(error??"REMAINING_CAPTURE_PASS");EditorApplication.ExitPlaymode();}
}}
