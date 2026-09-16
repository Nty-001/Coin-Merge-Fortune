using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
namespace CoinMerge.Recovery.Editor {
public static class CoinWithdrawAuthor {
 static readonly Color Blue=new Color(.015f,.20f,.76f),Ink=new Color(.015f,.18f,.66f),Gold=new Color(1,.85f,.15f);
 static Font bold,body;
 static readonly Dictionary<Image,MenuImageBinding> bindings=new Dictionary<Image,MenuImageBinding>();
 static Sprite S(string name)=>AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Resources/"+(name.Contains("/")?name:"VisualRepair/"+name)+".png");
 static void Collect(GameObject root){bindings.Clear();foreach(var a in root.GetComponentsInChildren<RecoveredMenuArt>(true))foreach(var b in a.images)bindings[b.image]=b;}
 static void Art(Image i,string name,bool slice=false){if(!i)return;string path=name.Contains("/")?name:"VisualRepair/"+name;if(bindings.TryGetValue(i,out var b))b.resourcePath=path;else i.sprite=S(name);i.color=Color.white;i.type=slice?Image.Type.Sliced:Image.Type.Simple;i.preserveAspect=false;i.pixelsPerUnitMultiplier=.32f;i.enabled=true;}
 static RectTransform N(GameObject root,int id){foreach(var n in root.GetComponentsInChildren<RecoveredNode>(true))if(n.sourceObjectId==id)return (RectTransform)n.transform;throw new Exception(root.name+" missing "+id);}
 static RectTransform Child(Transform parent,string name){var t=parent.Find(name);if(t)return (RectTransform)t;var g=new GameObject(name,typeof(RectTransform));g.transform.SetParent(parent,false);return (RectTransform)g.transform;}
 static RectTransform Surface(GameObject page,Camera camera,bool full){var parent=page.transform.Find("content")??page.transform;var r=Child(parent,"ApprovedDesign");r.anchorMin=r.anchorMax=r.pivot=new Vector2(.5f,.5f);r.anchoredPosition=Vector2.zero;r.sizeDelta=new Vector2(941,1672);r.localScale=Vector3.one*(750f/941);var fit=r.GetComponent<ApprovedViewportLayout>()??r.gameObject.AddComponent<ApprovedViewportLayout>();fit.targetCamera=camera;fit.surface=r;fit.fullPage=full;return r;}
 // Pixel rectangles measured in the approved 941 x 1672 reference.
 static void At(RectTransform r,RectTransform parent,float x,float y,float w,float h,bool responsive=false){r.SetParent(parent,false);r.anchorMin=r.anchorMax=responsive?new Vector2(.5f,1-(y+h*.5f)/1672):new Vector2(.5f,.5f);r.pivot=new Vector2(.5f,.5f);r.localScale=Vector3.one;r.localRotation=Quaternion.identity;r.sizeDelta=new Vector2(w,h);r.anchoredPosition=new Vector2(x+w*.5f-470.5f,responsive?0:836-y-h*.5f);}
 static void Local(RectTransform r,RectTransform parent,float x,float y,float w,float h){r.SetParent(parent,false);r.anchorMin=r.anchorMax=r.pivot=new Vector2(.5f,.5f);r.localScale=Vector3.one;r.localRotation=Quaternion.identity;r.sizeDelta=new Vector2(w,h);r.anchoredPosition=new Vector2(x,y);}
 static void TextStyle(Text t,int size,Color color,float stroke=0,Color? outline=null){if(!t)return;t.font=bold;t.fontStyle=FontStyle.Normal;t.fontSize=size;t.resizeTextForBestFit=true;t.resizeTextMinSize=Mathf.Max(15,(int)(size*.65f));t.resizeTextMaxSize=size;t.color=color;t.alignment=TextAnchor.MiddleCenter;t.horizontalOverflow=HorizontalWrapMode.Wrap;t.verticalOverflow=VerticalWrapMode.Truncate;t.lineSpacing=.9f;var o=t.GetComponent<Outline>();if(stroke>0){if(!o)o=t.gameObject.AddComponent<RecoveredRoundOutline>();o.enabled=true;o.effectColor=outline??Blue;o.effectDistance=Vector2.one*stroke;o.useGraphicAlpha=true;}else if(o)o.enabled=false;foreach(var sh in t.GetComponents<Shadow>())if(!(sh is Outline))sh.enabled=false;var depth=t.GetComponent<ApprovedTextDepth>();if(stroke>0&&(color==Color.white||color==Gold||color==Color.red)){if(!depth)depth=t.gameObject.AddComponent<ApprovedTextDepth>();depth.enabled=true;depth.top=color==Color.white?Color.white:color==Gold?new Color(1,1,.62f):new Color(1,.24f,.18f);depth.bottom=color==Color.white?new Color(.87f,.96f,1):color==Gold?new Color(1,.58f,.015f):new Color(.8f,0,0);depth.shadow=(outline??Blue)*new Color(1,1,1,.5f);depth.depth=Mathf.Max(2,stroke*.6f);}else if(depth)depth.enabled=false;}
 static Image NewImage(RectTransform p,string name,string sprite){var r=Child(p,name);var i=r.GetComponent<Image>()??r.gameObject.AddComponent<Image>();Art(i,sprite);i.raycastTarget=false;return i;}
 static void Box(GameObject p,int id,RectTransform surface,float x,float y,float w,float h,string art=null,bool responsive=false){var r=N(p,id);At(r,surface,x,y,w,h,responsive);if(art!=null)Art(r.GetComponent<Image>(),art);}
 static void Letter(Text text,string value,string asset,float width,float height){var image=NewImage(text.rectTransform,"ApprovedLettering",asset);Local(image.rectTransform,text.rectTransform,0,0,width,height);var match=text.GetComponent<ApprovedLabelArtwork>()??text.gameObject.AddComponent<ApprovedLabelArtwork>();match.label=text;match.artwork=image;match.matchingText=value;}
 public static void Run(){
  AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
  foreach(var f in Directory.GetFiles("Assets/Resources/VisualRepair","*.png")){var t=(TextureImporter)AssetImporter.GetAtPath(f);t.textureType=TextureImporterType.Sprite;t.spriteImportMode=SpriteImportMode.Single;t.spritePixelsPerUnit=100;t.mipmapEnabled=false;t.alphaIsTransparency=true;t.textureCompression=TextureImporterCompression.Uncompressed;t.npotScale=TextureImporterNPOTScale.None;t.filterMode=FilterMode.Bilinear;t.wrapMode=TextureWrapMode.Clamp;t.maxTextureSize=2048;string n=Path.GetFileNameWithoutExtension(f);t.spriteBorder=(n=="Green"||n=="Disabled"||n=="LoadingFill")?new Vector4(t.CoinCapBorder(),0,t.CoinCapBorder(),0):Vector4.zero;t.SaveAndReimport();}
  bold=AssetDatabase.LoadAssetAtPath<Font>("Assets/Resources/VisualRepair/Nunito-Black.ttf");body=AssetDatabase.LoadAssetAtPath<Font>("Assets/Resources/VisualRepair/Nunito-ExtraBold.ttf");
  const string path="Assets/Prefabs/Runtime/RecoveredMain.prefab";var root=PrefabUtility.LoadPrefabContents(path);try{Collect(root);CoinPage(root.GetComponent<RecoveredGameSession>());PrefabUtility.SaveAsPrefabAsset(root,path);}finally{PrefabUtility.UnloadPrefabContents(root);}
  var scene=EditorSceneManager.OpenScene("Assets/Scenes/RecoveredMain.unity");foreach(var r in scene.GetRootGameObjects())if(r.GetComponent<RecoveredGameSession>()){Collect(r);CoinPage(r.GetComponent<RecoveredGameSession>());}EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();Debug.Log("COIN_ONLY_AUTHORED");
 }
 static void CoinPage(RecoveredGameSession s){var m=s.menus;var p=m.pages[4];var d=Surface(p,s.worldCamera,true);var playfield=s.GetComponentInChildren<RecoveredPlayfieldLayout>();if(playfield&&playfield.gmButton)playfield.gmButton.gameObject.SetActive(false);
  var bg=N(p,23);At(bg,d,0,0,941,1672);Art(bg.GetComponent<Image>(),"Sky");bg.anchorMin=Vector2.zero;bg.anchorMax=Vector2.one;bg.offsetMin=bg.offsetMax=Vector2.zero;bg.SetAsFirstSibling();
  Box(p,33,d,14,23,915,147,"Header",true);Box(p,24,d,33,46,99,99,null,true);Local(N(p,34),N(p,24),0,0,99,99);Art(N(p,34).GetComponent<Image>(),"Back");
  Box(p,10,d,230,36,482,112,null,true);TextStyle(N(p,10).GetComponent<Text>(),81,Color.white,6);
  Box(p,11,d,18,211,910,305,"CoinCard",true);
  var card=N(p,11);Local(N(p,15),card,0,0,910,305);
  Local(N(p,39),N(p,15),-195,0,265,265);Art(N(p,39).GetComponent<Image>(),"Coin2000");
  Local(N(p,16),N(p,15),92,17,380,260);TextStyle(m.coinCount,165,Color.white,6);
  var counter=m.coinCount.GetComponent<ApprovedCounterDigits>()??m.coinCount.gameObject.AddComponent<ApprovedCounterDigits>();counter.source=m.coinCount;counter.slots=new Image[10];counter.digits=new Sprite[10];for(int i=0;i<10;i++){counter.digits[i]=S("Digit"+i);counter.slots[i]=NewImage(m.coinCount.rectTransform,"ApprovedDigit"+i,"Digit"+i);Local(counter.slots[i].rectTransform,m.coinCount.rectTransform,0,0,100,135);}
  Local(N(p,17),N(p,15),97,-82,330,72);TextStyle(N(p,17).GetComponent<Text>(),52,Color.white,4);
  Box(p,5,d,19,534,904,624,"ChoicesPanel",true);var middle=N(p,5);
  Local(N(p,6),middle,0,246,820,100);TextStyle(N(p,6).GetComponent<Text>(),74,Color.white,5);
  // Keep the existing six native Buttons, but author a complete 2 x 3 grid.
  for(int n=0;n<m.coinRows.Length;n++){var row=m.coinRows[n];var r=(RectTransform)row.root.transform;Local(r,middle,n%2==0?-211:215,108-(n/2)*155,408,142);
   foreach(var image in row.root.GetComponentsInChildren<Image>(true))if(image.name=="ApprovedCheck"&&image.transform.parent!=row.selected.transform)UnityEngine.Object.DestroyImmediate(image.gameObject);
   foreach(var im in row.root.GetComponentsInChildren<Image>(true)){var node=im.GetComponent<RecoveredNode>();if(!node)continue;Local(im.rectTransform,r,0,0,408,142);Art(im,im.gameObject==row.selected?"ChoiceGreen":"ChoiceBlue");}
   foreach(var t in row.amounts){Local(t.rectTransform,(RectTransform)t.transform.parent,0,3,382,112);bool selected=t.transform.IsChildOf(row.selected.transform);TextStyle(t,80,selected?Color.white:n>=4?Color.red:Gold,selected?4:3,selected?new Color(0,.3f,.05f):n>=4?Color.white:new Color(.6f,.24f,.005f));}
   var check=NewImage((RectTransform)row.selected.transform,"ApprovedCheck","Check");Local(check.rectTransform,(RectTransform)row.selected.transform,167,-41,67,67);
   row.selected.transform.SetAsLastSibling();
  }
  N(p,18).gameObject.SetActive(false); // The old oversized scrolling container is empty.
  Box(p,8,d,19,1175,903,295,"ConditionCard",true);var condition=N(p,8);
  Local(N(p,53),condition,0,78,855,100);TextStyle(N(p,53).GetComponent<Text>(),61,Color.white,5);
  Local(N(p,21),condition,0,-9,823,66);Art(N(p,21).GetComponent<Image>(),"CoinTrack");
  Local(m.coinFill.rectTransform,N(p,21),0,0,800,47);Art(m.coinFill,"LoadingFill",true);m.coinFill.pixelsPerUnitMultiplier=.32f*87/47;m.coinFill.rectTransform.pivot=new Vector2(0,.5f);m.coinFill.rectTransform.anchoredPosition=new Vector2(-400,0);var progress=m.coinFill.GetComponent<ApprovedProgressFill>()??m.coinFill.gameObject.AddComponent<ApprovedProgressFill>();progress.fill=m.coinFill;progress.fullWidth=800;
  if(!m.coinPercent){var r=Child(N(p,21),"ApprovedProgressCount");m.coinPercent=r.gameObject.AddComponent<Text>();}
  Local(m.coinPercent.rectTransform,N(p,21),0,0,440,63);TextStyle(m.coinPercent,39,Color.white,3);m.coinPercent.raycastTarget=false;
  Local(m.coinHint.rectTransform,condition,0,-89,847,65);TextStyle(m.coinHint,33,Blue,1,Color.white);m.coinHint.font=body;
  Box(p,9,d,193,1493,557,147,null,true);Box(p,22,d,193,1493,557,147,null,true);
  Local(N(p,57),N(p,9),0,0,557,147);Art(N(p,57).GetComponent<Image>(),"Green",true);
  Local(N(p,58),N(p,22),0,0,557,147);Art(N(p,58).GetComponent<Image>(),"Disabled",true);
  Local(N(p,28),N(p,9),0,5,510,115);Local(N(p,29),N(p,22),0,5,510,115);TextStyle(N(p,28).GetComponent<Text>(),77,Color.white,5,new Color(0,.3f,.07f));TextStyle(N(p,29).GetComponent<Text>(),77,Color.white,5,new Color(.15f,.28f,.43f));
  N(p,2).GetComponent<Image>().enabled=false;N(p,49).GetComponent<Image>().enabled=false;
  Letter(N(p,10).GetComponent<Text>(),"Withdraw","LabelWithdraw",382,103);
  Letter(N(p,17).GetComponent<Text>(),"My Coins","LabelMyCoins",215,66);
  Letter(N(p,6).GetComponent<Text>(),"Select amount","LabelSelect",465,79);
  Letter(N(p,53).GetComponent<Text>(),"Withdrawal Conditions","LabelConditions",656,84);
  Letter(N(p,29).GetComponent<Text>(),"Withdraw","LabelWithdrawDisabled",335,92);
  string[] values={"$500","$800","$1,000","$2,000","$3,000","$5,000"};string[] assets={"Amount500","Amount800","Amount1000","Amount2000","Amount3000","Amount5000"};float[] widths={211,194,250,259,260,256};
  for(int n=0;n<m.coinRows.Length;n++)foreach(var label in m.coinRows[n].amounts){bool selected=label.transform.IsChildOf(m.coinRows[n].selected.transform);if(selected==(n==0))Letter(label,values[n],assets[n],widths[n],n==0?95:n==1?99:96);}
 }
 static int CoinCapBorder(this TextureImporter t){t.GetSourceTextureWidthAndHeight(out int w,out int h);return Mathf.Min(h/2,w/2);}
}}
