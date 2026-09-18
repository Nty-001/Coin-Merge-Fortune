using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
namespace CoinMerge.Recovery.Editor
{
    // Authoring only: produces real, editable native UI in the prefab and scene.
    public static class UnifiedPopupReskinAuthor
    {
        static readonly Color Blue=new Color(0,.26f,.73f),Stroke=new Color(0,.21f,.65f);
        static Font font,heavyFont;
        [MenuItem("Coin Merge/Reskin/Apply remaining sky popups and text fitting")]
        public static void Run()
        {
            if(EditorApplication.isPlaying)throw new InvalidOperationException("Stop Play before authoring.");
            var previous=UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            if(previous.isDirty){Directory.CreateDirectory("Temp/UnifiedReskinBackup");EditorSceneManager.SaveScene(previous,"Temp/UnifiedReskinBackup/BeforeAuthoring.unity",true);}
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            foreach(var file in Directory.GetFiles("Assets/Resources/UnifiedReskin","*.png"))
            {
                var i=(TextureImporter)AssetImporter.GetAtPath(file.Replace('\\','/'));
                i.textureType=TextureImporterType.Sprite;i.spriteImportMode=SpriteImportMode.Single;i.spritePixelsPerUnit=32;
                i.spriteBorder=file.EndsWith("Panel.png")?new Vector4(125,205,125,275):file.EndsWith("Button.png")?new Vector4(104,82,104,82):file.EndsWith("Body.png")?new Vector4(105,170,105,105):Vector4.zero;
                i.mipmapEnabled=false;i.alphaIsTransparency=true;i.textureCompression=TextureImporterCompression.Uncompressed;i.maxTextureSize=2048;i.filterMode=FilterMode.Bilinear;i.wrapMode=TextureWrapMode.Clamp;
                var setting=new TextureImporterSettings();i.ReadTextureSettings(setting);setting.spriteMeshType=SpriteMeshType.FullRect;i.SetTextureSettings(setting);i.SaveAndReimport();
            }
            foreach(var file in Directory.GetFiles("Assets/Resources/CoinReskin","*.png"))
            {
                var i=(TextureImporter)AssetImporter.GetAtPath(file.Replace('\\','/'));i.textureType=TextureImporterType.Sprite;i.spriteImportMode=SpriteImportMode.Single;i.spritePixelsPerUnit=32;
                i.spriteBorder=file.EndsWith("Amounts.png")?new Vector4(80,70,80,150):file.EndsWith("WithdrawDisabled.png")?new Vector4(90,80,90,80):Vector4.zero;
                i.mipmapEnabled=false;i.alphaIsTransparency=true;i.textureCompression=TextureImporterCompression.Uncompressed;i.maxTextureSize=2048;i.filterMode=FilterMode.Bilinear;i.wrapMode=TextureWrapMode.Clamp;
                var settings=new TextureImporterSettings();i.ReadTextureSettings(settings);settings.spriteMeshType=SpriteMeshType.FullRect;i.SetTextureSettings(settings);i.SaveAndReimport();
            }
            font=AssetDatabase.LoadAssetAtPath<Font>("Assets/Art/PopupFonts/Nunito-ExtraBold.ttf");
            foreach(var path in new[]{"Assets/Prefabs/Runtime/RecoveredMain.prefab","Assets/Prefabs/Runtime/RecoveredPackaged.prefab","Assets/Prefabs/Runtime/VersionGM.prefab"})
            {
                var root=PrefabUtility.LoadPrefabContents(path);try{if(root.GetComponent<RecoveredGameSession>())Apply(root);if(root.GetComponent<PackagedGameSession>())Packaged(root);FitAll(root);PrefabUtility.SaveAsPrefabAsset(root,path);}finally{PrefabUtility.UnloadPrefabContents(root);}
            }
            foreach(var path in new[]{"Assets/Scenes/RecoveredMain.unity","Assets/Scenes/RecoveredPackaged.unity","Assets/Scenes/RecoveredLoading.unity"})
            {
                var scene=EditorSceneManager.OpenScene(path);foreach(var root in scene.GetRootGameObjects()){if(root.GetComponent<RecoveredGameSession>())Apply(root);if(root.GetComponent<PackagedGameSession>())Packaged(root);FitAll(root);}EditorSceneManager.SaveScene(scene);
            }
            AssetDatabase.SaveAssets();Debug.Log("UNIFIED_RESKIN_AUTHORED");
        }
        static Dictionary<int,RectTransform> Nodes(GameObject root)
        {
            var found=root.GetComponent<RecoveredNode>();if(!found)throw new Exception("Missing source root "+root.name);
            var n=new Dictionary<int,RectTransform>();foreach(var x in root.GetComponentsInChildren<RecoveredNode>(true))if(x.sourceUuid==found.sourceUuid)n[x.sourceObjectId]=(RectTransform)x.transform;return n;
        }
        static void At(RectTransform r,Transform parent,float x,float y,float w,float h)
        {r.SetParent(parent,false);r.anchorMin=r.anchorMax=r.pivot=Vector2.one*.5f;r.localScale=Vector3.one;r.localRotation=Quaternion.identity;r.anchoredPosition=new Vector2(x,y);r.sizeDelta=new Vector2(w,h);}
        static Image NewImage(Transform parent,string name)
        {var old=parent.Find(name);if(old)return old.GetComponent<Image>();var obj=new GameObject(name,typeof(RectTransform),typeof(CanvasRenderer),typeof(Image));obj.transform.SetParent(parent,false);var image=obj.GetComponent<Image>();image.raycastTarget=false;return image;}
        static void Stretch(RectTransform r,float left,float bottom,float right,float top)
        {r.anchorMin=Vector2.zero;r.anchorMax=Vector2.one;r.pivot=Vector2.one*.5f;r.localScale=Vector3.one;r.anchoredPosition=Vector2.zero;r.offsetMin=new Vector2(left,bottom);r.offsetMax=new Vector2(-right,-top);}
        static void TextStyle(Text t,int size,bool title=false)
        {
            if(title&&!heavyFont)heavyFont=AssetDatabase.LoadAssetAtPath<Font>("Assets/Art/PopupFonts/Nunito-Black.ttf");
            t.font=title?heavyFont:font;t.fontStyle=FontStyle.Normal;t.fontSize=size;t.color=title?Color.white:Blue;t.lineSpacing=1;t.alignment=TextAnchor.MiddleCenter;
            t.resizeTextForBestFit=true;t.resizeTextMinSize=8;t.resizeTextMaxSize=size;t.horizontalOverflow=HorizontalWrapMode.Wrap;t.verticalOverflow=VerticalWrapMode.Truncate;
            var fit=t.GetComponent<RecoveredTextFit>();if(fit)fit.maximumFontSize=size;
            foreach(var effect in t.GetComponents<Shadow>())effect.enabled=false;
            var gradient=t.GetComponent<RecoveredTextVerticalGradient>();if(gradient)gradient.enabled=false;
            if(title){var outline=t.GetComponent<RecoveredRoundOutline>();if(!outline)outline=t.gameObject.AddComponent<RecoveredRoundOutline>();outline.enabled=true;outline.effectColor=Stroke;outline.effectDistance=Vector2.one*Mathf.Clamp(size*.065f,2,5);}
        }
        sealed class Skin
        {
            readonly GameObject root;readonly RecoveredMenuArt loader;readonly List<MenuImageBinding> list;
            public Skin(GameObject root){this.root=root;loader=root.GetComponent<RecoveredMenuArt>();if(!loader)loader=root.AddComponent<RecoveredMenuArt>();list=new List<MenuImageBinding>(loader.images??Array.Empty<MenuImageBinding>());}
            public void Art(Image image,string path,bool sliced=false)
            {
                if(!image)throw new Exception("Missing Image for "+path);
                foreach(var nested in root.GetComponentsInChildren<RecoveredMenuArt>(true))if(nested!=loader&&nested.images!=null){var b=new List<MenuImageBinding>(nested.images);b.RemoveAll(x=>x.image==image);nested.images=b.ToArray();}
                var entry=list.Find(x=>x.image==image);if(entry==null){entry=new MenuImageBinding{image=image};list.Add(entry);}entry.resourcePath=path;
                image.sprite=null;image.color=Color.white;image.type=sliced?Image.Type.Sliced:Image.Type.Simple;image.preserveAspect=!sliced;image.pixelsPerUnitMultiplier=path.EndsWith("Panel")?1.85f:1.4f;image.enabled=true;
            }
            public void Panel(Image image){Art(image,"UnifiedReskin/Panel",true);image.raycastTarget=false;if(image.transform.parent==root.transform)image.transform.SetAsLastSibling();else image.transform.SetAsFirstSibling();}
            public void Pill(Button button)
            {
                if(!button||!(button.targetGraphic is Image image))return;Art(image,"UnifiedReskin/Button",true);image.raycastTarget=true;
                foreach(var label in button.GetComponentsInChildren<Text>(true))if(label.transform.parent==button.transform){TextStyle(label,Mathf.Clamp(label.fontSize,28,58),true);Stretch(label.rectTransform,40,12,40,12);var o=label.GetComponent<RecoveredRoundOutline>();o.effectColor=new Color(0,.3f,.02f);}
            }
            public void Close(Button button,RectTransform frame)
            {if(!button)return;At(button.targetGraphic.rectTransform,frame,frame.rect.width*.5f-53,frame.rect.height*.5f-55,70,70);Art((Image)button.targetGraphic,"SettingsReskin/Close");button.targetGraphic.raycastTarget=true;button.transform.SetAsLastSibling();}
            public void Save(){loader.images=list.ToArray();}
        }
        static void HideGraphic(RectTransform r){foreach(var g in r.GetComponents<Graphic>())g.enabled=false;}
        static void HideSkeleton(RectTransform root){foreach(var g in root.GetComponentsInChildren<NativeSkeletonGraphic>(true))g.enabled=false;}
        static void Apply(GameObject root)
        {
            var s=root.GetComponent<RecoveredGameSession>();Rewards(s);Wheel(s);WheelReward(s);Fail(s);Rating(s);Guides(s);OtherMenus(s);HomeText(s);
        }
        static void Rewards(RecoveredGameSession s)
        {
            var v=s.rewardView;var n=Nodes(v.gameObject);var skin=new Skin(v.gameObject);
            // Keep live amounts, guide hand, click Buttons and confetti. Only decorative old frames are retired.
            foreach(int id in new[]{14,19,26})HideSkeleton(n[id]);HideGraphic(n[45]);
            GameObject[] groups={v.highestGroup,v.normalGroup,v.guideGroup,v.doubleGroup};Text[] titles={v.highestTitle,v.normalTitle,v.guideTitle,v.doubleTitle};Text[] values={v.highestAmount,v.normalAmount,v.guideAmount,v.doubleAmount};
            int[] panels={32,44,48,39},icons={36,24,28,21},glows={13,22,25,18};float[] heights={755,535,710,535};
            for(int i=0;i<groups.Length;i++)
            {
                var group=(RectTransform)groups[i].transform;group.anchoredPosition=Vector2.zero;float h=heights[i];var frame=n[panels[i]];
                At(frame,group,0,0,662,h);skin.Panel(frame.GetComponent<Image>());
                At(titles[i].rectTransform,group,0,h*.5f-78,540,112);TextStyle(titles[i],54,true);
                At(n[icons[i]],group,0,i==1||i==3?-10:22,i==0?255:330,i==0?255:230);
                if(i==0)skin.Art(n[icons[i]].GetComponent<Image>(),"RulesReskin/Chip2000");
                else n[icons[i]].GetComponent<Image>().preserveAspect=true;
                At(n[glows[i]],group,0,0,410,410);n[glows[i]].GetComponent<Image>().color=new Color(.4f,.87f,1,.45f);n[glows[i]].SetSiblingIndex(1);
                At(values[i].rectTransform,group,0,i==1||i==3?-h*.5f+80:-145,552,96);TextStyle(values[i],65);
                if(i==0||i==2){var button=i==0?v.highestClose:v.guideClose;At((RectTransform)button.transform,group,0,-h*.5f+91,470,122);skin.Pill(button);}
                titles[i].transform.SetAsLastSibling();values[i].transform.SetAsLastSibling();
            }
            skin.Save();
        }
        static void Wheel(RecoveredGameSession s)
        {
            var v=s.wheelView;var n=Nodes(v.gameObject);var skin=new Skin(v.gameObject);var content=n[3];
            HideSkeleton(n[28]); // Player remains active: its completion event still drives the original draw state machine.
            var machine=NewImage(content,"SkyPrizeMachine");At(machine.rectTransform,content,0,62,700,700*1360f/1156);skin.Art(machine,"UnifiedReskin/Machine");machine.transform.SetAsFirstSibling();
            // Eight peripheral live slots + the centre chip, all inside the generated machine recess.
            At(n[2],content,-2,130,345,300);
            int[] xs={-1,0,1,1,1,0,-1,-1},ys={1,1,1,0,-1,-1,-1,0};
            for(int i=0;i<8;i++)
            {
                var slot=v.slots[i];var holder=n[4+i];At(holder,n[2],xs[i]*111,ys[i]*100,102,92);
                var tile=n[32+i*4].GetComponent<Image>();At(tile.rectTransform,holder,0,0,102,92);skin.Art(tile,"UnifiedReskin/Body",true);tile.pixelsPerUnitMultiplier=6;
                var selected=slot.selected.GetComponent<Image>();At(selected.rectTransform,holder,0,0,102,92);skin.Art(selected,"UnifiedReskin/Body",true);selected.color=new Color(.5f,1,.1f);selected.pixelsPerUnitMultiplier=6;
                At((RectTransform)slot.coin.transform,holder,0,9,62,62);skin.Art(slot.coin.GetComponent<Image>(),"RulesReskin/Chip2000");
                At(slot.coinAmount.rectTransform,slot.coin.transform,0,-38,100,24);TextStyle(slot.coinAmount,20);
                At(slot.money.rectTransform,holder,0,0,80,60);slot.money.preserveAspect=true;
                tile.transform.SetAsFirstSibling();selected.transform.SetSiblingIndex(1);slot.coin.transform.SetAsLastSibling();slot.money.transform.SetAsLastSibling();
            }
            // The centre uses one accepted chip instead of the old coin-filled icon.
            foreach(int id in new[]{15,30,31})if(n.ContainsKey(id))HideGraphic(n[id]);
            var center=NewImage(n[2],"CentreChip");At(center.rectTransform,n[2],0,0,83,83);skin.Art(center,"RulesReskin/Chip2000");
            At((RectTransform)v.draw.transform,content,0,-443,470,122);skin.Pill(v.draw);TextStyle(v.drawLabel,44,true);Stretch(v.drawLabel.rectTransform,35,15,35,15);
            At(v.nextScoreLabel.rectTransform,content,0,-548,650,70);TextStyle(v.nextScoreLabel,28,true);
            At(v.countLabel.rectTransform,content,0,-284,220,46);TextStyle(v.countLabel,30,true);
            if(n.ContainsKey(24))HideGraphic(n[24]);skin.Save();
        }
        static void WheelReward(RecoveredGameSession s)
        {
            var v=s.wheelRewardView;var n=Nodes(v.gameObject);var skin=new Skin(v.gameObject);var content=n[2];At(content,v.transform,0,0,694,879);
            At(n[10],content,0,0,694,879);skin.Panel(n[10].GetComponent<Image>());
            At(v.titleLabel.rectTransform,content,0,352,560,108);TextStyle(v.titleLabel,53,true);
            At(v.glow,content,0,57,430,430);v.glow.GetComponent<Image>().color=new Color(.3f,.85f,1,.5f);
            At((RectTransform)v.coin.transform,content,0,74,260,260);skin.Art(v.coin.GetComponent<Image>(),"RulesReskin/Chip2000");At(v.money.rectTransform,content,0,74,330,240);v.money.preserveAspect=true;
            At(v.amountLabel.rectTransform,content,0,-115,560,100);TextStyle(v.amountLabel,65);
            At((RectTransform)v.claim.transform,content,0,-268,470,122);skin.Pill(v.claim);Stretch(v.claimLabel.rectTransform,40,13,40,13);TextStyle(v.claimLabel,48,true);
            At(v.hintLabel.rectTransform,content,0,-372,580,78);TextStyle(v.hintLabel,27);skin.Save();
        }
        static void Fail(RecoveredGameSession s)
        {
            var v=s.failView;var n=Nodes(v.gameObject);var skin=new Skin(v.gameObject);var group=n[2];
            At(n[19],group,0,-499,662,780);skin.Panel(n[19].GetComponent<Image>());HideGraphic(n[11]);HideGraphic(n[3]);
            At(n[12],group,0,-188,520,110);TextStyle(n[12].GetComponent<Text>(),54,true);
            At(n[22],group,0,-307,540,74);TextStyle(n[22].GetComponent<Text>(),40);
            At(v.scoreText.rectTransform,group,0,-407,520,120);TextStyle(v.scoreText,80);
            At(n[26],group,-110,-518,290,48);TextStyle(n[26].GetComponent<Text>(),27);n[26].GetComponent<Text>().alignment=TextAnchor.MiddleLeft;
            At(v.bestText.rectTransform,group,175,-518,190,48);TextStyle(v.bestText,30);
            At(n[28],group,-110,-574,290,48);TextStyle(n[28].GetComponent<Text>(),27);n[28].GetComponent<Text>().alignment=TextAnchor.MiddleLeft;
            At(v.mergesText.rectTransform,group,175,-574,190,48);TextStyle(v.mergesText,30);HideGraphic(n[32]);HideGraphic(n[20]);
            At((RectTransform)v.revive.transform,group,0,-760,470,122);skin.Pill(v.revive);
            At(v.close.targetGraphic.rectTransform,group,273,-168,65,65);skin.Art((Image)v.close.targetGraphic,"SettingsReskin/Close");
            skin.Save();
        }
        static void Rating(RecoveredGameSession s)
        {
            var v=s.rating;var n=Nodes(v.gameObject);var skin=new Skin(v.gameObject);var content=n[5];
            At(n[3],content,0,0,654,655);skin.Panel(n[3].GetComponent<Image>());HideGraphic(n[6]);HideGraphic(n[12]);
            At(v.title.rectTransform,content,-10,248,500,110);TextStyle(v.title,48,true);
            skin.Close(v.close,n[3]);At(v.tips.rectTransform,content,0,-27,552,130);TextStyle(v.tips,32);
            At((RectTransform)v.confirm.transform,content,0,-211,470,122);skin.Pill(v.confirm);TextStyle(v.confirmLabel,45,true);Stretch(v.confirmLabel.rectTransform,40,13,40,13);skin.Save();
        }
        static void Guides(RecoveredGameSession s)
        {
            var n=Nodes(s.guideView.gameObject);var skin=new Skin(s.guideView.gameObject);
            foreach(int id in new[]{2,8,27,32})skin.Art(n[id].GetComponent<Image>(),"UnifiedReskin/Body",true);
            foreach(int id in new[]{25,28,33,36})skin.Art(n[id].GetComponent<Image>(),"RulesReskin/Chip2000");
            foreach(int id in new[]{26,29,30,34,37,38}){var t=n[id].GetComponent<Text>();TextStyle(t,t.fontSize);}
            foreach(int id in new[]{26,29,34,37}){var r=n[id];r.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,480);}
            skin.Save();
        }
        static void OtherMenus(RecoveredGameSession s)
        {
            var menus=s.menus;
            // Existing Rules, Settings and cash designs are retained. These are the remaining functional pages.
            int[] frameIds={4,0,2,22,21,2,2,2};int[] pages={2,4,5,6,7,8,9,10};int[] titles={13,10,9,11,9,12,6,4};int[] banners={12,33,8,10,8,11,5,0};
            for(int p=0;p<pages.Length;p++)
            {
                int index=pages[p];var page=menus.pages[index];var n=Nodes(page);var skin=new Skin(page);
                // The coin withdrawal page is being authored to its separate approved reference.
                if(index==4){CoinPage(menus,n,skin);skin.Save();continue;}
                var frame=n[frameIds[p]];skin.Panel(frame.GetComponent<Image>());
                var title=n[titles[p]].GetComponent<Text>();At(title.rectTransform,frame,-10,frame.rect.height*.5f-81,frame.rect.width-150,110);TextStyle(title,48,true);
                if(banners[p]!=0)HideGraphic(n[banners[p]]);
                int closeId=index==2?25:index==5?16:index==6?24:index==7?23:index==9?7:index==10?11:0;
                foreach(var binding in menus.actions)if(binding.button.transform.IsChildOf(page.transform))
                {if(closeId!=0&&binding.button.gameObject==n[closeId].gameObject)skin.Close(binding.button,frame);else if(binding.button.targetGraphic is Image&&binding.button.GetComponentInChildren<Text>(true))skin.Pill(binding.button);}
                if(index==8||index==9||index==10)
                {
                    int confirm=index==8?13:index==9?8:3;At(n[confirm],frame,0,-frame.rect.height*.5f+112,470,122);skin.Pill(n[confirm].GetComponent<Button>());
                    if(index==9){HideGraphic(n[15]);At(n[18],frame,0,150,530,100);At(n[16],frame,0,20,520,100);skin.Art(n[14].GetComponent<Image>(),"HomeReskin/Notice");n[14].GetComponent<Image>().color=new Color(.65f,.87f,1,.3f);}
                }
                // Remove only the old header coin collage; body diagrams and verification animations remain live.
                int decoration=index==5?14:index==6?21:index==7?20:index==8?15:0;
                if(decoration!=0&&n.ContainsKey(decoration))HideGraphic(n[decoration]);
                foreach(var text in page.GetComponentsInChildren<Text>(true))if(text!=title&&!text.GetComponentInParent<Button>())TextStyle(text,Mathf.Clamp(text.fontSize,24,56));
                foreach(var input in page.GetComponentsInChildren<InputField>(true))
                {
                    if(input.targetGraphic is Image field){skin.Art(field,"UnifiedReskin/Body",true);field.pixelsPerUnitMultiplier=3;}
                    TextStyle(input.textComponent,30);input.textComponent.alignment=TextAnchor.MiddleLeft;input.textComponent.horizontalOverflow=HorizontalWrapMode.Overflow;
                    if(input.placeholder is Text hint){TextStyle(hint,28);hint.alignment=TextAnchor.MiddleLeft;}
                }
                skin.Save();
            }
            foreach(var form in menus.forms){form.enabledSprite="UnifiedReskin/Button";form.disabledSprite="CoinReskin/WithdrawDisabled";}
            var toast=menus.toast.GetComponent<Image>();if(toast){var skin=new Skin(menus.toast);skin.Art(toast,"HomeReskin/Notice");toast.preserveAspect=false;skin.Save();TextStyle(menus.toastText,30,true);Stretch(menus.toastText.rectTransform,25,16,25,16);}
        }
        static void CoinPage(RecoveredMainMenus menus,Dictionary<int,RectTransform> n,Skin skin)
        {
            const float k=750f/941f;
            var page=menus.pages[4];var content=n[3];Stretch(content,0,0,0,0);
            At(n[23],page.transform,0,0,750,1624);Stretch(n[23],0,0,0,0);skin.Art(n[23].GetComponent<Image>(),"CoinReskin/Background");n[23].GetComponent<Image>().preserveAspect=false;n[23].SetAsFirstSibling();
            foreach(int id in new[]{33,49,2})HideGraphic(n[id]);
            // Native anchors keep the reference's header/footer proportions, extending only the scroll area on tall screens.
            At(n[4],content,0,-465*k*.5f,750,465*k);n[4].anchorMin=n[4].anchorMax=new Vector2(.5f,1);
            At(n[2],content,0,571*k*.5f,750,571*k);n[2].anchorMin=n[2].anchorMax=new Vector2(.5f,0);
            n[5].SetParent(content,false);Stretch(n[5],17*k,591*k,15*k,482*k);skin.Art(n[5].GetComponent<Image>(),"CoinReskin/Amounts",true);n[5].GetComponent<Image>().pixelsPerUnitMultiplier=1/k;
            Place(n[10],n[4],470,109,690,122,941,465);TextStyle(n[10].GetComponent<Text>(),73,true);
            Place(n[34],n[4],62,115,94,94,941,465);skin.Art(n[34].GetComponent<Image>(),"CoinReskin/Back");
            Place(n[11],n[4],471.5f,338,909,252,941,465);skin.Art(n[11].GetComponent<Image>(),"CoinReskin/Balance");
            Place(n[15],n[4],471.5f,338,909,252,941,465);
            At(n[39],n[15],-174*k,12*k,154*k,154*k);skin.Art(n[39].GetComponent<Image>(),"RulesReskin/Chip2000");
            At(n[16],n[15],44*k,26*k,270*k,119*k);TextStyle(n[16].GetComponent<Text>(),86,true);n[16].GetComponent<RecoveredRoundOutline>().effectColor=new Color(.3f,0,.62f);
            At(n[17],n[15],20*k,-64*k,640*k,67*k);TextStyle(n[17].GetComponent<Text>(),43,true);n[17].GetComponent<RecoveredRoundOutline>().effectColor=new Color(.3f,0,.62f);
            n[12].gameObject.SetActive(false);n[11].SetAsFirstSibling();n[15].SetAsLastSibling();
            // Existing locale/payment controls are retained in the header's spare line.
            Place(n[7],n[4],710,171,340,40,941,465);
            At(n[6],n[5],0,-72*k,825*k,110*k);n[6].anchorMin=n[6].anchorMax=new Vector2(.5f,1);TextStyle(n[6].GetComponent<Text>(),55,true);
            n[18].SetParent(n[5],false);Stretch(n[18],29*k,34*k,29*k,157*k);Stretch(n[19],0,0,0,0);
            var scroll=n[18].GetComponent<ScrollRect>();scroll.horizontal=false;scroll.vertical=true;scroll.movementType=ScrollRect.MovementType.Clamped;
            var list=n[20];list.anchorMin=list.anchorMax=new Vector2(.5f,1);list.pivot=new Vector2(.5f,1);list.sizeDelta=new Vector2(853*k,510*k);list.anchoredPosition=Vector2.zero;
            for(int i=0;i<menus.coinRows.Length;i++)
            {
                var row=menus.coinRows[i];var rn=Nodes(row.root);var r=(RectTransform)row.root.transform;
                At(r,list,(-219+(i%2)*439)*k,-(83+(i/2)*173)*k,421*k,166*k);r.anchorMin=r.anchorMax=new Vector2(.5f,1);
                foreach(int id in new[]{2,3,4})
                {
                    var img=rn[id].GetComponent<Image>();At(rn[id],r,0,0,421*k,166*k);skin.Art(img,id==2?"CoinReskin/CardSelected":"CoinReskin/Card");img.raycastTarget=true;
                    var label=img.GetComponentInChildren<Text>(true);if(label){TextStyle(label,54,true);At(label.rectTransform,img.transform,-4*k,6*k,366*k,110*k);label.color=id==2?Color.white:new Color(1,.92f,.18f);label.GetComponent<RecoveredRoundOutline>().effectColor=id==2?new Color(.05f,.49f,.03f):new Color(.85f,.25f,0);}
                }
            }
            Place(n[8],n[2],471.5f,149.5f,909,299,941,571);skin.Art(n[8].GetComponent<Image>(),"CoinReskin/Conditions");
            Place(n[53],n[2],471,65,840,96,941,571);TextStyle(n[53].GetComponent<Text>(),47,true);
            Place(n[55],n[2],470,239,800,77,941,571);TextStyle(n[55].GetComponent<Text>(),28);n[55].GetComponent<Text>().color=new Color(.19f,.39f,.76f);
            // Track is painted in the panel; the native filled Image remains the live progress indicator.
            Place(n[21],n[2],470,157,683,58,941,571);HideGraphic(n[21]);At(n[50],n[21],0,0,667*k,44*k);skin.Art(n[50].GetComponent<Image>(),"CashReskin/ProgressFill");n[50].GetComponent<Image>().type=Image.Type.Filled;n[50].GetComponent<Image>().fillMethod=Image.FillMethod.Horizontal;n[50].GetComponent<Image>().fillOrigin=0;n[50].GetComponent<Image>().preserveAspect=false;
            foreach(int id in new[]{9,22}){Place(n[id],n[2],472.5f,416.5f,525,169,941,571);}
            foreach(int id in new[]{57,58}){At(n[id],n[id==57?9:22],0,0,525*k,169*k);skin.Art(n[id].GetComponent<Image>(),id==57?"CashReskin/Withdraw":"CoinReskin/WithdrawDisabled");n[id].GetComponent<Image>().raycastTarget=true;var label=n[id==57?28:29];At(label,n[id],0,2*k,452*k,131*k);TextStyle(label.GetComponent<Text>(),72,true);label.GetComponent<RecoveredRoundOutline>().effectColor=id==57?new Color(0,.3f,0):new Color(.31f,.37f,.53f);}
            n[8].SetAsFirstSibling();
        }
        static void Place(RectTransform r,Transform parent,float x,float y,float w,float h,float pw,float ph)
        {
            const float k=750f/941f;At(r,parent,(x-pw*.5f)*k,(ph*.5f-y)*k,w*k,h*k);
        }
        static void HomeText(RecoveredGameSession s)
        {
            // The frame sizes stay untouched. Fit labels to the usable interior, not source placeholder widths.
            var source=s.moneyText.GetComponent<RecoveredNode>().sourceUuid;var n=new Dictionary<int,RectTransform>();foreach(var node in s.GetComponentsInChildren<RecoveredNode>(true))if(node.sourceUuid==source)n[node.sourceObjectId]=(RectTransform)node.transform;
            TextStyle(s.moneyText,42);s.moneyText.color=Color.white;s.moneyText.alignment=TextAnchor.MiddleRight;
            At(s.moneyText.rectTransform,n[6],10,0,140,58);
            At(n[25],n[42],0,0,137,65);TextStyle(n[25].GetComponent<Text>(),40,true);
            At(n[34],n[73],0,0,139,65);TextStyle(n[34].GetComponent<Text>(),43,true);
            At(n[56],n[21],.707f,-62.212f,98,36);TextStyle(n[56].GetComponent<Text>(),24);n[56].GetComponent<Text>().color=Color.white;
            foreach(var label in s.localizedLabels)if(label.label&&label.label.GetComponentInParent<Button>()){TextStyle(label.label,Mathf.Clamp(label.label.fontSize,28,58),true);if(label.label.transform.parent.GetComponent<Button>())Stretch(label.label.rectTransform,12,6,12,6);}
            if(s.bubbleText){TextStyle(s.bubbleText,34);s.bubbleText.alignment=TextAnchor.MiddleLeft;}
            var notice=s.notice;var frame=n[16];var viewport=frame.Find("NoticeViewport") as RectTransform;
            if(!viewport){viewport=(RectTransform)new GameObject("NoticeViewport",typeof(RectTransform),typeof(RectMask2D)).transform;viewport.SetParent(frame,false);}
            Stretch(viewport,70,8,12,8);At(notice.content,viewport,0,0,frame.rect.width-82,frame.rect.height-16);
            notice.label.rectTransform.SetParent(notice.content,false);Stretch(notice.label.rectTransform,4,5,4,5);TextStyle(notice.label,28);notice.label.color=Color.white;
            var hint=s.remainingText.rectTransform;At(hint,hint.parent,-115,hint.anchoredPosition.y,500,62);TextStyle(s.remainingText,33,true);
            TextStyle(s.progressText,27,true);if(s.highestText)TextStyle(s.highestText,34,true);
        }
        static void Packaged(GameObject root)
        {
            var session=root.GetComponent<PackagedGameSession>();int[] frames={9,14,39,10,12,12,11},titles={4,5,13,5,6,5,5},banners={7,4,12,8,5,4,4},closes={0,10,23,9,8,8,9};
            for(int i=0;i<session.dialogs.Length;i++)
            {
                var page=session.dialogs[i];var n=Nodes(page);var skin=new Skin(page);var frame=n[frames[i]];skin.Panel(frame.GetComponent<Image>());HideGraphic(n[banners[i]]);
                if(i==2)HideGraphic(n[38]);
                var title=n[titles[i]].GetComponent<Text>();At(title.rectTransform,frame,-15,frame.rect.height*.5f-82,frame.rect.width-140,108);TextStyle(title,48,true);
                if(closes[i]!=0)skin.Close(n[closes[i]].GetComponent<Button>(),frame);
                foreach(var b in page.GetComponentsInChildren<Button>(true))if(b.targetGraphic is Image&&b.GetComponentInChildren<Text>(true)&&b!=n[closes[i]==0?3:closes[i]].GetComponent<Button>())skin.Pill(b);
                foreach(var text in page.GetComponentsInChildren<Text>(true))if(text!=title&&!text.GetComponentInParent<Button>())TextStyle(text,Mathf.Clamp(text.fontSize,22,50));
                if(i==1)HideGraphic(n[9]);
                if(i==2)for(int row=3;row<=9;row++)skin.Art(n[row].GetComponent<Image>(),"UnifiedReskin/Body",true);
                skin.Save();
            }
            var toast=new Skin(session.toast);var tn=Nodes(session.toast);toast.Art(tn[2].GetComponent<Image>(),"HomeReskin/Notice");tn[2].GetComponent<Image>().preserveAspect=false;Stretch(session.toastText.rectTransform,24,6,24,6);toast.Save();
        }
        public static void FitAll(GameObject root)
        {
            if(!font)font=AssetDatabase.LoadAssetAtPath<Font>("Assets/Art/PopupFonts/Nunito-ExtraBold.ttf");
            var session=root.GetComponent<RecoveredGameSession>();
            foreach(var t in root.GetComponentsInChildren<Text>(true))
            {
                if(t.font&&t.font.name=="FZY4JW")t.font=font;
                t.lineSpacing=1;t.resizeTextForBestFit=true;t.resizeTextMaxSize=Mathf.Max(8,t.fontSize);t.resizeTextMinSize=8;
                t.horizontalOverflow=HorizontalWrapMode.Wrap;t.verticalOverflow=VerticalWrapMode.Truncate;
                var input=t.GetComponentInParent<InputField>();if(input&&input.textComponent==t){t.horizontalOverflow=HorizontalWrapMode.Overflow;continue;}
                var fit=t.GetComponent<RecoveredTextFit>();if(!fit){fit=t.gameObject.AddComponent<RecoveredTextFit>();fit.maximumFontSize=Mathf.Max(8,t.fontSize);}fit.minimumFontSize=8;fit.padding=new Vector2(3,3);var outline=t.GetComponent<RecoveredRoundOutline>();
                fit.singleLine=outline&&outline.enabled||t.GetComponentInParent<Button>()||t.rectTransform.rect.height<fit.maximumFontSize*2.6f&&t.text.IndexOf('\n')<0;
                if(session&&session.notice&&t==session.notice.label)fit.singleLine=false;
                t.resizeTextForBestFit=false;t.verticalOverflow=VerticalWrapMode.Overflow;
                // A direct child label fills its existing Button with an inset; other hierarchical labels retain their authored layout.
                if(t.transform.parent&&t.transform.parent.GetComponent<Button>()&&t.gameObject!=t.transform.parent.gameObject&&!(t.transform.parent.GetComponent<Button>().targetGraphic is Text))
                {
                    var parent=(RectTransform)t.transform.parent;if(parent.rect.width>110&&parent.rect.height>50)Stretch(t.rectTransform,Mathf.Min(32,parent.rect.width*.10f),6,Mathf.Min(32,parent.rect.width*.10f),6);
                }
            }
        }
    }
}
