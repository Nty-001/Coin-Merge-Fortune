using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace CoinMerge.Recovery.Editor
{
    public static class SettingsReskinAuthor
    {
        const float Scale=750f/1254;
        const string Art="Assets/Resources/SettingsReskin/";
        [MenuItem("Coin Merge/Reskin/Apply reference Settings")]
        public static void Run()
        {
            if(EditorApplication.isPlaying)throw new InvalidOperationException("Stop Play before authoring settings assets.");
            var previous=UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            if(previous.isDirty){Directory.CreateDirectory("Temp/SettingsReskinBackup");EditorSceneManager.SaveScene(previous,"Temp/SettingsReskinBackup/BeforeAuthoring.unity",true);}
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
            EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();Debug.Log("SETTINGS_RESKIN_AUTHORED: native Buttons, localized text and existing settings state preserved");
        }
        static void Center(RectTransform rect,RectTransform parent,Vector2 position,Vector2 size)
        {
            rect.SetParent(parent,false);rect.anchorMin=rect.anchorMax=rect.pivot=new Vector2(.5f,.5f);
            rect.localRotation=Quaternion.identity;rect.localScale=Vector3.one;rect.anchoredPosition=position;rect.sizeDelta=size;
        }
        static void Reference(RectTransform rect,RectTransform parent,float x,float y,float w,float h)
        {Center(rect,parent,new Vector2(x+w*.5f-627,610.5f-y-h*.5f)*Scale,new Vector2(w,h)*Scale);}
        static void Style(Text text,int pixels,Color color,float outline=0,bool bold=false)
        {
            text.font=AssetDatabase.LoadAssetAtPath<Font>("Assets/Art/PopupFonts/Nunito-"+(bold?"Black":"ExtraBold")+".ttf");
            text.fontSize=Mathf.RoundToInt(pixels*Scale);text.fontStyle=FontStyle.Normal;text.color=color;
            text.resizeTextForBestFit=true;text.resizeTextMaxSize=text.fontSize;text.resizeTextMinSize=Mathf.RoundToInt(text.fontSize*.65f);
            text.alignment=TextAnchor.MiddleLeft;text.horizontalOverflow=HorizontalWrapMode.Wrap;text.verticalOverflow=VerticalWrapMode.Truncate;
            var effect=text.GetComponent<RecoveredRoundOutline>();
            if(outline>0){if(!effect)effect=text.gameObject.AddComponent<RecoveredRoundOutline>();effect.enabled=true;effect.effectColor=new Color(0,.2f,.75f);effect.effectDistance=Vector2.one*(outline*Scale);}
            else if(effect)effect.enabled=false;
        }
        static void Apply(GameObject root)
        {
            var page=root.GetComponent<RecoveredGameSession>().menus.pages[0];
            var nodes=new Dictionary<int,RectTransform>();foreach(var n in page.GetComponentsInChildren<RecoveredNode>(true))nodes[n.sourceObjectId]=(RectTransform)n.transform;
            var content=nodes[2];var loader=page.GetComponent<RecoveredMenuArt>();var bindings=new List<MenuImageBinding>(loader.images);
            void ArtFor(Image image,string name)
            {
                var binding=bindings.Find(x=>x.image==image);if(binding==null){binding=new MenuImageBinding{image=image};bindings.Add(binding);}binding.resourcePath="SettingsReskin/"+name;
                image.sprite=null;image.type=Image.Type.Simple;image.preserveAspect=true;image.color=Color.white;image.enabled=true;
            }
            // One fixed, serialized panel; all live labels and controls remain native children.
            Center(content,(RectTransform)page.transform,new Vector2(0,16.5f)*Scale,new Vector2(1142,961)*Scale);
            ArtFor(content.GetComponent<Image>(),"Panel");
            content.GetComponent<Image>().preserveAspect=false;
            nodes[5].GetComponent<Image>().enabled=false;nodes[10].GetComponent<Image>().enabled=false;nodes[25].GetComponent<Image>().enabled=false;
            Reference(nodes[15],content,510,205,414,135);Style(nodes[15].GetComponent<Text>(),113,Color.white,7,true);
            Reference(nodes[6],content,346,494,474,110);Style(nodes[6].GetComponent<Text>(),75,new Color(0,.18f,.66f),0,true);
            Reference(nodes[11],content,346,729,490,110);Style(nodes[11].GetComponent<Text>(),75,new Color(0,.18f,.66f),0,true);
            // On/off images each keep the source aspect; the state containers keep their original binding.
            foreach(int id in new[]{8,9,13,14})Center(nodes[id],content,new Vector2(970-627,610.5f-(id<10?550:782))*Scale,new Vector2(247,136)*Scale);
            foreach(int id in new[]{19,20,22,23})
            {
                bool on=id==19||id==22;Center(nodes[id],nodes[on?(id==19?8:13):(id==20?9:14)],Vector2.zero,new Vector2(on?247:242,on?136:129)*Scale);
                ArtFor(nodes[id].GetComponent<Image>(),on?"SwitchOn":"SwitchOff");
            }
            Reference(nodes[27],content,1033,157,153,153);ArtFor(nodes[27].GetComponent<Image>(),"Close");
            Reference(nodes[28],content,225,931,400,94);Style(nodes[28].GetComponent<Text>(),49,new Color(0,.24f,.82f),0,true);nodes[28].GetComponent<Text>().alignment=TextAnchor.MiddleCenter;
            Reference(nodes[29],content,689,931,340,94);Style(nodes[29].GetComponent<Text>(),49,new Color(0,.24f,.82f),0,true);nodes[29].GetComponent<Text>().alignment=TextAnchor.MiddleCenter;
            var divider=content.Find("PolicyDivider");Image line;
            if(divider)line=divider.GetComponent<Image>();else{var go=new GameObject("PolicyDivider",typeof(RectTransform),typeof(CanvasRenderer),typeof(Image));line=go.GetComponent<Image>();}
            Reference(line.rectTransform,content,639,956,4,45);line.color=new Color(.45f,.76f,.94f);line.raycastTarget=false;
            loader.images=bindings.ToArray();
            foreach(int id in new[]{19,20,22,23,27,28,29}){var b=nodes[id].GetComponent<Button>();if(!b||b.targetGraphic.gameObject!=b.gameObject||b.onClick.GetPersistentEventCount()!=0)throw new Exception("Settings native Button binding changed: "+id);}
        }
    }
}
