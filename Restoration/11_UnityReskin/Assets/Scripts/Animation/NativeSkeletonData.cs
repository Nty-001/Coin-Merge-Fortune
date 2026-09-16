using System;
using UnityEngine;
namespace CoinMerge.Recovery
{
    [Serializable] public sealed class NativeBoneModel
    {public string name;public int parent;public float x,y,rotation,scaleX=1,scaleY=1,shearX,shearY;}
    [Serializable] public sealed class NativeSlotModel
    {public string name;public int bone,initialAttachment,blend,deformLength;public float[] color;}
    [Serializable] public sealed class NativeAttachmentModel
    {
        public string name,skin;public int slot,type,endSlot;
        public bool weighted,clipConvex;
        public float[] color,positions,weights,uvs;
        public int[] offsets,boneIndices,triangles,clipTriangles;
        public int VertexCount=>offsets.Length-1;
    }
    [Serializable] public sealed class NativeCurveKey {public float time,value;public bool hold;}
    [Serializable] public sealed class NativeCurveModel {public int target,index;public string property;public NativeCurveKey[] keys;}
    [Serializable] public sealed class NativeDeformModel {public int slot,attachment;public float firstTime;public NativeCurveModel[] curves;}
    [Serializable] public sealed class NativeAnimationModel {public string name;public float duration;public NativeCurveModel[] curves;public NativeDeformModel[] deforms;}
    [Serializable] public sealed class NativeSkeletonModel
    {public string name,uuid,texturePath;public NativeBoneModel[] bones;public NativeSlotModel[] slots;public NativeAttachmentModel[] attachments;public NativeAnimationModel[] animations;}
    [Serializable] public sealed class NativeDeformAnimation
    {public int slot,attachment;public float firstTime;public AnimationCurve[] curves;}
    [Serializable] public sealed class NativeSkeletalAnimation
    {public string name,resourcePath;public float duration;public NativeDeformAnimation[] deforms;}
    public sealed class NativeSkeletonData : ScriptableObject
    {
        public string sourceUuid,texturePath;
        public NativeBoneModel[] bones;
        public NativeSlotModel[] slots;
        public NativeAttachmentModel[] attachments;
        public NativeSkeletalAnimation[] animations;
    }
}
