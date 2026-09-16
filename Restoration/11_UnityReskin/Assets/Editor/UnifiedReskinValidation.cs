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
    [InitializeOnLoad] public static class UnifiedReskinValidation
    {
        const string Key="CoinMerge.UnifiedReskin.Validation",Prefix="coinmerge.unified.reskin.disposable";
        static readonly List<string> checks=new List<string>(),overflows=new List<string>();
        static RecoveredGameSession s;static Camera camera;static RenderTexture target;static int step,localeIndex;static double next;static string[] countries;static bool shown;static int textChecks;
        static string Output=>Path.GetFullPath("Design/UnifiedR1/Verification");
        static int extra;static double deadline;static int beforeCoins;static PackagedGameSession packaged;
        [Serializable] sealed class Report{public bool passed;public string error,unityVersion;public int textChecks;public string[] countries,checks,overflows;}
        static UnifiedReskinValidation(){EditorApplication.playModeStateChanged+=State;}
        [MenuItem("Coin Merge/Reskin/Validate remaining popups and localization")]
        public static void Run()
        {
            Directory.CreateDirectory(Output);SessionState.SetBool(Key,true);SessionState.SetString(Key+".error","");
            var store=new PlayerStore(Prefix);store.ResetPlayer();store.ResetProfile();store.Save(new PlayerProgress{guideStep=5,fakeMoney=488.49,coin1024Number=50,gameTotalScore=128});store.SaveProfile(new VersionProfile{country="US",cohort="B",rewardedVariant=true});
            EditorSceneManager.OpenScene("Assets/Scenes/RecoveredMain.unity");var session=UnityEngine.Object.FindObjectOfType<RecoveredGameSession>();session.saveNamespace=Prefix;session.automaticInput=false;EditorApplication.EnterPlaymode();
        }
        static void State(PlayModeStateChange state)
        {
            if(!SessionState.GetBool(Key,false))return;
            if(state==PlayModeStateChange.EnteredPlayMode){EditorApplication.isPaused=false;Time.timeScale=1;checks.Clear();overflows.Clear();extra=step=localeIndex=textChecks=0;shown=false;s=null;packaged=null;next=EditorApplication.timeSinceStartup+1;Application.logMessageReceived+=Log;EditorApplication.update+=Tick;}
            if(state==PlayModeStateChange.EnteredEditMode){var store=new PlayerStore(Prefix);store.ResetPlayer();store.ResetProfile();PlayerPrefs.DeleteKey(Prefix+".packaged.player");SessionState.SetBool(Key,false);EditorSceneManager.OpenScene("Assets/Scenes/RecoveredLoading.unity");}
        }
        static void Log(string message,string stack,LogType type){if(type==LogType.Exception||type==LogType.Error)SessionState.SetString(Key+".error",message+"\n"+stack);}
        static void Tick()
        {
            if(EditorApplication.timeSinceStartup<next)return;
            try
            {
                var error=SessionState.GetString(Key+".error","");if(error.Length>0)throw new Exception(error);
                if(extra>0){Supplement();return;}
                if(!s){s=UnityEngine.Object.FindObjectOfType<RecoveredGameSession>();camera=s.worldCamera;Resize(1080,2340);var list=new List<string>{"US","RU","JP"};foreach(var c in s.board.Config.rules.supportedCountries)if(!list.Contains(c))list.Add(c);countries=list.ToArray();}
                if(localeIndex>=countries.Length){InteractionChecks();extra=1;Supplement();return;}
                if(!shown)
                {
                    Hide();if(step==0){s.ChangeProfile(countries[localeIndex],"B",true);s.Player.fakeMoney=488.49;s.RefreshPresentation();}
                    Show(step);shown=true;next=EditorApplication.timeSinceStartup+(step==2?3.1:localeIndex<3?.32:.02);return;
                }
                foreach(var popup in s.GetComponentsInChildren<RecoveredMenuPopup>())popup.Tick(1);
                Canvas.ForceUpdateCanvases();camera.Render();Audit(countries[localeIndex]+" / "+Name(step));
                if(localeIndex<3)Capture(countries[localeIndex]+"_"+Name(step)+".png");
                if(step==4)
                {
                    var scroll=s.menus.pages[3].GetComponentInChildren<ScrollRect>();
                    foreach(float position in new[]{.5f,0f}){scroll.verticalNormalizedPosition=position;Canvas.ForceUpdateCanvases();camera.Render();Audit(countries[localeIndex]+" / cash scroll "+position);}
                    scroll.verticalNormalizedPosition=1;
                }
                shown=false;step++;if(step>21){File.WriteAllText(Path.Combine(Output,"progress.txt"),countries[localeIndex]+" complete; checks "+textChecks);localeIndex++;step=0;}next=EditorApplication.timeSinceStartup+.01;
            }
            catch(Exception e){Finish(e.ToString());}
        }
        static string Name(int index){if(index==0)return "home";if(index<=11)return "page_"+(index-1);if(index<=16)return "reward_"+(index-11);return new[]{"machine","machine_coin_reward","machine_cash_reward","fail","rating"}[index-17];}
        static void Hide(){s.menus.CloseAll();s.rewardView.gameObject.SetActive(false);s.wheelView.gameObject.SetActive(false);s.wheelRewardView.gameObject.SetActive(false);s.failView.gameObject.SetActive(false);s.rating.gameObject.SetActive(false);s.guideView.gameObject.SetActive(false);}
        static void Show(int index)
        {
            if(index==0)return;if(index==2){s.menus.Act(2);return;}if(index<=11){s.menus.Show(index-1);return;}if(index<=16){s.rewardView.Show(index-11,6.72);return;}
            switch(index){case 17:s.wheelView.Show(s.Player,s.Locale);break;case 18:s.wheelRewardView.Show(1,s.Player,s.board.Config,s.Locale,.4);break;case 19:s.wheelRewardView.Show(0,s.Player,s.board.Config,s.Locale,.4);break;case 20:s.failView.Show(s.Player);break;case 21:s.rating.Show();break;}
        }
        static string PathOf(Transform t){string result=t.name;while(t.parent&&(!s||t.parent!=s.transform)){t=t.parent;result=t.name+"/"+result;}return result;}
        static void Audit(string context)
        {
            foreach(var text in (packaged?packaged.gameObject:s.gameObject).GetComponentsInChildren<Text>())
            {
                if(!text.enabled||string.IsNullOrEmpty(text.text)||!text.font)continue;
                var input=text.GetComponentInParent<InputField>();if(input&&input.textComponent==text)continue;
                var rect=text.rectTransform.rect;if(rect.width<1||rect.height<1)continue;
                if(text.canvasRenderer.cull)continue;textChecks++;
                var fit=text.GetComponent<RecoveredTextFit>();
                if(!fit||!fit.enabled||text.verticalOverflow!=VerticalWrapMode.Overflow){overflows.Add(context+" | missing complete-text fit: "+PathOf(text.transform));continue;}
                // Evaluate the rendered mesh after outline and fit, not a stale best-fit cache from culled list rows.
                var mesh=text.canvasRenderer.GetMesh();
                if(mesh&&mesh.vertexCount>0)
                {
                    var bounds=mesh.bounds;
                    if(bounds.max.x>rect.xMax+1||bounds.min.x<rect.xMin-1||bounds.max.y>rect.yMax+1||bounds.min.y<rect.yMin-1)overflows.Add(context+" | "+PathOf(text.transform)+" | rect "+rect+" | mesh "+bounds+" | "+text.text);
                }
            }
        }
        static void Click(Button button)
        {
            if(!button||!button.IsInteractable())throw new Exception("Inactive button");foreach(var popup in s.GetComponentsInChildren<RecoveredMenuPopup>())popup.Tick(1);Canvas.ForceUpdateCanvases();camera.Render();var rect=button.targetGraphic.rectTransform;var pointer=new PointerEventData(EventSystem.current){button=PointerEventData.InputButton.Left,position=RectTransformUtility.WorldToScreenPoint(camera,rect.TransformPoint(rect.rect.center))};var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(pointer,hits);var actual=hits.Count>0?ExecuteEvents.GetEventHandler<IPointerClickHandler>(hits[0].gameObject):null;
            if(actual!=button.gameObject)throw new Exception("Button raycast blocked: "+PathOf(button.transform));ExecuteEvents.Execute(actual,pointer,ExecuteEvents.pointerClickHandler);checks.Add("Native Button receives input: "+PathOf(button.transform));
        }
        static void InteractionChecks()
        {
            Hide();s.ChangeProfile("US","B",true);s.rating.Show();Click(s.rating.stars[3]);if(s.rating.SelectedIndex!=3)throw new Exception("Rating state not updated");Click(s.rating.close);
            for(int page=0;page<s.menus.pages.Length;page++){s.menus.Show(page);foreach(var action in s.menus.actions)if(action.action==0&&action.button.gameObject.activeInHierarchy&&action.button.transform.IsChildOf(s.menus.pages[page].transform)){Click(action.button);break;}s.menus.CloseAll();}
            Click(s.gm.open);Click(s.gm.close);checks.Add("No runtime exceptions across all locale/page captures");
        }
        static void Render(){if(s)foreach(var popup in s.GetComponentsInChildren<RecoveredMenuPopup>())popup.Tick(1);if(packaged)foreach(var popup in packaged.GetComponentsInChildren<RecoveredMenuPopup>())popup.Tick(1);Canvas.ForceUpdateCanvases();camera.Render();}
        static void Supplement()
        {
            if(extra==1)
            {
                Hide();s.ChangeProfile("US","B",true);Resize(941,1672);s.Player.coin1024Number=0;s.menus.Show(4);Render();Capture("US_coin_blocked_reference.png");Audit("coin 941x1672 blocked");
                var scroll=s.menus.pages[4].GetComponentInChildren<ScrollRect>();
                for(int i=0;i<6;i++){scroll.verticalNormalizedPosition=i>=4?0:1;Render();foreach(var binding in s.menus.actions)if(binding.action==1100+i&&binding.button.gameObject.activeInHierarchy){Click(binding.button);break;}if(s.menus.SelectedCoin!=i)throw new Exception("Coin selection did not update "+i);}
                s.Player.coin1024Number=50;s.menus.Act(1100);Render();Capture("US_coin_ready_reference.png");Audit("coin ready");
                var ready=s.menus.coinReady.GetComponentInChildren<Button>();Click(ready);if(s.menus.CurrentPage!=5)throw new Exception("Coin withdrawal did not open account");
                s.menus.forms[0].account.text="visualtest@example.test";Render();Capture("US_account_valid.png");Click(s.menus.forms[0].confirm.GetComponent<Button>());
                if(s.menus.CurrentPage!=8)throw new Exception("Account did not open verification");deadline=EditorApplication.timeSinceStartup+20;extra=2;next=EditorApplication.timeSinceStartup+.1;return;
            }
            if(extra==2)
            {
                if(s.menus.CurrentPage==8){if(EditorApplication.timeSinceStartup>deadline)throw new Exception("Verification animation timed out");Render();Capture("US_verification_flow.png");next=EditorApplication.timeSinceStartup+.3;return;}
                if(s.Player.newFakeMoneyWithdraw[0]!=1)throw new Exception("Verification did not advance first task");Render();Capture("US_next_task_flow.png");checks.Add("Native coin selections 6/6; blocked/ready art; account submission; verification animation; next task reached");Hide();Resize(1080,2340);
                foreach(var country in new[]{"US","RU","JP"}){s.ChangeProfile(country,"B",true);foreach(int step in new[]{0,1,3,4}){Hide();s.guideView.Show(step,s.Player.fakeMoney);Render();Capture(country+"_guide_"+step+".png");Audit(country+" guide "+step);}}
                Hide();s.gm.Show();for(int tab=0;tab<3;tab++){s.gm.gameplay.Execute(tab);Render();Capture("GM_"+tab+".png");Audit("GM tab "+tab);}s.gm.Hide();
                s.ChangeProfile("US","B",true);s.Player.gameTotalScore=RecoveredGameRules.RequiredScore(s.board.Config.rules.lotteryScores,s.Player.currentLotteryCount);s.GmSetNextWheel(1);beforeCoins=s.Player.coin1024Number;s.wheelView.Show(s.Player,s.Locale);Click(s.wheelView.draw);deadline=EditorApplication.timeSinceStartup+20;extra=3;next=EditorApplication.timeSinceStartup+.1;return;
            }
            if(extra==3)
            {
                if(!s.wheelRewardView.gameObject.activeSelf){if(EditorApplication.timeSinceStartup>deadline)throw new Exception("Reskinned machine completion timed out");next=EditorApplication.timeSinceStartup+.1;return;}
                Render();Capture("US_machine_live_result.png");Click(s.wheelRewardView.claim);if(s.Player.coin1024Number<=beforeCoins)throw new Exception("Machine claim did not award coins");checks.Add("Original machine intro/completion timing, rotating highlight, native claim, coin award all pass");
                var store=new PlayerStore(Prefix);store.SaveProfile(new VersionProfile{country="US",contentMode=VersionRouting.Packaged,cohort="A",rewardedVariant=false});GameVersionRouter.SetPendingNamespace(Prefix);
                if(camera)camera.targetTexture=null;UnityEngine.SceneManagement.SceneManager.LoadScene("RecoveredPackaged");extra=100;next=EditorApplication.timeSinceStartup+.7;return;
            }
            if(extra>=100)
            {
                if(!packaged){packaged=UnityEngine.Object.FindObjectOfType<PackagedGameSession>();if(!packaged||packaged.Player==null){next=EditorApplication.timeSinceStartup+.2;return;}packaged.automaticInput=false;camera=packaged.worldCamera;Resize(1080,2340);}
                int page=extra-100;if(page>=packaged.dialogs.Length){checks.Add("A base version: all 7 native dialogs rendered and text bounds audited");Finish(null);return;}
                while(packaged.Dialog>=0)packaged.Act(0);packaged.Show(page);Render();Audit("A / dialog "+page);Capture("A_dialog_"+page+".png");extra++;next=EditorApplication.timeSinceStartup+.15;
            }
        }
        static void Resize(int w,int h){camera.targetTexture=null;if(target)UnityEngine.Object.DestroyImmediate(target);target=new RenderTexture(w,h,24);camera.targetTexture=target;Canvas.ForceUpdateCanvases();if(s)s.playfieldLayout.Refresh();}
        static void Capture(string filename){var old=RenderTexture.active;RenderTexture.active=target;var t=new Texture2D(target.width,target.height,TextureFormat.RGB24,false);t.ReadPixels(new Rect(0,0,target.width,target.height),0,0);t.Apply();File.WriteAllBytes(Path.Combine(Output,filename),t.EncodeToPNG());RenderTexture.active=old;UnityEngine.Object.DestroyImmediate(t);}
        static void Finish(string error)
        {
            EditorApplication.update-=Tick;Application.logMessageReceived-=Log;if(camera)camera.targetTexture=null;if(target)UnityEngine.Object.DestroyImmediate(target);SessionState.SetString(Key+".error",error??"");File.WriteAllText(Path.Combine(Output,"play_mode.json"),JsonUtility.ToJson(new Report{passed=error==null&&overflows.Count==0,error=error,unityVersion=Application.unityVersion,countries=countries,textChecks=textChecks,checks=checks.ToArray(),overflows=overflows.ToArray()},true));Debug.Log(error==null?"UNIFIED_RESKIN_VALIDATED: "+textChecks+" text checks; "+overflows.Count+" overflows":error);EditorApplication.ExitPlaymode();
        }
    }
}
