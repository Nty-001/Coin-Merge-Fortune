using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace CoinMerge.Recovery.Editor
{
    public static class DeviceSafeAreaChecks
    {
        public static void Run()
        {
            var sizes = new[] { new Vector2Int(720,1280), new Vector2Int(900,1600),
                new Vector2Int(1080,1920), new Vector2Int(1080,2340), new Vector2Int(1080,2400),
                new Vector2Int(1179,2556), new Vector2Int(1536,2048), new Vector2Int(1800,2400) };
            int count = 0;
            foreach (var size in sizes)
            foreach (var design in new[] { new Vector2(750,1624), new Vector2(750,1332.625f) })
            foreach (var safe in new[] { new Rect(0,0,size.x,size.y),
                new Rect(0,72,size.x,size.y-210), new Rect(50,90,size.x-100,size.y-270) })
            {
                var fitted = DeviceSafeViewport.Fit(size,safe,design,6);
                if (fitted.xMin < safe.xMin || fitted.yMin < safe.yMin || fitted.xMax > safe.xMax || fitted.yMax > safe.yMax)
                    throw new Exception("Content crossed safe area: "+size);
                if (Mathf.Abs(fitted.width-safe.width)>.01f) throw new Exception("Width-fit introduced side gutters: "+size);
                if (Mathf.Abs(fitted.center.x-safe.center.x)>.01f) throw new Exception("Content not centered");
                count++;
            }
            foreach (var path in new[] {"Assets/Prefabs/Runtime/RecoveredMain.prefab", "Assets/Resources/Startup/RewardedLoading.prefab"})
            {
                var root = PrefabUtility.LoadPrefabContents(path);
                try
                {
                    var fit = root.GetComponentInChildren<DeviceSafeViewport>(true);
                    if (!fit || !fit.contentCamera || fit.scalers.Length == 0) throw new Exception("Missing authored safety: "+path);
                    foreach (var scaler in root.GetComponentsInChildren<CanvasScaler>(true))
                    {
                        if (Array.IndexOf(fit.scalers,scaler)<0 || scaler.uiScaleMode != CanvasScaler.ScaleMode.ConstantPixelSize)
                            throw new Exception("Uncontrolled scaler: "+scaler.name);
                    }
                    if (!root.transform.Find("SafeAreaBackdrop")) throw new Exception("Missing full bleed backdrop");
                }
                finally { PrefabUtility.UnloadPrefabContents(root); }
            }
            Directory.CreateDirectory("Temp");
            File.WriteAllText("Temp/DeviceSafeAreaChecks.txt", "PASS: "+count+" safe-area geometry cases and both authored prefab references.\n");
            Debug.Log("SAFE_AREA_CHECKS_PASSED "+count);
        }
    }
}
