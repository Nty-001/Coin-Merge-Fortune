using System;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
namespace CoinMerge.Recovery.Editor {
[InitializeOnLoad] public static class CoinWithdrawCapture {
 const string Key="CoinMerge.CoinReview",Prefix="coinmerge.coin.review.disposable",Out="../07_Verification/CoinWithdrawReview";
 static RecoveredGameSession s;static RenderTexture rt;static int phase;static double next;static readonly List<string> checks=new List<string>();
 [Serializable] class Report {public bool passed;public string error;public string[] checks;}
 static CoinWithdrawCapture(){EditorApplication.playModeStateChanged+=State;}
 public static void Run(){var store=new PlayerStore(Prefix);store.ResetPlayer();store.ResetProfile();store.Save(new PlayerProgress{guideStep=9999,fakeMoney=446.43,gameTotalScore=128,currentLotteryCount=3});store.SaveProfile(new VersionProfile{country="US",cohort="B",rewardedVariant=true});SessionState.SetBool(Key,true);SessionState.SetString(Key+"error","");EditorSceneManager.OpenScene("Assets/Scenes/RecoveredMain.unity");var game=UnityEngine.Object.FindObjectOfType<RecoveredGameSession>();game.saveNamespace=Prefix;game.automaticInput=false;EditorApplication.EnterPlaymode();}
 static void State(PlayModeStateChange state){if(!SessionState.GetBool(Key,false))return;if(state==PlayModeStateChange.EnteredPlayMode){phase=0;next=EditorApplication.timeSinceStartup+1;checks.Clear();EditorApplication.update+=Tick;Application.logMessageReceived+=Log;}if(state==PlayModeStateChange.EnteredEditMode){SessionState.SetBool(Key,false);new PlayerStore(Prefix).ResetPlayer();new PlayerStore(Prefix).ResetProfile();EditorApplication.Exit(SessionState.GetString(Key+"error","").Length==0?0:1);}}
 static void Log(string m,string stack,LogType t){if(t==LogType.Error||t==LogType.Exception)SessionState.SetString(Key+"error",m+"\n"+stack);}
 static void Size(int w,int h){if(rt){s.worldCamera.targetTexture=null;UnityEngine.Object.DestroyImmediate(rt);}rt=new RenderTexture(w,h,24);s.worldCamera.targetTexture=rt;}
 static void Capture(string name){foreach(var p in s.menus.pages[4].GetComponentsInChildren<RecoveredMenuPopup>())p.Tick(1);Canvas.ForceUpdateCanvases();s.worldCamera.Render();var old=RenderTexture.active;RenderTexture.active=rt;var image=new Texture2D(rt.width,rt.height,TextureFormat.RGB24,false);image.ReadPixels(new Rect(0,0,rt.width,rt.height),0,0);image.Apply();Directory.CreateDirectory(Out);File.WriteAllBytes(Out+"/"+name+".png",image.EncodeToPNG());UnityEngine.Object.DestroyImmediate(image);RenderTexture.active=old;}
 static void Check(bool yes,string message){if(!yes)throw new Exception(message);checks.Add(message);}
 static void Click(Button button){Canvas.ForceUpdateCanvases();s.worldCamera.Render();var rect=(RectTransform)button.transform;var pointer=new PointerEventData(EventSystem.current){button=PointerEventData.InputButton.Left,position=RectTransformUtility.WorldToScreenPoint(s.worldCamera,rect.TransformPoint(rect.rect.center))};var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(pointer,hits);var actual=hits.Count==0?null:ExecuteEvents.GetEventHandler<IPointerClickHandler>(hits[0].gameObject);Check(actual==button.gameObject,"Visible Button receives pointer: "+button.name);Check(button.onClick.GetPersistentEventCount()==0,"Code-bound Button: "+button.name);ExecuteEvents.Execute(actual,pointer,ExecuteEvents.pointerClickHandler);}
 static Button Action(int code){foreach(var a in s.menus.actions)if(a.action==code&&a.button.gameObject.activeInHierarchy)return a.button;throw new Exception("Missing visible action "+code);}
 static void Tick(){if(EditorApplication.timeSinceStartup<next)return;next=EditorApplication.timeSinceStartup+.5;try{var error=SessionState.GetString(Key+"error","");if(error.Length>0)throw new Exception(error);switch(phase++){
  case 0:s=UnityEngine.Object.FindObjectOfType<RecoveredGameSession>();Size(941,1672);s.guideView.gameObject.SetActive(false);s.menus.CloseAll();s.menus.Act(4);next+=.7;break;
  case 1:Time.timeScale=0;s.enabled=false;Capture("01_locked_941x1672");Check(!s.menus.CoinConditionsMet,"Zero coins stays blocked");Check(s.menus.coinPercent.text=="0/50","Progress shows the live 0/50 value");s.Player.coin1024Number=503;s.menus.Refresh();break;
  case 2:Capture("02_ready_503_941x1672");Check(s.menus.coinCount.text=="503"&&s.menus.CoinConditionsMet,"503 coins enables the original withdrawal branch");Size(407,881);break;
  case 3:Capture("03_ready_503_407x881");for(int n=0;n<6;n++){Click(Action(1100+n));Check(s.menus.SelectedCoin==n,"Amount option selected: "+n);}Click(Action(1100));break;
  case 4:Capture("04_reselected_500_407x881");Click(Action(10));Check(s.menus.CurrentPage==5,"Withdraw opens the original account page");s.menus.CloseAll();s.menus.Act(4);break;
  case 5:Click(Action(0));Check(!s.menus.IsOpen,"Back returns to the original game");Finish(null);break;
 }}catch(Exception e){Finish(e.ToString());}}
 static void Finish(string error){EditorApplication.update-=Tick;Application.logMessageReceived-=Log;Time.timeScale=1;if(s)s.worldCamera.targetTexture=null;if(rt)UnityEngine.Object.DestroyImmediate(rt);Directory.CreateDirectory(Out);File.WriteAllText(Out+"/checks.json",JsonUtility.ToJson(new Report{passed=error==null,error=error,checks=checks.ToArray()},true));SessionState.SetString(Key+"error",error??"");Debug.Log(error??"COIN_WITHDRAW_REVIEW_PASS");EditorApplication.ExitPlaymode();}
}}
