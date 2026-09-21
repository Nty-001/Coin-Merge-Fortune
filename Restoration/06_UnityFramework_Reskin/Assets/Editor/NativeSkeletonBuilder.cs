using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace CoinMerge.Recovery.Editor
{
    public static class NativeSkeletonBuilder
    {
        static string Safe(string name)=>name.Replace('/','_').Replace('\\','_').Replace(':','_');
        public static AnimationCurve Curve(NativeCurveModel source,float duration)
        {
            var values=new List<NativeCurveKey>(source.keys);
            if(values[values.Count-1].time<duration)values.Add(new NativeCurveKey {time=duration,value=values[values.Count-1].value,hold=true});
            var keys=new Keyframe[values.Count];
            for(int i=0;i<keys.Length;i++)keys[i]=new Keyframe(values[i].time,values[i].value);
            for(int i=0;i<keys.Length-1;i++)
            {
                float delta=keys[i+1].time-keys[i].time;
                float slope=delta>0?(keys[i+1].value-keys[i].value)/delta:0;
                keys[i].outTangent=values[i].hold?float.PositiveInfinity:slope;
                keys[i+1].inTangent=values[i].hold?float.PositiveInfinity:slope;
            }
            return new AnimationCurve(keys);
        }
        [MenuItem("Coin Merge/Author native skeletal prefabs and animation clips")]
        public static void Run()
        {
            Directory.CreateDirectory("Assets/Resources/Skeletal/Data");Directory.CreateDirectory("Assets/Resources/Skeletal/Clips");Directory.CreateDirectory("Assets/Prefabs/Runtime/Skeletal");
            AssetDatabase.Refresh();
            var shader=Shader.Find("CoinMerge/NativeSkeletalUI");if(!shader)throw new Exception("Native skeletal UI shader did not compile.");
            const string materialPath="Assets/Resources/Skeletal/NativeSkeletalUI.mat";
            var material=AssetDatabase.LoadAssetAtPath<Material>(materialPath);
            if(!material){material=new Material(shader);AssetDatabase.CreateAsset(material,materialPath);}
            int count=0,clips=0;
            foreach(var path in Directory.GetFiles("../05_PrefabModel/NativeSkeletal","*.json"))
            {
                var model=JsonUtility.FromJson<NativeSkeletonModel>(File.ReadAllText(path));
                var texture=(TextureImporter)AssetImporter.GetAtPath("Assets/Resources/"+model.texturePath+".png");
                texture.textureType=TextureImporterType.Default;texture.mipmapEnabled=false;texture.alphaIsTransparency=false;
                texture.textureCompression=TextureImporterCompression.Uncompressed;texture.wrapMode=TextureWrapMode.Clamp;texture.SaveAndReimport();
                string dataPath="Assets/Resources/Skeletal/Data/"+model.name+".asset";
                var data=AssetDatabase.LoadAssetAtPath<NativeSkeletonData>(dataPath);
                if(!data){data=ScriptableObject.CreateInstance<NativeSkeletonData>();AssetDatabase.CreateAsset(data,dataPath);}
                data.sourceUuid=model.uuid;data.texturePath=model.texturePath;data.bones=model.bones;data.slots=model.slots;data.attachments=model.attachments;
                var root=new GameObject(model.name,typeof(RectTransform),typeof(CanvasRenderer),typeof(Animation),typeof(NativeSkeletonGraphic),typeof(NativeSkeletonPlayer));
                var player=root.GetComponent<NativeSkeletonPlayer>();player.dataPath="Skeletal/Data/"+model.name;player.animationComponent=root.GetComponent<Animation>();
                player.animationComponent.playAutomatically=false;player.animationComponent.cullingType=AnimationCullingType.AlwaysAnimate;
                player.defaultAnimation=model.animations[0].name;player.graphic=root.GetComponent<NativeSkeletonGraphic>();player.graphic.material=material;player.graphic.raycastTarget=false;
                player.bones=new NativeSkeletonBone[model.bones.Length];player.slots=new NativeSkeletonSlot[model.slots.Length];
                var boneRoot=new GameObject("Bones").transform;boneRoot.SetParent(root.transform,false);
                var slotRoot=new GameObject("Slots").transform;slotRoot.SetParent(root.transform,false);
                for(int i=0;i<model.bones.Length;i++)
                {
                    var source=model.bones[i];var node=new GameObject("Bone_"+i.ToString("D3")+"_"+Safe(source.name));
                    node.transform.SetParent(source.parent<0?boneRoot:player.bones[source.parent].transform,false);
                    var bone=node.AddComponent<NativeSkeletonBone>();player.bones[i]=bone;bone.parentIndex=source.parent;
                    bone.x=source.x;bone.y=source.y;bone.rotation=source.rotation;bone.scaleX=source.scaleX;bone.scaleY=source.scaleY;bone.shearX=source.shearX;bone.shearY=source.shearY;
                    node.transform.localPosition=new Vector3(source.x,source.y);node.transform.localRotation=Quaternion.Euler(0,0,source.rotation);node.transform.localScale=new Vector3(source.scaleX,source.scaleY,1);
                }
                for(int i=0;i<model.slots.Length;i++)
                {
                    var source=model.slots[i];var node=new GameObject("Slot_"+i.ToString("D3")+"_"+Safe(source.name));node.transform.SetParent(slotRoot,false);
                    var slot=node.AddComponent<NativeSkeletonSlot>();player.slots[i]=slot;slot.r=source.color[0];slot.g=source.color[1];slot.b=source.color[2];slot.a=source.color[3];slot.attachment=source.initialAttachment;
                }
                data.animations=new NativeSkeletalAnimation[model.animations.Length];
                for(int i=0;i<model.animations.Length;i++)
                {
                    var source=model.animations[i];var clip=new AnimationClip {name=source.name,legacy=true,frameRate=60};
                    foreach(var curve in source.curves)
                    {
                        var target=curve.target==0?player.bones[curve.index].transform:player.slots[curve.index].transform;
                        clip.SetCurve(AnimationUtility.CalculateTransformPath(target,root.transform),curve.target==0?typeof(NativeSkeletonBone):typeof(NativeSkeletonSlot),curve.property,Curve(curve,source.duration));
                    }
                    string resource="Skeletal/Clips/"+model.name+"/"+Safe(source.name),clipPath="Assets/Resources/"+resource+".anim";
                    Directory.CreateDirectory(Path.GetDirectoryName(clipPath));
                    var previous=AssetDatabase.LoadAssetAtPath<AnimationClip>(clipPath);
                    if(previous){EditorUtility.CopySerialized(clip,previous);UnityEngine.Object.DestroyImmediate(clip);EditorUtility.SetDirty(previous);}
                    else AssetDatabase.CreateAsset(clip,clipPath);
                    var animation=new NativeSkeletalAnimation {name=source.name,duration=source.duration,resourcePath=resource,deforms=new NativeDeformAnimation[source.deforms.Length]};
                    for(int d=0;d<source.deforms.Length;d++)
                    {
                        var deform=source.deforms[d];var target=new NativeDeformAnimation {slot=deform.slot,attachment=deform.attachment,firstTime=deform.firstTime,curves=new AnimationCurve[deform.curves.Length]};
                        for(int k=0;k<deform.curves.Length;k++)target.curves[k]=Curve(deform.curves[k],source.duration);
                        animation.deforms[d]=target;
                    }
                    data.animations[i]=animation;clips++;
                }
                EditorUtility.SetDirty(data);
                PrefabUtility.SaveAsPrefabAsset(root,"Assets/Prefabs/Runtime/Skeletal/"+model.name+".prefab");UnityEngine.Object.DestroyImmediate(root);count++;
            }
            AssetDatabase.SaveAssets();AssetDatabase.Refresh();
            Debug.Log("NATIVE_SKELETAL_ASSETS_AUTHORED "+count+" prefabs, "+clips+" AnimationClips; numeric parity verification follows.");
            NativeSkeletonValidation.Run();
        }
    }
}
