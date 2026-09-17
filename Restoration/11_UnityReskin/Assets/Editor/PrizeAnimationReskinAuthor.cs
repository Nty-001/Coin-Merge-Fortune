using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
namespace CoinMerge.Recovery.Editor
{
    [InitializeOnLoad] public static class PrizeAnimationReskinAuthor
    {
        const string Output="Design/WheelLeverR1/Verification";
        static PrizeAnimationReskinAuthor(){EditorApplication.update+=Poll;}
        static void Poll()
        {
            const string request="Temp/PrizeAnimationReskin.request";
            if(!File.Exists(request)||EditorApplication.isCompiling||EditorApplication.isUpdating)return;
            if(EditorApplication.isPlaying){EditorApplication.ExitPlaymode();return;}
            if(EditorApplication.isPlayingOrWillChangePlaymode)return;
            File.Delete(request);
            try{Run();PrizeAnimationReskinValidation.Run();}
            catch(Exception e){Directory.CreateDirectory(Output);File.WriteAllText(Output+"/author_error.txt",e.ToString());Debug.LogException(e);}
        }
        [MenuItem("Coin Merge/Reskin/Restore prize machine lever and highest chip")]
        public static void Run()
        {
            if(EditorApplication.isPlaying)throw new InvalidOperationException("Stop Play before authoring.");
            Directory.CreateDirectory(Output);
            var previous=UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            if(previous.isDirty){Directory.CreateDirectory("Temp/PrizeAnimationBackup");EditorSceneManager.SaveScene(previous,"Temp/PrizeAnimationBackup/BeforeAuthoring.unity",true);}
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            foreach(var file in Directory.GetFiles("Assets/Resources/WheelReskin","*.png"))
            {
                var importer=(TextureImporter)AssetImporter.GetAtPath(file.Replace('\\','/'));
                importer.textureType=TextureImporterType.Sprite;importer.spriteImportMode=SpriteImportMode.Single;importer.spritePixelsPerUnit=32;
                importer.mipmapEnabled=false;importer.alphaIsTransparency=true;importer.npotScale=TextureImporterNPOTScale.None;
                importer.textureCompression=TextureImporterCompression.Uncompressed;importer.maxTextureSize=2048;importer.filterMode=FilterMode.Bilinear;importer.wrapMode=TextureWrapMode.Clamp;
                var settings=new TextureImporterSettings();importer.ReadTextureSettings(settings);settings.spriteMeshType=SpriteMeshType.FullRect;importer.SetTextureSettings(settings);importer.SaveAndReimport();
            }
            string skeletonPath="Assets/Prefabs/Runtime/Skeletal/GuangH_TX.prefab";
            var effect=PrefabUtility.LoadPrefabContents(skeletonPath);
            try{Highest(effect.GetComponent<NativeSkeletonPlayer>());PrefabUtility.SaveAsPrefabAsset(effect,skeletonPath);}finally{PrefabUtility.UnloadPrefabContents(effect);}
            string prefab="Assets/Prefabs/Runtime/RecoveredMain.prefab";var root=PrefabUtility.LoadPrefabContents(prefab);
            try{Apply(root);PrefabUtility.SaveAsPrefabAsset(root,prefab);}finally{PrefabUtility.UnloadPrefabContents(root);}
            var scene=EditorSceneManager.OpenScene("Assets/Scenes/RecoveredMain.unity");
            foreach(var obj in scene.GetRootGameObjects())if(obj.GetComponent<RecoveredGameSession>())Apply(obj);
            EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();Debug.Log("PRIZE_ANIMATION_RESKIN_AUTHORED");
        }
        static Image ImageAt(Transform parent,string name,Vector2 position,Vector2 size,Vector2 pivot)
        {
            var old=parent.Find(name);var obj=old?old.gameObject:new GameObject(name,typeof(RectTransform),typeof(CanvasRenderer),typeof(Image));
            obj.transform.SetParent(parent,false);var image=obj.GetComponent<Image>();var rect=image.rectTransform;
            rect.anchorMin=rect.anchorMax=Vector2.one*.5f;rect.pivot=pivot;rect.localRotation=Quaternion.identity;rect.localScale=Vector3.one;rect.anchoredPosition=position;rect.sizeDelta=size;
            image.raycastTarget=false;image.preserveAspect=true;image.type=Image.Type.Simple;image.color=Color.white;return image;
        }
        static void Highest(NativeSkeletonPlayer player)
        {
            // C1 is the last render slot, above the original ring and star layers; there are no C1 mesh deforms.
            player.graphic.hiddenSlots=new[]{21};
            var image=ImageAt(player.transform,"ApprovedHighestChip",Vector2.zero,new Vector2(186,186),Vector2.one*.5f);
            var replacement=image.GetComponent<RecoveredSkeletalSprite>();if(!replacement)replacement=image.gameObject.AddComponent<RecoveredSkeletalSprite>();
            replacement.player=player;replacement.boneIndex=1;replacement.slotIndex=21;replacement.image=image;replacement.resourcePath="Gameplay/Coins/2000";
            image.sprite=null;
        }
        static void Apply(GameObject root)
        {
            var session=root.GetComponent<RecoveredGameSession>();var wheel=session.wheelView;
            Image machine=null;foreach(var candidate in wheel.GetComponentsInChildren<Image>(true))if(candidate.name=="SkyPrizeMachine"){machine=candidate;break;}
            if(!machine)throw new Exception("Missing authored sky machine");
            foreach(var loader in root.GetComponentsInChildren<RecoveredMenuArt>(true))
                foreach(var binding in loader.images)
                {
                    if(binding.image==machine)binding.resourcePath="WheelReskin/Body";
                    if(binding.resourcePath=="RulesReskin/Chip2000")binding.resourcePath="Gameplay/Coins/2000";
                }
            float u=machine.rectTransform.rect.width/1156f;
            var shaft=ImageAt(machine.transform,"LeverShaft",new Vector2(1034-578,680-580)*u,new Vector2(78,183)*u,new Vector2(.205f,.025f));
            var knob=ImageAt(machine.transform,"LeverKnob",new Vector2(1064-578,680-362.5f)*u,new Vector2(144,145)*u,Vector2.one*.5f);
            var art=machine.GetComponent<RecoveredMenuArt>();if(!art)art=machine.gameObject.AddComponent<RecoveredMenuArt>();
            art.images=new[]{new MenuImageBinding{image=shaft,resourcePath="WheelReskin/Shaft"},new MenuImageBinding{image=knob,resourcePath="WheelReskin/Knob"}};
            var lever=machine.GetComponent<RecoveredWheelLever>();if(!lever)lever=machine.gameObject.AddComponent<RecoveredWheelLever>();
            lever.sourceKnob=wheel.backgroundAnimation.bones[5];lever.knob=knob.rectTransform;lever.shaft=shaft.rectTransform;
            lever.knobRest=knob.rectTransform.anchoredPosition;lever.shaftRest=shaft.rectTransform.anchoredPosition;
            lever.sourceRestY=586.68f;lever.sourcePressedY=502.22f;
            // Calibrate against the source knob-to-pivot distance, keeping the new art's rest pose.
            lever.travel=(580-362.5f)*u*(84.46f/(586.68f-415.27f));
            lever.shaftCompressedScale=1-lever.travel/(183*u);
            wheel.backgroundAnimation.playOnEnable=false;
        }
    }
}
