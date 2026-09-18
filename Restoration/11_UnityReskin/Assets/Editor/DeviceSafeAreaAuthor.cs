using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace CoinMerge.Recovery.Editor
{
    public static class DeviceSafeAreaAuthor
    {
        const int BackgroundLayer = 30;
        static void Backdrop(GameObject root)
        {
            if (root.transform.Find("SafeAreaBackdrop")) return;
            var go = new GameObject("SafeAreaBackdrop", typeof(Camera));
            go.transform.SetParent(root.transform, false);
            var camera = go.GetComponent<Camera>();
            camera.orthographic = true; camera.depth = -20;
            camera.clearFlags = CameraClearFlags.SolidColor; camera.backgroundColor = new Color(.3f,.7f,1);
            camera.cullingMask = 1 << BackgroundLayer;
            var canvasGo = new GameObject("BackgroundCanvas", typeof(RectTransform), typeof(Canvas));
            canvasGo.layer = BackgroundLayer; canvasGo.transform.SetParent(go.transform, false);
            var canvas = canvasGo.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera; canvas.worldCamera = camera; canvas.planeDistance = 10;
            var art = new GameObject("Sky", typeof(RectTransform), typeof(Image), typeof(AspectRatioFitter));
            art.layer = BackgroundLayer; art.transform.SetParent(canvasGo.transform, false);
            var image = art.GetComponent<Image>(); image.raycastTarget = false;
            image.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Resources/HomeReskin/Sky.png");
            var fitter = art.GetComponent<AspectRatioFitter>();
            fitter.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
            fitter.aspectRatio = image.sprite.rect.width/image.sprite.rect.height;
        }
        static void Configure(GameObject root, Camera camera, Vector2 design)
        {
            Backdrop(root);
            camera.cullingMask &= ~(1 << BackgroundLayer);
            var safe = camera.GetComponent<DeviceSafeViewport>();
            if (!safe) safe = camera.gameObject.AddComponent<DeviceSafeViewport>();
            safe.contentCamera = camera; safe.minimumDesignSize = design; safe.edgePadding = 6; safe.logChanges = false;
            safe.scalers = root.GetComponentsInChildren<CanvasScaler>(true);
            foreach (var scaler in safe.scalers)
            {
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
                scaler.scaleFactor = 1;
            }
        }
        static void Main(GameObject root)
        {
            var s = root.GetComponent<RecoveredGameSession>();
            Configure(root, s.worldCamera, new Vector2(750,1624));
            // A single full-window sky avoids a visible seam at the safe viewport edges.
            s.worldCamera.clearFlags = CameraClearFlags.Depth;
            s.playfieldLayout.background.GetComponent<Image>().enabled = false;
        }
        static void Loading(GameObject root)
        {
            var canvas = root.GetComponentInChildren<Canvas>(true);
            var cameraGo = new GameObject("SafeLoadingCamera", typeof(Camera));
            cameraGo.transform.SetParent(root.transform, false);
            var camera = cameraGo.GetComponent<Camera>(); camera.orthographic = true; camera.depth = 1;
            camera.clearFlags = CameraClearFlags.Depth; camera.cullingMask = 1 << 5;
            canvas.renderMode = RenderMode.ScreenSpaceCamera; canvas.worldCamera = camera; canvas.planeDistance = 10;
            Configure(root, camera, new Vector2(750,1332.625f));
        }
        public static void Author()
        {
            string p = "Assets/Prefabs/Runtime/RecoveredMain.prefab";
            var root = PrefabUtility.LoadPrefabContents(p);
            try { Main(root); PrefabUtility.SaveAsPrefabAsset(root,p); }
            finally { PrefabUtility.UnloadPrefabContents(root); }
            var scene = EditorSceneManager.OpenScene("Assets/Scenes/RecoveredMain.unity");
            foreach (var go in scene.GetRootGameObjects()) if (go.GetComponent<RecoveredGameSession>()) Main(go);
            EditorSceneManager.SaveScene(scene);
            p = "Assets/Resources/Startup/RewardedLoading.prefab";
            root = PrefabUtility.LoadPrefabContents(p);
            try { if (!root.GetComponentInChildren<DeviceSafeViewport>(true)) Loading(root); PrefabUtility.SaveAsPrefabAsset(root,p); }
            finally { PrefabUtility.UnloadPrefabContents(root); }
            scene = EditorSceneManager.OpenScene("Assets/Scenes/RecoveredLoading.unity");
            foreach (var go in scene.GetRootGameObjects())
                foreach (var camera in go.GetComponentsInChildren<Camera>(true)) camera.depth = -30;
            EditorSceneManager.SaveScene(scene);
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
            PlayerSettings.Android.renderOutsideSafeArea = true;
            AssetDatabase.SaveAssets();
        }
        public static void Build() { Author(); DeviceSafeAreaChecks.Run(); AndroidApkBuild.Run(); }
    }
}
