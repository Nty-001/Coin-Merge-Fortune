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
    [InitializeOnLoad] public static class RulesReskinValidation
    {
        const string Key="CoinMerge.RulesReskin.Validation",Prefix="coinmerge.rules.reskin.disposable";
        static readonly List<string> checks=new List<string>();
        static RecoveredGameSession session;static Camera camera;static RenderTexture target;
        static int phase;static double next,deadline;
        static string Output=>Path.GetFullPath("Design/RulesR1/Verification");
        [Serializable] sealed class Report {public bool passed;public string error,unityVersion;public string[] checks;}
        static RulesReskinValidation(){EditorApplication.playModeStateChanged+=State;}
        [MenuItem("Coin Merge/Reskin/Validate reference Merge Rules")]
        public static void Run()
        {
            Directory.CreateDirectory(Output);SessionState.SetBool(Key,true);SessionState.SetString(Key+".error","");
            var store=new PlayerStore(Prefix);store.ResetPlayer();store.ResetProfile();
            store.Save(new PlayerProgress{guideStep=5,fakeMoney=446.43});store.SaveProfile(new VersionProfile{country="US",cohort="B",rewardedVariant=true});
            EditorSceneManager.OpenScene("Assets/Scenes/RecoveredMain.unity");
            var s=UnityEngine.Object.FindObjectOfType<RecoveredGameSession>();s.saveNamespace=Prefix;s.automaticInput=false;
            EditorApplication.EnterPlaymode();
        }
        static void State(PlayModeStateChange state)
        {
            if(!SessionState.GetBool(Key,false))return;
            if(state==PlayModeStateChange.EnteredPlayMode)
            {checks.Clear();phase=0;next=EditorApplication.timeSinceStartup+1;deadline=next+80;Application.logMessageReceived+=Log;EditorApplication.update+=Tick;}
            if(state==PlayModeStateChange.EnteredEditMode)
            {
                var store=new PlayerStore(Prefix);store.ResetPlayer();store.ResetProfile();SessionState.SetBool(Key,false);
                EditorSceneManager.OpenScene("Assets/Scenes/RecoveredLoading.unity");
                if(Application.isBatchMode)EditorApplication.Exit(SessionState.GetString(Key+".error","").Length==0?0:1);
            }
        }
        static void Log(string m,string s,LogType t){if(t==LogType.Exception||t==LogType.Error)SessionState.SetString(Key+".error",m+"\n"+s);}
        static void Require(bool condition,string text){if(!condition)throw new Exception(text);checks.Add(text);}
        static Button Button(int action,bool inRules=false)
        {
            foreach(var binding in session.menus.actions)
                if(binding.action==action&&binding.button.gameObject.activeInHierarchy&&(!inRules||binding.button.transform.IsChildOf(session.menus.pages[1].transform)))return binding.button;
            throw new Exception("No visible Button "+action);
        }
        static Button CloseButton()
        {
            foreach(var binding in session.menus.actions)
                if(binding.action==0&&binding.button.gameObject.activeInHierarchy&&binding.button.name=="btnClose")return binding.button;
            throw new Exception("Missing rules close button");
        }
        static void Click(Button button)
        {
            Require(button.IsInteractable(),"Interactable "+button.name);Require(button.onClick.GetPersistentEventCount()==0,"Code-bound "+button.name);
            Canvas.ForceUpdateCanvases();camera.Render();var rect=button.targetGraphic.rectTransform;
            var pointer=new PointerEventData(EventSystem.current){button=PointerEventData.InputButton.Left,position=RectTransformUtility.WorldToScreenPoint(camera,rect.TransformPoint(rect.rect.center))};
            var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(pointer,hits);
            var actual=hits.Count>0?ExecuteEvents.GetEventHandler<IPointerClickHandler>(hits[0].gameObject):null;
            Require(actual==button.gameObject,"Visible graphic receives click: "+button.name);
            ExecuteEvents.Execute(actual,pointer,ExecuteEvents.pointerDownHandler);ExecuteEvents.Execute(actual,pointer,ExecuteEvents.pointerUpHandler);ExecuteEvents.Execute(actual,pointer,ExecuteEvents.pointerClickHandler);
        }
        static void Tick()
        {
            if(EditorApplication.timeSinceStartup<next)return;next=EditorApplication.timeSinceStartup+.6;
            try
            {
                if(EditorApplication.timeSinceStartup>deadline)throw new Exception("Rules validation timeout");
                string err=SessionState.GetString(Key+".error","");if(err.Length>0)throw new Exception(err);
                switch(phase++)
                {
                    case 0:session=UnityEngine.Object.FindObjectOfType<RecoveredGameSession>();camera=session.worldCamera;Resize(1024,1536);break;
                    case 1:Capture("home_unchanged.png");Click(Button(2));next+=4;break;
                    case 2:
                        Require(session.menus.CurrentPage==1,"Rules opens through main native Button");
                        Require(session.menus.ruleAnimation.Data.texturePath=="RulesReskin/ChipAtlas","Animation uses exact requested chip atlas");
                        Require(Mathf.Abs(session.menus.ruleAnimation.Data.animations[0].duration-3.3333f)<.001,"Original animation duration retained");
                        ValidateShapes();Capture("rules_1024x1536.png");Click(CloseButton());break;
                    case 3:
                        Require(!session.menus.IsOpen,"X closes the rules popup");Click(Button(2));next=EditorApplication.timeSinceStartup+.45;break;
                    case 4:Capture("rules_reveal_animation.png");next+=4;break;
                    case 5:Click(Button(0,true));Require(!session.menus.IsOpen,"OK closes the rules popup");Resize(1080,2340);Click(Button(2));next+=4;break;
                    case 6:ValidateShapes();Capture("rules_1080x2340.png");Click(CloseButton());session.ChangeProfile("JP","B",true);Click(Button(2));next+=4;break;
                    case 7:
                        Capture("rules_jp.png");Require(session.menus.pages[1].GetComponentsInChildren<Text>().Length>=4,"Localized native text remains separate");Click(CloseButton());Finish(null);break;
                }
            }
            catch(Exception e){Finish(e.ToString());}
        }
        static void ValidateShapes()
        {
            foreach(var button in session.menus.pages[1].GetComponentsInChildren<Button>())
            {
                var image=button.targetGraphic as Image;Require(image&&image.preserveAspect&&image.gameObject==button.gameObject,"Button graphic remains on same object with preserved aspect");
                float pixel=image.sprite.rect.width/image.sprite.rect.height;float rect=image.rectTransform.rect.width/image.rectTransform.rect.height;
                Require(Mathf.Abs(pixel-rect)<.01f,"Button rectangle matches source image aspect: "+button.name);
            }
            foreach(var a in session.menus.ruleAnimation.Data.attachments)
            {
                if(a.name=="ZZ"||a.name=="11_11")continue;
                Require(Mathf.Abs(Mathf.Abs(a.positions[4]-a.positions[0])-Mathf.Abs(a.positions[3]-a.positions[1]))<.01,"Circular chip geometry "+a.name);
            }
        }
        static void Resize(int w,int h){camera.targetTexture=null;if(target)UnityEngine.Object.DestroyImmediate(target);target=new RenderTexture(w,h,24);camera.targetTexture=target;Canvas.ForceUpdateCanvases();session.playfieldLayout.Refresh();}
        static void Capture(string name)
        {
            Canvas.ForceUpdateCanvases();camera.Render();var old=RenderTexture.active;RenderTexture.active=target;
            var image=new Texture2D(target.width,target.height,TextureFormat.RGB24,false);image.ReadPixels(new Rect(0,0,target.width,target.height),0,0);image.Apply();
            File.WriteAllBytes(Path.Combine(Output,name),image.EncodeToPNG());RenderTexture.active=old;UnityEngine.Object.DestroyImmediate(image);
        }
        static void Finish(string error)
        {
            EditorApplication.update-=Tick;Application.logMessageReceived-=Log;if(camera)camera.targetTexture=null;if(target)UnityEngine.Object.DestroyImmediate(target);
            SessionState.SetString(Key+".error",error??"");File.WriteAllText(Path.Combine(Output,"play_mode.json"),JsonUtility.ToJson(new Report{passed=error==null,error=error,unityVersion=Application.unityVersion,checks=checks.ToArray()},true));
            Debug.Log(error==null?"RULES_RESKIN_VALIDATED "+checks.Count:error);EditorApplication.ExitPlaymode();
        }
    }
}
