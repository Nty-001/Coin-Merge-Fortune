using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
namespace CoinMerge.Recovery.Editor
{
    public static class InteractionParityAuthor
    {
        static Font font;
        static RectTransform Rect(string name,Transform parent,Vector2 size,Vector2 position)
        {
            var go=new GameObject(name,typeof(RectTransform));go.layer=5;go.transform.SetParent(parent,false);
            var rect=(RectTransform)go.transform;rect.sizeDelta=size;rect.anchoredPosition=position;return rect;
        }
        static Text Label(string name,Transform parent,string value,int size,Vector2 position)
        {
            var r=Rect(name,parent,new Vector2(580,90),position);var label=r.gameObject.AddComponent<Text>();
            label.font=font;label.fontSize=size;label.alignment=TextAnchor.MiddleCenter;label.text=value;label.color=Color.white;label.raycastTarget=false;return label;
        }
        static Button Button(string name,Transform parent,string value,Vector2 position)
        {
            var rect=Rect(name,parent,new Vector2(500,100),position);var image=rect.gameObject.AddComponent<Image>();
            image.sprite=AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Resources/HomeReskin/GreenButton.png");
            var button=rect.gameObject.AddComponent<Button>();button.targetGraphic=image;
            Label("Label",rect,value,32,Vector2.zero);return button;
        }
        static void Apply(GameObject root,NativeSkeletonPlayer collision)
        {
            var s=root.GetComponent<RecoveredGameSession>();
            var feedback=root.GetComponent<RecoveredCoinFeedback>();if(!feedback)feedback=root.AddComponent<RecoveredCoinFeedback>();
            s.coinFeedback=feedback;feedback.session=s;feedback.collisionPrefab=collision;feedback.effectRoot=s.mergeFeedback.effectsRoot;
            if(!feedback.dropAudio){var sound=new GameObject("DropAudio",typeof(AudioSource));sound.transform.SetParent(root.transform,false);feedback.dropAudio=sound.GetComponent<AudioSource>();feedback.dropAudio.playOnAwake=false;feedback.dropAudio.spatialBlend=0;}
            if(!s.adPlayback)
            {
                var canvasGo=new GameObject("MockAdPlayback",typeof(RectTransform),typeof(Canvas),typeof(CanvasScaler),typeof(GraphicRaycaster),typeof(MockAdPlaybackView));
                canvasGo.layer=5;canvasGo.transform.SetParent(root.transform,false);
                var canvas=canvasGo.GetComponent<Canvas>();canvas.renderMode=RenderMode.ScreenSpaceOverlay;canvas.sortingOrder=4000;
                var scaler=canvasGo.GetComponent<CanvasScaler>();scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;scaler.referenceResolution=new Vector2(750,1624);scaler.matchWidthOrHeight=0;
                var view=canvasGo.GetComponent<MockAdPlaybackView>();s.adPlayback=view;view.session=s;
                var panel=Rect("PlaybackPanel",canvasGo.transform,Vector2.zero,Vector2.zero);panel.anchorMin=Vector2.zero;panel.anchorMax=Vector2.one;panel.offsetMin=panel.offsetMax=Vector2.zero;
                var mask=panel.gameObject.AddComponent<Image>();mask.color=new Color(.025f,.07f,.15f,.98f);mask.raycastTarget=true;view.panel=panel.gameObject;
                Label("Title",panel,"广告模拟",48,new Vector2(0,310));
                view.placementLabel=Label("Placement",panel,"激励广告 · 本地模拟",26,new Vector2(0,210));
                Label("Explanation",panel,"正在模拟广告播放，完成后可返回游戏",25,new Vector2(0,100));
                view.countdown=Label("Countdown",panel,"模拟播放中",36,new Vector2(0,-10));
                view.finish=Button("Finish",panel,"关闭广告并继续",new Vector2(0,-155));
                view.cancel=Button("Cancel",panel,"提前关闭",new Vector2(0,-295));
                view.finish.interactable=false;panel.gameObject.SetActive(false);
            }
            s.adPlayback.audioToPause=root.GetComponentsInChildren<AudioSource>(true);
        }
        public static void Author()
        {
            font=AssetDatabase.LoadAssetAtPath<Font>("Assets/Art/HotUpdate/Fonts/FZY4JW.ttf");
            Directory.CreateDirectory("Assets/Resources/CoinFeedback");
            File.Copy("Assets/Art/HotUpdate/Audio/Audio/sfx_bom.wav","Assets/Resources/CoinFeedback/sfx_bom.wav",true);
            AssetDatabase.Refresh();
            var original=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Runtime/Skeletal/PZ_TX.prefab");
            var copy=UnityEngine.Object.Instantiate(original);copy.name="CollisionEffect";copy.GetComponent<NativeSkeletonPlayer>().playOnEnable=false;
            var asset=PrefabUtility.SaveAsPrefabAsset(copy,"Assets/Resources/CoinFeedback/CollisionEffect.prefab");UnityEngine.Object.DestroyImmediate(copy);
            var config=AssetDatabase.LoadAssetAtPath<LifecycleVisualConfig>("Assets/Config/Runtime/LifecycleVisuals.asset");config.reviveRemoveDuration=.12f;config.reviveRemoveStagger=.04f;EditorUtility.SetDirty(config);
            const string path="Assets/Prefabs/Runtime/RecoveredMain.prefab";var root=PrefabUtility.LoadPrefabContents(path);
            try{Apply(root,asset.GetComponent<NativeSkeletonPlayer>());PrefabUtility.SaveAsPrefabAsset(root,path);}finally{PrefabUtility.UnloadPrefabContents(root);}
            var scene=EditorSceneManager.OpenScene("Assets/Scenes/RecoveredMain.unity");
            foreach(var go in scene.GetRootGameObjects())if(go.GetComponent<RecoveredGameSession>())Apply(go,asset.GetComponent<NativeSkeletonPlayer>());
            EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();
        }
    }
}
