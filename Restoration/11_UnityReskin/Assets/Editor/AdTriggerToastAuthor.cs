using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace CoinMerge.Recovery.Editor
{
    public static class AdTriggerToastAuthor
    {
        static void Apply(GameObject root)
        {
            if (root.GetComponentInChildren<AdTriggerToast>(true)) return;
            var session = root.GetComponent<RecoveredGameSession>();
            var go = new GameObject("AdTriggerHints", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(AdTriggerToast));
            go.transform.SetParent(root.transform, false); go.layer = 5;
            var canvas = go.GetComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = session.worldCamera; canvas.planeDistance = 10; canvas.sortingOrder = 3000;
            var scaler = go.GetComponent<CanvasScaler>(); scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
            var viewport = session.worldCamera.GetComponent<DeviceSafeViewport>();
            var scalers = new System.Collections.Generic.List<CanvasScaler>(viewport.scalers); scalers.Add(scaler); viewport.scalers = scalers.ToArray();
            var card = new GameObject("FloatingHint", typeof(RectTransform), typeof(Image), typeof(CanvasGroup));
            card.transform.SetParent(go.transform, false); card.layer = 5;
            var rect = card.GetComponent<RectTransform>(); rect.anchorMin = rect.anchorMax = new Vector2(.5f, .78f); rect.sizeDelta = new Vector2(650, 100);
            var background = card.GetComponent<Image>(); background.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Resources/HomeReskin/Notice.png");
            background.preserveAspect = true; background.raycastTarget = false;
            var textGo = new GameObject("Label", typeof(RectTransform), typeof(Text), typeof(RecoveredRoundOutline));
            textGo.transform.SetParent(card.transform, false); textGo.layer = 5;
            var text = textGo.GetComponent<Text>(); text.font = AssetDatabase.LoadAssetAtPath<Font>("Assets/Art/HotUpdate/Fonts/FZY4JW.ttf");
            if (!text.font || !background.sprite) throw new Exception("Ad hint assets missing");
            text.fontSize = 34; text.alignment = TextAnchor.MiddleCenter; text.color = Color.white; text.raycastTarget = false;
            text.supportRichText = false; text.horizontalOverflow = HorizontalWrapMode.Wrap; text.verticalOverflow = VerticalWrapMode.Truncate;
            text.rectTransform.anchorMin = Vector2.zero; text.rectTransform.anchorMax = Vector2.one;
            text.rectTransform.offsetMin = new Vector2(24, 12); text.rectTransform.offsetMax = new Vector2(-24, -12);
            var outline = textGo.GetComponent<RecoveredRoundOutline>(); outline.effectColor = new Color(0, .22f, .65f, 1); outline.effectDistance = new Vector2(2, 2);
            var hint = go.GetComponent<AdTriggerToast>(); hint.session = session; hint.card = rect; hint.label = text; hint.group = card.GetComponent<CanvasGroup>();
            hint.group.alpha = 0; hint.group.blocksRaycasts = false; hint.group.interactable = false;
        }
        public static void Author()
        {
            const string path = "Assets/Prefabs/Runtime/RecoveredMain.prefab";
            var root = PrefabUtility.LoadPrefabContents(path);
            try { Apply(root); PrefabUtility.SaveAsPrefabAsset(root, path); } finally { PrefabUtility.UnloadPrefabContents(root); }
            var scene = EditorSceneManager.OpenScene("Assets/Scenes/RecoveredMain.unity");
            foreach (var go in scene.GetRootGameObjects()) if (go.GetComponent<RecoveredGameSession>()) Apply(go);
            EditorSceneManager.SaveScene(scene); AssetDatabase.SaveAssets();
        }
        public static void Review() { Author(); AdTriggerToastReview.Run(); }
    }
}
