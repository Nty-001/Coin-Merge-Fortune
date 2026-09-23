using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace CoinMerge.Recovery.Editor
{
    // Explicit art migration. The six views retain their original typed controllers and events.
    public static class ApprovedScreensAuthor
    {
        const string Art="ApprovedScreens/";
        static readonly Color Navy=new Color(.015f,.11f,.37f);
        const string Prefab="Assets/Prefabs/Runtime/RecoveredMain.prefab";
        const string Scene="Assets/Scenes/RecoveredMain.unity";

        [MenuItem("Coin Merge/Reskin/Apply six approved popup designs")]
        public static void Run()
        {
            if(EditorApplication.isPlayingOrWillChangePlaymode)throw new InvalidOperationException("Author in Edit Mode.");
            var current=UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            if(current.isDirty){Directory.CreateDirectory("Temp/ApprovedScreensBackup");EditorSceneManager.SaveScene(current,"Temp/ApprovedScreensBackup/UnsavedScene.unity",true);}
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            Import();
            var root=PrefabUtility.LoadPrefabContents(Prefab);
            try{Apply(root);PrefabUtility.SaveAsPrefabAsset(root,Prefab);}finally{PrefabUtility.UnloadPrefabContents(root);}
            var scene=EditorSceneManager.OpenScene(Scene);
            foreach(var obj in scene.GetRootGameObjects())if(obj.GetComponent<RecoveredGameSession>())Apply(obj);
            EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();Debug.Log("APPROVED_SIX_SCREENS_AUTHORED");
        }

        static void Import()
        {
            foreach(var file in Directory.GetFiles("Assets/Resources/ApprovedScreens","*.png"))
            {
                string path=file.Replace('\\','/');var texture=new Texture2D(2,2,TextureFormat.RGBA32,false);
                if(!texture.LoadImage(File.ReadAllBytes(path)))throw new InvalidOperationException("Invalid PNG "+path);
                var pixels=texture.GetPixels32();int x0=texture.width,y0=texture.height,x1=-1,y1=-1;
                for(int y=0;y<texture.height;y++)for(int x=0;x<texture.width;x++)if(pixels[y*texture.width+x].a>12)
                {x0=Math.Min(x0,x);x1=Math.Max(x1,x);y0=Math.Min(y0,y);y1=Math.Max(y1,y);}
                if(x1<x0)throw new InvalidOperationException("Empty sprite "+path);
                var rect=new Rect(Math.Max(0,x0-1),Math.Max(0,y0-1),Math.Min(texture.width-1,x1+1)-Math.Max(0,x0-1)+1,Math.Min(texture.height-1,y1+1)-Math.Max(0,y0-1)+1);
                UnityEngine.Object.DestroyImmediate(texture);
                var t=(TextureImporter)AssetImporter.GetAtPath(path);t.textureType=TextureImporterType.Sprite;t.spriteImportMode=SpriteImportMode.Multiple;
                // Crop transparent export margin through the native sprite rect, without resampling source pixels.
                t.spritesheet=new[]{new SpriteMetaData{name=Path.GetFileNameWithoutExtension(path),rect=rect,pivot=Vector2.one*.5f,alignment=(int)SpriteAlignment.Center}};
                t.spritePixelsPerUnit=100;t.mipmapEnabled=false;t.alphaIsTransparency=true;t.npotScale=TextureImporterNPOTScale.None;
                t.textureCompression=TextureImporterCompression.Uncompressed;t.crunchedCompression=false;t.maxTextureSize=2048;
                t.filterMode=FilterMode.Bilinear;t.wrapMode=TextureWrapMode.Clamp;
                var settings=new TextureImporterSettings();t.ReadTextureSettings(settings);settings.spriteMeshType=SpriteMeshType.FullRect;t.SetTextureSettings(settings);
                var platform=t.GetDefaultPlatformTextureSettings();platform.format=TextureImporterFormat.RGBA32;platform.textureCompression=TextureImporterCompression.Uncompressed;platform.maxTextureSize=2048;t.SetPlatformTextureSettings(platform);
                foreach(var target in new[]{"Standalone","Android","iPhone"})t.ClearPlatformTextureSettings(target);
                t.SaveAndReimport();
            }
        }

        static Dictionary<int,RectTransform> Nodes(GameObject root)
        {
            string uuid=root.GetComponent<RecoveredNode>().sourceUuid;var map=new Dictionary<int,RectTransform>();
            foreach(var n in root.GetComponentsInChildren<RecoveredNode>(true))if(n.sourceUuid==uuid)map[n.sourceObjectId]=(RectTransform)n.transform;
            return map;
        }
        static void At(RectTransform r,Transform parent,float x,float y,float w,float h)
        {
            r.SetParent(parent,false);r.anchorMin=r.anchorMax=r.pivot=Vector2.one*.5f;r.localScale=Vector3.one;r.localRotation=Quaternion.identity;
            r.anchoredPosition=new Vector2(x,y);r.sizeDelta=new Vector2(w,h);
        }
        static Image NewImage(string name,Transform parent)
        {
            var old=parent.Find(name);if(old)return old.GetComponent<Image>();
            var obj=new GameObject(name,typeof(RectTransform),typeof(CanvasRenderer),typeof(Image));obj.layer=parent.gameObject.layer;obj.transform.SetParent(parent,false);
            var image=obj.GetComponent<Image>();image.raycastTarget=false;return image;
        }
        sealed class Skin
        {
            readonly GameObject scope;readonly RecoveredMenuArt loader;readonly List<MenuImageBinding> bindings;
            public Skin(GameObject root,GameObject owner)
            {scope=root;loader=owner.GetComponent<RecoveredMenuArt>();if(!loader)loader=owner.AddComponent<RecoveredMenuArt>();bindings=new List<MenuImageBinding>(loader.images??Array.Empty<MenuImageBinding>());}
            public void Set(Image image,string path,bool aspect=false)
            {
                foreach(var art in scope.GetComponentsInChildren<RecoveredMenuArt>(true))if(art!=loader&&art.images!=null)
                {var other=new List<MenuImageBinding>(art.images);other.RemoveAll(b=>b.image==image);art.images=other.ToArray();}
                var binding=bindings.Find(b=>b.image==image);if(binding==null){binding=new MenuImageBinding{image=image};bindings.Add(binding);}binding.resourcePath=path;
                image.sprite=null;image.color=Color.white;image.type=Image.Type.Simple;image.preserveAspect=aspect;image.enabled=true;
            }
            public void Save(){loader.images=bindings.ToArray();}
        }
        static void Style(Text t,int size,bool title=false,bool amount=false)
        {
            foreach(var effect in t.GetComponents<BaseMeshEffect>())effect.enabled=false;
            t.font=AssetDatabase.LoadAssetAtPath<Font>("Assets/Art/PopupFonts/Nunito-Black.ttf");t.fontStyle=FontStyle.Normal;
            t.fontSize=size;t.color=title||amount?Color.white:new Color(.015f,.20f,.56f);t.alignment=TextAnchor.MiddleCenter;
            t.resizeTextForBestFit=true;t.resizeTextMinSize=Math.Max(12,size/2);t.resizeTextMaxSize=size;t.horizontalOverflow=HorizontalWrapMode.Wrap;t.verticalOverflow=VerticalWrapMode.Truncate;t.raycastTarget=false;
            if(title||amount)
            {
                var gradient=t.GetComponent<RecoveredTextVerticalGradient>();if(!gradient)gradient=t.gameObject.AddComponent<RecoveredTextVerticalGradient>();gradient.enabled=true;
                gradient.top=amount?new Color(1,1,.60f):Color.white;gradient.bottom=amount?new Color(1,.43f,.015f):new Color(.68f,.93f,1);
                var outline=t.GetComponent<RecoveredPopupLettering>();if(!outline)outline=t.gameObject.AddComponent<RecoveredPopupLettering>();outline.enabled=true;
                outline.outline=amount?new Color(.30f,.075f,.005f):Navy;outline.depthColor=amount?new Color(.20f,.035f,0):new Color(.01f,.045f,.15f);
                outline.radius=amount?4:3;outline.depth=3;outline.edge=.75f;
            }
        }
        static void ButtonSkin(Skin skin,Button button,int size)
        {
            if(!(button.targetGraphic is Image image)||image.gameObject!=button.gameObject)throw new InvalidOperationException("Button must own its visible Image: "+button.name);
            skin.Set(image,Art+"GreenButton");image.raycastTarget=true;
            foreach(var text in button.GetComponentsInChildren<Text>(true))Style(text,size,true);
        }
        static void Hide(RectTransform rect){foreach(var g in rect.GetComponents<Graphic>())g.enabled=false;}
        static void Apply(GameObject root)
        {
            var session=root.GetComponent<RecoveredGameSession>();Fail(root,session);Rating(root,session);Rewards(root,session);WheelReward(root,session);Wheel(root,session);
        }
        static void Fail(GameObject root,RecoveredGameSession s)
        {
            var v=s.failView;var n=Nodes(v.gameObject);var skin=new Skin(root,v.gameObject);
            Hide(n[19]);var frame=NewImage("ApprovedFailFrame",n[19].parent);
            // Keep the original panel body and all control positions. The decorative crown extends above its top.
            At(frame.rectTransform,n[19].parent,n[19].anchoredPosition.x,n[19].anchoredPosition.y+30,n[19].rect.width,n[19].rect.height+60);
            skin.Set(frame,Art+"FailPanel");frame.transform.SetAsFirstSibling();
            Style(n[12].GetComponent<Text>(),54,true);Style(n[22].GetComponent<Text>(),40);Style(v.scoreText,104,false,true);
            foreach(var id in new[]{26,28}){Style(n[id].GetComponent<Text>(),27);n[id].GetComponent<Text>().alignment=TextAnchor.MiddleLeft;}
            Style(v.bestText,30);Style(v.mergesText,30);ButtonSkin(skin,v.revive,48);
            skin.Set((Image)v.close.targetGraphic,"ReferencePopups/Close");v.close.targetGraphic.raycastTarget=true;skin.Save();
        }
        static void Rating(GameObject root,RecoveredGameSession s)
        {
            var v=s.rating;var n=Nodes(v.gameObject);var skin=new Skin(root,v.gameObject);skin.Set(n[3].GetComponent<Image>(),Art+"RatingPanel");
            Style(v.title,55,true);Style(v.tips,31);ButtonSkin(skin,v.confirm,48);skin.Set((Image)v.close.targetGraphic,"ReferencePopups/Close");v.close.targetGraphic.raycastTarget=true;
            // Five native star Buttons and both filled/empty states remain bound to the rating controller.
            skin.Save();
        }
        static void Rewards(GameObject root,RecoveredGameSession s)
        {
            var v=s.rewardView;var n=Nodes(v.gameObject);var skin=new Skin(root,v.gameObject);
            skin.Set(n[32].GetComponent<Image>(),Art+"PrizePanel");skin.Set(n[36].GetComponent<Image>(),"Gameplay/Coins/2000",true);Hide(n[13]);
            Style(v.highestTitle,50,true);Style(v.highestAmount,92,false,true);ButtonSkin(skin,v.highestClose,48);
            skin.Set(n[39].GetComponent<Image>(),Art+"RevivePanel");n[21].gameObject.SetActive(false);Hide(n[18]);
            Style(v.doubleTitle,54,true);Style(v.doubleAmount,88,false,true);
            v.doubleTitle.rectTransform.anchoredPosition=new Vector2(0,154);
            v.doubleAmount.rectTransform.anchoredPosition=new Vector2(0,-170);
            // Double, ordinary and guide reward groups are deliberately untouched.
            skin.Save();
        }
        static void WheelReward(GameObject root,RecoveredGameSession s)
        {
            var v=s.wheelRewardView;var n=Nodes(v.gameObject);var skin=new Skin(root,v.gameObject);
            Hide(n[2]);skin.Set(n[10].GetComponent<Image>(),Art+"PrizePanel");skin.Set(v.coin.GetComponent<Image>(),"Gameplay/Coins/2000",true);Hide(v.glow);
            Style(v.titleLabel,50,true);Style(v.amountLabel,100,false,true);Style(v.hintLabel,27);ButtonSkin(skin,v.claim,48);skin.Save();
        }
        static void Wheel(GameObject root,RecoveredGameSession s)
        {
            var v=s.wheelView;var n=Nodes(v.gameObject);var skin=new Skin(root,v.gameObject);Image machine=null;
            foreach(var image in v.GetComponentsInChildren<Image>(true))if(image.name=="SkyPrizeMachine"){machine=image;break;}
            if(!machine)throw new InvalidOperationException("Missing original prize machine art");
            skin.Set(machine,Art+"MachineBody");
            var lever=machine.GetComponent<RecoveredWheelLever>();if(!lever)throw new InvalidOperationException("Missing original source-driven lever");
            skin.Set(lever.knob.GetComponent<Image>(),Art+"LeverKnob",true);skin.Set(lever.shaft.GetComponent<Image>(),Art+"LeverShaft");
            RewardMotionAuthor.CalibrateLever(v);
            for(int i=0;i<v.slots.Length;i++)
            {
                var slot=v.slots[i];skin.Set(n[32+i*4].GetComponent<Image>(),Art+"PrizeTile");
                var selected=slot.selected.GetComponent<Image>();skin.Set(selected,Art+"PrizeTile");selected.color=new Color(.65f,1,.24f);
                skin.Set(slot.coin.GetComponent<Image>(),"Gameplay/Coins/2000",true);Style(slot.coinAmount,20);
            }
            var centre=n[2].Find("CentreChip");skin.Set(centre.GetComponent<Image>(),"Gameplay/Coins/2000",true);
            ButtonSkin(skin,v.draw,44);Style(v.nextScoreLabel,28,true);Style(v.countLabel,30,true);skin.Save();
        }
    }
}
