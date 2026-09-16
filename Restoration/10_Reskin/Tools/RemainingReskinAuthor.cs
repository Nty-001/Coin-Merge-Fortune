using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
namespace CoinMerge.Recovery.Editor {
public static class RemainingReskinAuthor {
 static readonly Color Ink=new Color(.02f,.25f,.7f,1);
 static readonly Dictionary<Image,MenuImageBinding> deferred=new Dictionary<Image,MenuImageBinding>();
 static readonly List<string> log=new List<string>();
 static Sprite Sprite(string path)=>AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Resources/"+path+".png");
 static void Set(Image i,string name,bool sliced=false) {
  string path=name.Contains("/")?name:"ReskinRemaining/"+name;
  if(!Sprite(path))throw new Exception("Missing sprite "+path);
  if(deferred.TryGetValue(i,out var b))b.resourcePath=path;else i.sprite=Sprite(path);
  i.color=Color.white;i.preserveAspect=false;
  if(i.type!=Image.Type.Filled)i.type=sliced?Image.Type.Sliced:Image.Type.Simple;
  log.Add(ReskinAudit.PathOf(i.transform)+" => "+path);
 }
 static void Collect(GameObject root){deferred.Clear();foreach(var art in root.GetComponentsInChildren<RecoveredMenuArt>(true))foreach(var b in art.images)deferred[b.image]=b;}
 public static void Run() {
  AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
  foreach(var f in Directory.GetFiles("Assets/Resources/ReskinRemaining","*.png")){
   var t=(TextureImporter)AssetImporter.GetAtPath(f);t.textureType=TextureImporterType.Sprite;t.spriteImportMode=SpriteImportMode.Single;t.alphaSource=TextureImporterAlphaSource.FromInput;t.alphaIsTransparency=true;
   t.mipmapEnabled=false;t.wrapMode=TextureWrapMode.Clamp;t.filterMode=FilterMode.Bilinear;t.textureCompression=TextureImporterCompression.Uncompressed;t.npotScale=TextureImporterNPOTScale.None;t.spritePixelsPerUnit=100;t.maxTextureSize=2048;
   if(Path.GetFileNameWithoutExtension(f)=="Input")t.spriteBorder=new Vector4(30,30,30,30);
   var s=new TextureImporterSettings();t.ReadTextureSettings(s);s.spriteMeshType=SpriteMeshType.FullRect;t.SetTextureSettings(s);t.SaveAndReimport();
  }
  const string path="Assets/Prefabs/Runtime/RecoveredMain.prefab";
  var root=PrefabUtility.LoadPrefabContents(path);try{Apply(root);PrefabUtility.SaveAsPrefabAsset(root,path);}finally{PrefabUtility.UnloadPrefabContents(root);}
  var scene=EditorSceneManager.OpenScene("Assets/Scenes/RecoveredMain.unity");foreach(var r in scene.GetRootGameObjects())if(r.GetComponent<RecoveredGameSession>())Apply(r);EditorSceneManager.SaveScene(scene);
  Loading();AssetDatabase.SaveAssets();File.WriteAllLines("../07_Verification/remaining_reskin_map.txt",log);Debug.Log("REMAINING_RESKIN_AUTHORED "+log.Count);
 }
 static void Apply(GameObject root){
  Collect(root);
  foreach(var i in root.GetComponentsInChildren<Image>(true)){
   string p=ReskinAudit.PathOf(i.transform);var n=i.GetComponent<RecoveredNode>();int id=n?n.sourceObjectId:-1;
   if(p.Contains("/GameRealWDDialog/")){
    if(p.Contains("/GameWithdrawItem/")){if(id==2)Set(i,"ChoiceSelected");else if(id==3||id==4)Set(i,"Input",true);}
    else if(id==23)Set(i,"Reskin/Sky");else if(id==33||id==49||id==2)Set(i,"Reskin/Header");else if(id==34)Set(i,"Reskin/UI6");
    else if(id==11)Set(i,"CoinHeader");else if(id==36||id==39){Set(i,"Reskin/Coin2000");i.rectTransform.sizeDelta=new Vector2(120,124);}else if(id==5)Set(i,"CoinChoices");else if(id==8)Set(i,"CoinCondition");
    else if(id==21)Set(i,"Reskin/ProgressTrack");else if(id==50)Set(i,"Reskin/ProgressFill");else if(id==57)Set(i,"Reskin/UI0");else if(id==58)Set(i,"Disabled");
   } else if(p.Contains("/GameRealWDAccount")){
    if(p.EndsWith("/titleicon"))Set(i,"AccountCrest");
    else if(p.EndsWith("/content/bg"))Set(i,"AccountUS");
    else if(p.EndsWith("/bgNode/bg"))Set(i,p.Contains("AccountBR/")?"AccountBR":"AccountID");
    else if(p.EndsWith("/btnClose"))Set(i,"Reskin/UI7");else if(p.EndsWith("/BACKGROUND_SPRITE"))Set(i,"Input",true);else if(p.EndsWith("/conform"))Set(i,"Disabled");
   } else if(p.Contains("/GameRealTXYZ/")){
    if(id==2)Set(i,"Verify");else if(id==15)Set(i,"Reskin/CashCoins");else if(id==20)Set(i,"Reskin/UI7");else if(id==13)Set(i,"Reskin/UI0");else if(id==7)Set(i,"Input",true);
   } else if(p.Contains("/GameRealWDTXTips/")){
    if(id==2)Set(i,"NextCondition");else if(id==7)Set(i,"Reskin/UI7");else if(id==14)Set(i,"Input",true);else if(id==8)Set(i,"Reskin/UI0");else if(id==3)Set(i,"Reskin/ProgressTrack");else if(id==21)Set(i,"Reskin/ProgressFill");else if(id==11)Set(i,"CashStack");
   } else if(p.Contains("/GameRealWDActiveTips/")){
    if(id==2)Set(i,"Limit");else if(id==11)Set(i,"Reskin/UI7");else if(id==6)Set(i,"People");else if(id==3)Set(i,"Reskin/UI0");
   } else if(p.Contains("/PrivacyPolicyView/")){
    if(id==4)Set(i,"Policy");else if(id==25)Set(i,"Reskin/UI7");
   } else if(p.Contains("/ScoreDialog/")){
    if(id==3)Set(i,"Rating");else if(id==12)Set(i,"StarCrest");else if(id==13)Set(i,"Reskin/UI7");else if(id==7)Set(i,"Reskin/UI0");
   } else if(p.Contains("/RewardDialog/")){
    if(id==32)Set(i,"RewardLarge");else if(id==39||id==44)Set(i,"RewardSmall");else if(id==48)Set(i,"RewardGuide");else if(id==45)Set(i,"RewardTitle");else if(id==36)Set(i,"Reskin/Coin2000");
    else if(id==16||id==11)Set(i,"Reskin/UI0");else if(id==30)Set(i,"Hand");
   } else if(p.Contains("/LuckyDrawRewardDialog/")){
    if(id==2||id==10)Set(i,"WheelReward");else if(id==11)Set(i,"Reskin/Coin2000");else if(id==3)Set(i,"Reskin/UI0");
   } else if(p.Contains("/FailDialog/")){
    if(id==19)Set(i,"Fail");else if(id==11)Set(i,"RewardTitle");else if(id==3)Set(i,"Inset");else if(id==32)Set(i,"Reskin/Coin2000");else if(id==7||id==6)Set(i,"Reskin/UI0");else if(id==35||id==42)Set(i,"Video");else if(id==37)Set(i,"Reskin/UI7");
   } else if(p.Contains("/GuideDialog/")){
    if(id==8||id==32)Set(i,"GuideWide");else if(id==27||id==2)Set(i,"GuideTall");else if(id==28||id==33||id==36||id==14)Set(i,"Reskin/Coin2000");
    else if(id==15||id==16||id==20||id==21)Set(i,"Hand");else if(id==23||id==24||id==31||id==39)Set(i,"Arrow");else if(id==6)Set(i,"Reskin/Header");else if(id==18)Set(i,"Reskin/UI0");
   } else if(p.Contains("/Toast/")&&id==2)Set(i,"Reskin/Notice");
  }
  var session=root.GetComponent<RecoveredGameSession>();var menus=session.menus;
  foreach(var f in menus.forms){f.enabledSprite="Reskin/UI0";f.disabledSprite="ReskinRemaining/Disabled";f.enabledOutline=new Color(0,.4f,.1f);f.disabledOutline=new Color(.2f,.35f,.5f);}
  session.rating.emptyStar=Sprite("ReskinRemaining/StarEmpty");session.rating.filledStar=Sprite("ReskinRemaining/StarGold");foreach(var b in session.rating.stars)b.image.sprite=session.rating.emptyStar;
  foreach(var t in root.GetComponentsInChildren<Text>(true)){
   string p=ReskinAudit.PathOf(t.transform);
   if(!(p.Contains("/GameReal")||p.Contains("/GuideDialog/")||p.Contains("/ScoreDialog/")||p.Contains("/PrivacyPolicyView/")||p.Contains("/FailDialog/")||p.Contains("/RewardDialog/")||p.Contains("/LuckyDrawRewardDialog/")))continue;
   bool amount=p.Contains("moneyTxt")||p.Contains("coinLabel")||p.Contains("/plusMoneyTxt")||p.Contains("numberTxt")||p.Contains("scoreTxt1");
   bool title=t.name=="title"||t.name=="titleLabel"||t.name=="titleLabel1"||t.name=="titleTxt"||p.Contains("/titbg/");
   bool button=t.GetComponentInParent<Button>()!=null||t.name=="conformLabel"||t.name=="btnLabel";
   if(!amount)t.color=title||button?Color.white:Ink;
   var outline=t.GetComponent<Outline>();if(outline){outline.effectColor=title?new Color(.02f,.2f,.65f):button?new Color(.01f,.28f,.1f):new Color(1,1,1,.7f);if(!title&&!button&&!amount)outline.enabled=false;}
   if(t.name.StartsWith("coinLabel")){t.color=Ink;if(outline)outline.enabled=false;}
  }
  // Keep long translated titles inside their existing RectTransforms.
  foreach(var page in menus.pages)foreach(var t in page.GetComponentsInChildren<Text>(true))if(t.name=="title"||t.name=="titleLabel"){t.resizeTextForBestFit=true;t.resizeTextMinSize=22;t.resizeTextMaxSize=t.fontSize;t.horizontalOverflow=HorizontalWrapMode.Wrap;}
  foreach(var text in new[]{session.rewardView.highestAmount,session.rewardView.normalAmount,session.rewardView.guideAmount,session.rewardView.doubleAmount,session.wheelRewardView.amountLabel}){text.color=Color.white;var o=text.GetComponent<Outline>();if(!o)o=text.gameObject.AddComponent<RecoveredRoundOutline>();o.enabled=true;o.effectColor=Ink;o.effectDistance=new Vector2(2.5f,2.5f);}
  foreach(var glow in session.rewardView.glows){var i=glow.GetComponent<Image>();if(i)i.color=new Color(1,1,1,.3f);}
  foreach(var text in session.failView.revive.GetComponentsInChildren<Text>(true))text.color=Color.white;
  foreach(var row in menus.coinRows)foreach(var text in row.selected.GetComponentsInChildren<Text>(true))text.color=Color.white;
  session.failView.scoreText.color=new Color(1,.65f,.02f);session.menus.coinCount.color=Ink;
  AuthorRestart(session);
 }
 static void AuthorRestart(RecoveredGameSession s){
  var view=s.failView;var parent=view.revive.transform.parent;
  // Reuse the original bound Restart Button from the legacy visual group.
  var surface=parent.Find("ReskinRestartSurface");Image img;
  if(surface)img=surface.GetComponent<Image>();else{var go=new GameObject("ReskinRestartSurface",typeof(RectTransform),typeof(CanvasRenderer),typeof(Image));go.transform.SetParent(parent,false);img=go.GetComponent<Image>();}
  img.sprite=Sprite("ReskinRemaining/Secondary");img.raycastTarget=false;img.rectTransform.anchorMin=img.rectTransform.anchorMax=new Vector2(.5f,.5f);img.rectTransform.pivot=new Vector2(.5f,.5f);img.rectTransform.anchoredPosition=new Vector2(0,-925);img.rectTransform.sizeDelta=new Vector2(350,85);
  var rect=(RectTransform)view.restart.transform;rect.SetParent(img.transform,false);rect.anchorMin=Vector2.zero;rect.anchorMax=Vector2.one;rect.offsetMin=rect.offsetMax=Vector2.zero;rect.localScale=Vector3.one;rect.gameObject.SetActive(true);
  var label=rect.GetComponent<Text>();label.color=Color.white;label.fontSize=40;label.alignment=TextAnchor.MiddleCenter;label.raycastTarget=true;view.restart.targetGraphic=label;
  foreach(var n in parent.GetComponentsInChildren<RecoveredNode>(true))if(n.sourceObjectId==19){var r=(RectTransform)n.transform;r.sizeDelta=new Vector2(662,930);r.anchoredPosition=new Vector2(0,-555);}
 }
 static void Loading(){
  const string path="Assets/Resources/Startup/RewardedLoading.prefab";var root=PrefabUtility.LoadPrefabContents(path);
  try{
   Collect(root);
   foreach(var i in root.GetComponentsInChildren<Image>(true)){
    var n=i.GetComponent<RecoveredNode>();if(!n)continue;
    if(n.sourceObjectId==3)Set(i,"Loading");else if(n.sourceObjectId==8)i.enabled=false;else if(n.sourceObjectId==2)Set(i,"Reskin/ProgressTrack");else if(n.sourceObjectId==9)Set(i,"Reskin/ProgressFill");
   }
   foreach(var t in root.GetComponentsInChildren<Text>(true)){t.color=Color.white;var o=t.GetComponent<Outline>();if(o)o.effectColor=Ink;}
   PrefabUtility.SaveAsPrefabAsset(root,path);
  }finally{PrefabUtility.UnloadPrefabContents(root);}
 }
}}
