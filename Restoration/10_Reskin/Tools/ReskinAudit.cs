using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
namespace CoinMerge.Recovery.Editor {
public static class ReskinAudit {
 [Serializable] public class Item { public string path,kind,asset,resource,text; public int node; public Vector2 size,pos; public Color color; }
 [Serializable] public class Report { public List<Item> items=new List<Item>(); }
 public static string PathOf(Transform t) { return t.parent?PathOf(t.parent)+"/"+t.name:t.name; }
 public static void Run() {
  var root=PrefabUtility.LoadPrefabContents("Assets/Prefabs/Runtime/RecoveredMain.prefab"); var report=new Report();
  try {
   var deferred=new Dictionary<Image,string>(); foreach(var art in root.GetComponentsInChildren<RecoveredMenuArt>(true))foreach(var b in art.images)deferred[b.image]=b.resourcePath;
   foreach(var g in root.GetComponentsInChildren<Graphic>(true)) {
    var n=g.GetComponent<RecoveredNode>();var i=new Item{path=PathOf(g.transform),kind=g.GetType().Name,node=n?n.sourceObjectId:-1,size=g.rectTransform.sizeDelta,pos=g.rectTransform.anchoredPosition,color=g.color};
    if(g is Image img){i.asset=img.sprite?AssetDatabase.GetAssetPath(img.sprite):"";if(deferred.TryGetValue(img,out var r))i.resource=r;}
    if(g is Text text)i.text=text.text;report.items.Add(i);
   }
   foreach(var sk in root.GetComponentsInChildren<NativeSkeletonPlayer>(true))report.items.Add(new Item{path=PathOf(sk.transform),kind="Skeleton",resource=sk.dataPath});
   Directory.CreateDirectory("../07_Verification");File.WriteAllText("../07_Verification/reskin_asset_map.json",JsonUtility.ToJson(report,true));
  }finally{PrefabUtility.UnloadPrefabContents(root);}
  Debug.Log("RESKIN_AUDIT_DONE");
 }
}}
