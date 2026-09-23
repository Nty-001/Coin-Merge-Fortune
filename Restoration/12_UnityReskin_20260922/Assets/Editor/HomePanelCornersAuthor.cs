using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace CoinMerge.Recovery.Editor
{
    // Changes only the two home background graphics. Their text, hit areas and
    // gameplay anchors stay on the original transforms.
    public static class HomePanelCornersAuthor
    {
        const float HorizontalArtScale = 2f / 3f;
        const string Art = "Assets/Resources/HomeReskin/";

        [MenuItem("Coin Merge/Reskin/Repair home panel corners")]
        public static void Run()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                throw new InvalidOperationException("Apply the authored repair in Edit Mode.");
            Slice("Notice", new Vector4(48, 32, 48, 32));
            Slice("Bottom", new Vector4(90, 60, 90, 60));
            const string prefab = "Assets/Prefabs/Runtime/RecoveredMain.prefab";
            var root = PrefabUtility.LoadPrefabContents(prefab);
            try { Apply(root); PrefabUtility.SaveAsPrefabAsset(root, prefab); }
            finally { PrefabUtility.UnloadPrefabContents(root); }
            var scene = EditorSceneManager.OpenScene("Assets/Scenes/RecoveredMain.unity");
            foreach (var candidate in scene.GetRootGameObjects())
                if (candidate.GetComponent<RecoveredGameSession>()) Apply(candidate);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            Debug.Log("HOME_PANEL_CORNERS_AUTHORED");
        }

        static void Slice(string name, Vector4 border)
        {
            var importer = (TextureImporter)AssetImporter.GetAtPath(Art + name + ".png");
            importer.spriteBorder = border;
            importer.SaveAndReimport();
        }

        static void Apply(GameObject root)
        {
            var session = root.GetComponent<RecoveredGameSession>();
            string uuid = session.moneyText.GetComponent<RecoveredNode>().sourceUuid;
            var nodes = new Dictionary<int, RectTransform>();
            foreach (var node in root.GetComponentsInChildren<RecoveredNode>(true))
                if (node.sourceUuid == uuid) nodes[node.sourceObjectId] = (RectTransform)node.transform;

            var notice = nodes[16];
            // Keep text and trophy at their original size. Only the art leaf
            // compensates for the ellipse already present in the supplied PNG.
            var leaf = notice.Find("NoticePanelCorners") as RectTransform;
            if (!leaf)
            {
                leaf = (RectTransform)new GameObject("NoticePanelCorners", typeof(RectTransform),
                    typeof(CanvasRenderer), typeof(Image)).transform;
                leaf.SetParent(notice, false);
                leaf.gameObject.layer = notice.gameObject.layer;
            }
            leaf.SetAsFirstSibling();
            leaf.anchorMin = leaf.anchorMax = leaf.pivot = new Vector2(.5f, .5f);
            leaf.anchoredPosition = Vector2.zero;
            leaf.sizeDelta = new Vector2(notice.rect.width / HorizontalArtScale, notice.rect.height);
            leaf.localScale = new Vector3(HorizontalArtScale, 1, 1);
            var noticeImage = leaf.GetComponent<Image>();
            noticeImage.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(Art + "Notice.png");
            noticeImage.type = Image.Type.Sliced;
            noticeImage.pixelsPerUnitMultiplier = Density(noticeImage) * 4f / 3f;
            noticeImage.color = new Color(.95f, .78f, .64f, .80f);
            noticeImage.raycastTarget = false;
            notice.GetComponent<Image>().enabled = false;

            var bottom = nodes[32];
            if (bottom.childCount != 0) throw new InvalidOperationException("Bottom art must be a visual leaf.");
            // Source corners are about 80 by 55 pixels. Fix both slice axes and
            // normalize that ellipse to a ~32-unit radius without moving the floor.
            bottom.localScale = new Vector3(HorizontalArtScale, 1, 1);
            bottom.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 750 / HorizontalArtScale);
            var bottomImage = bottom.GetComponent<Image>();
            bottomImage.type = Image.Type.Sliced;
            bottomImage.pixelsPerUnitMultiplier = Density(bottomImage) * 5f / 3f;
        }

        static float Density(Image image)
        {
            // The recovered gameplay canvas uses 32 reference pixels per unit,
            // while these sprites import at 100; account for both authored values.
            var scaler = image.GetComponentInParent<CanvasScaler>();
            if (!scaler || !image.sprite) throw new InvalidOperationException("Missing home art density configuration.");
            return scaler.referencePixelsPerUnit / image.sprite.pixelsPerUnit;
        }
    }
}
