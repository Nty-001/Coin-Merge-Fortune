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
    [InitializeOnLoad] public static class LoadingReskinValidation
    {
        const string Key="CoinMerge.LoadingReskin.Validation",Prefix="coinmerge.loading.reskin.disposable";
        static readonly List<string> checks=new List<string>();
        static int stage,variant,resolution,progressIndex,captures;static double deadline,next;static float lastProgress;static bool sawLoading,sawCompleted;
        static GameObject fixture;static Camera camera;static RenderTexture texture;
        static readonly Vector2Int[] sizes={new Vector2Int(941,1672),new Vector2Int(1080,2340),new Vector2Int(720,1280)};
        static readonly float[] values={0,.25f,.5f,1};
        static string Output=>Path.GetFullPath("Design/LoadingR1/Verification");
        [Serializable] sealed class Report{public bool passed;public string error,unityVersion;public int captures;public string[] checks;}
        static LoadingReskinValidation(){EditorApplication.playModeStateChanged+=State;}
        [MenuItem("Coin Merge/Reskin/Validate sky loading")]
        public static void Run()
        {
            Directory.CreateDirectory(Output);SessionState.SetBool(Key,true);SessionState.SetString(Key+".error","");
            var store=new PlayerStore(Prefix);store.ResetPlayer();store.ResetProfile();store.Save(new PlayerProgress{guideStep=5,fakeMoney=488.49});store.SaveProfile(new VersionProfile{country="US",cohort="B",rewardedVariant=true});
            EditorSceneManager.OpenScene("Assets/Scenes/RecoveredLoading.unity");UnityEngine.Object.FindObjectOfType<RecoveredStartup>().saveNamespace=Prefix;EditorApplication.EnterPlaymode();
        }
        static void State(PlayModeStateChange state)
        {
            if(!SessionState.GetBool(Key,false))return;
            if(state==PlayModeStateChange.EnteredPlayMode)
            {
                checks.Clear();captures=stage=variant=resolution=progressIndex=0;lastProgress=0;sawLoading=sawCompleted=false;deadline=EditorApplication.timeSinceStartup+45;next=0;
                EditorApplication.isPaused=false;Time.timeScale=1;Application.logMessageReceived+=Log;SceneManager.sceneLoaded+=Loaded;EditorApplication.update+=Tick;
            }
            if(state==PlayModeStateChange.EnteredEditMode)
            {
                var store=new PlayerStore(Prefix);store.ResetPlayer();store.ResetProfile();PlayerPrefs.DeleteKey(Prefix+".packaged.player");SessionState.SetBool(Key,false);EditorSceneManager.OpenScene("Assets/Scenes/RecoveredLoading.unity");
            }
        }
        static void Loaded(Scene scene,LoadSceneMode mode)
        {if(scene.name=="RecoveredLoading")foreach(var root in scene.GetRootGameObjects()){var startup=root.GetComponent<RecoveredStartup>();if(startup)startup.saveNamespace=Prefix;}}
        static void Log(string message,string stack,LogType type){if(type==LogType.Error||type==LogType.Exception)SessionState.SetString(Key+".error",message+"\n"+stack);}
        static void Require(bool condition,string message){if(!condition)throw new Exception(message);}
        static void Tick()
        {
            if(EditorApplication.timeSinceStartup<next)return;
            try
            {
                string error=SessionState.GetString(Key+".error","");if(error.Length>0)throw new Exception(error);
                if(EditorApplication.timeSinceStartup>deadline)throw new Exception("Loading verification timed out at stage "+stage);
                if(stage==0||stage==2)
                {
                    var startup=UnityEngine.Object.FindObjectOfType<RecoveredStartup>();
                    if(startup)
                    {
                        Require(startup.saveNamespace==Prefix,"Startup must use the disposable test namespace.");
                        Require(Mathf.Abs(startup.fakeTickSeconds-.1f)<.001f&&Mathf.Abs(startup.completedHoldSeconds-.3f)<.001f&&Mathf.Abs(startup.packagedSeconds-2)<.001f,"Original startup timing changed.");
                        if(startup.View)
                        {
                            sawLoading=true;Require(startup.Progress+.001f>=lastProgress,"Startup progress moved backwards.");lastProgress=startup.Progress;
                            Require(startup.View.fill.sprite&&startup.View.fill.sprite.name=="Fill","New bar is not bound during actual startup.");
                            Require(startup.View.percentage.text==Mathf.FloorToInt(startup.View.fill.fillAmount*100+.5f)+"%","Live startup percentage is not synchronized.");
                            if(startup.Completed)sawCompleted=true;
                        }
                        return;
                    }
                    if(stage==0)
                    {
                        var game=UnityEngine.Object.FindObjectOfType<RecoveredGameSession>();if(!game||game.Player==null)return;
                        Require(sawLoading&&sawCompleted&&lastProgress==1,"Rewarded startup did not display monotonic progress through 100%.");
                        game.automaticInput=false;checks.Add("Rewarded native startup reached 100%, retained original timing, and entered RecoveredMain.");stage=1;deadline=EditorApplication.timeSinceStartup+120;CreateFixture();return;
                    }
                    var packaged=UnityEngine.Object.FindObjectOfType<PackagedGameSession>();if(!packaged||packaged.Player==null)return;
                    Require(sawLoading&&lastProgress>.8f,"Packaged startup was not observed progressing.");
                    checks.Add("Packaged native startup preserved its two-second progress and entered RecoveredPackaged.");Finish(null);return;
                }
                if(stage==1)
                {
                    var view=fixture.GetComponent<RecoveredLoadingView>();float value=values[progressIndex];view.SetProgress(value);Canvas.ForceUpdateCanvases();camera.Render();
                    var track=fixture.transform.Find("LoadingScene/SkyLoadingPresentation/ProgressTrack");if(!track)foreach(var image in fixture.GetComponentsInChildren<Image>())if(image.name=="ProgressTrack"){track=image.transform;break;}
                    Require(track!=null,"Progress track missing.");var imageTrack=track.GetComponent<Image>();
                    AssertAspect(imageTrack,"track");AssertAspect(view.fill,"fill");
                    Require(view.fill.type==Image.Type.Filled&&view.fill.fillMethod==Image.FillMethod.Horizontal,"Progress must clip horizontally without scaling geometry.");
                    Require(Mathf.Abs(view.fill.fillAmount-value)<.0001f&&view.percentage.text==Mathf.RoundToInt(value*100)+"%","Progress value/percentage incorrect.");
                    Require(Mathf.Abs(track.localScale.x-track.localScale.y)<.0001f&&track.localScale.x==1,"Track scale deformed.");
                    var mesh=view.fill.canvasRenderer.GetMesh();if(value>0){Require(mesh&&mesh.vertexCount>0,"Fill mesh missing.");Require(Mathf.Abs(mesh.bounds.size.x-view.fill.rectTransform.rect.width*value)<1,"Fill geometry was stretched instead of clipped.");}
                    foreach(var text in fixture.GetComponentsInChildren<Text>())
                    {
                        var rendered=text.canvasRenderer.GetMesh();var rect=text.rectTransform.rect;if(rendered&&rendered.vertexCount>0){Require(rendered.bounds.min.x>=rect.xMin-1&&rendered.bounds.max.x<=rect.xMax+1&&rendered.bounds.min.y>=rect.yMin-1&&rendered.bounds.max.y<=rect.yMax+1,"Loading text overflow: "+text.name);}
                    }
                    bool caption=false;foreach(var text in fixture.GetComponentsInChildren<Text>())if(text.text==(variant==0?"loading...":"Loading..."))caption=true;Require(caption,"Original loading caption changed.");
                    string name=(variant==0?"Rewarded":"Packaged")+"_"+texture.width+"x"+texture.height+"_"+Mathf.RoundToInt(value*100)+".png";Capture(name);captures++;
                    checks.Add(name+": original sprite ratios, fixed track geometry, horizontal fill clipping, percentage and caption pass.");
                    view.SetProgress(-1);Require(view.fill.fillAmount==0,"Negative progress not clamped.");view.SetProgress(2);Require(view.fill.fillAmount==1,"Over-complete progress not clamped.");
                    if(++progressIndex>=values.Length){progressIndex=0;resolution++;if(resolution>=sizes.Length){resolution=0;variant++;}if(variant>=2){DestroyFixture();var store=new PlayerStore(Prefix);store.SaveProfile(new VersionProfile{country="US",cohort="A",rewardedVariant=false,contentMode=VersionRouting.Packaged});lastProgress=0;sawLoading=sawCompleted=false;stage=2;deadline=EditorApplication.timeSinceStartup+45;SceneManager.LoadScene("RecoveredLoading");return;}CreateFixture();}
                    next=EditorApplication.timeSinceStartup+.08;
                }
            }
            catch(Exception e){Finish(e.ToString());}
        }
        static void AssertAspect(Image image,string name)
        {Require(image.sprite&&image.preserveAspect,"Aspect preservation disabled for "+name);var r=image.rectTransform.rect;Require(Mathf.Abs(r.width/r.height-image.sprite.rect.width/image.sprite.rect.height)<.001f,"Sprite stretched: "+name);}
        static void CreateFixture()
        {
            DestroyFixture();fixture=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Startup/"+(variant==0?"RewardedLoading":"PackagedLoading")));
            foreach(var t in fixture.GetComponentsInChildren<Transform>(true))t.gameObject.layer=31;
            camera=new GameObject("LoadingVisualCapture",typeof(Camera)).GetComponent<Camera>();camera.orthographic=true;camera.orthographicSize=10;camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=Color.magenta;camera.cullingMask=1<<31;camera.transform.position=new Vector3(0,0,-100);camera.farClipPlane=200;
            var size=sizes[resolution];texture=new RenderTexture(size.x,size.y,24);camera.targetTexture=texture;
            var canvas=fixture.GetComponentInChildren<Canvas>();canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=camera;canvas.planeDistance=100;
            Canvas.ForceUpdateCanvases();next=EditorApplication.timeSinceStartup+.15;
        }
        static void Capture(string name)
        {var old=RenderTexture.active;RenderTexture.active=texture;var image=new Texture2D(texture.width,texture.height,TextureFormat.RGB24,false);image.ReadPixels(new Rect(0,0,texture.width,texture.height),0,0);image.Apply();File.WriteAllBytes(Path.Combine(Output,name),image.EncodeToPNG());RenderTexture.active=old;UnityEngine.Object.DestroyImmediate(image);}
        static void DestroyFixture(){if(camera){camera.targetTexture=null;UnityEngine.Object.DestroyImmediate(camera.gameObject);}if(texture)UnityEngine.Object.DestroyImmediate(texture);if(fixture)UnityEngine.Object.DestroyImmediate(fixture);}
        static void Finish(string error)
        {
            EditorApplication.update-=Tick;Application.logMessageReceived-=Log;SceneManager.sceneLoaded-=Loaded;DestroyFixture();
            File.WriteAllText(Path.Combine(Output,"play_mode.json"),JsonUtility.ToJson(new Report{passed=error==null,error=error,unityVersion=Application.unityVersion,captures=captures,checks=checks.ToArray()},true));
            Debug.Log(error==null?"LOADING_RESKIN_VALIDATED: "+captures+" captures":error);EditorApplication.ExitPlaymode();
        }
    }
}
