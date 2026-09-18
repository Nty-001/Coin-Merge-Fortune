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
    [InitializeOnLoad] public static class SettingsReskinValidation
    {
        const string Key="CoinMerge.SettingsReskin.Validation",Prefix="coinmerge.settings.reskin.disposable";
        static readonly List<string> checks=new List<string>();
        static RecoveredGameSession session;static Camera camera;static RenderTexture target;static int phase;static double next,deadline;
        static string Output=>Path.GetFullPath("Design/SettingsR1/Verification");
        [Serializable] sealed class Report{public bool passed;public string error,unityVersion;public string[] checks;}
        static SettingsReskinValidation(){EditorApplication.playModeStateChanged+=State;}
        [MenuItem("Coin Merge/Reskin/Validate reference Settings")]
        public static void Run()
        {
            Directory.CreateDirectory(Output);SessionState.SetBool(Key,true);SessionState.SetString(Key+".error","");
            var store=new PlayerStore(Prefix);store.ResetPlayer();store.ResetProfile();
            store.Save(new PlayerProgress{guideStep=5,fakeMoney=446.43,open_bgm=false,open_music=false,open_vibrate=true});
            store.SaveProfile(new VersionProfile{country="US",cohort="B",rewardedVariant=true});
            EditorSceneManager.OpenScene("Assets/Scenes/RecoveredMain.unity");
            var s=UnityEngine.Object.FindObjectOfType<RecoveredGameSession>();s.saveNamespace=Prefix;s.automaticInput=false;EditorApplication.EnterPlaymode();
        }
        static void State(PlayModeStateChange state)
        {
            if(!SessionState.GetBool(Key,false))return;
            if(state==PlayModeStateChange.EnteredPlayMode){checks.Clear();phase=0;next=EditorApplication.timeSinceStartup+1;deadline=next+80;Application.logMessageReceived+=Log;EditorApplication.update+=Tick;}
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
                if(EditorApplication.timeSinceStartup>deadline)throw new Exception("Settings validation timeout");
                string error=SessionState.GetString(Key+".error","");if(error.Length>0)throw new Exception(error);
                switch(phase++)
                {
                    case 0:session=UnityEngine.Object.FindObjectOfType<RecoveredGameSession>();camera=session.worldCamera;Resize(1254,1254);Click(Button(1));break;
                    case 1:
                        Require(session.menus.CurrentPage==0,"Settings opens through main native Button");ValidateShapes();Capture("settings_reference_state_1254.png");
                        Click(Button(5,0));Require(session.Player.open_bgm&&session.Player.open_music,"Music ON toggles original music and effect flags");Require(session.menus.audioCues.music.isPlaying,"Music ON plays native AudioSource");Require(new PlayerStore(Prefix).LoadPlayer().open_bgm,"Music ON persisted");break;
                    case 2:
                        Click(Button(5,0));Require(!session.Player.open_bgm&&!session.Player.open_music,"Music OFF clears original music and effect flags");Require(!session.menus.audioCues.music.isPlaying,"Music OFF stops native AudioSource");Require(!new PlayerStore(Prefix).LoadPlayer().open_bgm,"Music OFF persisted");
                        Click(Button(6,0));Require(!session.Player.open_vibrate&&!new PlayerStore(Prefix).LoadPlayer().open_vibrate,"Vibration OFF persisted");break;
                    case 3:
                        Capture("settings_both_off.png");Click(Button(6,0));Require(session.Player.open_vibrate&&new PlayerStore(Prefix).LoadPlayer().open_vibrate,"Vibration ON persisted");Resize(1080,2340);Click(Button(8,0));break;
                    case 4:
                        Require(session.menus.CurrentPage==2&&session.menus.termsScroll.activeInHierarchy,"User Agreement opens existing terms page");Capture("terms.png");Click(Button(0,2));break;
                    case 5:
                        Require(session.menus.CurrentPage==0,"Terms close returns to Settings");Click(Button(7,0));break;
                    case 6:
                        Require(session.menus.CurrentPage==2&&session.menus.privacyScroll.activeInHierarchy,"Privacy Policy opens existing policy page");Capture("privacy.png");Click(Button(0,2));break;
                    case 7:
                        Require(session.menus.CurrentPage==0,"Privacy close returns to Settings");Click(Button(0,0));Require(!session.menus.IsOpen,"Red X closes Settings");Resize(1080,2340);Click(Button(1));break;
                    case 8:
                        ValidateShapes();Capture("settings_1080x2340.png");Click(Button(0,0));session.ChangeProfile("JP","B",true);Click(Button(1));break;
                    case 9:
                        Capture("settings_jp.png");Click(Button(0,0));Click(session.gm.open);Require(session.gm.IsOpen,"GM entry remains clickable after modal closes");break;
                    case 10:
                        Click(session.gm.close);Require(!session.gm.IsOpen,"GM popup close retains topmost native hit target");Click(Button(3));break;
                    case 11:
                        Require(session.menus.CurrentPage==3,"Cash withdrawal layering regression check");Click(Button(0,3));Click(Button(4));break;
                    case 12:
                        Require(session.menus.CurrentPage==4,"Chip withdrawal layering regression check");Click(Button(0,4));Finish(null);break;
                }
            }
            catch(Exception e){Finish(e.ToString());}
        }
        static void ValidateShapes()
        {
            var gmPointer=new PointerEventData(EventSystem.current){position=RectTransformUtility.WorldToScreenPoint(camera,session.gm.open.transform.position)};
            var gmHits=new List<RaycastResult>();EventSystem.current.RaycastAll(gmPointer,gmHits);
            Require(gmHits.Count>0&&ExecuteEvents.GetEventHandler<IPointerClickHandler>(gmHits[0].gameObject)!=session.gm.open.gameObject,"Modal mask covers GM entry");
            foreach(var button in session.menus.pages[0].GetComponentsInChildren<Button>())
            {
                Require(button.targetGraphic.gameObject==button.gameObject,"Button visual and click target share native object");
                var image=button.targetGraphic as Image;if(!image)continue;
                Require(image.preserveAspect,"Native Image preserves button aspect");
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
            SessionState.SetString(Key+".error",error??"");File.WriteAllText(Path.Combine(Output,"play_mode.json"),JsonUtility.ToJson(new Report{passed=error==null,error=error,unityVersion=Application.unityVersion,checks=checks.ToArray()},true));Debug.Log(error==null?"SETTINGS_RESKIN_VALIDATED "+checks.Count:error);EditorApplication.ExitPlaymode();
        }
    }
}
