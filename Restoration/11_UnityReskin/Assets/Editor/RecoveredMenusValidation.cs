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
    [InitializeOnLoad] public static class RecoveredMenusValidation
    {
        const string Key="CoinMerge.MenuValidation",Prefix="coinmerge.menu.validation.disposable";
        static RecoveredGameSession session;static RecoveredMainMenus menus;static Camera camera;static RenderTexture target;
        static readonly List<string> checks=new List<string>();static int phase;static double next,deadline;
        [Serializable] sealed class Report{public bool passed;public string error,unityVersion;public string[] checks;}
        static RecoveredMenusValidation(){EditorApplication.playModeStateChanged+=State;}
        public static void Run()
        {
            WithdrawalRulesValidation.Run();
            SessionState.SetBool(Key,true);SessionState.SetString(Key+".error","");
            var store=new PlayerStore(Prefix);store.ResetPlayer();store.ResetProfile();store.Save(new PlayerProgress {guideStep=5});
            EditorSceneManager.OpenScene("Assets/Scenes/RecoveredMain.unity");var s=UnityEngine.Object.FindObjectOfType<RecoveredGameSession>();s.saveNamespace=Prefix;s.automaticInput=false;
            EditorApplication.EnterPlaymode();
        }
        public static void AuthorAndRun(){RecoveredMainMenusBuilder.Run();Run();}
        static void State(PlayModeStateChange state)
        {
            if(!SessionState.GetBool(Key,false))return;
            if(state==PlayModeStateChange.EnteredPlayMode){checks.Clear();phase=0;next=EditorApplication.timeSinceStartup+1;Application.logMessageReceived+=Log;EditorApplication.update+=Tick;}
            if(state==PlayModeStateChange.EnteredEditMode){new PlayerStore(Prefix).ResetPlayer();new PlayerStore(Prefix).ResetProfile();SessionState.SetBool(Key,false);EditorApplication.Exit(SessionState.GetString(Key+".error","").Length==0?0:1);}
        }
        static void Log(string value,string stack,LogType kind){if(kind==LogType.Exception||kind==LogType.Error)SessionState.SetString(Key+".error",value+"\n"+stack);}
        static void Require(bool value,string reason){if(!value)throw new Exception(reason);checks.Add(reason);}
        static Button Action(int action)
        {
            foreach(var binding in menus.actions)
                if(binding.action==action&&binding.button.gameObject.activeInHierarchy&&
                    (action>=1&&action<=4||menus.CurrentPage<0||binding.button.transform.IsChildOf(menus.pages[menus.CurrentPage].transform)))return binding.button;
            throw new Exception("Missing visible action "+action+" page "+menus.CurrentPage);
        }
        static void Click(int action)=>Click(Action(action));
        static void Click(Button button)
        {
            Require(button.gameObject.activeInHierarchy&&button.targetGraphic.enabled&&button.IsInteractable(),"Interactive visible Button "+button.name);
            Canvas.ForceUpdateCanvases();camera.Render();var r=button.targetGraphic.rectTransform;
            var pointer=new PointerEventData(EventSystem.current){button=PointerEventData.InputButton.Left,position=RectTransformUtility.WorldToScreenPoint(camera,r.TransformPoint(r.rect.center))};
            var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(pointer,hits);
            var actual=hits.Count>0?ExecuteEvents.GetEventHandler<IPointerClickHandler>(hits[0].gameObject):null;
            if(actual!=button.gameObject)throw new Exception("Pointer blocked: "+button.name+" by "+(hits.Count>0?hits[0].gameObject.name:"none")+" at "+pointer.position+" rect "+r.rect+" depth "+button.targetGraphic.depth+" raycast "+button.targetGraphic.raycastTarget+" culled "+button.targetGraphic.canvasRenderer.cull+" canvas "+button.targetGraphic.canvas.name);
            ExecuteEvents.Execute(actual,pointer,ExecuteEvents.pointerDownHandler);ExecuteEvents.Execute(actual,pointer,ExecuteEvents.pointerUpHandler);ExecuteEvents.Execute(actual,pointer,ExecuteEvents.pointerClickHandler);
        }
        static void Tick()
        {
            if(EditorApplication.timeSinceStartup<next)return;next=EditorApplication.timeSinceStartup+.5;
            try
            {
                string error=SessionState.GetString(Key+".error","");if(error.Length>0)throw new Exception(error);
                switch(phase++)
                {
                    case 0:
                        session=UnityEngine.Object.FindObjectOfType<RecoveredGameSession>();menus=session.menus;camera=session.worldCamera;target=new RenderTexture(750,1624,24);camera.targetTexture=target;
                        Require(menus&&menus.pages.Length==11,"Eleven original menu pages authored and wired");
                        foreach(var binding in menus.actions)Require(binding.button&&binding.button.targetGraphic&&binding.button.onClick.GetPersistentEventCount()==0,"Code-bound visible Button "+binding.action);
                        Click(1);break;
                    case 1:
                        Require(menus.CurrentPage==0&&session.board.InputBlocked,"Settings opens and blocks board input");Capture("menus_settings.png");Click(5);Require(!session.Player.open_bgm&&!session.Player.open_music&&!menus.audioCues.music.isPlaying,"Music off persists and stops AudioSource");
                        Click(5);Require(session.Player.open_bgm&&menus.audioCues.music.clip,"Music on loads original bgm");Click(6);Require(!new PlayerStore(Prefix).LoadPlayer().open_vibrate,"Vibration choice saved");Click(8);break;
                    case 2:
                        Require(menus.CurrentPage==2&&menus.termsScroll.activeSelf&&!menus.privacyScroll.activeSelf,"Terms opens original scroll page");Capture("menus_terms.png");
                        var scroll=menus.termsScroll.GetComponent<ScrollRect>();scroll.verticalNormalizedPosition=.4f;Require(scroll.content.anchoredPosition.y>0,"Native terms ScrollRect moves long content");Click(0);Click(7);break;
                    case 3:
                        Require(menus.privacyScroll.activeSelf&&!menus.termsScroll.activeSelf,"Privacy opens separate recovered document");Capture("menus_privacy.png");Click(0);Click(0);Click(2);break;
                    case 4:
                        Require(menus.CurrentPage==1&&menus.ruleAnimation.gameObject.activeInHierarchy,"Rules opens native skeletal explanation");Capture("menus_rules.png");Click(0);Click(3);break;
                    case 5:
                        Require(menus.CurrentPage==3&&menus.fakeRows.Length==6,"Top Withdraw opens six original balance products");Capture("menus_cash.png");Click(9);Require(menus.toast.activeSelf&&menus.CurrentPage==3,"Insufficient balance shows original condition toast");
                        var cashScroll=menus.pages[3].GetComponentInChildren<ScrollRect>();cashScroll.verticalNormalizedPosition=0;next=EditorApplication.timeSinceStartup+.6;break;
                    case 6:
                        Click(1005);Require(menus.SelectedCash==5,"Scrolled sixth cash product receives actual pointer click");Capture("menus_cash_last.png");Click(0);Click(4);break;
                    case 7:
                        Require(menus.CurrentPage==4&&!menus.CoinConditionsMet&&menus.coinBlocked.activeSelf,"2000 coin button opens separate gated coin withdrawal page");Capture("menus_coin_locked.png");Click(14);Require(menus.CurrentPage==4,"Original gray button keeps current page");
                        Click(1105);Require(menus.SelectedCoin==5,"Sixth coin product pointer selection");Click(1100);session.Player.coin1024Number=50;menus.Refresh();break;
                    case 8:
                        Require(menus.CoinConditionsMet&&menus.coinReady.activeSelf,"Original merge requirement enables withdrawal");Click(10);break;
                    case 9:
                        Require(menus.CurrentPage==5,"US withdrawal routes to email account form");Capture("menus_email.png");menus.forms[0].account.text="invalid";Click(11);Require(menus.CurrentPage==5&&menus.toast.activeSelf,"Invalid account stays on form with original hint");
                        menus.forms[0].account.text="Fixture@Example.Test";Click(11);Require(menus.CurrentPage==8&&session.Player.raccountName=="fixture@example.test","Valid synthetic account persists normalized email and enters verification");deadline=EditorApplication.timeSinceStartup+15;next=EditorApplication.timeSinceStartup+1.8;break;
                    case 10:
                        Capture("menus_verification.png");break;
                    case 11:
                        if(menus.CurrentPage==8){if(EditorApplication.timeSinceStartup>deadline)throw new Exception("Native verification sequence did not finish");phase--;break;}
                        Require(menus.CurrentPage==9&&session.Player.newFakeMoneyWithdraw[0]==1,"Native animation completion advances to first unmet ad-count stage");Capture("menus_next_condition.png");Require(session.Player.coin1024Number==50&&session.Player.fakeMoney==0,"Stage progression does not deduct balances, as original active flow");Click(0);
                        session.Player.watch_video_count=200000;session.Player.fakeMoney=100000;session.Player.loginDays=30;menus.Refresh();Click(10);deadline=EditorApplication.timeSinceStartup+15;break;
                    case 12:
                        if(menus.CurrentPage==8){if(EditorApplication.timeSinceStartup>deadline)throw new Exception("Second verification stalled");phase--;break;}
                        Require(menus.CurrentPage==10&&session.Player.newFakeMoneyWithdraw[0]==5,"All five conditions reach original active-tips result");Capture("menus_active_tips.png");Click(0);Click(0);Click(3);break;
                    case 13:
                        Click(9);Require(menus.CurrentPage==3,"Balance withdrawal retains its distinct 100-merge gate after coin withdrawal's 50-merge gate passed");
                        session.Player.coin1024Number=100;menus.Refresh();Click(9);Require(menus.CurrentPage==10,"Balance withdrawal reaches original active-tips when all its own conditions are met");menus.CloseAll();session.ChangeProfile("BR","B",true);session.Player.raccountName="";session.Player.rfullName="";session.Player.rdocumentId="";session.Player.newFakeMoneyWithdraw[0]=0;Click(4);break;
                    case 14:Click(10);break;
                    case 15:
                        Require(menus.CurrentPage==6&&session.Player.realSelectPlatform=="Pagbank","Brazil selects recovered Pagbank default and full-name/CPF form");Capture("menus_brazil.png");
                        menus.forms[1].account.text="fixture@example.test";menus.forms[1].fullName.text="Test Player";menus.forms[1].taxId.text="00000000000";Click(12);Require(menus.CurrentPage==8&&session.Player.rdocumentId=="00000000000","Synthetic BR account passes original format validators");menus.CloseAll();
                        session.ChangeProfile("ID","B",true);session.Player.raccountName="";session.Player.rfullName="";session.Player.newFakeMoneyWithdraw[0]=0;Click(4);next=EditorApplication.timeSinceStartup+3.2;break;
                    case 16:Click(10);break;
                    case 17:
                        Require(menus.CurrentPage==7&&session.Player.realSelectPlatform=="DANA","Indonesia selects original DANA phone form");Capture("menus_indonesia.png");menus.forms[2].account.text="08123456789";menus.forms[2].fullName.text="Test Player";Click(13);Require(menus.CurrentPage==8,"Phone form continues through the same original validation sequence");menus.CloseAll();
                        session.ChangeProfile("US","B",true);session.Player.gameTotalScore=0;Click(session.wheelButton);Require(menus.toast.activeSelf,"Wheel insufficient-points Button displays original toast");next=EditorApplication.timeSinceStartup+.2;break;
                    case 18:
                        Require(!menus.IsOpen&&!session.board.InputBlocked,"Closing all menus restores board input");Finish(null);break;
                }
            }
            catch(Exception e){Finish(e.ToString());}
        }
        static void Capture(string name)
        {
            Canvas.ForceUpdateCanvases();camera.Render();var previous=RenderTexture.active;RenderTexture.active=target;var texture=new Texture2D(750,1624,TextureFormat.RGB24,false);
            texture.ReadPixels(new Rect(0,0,750,1624),0,0);texture.Apply();File.WriteAllBytes("../07_Verification/"+name,texture.EncodeToPNG());RenderTexture.active=previous;UnityEngine.Object.DestroyImmediate(texture);
        }
        static void Finish(string error)
        {
            EditorApplication.update-=Tick;Application.logMessageReceived-=Log;if(camera)camera.targetTexture=null;if(target)UnityEngine.Object.DestroyImmediate(target);
            SessionState.SetString(Key+".error",error??"");File.WriteAllText("../07_Verification/recovered_menus_validation.json",JsonUtility.ToJson(new Report {passed=error==null,error=error,unityVersion=Application.unityVersion,checks=checks.ToArray()},true));
            Debug.Log(error==null?"RECOVERED_MENUS_VALIDATED "+checks.Count:error);EditorApplication.ExitPlaymode();
        }
    }
}
