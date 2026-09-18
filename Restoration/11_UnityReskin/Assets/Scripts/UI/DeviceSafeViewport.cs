using UnityEngine;
using UnityEngine.UI;

namespace CoinMerge.Recovery
{
    // One viewport for world physics, UI and input: no device-specific gameplay path.
    [DefaultExecutionOrder(-200)]
    public sealed class DeviceSafeViewport : MonoBehaviour
    {
        public Camera contentCamera;
        public CanvasScaler[] scalers;
        public Vector2 minimumDesignSize = new Vector2(750, 1624);
        public float edgePadding = 6;
        public bool logChanges;
        Rect lastViewport;
        Vector2Int lastResolution;

        void OnEnable() { Refresh(); }
        void LateUpdate() { Refresh(); }

        public static Rect Fit(Vector2Int resolution, Rect safeArea, Vector2 design, float padding)
        {
            float w = Mathf.Max(1, resolution.x), h = Mathf.Max(1, resolution.y);
            float left = Mathf.Clamp(safeArea.xMin, 0, w), right = Mathf.Clamp(safeArea.xMax, 0, w);
            float bottom = Mathf.Clamp(safeArea.yMin, 0, h), top = Mathf.Clamp(safeArea.yMax, 0, h);
            if (right <= left || top <= bottom) { left = bottom = 0; right = w; top = h; }
            // Padding is expressed in reference-canvas units, independent of pixel density.
            float scale = Mathf.Min((right-left)/design.x, (top-bottom)/design.y);
            float gap = Mathf.Max(0, padding)*scale;
            // Original cc.Canvas: fitWidth=true, fitHeight=false. The visible height
            // changes with the device; never letterbox a tablet into a phone frame.
            bottom += gap; top -= gap;
            return new Rect(left, bottom, right-left, top-bottom);
        }

        public void Refresh()
        {
            if (!contentCamera || Screen.width <= 0 || Screen.height <= 0) return;
            Apply(new Vector2Int(Screen.width, Screen.height), Screen.safeArea);
        }

        // Explicit dimensions also allow the same calculation to be validated without device hacks.
        public void Apply(Vector2Int resolution, Rect safeArea)
        {
            Rect pixels = Fit(resolution, safeArea, minimumDesignSize, edgePadding);
            if (pixels == lastViewport && resolution == lastResolution) return;
            contentCamera.rect = new Rect(pixels.x/resolution.x, pixels.y/resolution.y,
                pixels.width/resolution.x, pixels.height/resolution.y);
            float scale = pixels.width/minimumDesignSize.x;
            foreach (var scaler in scalers) if (scaler) scaler.scaleFactor = scale;
            lastViewport = pixels; lastResolution = resolution;
            if (logChanges) Debug.Log("SAFE_VIEWPORT screen="+resolution+" safe="+safeArea+" content="+pixels);
        }
    }
}
