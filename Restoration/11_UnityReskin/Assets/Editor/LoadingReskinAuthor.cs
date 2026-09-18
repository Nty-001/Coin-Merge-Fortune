using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
namespace CoinMerge.Recovery.Editor
{
    // Asset authoring only. Runtime continues through RecoveredStartup / RecoveredLoadingView.
    public static class LoadingReskinAuthor
    {
        const string Art="Assets/Resources/LoadingReskin/";
        [MenuItem("Coin Merge/Reskin/Apply sky loading illustration")]
        public static void Run()
        {
            if(EditorApplication.isPlaying)throw new InvalidOperationException("Stop Play before authoring loading assets.");
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            foreach(var file in Directory.GetFiles(Art,"*.png"))
            {
                var importer=(TextureImporter)AssetImporter.GetAtPath(file.Replace('\\','/'));
                importer.textureType=TextureImporterType.Sprite;importer.spriteImportMode=SpriteImportMode.Single;
                importer.spritePixelsPerUnit=100;importer.spriteBorder=Vector4.zero;importer.mipmapEnabled=false;
                importer.alphaIsTransparency=true;importer.textureCompression=TextureImporterCompression.Uncompressed;
                importer.maxTextureSize=2048;importer.filterMode=FilterMode.Bilinear;importer.wrapMode=TextureWrapMode.Clamp;
                var settings=new TextureImporterSettings();importer.ReadTextureSettings(settings);settings.spriteMeshType=SpriteMeshType.FullRect;importer.SetTextureSettings(settings);importer.SaveAndReimport();
            }
            foreach(var name in new[]{"RewardedLoading","PackagedLoading"})
            {
                string path="Assets/Resources/Startup/"+name+".prefab";var root=PrefabUtility.LoadPrefabContents(path);
                try{Apply(root);PrefabUtility.SaveAsPrefabAsset(root,path);}finally{PrefabUtility.UnloadPrefabContents(root);}
            }
            AssetDatabase.SaveAssets();Debug.Log("LOADING_RESKIN_AUTHORED");
        }
        static RectTransform Rect(Transform parent,string name)
        {var node=new GameObject(name,typeof(RectTransform));node.transform.SetParent(parent,false);return (RectTransform)node.transform;}
        static Image Image(Transform parent,string name)
        {var rect=Rect(parent,name);var image=rect.gameObject.AddComponent<Image>();image.raycastTarget=false;image.color=Color.white;image.type=UnityEngine.UI.Image.Type.Simple;image.preserveAspect=true;return image;}
        static void Place(RectTransform rect,Transform parent,Vector2 anchor,Vector2 offset,Vector2 size)
        {rect.SetParent(parent,false);rect.anchorMin=rect.anchorMax=anchor;rect.pivot=Vector2.one*.5f;rect.localScale=Vector3.one;rect.localRotation=Quaternion.identity;rect.anchoredPosition=offset;rect.sizeDelta=size;}
        static void Stretch(RectTransform rect)
        {rect.anchorMin=Vector2.zero;rect.anchorMax=Vector2.one;rect.offsetMin=rect.offsetMax=Vector2.zero;rect.localScale=Vector3.one;}
        static void Label(Text text,int size)
        {
            text.enabled=true;text.gameObject.SetActive(true);text.font=AssetDatabase.LoadAssetAtPath<Font>("Assets/Art/PopupFonts/Nunito-ExtraBold.ttf");
            text.fontSize=size;text.fontStyle=FontStyle.Normal;text.color=Color.white;text.alignment=TextAnchor.MiddleCenter;text.raycastTarget=false;text.lineSpacing=1;
            text.resizeTextForBestFit=false;text.horizontalOverflow=HorizontalWrapMode.Overflow;text.verticalOverflow=VerticalWrapMode.Overflow;
            foreach(var effect in text.GetComponents<Shadow>())effect.enabled=false;
            var outline=text.GetComponent<RecoveredRoundOutline>()??text.gameObject.AddComponent<RecoveredRoundOutline>();outline.enabled=true;outline.effectDistance=Vector2.one*2;outline.effectColor=new Color(.02f,.23f,.61f,1);
            var fit=text.GetComponent<RecoveredTextFit>()??text.gameObject.AddComponent<RecoveredTextFit>();fit.maximumFontSize=size;fit.minimumFontSize=8;fit.singleLine=true;fit.padding=new Vector2(3,3);
        }
        static void Apply(GameObject root)
        {
            var view=root.GetComponent<RecoveredLoadingView>();var canvas=root.GetComponentInChildren<Canvas>(true);
            var scaler=canvas.GetComponent<CanvasScaler>();scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;scaler.referenceResolution=new Vector2(750,750f*1672/941);scaler.screenMatchMode=CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;scaler.matchWidthOrHeight=0;
            canvas.renderMode=RenderMode.ScreenSpaceOverlay;canvas.sortingOrder=1000;
            Text caption=null;foreach(var text in root.GetComponentsInChildren<Text>(true))if(text!=view.percentage&&text.text.IndexOf("loading",StringComparison.OrdinalIgnoreCase)>=0){caption=text;break;}
            if(!caption)throw new InvalidOperationException("Original loading caption missing.");
            // Move the original live widgets before removing the old static art hierarchy.
            var presentation=Rect(canvas.transform,"SkyLoadingPresentation");Stretch(presentation);
            view.fill.transform.SetParent(presentation,false);if(view.percentage)view.percentage.transform.SetParent(presentation,false);caption.transform.SetParent(presentation,false);
            var obsolete=new List<GameObject>();foreach(Transform child in canvas.transform)if(child!=presentation)obsolete.Add(child.gameObject);
            foreach(var child in obsolete)UnityEngine.Object.DestroyImmediate(child);
            var art=root.GetComponent<RecoveredMenuArt>()??root.AddComponent<RecoveredMenuArt>();var bindings=new List<MenuImageBinding>();
            void Bind(Image image,string name){image.sprite=null;bindings.Add(new MenuImageBinding{image=image,resourcePath="LoadingReskin/"+name});}
            // Mirror the original bottom cloud strip at its shared edge. Each strip keeps
            // its native aspect ratio; no stretched clouds or discontinuity on tall phones.
            const float posterHeight=750f*1672/941,cloudHeight=750f*300/941;
            for(int i=0;i<4;i++)
            {
                var clouds=Image(presentation,"CloudExtension"+i);
                Place(clouds.rectTransform,presentation,new Vector2(.5f,1),new Vector2(0,-posterHeight-cloudHeight*(i+.5f)),new Vector2(750,cloudHeight));
                if(i%2==0)clouds.rectTransform.localScale=new Vector3(1,-1,1);
                Bind(clouds,"Clouds");clouds.transform.SetAsFirstSibling();
            }
            var illustration=Image(presentation,"ReferenceIllustration");var r=illustration.rectTransform;r.anchorMin=new Vector2(0,1);r.anchorMax=Vector2.one;r.pivot=new Vector2(.5f,1);r.anchoredPosition=Vector2.zero;r.sizeDelta=Vector2.zero;
            var ratio=illustration.gameObject.AddComponent<AspectRatioFitter>();ratio.aspectMode=AspectRatioFitter.AspectMode.WidthControlsHeight;ratio.aspectRatio=941f/1672;Bind(illustration,"Illustration");illustration.transform.SetSiblingIndex(4);
            var track=Image(presentation,"ProgressTrack");var trackSprite=AssetDatabase.LoadAssetAtPath<Sprite>(Art+"Track.png");var fillSprite=AssetDatabase.LoadAssetAtPath<Sprite>(Art+"Fill.png");
            float scale=584f/trackSprite.rect.width;
            Place(track.rectTransform,presentation,new Vector2(.5f,.113f),Vector2.zero,trackSprite.rect.size*scale);Bind(track,"Track");
            view.fill.enabled=true;view.fill.gameObject.SetActive(true);Place(view.fill.rectTransform,track.transform,Vector2.one*.5f,Vector2.zero,fillSprite.rect.size*scale);
            Bind(view.fill,"Fill");view.fill.color=Color.white;view.fill.preserveAspect=true;view.fill.type=UnityEngine.UI.Image.Type.Filled;view.fill.fillMethod=UnityEngine.UI.Image.FillMethod.Horizontal;view.fill.fillOrigin=0;view.fill.fillAmount=0;view.fill.raycastTarget=false;
            if(!view.percentage)view.percentage=Rect(track.transform,"Percentage").gameObject.AddComponent<Text>();
            Place(view.percentage.rectTransform,track.transform,Vector2.one*.5f,Vector2.zero,new Vector2(500,48));Label(view.percentage,30);view.percentage.text="0%";view.percentage.transform.SetAsLastSibling();
            Place(caption.rectTransform,presentation,new Vector2(.5f,.113f),new Vector2(0,-track.rectTransform.sizeDelta.y*.5f-24),new Vector2(584,40));Label(caption,30);caption.transform.SetAsLastSibling();
            art.images=bindings.ToArray();
        }
    }
}
