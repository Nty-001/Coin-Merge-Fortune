using System;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;
namespace CoinMerge.Recovery.Editor
{
    [InitializeOnLoad] public static class BVisualFixAuthor
    {
        const string Output="Design/BVisualFix20260917";
        static readonly Color Blue=new Color(0,.25f,.72f);
        static BVisualFixAuthor(){EditorApplication.update+=Poll;}
        static void Poll()
        {
            const string request="Temp/BVisualFixAuthor.request";
            if(!File.Exists(request)||EditorApplication.isCompiling||EditorApplication.isUpdating)return;
            if(EditorApplication.isPlaying){EditorApplication.ExitPlaymode();return;}
            if(EditorApplication.isPlayingOrWillChangePlaymode)return;File.Delete(request);
            try{Run();}catch(Exception e){File.WriteAllText(Output+"/author_error.txt",e.ToString());Debug.LogException(e);}
        }
        public static void Run()
        {
            Directory.CreateDirectory(Output);AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            foreach(var path in Directory.GetFiles("Assets/Resources/BVisualFix","*.png"))
            {
                var i=(TextureImporter)AssetImporter.GetAtPath(path.Replace('\\','/'));i.textureType=TextureImporterType.Sprite;i.spriteImportMode=SpriteImportMode.Single;i.spritePixelsPerUnit=100;
                i.alphaIsTransparency=true;i.mipmapEnabled=false;i.textureCompression=TextureImporterCompression.Uncompressed;i.npotScale=TextureImporterNPOTScale.None;i.filterMode=FilterMode.Bilinear;i.wrapMode=TextureWrapMode.Clamp;i.maxTextureSize=512;i.SaveAndReimport();
            }
            const string prefab="Assets/Prefabs/Runtime/RecoveredMain.prefab";
            var root=PrefabUtility.LoadPrefabContents(prefab);try{Apply(root);PrefabUtility.SaveAsPrefabAsset(root,prefab);}finally{PrefabUtility.UnloadPrefabContents(root);}
            var scene=EditorSceneManager.OpenScene("Assets/Scenes/RecoveredMain.unity");foreach(var go in scene.GetRootGameObjects())if(go.GetComponent<RecoveredGameSession>())Apply(go);EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();EditorSceneManager.OpenScene("Assets/Scenes/RecoveredLoading.unity");File.WriteAllText(Output+"/author_complete.txt",DateTime.UtcNow.ToString("O"));Debug.Log("B_VISUAL_FIX_AUTHORED");
        }
        static void At(RectTransform r,Transform parent,float x,float y,float w,float h)
        {r.SetParent(parent,false);r.anchorMin=r.anchorMax=r.pivot=Vector2.one*.5f;r.localScale=Vector3.one;r.localRotation=Quaternion.identity;r.anchoredPosition=new Vector2(x,y);r.sizeDelta=new Vector2(w,h);}
        static void Art(GameObject root,Image image,string resource,bool sliced=false)
        {
            foreach(var loader in root.GetComponentsInChildren<RecoveredMenuArt>(true))
            {var kept=new List<MenuImageBinding>(loader.images??Array.Empty<MenuImageBinding>());kept.RemoveAll(x=>x.image==image);loader.images=kept.ToArray();}
            var local=image.GetComponent<RecoveredMenuArt>();if(!local)local=image.gameObject.AddComponent<RecoveredMenuArt>();local.images=new[]{new MenuImageBinding{image=image,resourcePath=resource}};
            image.sprite=null;image.color=Color.white;image.type=sliced?Image.Type.Sliced:Image.Type.Simple;image.preserveAspect=!sliced;image.pixelsPerUnitMultiplier=4;image.enabled=true;
        }
        static void Fit(Text t,int max,bool single=false,int lines=0)
        {
            var f=t.GetComponent<RecoveredTextFit>();if(!f)f=t.gameObject.AddComponent<RecoveredTextFit>();
            f.boundedRasterization=true;f.rasterFontSize=max>56?64:40;f.maximumFontSize=max;f.minimumFontSize=8;f.singleLine=single;f.maximumLines=lines;f.padding=new Vector2(6,6);
            // Fit generates the native glyph mesh; authored outline and gradient effects run afterwards.
            while(ComponentUtility.MoveComponentUp(f)){}
            t.fontSize=f.rasterFontSize;t.resizeTextForBestFit=false;t.horizontalOverflow=single?HorizontalWrapMode.Overflow:HorizontalWrapMode.Wrap;t.verticalOverflow=VerticalWrapMode.Overflow;
        }
        static void ButtonLabel(Text t)
        {
            t.color=Color.white;t.font=AssetDatabase.LoadAssetAtPath<Font>("Assets/Art/PopupFonts/Nunito-ExtraBold.ttf");t.fontStyle=FontStyle.Normal;
            foreach(var e in t.GetComponents<Shadow>())e.enabled=false;var g=t.GetComponent<RecoveredTextVerticalGradient>();if(g)g.enabled=false;
            var o=t.GetComponent<RecoveredRoundOutline>();if(!o)o=t.gameObject.AddComponent<RecoveredRoundOutline>();o.enabled=true;o.effectColor=Blue;o.effectDistance=Vector2.one*2.4f;
        }
        static void Apply(GameObject root)
        {
            var s=root.GetComponent<RecoveredGameSession>();var m=s.menus;
            foreach(var t in root.GetComponentsInChildren<Text>(true))
            {
                var old=t.GetComponent<RecoveredTextFit>();if(old){int max=old.maximumFontSize;bool single=old.singleLine;Fit(t,max,single,old.maximumLines);}
            }
            s.wheelView.countLabel.gameObject.SetActive(false);
            Fit(s.bubbleText,34,false,2);s.bubbleText.alignment=TextAnchor.MiddleLeft;s.bubbleText.lineSpacing=.94f;
            Fit(s.notice.label,28,false,2);s.notice.label.lineSpacing=.9f;s.notice.label.alignment=TextAnchor.MiddleCenter;s.notice.matchFallbackWeight=true;
            var nr=s.notice.label.rectTransform;nr.offsetMin=new Vector2(4,0);nr.offsetMax=new Vector2(-4,0);
            foreach(var b in root.GetComponentsInChildren<Button>(true))
            {
                var im=b.targetGraphic as Image;if(!im)continue;string resource="";
                foreach(var loader in root.GetComponentsInChildren<RecoveredMenuArt>(true))foreach(var binding in loader.images)if(binding.image==im)resource=binding.resourcePath;
                bool green=resource.EndsWith("/Button")||resource.EndsWith("/Withdraw")||resource.Contains("GreenButton")||resource.EndsWith("CardSelected")||resource.Contains("ConfirmEnabled");
                if(!green)continue;foreach(var t in b.GetComponentsInChildren<Text>(true))ButtonLabel(t);
            }
            foreach(var f in m.forms){f.enabledOutline=Blue;foreach(var t in f.confirm.GetComponentsInChildren<Text>(true))ButtonLabel(t);}
            Verification(root,m);Task(root,m);Policy(root,m);Rating(root,s.rating);
        }
        static void Verification(GameObject root,RecoveredMainMenus m)
        {
            var frame=m.pages[8].transform.Find("content/bg");var layout=frame.Find("Layout");
            var inner=layout.Find("bg").GetComponent<Image>();Art(root,inner,"UnifiedReskin/Body",true);
            At((RectTransform)layout,frame,0,-65,600,650);At(inner.rectTransform,layout,0,270,582,146);
            for(int i=0;i<3;i++)
            {
                var row=layout.Find("New"+(i+1));At((RectTransform)row,layout,0,135-i*155,570,i==2?185:125);
                var card=row.Find("SkyStatusCard");if(!card){card=new GameObject("SkyStatusCard",typeof(RectTransform),typeof(CanvasRenderer),typeof(Image)).transform;}
                At((RectTransform)card,row,0,0,570,i==2?185:125);var im=card.GetComponent<Image>();im.raycastTarget=false;Art(root,im,"UnifiedReskin/Body",true);card.SetAsFirstSibling();
                var title=row.Find("stageTips"+(i+1)).GetComponent<Text>();At(title.rectTransform,row,35,i==2?44:0,415,62);title.alignment=TextAnchor.MiddleLeft;Fit(title,32,false,2);
                var sp=row.Find("spNode");var rect=(RectTransform)sp;rect.anchoredPosition=new Vector2(-233,i==2?86:45);
                if(i==2){var detail=row.Find("stageTips4").GetComponent<Text>();At(detail.rectTransform,row,35,-34,415,76);detail.alignment=TextAnchor.MiddleLeft;Fit(detail,26,false,2);}
                Badge(root,row.GetComponentInChildren<NativeSkeletonPlayer>(true));
            }
            foreach(var t in new[]{m.verifyCommission,m.verifyCredited}){t.alignment=TextAnchor.MiddleLeft;Fit(t,30,false,1);}
            At(m.verifyCommission.rectTransform,inner.transform,0,32,500,55);At(m.verifyCredited.rectTransform,inner.transform,0,-32,500,55);
        }
        static void Task(GameObject root,RecoveredMainMenus m)
        {
            var frame=m.pages[9].transform.Find("content/bg");var card=frame.Find("bg").GetComponent<Image>();Art(root,card,"UnifiedReskin/Body",true);At(card.rectTransform,frame,0,-30,574,270);card.transform.SetAsFirstSibling();
            At(m.stageAmount.rectTransform,frame,0,159,530,95);Fit(m.stageAmount,56,true);
            At(m.stageHint.rectTransform,frame,0,25,512,102);Fit(m.stageHint,32,false,3);
            var track=frame.Find("ProgressBar").GetComponent<Image>();Art(root,track,"LoadingReskin/Track",true);At(track.rectTransform,frame,0,-65,510,50);
            Art(root,m.stageFill,"CashReskin/ProgressFill");At(m.stageFill.rectTransform,track.transform,0,0,490,32);m.stageFill.type=Image.Type.Filled;m.stageFill.fillMethod=Image.FillMethod.Horizontal;m.stageFill.fillOrigin=0;m.stageFill.preserveAspect=false;
            At(m.stagePercent.rectTransform,track.transform,0,0,440,42);Fit(m.stagePercent,27,true);m.stagePercent.color=Color.white;ButtonLabel(m.stagePercent);
            var oldCoin=frame.Find("coinspr");if(oldCoin)oldCoin.gameObject.SetActive(false);
        }
        static void Policy(GameObject root,RecoveredMainMenus m)
        {
            foreach(var item in new[]{m.privacyScroll,m.termsScroll})
            {
                bool terms=item==m.termsScroll;var scroll=item.GetComponent<ScrollRect>();
                var content=scroll.content;bool wasNative=content.name=="NativePolicyText";
                if(wasNative){while(content.childCount>0)UnityEngine.Object.DestroyImmediate(content.GetChild(0).gameObject);}
                else foreach(var g in content.GetComponentsInChildren<Graphic>(true))g.enabled=false;
                // Reuse the viewport and input ScrollRect. Paragraphs are actual serialized Text components.
                var native=wasNative?content:(RectTransform)new GameObject("NativePolicyText",typeof(RectTransform),typeof(VerticalLayoutGroup),typeof(ContentSizeFitter)).transform;
                native.SetParent(scroll.viewport,false);native.anchorMin=new Vector2(0,1);native.anchorMax=Vector2.one;native.pivot=new Vector2(.5f,1);native.anchoredPosition=Vector2.zero;native.sizeDelta=Vector2.zero;
                var group=native.GetComponent<VerticalLayoutGroup>();group.padding=new RectOffset(18,18,20,30);group.spacing=16;group.childControlHeight=true;group.childControlWidth=true;group.childForceExpandHeight=false;group.childForceExpandWidth=true;
                native.GetComponent<ContentSizeFitter>().verticalFit=ContentSizeFitter.FitMode.PreferredSize;
                var source=AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Resources/BVisualFix/"+(terms?"Terms":"Privacy")+".txt");int number=0;
                foreach(string paragraph in source.text.Replace("\r","").Split(new[]{"\n\n"},StringSplitOptions.RemoveEmptyEntries))
                {
                    var go=new GameObject("Section"+(++number).ToString("00"),typeof(RectTransform),typeof(CanvasRenderer),typeof(Text));go.transform.SetParent(native,false);var t=go.GetComponent<Text>();t.font=AssetDatabase.LoadAssetAtPath<Font>("Assets/Art/PopupFonts/Nunito-ExtraBold.ttf");t.text=paragraph;t.color=Blue;t.alignment=TextAnchor.UpperLeft;t.lineSpacing=1.15f;t.supportRichText=false;t.raycastTarget=true;t.fontSize=28;t.horizontalOverflow=HorizontalWrapMode.Wrap;t.verticalOverflow=VerticalWrapMode.Overflow;
                    Fit(t,28,false);var fit=t.GetComponent<RecoveredTextFit>();fit.minimumFontSize=28;fit.rasterFontSize=28;t.fontSize=28;fit.padding=Vector2.zero;
                }
                if(!wasNative)content.gameObject.SetActive(false);native.gameObject.SetActive(true);scroll.content=native;scroll.horizontal=false;scroll.vertical=true;scroll.movementType=ScrollRect.MovementType.Clamped;scroll.verticalNormalizedPosition=1;
                foreach(var loader in root.GetComponentsInChildren<RecoveredMenuArt>(true)){var kept=new List<MenuImageBinding>(loader.images);kept.RemoveAll(x=>x.image&&x.image.transform.IsChildOf(content));loader.images=kept.ToArray();}
                At((RectTransform)item.transform,item.transform.parent,0,-55,601,980);
            }
        }
        static void Rating(GameObject root,RecoveredRatingView v)
        {
            v.emptyStar=AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Resources/BVisualFix/StarEmpty.png");v.filledStar=AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Resources/BVisualFix/StarSelected.png");
            foreach(var star in v.stars){star.image.preserveAspect=true;star.image.type=Image.Type.Simple;star.image.sprite=v.emptyStar;}
            var bg=v.transform.Find("node/bg/startBgNode");if(bg)bg.gameObject.SetActive(false);
        }
        static void Badge(GameObject root,NativeSkeletonPlayer player)
        {
            // Keep the source Animation and callbacks active, but replace its flat drawing surface.
            player.graphic.enabled=false;var badge=player.GetComponent<RecoveredVerificationBadge>();if(!badge)badge=player.gameObject.AddComponent<RecoveredVerificationBadge>();badge.source=player;
            foreach(bool done in new[]{false,true})
            {
                string name=done?"SkyComplete":"SkyWaiting";var child=player.transform.Find(name);if(!child)child=new GameObject(name,typeof(RectTransform),typeof(CanvasRenderer),typeof(Image)).transform;
                At((RectTransform)child,player.transform,0,0,94,94);var im=child.GetComponent<Image>();im.raycastTarget=false;Art(root,im,done?"BVisualFix/StatusComplete":"BVisualFix/StatusWaiting");
                if(done)badge.complete=im;else badge.waiting=im;
            }
        }
    }
}
