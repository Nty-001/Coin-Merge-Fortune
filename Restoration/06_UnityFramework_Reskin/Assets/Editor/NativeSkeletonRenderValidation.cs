using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace CoinMerge.Recovery.Editor
{
    public static class NativeSkeletonRenderValidation
    {
        [Serializable] sealed class Receipt {public string asset,animation,file;public int changedPixels,meshVertices;}
        [Serializable] sealed class Report {public bool passed;public string scope;public List<Receipt> renders=new List<Receipt>();public List<string> errors=new List<string>();}
        [MenuItem("Coin Merge/Render all native skeletal animations")]
        public static void Run()
        {
            var report=new Report {scope="Native Unity GPU render smoke check at one visible pose per animation; not pixel parity against original app."};
            Application.LogCallback collect=(message,stack,type)=>{if(type==LogType.Error||type==LogType.Exception)report.errors.Add(message);};
            Application.logMessageReceived+=collect;
            var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
            var camera=new GameObject("RenderCamera",typeof(Camera)).GetComponent<Camera>();camera.transform.position=new Vector3(0,0,-100);
            camera.orthographic=true;camera.orthographicSize=256;camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=Color.black;
            var target=new RenderTexture(512,512,24);camera.targetTexture=target;
            var canvas=new GameObject("RenderCanvas",typeof(RectTransform),typeof(Canvas)).GetComponent<Canvas>();
            canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=camera;canvas.planeDistance=100;canvas.additionalShaderChannels=AdditionalCanvasShaderChannels.TexCoord1;
            string folder="../07_Verification/NativeSkeletalRenders";Directory.CreateDirectory(folder);
            foreach(string path in Directory.GetFiles("Assets/Prefabs/Runtime/Skeletal","*.prefab"))
            {
                var root=(GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(path),canvas.transform);
                var player=root.GetComponent<NativeSkeletonPlayer>();player.Initialize();var vertices=new Vector2[player.MaxVertices];
                foreach(var animation in player.Data.animations)
                {
                    try
                    {
                        player.EvaluateAt(animation.name,animation.duration*.333f);
                        Vector2 min=new Vector2(float.MaxValue,float.MaxValue),max=new Vector2(float.MinValue,float.MinValue);
                        for(int slot=0;slot<player.slots.Length;slot++)
                        {
                            int index=player.slots[slot].AttachmentIndex;if(index<0||player.slots[slot].a<=0||player.Data.attachments[index].type==2)continue;
                            int count=player.FillWorldVertices(index,vertices);
                            for(int v=0;v<count;v++){min=Vector2.Min(min,vertices[v]);max=Vector2.Max(max,vertices[v]);}
                        }
                        float scale=450/Mathf.Max(max.x-min.x,max.y-min.y);
                        if(float.IsInfinity(scale)||float.IsNaN(scale)||scale<=0)throw new Exception("No visible pose bounds.");
                        var rect=(RectTransform)root.transform;rect.localScale=Vector3.one*scale;rect.anchoredPosition=-(min+max)*.5f*scale;
                        Canvas.ForceUpdateCanvases();camera.Render();RenderTexture.active=target;
                        var image=new Texture2D(512,512,TextureFormat.RGB24,false);image.ReadPixels(new Rect(0,0,512,512),0,0);image.Apply();
                        int changed=0;foreach(var pixel in image.GetPixels32())if(pixel.r>3||pixel.g>3||pixel.b>3)changed++;
                        var mesh=player.graphic.canvasRenderer.GetMesh();
                        string file=player.Data.name+"_"+animation.name+".png";File.WriteAllBytes(folder+"/"+file,image.EncodeToPNG());
                        report.renders.Add(new Receipt {asset=player.Data.name,animation=animation.name,file=file,changedPixels=changed,meshVertices=mesh.vertexCount});
                        if(changed<10||mesh.vertexCount==0)report.errors.Add("Empty native render: "+file);
                        UnityEngine.Object.DestroyImmediate(image);
                    }
                    catch(Exception ex){report.errors.Add(path+"/"+animation.name+": "+ex.Message);}
                }
                UnityEngine.Object.DestroyImmediate(root);
            }
            RenderTexture.active=null;camera.targetTexture=null;UnityEngine.Object.DestroyImmediate(target);
            Application.logMessageReceived-=collect;report.passed=report.errors.Count==0&&report.renders.Count==14;
            File.WriteAllText("../07_Verification/native_skeletal_render_validation.json",JsonUtility.ToJson(report,true));
            if(!report.passed)throw new Exception("Native skeletal GPU render check failed; see report.");
            Debug.Log("NATIVE_SKELETAL_GPU_RENDER_PASSED "+report.renders.Count);
        }
    }
}
