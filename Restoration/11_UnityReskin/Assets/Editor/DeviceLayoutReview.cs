using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace CoinMerge.Recovery.Editor
{
    [InitializeOnLoad]
    public static class DeviceLayoutReview
    {
        const string Key="DeviceLayoutReview.running", Prefix="coinmerge.safearea.disposable";
        const string Output="../Android/DeviceLayoutReview";
        static RecoveredGameSession s;
        static RenderTexture target;
        static int resolutionIndex, pageIndex, checks, exitCode;
        static double next;
        static readonly Vector3[] corners=new Vector3[4];
        static readonly Vector2Int[] Sizes={new Vector2Int(900,1600),new Vector2Int(1080,2400),new Vector2Int(1536,2048)};
        static DeviceLayoutReview(){EditorApplication.playModeStateChanged+=State;}
        public static void Run()
        {
            Directory.CreateDirectory(Output);
            EditorSceneManager.OpenScene("Assets/Scenes/RecoveredMain.unity");
            var session=UnityEngine.Object.FindObjectOfType<RecoveredGameSession>();session.saveNamespace=Prefix;session.automaticInput=false;
            var store=new PlayerStore(Prefix);store.ResetPlayer();store.ResetProfile();store.Save(new PlayerProgress{guideStep=9999,fakeMoney=220});
            store.SaveProfile(new VersionProfile{country="US",cohort="B",rewardedVariant=true,contentMode=2,cohortMode=2});
            SessionState.SetBool(Key,true);EditorApplication.EnterPlaymode();
        }
        static void State(PlayModeStateChange state)
        {
            if(!SessionState.GetBool(Key,false))return;
            if(state==PlayModeStateChange.EnteredPlayMode){next=EditorApplication.timeSinceStartup+1;s=null;resolutionIndex=0;pageIndex=-2;checks=0;EditorApplication.update+=Tick;}
            if(state==PlayModeStateChange.EnteredEditMode){SessionState.SetBool(Key,false);var store=new PlayerStore(Prefix);store.ResetPlayer();store.ResetProfile();EditorApplication.Exit(exitCode);}
        }
        static void Tick()
        {
            if(EditorApplication.timeSinceStartup<next)return;
            next=EditorApplication.timeSinceStartup+.3;
            try
            {
                if(!s)
                {
                    s=UnityEngine.Object.FindObjectOfType<RecoveredGameSession>();s.automaticInput=false;
                    s.guideView.gameObject.SetActive(false);s.gm.Hide();
                    s.worldCamera.GetComponent<DeviceSafeViewport>().enabled=false;
                }
                if(pageIndex==-2)
                {
                    var size=Sizes[resolutionIndex];
                    var safe=resolutionIndex==1?new Rect(0,96,size.x,size.y-252):new Rect(0,0,size.x,size.y);
                    var fitted=DeviceSafeViewport.Fit(size,safe,new Vector2(750,1624),6);
                    if(target){s.worldCamera.targetTexture=null;UnityEngine.Object.DestroyImmediate(target);}
                    target=new RenderTexture(Mathf.RoundToInt(fitted.width),Mathf.RoundToInt(fitted.height),24);
                    s.worldCamera.targetTexture=target;s.worldCamera.rect=new Rect(0,0,1,1);
                    foreach(var scaler in s.worldCamera.GetComponent<DeviceSafeViewport>().scalers)scaler.scaleFactor=target.width/750f;
                    s.worldCamera.clearFlags=CameraClearFlags.SolidColor;s.worldCamera.backgroundColor=new Color(.3f,.7f,1);
                    s.menus.CloseAll();pageIndex=-1;return;
                }
                Canvas.ForceUpdateCanvases();s.playfieldLayout.Refresh();
                foreach(var popup in s.GetComponentsInChildren<RecoveredMenuPopup>())popup.Tick(1);
                s.worldCamera.Render();
                if(pageIndex==-1){Bounds(s.playfieldLayout.upper);Bounds(s.playfieldLayout.bottom);}
                else
                {
                    var page=s.menus.pages[pageIndex];
                    foreach(var button in page.GetComponentsInChildren<Button>())
                    {
                        if(!button.gameObject.activeInHierarchy || !button.targetGraphic)continue;
                        if(button.GetComponentInParent<RectMask2D>()||button.GetComponentInParent<Mask>())continue;
                        Bounds(button.targetGraphic.rectTransform);
                    }
                }
                Capture(Sizes[resolutionIndex]+"-page"+pageIndex+".png");checks++;
                s.menus.CloseAll();pageIndex++;
                if(pageIndex>=s.menus.pages.Length){resolutionIndex++;pageIndex=-2;if(resolutionIndex>=Sizes.Length){ReviewLoading();Finish(null);return;}}
                if(pageIndex>=0)s.menus.Show(pageIndex);
            }
            catch(Exception e){Finish(e.ToString());}
        }
        static void Bounds(RectTransform rect)
        {
            rect.GetWorldCorners(corners);
            foreach(var corner in corners)
            {
                var p=s.worldCamera.WorldToViewportPoint(corner);
                if(p.x<-.01f||p.x>1.01f||p.y<-.01f||p.y>1.01f)
                    throw new Exception("Out of safe viewport: "+rect.name+" "+p+" page="+pageIndex+" size="+Sizes[resolutionIndex]);
            }
        }
        static void Capture(string file)
        {
            var old=RenderTexture.active;RenderTexture.active=target;
            var texture=new Texture2D(target.width,target.height,TextureFormat.RGB24,false);
            texture.ReadPixels(new Rect(0,0,target.width,target.height),0,0);texture.Apply();
            File.WriteAllBytes(Path.Combine(Output,file),texture.EncodeToPNG());
            UnityEngine.Object.DestroyImmediate(texture);RenderTexture.active=old;
        }
        static void ReviewLoading()
        {
            s.gameObject.SetActive(false);
            foreach(var size in Sizes)
            {
                var root=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Startup/RewardedLoading"));
                var safe=root.GetComponentInChildren<DeviceSafeViewport>();safe.enabled=false;
                var camera=safe.contentCamera;
                var fitted=DeviceSafeViewport.Fit(size,new Rect(0,96,size.x,size.y-252),safe.minimumDesignSize,6);
                s.worldCamera.targetTexture=null;UnityEngine.Object.DestroyImmediate(target);
                target=new RenderTexture(Mathf.RoundToInt(fitted.width),Mathf.RoundToInt(fitted.height),24);
                camera.targetTexture=target;camera.rect=new Rect(0,0,1,1);
                camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=new Color(.3f,.7f,1);
                foreach(var scaler in safe.scalers){scaler.scaleFactor=target.width/750f;scaler.GetComponent<Canvas>().scaleFactor=scaler.scaleFactor;}
                var view=root.GetComponent<RecoveredLoadingView>();
                foreach(float progress in new[]{0f,.5f,1f})
                {
                    view.SetProgress(progress);Canvas.ForceUpdateCanvases();camera.Render();
                    foreach(var text in root.GetComponentsInChildren<Text>())
                    {
                        text.rectTransform.GetWorldCorners(corners);
                        foreach(var c in corners){var p=camera.WorldToViewportPoint(c);if(p.x<-.01f||p.x>1.01f||p.y<-.01f||p.y>1.01f)throw new Exception("Loading label clipped: "+size);}
                    }
                    Capture(size+"-loading-"+progress+".png");checks++;
                }
                camera.targetTexture=null;UnityEngine.Object.DestroyImmediate(root);
            }
        }
        static void Finish(string error)
        {
            EditorApplication.update-=Tick;exitCode=error==null?0:1;
            File.WriteAllText(Output+"/result.txt",(error==null?"PASS":error)+"\nPage cases: "+checks);
            if(s)s.worldCamera.targetTexture=null;if(target)UnityEngine.Object.DestroyImmediate(target);
            EditorApplication.ExitPlaymode();
        }
    }
}
