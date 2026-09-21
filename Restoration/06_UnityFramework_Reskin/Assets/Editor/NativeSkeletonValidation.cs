using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace CoinMerge.Recovery.Editor
{
    public static class NativeSkeletonValidation
    {
        [Serializable] sealed class Oracle {public Asset[] assets;}
        [Serializable] sealed class Asset {public string name,uuid;public Clip[] animations;}
        [Serializable] sealed class Clip {public string name;public Frame[] frames;}
        [Serializable] sealed class Frame {public float time;public Bone[] bones;public Slot[] slots;}
        [Serializable] sealed class Bone {public string name;public float a,b,c,d,x,y;}
        [Serializable] sealed class Slot {public string name,attachment;public float[] vertices,color;}
        [Serializable] sealed class Report {public bool passed;public int assets,frames,vertexCoordinates,boneComponents;public float maxVertexError,maxBoneError,maxColorError;public List<string> errors=new List<string>();}
        [MenuItem("Coin Merge/Compare native bones and meshes with source poses")]
        public static void Run()
        {
            var oracle=JsonUtility.FromJson<Oracle>(File.ReadAllText("../07_Verification/skeletal_reference_poses.json"));var report=new Report();
            foreach(var asset in oracle.assets)
            {
                GameObject root=null;
                try
                {
                    root=PrefabUtility.LoadPrefabContents("Assets/Prefabs/Runtime/Skeletal/"+asset.name+".prefab");
                    var player=root.GetComponent<NativeSkeletonPlayer>();player.Initialize();var vertices=new Vector2[player.MaxVertices];report.assets++;
                    var names=new Dictionary<string,int>();for(int i=0;i<player.Data.slots.Length;i++)names.Add(player.Data.slots[i].name,i);
                    foreach(var clip in asset.animations)foreach(var frame in clip.frames)
                    {
                        player.EvaluateAt(clip.name,frame.time);report.frames++;
                        for(int i=0;i<frame.bones.Length;i++)
                        {
                            var a=player.bones[i];var e=frame.bones[i];
                            float delta=Mathf.Max(Mathf.Abs(a.A-e.a),Mathf.Abs(a.B-e.b),Mathf.Abs(a.C-e.c),Mathf.Abs(a.D-e.d),Mathf.Abs(a.WorldX-e.x),Mathf.Abs(a.WorldY-e.y));
                            report.boneComponents+=6;report.maxBoneError=Mathf.Max(report.maxBoneError,delta);
                            if(delta>.02f&&report.errors.Count<30)report.errors.Add(asset.name+"/"+clip.name+" time="+frame.time+" bone="+e.name+" error="+delta);
                        }
                        foreach(var expected in frame.slots)
                        {
                            int slotIndex=names[expected.name];var slot=player.slots[slotIndex];int index=slot.AttachmentIndex;
                            if(index<0||player.Data.attachments[index].name!=expected.attachment){if(report.errors.Count<30)report.errors.Add(asset.name+" attachment mismatch: "+expected.name+" time="+frame.time);continue;}
                            var tint=slot.Tint;float colorError=Mathf.Max(Mathf.Abs(tint.r-expected.color[0]),Mathf.Abs(tint.g-expected.color[1]),Mathf.Abs(tint.b-expected.color[2]),Mathf.Abs(tint.a-expected.color[3]));report.maxColorError=Mathf.Max(report.maxColorError,colorError);
                            int count=player.FillWorldVertices(index,vertices);
                            if(count*2!=expected.vertices.Length)throw new Exception("Vertex count differs for "+expected.name);
                            for(int i=0;i<count;i++)
                            {
                                float delta=Mathf.Max(Mathf.Abs(vertices[i].x-expected.vertices[2*i]),Mathf.Abs(vertices[i].y-expected.vertices[2*i+1]));
                                report.vertexCoordinates+=2;report.maxVertexError=Mathf.Max(report.maxVertexError,delta);
                                if(delta>.03f&&report.errors.Count<30)report.errors.Add(asset.name+"/"+clip.name+" time="+frame.time+" slot="+expected.name+" vertex="+i+" error="+delta);
                            }
                        }
                    }
                }
                catch(Exception ex){report.errors.Add(asset.name+": "+ex.Message);}
                finally{if(root)PrefabUtility.UnloadPrefabContents(root);}
            }
            report.passed=report.errors.Count==0&&report.maxColorError<.001f;
            File.WriteAllText("../07_Verification/native_skeletal_validation.json",JsonUtility.ToJson(report,true));
            if(!report.passed)throw new Exception("Native skeletal numeric comparison failed; see native_skeletal_validation.json");
            Debug.Log("NATIVE_SKELETAL_NUMERIC_VALIDATION_PASSED "+report.frames+" poses, "+report.vertexCoordinates+" vertex coordinates.");
        }
    }
}
