using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
namespace CoinMerge.Recovery.Editor
{
    // Only the seven designs explicitly approved on 2026-09-23 are migrated.
    [InitializeOnLoad] public static class RemainingApprovedAuthor
    {
        const string Art="RemainingApproved/",Prefab="Assets/Prefabs/Runtime/RecoveredMain.prefab",Scene="Assets/Scenes/RecoveredMain.unity";
        const float K=750f/941f;
        static readonly Color Blue=new Color(0,.18f,.66f),Navy=new Color(.01f,.09f,.34f);
        static RemainingApprovedAuthor(){EditorApplication.update+=Poll;}
        static void Poll(){if(!File.Exists("Temp/RemainingApproved.request")||EditorApplication.isCompiling||EditorApplication.isUpdating)return;if(EditorApplication.isPlayingOrWillChangePlaymode){EditorApplication.ExitPlaymode();return;}File.Delete("Temp/RemainingApproved.request");try{Run();File.WriteAllText("Temp/RemainingApproved.applied","OK");}catch(Exception e){File.WriteAllText("Temp/RemainingApproved.applied",e.ToString());Debug.LogException(e);}}
        [MenuItem("Coin Merge/Reskin/Apply seven approved remaining designs")]
        public static void Run()
        {
            if(EditorApplication.isPlayingOrWillChangePlaymode)throw new InvalidOperationException("Edit mode required");
            var current=UnityEngine.SceneManagement.SceneManager.GetActiveScene();if(current.isDirty){Directory.CreateDirectory("Temp/RemainingApprovedBackup");EditorSceneManager.SaveScene(current,"Temp/RemainingApprovedBackup/UnsavedScene.unity",true);}
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);Import();
            var root=PrefabUtility.LoadPrefabContents(Prefab);try{Apply(root);PrefabUtility.SaveAsPrefabAsset(root,Prefab);}finally{PrefabUtility.UnloadPrefabContents(root);}
            var scene=EditorSceneManager.OpenScene(Scene);foreach(var obj in scene.GetRootGameObjects())if(obj.GetComponent<RecoveredGameSession>())Apply(obj);
            EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();Debug.Log("APPROVED_SEVEN_AUTHORED");
        }
        static void Import()
        {
            foreach(var file in Directory.GetFiles("Assets/Resources/RemainingApproved","*.png"))
            {
                var path=file.Replace('\\','/');var tex=new Texture2D(2,2,TextureFormat.RGBA32,false);tex.LoadImage(File.ReadAllBytes(path));var px=tex.GetPixels32();
                string name=Path.GetFileNameWithoutExtension(path);int x0=tex.width,y0=tex.height,x1=-1,y1=-1,start=0,end=tex.width;
                int cell=name=="Check"?0:name=="Spinner"?1:name=="Hourglass"?2:-1;if(cell>=0){start=tex.width*cell/3;end=tex.width*(cell+1)/3;}
                for(int y=0;y<tex.height;y++)for(int x=start;x<end;x++)if(px[y*tex.width+x].a>16){x0=Math.Min(x0,x);x1=Math.Max(x1,x);y0=Math.Min(y0,y);y1=Math.Max(y1,y);}
                var rect=new Rect(x0,y0,x1-x0+1,y1-y0+1);UnityEngine.Object.DestroyImmediate(tex);
                var t=(TextureImporter)AssetImporter.GetAtPath(path);t.textureType=TextureImporterType.Sprite;t.spriteImportMode=SpriteImportMode.Multiple;
                Vector4 border=Vector4.zero;
                if(name=="StatusCard"||name=="Field")border=new Vector4(rect.height*.3f,rect.height*.3f,rect.height*.3f,rect.height*.3f);
                t.spritesheet=new[]{new SpriteMetaData{name=name,rect=rect,pivot=Vector2.one*.5f,alignment=(int)SpriteAlignment.Center,border=border}};
                t.spritePixelsPerUnit=100;t.mipmapEnabled=false;t.alphaIsTransparency=true;t.npotScale=TextureImporterNPOTScale.None;t.textureCompression=TextureImporterCompression.Uncompressed;t.crunchedCompression=false;t.maxTextureSize=4096;t.filterMode=FilterMode.Bilinear;t.wrapMode=TextureWrapMode.Clamp;
                var settings=new TextureImporterSettings();t.ReadTextureSettings(settings);settings.spriteMeshType=SpriteMeshType.FullRect;t.SetTextureSettings(settings);
                var platform=t.GetDefaultPlatformTextureSettings();platform.maxTextureSize=4096;platform.format=TextureImporterFormat.RGBA32;platform.textureCompression=TextureImporterCompression.Uncompressed;t.SetPlatformTextureSettings(platform);foreach(var target in new[]{"Standalone","Android","iPhone"})t.ClearPlatformTextureSettings(target);t.SaveAndReimport();
            }
        }
        static Dictionary<int,RectTransform> Nodes(GameObject root){var map=new Dictionary<int,RectTransform>();var uuid=root.GetComponent<RecoveredNode>().sourceUuid;foreach(var n in root.GetComponentsInChildren<RecoveredNode>(true))if(n.sourceUuid==uuid)map[n.sourceObjectId]=(RectTransform)n.transform;return map;}
        static void At(RectTransform r,Transform p,float x,float y,float w,float h){r.SetParent(p,false);r.anchorMin=r.anchorMax=r.pivot=Vector2.one*.5f;r.localScale=Vector3.one;r.localRotation=Quaternion.identity;r.anchoredPosition=new Vector2(x,y);r.sizeDelta=new Vector2(w,h);}
        static void P(RectTransform r,RectTransform frame,float x,float y,float w,float h,float fw,float fh){At(r,frame,(x-fw*.5f)*K,(fh*.5f-y)*K,w*K,h*K);}
        static void Stretch(RectTransform r,float l=0,float b=0,float rt=0,float top=0){r.anchorMin=Vector2.zero;r.anchorMax=Vector2.one;r.offsetMin=new Vector2(l,b);r.offsetMax=new Vector2(-rt,-top);r.localScale=Vector3.one;}
        static Image NewImage(string name,Transform parent){var old=parent.Find(name);if(old)return old.GetComponent<Image>();var obj=new GameObject(name,typeof(RectTransform),typeof(CanvasRenderer),typeof(Image));obj.layer=parent.gameObject.layer;obj.transform.SetParent(parent,false);var i=obj.GetComponent<Image>();i.raycastTarget=false;return i;}
        sealed class Skin
        {
            readonly GameObject root;readonly RecoveredMenuArt loader;readonly List<MenuImageBinding> bindings;
            public Skin(GameObject root,GameObject owner){this.root=root;loader=owner.GetComponent<RecoveredMenuArt>();if(!loader)loader=owner.AddComponent<RecoveredMenuArt>();bindings=new List<MenuImageBinding>(loader.images??Array.Empty<MenuImageBinding>());}
            public void Set(Image image,string path,bool aspect=false)
            {
                foreach(var a in root.GetComponentsInChildren<RecoveredMenuArt>(true))if(a!=loader&&a.images!=null){var v=new List<MenuImageBinding>(a.images);v.RemoveAll(x=>x.image==image);a.images=v.ToArray();}
                var b=bindings.Find(x=>x.image==image);if(b==null){b=new MenuImageBinding{image=image};bindings.Add(b);}b.resourcePath=path;
                image.sprite=null;image.material=null;image.color=Color.white;image.type=Image.Type.Simple;image.preserveAspect=aspect;image.enabled=true;
            }
            public void Card(Image i,string name="StatusCard"){Set(i,Art+name);i.type=Image.Type.Sliced;i.pixelsPerUnitMultiplier=1.05f;}
            public void Save(){loader.images=bindings.ToArray();}
        }
        static void Style(Text t,int size,bool title=false)
        {
            foreach(var e in t.GetComponents<BaseMeshEffect>())e.enabled=false;var fit=t.GetComponent<RecoveredTextFit>();if(fit)fit.enabled=false;
            t.font=AssetDatabase.LoadAssetAtPath<Font>("Assets/Art/PopupFonts/Nunito-ExtraBold.ttf");t.fontStyle=FontStyle.Normal;t.fontSize=size;t.color=title?Color.white:Blue;t.alignment=TextAnchor.MiddleCenter;t.resizeTextForBestFit=true;t.resizeTextMinSize=Math.Max(14,size*2/3);t.resizeTextMaxSize=size;t.horizontalOverflow=HorizontalWrapMode.Wrap;t.verticalOverflow=VerticalWrapMode.Truncate;t.lineSpacing=1.05f;t.raycastTarget=false;
            if(title){var o=t.GetComponent<RecoveredRoundOutline>();if(!o)o=t.gameObject.AddComponent<RecoveredRoundOutline>();o.enabled=true;o.effectColor=Navy;o.effectDistance=new Vector2(2.5f,2.5f);}
        }
        static void Hide(RectTransform r){foreach(var g in r.GetComponents<Graphic>())g.enabled=false;}
        static void Button(Skin skin,Button b,RectTransform frame,float x,float y,float w,float h,float fw,float fh,string path="ApprovedScreens/GreenButton")
        {
            At((RectTransform)b.transform,frame,(x-fw*.5f)*K,(fh*.5f-y)*K,w*K,h*K);skin.Set((Image)b.targetGraphic,path);b.targetGraphic.raycastTarget=true;
            foreach(var t in b.GetComponentsInChildren<Text>(true)){Style(t,48,true);Stretch(t.rectTransform,26,5,26,5);}
        }
        static void Close(Skin skin,Button b,RectTransform frame,float fw,float fh){Button(skin,b,frame,fw-70,84,86,86,fw,fh,Art+"Close");}
        static void Fit(GameObject page,RectTransform frame,float w,float h,float y)
        {
            At(frame,frame.parent,0,y,w*K,h*K);var pop=page.GetComponent<RecoveredMenuPopup>();if(!pop)pop=page.GetComponentInChildren<RecoveredMenuPopup>(true);
            if(pop){pop.fittedVisualSize=new Vector2(w*K,h*K+Mathf.Abs(y)*2);pop.availableFrame=(RectTransform)page.transform;pop.fitMargin=14;}
        }
        static void Apply(GameObject root){var s=root.GetComponent<RecoveredGameSession>();Policy(root,s.menus);Email(root,s.menus);Verify(root,s.menus);Stage(root,s.menus);Limit(root,s.menus);Newbie(root,s);}
        static void Policy(GameObject root,RecoveredMainMenus m)
        {
            var page=m.pages[2];var n=Nodes(page);var skin=new Skin(root,page);var frame=n[4];const float w=845,h=1510;
            Fit(page,frame,w,h,18);skin.Set(frame.GetComponent<Image>(),Art+"PrivacyPanel");m.policyPlate=frame.GetComponent<Image>();m.privacyPlateResource=Art+"PrivacyPanel";m.termsPlateResource=Art+"TermsPanel";
            P(m.policyTitle.rectTransform,frame,w*.5f,102,645,110,w,h);Style(m.policyTitle,48,true);Close(skin,n[25].GetComponent<Button>(),frame,w,h);Hide(n[12]);
            foreach(var obj in new[]{m.privacyScroll,m.termsScroll})
            {
                var scroll=obj.GetComponent<ScrollRect>();P((RectTransform)obj.transform,frame,w*.5f,841,716,1250,w,h);Stretch(scroll.viewport);
                var c=scroll.content;c.anchorMin=new Vector2(0,1);c.anchorMax=Vector2.one;c.pivot=new Vector2(.5f,1);c.anchoredPosition=Vector2.zero;c.sizeDelta=Vector2.zero;
                foreach(var text in c.GetComponentsInChildren<Text>(true)){Style(text,26);text.resizeTextForBestFit=false;text.alignment=TextAnchor.UpperLeft;text.verticalOverflow=VerticalWrapMode.Overflow;text.lineSpacing=1.2f;text.raycastTarget=true;}
                var layout=c.GetComponent<VerticalLayoutGroup>();layout.padding=new RectOffset(6,18,8,20);layout.spacing=18;scroll.verticalNormalizedPosition=1;
                var track=NewImage("ApprovedScrollbar",obj.transform);At(track.rectTransform,obj.transform,363*K,0,12*K,1240*K);track.sprite=AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");track.type=Image.Type.Sliced;track.color=new Color(.92f,.99f,1,.9f);track.raycastTarget=true;
                var thumb=NewImage("Handle",track.transform);Stretch(thumb.rectTransform);thumb.sprite=track.sprite;thumb.type=Image.Type.Sliced;thumb.color=new Color(.38f,.78f,1);thumb.raycastTarget=true;
                var bar=track.GetComponent<Scrollbar>();if(!bar)bar=track.gameObject.AddComponent<Scrollbar>();bar.handleRect=thumb.rectTransform;bar.targetGraphic=thumb;bar.direction=Scrollbar.Direction.BottomToTop;scroll.verticalScrollbar=bar;scroll.verticalScrollbarVisibility=ScrollRect.ScrollbarVisibility.Permanent;
            }
            skin.Save();
        }
        static void Email(GameObject root,RecoveredMainMenus m)
        {
            var page=m.pages[5];var n=Nodes(page);var skin=new Skin(root,page);var f=n[2];const float w=870,h=1091;
            Fit(page,f,w,h,30);skin.Set(f.GetComponent<Image>(),Art+"EmailPanel");Hide(n[8]);Hide(n[14]);
            P(n[9],f,427,105,615,102,w,h);Style(n[9].GetComponent<Text>(),45,true);Close(skin,n[16].GetComponent<Button>(),f,w,h);
            P(n[6],f,238,288,344,154,w,h);skin.Set(n[6].GetComponent<Image>(),Art+"PayPalCard");
            foreach(int id in new[]{17,18})Hide(n[id]);
            var check=NewImage("ApprovedSelected",n[6]);At(check.rectTransform,n[6],150*K,60*K,66*K,66*K);skin.Set(check,Art+"Check",true);
            P(n[19],f,325,441,320,72,w,h);Style(n[19].GetComponent<Text>(),39);
            var form=m.forms[0];P((RectTransform)form.account.transform,f,434,551,745,125,w,h);skin.Card((Image)form.account.targetGraphic,"Field");Stretch(form.account.targetGraphic.rectTransform);form.account.targetGraphic.transform.SetAsFirstSibling();
            foreach(var tx in form.account.GetComponentsInChildren<Text>(true)){Style(tx,29);tx.alignment=TextAnchor.MiddleLeft;tx.horizontalOverflow=HorizontalWrapMode.Overflow;Stretch(tx.rectTransform,25,8,25,8);}
            P(n[26],f,435,724,595,140,w,h);Style(n[26].GetComponent<Text>(),37);
            Button(skin,n[7].GetComponent<Button>(),f,435,869,600,144,w,h,Art+"DisabledButton");form.enabledSprite="ApprovedScreens/GreenButton";form.disabledSprite=Art+"DisabledButton";form.enabledOutline=Navy;form.disabledOutline=Navy;
            skin.Save();
        }
        static void Verify(GameObject root,RecoveredMainMenus m)
        {
            var page=m.pages[8];var n=Nodes(page);var skin=new Skin(root,page);var f=n[2];const float w=845,h=1344;
            Fit(page,f,w,h,-26);skin.Set(f.GetComponent<Image>(),Art+"VerifyPanel");Hide(n[15]);Hide(n[11]);Hide(n[10]);Hide(n[18]);Hide(n[20]);
            P(n[12],f,422,102,625,110,w,h);Style(n[12].GetComponent<Text>(),45,true);
            P(m.verifyAmount.rectTransform,f,422,269,680,114,w,h);Style(m.verifyAmount,68);
            P(n[7],f,422,421,738,198,w,h);skin.Card(n[7].GetComponent<Image>());
            P(m.verifyCommission.rectTransform,n[7],369,63,635,64,738,198);P(m.verifyCredited.rectTransform,n[7],369,136,635,64,738,198);
            foreach(var t in new[]{m.verifyCommission,m.verifyCredited}){Style(t,35);t.alignment=TextAnchor.MiddleLeft;}
            int[] rows={8,9,4};int[] labels={30,35,40};
            for(int i=0;i<3;i++)
            {
                float rh=i==2?198:154,cy=i==0?619:i==1?791:984;var row=n[rows[i]];P(row,f,422,cy,738,rh,w,h);
                var card=row.Find("SkyStatusCard").GetComponent<Image>();At(card.rectTransform,row,0,0,738*K,rh*K);skin.Card(card);card.transform.SetAsFirstSibling();
                P(n[labels[i]],row,433,i==2?59:77,530,i==2?64:94,738,rh);Style(n[labels[i]].GetComponent<Text>(),35);n[labels[i]].GetComponent<Text>().alignment=TextAnchor.MiddleLeft;
                var player=row.GetComponentInChildren<NativeSkeletonPlayer>(true);var badge=player.GetComponent<RecoveredVerificationBadge>();var holder=(RectTransform)player.transform.parent;
                At(holder,row,-283*K,0,0,0);At((RectTransform)player.transform,holder,0,0,100,100);
                At(badge.waiting.rectTransform,player.transform,0,0,88*K,(i==2?105:88)*K);At(badge.complete.rectTransform,player.transform,0,0,88*K,88*K);
                skin.Set(badge.waiting,Art+(i==2?"Hourglass":"Spinner"),true);skin.Set(badge.complete,Art+"Check",true);badge.smoothMotion=true;if(i==2)badge.rotationSpeed=0;
                if(i==2){P(n[42],row,405,138,478,92,738,rh);Style(n[42].GetComponent<Text>(),29);n[42].GetComponent<Text>().alignment=TextAnchor.MiddleLeft;}
            }
            Button(skin,n[13].GetComponent<Button>(),f,422,1194,618,154,w,h);skin.Save();
        }
        static void Stage(GameObject root,RecoveredMainMenus m)
        {
            var page=m.pages[9];var n=Nodes(page);var skin=new Skin(root,page);var f=n[2];const float w=822,h=932;
            Fit(page,f,w,h,48);skin.Set(f.GetComponent<Image>(),Art+"StagePanel");Hide(n[5]);Hide(n[15]);n[11].gameObject.SetActive(false);
            P(n[6],f,403,102,618,108,w,h);Style(n[6].GetComponent<Text>(),46,true);Close(skin,n[7].GetComponent<Button>(),f,w,h);
            P(m.stageAmount.rectTransform,f,411,268,650,105,w,h);Style(m.stageAmount,68);
            P(n[14],f,411,506,721,342,w,h);skin.Set(n[14].GetComponent<Image>(),Art+"ConditionCard");n[14].SetAsFirstSibling();
            P(m.stageHint.rectTransform,f,411,433,665,124,w,h);Style(m.stageHint,34);
            P(n[3],f,411,552,631,80,w,h);skin.Set(n[3].GetComponent<Image>(),Art+"ProgressTrack");
            skin.Set(m.stageFill,"CashReskin/ProgressFill");At(m.stageFill.rectTransform,n[3],0,0,598*K,48*K);m.stageFill.type=Image.Type.Filled;m.stageFill.fillMethod=Image.FillMethod.Horizontal;m.stageFill.fillOrigin=0;
            At(m.stagePercent.rectTransform,n[3],0,0,510*K,55*K);Style(m.stagePercent,30,true);m.stagePercent.transform.SetAsLastSibling();
            Button(skin,n[8].GetComponent<Button>(),f,411,794,596,162,w,h);skin.Save();
        }
        static void Limit(GameObject root,RecoveredMainMenus m)
        {
            var page=m.pages[10];var n=Nodes(page);var skin=new Skin(root,page);var f=n[2];const float w=825,h=1151;
            Fit(page,f,w,h,29);skin.Set(f.GetComponent<Image>(),Art+"LimitPanel");P(n[4],f,403,102,623,106,w,h);Style(n[4].GetComponent<Text>(),44,true);Close(skin,n[11].GetComponent<Button>(),f,w,h);
            P(n[6],f,413,459,524,374,w,h);skin.Set(n[6].GetComponent<Image>(),Art+"People",true);
            P(n[12],f,413,782,683,152,w,h);Style(n[12].GetComponent<Text>(),39);
            Button(skin,n[3].GetComponent<Button>(),f,413,1013,588,150,w,h);skin.Save();
        }
        static void Newbie(GameObject root,RecoveredGameSession s)
        {
            var v=s.rewardView;var n=Nodes(v.gameObject);var skin=new Skin(root,v.gameObject);var g=(RectTransform)v.guideGroup.transform;var f=n[48];const float w=849,h=892;
            At(f,g,0,0,w*K,h*K);skin.Set(f.GetComponent<Image>(),Art+"NewbiePanel");f.SetAsFirstSibling();
            P(v.guideTitle.rectTransform,f,424,99,624,104,w,h);Style(v.guideTitle,49,true);
            P(n[28],f,424,427,394,277,w,h);P(v.guideAmount.rectTransform,f,424,628,680,105,w,h);Style(v.guideAmount,68);
            P(n[25],f,424,421,535,535,w,h);n[25].SetAsFirstSibling();
            Button(skin,v.guideClose,f,424,776,597,155,w,h);At(n[30],v.guideClose.transform,184*K,-60*K,169*K,174*K);n[30].SetAsLastSibling();skin.Save();
        }
    }
}
