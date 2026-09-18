using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
namespace CoinMerge.Recovery.Editor
{
    public static class CashReskinAuthor
    {
        const float S=750f/941;
        const string Art="Assets/Resources/CashReskin/";
        [MenuItem("Coin Merge/Reskin/Apply reference Cash withdrawal")]
        public static void Run()
        {
            if(EditorApplication.isPlaying)throw new InvalidOperationException("Stop Play before authoring cash page.");
            var previous=UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            if(previous.isDirty){Directory.CreateDirectory("Temp/CashReskinBackup");EditorSceneManager.SaveScene(previous,"Temp/CashReskinBackup/BeforeAuthoring.unity",true);}
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            foreach(var file in Directory.GetFiles(Art,"*.png"))
            {
                var importer=(TextureImporter)AssetImporter.GetAtPath(file.Replace('\\','/'));
                importer.textureType=TextureImporterType.Sprite;importer.spriteImportMode=SpriteImportMode.Single;
                importer.spritePixelsPerUnit=32;importer.mipmapEnabled=false;importer.alphaIsTransparency=true;
                importer.textureCompression=TextureImporterCompression.Uncompressed;importer.maxTextureSize=2048;
                importer.filterMode=FilterMode.Bilinear;importer.wrapMode=TextureWrapMode.Clamp;
                var settings=new TextureImporterSettings();importer.ReadTextureSettings(settings);settings.spriteMeshType=SpriteMeshType.FullRect;importer.SetTextureSettings(settings);importer.SaveAndReimport();
            }
            const string path="Assets/Prefabs/Runtime/RecoveredMain.prefab";
            var root=PrefabUtility.LoadPrefabContents(path);
            try{Apply(root);PrefabUtility.SaveAsPrefabAsset(root,path);}finally{PrefabUtility.UnloadPrefabContents(root);}
            var scene=EditorSceneManager.OpenScene("Assets/Scenes/RecoveredMain.unity");
            foreach(var item in scene.GetRootGameObjects())if(item.GetComponent<RecoveredGameSession>())Apply(item);
            EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();Debug.Log("CASH_RESKIN_AUTHORED");
        }
        static Dictionary<int,RectTransform> Nodes(GameObject root)
        {
            string uuid=root.GetComponent<RecoveredNode>().sourceUuid;var result=new Dictionary<int,RectTransform>();
            foreach(var node in root.GetComponentsInChildren<RecoveredNode>(true))if(node.sourceUuid==uuid)result[node.sourceObjectId]=(RectTransform)node.transform;return result;
        }
        static void Place(RectTransform rect,RectTransform parent,float x,float y,float w,float h,float pw,float ph)
        {
            rect.SetParent(parent,false);rect.anchorMin=rect.anchorMax=rect.pivot=new Vector2(.5f,.5f);rect.localRotation=Quaternion.identity;rect.localScale=Vector3.one;
            rect.anchoredPosition=new Vector2(x+w*.5f-pw*.5f,ph*.5f-y-h*.5f)*S;rect.sizeDelta=new Vector2(w,h)*S;
        }
        static void Style(Text t,int pixels,Color color,bool black=false,float outline=0,Color? stroke=null,TextAnchor alignment=TextAnchor.MiddleLeft)
        {
            t.font=AssetDatabase.LoadAssetAtPath<Font>("Assets/Art/PopupFonts/Nunito-"+(black?"Black":"ExtraBold")+".ttf");
            t.fontStyle=FontStyle.Normal;t.fontSize=Mathf.RoundToInt(pixels*S);t.resizeTextForBestFit=true;t.resizeTextMaxSize=t.fontSize;t.resizeTextMinSize=Mathf.RoundToInt(t.fontSize*.50f);
            t.color=color;t.alignment=alignment;t.horizontalOverflow=HorizontalWrapMode.Wrap;t.verticalOverflow=VerticalWrapMode.Truncate;t.raycastTarget=false;
            foreach(var effect in t.GetComponents<Shadow>())effect.enabled=false;
            if(outline>0){var effect=t.GetComponent<RecoveredRoundOutline>();if(!effect)effect=t.gameObject.AddComponent<RecoveredRoundOutline>();effect.enabled=true;effect.effectColor=stroke??new Color(0,.28f,.8f);effect.effectDistance=Vector2.one*(outline*S);}
        }
        static Image ImageChild(RectTransform parent,string name)
        {
            var old=parent.Find(name);if(old)return old.GetComponent<Image>();
            return new GameObject(name,typeof(RectTransform),typeof(CanvasRenderer),typeof(Image)).GetComponent<Image>();
        }
        static void Relief(Text text,Color light,Color dark,Color shadowColor,float depth)
        {
            var gradient=text.GetComponent<RecoveredTextVerticalGradient>();if(!gradient)gradient=text.gameObject.AddComponent<RecoveredTextVerticalGradient>();gradient.top=light;gradient.bottom=dark;
            Shadow shadow=null;foreach(var item in text.GetComponents<Shadow>())if(!(item is Outline)){shadow=item;break;}
            if(!shadow)shadow=text.gameObject.AddComponent<Shadow>();shadow.enabled=true;shadow.effectColor=shadowColor;shadow.effectDistance=new Vector2(0,-depth*S);
        }
        static void Apply(GameObject root)
        {
            var menus=root.GetComponent<RecoveredGameSession>().menus;var page=menus.pages[3];var n=Nodes(page);var content=n[2];
            var loader=page.GetComponent<RecoveredMenuArt>();var bindings=new List<MenuImageBinding>(loader.images);
            void Sprite(Image image,string path,bool preserve=true){var binding=bindings.Find(b=>b.image==image);if(binding==null){binding=new MenuImageBinding{image=image};bindings.Add(binding);}binding.resourcePath=path;image.sprite=null;image.color=Color.white;image.type=Image.Type.Simple;image.preserveAspect=preserve;image.enabled=true;}
            var layout=page.GetComponent<RecoveredCashPageLayout>();layout.topHeight=523*S;layout.bottomHeight=246*S;layout.gap=0;layout.minScrollHeight=300;layout.useAuthoredMargin=true;layout.authoredMargin=0;
            n[3].sizeDelta=new Vector2(750,523*S);n[5].sizeDelta=new Vector2(750,246*S);
            var bg=n[10].GetComponent<Image>();Sprite(bg,"HomeReskin/Sky",false);n[10].anchorMin=Vector2.zero;n[10].anchorMax=Vector2.one;n[10].offsetMin=n[10].offsetMax=Vector2.zero;n[10].SetAsFirstSibling();bg.raycastTarget=true;
            foreach(int id in new[]{4,19})n[id].GetComponent<Image>().enabled=false;
            var hero=ImageChild(n[3],"CashHeroSky");Place(hero.rectTransform,n[3],0,173,941,350,941,523);Sprite(hero,"CashReskin/HeroSky");hero.raycastTarget=false;hero.transform.SetAsFirstSibling();
            Place(n[15],n[3],14,22,915,150,941,523);Sprite(n[15].GetComponent<Image>(),"CashReskin/Header");n[15].SetAsFirstSibling();
            Place(n[16],n[3],33,47,98,98,941,523);Sprite(n[16].GetComponent<Image>(),"CashReskin/Back");
            Place(n[17],n[3],275,30,405,125,941,523);Style(n[17].GetComponent<Text>(),91,Color.white,true,5,null,TextAnchor.MiddleCenter);
            Place(n[20],n[3],31,283,405,112,941,523);Style(n[20].GetComponent<Text>(),89,new Color(1,.91f,.17f),true,3,new Color(.76f,.35f,.02f));
            Place(n[22],n[3],34,385,376,100,941,523);Style(n[22].GetComponent<Text>(),72,Color.white,true,3,null,TextAnchor.MiddleCenter);
            Relief(n[17].GetComponent<Text>(),Color.white,new Color(.72f,.95f,1),new Color(0,.2f,.6f,.45f),5);
            Relief(n[20].GetComponent<Text>(),new Color(1,1,.65f),new Color(1,.65f,.015f),new Color(.45f,.24f,.08f,.4f),5);
            Relief(n[22].GetComponent<Text>(),Color.white,new Color(.75f,.95f,1),new Color(0,.24f,.65f,.4f),4);
            // Accepted individual chip sprites remain separate, uniformly scaled native Images.
            int[] values={1000,500,2000};float[] xs={410,744,529},ys={317,336,231},sizes={184,165,263};
            for(int i=0;i<values.Length;i++){var img=ImageChild(n[3],"CashChip"+values[i]);Place(img.rectTransform,n[3],xs[i],ys[i],sizes[i],sizes[i],941,523);Sprite(img,"RulesReskin/Chip"+values[i]);img.raycastTarget=false;}
            Place(n[24],n[5],0,0,941,246,941,246);Sprite(n[24].GetComponent<Image>(),"CashReskin/Footer",false);n[24].SetAsFirstSibling();
            Place(n[25],n[5],207,28,525,169,941,246);Sprite(n[25].GetComponent<Image>(),"CashReskin/Withdraw");
            Place(n[12],n[25],28,4,469,145,525,169);Style(n[12].GetComponent<Text>(),83,Color.white,true,5,new Color(0,.35f,.015f),TextAnchor.MiddleCenter);n[12].SetAsLastSibling();
            Relief(n[12].GetComponent<Text>(),Color.white,new Color(.8f,1,.7f),new Color(0,.23f,0,.45f),5);
            n[13].gameObject.SetActive(false);
            n[5].SetAsLastSibling();
            var list=n[9];list.sizeDelta=new Vector2(750,6*436*S);list.anchoredPosition=Vector2.zero;
            for(int i=0;i<menus.fakeRows.Length;i++)
            {
                var row=menus.fakeRows[i];var r=(RectTransform)row.root.transform;var rn=Nodes(row.root);
                r.anchorMin=r.anchorMax=new Vector2(.5f,1);r.pivot=new Vector2(.5f,1);r.sizeDelta=new Vector2(905,420)*S;r.anchoredPosition=new Vector2(0,-i*436*S);
                foreach(var graphic in row.root.GetComponentsInChildren<Graphic>(true))graphic.enabled=false;
                // Reparent live fields to the row so selected/unselected background visibility never hides text.
                Place((RectTransform)row.selected.transform,r,0,0,905,420,905,420);Sprite(row.selected.GetComponent<Image>(),"CashReskin/CardSelected");
                Place((RectTransform)row.unselected.transform,r,0,0,905,420,905,420);Sprite(row.unselected.GetComponent<Image>(),"CashReskin/Card");
                row.selected.transform.SetAsFirstSibling();row.unselected.transform.SetAsFirstSibling();
                row.selected.GetComponent<Image>().raycastTarget=true;row.unselected.GetComponent<Image>().raycastTarget=true;
                Place(row.amount.rectTransform,r,44,27,687,133,905,420);row.amount.enabled=true;Style(row.amount,107,new Color(.97f,.015f,.025f),true,2,Color.white);
                Relief(row.amount,new Color(1,.25f,.19f),new Color(.8f,0,0),new Color(.7f,.1f,.1f,.35f),4);
                Place(row.condition.rectTransform,r,44,185,816,60,905,420);row.condition.enabled=true;Style(row.condition,44,new Color(0,.28f,.9f));
                Place(row.remaining.rectTransform,r,44,238,816,60,905,420);row.remaining.enabled=true;Style(row.remaining,44,new Color(0,.28f,.9f));
                Place(row.fill.rectTransform,r,59,325,785,42,905,420);Sprite(row.fill,"CashReskin/ProgressFill",false);row.fill.type=Image.Type.Filled;row.fill.fillMethod=Image.FillMethod.Horizontal;row.fill.fillOrigin=0;row.fill.raycastTarget=false;row.fill.transform.SetAsLastSibling();
                var rowArt=row.root.GetComponent<RecoveredMenuArt>();
                if(rowArt){var b=new List<MenuImageBinding>(rowArt.images);b.RemoveAll(x=>x.image==row.selected.GetComponent<Image>()||x.image==row.unselected.GetComponent<Image>()||x.image==row.fill);rowArt.images=b.ToArray();}
            }
            loader.images=bindings.ToArray();
            foreach(int id in new[]{16,25}){var b=n[id].GetComponent<Button>();if(!b||b.targetGraphic.gameObject!=b.gameObject||b.onClick.GetPersistentEventCount()!=0)throw new Exception("Cash button binding changed: "+id);}
        }
    }
}
