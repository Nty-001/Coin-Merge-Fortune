using System;
using System.Collections.Generic;
using UnityEngine;
namespace CoinMerge.Recovery
{
    [Serializable] public struct NativeAttachedNode {public Transform target;public int bone;}
    public sealed class NativeSkeletonPlayer : MonoBehaviour
    {
        public string dataPath,defaultAnimation;
        public bool loop=true,playOnEnable=true;
        public float timeScale=1;
        public Animation animationComponent;
        public NativeSkeletonBone[] bones;
        public NativeSkeletonSlot[] slots;
        public NativeSkeletonGraphic graphic;
        public NativeAttachedNode[] attachedNodes=Array.Empty<NativeAttachedNode>();
        public NativeSkeletonData Data {get;private set;}
        public event Action Completed;
        readonly Dictionary<string,AnimationClip> clips=new Dictionary<string,AnimationClip>(4);
        NativeSkeletalAnimation current;
        AnimationState state;
        bool signalled;
        public int MaxVertices {get;private set;}
        void Awake(){Initialize();}
        void OnEnable(){if(playOnEnable&&!string.IsNullOrEmpty(defaultAnimation))Play(defaultAnimation,loop);}
        public void Initialize()
        {
            if(Data)return;
            Data=Resources.Load<NativeSkeletonData>(dataPath);
            if(!Data)throw new InvalidOperationException("Missing native skeleton data: "+dataPath);
            if(bones.Length!=Data.bones.Length||slots.Length!=Data.slots.Length)throw new InvalidOperationException("Native skeletal prefab bindings differ from source data.");
            for(int i=0;i<slots.Length;i++)slots[i].deform=new float[Data.slots[i].deformLength];
            foreach(var a in Data.attachments)MaxVertices=Math.Max(MaxVertices,a.VertexCount);
            EvaluateMatrices();graphic.Bind(this);
        }
        AnimationClip Clip(string name)
        {
            Initialize();current=null;
            foreach(var candidate in Data.animations)if(candidate.name==name){current=candidate;break;}
            if(current==null)throw new InvalidOperationException("Unknown source animation: "+name);
            if(!clips.TryGetValue(name,out var clip))
            {
                clip=Resources.Load<AnimationClip>(current.resourcePath);
                if(!clip)throw new InvalidOperationException("Missing native animation clip: "+current.resourcePath);
                clips.Add(name,clip);animationComponent.AddClip(clip,name);
            }
            return clip;
        }
        public void Play(string name,bool repeat)
        {
            Clip(name);loop=repeat;signalled=false;
            animationComponent.Play(name);state=animationComponent[name];state.wrapMode=repeat?WrapMode.Loop:WrapMode.ClampForever;state.speed=timeScale;state.time=0;
            animationComponent.Sample();EvaluateDeforms(0);EvaluateMatrices();graphic.SetVerticesDirty();
        }
        public void EvaluateAt(string name,float time)
        {
            // Same native clip sampling operation is also used by the numeric verifier.
            var clip=Clip(name);clip.SampleAnimation(gameObject,time);EvaluateDeforms(time);EvaluateMatrices();graphic.SetVerticesDirty();
        }
        public void ResetPose(string name)
        {
            animationComponent.Stop();state=null;signalled=false;EvaluateAt(name,0);
        }
        void LateUpdate()
        {
            if(state==null||current==null)return;
            float time=loop&&current.duration>0?state.time%current.duration:Mathf.Min(state.time,current.duration);
            EvaluateDeforms(time);EvaluateMatrices();graphic.SetVerticesDirty();
            if(!loop&&!signalled&&state.time>=current.duration){signalled=true;Completed?.Invoke();}
        }
        void EvaluateDeforms(float time)
        {
            for(int i=0;i<slots.Length;i++)slots[i].deformAttachment=-1;
            if(current==null)return;
            foreach(var d in current.deforms)
            {
                var slot=slots[d.slot];if(time<d.firstTime||slot.AttachmentIndex!=d.attachment)continue;
                slot.deformAttachment=d.attachment;
                for(int i=0;i<d.curves.Length;i++)slot.deform[i]=d.curves[i].Evaluate(time);
            }
        }
        public void EvaluateMatrices()
        {
            for(int i=0;i<bones.Length;i++)bones[i].Evaluate(bones[i].parentIndex<0?null:bones[bones[i].parentIndex]);
            foreach(var node in attachedNodes)
            {
                var bone=bones[node.bone];node.target.localPosition=new Vector3(bone.x,bone.y,0);
                node.target.localRotation=Quaternion.Euler(0,0,bone.rotation);node.target.localScale=new Vector3(bone.scaleX,bone.scaleY,1);
            }
        }
        public int FillWorldVertices(int attachmentIndex,Vector2[] target)
        {
            var a=Data.attachments[attachmentIndex];var slot=slots[a.slot];bool deform=slot.deformAttachment==attachmentIndex;
            for(int v=0;v<a.VertexCount;v++)
            {
                Vector2 point=Vector2.zero;
                for(int k=a.offsets[v];k<a.offsets[v+1];k++)
                {
                    float x=a.positions[k*2],y=a.positions[k*2+1];
                    if(deform){if(a.weighted){x+=slot.deform[k*2];y+=slot.deform[k*2+1];}else{x=slot.deform[k*2];y=slot.deform[k*2+1];}}
                    point+=bones[a.boneIndices[k]].TransformPoint(x,y)*a.weights[k];
                }
                target[v]=point;
            }
            return a.VertexCount;
        }
    }
}
