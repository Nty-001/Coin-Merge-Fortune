using System;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace CoinMerge.Recovery.Editor {
[InitializeOnLoad] public static class ReskinRuntimeCapture {
 const string Key="CoinMerge.ReskinCapture",Prefix="coinmerge.reskin.capture.disposable";
 static RecoveredGameSession s;static RenderTexture rt;static int phase;static double next,deadline;
 static readonly List<string> checks=new List<string>();
 [Serializable] class Report {public bool passed;public string error;public string[] checks;}
 static ReskinRuntimeCapture(){EditorApplication.playModeStateChanged+=State;}
 public static void Run(){
  checks.Clear();var store=new PlayerStore(Prefix);store.ResetPlayer();store.ResetProfile();
  store.Save(new PlayerProgress {guideStep=9999,fakeMoney=446.43,gameTotalScore=128,currentLotteryCount=3});
  store.SaveProfile(new VersionProfile{country="US",cohort="B",rewardedVariant=true});
  SessionState.SetBool(Key,true);SessionState.SetString(Key+"error","");
  EditorSceneManager.OpenScene("Assets/Scenes/RecoveredMain.unity");
  var game=UnityEngine.Object.FindObjectOfType<RecoveredGameSession>();game.saveNamespace=Prefix;game.automaticInput=false;
  EditorApplication.EnterPlaymode();
 }
 static void State(PlayModeStateChange state){
  if(!SessionState.GetBool(Key,false))return;
  if(state==PlayModeStateChange.EnteredPlayMode){phase=0;next=EditorApplication.timeSinceStartup+1;deadline=next+80;EditorApplication.update+=Tick;Application.logMessageReceived+=Log;}
  if(state==PlayModeStateChange.EnteredEditMode){SessionState.SetBool(Key,false);new PlayerStore(Prefix).ResetPlayer();new PlayerStore(Prefix).ResetProfile();EditorApplication.Exit(SessionState.GetString(Key+"error","").Length==0?0:1);}
 }
 static void Log(string m,string st,LogType t){if(t==LogType.Exception||t==LogType.Error)SessionState.SetString(Key+"error",m+"\n"+st);}
 static void Check(bool yes,string reason){if(!yes)throw new Exception(reason);checks.Add(reason);}
 static void Capture(string name){
  Canvas.ForceUpdateCanvases();s.worldCamera.Render();var old=RenderTexture.active;RenderTexture.active=rt;
  var t=new Texture2D(rt.width,rt.height,TextureFormat.RGB24,false);t.ReadPixels(new Rect(0,0,rt.width,rt.height),0,0);t.Apply();
  Directory.CreateDirectory("../07_Verification/ReskinRuntime");File.WriteAllBytes("../07_Verification/ReskinRuntime/"+name+".png",t.EncodeToPNG());
  UnityEngine.Object.DestroyImmediate(t);RenderTexture.active=old;
 }
 static void Tick(){
  if(EditorApplication.timeSinceStartup<next)return;next=EditorApplication.timeSinceStartup+.5;
  try{
   if(EditorApplication.timeSinceStartup>deadline)throw new Exception("Capture timeout");
   var e=SessionState.GetString(Key+"error","");if(e.Length>0)throw new Exception(e);
   switch(phase++){
    case 0:
     s=UnityEngine.Object.FindObjectOfType<RecoveredGameSession>();rt=new RenderTexture(750,1624,24);s.worldCamera.targetTexture=rt;
     s.guideView.gameObject.SetActive(false);s.Player.hasSavedGameScene=true;
     s.Player.savedCoins=new[]{new SavedCoin{value=500,x=-110,y=-285},new SavedCoin{value=100,x=170,y=-280},new SavedCoin{value=20,x=-280,y=-350},new SavedCoin{value=10,x=255,y=-435},new SavedCoin{value=5,x=-280,y=-465},new SavedCoin{value=2,x=85,y=-480},new SavedCoin{value=20,x=35,y=-465},new SavedCoin{value=1,x=180,y=-490},new SavedCoin{value=10,x=-240,y=-100},new SavedCoin{value=5,x=0,y=-100},new SavedCoin{value=20,x=255,y=-80}};
     s.Player.savedCurrentCoinValue=5;s.Player.savedNextCoinValue=10;s.board.Initialize(s.Player);next=EditorApplication.timeSinceStartup+2;break;
    case 1:
     foreach(var coin in s.board.Coins)coin.Freeze();Check(s.board.coinPrefab.GetComponent<NativeMergeCoin>()!=null,"World-space native coin prefab retained");
     foreach(var c in s.board.Config.rules.coins)Check(Resources.Load<Sprite>(c.spritePath)!=null,"Coin sprite loads: "+c.value);
     Capture("01_home");s.menus.Act(1);break;
    case 2:
     Check(s.menus.CurrentPage==0,"Settings opens through existing action");Capture("04_settings_on");s.menus.Act(5);break;
    case 3:
     Check(!s.Player.open_bgm,"Music switch changes actual player state");Capture("04_settings_off");s.menus.Act(5);s.menus.CloseAll();s.menus.Act(2);next=EditorApplication.timeSinceStartup+s.menus.ruleAnimation.Data.animations[0].duration+.5;break;
    case 4:
     Capture("05_rules_complete");Check(s.menus.CurrentPage==1,"Rules displays existing animated synthesis order");s.menus.CloseAll();s.menus.Act(3);break;
    case 5:
     Capture("03_cash");Check(s.menus.fakeRows.Length==6,"Six live withdrawal products retained");s.menus.CloseAll();
     s.Player.gameTotalScore=500;s.ChangeProfile("US","B",true);s.wheelButton.onClick.Invoke();next=EditorApplication.timeSinceStartup+s.board.Config.rules.flow.popupDelay+1;break;
    case 6:
     Check(s.wheelView.gameObject.activeSelf,"Score refresh opens wheel through original delayed popup flow");Capture("02_wheel");s.wheelView.draw.onClick.Invoke();next=EditorApplication.timeSinceStartup+.6;break;
    case 7:
     Check(s.wheelView.IsSpinning,"Draw Button starts actual original animation");Capture("02_wheel_spinning");next=EditorApplication.timeSinceStartup+9;break;
    case 8:
     Check(!s.wheelView.IsSpinning,"Original draw animation and result sequence finishes");Capture("02_wheel_result");Finish(null);break;
   }
  }catch(Exception ex){Finish(ex.ToString());}
 }
 static void Finish(string error){
  EditorApplication.update-=Tick;Application.logMessageReceived-=Log;
  if(s)s.worldCamera.targetTexture=null;if(rt)UnityEngine.Object.DestroyImmediate(rt);
  SessionState.SetString(Key+"error",error??"");Directory.CreateDirectory("../07_Verification/ReskinRuntime");
  File.WriteAllText("../07_Verification/ReskinRuntime/checks.json",JsonUtility.ToJson(new Report{passed=error==null,error=error,checks=checks.ToArray()},true));
  Debug.Log(error??"RESKIN_RUNTIME_CAPTURE_PASS");EditorApplication.ExitPlaymode();
 }
}}
