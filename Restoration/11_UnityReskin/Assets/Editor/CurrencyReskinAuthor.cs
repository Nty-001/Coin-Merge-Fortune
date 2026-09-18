using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
namespace CoinMerge.Recovery.Editor
{
    [InitializeOnLoad] public static class CurrencyReskinAuthor
    {
        const string Folder="Assets/Prefabs/Runtime/",Output="Design/CurrencyR1/Verification";
        static readonly List<string> audit=new List<string>();
        static CurrencyReskinAuthor(){EditorApplication.update+=Poll;}
        static void Poll()
        {
            string request=File.Exists("Temp/CurrencyReskin.request")?"Temp/CurrencyReskin.request":"Temp/CurrencyReskinValidate.request";
            if(!File.Exists(request)||EditorApplication.isCompiling||EditorApplication.isUpdating)return;
            if(EditorApplication.isPlaying){EditorApplication.ExitPlaymode();return;}
            if(EditorApplication.isPlayingOrWillChangePlaymode)return;
            File.Delete(request);
            try{if(!request.Contains("Validate"))Run();CurrencyReskinValidation.Run();}
            catch(Exception e){Directory.CreateDirectory(Output);File.WriteAllText(Output+"/author_error.txt",e.ToString());Debug.LogException(e);}
        }
        [MenuItem("Coin Merge/Reskin/Apply approved localized banknotes")]
        public static void Run()
        {
            if(EditorApplication.isPlaying)throw new InvalidOperationException("Stop Play before authoring.");
            Directory.CreateDirectory(Output);audit.Clear();
            var previous=UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            if(previous.isDirty){Directory.CreateDirectory("Temp/CurrencyReskinBackup");EditorSceneManager.SaveScene(previous,"Temp/CurrencyReskinBackup/BeforeAuthoring.unity",true);}
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            foreach(string file in Directory.GetFiles("Assets/Resources/Localization/Currency/1","*.png"))
            {
                var importer=(TextureImporter)AssetImporter.GetAtPath(file.Replace('\\','/'));
                importer.textureType=TextureImporterType.Sprite;importer.spriteImportMode=SpriteImportMode.Single;
                importer.alphaIsTransparency=true;importer.mipmapEnabled=false;importer.npotScale=TextureImporterNPOTScale.None;
                importer.textureCompression=TextureImporterCompression.Uncompressed;importer.SaveAndReimport();
            }
            CreatePile(false);CreatePile(true);
            string prefab=Folder+"RecoveredMain.prefab";var root=PrefabUtility.LoadPrefabContents(prefab);
            try{Apply(root);PrefabUtility.SaveAsPrefabAsset(root,prefab);}finally{PrefabUtility.UnloadPrefabContents(root);}
            var scene=EditorSceneManager.OpenScene("Assets/Scenes/RecoveredMain.unity");
            foreach(var obj in scene.GetRootGameObjects())if(obj.GetComponent<RecoveredGameSession>())Apply(obj);
            EditorSceneManager.SaveScene(scene);
            var flight=PrefabUtility.LoadPrefabContents(Folder+"CashFlight.prefab");
            try{var image=flight.GetComponent<Image>();image.sprite=null;image.preserveAspect=true;PrefabUtility.SaveAsPrefabAsset(flight,Folder+"CashFlight.prefab");}finally{PrefabUtility.UnloadPrefabContents(flight);}
            AssetDatabase.SaveAssets();File.WriteAllLines(Output+"/authored-bindings.txt",audit);Debug.Log("APPROVED_CURRENCY_ART_AUTHORED");
        }
        static void CreatePile(bool large)
        {
            var root=new GameObject(large?"CurrencyLargePile":"CurrencySmallPile",typeof(RectTransform),typeof(AspectRatioFitter));
            try
            {
                var rect=(RectTransform)root.transform;rect.sizeDelta=new Vector2(360,240);
                var fit=root.GetComponent<AspectRatioFitter>();fit.aspectMode=AspectRatioFitter.AspectMode.FitInParent;fit.aspectRatio=1.5f;
                Vector2[] positions=large?new[]{new Vector2(-58,27),new Vector2(65,32),new Vector2(2,69),new Vector2(-103,-8),new Vector2(0,0),new Vector2(102,-10),new Vector2(-87,-62),new Vector2(2,-60),new Vector2(88,-63)}:new[]{new Vector2(-62,-25),new Vector2(62,-25),new Vector2(0,38)};
                Vector2 size=large?new Vector2(138,110):new Vector2(174,143);
                for(int i=0;i<positions.Length;i++)
                {
                    var child=new GameObject("Banknote_"+(i+1),typeof(RectTransform),typeof(CanvasRenderer),typeof(Image));child.transform.SetParent(rect,false);
                    var r=(RectTransform)child.transform;Vector2 center=Vector2.one*.5f+new Vector2(positions[i].x/360,positions[i].y/240),half=new Vector2(size.x/720,size.y/480);
                    r.anchorMin=center-half;r.anchorMax=center+half;r.offsetMin=r.offsetMax=Vector2.zero;
                    var image=child.GetComponent<Image>();image.raycastTarget=false;image.preserveAspect=true;image.sprite=null;
                }
                PrefabUtility.SaveAsPrefabAsset(root,Folder+root.name+".prefab");
            }
            finally{UnityEngine.Object.DestroyImmediate(root);}
        }
        static void Apply(GameObject root)
        {
            var s=root.GetComponent<RecoveredGameSession>();var seen=new HashSet<Image>();
            foreach(var binding in s.currencyIcons)Bind(binding.image,binding.type,seen);
            foreach(var binding in s.menus.currencies)Bind(binding.image,binding.type,seen);
            foreach(var slot in s.wheelView.slots)Bind(slot.money,3,seen,true);
            Bind(s.wheelRewardView.money,3,seen,true);
            // A decorative art loader must never replace a localized note with a fixed-country image.
            foreach(var loader in root.GetComponentsInChildren<RecoveredMenuArt>(true))
            {
                var list=new List<MenuImageBinding>(loader.images??Array.Empty<MenuImageBinding>());
                list.RemoveAll(x=>seen.Contains(x.image));loader.images=list.ToArray();
            }
        }
        static void Bind(Image image,int type,HashSet<Image> seen,bool dynamic=false)
        {
            if(!image)throw new Exception("Missing currency Image");
            if(!seen.Add(image))return;
            image.sprite=null;image.type=Image.Type.Simple;image.preserveAspect=true;image.color=Color.white;image.raycastTarget=false;
            if(type>1||dynamic)
            {
                var view=image.GetComponent<RecoveredCurrencyArtwork>();if(!view)view=image.gameObject.AddComponent<RecoveredCurrencyArtwork>();view.single=image;
                if(!view.smallPile)view.smallPile=(GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(Folder+"CurrencySmallPile.prefab"),image.transform);
                if(!view.largePile)view.largePile=(GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(Folder+"CurrencyLargePile.prefab"),image.transform);
                view.smallNotes=view.smallPile.GetComponentsInChildren<Image>(true);view.largeNotes=view.largePile.GetComponentsInChildren<Image>(true);
                view.smallPile.SetActive(!dynamic&&type==2);view.largePile.SetActive(!dynamic&&type==3);image.enabled=dynamic||type==1;
            }
            string p=image.name;for(var t=image.transform.parent;t;t=t.parent)p=t.name+"/"+p;
            audit.Add("type="+type+(dynamic?" (1/3 dynamic)":"")+" | "+p);
        }
    }
}
