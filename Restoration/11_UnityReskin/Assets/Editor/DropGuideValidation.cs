using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace CoinMerge.Recovery.Editor
{
    [InitializeOnLoad] public static class DropGuideValidation
    {
        const string Key="CoinMerge.DropGuideValidation",Prefix="coinmerge.dropguide.disposable";
        static int result;static readonly List<string> checks=new List<string>();
        static DropGuideValidation(){EditorApplication.playModeStateChanged+=State;}
        public static void Run()
        {
            var store=new PlayerStore(Prefix);store.ResetPlayer();store.ResetProfile();
            store.Save(new PlayerProgress{guideStep=9999,hasSavedGameScene=true,currentLotteryCount=100});
            EditorSceneManager.OpenScene("Assets/Scenes/RecoveredMain.unity");
            var s=UnityEngine.Object.FindObjectOfType<RecoveredGameSession>();s.saveNamespace=Prefix;s.automaticInput=false;
            SessionState.SetBool(Key,true);EditorApplication.EnterPlaymode();
        }
        static void State(PlayModeStateChange state)
        {
            if(!SessionState.GetBool(Key,false))return;
            if(state==PlayModeStateChange.EnteredPlayMode)EditorApplication.delayCall+=Check;
            if(state==PlayModeStateChange.EnteredEditMode){SessionState.SetBool(Key,false);var store=new PlayerStore(Prefix);store.ResetPlayer();store.ResetProfile();EditorApplication.Exit(result);}
        }
        static void Require(bool value,string message){if(!value)throw new Exception(message);checks.Add(message);}
        static void Check()
        {
            result=0;checks.Clear();string error="";RenderTexture surface=null;RecoveredGameSession s=null;
            try
            {
                s=UnityEngine.Object.FindObjectOfType<RecoveredGameSession>();s.Initialize(new PlayerStore(Prefix));s.enabled=false;
                var guide=s.GetComponentInChildren<RecoveredDropGuide>();var b=s.board;
                Require(guide&&guide.dots.Length==128,"Delivered scene contains 128 authored world-space SpriteRenderer dots");
                s.worldCamera.GetComponent<DeviceSafeViewport>().enabled=false;
                surface=new RenderTexture(750,1624,24);s.worldCamera.targetTexture=surface;s.worldCamera.rect=new Rect(0,0,1,1);
                Canvas.ForceUpdateCanvases();s.playfieldLayout.Refresh();b.Tick(.4f);b.MovePreview(0);
                guide.Refresh(false);Require(guide.VisibleDots==0,"No guide without a held pointer");
                guide.Refresh(true);int emptyCount=guide.VisibleDots;
                Require(emptyCount>10,"Holding draws dotted line on empty board");
                Require(Mathf.Abs(guide.dots[0].transform.position.y-(b.Preview.Position.y-b.Preview.Radius))<.001f,"First dot begins at preview coin bottom");
                Require(Mathf.Abs((guide.dots[0].transform.position.y-guide.dots[1].transform.position.y)*b.Units-30)<.001f,"Original 30-pixel spacing");
                Require(Mathf.Abs(guide.dots[0].bounds.size.x*b.Units-10)<.1f&&Mathf.Abs(guide.dots[0].color.a-180f/255)<.001f,"Original 5-pixel radius and 180 alpha");
                Capture(s,surface,"drop-guide-empty.png");
                var coin=b.Spawn(100,new Vector3(0,b.ground.position.y+4));coin.body.simulated=false;
                guide.Refresh(true);
                Require(guide.VisibleDots<emptyCount,"Guide shortens above a coin stack");
                float expected=coin.Position.y+coin.Radius;
                Require(Mathf.Abs(b.GetDropLandingBottom()-expected)<.001f,"Centered landing point matches upper coin surface");
                var last=guide.dots[guide.VisibleDots-1].transform.position;
                Require(last.y>expected&&last.y-expected<=30/b.Units+.001f,"No dots extend below predicted landing point");
                Capture(s,surface,"drop-guide-stack.png");
                b.MovePreview(2);guide.Refresh(true);
                float r=b.Preview.Radius+coin.Radius;
                expected=coin.Position.y+Mathf.Sqrt(r*r-4)-b.Preview.Radius;
                Require(Mathf.Abs(b.GetDropLandingBottom()-expected)<.001f,"Off-center landing uses circle intersection, not a vertical ray");
                Require(Mathf.Abs(guide.dots[0].transform.position.x-b.Preview.Position.x)<.001f,"Dots follow dragged preview horizontally");
                b.MovePreview(100);guide.Refresh(true);Require(b.Preview.Position.x<=b.widthPixels*.5f/b.Units-b.Preview.Radius+.001f,"Preview and line share wall clamping");
                guide.Refresh(false);Require(guide.VisibleDots==0,"Release hides the line");
                b.InputBlocked=true;guide.Refresh(true);Require(guide.VisibleDots==0,"Modal-blocked input hides guide");b.InputBlocked=false;
                b.RequestDrop();guide.Refresh(true);Require(guide.VisibleDots==0,"Dropped preview cannot leave a stale line");
                b.Tick(.4f);guide.Refresh(true);Require(guide.VisibleDots>0,"New preview supports a fresh hold");
                s.OnApplicationPause(true);Require(!s.IsBoardPointerHeld,"Backgrounding cancels held input");guide.Refresh(s.IsBoardPointerHeld);Require(guide.VisibleDots==0,"Backgrounding hides alignment dots");
                b.TriggerFailure();guide.Refresh(true);Require(guide.VisibleDots==0,"Game over hides alignment dots");
            }
            catch(Exception ex){error=ex.ToString();result=1;}
            File.WriteAllText("drop-guide-validation.txt",(result==0?"PASS":"FAIL")+"\n"+string.Join("\n",checks)+"\n"+error);
            if(surface){s.worldCamera.targetTexture=null;surface.Release();UnityEngine.Object.DestroyImmediate(surface);}
            Debug.Log("DROP_GUIDE_VALIDATION "+(result==0?"PASS":"FAIL")+" "+error);EditorApplication.ExitPlaymode();
        }
        static void Capture(RecoveredGameSession s,RenderTexture surface,string name)
        {
            Canvas.ForceUpdateCanvases();s.worldCamera.Render();var previous=RenderTexture.active;RenderTexture.active=surface;
            var image=new Texture2D(surface.width,surface.height,TextureFormat.RGB24,false);image.ReadPixels(new Rect(0,0,surface.width,surface.height),0,0);image.Apply();
            File.WriteAllBytes(name,image.EncodeToPNG());UnityEngine.Object.DestroyImmediate(image);RenderTexture.active=previous;
        }
    }
}
