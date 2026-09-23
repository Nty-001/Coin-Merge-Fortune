using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace CoinMerge.Recovery.Editor
{
    // Produces both native startup prefabs. No static UI is constructed at runtime.
    public static class LoadingReskinAuthor
    {
        const float Width = 941, Height = 1672;

        [MenuItem("Coin Merge/Reskin/Apply vault loading illustration")]
        public static void Run()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                throw new InvalidOperationException("Stop Play before authoring loading assets.");
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            Import("Assets/Resources/VaultLoading/Backdrop.png");
            foreach (var name in new[] { "RewardedLoading", "PackagedLoading" })
            {
                var path = "Assets/Resources/Startup/" + name + ".prefab";
                var root = PrefabUtility.LoadPrefabContents(path);
                try { Apply(root); PrefabUtility.SaveAsPrefabAsset(root, path); }
                finally { PrefabUtility.UnloadPrefabContents(root); }
            }
            AssetDatabase.SaveAssets();
            Debug.Log("VAULT_LOADING_AUTHORED");
        }

        static void Import(string path)
        {
            var importer = (TextureImporter)AssetImporter.GetAtPath(path);
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.spritePixelsPerUnit = 100;
            importer.spriteBorder = Vector4.zero;
            importer.mipmapEnabled = false;
            importer.npotScale = TextureImporterNPOTScale.None;
            importer.alphaIsTransparency = true;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.maxTextureSize = 2048;
            importer.filterMode = FilterMode.Bilinear;
            importer.wrapMode = TextureWrapMode.Clamp;
            var platform = importer.GetDefaultPlatformTextureSettings();
            platform.format = TextureImporterFormat.RGBA32;
            platform.textureCompression = TextureImporterCompression.Uncompressed;
            platform.maxTextureSize = 2048;
            importer.SetPlatformTextureSettings(platform);
            foreach (var name in new[] { "Standalone", "Android", "iPhone" }) importer.ClearPlatformTextureSettings(name);
            var settings = new TextureImporterSettings();
            importer.ReadTextureSettings(settings);
            settings.spriteMeshType = SpriteMeshType.FullRect;
            importer.SetTextureSettings(settings);
            importer.SaveAndReimport();
        }

        static RectTransform Rect(Transform parent, string name, Vector2 size, Vector2 position)
        {
            var node = new GameObject(name, typeof(RectTransform));
            node.layer = 5;
            var rect = (RectTransform)node.transform;
            rect.SetParent(parent, false);
            rect.anchorMin = rect.anchorMax = rect.pivot = Vector2.one * .5f;
            rect.sizeDelta = size;
            rect.anchoredPosition = position;
            return rect;
        }

        static Vector2 Point(float x, float y) => new Vector2(x - Width * .5f, Height * .5f - y);

        static Image Image(Transform parent, string name, Vector2 size, Vector2 position,
            string resource, List<MenuImageBinding> bindings)
        {
            var image = Rect(parent, name, size, position).gameObject.AddComponent<Image>();
            image.raycastTarget = false;
            image.preserveAspect = true;
            image.color = Color.white;
            bindings.Add(new MenuImageBinding { image = image, resourcePath = resource });
            return image;
        }

        static void Apply(GameObject root)
        {
            var view = root.GetComponent<RecoveredLoadingView>();
            var old = new List<GameObject>();
            foreach (Transform child in root.transform) old.Add(child.gameObject);
            foreach (var child in old) UnityEngine.Object.DestroyImmediate(child);
            var canvasRect = Rect(root.transform, "FullScreenLoadingCanvas", new Vector2(Width, Height), Vector2.zero);
            var canvas = canvasRect.gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 1000;
            var scaler = canvas.gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(Width, Height);
            // Uniform cover scale fills the complete screen, including safe-area edges.
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Shrink;
            var poster = Rect(canvas.transform, "VaultPresentation", new Vector2(Width, Height), Vector2.zero);
            var bindings = new List<MenuImageBinding>();
            Image(poster, "Backdrop", new Vector2(Width, Height), Vector2.zero, "VaultLoading/Backdrop", bindings);
            var burst = poster.gameObject.AddComponent<LoadingCoinBurst>();
            burst.origin = Point(466, 1147);
            burst.duration = .76f;
            burst.bobAmplitude = 4;
            burst.bobFrequency = 1.8f;
            var flights = new List<LoadingCoinBurst.Flight>();
            void Coin(int value, float x, float y, float diameter, float delay, float turn)
            {
                // Supplied originals have transparent padding; keep every source pixel.
                var sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Resources/Gameplay/Coins/" + value + ".png");
                if (!sprite) throw new InvalidOperationException("Missing supplied coin " + value);
                var coin = Image(poster, "Coin_" + value, Vector2.one * (diameter / .9f), Point(x, y), "Gameplay/Coins/" + value, bindings);
                flights.Add(new LoadingCoinBurst.Flight
                {
                    rect = coin.rectTransform, opacity = coin.gameObject.AddComponent<CanvasGroup>(),
                    destination = Point(x, y), delay = delay, initialAngle = turn
                });
            }
            Coin(1000, 265, 702, 209, .08f, -24);
            Coin(100, 460, 619, 230, .02f, 18);
            Coin(200, 666, 690, 201, .14f, 26);
            Coin(5, 314, 875, 143, .19f, -20);
            Coin(20, 480, 817, 170, .10f, 16);
            Coin(500, 688, 875, 181, .23f, 24);
            Coin(2, 387, 981, 117, .31f, -26);
            Coin(10, 559, 955, 129, .28f, 20);
            Coin(1, 481, 1045, 111, .38f, -16);
            burst.flights = flights.ToArray();
            var trackSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Resources/LoadingReskin/Track.png");
            var fillSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Resources/LoadingReskin/Fill.png");
            float scale = 646f / trackSprite.rect.width;
            var track = Image(poster, "ProgressTrack", trackSprite.rect.size * scale, Point(470.5f, 1481), "LoadingReskin/Track", bindings);
            view.fill = Image(track.transform, "LiveProgress", fillSprite.rect.size * scale, Vector2.zero, "LoadingReskin/Fill", bindings);
            view.fill.type = UnityEngine.UI.Image.Type.Filled;
            view.fill.fillMethod = UnityEngine.UI.Image.FillMethod.Horizontal;
            view.fill.fillOrigin = 0;
            view.fill.fillAmount = 0;
            // Reference uses a clean bar and separate caption, with no percentage overlay.
            view.percentage = null;
            var caption = Rect(poster, "LoadingCaption", new Vector2(640, 64), Point(470.5f, 1561)).gameObject.AddComponent<Text>();
            caption.font = AssetDatabase.LoadAssetAtPath<Font>("Assets/Art/PopupFonts/Nunito-ExtraBold.ttf");
            caption.text = "Loading...";
            caption.fontSize = 42;
            caption.alignment = TextAnchor.MiddleCenter;
            caption.color = Color.white;
            caption.raycastTarget = false;
            var outline = caption.gameObject.AddComponent<RecoveredRoundOutline>();
            outline.effectDistance = Vector2.one * 2;
            outline.effectColor = new Color(.02f, .23f, .61f, 1);
            var art = root.GetComponent<RecoveredMenuArt>() ?? root.AddComponent<RecoveredMenuArt>();
            art.images = bindings.ToArray();
        }
    }
}
