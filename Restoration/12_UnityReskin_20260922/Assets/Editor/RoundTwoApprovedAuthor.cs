using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
namespace CoinMerge.Recovery.Editor
{
    // Six approved second-round screens and the subsequently approved Rules A design.
    [InitializeOnLoad] public static class RoundTwoApprovedAuthor
    {
        const string Art="RoundTwoApproved/",Prefab="Assets/Prefabs/Runtime/RecoveredMain.prefab",Scene="Assets/Scenes/RecoveredMain.unity";
        const float K=750f/941f;
        static readonly Color Blue=new Color(0,.18f,.66f),Navy=new Color(.01f,.09f,.34f);
        static RoundTwoApprovedAuthor(){EditorApplication.update+=Poll;}
        static void Poll(){if(!File.Exists("Temp/RoundTwoApproved.request")||EditorApplication.isCompiling||EditorApplication.isUpdating)return;if(EditorApplication.isPlayingOrWillChangePlaymode){EditorApplication.ExitPlaymode();return;}File.Delete("Temp/RoundTwoApproved.request");try{Run();File.WriteAllText("Temp/RoundTwoApproved.applied","OK");}catch(Exception e){File.WriteAllText("Temp/RoundTwoApproved.applied",e.ToString());Debug.LogException(e);}}
        [MenuItem("Coin Merge/Reskin/Apply approved second-round designs and Rules A")]
        public static void Run()
        {
            if(EditorApplication.isPlayingOrWillChangePlaymode)throw new InvalidOperationException("Edit mode required");
            var current=UnityEngine.SceneManagement.SceneManager.GetActiveScene();if(current.isDirty){Directory.CreateDirectory("Temp/RoundTwoApprovedBackup");EditorSceneManager.SaveScene(current,"Temp/RoundTwoApprovedBackup/UnsavedScene.unity",true);}
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);Import();
            var root=PrefabUtility.LoadPrefabContents(Prefab);try{Apply(root);PrefabUtility.SaveAsPrefabAsset(root,Prefab);}finally{PrefabUtility.UnloadPrefabContents(root);}
            var scene=EditorSceneManager.OpenScene(Scene);foreach(var obj in scene.GetRootGameObjects())if(obj.GetComponent<RecoveredGameSession>())Apply(obj);
            EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();Debug.Log("ROUND_TWO_AND_RULES_AUTHORED");
        }
        static void Import()
        {
            foreach(var file in Directory.GetFiles("Assets/Resources/RoundTwoApproved","*.png"))
            {
                var path=file.Replace('\\','/');var tex=new Texture2D(2,2,TextureFormat.RGBA32,false);tex.LoadImage(File.ReadAllBytes(path));var px=tex.GetPixels32();
                string name=Path.GetFileNameWithoutExtension(path);int x0=tex.width,y0=tex.height,x1=-1,y1=-1,start=0,end=tex.width;
                int cell=name=="Check"?0:name=="Spinner"?1:name=="Hourglass"?2:-1;if(cell>=0){start=tex.width*cell/3;end=tex.width*(cell+1)/3;}
                for(int y=0;y<tex.height;y++)for(int x=start;x<end;x++)if(px[y*tex.width+x].a>16){x0=Math.Min(x0,x);x1=Math.Max(x1,x);y0=Math.Min(y0,y);y1=Math.Max(y1,y);}
                var rect=new Rect(x0,y0,x1-x0+1,y1-y0+1);UnityEngine.Object.DestroyImmediate(tex);
                var t=(TextureImporter)AssetImporter.GetAtPath(path);t.textureType=TextureImporterType.Sprite;t.spriteImportMode=SpriteImportMode.Multiple;
                Vector4 border=Vector4.zero;
                if(name=="BlueField")border=new Vector4(rect.height*.3f,rect.height*.3f,rect.height*.3f,rect.height*.3f);
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
        static void Close(Skin skin,Button b,RectTransform frame,float fw,float fh){Button(skin,b,frame,fw-70,84,86,86,fw,fh,"RemainingApproved/Close");}
        static void Fit(GameObject page,RectTransform frame,float w,float h,float y)
        {
            At(frame,frame.parent,0,y,w*K,h*K);var pop=page.GetComponent<RecoveredMenuPopup>();if(!pop)pop=page.GetComponentInChildren<RecoveredMenuPopup>(true);
            if(pop){pop.fittedVisualSize=new Vector2(w*K,h*K+Mathf.Abs(y)*2);pop.availableFrame=(RectTransform)page.transform;pop.fitMargin=14;}
        }
        static void Apply(GameObject root)
        {
            var session=root.GetComponent<RecoveredGameSession>();
            Form(root,session.menus,true);Form(root,session.menus,false);Guides(root,session);Rules(root,session);
        }
        static void Form(GameObject root,RecoveredMainMenus m,bool brazil)
        {
            var page=m.pages[brazil?6:7];var n=Nodes(page);var skin=new Skin(root,page);var f=n[2];
            float w=brazil?837:864,h=brazil?1500:1282;
            Fit(page,f,w,h,0);var plate=f.GetComponent<Image>();if(!plate)plate=f.gameObject.AddComponent<Image>();
            skin.Set(plate,Art+(brazil?"BrazilPanel":"PhonePanel"));plate.raycastTarget=false;
            foreach(int id in brazil?new[]{10,21,22,25,26}:new[]{7,8,20,21,30,31})Hide(n[id]);
            var title=n[brazil?11:9].GetComponent<Text>();P(title.rectTransform,f,w*.5f-10,99,w-190,115,w,h);Style(title,brazil?51:46,true);
            Close(skin,n[brazil?24:23].GetComponent<Button>(),f,w,h);
            var form=m.forms[brazil?1:2];
            for(int i=0;i<form.platformPaths.Length;i++)form.platformPaths[i]=form.names[i].EndsWith(":Pagbank")?Art+"PagBankCard":form.names[i].EndsWith(":DANA")?Art+"DanaCard":"RemainingApproved/PayPalCard";
            for(int i=0;i<form.platforms.Length;i++)
            {
                var platform=form.platforms[i];P(platform.rectTransform,f,brazil?224:432,brazil?284:335,brazil?345:770,brazil?154:205,w,h);skin.Set(platform,form.platformPaths[0],true);platform.raycastTarget=true;
                var selected=form.selected[i].GetComponent<Image>();At(selected.rectTransform,platform.transform,brazil?150*K:295*K,brazil?62*K:0,(brazil?70:94)*K,(brazil?70:94)*K);skin.Set(selected,brazil?Art+"GoldCheck":"RemainingApproved/Check",true);selected.raycastTarget=false;
            }
            int[] labels=brazil?new[]{27,34,41}:new[]{32,39};var inputs=brazil?new[]{form.account,form.fullName,form.taxId}:new[]{form.account,form.fullName};
            for(int i=0;i<inputs.Length;i++)
            {
                float labelY=brazil?437+i*248:505+i*220,inputY=brazil?550+i*248:600+i*220;
                var label=n[labels[i]].GetComponent<Text>();P(label.rectTransform,f,brazil?w*.5f:400,labelY,brazil?650:570,85,w,h);Style(label,brazil?46:43);label.alignment=brazil?TextAnchor.MiddleCenter:TextAnchor.MiddleLeft;
                var input=inputs[i];P((RectTransform)input.transform,f,w*.5f,inputY,brazil?731:750,brazil?130:112,w,h);skin.Card((Image)input.targetGraphic,"BlueField");Stretch(input.targetGraphic.rectTransform);input.targetGraphic.transform.SetAsFirstSibling();
                foreach(var text in input.GetComponentsInChildren<Text>(true)){Style(text,28);text.alignment=TextAnchor.MiddleLeft;text.horizontalOverflow=HorizontalWrapMode.Overflow;Stretch(text.rectTransform,24,8,24,8);}
            }
            var hint=n[brazil?48:46].GetComponent<Text>();P(hint.rectTransform,f,w*.5f,brazil?1217:969,w-170,136,w,h);Style(hint,39);
            Button(skin,n[brazil?8:18].GetComponent<Button>(),f,w*.5f,brazil?1366:1125,brazil?599:642,154,w,h,"RemainingApproved/DisabledButton");
            form.enabledSprite="ApprovedScreens/GreenButton";form.disabledSprite="RemainingApproved/DisabledButton";form.enabledOutline=Navy;form.disabledOutline=Navy;skin.Save();
        }
        static Image Decoration(Skin skin,string name,RectTransform parent,float x,float y,float w,float h,string resource)
        {var image=NewImage(name,parent);At(image.rectTransform,parent,x*K,y*K,w*K,h*K);skin.Set(image,resource,true);return image;}
        static void Hand(RectTransform hand,Transform parent,float x,float y)
        {At(hand,parent,x*K,y*K,169*K,174*K);hand.GetComponent<Image>().preserveAspect=true;hand.SetAsLastSibling();}
        static Text NewLabel(string name,RectTransform parent,string value)
        {var child=parent.Find(name);if(child)return child.GetComponent<Text>();var obj=new GameObject(name,typeof(RectTransform),typeof(CanvasRenderer),typeof(Text));obj.layer=parent.gameObject.layer;obj.transform.SetParent(parent,false);var t=obj.GetComponent<Text>();t.text=value;Style(t,48);return t;}
        static void Guides(GameObject root,RecoveredGameSession s)
        {
            var v=s.guideView;var n=Nodes(v.gameObject);var skin=new Skin(root,v.gameObject);
            // Only the four approved guide steps are authored. Original Buttons and event references survive.
            At(n[8],v.stepZero.transform,0,-3,627*K,205*K);skin.Set(n[8].GetComponent<Image>(),Art+"DragPanel");
            At(n[25],n[8],-223*K,0,105*K,114*K);skin.Set(n[25].GetComponent<Image>(),"Gameplay/Coins/1",true);
            At(n[26],n[8],67*K,0,453*K,147*K);Style(n[26].GetComponent<Text>(),40,true);
            foreach(int id in new[]{23,24}){At(n[id],v.stepZero.transform,(id==23?143:-143)*K,305*K,100*K,90*K);skin.Set(n[id].GetComponent<Image>(),Art+"GuideArrow",true);}n[24].localScale=new Vector3(-1,1,1);
            Hand(n[15],v.stepZero.transform,91,236);

            var mf=n[4];At(mf,v.stepOne.transform,0,87*K,837*K,447*K);var image=mf.GetComponent<Image>();if(!image)image=mf.gameObject.AddComponent<Image>();skin.Set(image,Art+"MergePanel");image.raycastTarget=false;
            At(n[29],mf,0,132*K,745*K,130*K);Style(n[29].GetComponent<Text>(),40,true);
            At(n[28],mf,-228*K,-28*K,128*K,136*K);skin.Set(n[28].GetComponent<Image>(),"Gameplay/Coins/1",true);
            Decoration(skin,"ApprovedSecondCoin",mf,-23,-28,128,136,"Gameplay/Coins/1");Decoration(skin,"ApprovedResultCoin",mf,220,-28,136,142,"Gameplay/Coins/2");
            var plus=NewLabel("ApprovedPlus",mf,"+");At(plus.rectTransform,mf,-125*K,-28*K,55*K,65*K);Style(plus,58);plus.color=new Color(0,.43f,1);
            Decoration(skin,"ApprovedMergeArrow",mf,94,-28,80,55,Art+"GuideArrow");Hide(n[31]);
            Button(skin,v.oneButton,mf,690,378,236,106,837,447);At(n[30],v.oneButton.transform,0,0,190*K,80*K);Style(n[30].GetComponent<Text>(),44,true);Hand(n[16],v.oneButton.transform,76,-110);

            var cf=n[11];At(cf,n[5],0,-40,800*K,224*K);At(n[32],cf,0,0,800*K,224*K);skin.Set(n[32].GetComponent<Image>(),Art+"CashBubble");n[32].SetAsFirstSibling();
            At(n[34],cf,115*K,-14*K,530*K,148*K);Style(n[34].GetComponent<Text>(),34,true);n[34].SetAsLastSibling();
            At(n[33],cf,-277*K,-15*K,144*K,135*K);n[33].GetComponent<Image>().preserveAspect=true;n[33].SetAsLastSibling();
            // The displayed banknote follows the same currency binding used by the live balance HUD.
            foreach(var loader in root.GetComponentsInChildren<RecoveredMenuArt>(true))if(loader.images!=null){var list=new List<MenuImageBinding>(loader.images);list.RemoveAll(b=>b.image==n[33].GetComponent<Image>());loader.images=list.ToArray();}
            var notes=new List<CurrencyIconBinding>(s.currencyIcons);notes.RemoveAll(b=>b.image==n[33].GetComponent<Image>());notes.Add(new CurrencyIconBinding{image=n[33].GetComponent<Image>(),type=1});s.currencyIcons=notes.ToArray();
            v.cashPanel=cf;v.cashPanelGap=12;

            var coin=NewImage("ApprovedCoinPanel",n[13]);At(coin.rectTransform,n[13],323*K,-177*K,739*K,354*K);skin.Set(coin,Art+"CoinBubble");coin.raycastTarget=false;
            At(n[37],coin.transform,0,34*K,642*K,149*K);Style(n[37].GetComponent<Text>(),39,true);
            Button(skin,v.fourButton,coin.rectTransform,569,286,258,97,739,354);At(n[38],v.fourButton.transform,0,0,220*K,80*K);Style(n[38].GetComponent<Text>(),43,true);Hide(n[39]);Hand(n[21],v.fourButton.transform,95,-107);
            At(n[36],n[13],-68*K,0,166*K,166*K);skin.Set(n[36].GetComponent<Image>(),"Gameplay/Coins/2000",true);
            n[14].gameObject.SetActive(false);v.coinAnchor=n[36];v.coinContent=n[13];
            foreach(var b in s.menus.actions)if(b.action==4&&b.button.transform.IsChildOf(s.playfieldLayout.upper))v.coinTarget=(RectTransform)b.button.transform;
            skin.Save();
            foreach(var loader in root.GetComponentsInChildren<RecoveredMenuArt>(true))if(loader.images!=null){var list=new List<MenuImageBinding>(loader.images);list.RemoveAll(b=>b.image==n[33].GetComponent<Image>());loader.images=list.ToArray();}
        }
        static void Rules(GameObject root,RecoveredGameSession s)
        {
            var page=s.menus.pages[1];var n=Nodes(page);var skin=new Skin(root,page);var f=n[10];const float w=880,h=1045;
            Fit(page,f,w,h,-83*K);skin.Set(f.GetComponent<Image>(),Art+"RulesPanel");f.SetAsFirstSibling();
            Hide(n[6]);Hide(n[9]);foreach(var i in n[9].GetComponentsInChildren<Image>(true))i.enabled=false;
            foreach(Transform child in n[2])if(child.name.StartsWith("SequenceDot",StringComparison.Ordinal))child.gameObject.SetActive(false);
            P(n[5],f,440,72,657,111,w,h);Style(n[5].GetComponent<Text>(),60,true);
            P(n[12],f,440,237,768,152,w,h);Style(n[12].GetComponent<Text>(),31);n[12].GetComponent<Text>().alignment=TextAnchor.MiddleLeft;n[12].GetComponent<Text>().lineSpacing=1;
            P(n[17],f,440,389,790,79,w,h);Style(n[17].GetComponent<Text>(),40,true);n[17].GetComponent<Text>().alignment=TextAnchor.MiddleLeft;n[17].GetComponent<Text>().color=new Color(0,.27f,.92f);
            n[17].GetComponent<RecoveredRoundOutline>().effectColor=Color.white;n[17].GetComponent<RecoveredRoundOutline>().effectDistance=Vector2.one;
            var track=NewImage("ApprovedSequenceTrack",f);P(track.rectTransform,f,440,638,815,401,w,h);skin.Set(track,Art+"RulesTrack",true);
            float[] arrowX={190,333,475,616,765,765,613,476,337,194},arrowY={518,518,518,518,563,675,716,716,716,716};
            for(int i=0;i<arrowX.Length;i++)
            {
                var arrow=NewLabel("ApprovedSequenceArrow"+i,f,"›");P(arrow.rectTransform,f,arrowX[i],arrowY[i],40,58,w,h);Style(arrow,48);arrow.color=new Color(.8f,.98f,1);
                arrow.rectTransform.localRotation=Quaternion.Euler(0,0,i<4?0:i==4?-90:i==5?-135:180);
            }
            Button(skin,n[3].GetComponent<Button>(),f,440,921,536,149,w,h);At(n[8],n[3],0,0,440*K,120*K);Style(n[8].GetComponent<Text>(),65,true);
            n[8].GetComponent<RecoveredRoundOutline>().effectColor=new Color(0,.25f,.02f);
            Close(skin,n[16].GetComponent<Button>(),f,w,h);
            var previousFan=f.Find("ApprovedFan");if(previousFan)previousFan.SetParent(n[2],false);
            var fan=NewImage("ApprovedFan",n[2]);fan.enabled=false;At(fan.rectTransform,n[2],f.anchoredPosition.x,f.anchoredPosition.y+h*K*.5f,880*K,250*K);fan.transform.SetAsFirstSibling();
            int[] fanValues={200,500,1000,2000,1000,500,200};float[] fx={90,169,266,440,602,700,784},fy={8,-10,-34,-69,-34,-10,8},fd={123,145,177,238,177,145,123};
            int[] order={0,6,1,5,2,4,3};foreach(int i in order){var chip=Decoration(skin,"ApprovedFanCoin"+i,fan.rectTransform,fx[i]-440,-fy[i],fd[i],fd[i],"Gameplay/Coins/"+fanValues[i]);chip.transform.SetAsLastSibling();}
            var source=AssetDatabase.LoadAssetAtPath<NativeSkeletonData>("Assets/Resources/Skeletal/Data/HeChengSM_TX.asset");
            var data=AssetDatabase.LoadAssetAtPath<NativeSkeletonData>("Assets/Resources/RoundTwoApproved/RulesSequence.asset");
            if(!data){data=ScriptableObject.CreateInstance<NativeSkeletonData>();AssetDatabase.CreateAsset(data,"Assets/Resources/RoundTwoApproved/RulesSequence.asset");}EditorUtility.CopySerialized(source,data);
            const string clipPath="Assets/Resources/RoundTwoApproved/RulesSequence.anim";
            var clip=AssetDatabase.LoadAssetAtPath<AnimationClip>(clipPath);if(!clip){clip=new AnimationClip();AssetDatabase.CreateAsset(clip,clipPath);}
            EditorUtility.CopySerialized(AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/Resources/Skeletal/Clips/HeChengSM_TX/animation.anim"),clip);
            float[] px={118,262,404,545,687,772,682,545,407,265,123},py={518,518,518,518,518,626,716,716,716,716,716};
            int[] values={1,2,5,10,20,50,100,200,500,1000,2000};
            foreach(var binding in AnimationUtility.GetCurveBindings(clip))
            {
                if(binding.type!=typeof(NativeSkeletonBone)||(binding.propertyName!="x"&&binding.propertyName!="y"))continue;
                for(int i=0;i<11;i++)if(binding.path.EndsWith("_11_"+(10-i).ToString("00"),StringComparison.Ordinal))
                {var bone=Array.Find(source.bones,b=>b.name=="11_"+(10-i).ToString("00"));float delta=binding.propertyName=="x"?px[i]-440-bone.x:h*.5f-py[i]-bone.y;var curve=AnimationUtility.GetEditorCurve(clip,binding);var keys=curve.keys;for(int k=0;k<keys.Length;k++)keys[k].value+=delta;curve.keys=keys;AnimationUtility.SetEditorCurve(clip,binding,curve);}
            }
            data.animations[0].resourcePath="RoundTwoApproved/RulesSequence";EditorUtility.SetDirty(data);EditorUtility.SetDirty(clip);
            var player=s.menus.ruleAnimation;At((RectTransform)player.transform,f,0,0,w*K,h*K);player.transform.localScale=Vector3.one*K;player.dataPath="RoundTwoApproved/RulesSequence";player.graphic.enabled=false;
            var visuals=player.GetComponent<NativeRulesCoinImages>();if(!visuals)visuals=player.gameObject.AddComponent<NativeRulesCoinImages>();visuals.coins=new NativeRulesCoinBinding[11];
            for(int i=0;i<11;i++)
            {
                string name="11_"+(10-i).ToString("00");int bi=Array.FindIndex(source.bones,b=>b.name==name),si=Array.FindIndex(source.slots,b=>b.bone==bi);if(bi<0||si<0)throw new InvalidOperationException("Missing original rules bone "+name);
                data.bones[bi].x=player.bones[bi].x=px[i]-440;data.bones[bi].y=player.bones[bi].y=h*.5f-py[i];
                var image=NewImage("ApprovedCoin"+values[i],player.transform);At(image.rectTransform,player.transform,px[i]-440,h*.5f-py[i],i==10?132:112,i==10?132:112);skin.Set(image,"Gameplay/Coins/"+values[i],true);
                visuals.coins[i]=new NativeRulesCoinBinding{image=image,bone=player.bones[bi],slot=player.slots[si]};
            }
            EditorUtility.SetDirty(data);skin.Save();
        }
    }
}
