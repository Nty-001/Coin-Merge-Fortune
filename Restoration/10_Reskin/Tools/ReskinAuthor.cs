using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
namespace CoinMerge.Recovery.Editor {
public static class ReskinAuthor {
 static readonly Color Ink=new Color(0.015f,.27f,.72f,1);
 static readonly List<string> changed=new List<string>();
 static readonly Dictionary<Image,MenuImageBinding> deferred=new Dictionary<Image,MenuImageBinding>();
 static void Set(Image img,string name,bool sliced=false) {
  string resource="Reskin/"+name;
  if(deferred.TryGetValue(img,out var b)){b.resourcePath=resource;img.sprite=null;}
  else img.sprite=AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Resources/"+resource+".png");
  img.color=Color.white;
  if(img.type!=Image.Type.Filled)img.type=sliced?Image.Type.Sliced:Image.Type.Simple;
  changed.Add(ReskinAudit.PathOf(img.transform)+" => "+resource);
 }
 static void Import() {
  AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
  foreach(var file in Directory.GetFiles("Assets/Resources/Reskin","*.png")) {
   var t=(TextureImporter)AssetImporter.GetAtPath(file);t.textureType=TextureImporterType.Sprite;t.spriteImportMode=SpriteImportMode.Single;
   t.alphaSource=TextureImporterAlphaSource.FromInput;t.alphaIsTransparency=true;t.mipmapEnabled=false;t.filterMode=FilterMode.Bilinear;
   t.wrapMode=TextureWrapMode.Clamp;t.textureCompression=TextureImporterCompression.Uncompressed;t.npotScale=TextureImporterNPOTScale.None;t.maxTextureSize=2048;
   t.spritePixelsPerUnit=100;
   if(Path.GetFileNameWithoutExtension(file)=="UI1")t.spriteBorder=new Vector4(35,30,35,30);
   var settings=new TextureImporterSettings();t.ReadTextureSettings(settings);settings.spriteMeshType=SpriteMeshType.FullRect;t.SetTextureSettings(settings);t.SaveAndReimport();
  }
  // Existing physics coin canvases, GUIDs and PPU are preserved by the PNG-only replacement.
  foreach(var f in Directory.GetFiles("Assets/Resources/Gameplay/Coins","*.png"))AssetDatabase.ImportAsset(f,ImportAssetOptions.ForceUpdate);
 }
 public static void Run() {
  Import();changed.Clear();const string path="Assets/Prefabs/Runtime/RecoveredMain.prefab";
  var root=PrefabUtility.LoadPrefabContents(path);try{Apply(root);PrefabUtility.SaveAsPrefabAsset(root,path);}finally{PrefabUtility.UnloadPrefabContents(root);}
  var scene=EditorSceneManager.OpenScene("Assets/Scenes/RecoveredMain.unity");foreach(var r in scene.GetRootGameObjects())if(r.GetComponent<RecoveredGameSession>())Apply(r);
  EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();File.WriteAllLines("../07_Verification/reskin_applied_map.txt",changed);Debug.Log("RESKIN_AUTHORED "+changed.Count);
 }
 static void Apply(GameObject root) {
  deferred.Clear();foreach(var art in root.GetComponentsInChildren<RecoveredMenuArt>(true))foreach(var b in art.images)deferred[b.image]=b;
  foreach(var img in root.GetComponentsInChildren<Image>(true)) {
   string p=ReskinAudit.PathOf(img.transform);var n=img.GetComponent<RecoveredNode>();int id=n?n.sourceObjectId:-1;
   bool setting=p.Contains("/SettingDialog/"),rules=p.Contains("/mergeRuleDialog/"),cash=p.Contains("/GameFakeWDDialog/"),wheel=p.Contains("/LuckDrawDialog/");
   if(setting) {
    if(id==2)Set(img,"SettingsPanel");else if(id==5||id==10)Set(img,"UI1",true);
    else if(id==19||id==22)Set(img,"UI8");else if(id==20||id==23)Set(img,"UI9");
    else if(id==25)Set(img,"UI4");else if(id==27)Set(img,"UI7");
   } else if(rules) {
    if(id==9)Set(img,"RuleCoins");else if(id==10)Set(img,"RulesPanel");else if(id==6)Set(img,"UI1",true);
    else if(id==16)Set(img,"RuleClose");else if(id==3)Set(img,"UI0");
   } else if(cash) {
    if(p.Contains("/GameFakeWDItem/")) {
     if(id==3)Set(img,"CashCardSelected");else if(id==5)Set(img,"CashCard");else if(id==6)Set(img,"UI13");else if(id==4)Set(img,"ProgressTrack");else if(id==8)Set(img,"ProgressFill");
    } else {
     if(id==10)Set(img,"Sky");else if(id==15||id==24)Set(img,"Header");else if(id==16)Set(img,"UI6");else if(id==19)Set(img,"CashCoins");else if(id==25)Set(img,"UI0");
    }
   } else if(wheel) {
    if(id==12)Set(img,"UI0");else if(id==31)Set(img,"Coin2000");
    else if(id>=16&&id<=23)Set(img,"Coin2000");
    else if(p.EndsWith("/unselect",StringComparison.Ordinal))Set(img,"UI1",true);
    else if(p.EndsWith("/select",StringComparison.Ordinal))Set(img,"Header");
   } else if(!p.Contains("Dialog/")&&!p.Contains("/VersionGM/")&&!p.Contains("PrivacyPolicyView")) {
    if(p.EndsWith("GameScene/bg",StringComparison.Ordinal))Set(img,"Sky");
    else if(p.Contains("/up/getMoneyTip")&&id==16)Set(img,"Notice");
    else if(p.EndsWith("/up/money",StringComparison.Ordinal))Set(img,"Header");
    else if(p.EndsWith("/up/bubble",StringComparison.Ordinal))Set(img,"UI1",true);
    else if(id==42||id==73)Set(img,"UI0");
    else if(id==52)Set(img,"UI4");else if(id==53)Set(img,"UI5");
    else if(p.EndsWith("/up/nextPreview",StringComparison.Ordinal))Set(img,"UI1",true);
    else if(p.EndsWith("/bottom/1",StringComparison.Ordinal))Set(img,"UI1",true);
    else if(p.EndsWith("/bottom/ProgressBar",StringComparison.Ordinal))Set(img,"ProgressTrack");
    else if(p.EndsWith("/bottom/ProgressBar/bar",StringComparison.Ordinal))Set(img,"ProgressFill");
   }
  }
  var s=root.GetComponent<RecoveredGameSession>();
  foreach(var t in s.menus.pages[1].GetComponentsInChildren<Text>(true)) {var n=t.GetComponent<RecoveredNode>();if(n&&(n.sourceObjectId==12||n.sourceObjectId==17))t.color=Ink;}
  // Preserve actual localized/dynamic text; only its visual color follows the approved blue theme.
  s.menus.fakeBalance.color=Color.white;
 }
}}
