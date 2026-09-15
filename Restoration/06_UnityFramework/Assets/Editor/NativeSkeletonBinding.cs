using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace CoinMerge.Recovery.Editor
{
    // Author-time projection of the original component onto a native child prefab.
    public static class NativeSkeletonBinding
    {
        [Serializable] sealed class AssetReference {public string asset;}
        [Serializable] sealed class Settings
        {
            public AssetReference skeletonData;
            public string defaultAnimation;
            public bool loop=true;
            public float timeScale=1;
        }
        static readonly Dictionary<string,GameObject> prefabs=new Dictionary<string,GameObject>();
        public static void Bind(GameObject root)
        {
            if(prefabs.Count==0)
                foreach(string guid in AssetDatabase.FindAssets("t:NativeSkeletonData",new[]{"Assets/Resources/Skeletal/Data"}))
                {
                    var data=AssetDatabase.LoadAssetAtPath<NativeSkeletonData>(AssetDatabase.GUIDToAssetPath(guid));
                    var prefab=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Runtime/Skeletal/"+data.name+".prefab");
                    if(!prefab)throw new InvalidOperationException("Missing authored native skeleton: "+data.name);
                    prefabs.Add(data.sourceUuid,prefab);
                }
            foreach(var node in root.GetComponentsInChildren<RecoveredNode>(true))
                foreach(var component in node.originalComponents)
                {
                    if(component.type!="sp.Skeleton")continue;
                    string json=component.rawJson.Replace("\"_N$skeletonData\"","\"skeletonData\"").Replace("\"$asset\"","\"asset\"");
                    var settings=new Settings();JsonUtility.FromJsonOverwrite(json,settings);
                    // Some original components (e.g. FailDialog node 15) have no skeleton asset assigned.
                    if(string.IsNullOrEmpty(settings.skeletonData?.asset))continue;
                    if(!prefabs.TryGetValue(settings.skeletonData.asset,out var prefab))
                        throw new InvalidOperationException("No native skeleton for source node "+node.sourceObjectId);
                    var child=(GameObject)PrefabUtility.InstantiatePrefab(prefab,node.transform);
                    child.name="NativeAnimation";
                    child.transform.SetAsFirstSibling();
                    var rect=(RectTransform)child.transform;rect.anchoredPosition=Vector2.zero;
                    var player=child.GetComponent<NativeSkeletonPlayer>();
                    player.defaultAnimation=settings.defaultAnimation;player.playOnEnable=!string.IsNullOrEmpty(settings.defaultAnimation);
                    player.loop=settings.loop;player.timeScale=settings.timeScale;
                    var data=Resources.Load<NativeSkeletonData>(player.dataPath);
                    var followers=new List<NativeAttachedNode>();
                    foreach(var transform in node.GetComponentsInChildren<Transform>(true))
                    {
                        const string prefix="ATTACHED_NODE:";if(!transform.name.StartsWith(prefix,StringComparison.Ordinal))continue;
                        string name=transform.name.Substring(prefix.Length);int index=Array.FindIndex(data.bones,b=>b.name==name);
                        if(index<0)throw new InvalidOperationException("Unknown attached source bone: "+name);
                        var source=data.bones[index];
                        if(source.parent>=0&&transform.parent.name!=prefix+data.bones[source.parent].name)
                            throw new InvalidOperationException("Attached source bone parent differs: "+name);
                        foreach(var animation in data.animations)
                        {
                            var clip=Resources.Load<AnimationClip>(animation.resourcePath);
                            string path=AnimationUtility.CalculateTransformPath(player.bones[index].transform,player.transform);
                            foreach(string property in new[]{"shearX","shearY"})
                            {
                                var curve=AnimationUtility.GetEditorCurve(clip,EditorCurveBinding.FloatCurve(path,typeof(NativeSkeletonBone),property));
                                if(curve!=null)foreach(var key in curve.keys)if(Mathf.Abs(key.value)>.0001f)
                                    throw new InvalidOperationException("Attached UI requires an explicit shear projection: "+name);
                            }
                        }
                        followers.Add(new NativeAttachedNode {target=transform,bone=index});
                    }
                    player.attachedNodes=followers.ToArray();
                    child.SetActive(component.enabled);
                }
            foreach(var canvas in root.GetComponentsInChildren<Canvas>(true))
                canvas.additionalShaderChannels|=AdditionalCanvasShaderChannels.TexCoord1;
        }
    }
}
