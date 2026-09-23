using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace CoinMerge.Recovery.Editor
{
    public static class CoinAmountPanelAuthor
    {
        const float K = 750f / 941f;
        const float CardWidth = 421;
        const float CardHeight = CardWidth * 155f / 412f;
        // Selected artwork includes a badge extending beyond its 411 x 153 plate.
        // Align the plate, not the complete 421 x 166 texture, to the normal card.
        static readonly Vector2 SelectedSize = new Vector2(CardWidth * 421f / 411f, CardHeight * 166f / 153f);
        static readonly Vector2 SelectedOffset = new Vector2((SelectedSize.x - CardWidth) * .5f, -(SelectedSize.y - CardHeight) * .5f);

        [MenuItem("Coin Merge/Reskin/Repair coin amount panel and selection fit")]
        public static void Run()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) throw new InvalidOperationException("Repair the coin page in Edit Mode.");
            var importer = (TextureImporter)AssetImporter.GetAtPath("Assets/Resources/CoinReskin/Amounts.png");
            // Include the complete inner rounded corner below the heading in the fixed slice.
            importer.spriteBorder = new Vector4(90, 76, 90, 198);
            importer.SaveAndReimport();
            const string prefab = "Assets/Prefabs/Runtime/RecoveredMain.prefab";
            var root = PrefabUtility.LoadPrefabContents(prefab);
            try { Apply(root); PrefabUtility.SaveAsPrefabAsset(root, prefab); }
            finally { PrefabUtility.UnloadPrefabContents(root); }
            var scene = EditorSceneManager.OpenScene("Assets/Scenes/RecoveredMain.unity");
            foreach (var candidate in scene.GetRootGameObjects())
                if (candidate.GetComponent<RecoveredGameSession>()) Apply(candidate);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            Debug.Log("COIN_AMOUNT_PANEL_AUTHORED");
        }

        static Dictionary<int, RectTransform> Nodes(GameObject root)
        {
            string uuid = root.GetComponent<RecoveredNode>().sourceUuid;
            var result = new Dictionary<int, RectTransform>();
            foreach (var node in root.GetComponentsInChildren<RecoveredNode>(true))
                if (node.sourceUuid == uuid) result[node.sourceObjectId] = (RectTransform)node.transform;
            return result;
        }

        static void Place(RectTransform rect, Vector2 position, Vector2 size)
        {
            rect.anchorMin = rect.anchorMax = rect.pivot = Vector2.one * .5f;
            rect.localScale = Vector3.one;
            rect.anchoredPosition = position * K;
            rect.sizeDelta = size * K;
        }

        static void Apply(GameObject root)
        {
            var menus = root.GetComponent<RecoveredGameSession>().menus;
            var page = Nodes(menus.pages[RecoveredMainMenus.Coin]);
            var panel = page[5].GetComponent<Image>();
            panel.type = Image.Type.Sliced;
            panel.preserveAspect = false;
            panel.pixelsPerUnitMultiplier = 1 / K;
            // Only moving rows are clipped. Leave room for both card borders and badges.
            var scroll = page[18].GetComponent<ScrollRect>();
            var frame = (RectTransform)scroll.transform;
            frame.offsetMin = new Vector2(10 * K, 34 * K);
            frame.offsetMax = new Vector2(-10 * K, -157 * K);
            scroll.horizontal = false;
            scroll.vertical = true;
            scroll.movementType = ScrollRect.MovementType.Clamped;
            int rows = (menus.coinRows.Length + 1) / 2;
            float height = (rows - 1) * 173 + 83 + CardHeight * .5f + (SelectedSize.y - CardHeight) + 6;
            scroll.content.sizeDelta = new Vector2(889, height) * K;
            scroll.content.anchoredPosition = Vector2.zero;
            foreach (var row in menus.coinRows)
            {
                var nodes = Nodes(row.root);
                foreach (int id in new[] { 2, 3, 4 })
                {
                    var image = nodes[id].GetComponent<Image>();
                    bool selected = id == 2;
                    Vector2 offset = selected ? SelectedOffset : Vector2.zero;
                    Place(nodes[id], offset, selected ? SelectedSize : new Vector2(CardWidth, CardHeight));
                    image.type = Image.Type.Simple;
                    image.preserveAspect = false;
                    // All three labels retain the same screen-space baseline when changing state.
                    var label = image.GetComponentInChildren<Text>(true);
                    if (label) Place(label.rectTransform, new Vector2(-4, 6) - offset, new Vector2(366, 110));
                }
            }
        }
    }
}
