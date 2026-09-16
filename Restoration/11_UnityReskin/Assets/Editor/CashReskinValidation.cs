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
    [InitializeOnLoad] public static class CashReskinValidation
    {
        const string Key="CoinMerge.CashReskin.Validation",Prefix="coinmerge.cash.reskin.disposable";
        static readonly List<string> checks=new List<string>();
        static RecoveredGameSession session;static Camera camera;static RenderTexture target;static int phase;static double next,deadline;
        static string Output=>Path.GetFullPath("Design/CashR1/Verification");
        [Serializable] sealed class Report{public bool passed;public string error,unityVersion;public string[] checks;}
        static CashReskinValidation(){EditorApplication.playModeStateChanged+=State;}
        [MenuItem("Coin Merge/Reskin/Validate reference Cash withdrawal")]
        public static void Run()
        {
            Directory.CreateDirectory(Output);SessionState.SetBool(Key,true);SessionState.SetString(Key+".error","");
            var store=new PlayerStore(Prefix);store.ResetPlayer();store.ResetProfile();
            store.Save(new PlayerProgress{guideStep=5,fakeMoney=488.49});
            store.SaveProfile(new VersionProfile{country="US",cohort="B",rewardedVariant=true});
            EditorSceneManager.OpenScene("Assets/Scenes/RecoveredMain.unity");
            var s=UnityEngine.Object.FindObjectOfType<RecoveredGameSession>();s.saveNamespace=Prefix;s.automaticInput=false;EditorApplication.EnterPlaymode();
        }
        static void State(PlayModeStateChange state)
        {
            if(!SessionState.GetBool(Key,false))return;
            if(state==PlayModeStateChange.EnteredPlayMode){checks.Clear();phase=0;next=EditorApplication.timeSinceStartup+1;deadline=next+90;Application.logMessageReceived+=Log;EditorApplication.update+=Tick;}
            if(state==PlayModeStateChange.EnteredEditMode){var store=new PlayerStore(Prefix);store.ResetPlayer();store.ResetProfile();SessionState.SetBool(Key,false);EditorSceneManager.OpenScene("Assets/Scenes/RecoveredLoading.unity");if(Application.isBatchMode)EditorApplication.Exit(SessionState.GetString(Key+".error","").Length==0?0:1);}
        }
        static void Log(string m,string s,LogType t){if(t==LogType.Exception||t==LogType.Error)SessionState.SetString(Key+".error",m+"\n"+s);}
        static void Require(bool condition,string label){if(!condition)throw new Exception(label);checks.Add(label);}
        static Button Button(int action,int page=-1)
        {foreach(var binding in session.menus.actions)if(binding.action==action&&binding.button.gameObject.activeInHierarchy&&(page<0||binding.button.transform.IsChildOf(session.menus.pages[page].transform)))return binding.button;throw new Exception("Missing visible Button "+action);}
        static void Click(Button button)
        {
            Require(button.IsInteractable(),"Interactable "+button.name);Require(button.onClick.GetPersistentEventCount()==0,"Code-bound "+button.name);
            Canvas.ForceUpdateCanvases();camera.Render();var r=button.targetGraphic.rectTransform;
            var pointer=new PointerEventData(EventSystem.current){button=PointerEventData.InputButton.Left,position=RectTransformUtility.WorldToScreenPoint(camera,r.TransformPoint(r.rect.center))};
            var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(pointer,hits);
            var actual=hits.Count>0?ExecuteEvents.GetEventHandler<IPointerClickHandler>(hits[0].gameObject):null;
            Require(actual==button.gameObject,"Visible graphic receives click: "+button.name);
            ExecuteEvents.Execute(actual,pointer,ExecuteEvents.pointerDownHandler);ExecuteEvents.Execute(actual,pointer,ExecuteEvents.pointerUpHandler);ExecuteEvents.Execute(actual,pointer,ExecuteEvents.pointerClickHandler);
        }
        static void Tick()
        {
            if(EditorApplication.timeSinceStartup<next)return;next=EditorApplication.timeSinceStartup+.7;
            try
            {
                if(EditorApplication.timeSinceStartup>deadline)throw new Exception("Cash validation timeout");
                string error=SessionState.GetString(Key+".error","");if(error.Length>0)throw new Exception(error);
                switch(phase++)
                {
                    case 0:session=UnityEngine.Object.FindObjectOfType<RecoveredGameSession>();camera=session.worldCamera;Resize(941,1672);Click(Button(3));break;
                    case 1:
                        Require(session.menus.CurrentPage==3,"Cash page opens through main native Button");ValidateShapes();
                        Require(Math.Abs(session.menus.fakeRows[0].progress-488.49/500)<.0001,"First progress uses live balance / 500");
                        Require(Math.Abs(session.menus.fakeRows[1].progress-488.49/800)<.0001,"Second progress uses live balance / 800");
                        Capture("cash_reference_941x1672.png");Click(Button(1001,3));break;
                    case 2:
                        Require(session.menus.SelectedCash==1&&session.menus.fakeRows[1].selected.activeSelf&&!session.menus.fakeRows[0].selected.activeSelf,"Native row selection moves green card and check");
                        Capture("cash_selected_800.png");Click(Button(9,3));Require(session.menus.CurrentPage==3&&session.menus.toast.activeInHierarchy,"Insufficient balance shows original remaining-condition toast");break;
                    case 3:
                        Resize(1080,2340);Click(Button(1000,3));break;
                    case 4:
                        ValidateShapes();Capture("cash_1080x2340.png");var scroll=session.menus.pages[3].GetComponentInChildren<ScrollRect>();
                        var pointer=new PointerEventData(EventSystem.current){scrollDelta=new Vector2(0,-60),position=RectTransformUtility.WorldToScreenPoint(camera,scroll.transform.position)};
                        ExecuteEvents.Execute(scroll.gameObject,pointer,ExecuteEvents.scrollHandler);break;
                    case 5:
                        var sr=session.menus.pages[3].GetComponentInChildren<ScrollRect>();Require(sr.verticalNormalizedPosition<.1f,"Native ScrollRect scroll reaches final cash tiers");Capture("cash_scrolled.png");
                        sr.verticalNormalizedPosition=1;string oldCondition=session.menus.fakeRows[0].condition.text;session.Player.fakeMoney=500;session.menus.Refresh();Require(session.menus.fakeRows[0].condition.text!=oldCondition,"Reaching balance advances original task stage");Click(Button(0,3));session.ChangeProfile("RU","B",true);Click(Button(3));break;
                    case 6:
                        Require(session.menus.fakeBalance.text==session.Locale.Money(session.Player.fakeMoney),"Russian locale preserves dynamic currency formatting");Capture("cash_ru.png");Click(Button(0,3));session.ChangeProfile("JP","B",true);Click(Button(3));break;
                    case 7:
                        Capture("cash_jp.png");Click(Button(0,3));session.ChangeProfile("US","B",true);
                        var product=RecoveredGameRules.CashConfiguration(session.board.Config.rules,"US").real_products[0];
                        session.Player.fakeMoney=product.withdrawAmount;session.Player.coin1024Number=product.condition_merge;session.Player.loginDays=product.condition_login_days;session.Player.watch_video_count=product.condition_video;Click(Button(3));break;
                    case 8:
                        Require(session.menus.fakeRows[0].progress==1,"Original cash task completion produces full progress");Click(Button(9,3));break;
                    case 9:
                        Require(session.menus.CurrentPage==10,"Eligible Withdraw continues to existing activation flow");Capture("cash_existing_next_step.png");Click(Button(0,10));Click(Button(0,3));Require(!session.menus.IsOpen,"Back returns to gameplay");Click(session.gm.open);break;
                    case 10:
                        Require(session.gm.IsOpen,"GM still opens after cash page closes");Click(session.gm.close);Require(!session.gm.IsOpen,"GM closes through native Button");Finish(null);break;
                }
            }
            catch(Exception e){Finish(e.ToString());}
        }
        static void ValidateShapes()
        {
            foreach(var button in session.menus.pages[3].GetComponentsInChildren<Button>())
            {
                Require(button.targetGraphic.gameObject==button.gameObject,"Button visual and click target share native object");var image=button.targetGraphic as Image;if(!image)continue;
                Require(image.sprite&&image.preserveAspect,"Button uses imported sprite with preserved aspect");
                Require(Mathf.Abs(image.sprite.rect.width/image.sprite.rect.height-image.rectTransform.rect.width/image.rectTransform.rect.height)<.01f,"Button rect matches source aspect: "+button.name);
            }
        }
        static void Resize(int w,int h){camera.targetTexture=null;if(target)UnityEngine.Object.DestroyImmediate(target);target=new RenderTexture(w,h,24);camera.targetTexture=target;Canvas.ForceUpdateCanvases();session.playfieldLayout.Refresh();}
        static void Capture(string name)
        {
            Canvas.ForceUpdateCanvases();camera.Render();var old=RenderTexture.active;RenderTexture.active=target;
            var image=new Texture2D(target.width,target.height,TextureFormat.RGB24,false);image.ReadPixels(new Rect(0,0,target.width,target.height),0,0);image.Apply();File.WriteAllBytes(Path.Combine(Output,name),image.EncodeToPNG());RenderTexture.active=old;UnityEngine.Object.DestroyImmediate(image);
        }
        static void Finish(string error)
        {
            EditorApplication.update-=Tick;Application.logMessageReceived-=Log;if(camera)camera.targetTexture=null;if(target)UnityEngine.Object.DestroyImmediate(target);
            SessionState.SetString(Key+".error",error??"");File.WriteAllText(Path.Combine(Output,"play_mode.json"),JsonUtility.ToJson(new Report{passed=error==null,error=error,unityVersion=Application.unityVersion,checks=checks.ToArray()},true));Debug.Log(error==null?"CASH_RESKIN_VALIDATED "+checks.Count:error);EditorApplication.ExitPlaymode();
        }
    }
}
