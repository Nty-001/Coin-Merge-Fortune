using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
namespace CoinMerge.Recovery.Editor
{
    public static class LifecycleParityBuilder
    {
        const string ResourcesRoot="Assets/Resources/Lifecycle/";
        static LifecycleVisualConfig config;
        static Image moneyPrefab;
        static readonly Dictionary<string,string> copied=new Dictionary<string,string>();
        [MenuItem("Coin Merge/Restore lifecycle parity")]
        public static void Run()
        {
            Directory.CreateDirectory(ResourcesRoot+"Art");Directory.CreateDirectory(ResourcesRoot+"Audio");AssetDatabase.Refresh();
            string path="Assets/Config/Runtime/LifecycleVisuals.asset";config=AssetDatabase.LoadAssetAtPath<LifecycleVisualConfig>(path);
            if(!config){config=ScriptableObject.CreateInstance<LifecycleVisualConfig>();AssetDatabase.CreateAsset(config,path);}
            foreach(var name in new[]{"fly_red_bag","redbag_show","reward","guide_redbag_show","collect","fail"})
            {string target=ResourcesRoot+"Audio/"+name+".mp3";if(!File.Exists(target)&&!AssetDatabase.CopyAsset("Assets/Art/HotUpdate/Audio/Audio/"+name+".mp3",target))throw new Exception("Missing original sound "+name);}
            var template=new GameObject("CashFlight",typeof(RectTransform),typeof(CanvasRenderer),typeof(Image));var img=template.GetComponent<Image>();
            img.raycastTarget=false;img.rectTransform.sizeDelta=new Vector2(162,130);template.SetActive(false);
            moneyPrefab=PrefabUtility.SaveAsPrefabAsset(template,"Assets/Prefabs/Runtime/CashFlight.prefab").GetComponent<Image>();UnityEngine.Object.DestroyImmediate(template);
            path="Assets/Prefabs/Runtime/RecoveredMain.prefab";var root=PrefabUtility.LoadPrefabContents(path);
            try{Configure(root);PrefabUtility.SaveAsPrefabAsset(root,path);}finally{PrefabUtility.UnloadPrefabContents(root);}
            var scene=EditorSceneManager.OpenScene("Assets/Scenes/RecoveredMain.unity");foreach(var rootObject in scene.GetRootGameObjects())if(rootObject.GetComponent<RecoveredGameSession>())Configure(rootObject);
            EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();Debug.Log("LIFECYCLE_PARITY_AUTHORED");
        }
        static T Get<T>(GameObject node) where T:Component{var value=node.GetComponent<T>();return value?value:node.AddComponent<T>();}
        static RectTransform Layer(Transform parent)
        {var obj=new GameObject("LifecycleFeedback",typeof(RectTransform),typeof(Canvas));obj.transform.SetParent(parent,false);var rect=(RectTransform)obj.transform;rect.anchorMin=Vector2.zero;rect.anchorMax=Vector2.one;rect.offsetMin=rect.offsetMax=Vector2.zero;var canvas=obj.GetComponent<Canvas>();canvas.overrideSorting=true;canvas.sortingOrder=999;return rect;}
        static Button Button(GameObject node)
        {var graphic=node.GetComponent<Graphic>();if(!graphic||!graphic.enabled)throw new Exception("Missing visible Button graphic: "+node.name);graphic.raycastTarget=true;var b=Get<Button>(node);b.targetGraphic=graphic;b.transition=Selectable.Transition.None;b.navigation=new Navigation{mode=Navigation.Mode.None};return b;}
        static void Popup(GameObject root,Transform content){Get<RecoveredMenuPopup>(root).content=content;}
        static void Configure(GameObject root)
        {
            var session=root.GetComponent<RecoveredGameSession>();var uuid=session.moneyText.GetComponent<RecoveredNode>().sourceUuid;
            var n=new Dictionary<int,GameObject>();foreach(var node in root.GetComponentsInChildren<RecoveredNode>(true))if(node.sourceUuid==uuid)n.Add(node.sourceObjectId,node.gameObject);
            if(session.lifecycle)UnityEngine.Object.DestroyImmediate(session.lifecycle.gameObject);
            var layer=Layer(n[1].transform);var f=layer.gameObject.AddComponent<RecoveredLifecycleFeedback>();session.lifecycle=f;f.session=session;f.config=config;session.board.visualConfig=config;
            f.cashLayer=layer;f.moneyPrefab=moneyPrefab;f.moneyTarget=(RectTransform)n[24].transform;f.highestTarget=(RectTransform)n[9].transform;
            f.plus=(RectTransform)n[18].transform;f.plusText=n[44].GetComponent<Text>();f.plusAlpha=Get<CanvasGroup>(n[18]);f.plusAlpha.blocksRaycasts=false;f.plus.gameObject.SetActive(false);
            f.highestEffect=n[61].GetComponentInChildren<NativeSkeletonPlayer>(true);if(!f.highestEffect)throw new Exception("Missing original GuangH_TX effect");
            // Keep the original source holder active; only its bound animation instance is controlled.
            n[61].SetActive(true);f.highestEffect.gameObject.SetActive(false);f.highestEffect.playOnEnable=false;f.highestAlpha=Get<CanvasGroup>(f.highestEffect.gameObject);f.highestAlpha.blocksRaycasts=false;
            f.sound=layer.gameObject.AddComponent<AudioSource>();f.sound.playOnAwake=false;f.sound.spatialBlend=0;
            f.warning=n[12].GetComponent<Image>();f.warning.raycastTarget=false;f.warning.enabled=true;n[12].SetActive(false);f.gameArea=(RectTransform)n[3].transform;
            var layout=Get<RecoveredPlayfieldLayout>(root);layout.board=session.board;layout.worldCamera=session.worldCamera;layout.gameArea=f.gameArea;layout.bottom=(RectTransform)n[5].transform;layout.warning=f.warning.rectTransform;session.playfieldLayout=layout;
            layout.canvasRoot=(RectTransform)n[1].transform;layout.upper=(RectTransform)n[2].transform;layout.background=(RectTransform)n[22].transform;layout.gmButton=session.gm.open.targetGraphic.rectTransform;
            // GM owns a camera canvas: let its RectTransform anchors follow the canvas
            // resize instead of projecting a world position before CanvasScaler settles.
            var gmRect=layout.gmButton;gmRect.GetComponentInParent<CanvasScaler>().matchWidthOrHeight=0;
            gmRect.anchorMin=gmRect.anchorMax=new Vector2(1,0);gmRect.anchoredPosition=new Vector2(-20-gmRect.rect.width*(1-gmRect.pivot.x),215+gmRect.rect.height*gmRect.pivot.y);
            var r=NativeGameplayBuilder.Nodes(session.rewardView.gameObject);session.rewardView.session=session;
            session.rewardView.highestTitle=r[15].GetComponent<Text>();session.rewardView.doubleTitle=r[20].GetComponent<Text>();session.rewardView.normalTitle=r[23].GetComponent<Text>();session.rewardView.guideTitle=r[27].GetComponent<Text>();
            session.rewardView.glows=new[]{(RectTransform)r[13].transform,(RectTransform)r[18].transform,(RectTransform)r[22].transform,(RectTransform)r[25].transform};
            Popup(session.rewardView.gameObject,r[8].transform);
            var fail=NativeGameplayBuilder.Nodes(session.failView.gameObject);session.failView.session=session;Popup(session.failView.gameObject,fail[2].transform);
            // Both lottery windows also inherit BaseUI.pop in the recovered module.
            BindSourcePopup(session.wheelView.gameObject);BindSourcePopup(session.wheelRewardView.gameObject);
            if(session.rating)UnityEngine.Object.DestroyImmediate(session.rating.gameObject);
            var rating=NativeGameplayBuilder.Source("GameDialog/ScoreDialog",n[1].transform);rating.SetActive(false);var rect=(RectTransform)rating.transform;
            rect.anchorMin=Vector2.zero;rect.anchorMax=Vector2.one;rect.offsetMin=rect.offsetMax=Vector2.zero;
            var canvas=rating.AddComponent<Canvas>();canvas.overrideSorting=true;canvas.sortingOrder=300;rating.AddComponent<GraphicRaycaster>();
            var stars=NativeGameplayBuilder.Nodes(rating);foreach(var graphic in rating.GetComponentsInChildren<Graphic>(true))graphic.raycastTarget=false;
            foreach(var image in rating.GetComponentsInChildren<Image>(true))if(!image.sprite)image.enabled=false;
            stars[8].GetComponent<Image>().raycastTarget=true;
            var v=rating.AddComponent<RecoveredRatingView>();session.rating=v;v.session=session;v.close=Button(stars[13]);v.confirm=Button(stars[7]);
            v.title=stars[10].GetComponent<Text>();v.tips=stars[15].GetComponent<Text>();v.confirmLabel=stars[11].GetComponent<Text>();
            v.emptyStar=stars[17].GetComponent<Image>().sprite;v.filledStar=stars[22].GetComponent<Image>().sprite;v.stars=new Button[5];
            // One visible standard Button per star, with empty/filled sprites on that same graphic.
            for(int i=0;i<5;i++){stars[17+i].SetActive(false);var star=stars[22+i].GetComponent<Image>();star.sprite=v.emptyStar;star.color=Color.white;v.stars[i]=Button(stars[22+i]);}
            foreach(var outline in rating.GetComponentsInChildren<Outline>(true))
            {if(outline is RecoveredRoundOutline)continue;var go=outline.gameObject;var color=outline.effectColor;var distance=outline.effectDistance;bool enabled=outline.enabled;UnityEngine.Object.DestroyImmediate(outline);var round=go.AddComponent<RecoveredRoundOutline>();round.effectColor=color;round.effectDistance=distance;round.enabled=enabled;}
            Popup(rating,stars[5].transform);DeferArt(rating,v.stars); // Large background art is loaded only while the popup is open.
        }
        [Serializable] sealed class Ref{public int reference;}
        [Serializable] sealed class SourcePopup{public Ref contentNode;}
        static void BindSourcePopup(GameObject root)
        {var nodes=NativeGameplayBuilder.Nodes(root);foreach(var node in root.GetComponentsInChildren<RecoveredNode>(true))foreach(var c in node.originalComponents)if(c.rawJson.Contains("\"contentNode\"")){var p=JsonUtility.FromJson<SourcePopup>(c.rawJson.Replace("$ref","reference"));if(p.contentNode!=null&&nodes.TryGetValue(p.contentNode.reference,out var content))Popup(root,content.transform);}}
        static void DeferArt(GameObject root,Button[] stars)
        {
            var images=new List<MenuImageBinding>();foreach(var img in root.GetComponentsInChildren<Image>(true))
            {
                bool isStar=false;foreach(var b in stars)if(b.image==img)isStar=true;if(isStar||!img.sprite)continue;
                string source=AssetDatabase.GetAssetPath(img.sprite);if(!copied.TryGetValue(source,out var target)){string name=AssetDatabase.AssetPathToGUID(source);target="Lifecycle/Art/"+name;string destination="Assets/Resources/"+target+Path.GetExtension(source);if(!File.Exists(destination)&&!AssetDatabase.CopyAsset(source,destination))throw new Exception(source);copied.Add(source,target);}
                images.Add(new MenuImageBinding{image=img,resourcePath=target});img.sprite=null;
            }
            root.AddComponent<RecoveredMenuArt>().images=images.ToArray();
        }
    }
}
