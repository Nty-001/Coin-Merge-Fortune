using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
namespace CoinMerge.Recovery.Editor
{
    [InitializeOnLoad] public static class RecoveredVisualsValidation
    {
        const string Key="CoinMerge.VisualValidation",Prefix="coinmerge.visual.validation.disposable";
        static readonly List<string> checks=new List<string>();
        static RecoveredGameSession session;static Camera camera;static RenderTexture target;
        static bool captured;static int phase;static float lastProgress;static double next,deadline;
        [Serializable] sealed class Report{public bool passed;public string error;public string[] checks;}
        static RecoveredVisualsValidation(){EditorApplication.playModeStateChanged+=State;}
        public static void Run(){Begin(false);}
        public static void RunPackaged(){Begin(true);}
        static void Begin(bool packaged)
        {
            RecoveredAssetValidation.Run();
            checks.Clear();SessionState.SetBool(Key,true);SessionState.SetBool(Key+".packaged",packaged);SessionState.SetString(Key+".error","");
            var store=new PlayerStore(Prefix);store.ResetPlayer();store.ResetProfile();
            store.Save(new PlayerProgress {guideStep=5,fakeMoney=434.77});
            store.SaveProfile(new VersionProfile {country="US",cohort="B",rewardedVariant=!packaged});
            EditorSceneManager.OpenScene("Assets/Scenes/RecoveredLoading.unity");
            UnityEngine.Object.FindObjectOfType<RecoveredStartup>().saveNamespace=Prefix;
            EditorApplication.EnterPlaymode();
        }
        static void State(PlayModeStateChange state)
        {
            if(!SessionState.GetBool(Key,false))return;
            if(state==PlayModeStateChange.EnteredPlayMode)
            {
                captured=false;phase=0;lastProgress=0;next=0;deadline=EditorApplication.timeSinceStartup+60;
                Application.logMessageReceived+=Log;EditorApplication.update+=Tick;
            }
            if(state==PlayModeStateChange.EnteredEditMode)
            {
                var store=new PlayerStore(Prefix);store.ResetPlayer();store.ResetProfile();
                SessionState.SetBool(Key,false);EditorApplication.Exit(SessionState.GetString(Key+".error","").Length==0?0:1);
            }
        }
        static void Log(string message,string trace,LogType type)
        {if(type==LogType.Error||type==LogType.Exception)SessionState.SetString(Key+".error",message+"\n"+trace);}
        static void Require(bool value,string reason){if(!value)throw new Exception(reason);checks.Add(reason);}
        static void Tick()
        {
            if(EditorApplication.timeSinceStartup<next)return;
            try
            {
                if(EditorApplication.timeSinceStartup>deadline)throw new Exception("Visual validation timed out");
                string error=SessionState.GetString(Key+".error","");if(error.Length>0)throw new Exception(error);
                bool packaged=SessionState.GetBool(Key+".packaged",false);
                if(phase==0)
                {
                    var startup=UnityEngine.Object.FindObjectOfType<RecoveredStartup>();
                    if(startup)
                    {
                        if(startup.Progress+1e-5f<lastProgress)throw new Exception("Startup progress regressed");lastProgress=startup.Progress;
                        if(startup.View&&!captured)
                        {
                            Require(startup.Profile.rewardedVariant!=packaged,"Saved version selects its original loading view");
                            Require(startup.View.fill.type==Image.Type.Filled,"Original progress graphic is a native filled Image");
                            CaptureLoading(startup.View,packaged?"startup_packaged.png":"startup_rewarded.png");captured=true;
                        }
                        return;
                    }
                    Require(captured,"Original startup view was visible before gameplay");
                    if(packaged)
                    {
                        var game=UnityEngine.Object.FindObjectOfType<PackagedGameSession>();
                        Require(game&&game.saveNamespace==Prefix,"Packaged loading enters A gameplay using isolated save namespace");
                        Require(SceneManager.GetActiveScene().name=="RecoveredPackaged","A loading activates the matching scene");Finish(null);return;
                    }
                    session=UnityEngine.Object.FindObjectOfType<RecoveredGameSession>();
                    Require(session&&session.saveNamespace==Prefix,"Rewarded loading enters B gameplay using isolated save namespace");
                    Require(session.notice&&session.notice.label.text.Length>0,"Notice is populated on the first gameplay frame");
                    session.automaticInput=false;camera=session.worldCamera;target=new RenderTexture(750,1624,24);camera.targetTexture=target;
                    phase=1;next=EditorApplication.timeSinceStartup+.5;return;
                }
                if(phase==1)
                {
                    Require(session.bubble.activeSelf&&session.bubbleText.text.Contains("$65.23"),"An exact 434.77 balance shows the original formatter's remaining-money text");
                    Capture("home_notice_and_remaining.png");
                    session.Player.fakeMoney=500;session.ChangeProfile("US","B",true);Require(!session.bubble.activeSelf,"Remaining-money bubble hides at the first 500 threshold");
                    session.Player.fakeMoney=23569.29;session.ChangeProfile("US","B",true);Require(!session.bubble.activeSelf,"Above-threshold screenshot state correctly hides the bubble");
                    session.Player.fakeMoney=220;session.ChangeProfile("US","B",true);Require(session.bubble.activeSelf&&session.bubbleText.text.Contains("$280"),"Bubble reappears below threshold without clearing player data");
                    var notice=session.notice;notice.enabled=false;notice.Configure(session.Locale,RecoveredGameRules.CashConfiguration(session.board.Config.rules,"US").new_Fake_products);
                    string initial=notice.label.text;int messages=notice.MessageCount;var bell=notice.content.parent.Find("icon_notice");float bellY=bell.localPosition.y;
                    notice.Tick(3.99f);Require(Mathf.Abs(notice.Offset)<.001f&&notice.MessageCount==messages,"Notice rests until the original four-second schedule");
                    notice.Tick(.011f);notice.Tick(.449f);Require(Mathf.Abs(notice.Offset-55)<.2f&&notice.label.text==initial,"Sine midpoint moves only the text container up by 55 units");
                    Capture("notice_outgoing.png");notice.Tick(.451f);Require(Mathf.Abs(notice.Offset+110)<.2f&&notice.MessageCount==messages+1,"At .9 seconds message refreshes offscreen and wraps below the mask");
                    notice.Tick(.45f);Require(Mathf.Abs(notice.Offset+55)<.3f,"Incoming text uses the same sine easing");notice.Tick(.45f);
                    Require(Mathf.Abs(notice.Offset)<.001f&&bell.localPosition.y==bellY,"After 1.8 seconds text rests at origin and the serialized bell remains fixed");
                    notice.Tick(2.5f);notice.Restart();Require(Mathf.Abs(notice.Offset)<.001f,"Round restart returns the notice to its original position and schedule");
                    Require(notice.label.text.Contains("<color=#FFF65E>")&&!notice.label.text.Contains("%{"),"Original localized highlight markup and all tokens are resolved");
                    foreach(string country in new[]{"US","BR","ID","JP","DE"})
                    {
                        var locale=new RecoveredLocalization(country);var group=RecoveredGameRules.CashConfiguration(session.board.Config.rules,country);
                        notice.Configure(locale,group.new_Fake_products);
                        Require(notice.label.text.Contains("<color=#FFF65E>")&&!notice.label.text.Contains("%{"),country+" notice uses its country localization and product table");
                    }
                    notice.Configure(session.Locale,RecoveredGameRules.CashConfiguration(session.board.Config.rules,"US").new_Fake_products);notice.enabled=true;
                    int strokes=0;
                    foreach(var outline in session.GetComponentsInChildren<Outline>(true))
                    {
                        if(!(outline is RecoveredRoundOutline))throw new Exception("Unconverted four-direction outline: "+outline.name);strokes++;
                    }
                    Require(strokes>20,"All "+strokes+" gameplay/menu outlines use the round native mesh effect");
                    Require(session.menus.forms[0].confirmOutline is RecoveredRoundOutline,"Serialized account button outline reference remains valid");
                    ValidateRadius();session.menus.Act(2);phase=2;
                    next=EditorApplication.timeSinceStartup+session.menus.ruleAnimation.Data.animations[0].duration+.2;return;
                }
                if(phase==2)
                {
                    Capture("rules_round_outline.png");session.menus.CloseAll();session.menus.Act(1);phase=3;next=EditorApplication.timeSinceStartup+.5;return;
                }
                if(phase==3){Capture("settings_round_outline.png");session.menus.CloseAll();Finish(null);}
            }
            catch(Exception ex){Finish(ex.ToString());}
        }
        static void ValidateRadius()
        {
            var node=new GameObject("StrokeGeometryFixture",typeof(RectTransform),typeof(CanvasRenderer),typeof(Text));
            var effect=node.AddComponent<RecoveredRoundOutline>();effect.effectDistance=new Vector2(6,-6);
            var mesh=new VertexHelper();var vertex=UIVertex.simpleVert;vertex.position=Vector3.zero;
            for(int i=0;i<3;i++)mesh.AddVert(vertex);mesh.AddTriangle(0,1,2);effect.ModifyMesh(mesh);
            var points=new List<UIVertex>();mesh.GetUIVertexStream(points);float max=0;
            foreach(var point in points)max=Mathf.Max(max,point.position.magnitude);
            Require(Mathf.Abs(max-6)<.001f,"Source six-unit stroke remains a six-unit circular radius, without diagonal expansion");
            Require(points.Count==51,"Sixteen stroke samples retain one original fill mesh");mesh.Dispose();UnityEngine.Object.DestroyImmediate(node);
        }
        static void CaptureLoading(RecoveredLoadingView view,string name)
        {
            var node=new GameObject("LoadingCapture",typeof(Camera));var c=node.GetComponent<Camera>();c.orthographic=true;c.orthographicSize=812/32f;c.transform.position=new Vector3(0,0,-100);c.farClipPlane=200;
            var rt=new RenderTexture(750,1624,24);c.targetTexture=rt;var canvas=view.GetComponentInChildren<Canvas>();
            canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=c;canvas.planeDistance=100;
            Save(c,rt,name);canvas.renderMode=RenderMode.ScreenSpaceOverlay;canvas.worldCamera=null;
            c.targetTexture=null;UnityEngine.Object.DestroyImmediate(rt);UnityEngine.Object.DestroyImmediate(node);
        }
        static void Capture(string name){Save(camera,target,name);}
        static void Save(Camera source,RenderTexture rt,string name)
        {
            Canvas.ForceUpdateCanvases();source.Render();var previous=RenderTexture.active;RenderTexture.active=rt;
            var texture=new Texture2D(750,1624,TextureFormat.RGB24,false);texture.ReadPixels(new Rect(0,0,750,1624),0,0);texture.Apply();
            File.WriteAllBytes("../07_Verification/"+name,texture.EncodeToPNG());RenderTexture.active=previous;UnityEngine.Object.DestroyImmediate(texture);
        }
        static void Finish(string error)
        {
            EditorApplication.update-=Tick;Application.logMessageReceived-=Log;if(camera)camera.targetTexture=null;if(target)UnityEngine.Object.DestroyImmediate(target);
            SessionState.SetString(Key+".error",error??"");
            string suffix=SessionState.GetBool(Key+".packaged",false)?"packaged":"rewarded";
            File.WriteAllText("../07_Verification/visuals_"+suffix+"_validation.json",JsonUtility.ToJson(new Report {passed=error==null,error=error,checks=checks.ToArray()},true));
            Debug.Log(error==null?"VISUALS_VALIDATED "+suffix+" "+checks.Count:error);EditorApplication.ExitPlaymode();
        }
    }
}
