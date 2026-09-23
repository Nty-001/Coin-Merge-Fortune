using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
namespace CoinMerge.Recovery.Editor
{
    [InitializeOnLoad] public static class RewardMotionAuthor
    {
        const string Prefab="Assets/Prefabs/Runtime/RecoveredMain.prefab",Scene="Assets/Scenes/RecoveredMain.unity";
        const string Request="Temp/RewardMotion.request";
        static RewardMotionAuthor(){EditorApplication.update+=Poll;}
        static void Poll()
        {
            if(!File.Exists(Request)||EditorApplication.isCompiling||EditorApplication.isUpdating)return;
            if(EditorApplication.isPlaying){EditorApplication.ExitPlaymode();return;}
            if(EditorApplication.isPlayingOrWillChangePlaymode)return;
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);if(EditorApplication.isCompiling||EditorApplication.isUpdating)return;
            File.Delete(Request);try{Run();File.WriteAllText("Temp/RewardMotionApplied.txt","OK: clean reward plates, localized cash, original motion, undistorted ratios and lever joint applied.");}catch(Exception e){File.WriteAllText("Temp/RewardMotionApplied.txt",e.ToString());Debug.LogException(e);}
        }
        [MenuItem("Coin Merge/Reskin/Restore source reward and hint motion")]
        public static void Run()
        {
            if(EditorApplication.isPlayingOrWillChangePlaymode)throw new InvalidOperationException("Author in Edit Mode.");
            var current=UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            if(current.isDirty){Directory.CreateDirectory("Temp/RewardMotionBackup");EditorSceneManager.SaveScene(current,"Temp/RewardMotionBackup/UnsavedScene.unity",true);}
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            const string folder="Assets/Resources/RewardMotion";Directory.CreateDirectory(folder);AssetDatabase.Refresh();
            foreach(var file in Directory.GetFiles(folder+"/Flat","*.png"))
            {
                var importer=(TextureImporter)AssetImporter.GetAtPath(file.Replace('\\','/'));
                importer.textureType=TextureImporterType.Default;importer.mipmapEnabled=false;importer.npotScale=TextureImporterNPOTScale.None;
                importer.textureCompression=TextureImporterCompression.Uncompressed;importer.maxTextureSize=2048;
                importer.filterMode=FilterMode.Bilinear;importer.wrapMode=TextureWrapMode.Clamp;importer.SaveAndReimport();
            }
            var shader=AssetDatabase.LoadAssetAtPath<Shader>("Assets/Shaders/RecoveredRewardLight.shader");
            if(!shader||ShaderUtil.ShaderHasError(shader))throw new InvalidOperationException("Reward light shader failed import");
            var material=AssetDatabase.LoadAssetAtPath<Material>(folder+"/Light.mat");
            if(!material){material=new Material(shader);AssetDatabase.CreateAsset(material,folder+"/Light.mat");}else material.shader=shader;
            material.SetTexture("_GlowTex",AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Art/HotUpdate/Sprites/GameDialog/common/图层 26__fd86e669.png"));
            EditorUtility.SetDirty(material);
            var root=PrefabUtility.LoadPrefabContents(Prefab);
            try{Apply(root);PrefabUtility.SaveAsPrefabAsset(root,Prefab);}finally{PrefabUtility.UnloadPrefabContents(root);}
            var scene=EditorSceneManager.OpenScene(Scene);foreach(var obj in scene.GetRootGameObjects())if(obj.GetComponent<RecoveredGameSession>())Apply(obj);
            EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();Debug.Log("REWARD_MOTION_AND_RATIO_AUTHORED");
        }
        static RectTransform Frame(GameObject scope,string name)
        {
            foreach(var image in scope.GetComponentsInChildren<Image>(true))if(image.name==name)return image.rectTransform;
            throw new InvalidOperationException("Missing panel "+name);
        }
        static Image Bound(GameObject scope,string path)
        {
            foreach(var art in scope.GetComponentInParent<RecoveredGameSession>().GetComponentsInChildren<RecoveredMenuArt>(true))if(art.images!=null)foreach(var b in art.images)if(b.resourcePath==path&&b.image.transform.IsChildOf(scope.transform))return b.image;
            throw new InvalidOperationException("Missing art "+path);
        }
        static void Label(Text text,RectTransform frame,float x,float y,float width,float height)
        {
            var t=text.rectTransform;t.SetParent(frame,false);t.anchorMin=t.anchorMax=t.pivot=Vector2.one*.5f;t.localScale=Vector3.one;
            t.anchoredPosition=new Vector2((x-627)*frame.rect.width/1254,(627-y)*frame.rect.height/1254);t.sizeDelta=new Vector2(width*frame.rect.width/1254,height*frame.rect.height/1254);
        }
        static RectTransform Light(Image panel,string flat,Vector2 center,Vector2 size,Vector4 body)
        {
            var angle=panel.transform.Find("SourceGlowAngle") as RectTransform;
            if(!angle){angle=(RectTransform)new GameObject("SourceGlowAngle",typeof(RectTransform)).transform;angle.gameObject.layer=panel.gameObject.layer;angle.SetParent(panel.transform,false);}
            var effect=panel.GetComponent<RecoveredRewardLight>();if(!effect)effect=panel.gameObject.AddComponent<RecoveredRewardLight>();
            effect.sourceGlow=angle;effect.flatResource="RewardMotion/Flat/"+flat;effect.lightCenter=center;effect.lightSize=size;effect.bodyRect=body;effect.strength=.9f;
            return angle;
        }
        static Image Currency(RecoveredGameSession s,GameObject group)
        {
            foreach(var binding in s.currencyIcons)if(binding.type==3&&binding.image.transform.IsChildOf(group.transform))return binding.image;
            throw new InvalidOperationException("Missing native localized reward money: "+group.name);
        }
        static void PlaceCash(Image cash,RectTransform frame,Vector2 center,Vector2 size)
        {
            var r=cash.rectTransform;r.SetParent(frame,false);r.anchorMin=r.anchorMax=r.pivot=Vector2.one*.5f;r.localScale=Vector3.one;r.localRotation=Quaternion.identity;
            r.anchoredPosition=Vector2.Scale(center-Vector2.one*.5f,frame.rect.size);r.sizeDelta=Vector2.Scale(size,frame.rect.size);
            cash.gameObject.SetActive(true);cash.raycastTarget=false;r.SetAsFirstSibling();
            var artwork=cash.GetComponent<RecoveredCurrencyArtwork>();
            if(!artwork)throw new InvalidOperationException("Missing authored country banknote pile");
            foreach(var image in artwork.smallNotes)image.enabled=true;
            foreach(var image in artwork.largeNotes)image.enabled=true;
        }
        public static void CalibrateLever(RecoveredWheelView wheel)
        {
            var lever=wheel.GetComponentInChildren<RecoveredWheelLever>(true);var machine=(RectTransform)lever.transform;
            // New body side axle; endpoints measured in the cropped approved shaft artwork.
            var joint=Vector2.Scale(new Vector2(.430f,.06f),machine.rect.size);
            var ball=Vector2.Scale(new Vector2(.468f,.26f),machine.rect.size);var delta=ball-joint;
            lever.shaft.pivot=new Vector2(.22f,.075f);lever.shaft.sizeDelta=new Vector2(delta.x/.525f,delta.y/.84f);
            lever.shaft.localRotation=Quaternion.identity;lever.shaft.localScale=Vector3.one;lever.shaft.anchoredPosition=joint;
            lever.knob.anchoredPosition=ball;lever.knob.localScale=Vector3.one;
            lever.shaftRest=joint;lever.knobRest=ball;
            lever.sourceRestY=586.68f;lever.sourcePressedY=502.22f;
            lever.travel=delta.y*(84.46f/(586.68f-415.27f));lever.shaftCompressedScale=1-lever.travel/delta.y;
            lever.shaft.GetComponent<Image>().preserveAspect=false;lever.knob.GetComponent<Image>().preserveAspect=true;
        }
        static void Apply(GameObject root)
        {
            var s=root.GetComponent<RecoveredGameSession>();var v=s.rewardView;
            var hint=s.bubble.GetComponent<RecoveredHintFloat>();if(!hint){var rest=((RectTransform)s.bubble.transform).anchoredPosition;hint=s.bubble.AddComponent<RecoveredHintFloat>();hint.restPosition=rest;}
            hint.amplitude=10;hint.riseTime=1;hint.fallTime=2;hint.returnTime=1;
            var normal=Frame(v.normalGroup,"ReferenceReward");normal.sizeDelta=new Vector2(719,719);normal.localScale=Vector3.one;normal.GetComponent<Image>().preserveAspect=true;
            Label(v.normalTitle,normal,627,174,600,230);Label(v.normalAmount,normal,627,964,730,300);
            var doubled=Frame(v.doubleRewardGroup,"DoubleRewardPanel");doubled.sizeDelta=new Vector2(719,719);doubled.localScale=Vector3.one;doubled.GetComponent<Image>().preserveAspect=true;
            Label(v.doubleRewardTitle,doubled,627,392,835,230);Label(v.doubleRewardAmount,doubled,627,1017,730,300);
            foreach(var coin in v.doubleRewardGroup.GetComponentsInChildren<Image>(true))
            {
                if(!coin.name.StartsWith("Coin",StringComparison.Ordinal))continue;
                float x=627,y=152,d=349;
                if(coin.name=="Coin500"){d=210;y=253;x=coin.rectTransform.anchoredPosition.x<0?229:1027;}
                if(coin.name=="Coin1000"){d=265;y=181;x=coin.rectTransform.anchoredPosition.x<0?386:863;}
                coin.rectTransform.anchoredPosition=new Vector2((x-627)*719/1254,77.112f+(627-y)*719/1254);coin.rectTransform.sizeDelta=Vector2.one*d*719/1254;coin.preserveAspect=true;
            }
            var glows=new List<RectTransform>();
            // Preserve guide's existing independent glow and its original native animation.
            foreach(var old in v.glows)if(old&&old.IsChildOf(v.guideGroup.transform))glows.Add(old);
            glows.Add(Light(normal.GetComponent<Image>(),"RewardPanel",new Vector2(.5f,.49f),new Vector2(1.12f,1.12f),new Vector4(.078f,.11f,.925f,.72f)));
            glows.Add(Light(doubled.GetComponent<Image>(),"DoubleRewardPanel",new Vector2(.5f,.42f),new Vector2(1.05f,1.05f),new Vector4(.078f,.095f,.925f,.615f)));
            var revive=Bound(v.doubleGroup,"ApprovedScreens/RevivePanel");
            glows.Add(Light(revive,"RevivePanel",new Vector2(.5f,.44f),new Vector2(1.02f,1.10f),new Vector4(.08f,.09f,.92f,.655f)));
            var highest=Bound(v.highestGroup,"ApprovedScreens/PrizePanel");
            glows.Add(Light(highest,"PrizePanel",new Vector2(.5f,.52f),new Vector2(1.1f,1.1f),new Vector4(.082f,.09f,.918f,.70f)));
            v.glows=glows.ToArray();
            s.wheelRewardView.glow=Light(Bound(s.wheelRewardView.gameObject,"ApprovedScreens/PrizePanel"),"PrizePanel",new Vector2(.5f,.54f),new Vector2(1.1f,1.1f),new Vector4(.082f,.09f,.918f,.70f));
            var normalCash=Currency(s,v.normalGroup);PlaceCash(normalCash,normal,new Vector2(.49f,.49f),new Vector2(.46f,.32f));
            Image doubleCash=null;foreach(var binding in s.currencyIcons)if(binding.type==3&&binding.image.transform.IsChildOf(doubled))doubleCash=binding.image;
            if(!doubleCash)
            {
                doubleCash=UnityEngine.Object.Instantiate(normalCash,doubled);doubleCash.name="LocalizedCash";
                foreach(var source in doubleCash.GetComponentsInChildren<RecoveredNode>(true))UnityEngine.Object.DestroyImmediate(source);
                var bindings=new List<CurrencyIconBinding>(s.currencyIcons);bindings.Add(new CurrencyIconBinding{image=doubleCash,type=3});s.currencyIcons=bindings.ToArray();
            }
            PlaceCash(doubleCash,doubled,new Vector2(.5f,.42f),new Vector2(.43f,.30f));
            PlaceCash(Currency(s,v.doubleGroup),revive.rectTransform,new Vector2(.5f,.44f),new Vector2(.36f,.43f));
            CalibrateLever(s.wheelView);
            // RewardDialog.show / LuckyDrawRewardDialog.show: by(4, angle:-360), repeatForever.
            s.lifecycle.config.glowSpeed=-90;EditorUtility.SetDirty(s.lifecycle.config);s.wheelRewardView.glowDegreesPerSecond=-90;
        }
    }
}
