using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace CoinMerge.Recovery.Editor
{
    public static class ReferencePopupsAuthor
    {
        const string Art="ReferencePopups/";
        static readonly Color Navy=new Color(.015f,.11f,.37f);
        [MenuItem("Coin Merge/Reskin/Apply settings and reward references")]
        public static void Run()
        {
            if(EditorApplication.isPlayingOrWillChangePlaymode)throw new InvalidOperationException("Author in Edit Mode.");
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            foreach(string file in Directory.GetFiles("Assets/Resources/ReferencePopups","*.png"))
            {
                var t=(TextureImporter)AssetImporter.GetAtPath(file.Replace('\\','/'));
                t.textureType=TextureImporterType.Sprite;t.spriteImportMode=SpriteImportMode.Single;t.spritePixelsPerUnit=100;
                t.mipmapEnabled=false;t.alphaIsTransparency=true;t.textureCompression=TextureImporterCompression.Uncompressed;
                t.maxTextureSize=2048;t.filterMode=FilterMode.Bilinear;t.wrapMode=TextureWrapMode.Clamp;
                var settings=new TextureImporterSettings();t.ReadTextureSettings(settings);settings.spriteMeshType=SpriteMeshType.FullRect;t.SetTextureSettings(settings);
                t.SaveAndReimport();
            }
            const string path="Assets/Prefabs/Runtime/RecoveredMain.prefab";
            var root=PrefabUtility.LoadPrefabContents(path);
            try{Apply(root);PrefabUtility.SaveAsPrefabAsset(root,path);}finally{PrefabUtility.UnloadPrefabContents(root);}
            var scene=EditorSceneManager.OpenScene("Assets/Scenes/RecoveredMain.unity");
            foreach(var rootObject in scene.GetRootGameObjects())if(rootObject.GetComponent<RecoveredGameSession>())Apply(rootObject);
            EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();Debug.Log("REFERENCE_POPUPS_AUTHORED");
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
            var g=new GameObject(name,typeof(RectTransform),typeof(CanvasRenderer),typeof(Image));g.layer=parent.gameObject.layer;g.transform.SetParent(parent,false);
            var image=g.GetComponent<Image>();image.raycastTarget=false;return image;
        }
        static Text NewText(string name,Transform parent)
        {
            var g=new GameObject(name,typeof(RectTransform),typeof(CanvasRenderer),typeof(Text));g.layer=parent.gameObject.layer;g.transform.SetParent(parent,false);
            return g.GetComponent<Text>();
        }
        sealed class Skin
        {
            readonly RecoveredMenuArt loader;readonly List<MenuImageBinding> list;
            public Skin(GameObject root){loader=root.GetComponent<RecoveredMenuArt>();if(!loader)loader=root.AddComponent<RecoveredMenuArt>();list=new List<MenuImageBinding>(loader.images??Array.Empty<MenuImageBinding>());}
            public void Set(Image image,string path)
            {
                var item=list.Find(b=>b.image==image);if(item==null){item=new MenuImageBinding{image=image};list.Add(item);}item.resourcePath=path;
                image.sprite=null;image.type=Image.Type.Simple;image.preserveAspect=false;image.color=Color.white;image.enabled=true;
            }
            public void Save(){loader.images=list.ToArray();}
        }
        static void Style(Text t,int fontSize,Color color,Color stroke,float radius,bool gradient=false)
        {
            foreach(var effect in t.GetComponents<BaseMeshEffect>())effect.enabled=false;
            t.font=AssetDatabase.LoadAssetAtPath<Font>("Assets/Art/PopupFonts/Nunito-Black.ttf");t.fontStyle=FontStyle.Normal;
            t.fontSize=fontSize;t.color=color;t.alignment=TextAnchor.MiddleCenter;t.horizontalOverflow=HorizontalWrapMode.Wrap;t.verticalOverflow=VerticalWrapMode.Truncate;
            t.resizeTextForBestFit=true;t.resizeTextMinSize=fontSize/2;t.resizeTextMaxSize=fontSize;
            if(gradient)
            {
                var g=t.GetComponent<RecoveredTextVerticalGradient>();if(!g)g=t.gameObject.AddComponent<RecoveredTextVerticalGradient>();
                g.enabled=true;g.top=Color.white;g.bottom=new Color(.62f,.90f,1);
            }
            if(radius>0)
            {
                var lettering=t.GetComponent<RecoveredPopupLettering>();if(!lettering)lettering=t.gameObject.AddComponent<RecoveredPopupLettering>();
                lettering.enabled=true;lettering.outline=stroke;lettering.depthColor=stroke*.64f;lettering.depthColor=new Color(lettering.depthColor.r,lettering.depthColor.g,lettering.depthColor.b,1);
                lettering.radius=radius;lettering.depth=4;lettering.edge=.9f;
            }
        }
        static void Apply(GameObject root){var s=root.GetComponent<RecoveredGameSession>();Settings(s);Rewards(s);}
        static void Settings(RecoveredGameSession s)
        {
            var page=s.menus.pages[RecoveredMainMenus.Settings];var n=Nodes(page);var panel=n[2];var skin=new Skin(page);
            // Original SettingDialog bg dimensions and location, from the recovered source node.
            At(panel,page.transform,-.34f,30.539f,654,562);skin.Set(panel.GetComponent<Image>(),Art+"SettingsPanel");
            foreach(int id in new[]{5,10,25})n[id].GetComponent<Graphic>().enabled=false;
            var divider=panel.Find("PolicyDivider");if(divider)divider.gameObject.SetActive(false);
            // Positions are expressed within the reference's complete panel, scaled to the original rect.
            void Ref(RectTransform r,float x,float y,float w,float h){At(r,panel,(x-.5f)*654,(.5f-y)*562,w*654,h*562);}
            Ref(n[15],.565f,.144f,.353f,.15f);Style(n[15].GetComponent<Text>(),60,Color.white,Navy,4,true);
            Ref(n[6],.295f,.411f,.40f,.13f);Ref(n[11],.302f,.630f,.415f,.13f);
            foreach(int id in new[]{6,11}){var t=n[id].GetComponent<Text>();Style(t,45,new Color(.01f,.17f,.48f),Navy,0);t.alignment=TextAnchor.MiddleLeft;}
            foreach(int id in new[]{8,9,13,14})Ref(n[id],.807f,id<10?.411f:.631f,.181f,.112f);
            foreach(int id in new[]{19,20,22,23})
            {
                bool on=id==19||id==22;var container=n[id==19?8:id==20?9:id==22?13:14];
                At(n[id],container,0,0,118,64);skin.Set(n[id].GetComponent<Image>(),"SettingsReskin/"+(on?"SwitchOn":"SwitchOff"));
                n[id].GetComponent<Image>().preserveAspect=true;
            }
            // Close sprite includes transparent export margin; visible button remains 62 x 60.
            Ref(n[27],.910f,.114f,.120f,.143f);skin.Set(n[27].GetComponent<Image>(),Art+"Close");
            Ref(n[28],.291f,.866f,.43f,.108f);Ref(n[29],.736f,.866f,.35f,.108f);
            foreach(int id in new[]{28,29})Style(n[id].GetComponent<Text>(),34,new Color(.01f,.17f,.48f),Navy,0);
            foreach(int id in new[]{15,6,11,8,9,13,14,27,28,29})n[id].SetAsLastSibling();
            skin.Save();
        }
        static void Rewards(RecoveredGameSession s)
        {
            var v=s.rewardView;var n=Nodes(v.gameObject);
            // Normal: original 719-wide ribbon reaches y381.6, original body ends at -232.24.
            var normal=(RectTransform)v.normalGroup.transform;normal.anchoredPosition=new Vector2(-2.135f,-2.693f);
            foreach(var g in normal.GetComponentsInChildren<Graphic>(true))g.enabled=false;
            // Currency localization can re-enable the old banknote graphic; retire its complete
            // authored container so it cannot overlap the cash illustration in the new panel.
            n[24].gameObject.SetActive(false);n[22].gameObject.SetActive(false);
            v.normalTitle.transform.SetParent(normal,false);v.normalAmount.transform.SetParent(normal,false);
            var normalArt=normal.Find("ReferenceReward");if(normalArt)UnityEngine.Object.DestroyImmediate(normalArt.gameObject);
            var frame=NewImage("ReferenceReward",normal);At(frame.rectTransform,normal,0,74.68f,719,613.84f);
            var normalSkin=new Skin(frame.gameObject);normalSkin.Set(frame,Art+"RewardPanel");normalSkin.Save();
            PlaceRewardLabel(v.normalTitle,frame.rectTransform,627,174,600,230,83,false);
            PlaceRewardLabel(v.normalAmount,frame.rectTransform,627,964,730,300,112,true);
            v.normalTitle.enabled=v.normalAmount.enabled=true;
            // Double-only native group leaves all revive, highest-coin and guide visuals intact.
            if(v.doubleRewardGroup)UnityEngine.Object.DestroyImmediate(v.doubleRewardGroup);
            var group=new GameObject("DoubleRewardReference",typeof(RectTransform));group.layer=normal.gameObject.layer;
            At((RectTransform)group.transform,normal.parent,-2.135f,-2.693f,800,800);
            v.doubleRewardGroup=group;var skin=new Skin(group);
            // Original DJB_TX coin crown reaches y454.8; original 662x535 body ends at -300.576.
            const float width=719,height=755.376f,cy=77.112f;
            void Coin(string value,float x,float y,float diameter)
            {
                var coin=NewImage("Coin"+value,group.transform);At(coin.rectTransform,group.transform,(x-627)*width/1254,cy+(627-y)*height/1254,diameter*width/1254,diameter*width/1254);
                skin.Set(coin,"Gameplay/Coins/"+value);coin.preserveAspect=true;
            }
            Coin("500",229,253,210);Coin("500",1027,253,210);Coin("1000",386,181,265);Coin("1000",863,181,265);Coin("2000",627,152,349);
            var panel=NewImage("DoubleRewardPanel",group.transform);At(panel.rectTransform,group.transform,0,cy,width,height);skin.Set(panel,Art+"DoubleRewardPanel");
            v.doubleRewardTitle=NewText("Title",group.transform);v.doubleRewardAmount=NewText("Amount",group.transform);
            PlaceRewardLabel(v.doubleRewardTitle,panel.rectTransform,627,392,835,230,73,false);
            v.doubleRewardTitle.rectTransform.localScale=new Vector3(.83f,1,1);
            v.doubleRewardTitle.rectTransform.sizeDelta=new Vector2(v.doubleRewardTitle.rectTransform.sizeDelta.x/.83f,v.doubleRewardTitle.rectTransform.sizeDelta.y);
            PlaceRewardLabel(v.doubleRewardAmount,panel.rectTransform,627,1017,730,300,112,true);
            skin.Save();group.SetActive(false);
        }
        static void PlaceRewardLabel(Text t,RectTransform frame,float x,float y,float w,float h,int size,bool amount)
        {
            At(t.rectTransform,frame,(x-627)*frame.rect.width/1254,(627-y)*frame.rect.height/1254,w*frame.rect.width/1254,h*frame.rect.height/1254);
            Style(t,size,Color.white,amount?new Color(.30f,.075f,.005f):Navy,amount?5:4,true);t.raycastTarget=false;
            if(amount){var g=t.GetComponent<RecoveredTextVerticalGradient>();g.top=new Color(1,1,.61f);g.bottom=new Color(1,.42f,.01f);}
            t.transform.SetAsLastSibling();
        }
    }
}
