using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
namespace CoinMerge.Recovery.Editor
{
    [InitializeOnLoad] public static class VersionVariantsValidation
    {
        const string Key="CoinMerge.VersionValidation",Prefix="coinmerge.version.validation.disposable";
        static readonly List<string> checks=new List<string>();
        static GameVersionRouter router;static VersionGmPanel gm;static PackagedGameSession basic;static RecoveredGameSession hot;static Camera camera;
        static RenderTexture target;static int phase;static double next;static float timeout;
        [Serializable] sealed class Report {public bool passed;public string error,unityVersion;public string[] checks;}
        static VersionVariantsValidation(){EditorApplication.playModeStateChanged+=State;}
        public static void Run()
        {
            VersionRulesValidation.Run();
            SessionState.SetBool(Key,true);SessionState.SetString(Key+".error","");ClearStore();
            EditorSceneManager.OpenScene("Assets/Scenes/RecoveredMain.unity");var s=UnityEngine.Object.FindObjectOfType<RecoveredGameSession>();s.saveNamespace=Prefix;s.automaticInput=false;
            EditorApplication.EnterPlaymode();
        }
        static void ClearStore(){var store=new PlayerStore(Prefix);store.ResetPlayer();store.ResetProfile();PlayerPrefs.DeleteKey(Prefix+".packaged.player");PlayerPrefs.Save();}
        static void State(PlayModeStateChange state)
        {
            if(!SessionState.GetBool(Key,false))return;
            if(state==PlayModeStateChange.EnteredPlayMode){checks.Clear();phase=0;next=EditorApplication.timeSinceStartup+1;Application.logMessageReceived+=Log;EditorApplication.update+=Tick;}
            if(state==PlayModeStateChange.EnteredEditMode){ClearStore();SessionState.SetBool(Key,false);EditorApplication.Exit(SessionState.GetString(Key+".error","").Length==0?0:1);}
        }
        static void Log(string text,string stack,LogType type){if(type==LogType.Exception||type==LogType.Error)SessionState.SetString(Key+".error",text+"\n"+stack);}
        static void BindScene()
        {
            router=UnityEngine.Object.FindObjectOfType<GameVersionRouter>();gm=UnityEngine.Object.FindObjectOfType<VersionGmPanel>();
            hot=router.rewarded;basic=router.packaged;camera=hot?hot.worldCamera:basic.worldCamera;
            if(target)UnityEngine.Object.DestroyImmediate(target);target=new RenderTexture(750,1624,24);camera.targetTexture=target;
            if(hot)hot.automaticInput=false;if(basic)basic.automaticInput=false;
            Require(router.SaveNamespace==Prefix,"Scene transition retains isolated store namespace");
        }
        static void Click(Button b)
        {
            Require(b.gameObject.activeInHierarchy&&b.targetGraphic.enabled&&b.IsInteractable(),"Visible and interactive Button: "+b.name);
            Canvas.ForceUpdateCanvases();camera.Render();var rect=b.targetGraphic.rectTransform;
            var pointer=new PointerEventData(EventSystem.current){button=PointerEventData.InputButton.Left,position=RectTransformUtility.WorldToScreenPoint(camera,rect.TransformPoint(rect.rect.center))};
            var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(pointer,hits);
            var actual=hits.Count>0?ExecuteEvents.GetEventHandler<IPointerClickHandler>(hits[0].gameObject):null;
            if(actual!=b.gameObject)throw new Exception("Pointer blocked before "+b.name+" by "+(hits.Count>0?hits[0].gameObject.name:"none"));
            ExecuteEvents.Execute(actual,pointer,ExecuteEvents.pointerDownHandler);ExecuteEvents.Execute(actual,pointer,ExecuteEvents.pointerUpHandler);ExecuteEvents.Execute(actual,pointer,ExecuteEvents.pointerClickHandler);
        }
        static void Action(int code){foreach(var a in basic.actions)if(a.action==code&&a.button.gameObject.activeInHierarchy){Click(a.button);return;}throw new Exception("Missing active action "+code);}
        static void Tick()
        {
            if(EditorApplication.timeSinceStartup<next)return;
            try
            {
                string error=SessionState.GetString(Key+".error","");if(error.Length>0)throw new Exception(error);
                switch(phase++)
                {
                    case 0:
                        BindScene();Require(hot&&router.Profile.country=="US"&&router.Profile.rewardedVariant,"Default US rewards scene");
                        for(int i=0;i<10;i++)Require(VersionRouting.OriginalCohort("test"+i)==(i<5?"A":"B"),"Original numeric AB tail "+i);
                        Require(VersionRouting.OriginalCohort("")=="B"&&VersionRouting.OriginalCohort("testF")=="A","Empty and nonnumeric AB tails preserve source behavior");
                        var p=VersionRouting.Copy(router.Profile);p.contentMode=0;p.localBucket=4999;p.rewardedShare=50;VersionRouting.Resolve(p);Require(p.rewardedVariant,"Local 50% route lower boundary");
                        p.localBucket=5000;VersionRouting.Resolve(p);Require(!p.rewardedVariant,"Local 50% route upper boundary");
                        hot.Player.fakeMoney=77;hot.Save();Click(gm.open);Click(gm.content);Click(gm.content);Click(gm.cohort);Click(gm.cohort);gm.Draft.country="JP";
                        Capture("version_gm_popup.png");
                        var card=(RectTransform)gm.popup.transform.Find("Card");Require(card.rect.height<1200&&card.rect.width<750,"GM card is not full screen");
                        Click(gm.close);Require(!gm.IsOpen&&router.Profile.contentMode==2&&router.Profile.country=="US","Cancel does not apply draft");
                        Click(gm.open);Click(gm.content);Click(gm.content);Click(gm.cohort);Click(gm.cohort);gm.Draft.country="JP";Click(gm.apply);next=EditorApplication.timeSinceStartup+2;break;
                    case 1:
                        BindScene();Require(basic&&SceneManager.GetActiveScene().name=="RecoveredPackaged","Apply loads actual packaged scene");
                        Require(router.Profile.country=="JP"&&router.Profile.cohort=="A"&&!router.Profile.rewardedVariant,"Country, cohort and content persist across scenes");
                        Require(basic.Player.heart==10&&basic.Player.coins==0&&basic.Dialog==0,"Independent packaged fresh save and agreement");
                        Capture("version_packaged_agreement.png");Action(1);Require(basic.InHome&&basic.Dialog<0&&!basic.Player.NewUser,"Agreement leads to source packaged home");Capture("version_packaged_home.png");
                        Action(100);Require(!basic.InHome&&basic.Player.heart==9,"First mode consumes one heart and enters gameplay");next=EditorApplication.timeSinceStartup+.7;break;
                    case 2:
                        Require(basic.board.Preview&&basic.board.Preview.Type==1,"Source first-appearance table selects type 1");
                        basic.board.Spawn(1,new Vector2(-.6f,-17));basic.board.Spawn(1,new Vector2(.6f,-17));next=EditorApplication.timeSinceStartup+1.2;break;
                    case 3:
                        Require(basic.Score==10&&basic.Player.coins==10&&basic.Player.mergedMaxLv==2,"Native polygon contact upgrades coin and awards original 10 coins/score");Capture("version_packaged_game.png");
                        Action(7);Action(3);Action(11);Require(!basic.Player.open_bgm&&!basic.audioCues.music.isPlaying,"Music Button stops native AudioSource");Action(11);Require(basic.Player.open_bgm&&basic.audioCues.music.clip!=null,"Music Button restores original BG clip");Action(0);
                        basic.Player.coins=1000;Action(101);Require(basic.InHome&&basic.Player.coins==0&&basic.Player.passStaus[1]==1&&basic.Player.heart==9,"Locked mode costs 1000 and unlocks without auto entry");
                        Action(101);Require(basic.Mode==1&&basic.Player.heart==8,"Unlocked mode entry consumes one heart");
                        basic.board.Fail();next=EditorApplication.timeSinceStartup+1;break;
                    case 4:
                        Require(basic.Dialog==5,"Failure signal opens source failure dialog");Action(10);Require(basic.Player.heart==7&&basic.Dialog<0&&!basic.board.GameOver,"Retry consumes heart and restarts board");
                        basic.Player.coins=100;Action(6);Action(9);Require(basic.Player.heart==8&&basic.Player.coins==0&&basic.Dialog<0,"Heart purchase spends original 100 coins for one heart");
                        Click(gm.open);Click(gm.reset);Require(basic.Player.heart==10&&basic.Player.coins==0&&router.Profile.country=="JP"&&router.Profile.cohort=="A"&&!router.Profile.rewardedVariant,"Reset clears current player while preserving country/cohort/content");
                        Click(gm.content); // Base -> rewards.
                        Click(gm.apply);next=EditorApplication.timeSinceStartup+2;break;
                    case 5:
                        BindScene();Require(hot&&hot.Player.fakeMoney==77,"Returning to rewards restores its separate player progress");
                        Require(hot.Profile.country=="JP"&&hot.Profile.cohort=="A","Rewarded runtime reads applied country and cohort");
                        Click(gm.open);Capture("version_gm_rewards_jp.png");string region=gm.Draft.country;Click(gm.regionNext);Require(gm.Draft.country!=region,"Country next Button changes region");Click(gm.regionPrevious);Require(gm.Draft.country==region,"Country previous Button restores region");
                        Click(gm.content);Require(gm.Draft.contentMode==0,"Content Button selects stable automatic route");int share=gm.Draft.rewardedShare;Click(gm.shareUp);Require(gm.Draft.rewardedShare==share+10,"Automatic route share increases by 10 percent");Click(gm.shareDown);Require(gm.Draft.rewardedShare==share,"Automatic route share decreases by 10 percent");
                        Click(gm.close);next=EditorApplication.timeSinceStartup+.1;break;
                    case 6:
                        Require(!gm.IsOpen&&!hot.board.InputBlocked,"Closing GM releases board input when no guide modal blocks it");Finish(null);break;
                }
            }
            catch(Exception e){Finish(e.ToString());}
        }
        static void Require(bool value,string description){if(!value)throw new Exception(description);checks.Add(description);}
        static void Capture(string file)
        {
            Canvas.ForceUpdateCanvases();camera.Render();var before=RenderTexture.active;RenderTexture.active=target;var image=new Texture2D(750,1624,TextureFormat.RGB24,false);
            image.ReadPixels(new Rect(0,0,750,1624),0,0);image.Apply();File.WriteAllBytes("../07_Verification/"+file,image.EncodeToPNG());RenderTexture.active=before;UnityEngine.Object.DestroyImmediate(image);
        }
        static void Finish(string error)
        {
            EditorApplication.update-=Tick;Application.logMessageReceived-=Log;if(camera)camera.targetTexture=null;if(target)UnityEngine.Object.DestroyImmediate(target);
            SessionState.SetString(Key+".error",error??"");File.WriteAllText("../07_Verification/version_variants_validation.json",JsonUtility.ToJson(new Report {passed=error==null,error=error,unityVersion=Application.unityVersion,checks=checks.ToArray()},true));
            Debug.Log(error==null?"VERSION_VARIANTS_VALIDATED "+checks.Count:error);EditorApplication.ExitPlaymode();
        }
    }
}
