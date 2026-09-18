using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace CoinMerge.Recovery.Editor
{
    [InitializeOnLoad]
    public static class ModalEdgesReview
    {
        const string Key="ModalEdgesReview.running",Prefix="coinmerge.modaledges.disposable";
        const string Output="../Android/ModalEdgesReview";
        static RecoveredGameSession session;
        static FullScreenModalEdges edges;
        static RenderTexture foreground,background,full;
        static int scenario,phase,checks,result;
        static double next;
        static Rect fit;
        static Color[] baseline;
        static readonly Vector2Int[] sizes={new Vector2Int(900,1600),new Vector2Int(1080,2400),new Vector2Int(1536,2048)};
        static ModalEdgesReview(){EditorApplication.playModeStateChanged+=State;}
        public static void Run()
        {
            Directory.CreateDirectory(Output);EditorSceneManager.OpenScene("Assets/Scenes/RecoveredMain.unity");
            var s=UnityEngine.Object.FindObjectOfType<RecoveredGameSession>();s.saveNamespace=Prefix;s.automaticInput=false;
            var store=new PlayerStore(Prefix);store.ResetPlayer();store.ResetProfile();
            store.Save(new PlayerProgress{guideStep=9999,fakeMoney=410.85});
            store.SaveProfile(new VersionProfile{country="US",cohort="B",rewardedVariant=true,contentMode=2,cohortMode=2});
            SessionState.SetBool(Key,true);EditorApplication.EnterPlaymode();
        }
        static void State(PlayModeStateChange state)
        {
            if(!SessionState.GetBool(Key,false))return;
            if(state==PlayModeStateChange.EnteredPlayMode){scenario=phase=checks=0;next=EditorApplication.timeSinceStartup+1;EditorApplication.update+=Tick;}
            if(state==PlayModeStateChange.EnteredEditMode){SessionState.SetBool(Key,false);var store=new PlayerStore(Prefix);store.ResetPlayer();store.ResetProfile();EditorApplication.Exit(result);}
        }
        static void Tick()
        {
            if(EditorApplication.timeSinceStartup<next)return;
            next=EditorApplication.timeSinceStartup+.5;
            try
            {
                if(!session)
                {
                    session=UnityEngine.Object.FindObjectOfType<RecoveredGameSession>();session.automaticInput=false;
                    session.guideView.gameObject.SetActive(false);session.gm.Hide();
                    session.worldCamera.GetComponent<DeviceSafeViewport>().enabled=false;
                    edges=session.GetComponentInChildren<FullScreenModalEdges>();edges.enabled=false;
                }
                if(phase==0)
                {
                    Release();var size=sizes[scenario];
                    var safe=scenario==0?new Rect(0,0,size.x,size.y):scenario==1?new Rect(0,80,size.x,size.y-220):new Rect(50,40,size.x-100,size.y-120);
                    fit=DeviceSafeViewport.Fit(size,safe,new Vector2(750,1624),6);
                    foreground=new RenderTexture(Mathf.RoundToInt(fit.width),Mathf.RoundToInt(fit.height),24);
                    background=new RenderTexture(size.x,size.y,24);full=new RenderTexture(size.x,size.y,0);full.Create();
                    edges.backdropCamera.targetTexture=background;session.worldCamera.targetTexture=foreground;session.worldCamera.rect=new Rect(0,0,1,1);
                    foreach(var scaler in session.worldCamera.GetComponent<DeviceSafeViewport>().scalers)scaler.scaleFactor=foreground.width/750f;
                    phase++;return;
                }
                Canvas.ForceUpdateCanvases();session.playfieldLayout.Refresh();
                foreach(var popup in session.GetComponentsInChildren<RecoveredMenuPopup>())popup.Tick(1);
                var sizeNow=sizes[scenario];
                var viewport=new Rect(fit.x/sizeNow.x,fit.y/sizeNow.y,fit.width/sizeNow.x,fit.height/sizeNow.y);
                edges.Refresh(viewport);Canvas.ForceUpdateCanvases();edges.backdropCamera.Render();
                Graphics.Blit(background,foreground,viewport.size,viewport.position);session.worldCamera.Render();
                Graphics.Blit(background,full);Graphics.CopyTexture(foreground,0,0,0,0,foreground.width,foreground.height,full,0,0,Mathf.RoundToInt(fit.x),Mathf.RoundToInt(fit.y));
                var old=RenderTexture.active;RenderTexture.active=full;
                var texture=new Texture2D(full.width,full.height,TextureFormat.RGB24,false);
                texture.ReadPixels(new Rect(0,0,full.width,full.height),0,0);texture.Apply();RenderTexture.active=old;
                Color[] samples={texture.GetPixel(2,2),texture.GetPixel(full.width/2,full.height-2)};
                if(phase==1)baseline=samples;
                else foreach(var pair in new[]{0,1})
                {
                    float ratio=samples[pair].grayscale/Mathf.Max(.001f,baseline[pair].grayscale);
                    if(phase==4?Mathf.Abs(ratio-1)>.03f:ratio>.35f)throw new Exception("Edge pixel did not follow modal state: "+scenario+" phase="+phase+" ratio="+ratio);
                }
                float expected=phase==1||phase==4?0:phase==2?.78431374f:1-(1-.78431374f)*(1-.78431374f);
                if(Mathf.Abs(edges.top.color.a-expected)>.002f)throw new Exception("Mask alpha mismatch "+edges.top.color.a+" expected="+expected);
                File.WriteAllBytes(Output+"/"+sizeNow+"-"+phase+".png",texture.EncodeToPNG());UnityEngine.Object.DestroyImmediate(texture);checks++;
                if(phase==1)session.rewardView.Show(1,9);
                if(phase==2)session.menus.Show(RecoveredMainMenus.Settings);
                if(phase==3){session.rewardView.gameObject.SetActive(false);session.menus.CloseAll();}
                phase++;if(phase==5){phase=0;scenario++;if(scenario==sizes.Length)Finish(null);}
            }
            catch(Exception e){Finish(e.ToString());}
        }
        static void Release()
        {
            if(session)session.worldCamera.targetTexture=null;if(edges)edges.backdropCamera.targetTexture=null;
            foreach(var rt in new[]{foreground,background,full})if(rt)UnityEngine.Object.DestroyImmediate(rt);
        }
        static void Finish(string error)
        {
            EditorApplication.update-=Tick;result=error==null?0:1;Release();
            File.WriteAllText(Output+"/result.txt",(error??"PASS")+"\nRendered state checks: "+checks);EditorApplication.ExitPlaymode();
        }
    }
}
