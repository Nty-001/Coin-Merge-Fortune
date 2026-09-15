using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
namespace CoinMerge.Recovery.Editor
{
    public static class MergeFeedbackBuilder
    {
        const string Runtime="Assets/Prefabs/Runtime/",ResourcesRoot="Assets/Resources/MergeFeedback/";
        [Serializable] sealed class AssetRef{public string asset;}
        [Serializable] sealed class SceneSettings{public AssetRef[] MergeIcon,MergeTimes;}
        static SpriteImportModel imports;
        static MergeFeedbackConfig config;
        static NativeSkeletonPlayer burst;
        static MergeStarImage star;
        static AnimationClip hand;
        [MenuItem("Coin Merge/Restore merge feedback and idle hand")]
        public static void Run()
        {
            Directory.CreateDirectory(ResourcesRoot+"Sprites");Directory.CreateDirectory(ResourcesRoot+"Audio");AssetDatabase.Refresh();
            imports=JsonUtility.FromJson<SpriteImportModel>(File.ReadAllText("Assets/Resources/Recovered/sprite_import.json"));
            var original=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/HotUpdate/scene/GameScene.prefab");
            SceneSettings settings=null;
            foreach(var node in original.GetComponentsInChildren<RecoveredNode>(true))foreach(var component in node.originalComponents)
                if(component.className=="GameScene")settings=JsonUtility.FromJson<SceneSettings>(component.rawJson.Replace("$asset","asset"));
            if(settings==null)throw new Exception("Missing original GameScene effect references");
            config=AssetDatabase.LoadAssetAtPath<MergeFeedbackConfig>("Assets/Config/Runtime/MergeFeedback.asset");
            if(!config){config=ScriptableObject.CreateInstance<MergeFeedbackConfig>();AssetDatabase.CreateAsset(config,"Assets/Config/Runtime/MergeFeedback.asset");}
            config.praisePaths=new string[settings.MergeIcon.Length];config.timesPaths=new string[settings.MergeTimes.Length];
            for(int i=0;i<config.praisePaths.Length;i++)config.praisePaths[i]=SpritePath(settings.MergeIcon[i].asset,"Praise_"+i);
            for(int i=0;i<config.timesPaths.Length;i++)config.timesPaths[i]=SpritePath(settings.MergeTimes[i].asset,"Times_"+(i+2));
            // Resolve the exact combo/star references through source components, not filename guesses.
            foreach(var node in original.GetComponentsInChildren<RecoveredNode>(true))
            {
                if(node.sourceObjectId==30)config.comboPath=CopySprite(node.GetComponent<Image>().sprite,"Combo");
                if(node.sourceObjectId==35)config.starPath=CopySprite(node.GetComponent<Image>().sprite,"Star");
            }
            config.mergeAudioPaths=new string[8];config.comboAudioPaths=new string[4];
            for(int i=0;i<8;i++)config.mergeAudioPaths[i]=AudioPath(i==0?"sfx_merge":"sfx_merge_"+(i+1));
            for(int i=0;i<4;i++)config.comboAudioPaths[i]=AudioPath("sfx_combo_"+(i+2));
            EditorUtility.SetDirty(config);AuthorTemplates();AuthorHandClip();
            string path=Runtime+"RecoveredMain.prefab";var root=PrefabUtility.LoadPrefabContents(path);
            try{Configure(root);PrefabUtility.SaveAsPrefabAsset(root,path);}finally{PrefabUtility.UnloadPrefabContents(root);}
            var scene=EditorSceneManager.OpenScene("Assets/Scenes/RecoveredMain.unity");
            foreach(var item in scene.GetRootGameObjects())if(item.GetComponent<RecoveredGameSession>())Configure(item);
            EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();Debug.Log("MERGE_FEEDBACK_IDLE_HAND_AUTHORED");
        }
        static string SpritePath(string uuid,string name)
        {
            foreach(var entry in imports.sprites)if(entry.uuid==uuid&&entry.variant=="HotUpdate")return CopySprite(AssetDatabase.LoadAssetAtPath<Sprite>(entry.path),name);
            throw new Exception("Missing sprite source "+uuid);
        }
        static string CopySprite(Sprite source,string name)
        {
            if(!source)throw new Exception("Missing original merge sprite "+name);
            string path=ResourcesRoot+"Sprites/"+name+".png";
            if(!File.Exists(path)&&!AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(source),path))throw new Exception("Unable to copy "+name);
            return "MergeFeedback/Sprites/"+name;
        }
        static string AudioPath(string name)
        {
            string path=ResourcesRoot+"Audio/"+name+".mp3";
            if(!File.Exists(path)&&!AssetDatabase.CopyAsset("Assets/Art/HotUpdate/Audio/Audio/"+name+".mp3",path))throw new Exception("Unable to copy "+name);
            return "MergeFeedback/Audio/"+name;
        }
        static void AuthorTemplates()
        {
            var source=AssetDatabase.LoadAssetAtPath<GameObject>(Runtime+"Skeletal/XX_TX.prefab");
            var root=(GameObject)PrefabUtility.InstantiatePrefab(source);PrefabUtility.UnpackPrefabInstance(root,PrefabUnpackMode.Completely,InteractionMode.AutomatedAction);
            root.name="MergeBurst";var player=root.GetComponent<NativeSkeletonPlayer>();player.defaultAnimation=config.burstAnimation;player.loop=false;player.playOnEnable=false;root.SetActive(false);
            burst=PrefabUtility.SaveAsPrefabAsset(root,Runtime+"MergeBurst.prefab").GetComponent<NativeSkeletonPlayer>();UnityEngine.Object.DestroyImmediate(root);
            root=new GameObject("MergeStar",typeof(RectTransform),typeof(CanvasRenderer),typeof(MergeStarImage));
            var image=root.GetComponent<MergeStarImage>();image.raycastTarget=false;image.rectTransform.sizeDelta=new Vector2(83,72);
            image.material=AssetDatabase.LoadAssetAtPath<Material>("Assets/Resources/Skeletal/NativeSkeletalUI.mat");root.SetActive(false);
            star=PrefabUtility.SaveAsPrefabAsset(root,Runtime+"MergeStar.prefab").GetComponent<MergeStarImage>();UnityEngine.Object.DestroyImmediate(root);
        }
        [Serializable] sealed class PositionKey{public float frame;public float[] value;}
        [Serializable] sealed class Props{public PositionKey[] position;}
        [Serializable] sealed class Curves{public Props props;}
        [Serializable] sealed class HandClip{public Curves curveData;}
        static void AuthorHandClip()
        {
            var source=JsonUtility.FromJson<HandClip>(File.ReadAllText("../04_Assets/HotUpdate/AnimationSource/b00460f7-cc4e-43f7-adba-350fc6c6bc97.json"));
            hand=new AnimationClip {name="handslip",legacy=true,frameRate=60,wrapMode=WrapMode.Loop};
            for(int axis=0;axis<2;axis++)
            {
                var keys=new Keyframe[source.curveData.props.position.Length];
                for(int i=0;i<keys.Length;i++){var key=source.curveData.props.position[i];keys[i]=new Keyframe(key.frame,key.value[axis]);}
                var curve=new AnimationCurve(keys);
                for(int i=0;i<keys.Length;i++){AnimationUtility.SetKeyLeftTangentMode(curve,i,AnimationUtility.TangentMode.Linear);AnimationUtility.SetKeyRightTangentMode(curve,i,AnimationUtility.TangentMode.Linear);}
                hand.SetCurve("",typeof(RectTransform),axis==0?"m_AnchoredPosition.x":"m_AnchoredPosition.y",curve);
            }
            string path="Assets/Config/Runtime/IdleHand.anim";var existing=AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
            if(existing){EditorUtility.CopySerialized(hand,existing);UnityEngine.Object.DestroyImmediate(hand);hand=existing;EditorUtility.SetDirty(hand);}else AssetDatabase.CreateAsset(hand,path);
        }
        static RectTransform Layer(string name,Transform parent,int order)
        {
            var node=new GameObject(name,typeof(RectTransform),typeof(Canvas));node.transform.SetParent(parent,false);
            var rect=(RectTransform)node.transform;rect.anchorMin=Vector2.zero;rect.anchorMax=Vector2.one;rect.offsetMin=rect.offsetMax=Vector2.zero;
            var canvas=node.GetComponent<Canvas>();canvas.overrideSorting=true;canvas.sortingOrder=order;canvas.additionalShaderChannels=AdditionalCanvasShaderChannels.TexCoord1;return rect;
        }
        static void Configure(GameObject root)
        {
            var session=root.GetComponent<RecoveredGameSession>();string uuid=session.moneyText.GetComponent<RecoveredNode>().sourceUuid;
            var nodes=new Dictionary<int,GameObject>();foreach(var node in root.GetComponentsInChildren<RecoveredNode>(true))if(node.sourceUuid==uuid)nodes.Add(node.sourceObjectId,node.gameObject);
            if(session.mergeFeedback)UnityEngine.Object.DestroyImmediate(session.mergeFeedback.gameObject);
            var layer=Layer("MergeFeedback",nodes[1].transform,21);var feedback=layer.gameObject.AddComponent<RecoveredMergeFeedback>();session.mergeFeedback=feedback;
            feedback.session=session;feedback.effectsRoot=layer;feedback.burstRoot=Layer("Bursts",layer,10);feedback.config=config;feedback.starPrefab=star;feedback.burstPrefab=burst;
            feedback.praise=nodes[63].GetComponent<Image>();feedback.combo=nodes[30].GetComponent<Image>();feedback.times=nodes[65].GetComponent<Image>();
            foreach(var image in new[]{feedback.praise,feedback.combo,feedback.times}){image.raycastTarget=false;image.enabled=true;image.type=Image.Type.Simple;image.gameObject.SetActive(false);}
            nodes[35].SetActive(false);feedback.sound=layer.gameObject.AddComponent<AudioSource>();feedback.sound.playOnAwake=false;feedback.sound.spatialBlend=0;
            var idle=layer.gameObject.AddComponent<RecoveredIdleGuide>();session.idleGuide=idle;idle.session=session;idle.guide=nodes[13];idle.guide.SetActive(false);
            idle.handAnimation=nodes[31].GetComponent<Animation>();if(!idle.handAnimation)idle.handAnimation=nodes[31].AddComponent<Animation>();
            idle.handAnimation.playAutomatically=false;idle.handAnimation.clip=hand;
            idle.handAnimation.AddClip(hand,"handslip");idle.handAnimation.wrapMode=WrapMode.Loop;idle.handAnimation.cullingType=AnimationCullingType.AlwaysAnimate;
            foreach(var image in idle.guide.GetComponentsInChildren<Graphic>(true))image.raycastTarget=false;
        }
    }
}
