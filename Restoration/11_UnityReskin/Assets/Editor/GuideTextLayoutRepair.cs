using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CoinMerge.Recovery.Editor
{
    /// <summary>Author the B guide's independent text regions and validate their rendered intersections.</summary>
    [InitializeOnLoad]
    public static class GuideTextLayoutRepair
    {
        const string Request = "Temp/GuideTextLayoutRepair.request";
        const string Key = "CoinMerge.GuideTextLayoutRepair";
        const string Prefix = "coinmerge.guide.layout.disposable";
        const string Output = "Design/GuideTextFix20260917";
        static RecoveredGameSession session;
        static RenderTexture target;
        static string[] countries;
        static readonly int[] Steps = { 1, 3, 4 };
        static int countryIndex, stepIndex, resolutionIndex, checks, phase;
        static bool shown;
        static double next;
        static readonly List<string> errors = new List<string>();

        [Serializable] sealed class Report
        {
            public bool passed;
            public int layoutCases;
            public string[] countries, errors;
            public string interaction, unityVersion;
        }

        static GuideTextLayoutRepair()
        {
            EditorApplication.update += Poll;
            EditorApplication.playModeStateChanged += State;
        }

        static void Poll()
        {
            if (!File.Exists(Request) || SessionState.GetBool(Key, false) || EditorApplication.isCompiling || EditorApplication.isUpdating) return;
            if (EditorApplication.isPlaying) { EditorApplication.ExitPlaymode(); return; }
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;
            File.Delete(Request);
            try { RepairAndValidate(); }
            catch (Exception e) { Directory.CreateDirectory(Output); File.WriteAllText(Output + "/author_error.txt", e.ToString()); Debug.LogException(e); }
        }

        // Separate body, chip and continuation label. Outer panel and native Button are unchanged.
        public static void Apply(GameObject root)
        {
            var guide = root.GetComponent<RecoveredGameSession>().guideView;
            var frame = guide.stepFour.transform.Find("content/bg");
            var body = frame.Find("step2Label1").GetComponent<Text>();
            var start = frame.Find("step2Label2").GetComponent<Text>();
            Region(body, new Vector2(76, 30), new Vector2(470, 156), 38, false, 3, TextAnchor.MiddleLeft);
            Region(start, new Vector2(140, -88), new Vector2(220, 58), 36, true, 1, TextAnchor.MiddleRight);
            // The replacement hand has a wider fingertip; reserve space so it cannot cover Start.
            ((RectTransform)frame.Find("hand")).anchoredPosition = new Vector2(324, -165);
        }

        static void Region(Text text, Vector2 position, Vector2 size, int fontSize, bool singleLine, int lines, TextAnchor alignment)
        {
            var rect = text.rectTransform;
            rect.anchorMin = rect.anchorMax = rect.pivot = Vector2.one * .5f;
            rect.localScale = Vector3.one;
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
            text.alignment = alignment;
            var fit = text.GetComponent<RecoveredTextFit>();
            fit.maximumFontSize = fontSize;
            fit.singleLine = singleLine;
            fit.maximumLines = lines;
            EditorUtility.SetDirty(text);
            EditorUtility.SetDirty(fit);
        }

        [MenuItem("Coin Merge/Reskin/Repair and validate guide text layout")]
        public static void RepairAndValidate()
        {
            Directory.CreateDirectory(Output);
            const string prefab = "Assets/Prefabs/Runtime/RecoveredMain.prefab";
            var root = PrefabUtility.LoadPrefabContents(prefab);
            try { Apply(root); PrefabUtility.SaveAsPrefabAsset(root, prefab); }
            finally { PrefabUtility.UnloadPrefabContents(root); }
            var scene = EditorSceneManager.OpenScene("Assets/Scenes/RecoveredMain.unity");
            foreach (var go in scene.GetRootGameObjects()) if (go.GetComponent<RecoveredGameSession>()) Apply(go);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            var store = new PlayerStore(Prefix);
            store.ResetPlayer(); store.ResetProfile();
            store.Save(new PlayerProgress { guideStep = 4, fakeMoney = 220 });
            store.SaveProfile(new VersionProfile { country = "US", cohort = "B", rewardedVariant = true });
            var instance = UnityEngine.Object.FindObjectOfType<RecoveredGameSession>();
            instance.saveNamespace = Prefix;
            instance.automaticInput = false;
            SessionState.SetBool(Key, true);
            EditorApplication.EnterPlaymode();
        }

        static void State(PlayModeStateChange state)
        {
            if (!SessionState.GetBool(Key, false)) return;
            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                session = null; shown = false; errors.Clear(); countryIndex = stepIndex = resolutionIndex = checks = phase = 0;
                EditorApplication.isPaused = false; Time.timeScale = 1;
                next = EditorApplication.timeSinceStartup + .5;
                Application.logMessageReceived += Log;
                EditorApplication.update += Tick;
            }
            if (state == PlayModeStateChange.EnteredEditMode)
            {
                var store = new PlayerStore(Prefix); store.ResetPlayer(); store.ResetProfile();
                SessionState.SetBool(Key, false);
                EditorSceneManager.OpenScene("Assets/Scenes/RecoveredLoading.unity");
            }
        }

        static void Log(string message, string stack, LogType type)
        { if (type == LogType.Error || type == LogType.Exception) errors.Add(message); }

        static void Resize()
        {
            session.worldCamera.targetTexture = null;
            if (target) UnityEngine.Object.DestroyImmediate(target);
            target = resolutionIndex == 0 ? new RenderTexture(1080, 2340, 24) : new RenderTexture(941, 1672, 24);
            session.worldCamera.targetTexture = target;
            Canvas.ForceUpdateCanvases(); session.playfieldLayout.Refresh();
        }

        static void Tick()
        {
            if (EditorApplication.timeSinceStartup < next) return;
            try
            {
                if (errors.Count > 0) throw new Exception(errors[0]);
                if (!session)
                {
                    session = UnityEngine.Object.FindObjectOfType<RecoveredGameSession>();
                    var list = new List<string> { "US", "RU", "JP" };
                    foreach (var country in session.board.Config.rules.supportedCountries) if (!list.Contains(country)) list.Add(country);
                    countries = list.ToArray(); Resize();
                }
                if (phase > 0) { Interaction(); return; }
                if (countryIndex == countries.Length)
                {
                    if (++resolutionIndex == 2) { phase = 1; Interaction(); return; }
                    countryIndex = 0; Resize();
                }
                if (!shown)
                {
                    if (stepIndex == 0) session.ChangeProfile(countries[countryIndex], "B", true);
                    session.Player.guideStep = Steps[stepIndex];
                    session.guideView.Show(Steps[stepIndex], session.Player.fakeMoney);
                    shown = true; next = EditorApplication.timeSinceStartup + .18; return;
                }
                Render(); Audit(Steps[stepIndex]); checks++;
                if (countryIndex < 3) Capture(countries[countryIndex] + "_step" + Steps[stepIndex] + "_" + target.width + ".png");
                shown = false;
                if (++stepIndex == Steps.Length) { stepIndex = 0; countryIndex++; }
            }
            catch (Exception e) { Finish(e.ToString()); }
        }

        static void Render() { Canvas.ForceUpdateCanvases(); session.worldCamera.Render(); }

        static Rect ScreenBounds(RectTransform transform, Rect rect)
        {
            var min = new Vector2(float.MaxValue, float.MaxValue); var max = -min;
            foreach (var point in new[] { rect.min, rect.max, new Vector2(rect.xMin, rect.yMax), new Vector2(rect.xMax, rect.yMin) })
            {
                var p = RectTransformUtility.WorldToScreenPoint(session.worldCamera, transform.TransformPoint(point));
                min = Vector2.Min(min, p); max = Vector2.Max(max, p);
            }
            return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
        }

        static Rect TextBounds(Text text)
        {
            var mesh = text.canvasRenderer.GetMesh();
            if (!mesh || mesh.vertexCount == 0) throw new Exception("Missing glyph mesh: " + text.name);
            var b = mesh.bounds;
            return ScreenBounds(text.rectTransform, Rect.MinMaxRect(b.min.x, b.min.y, b.max.x, b.max.y));
        }

        static void Audit(int step)
        {
            var guide = session.guideView;
            var group = step == 1 ? guide.stepOne.transform.Find("bg") : step == 3 ? guide.stepThree.transform.Find("content/bg") : guide.stepFour.transform.Find("content/bg");
            var panel = step == 4 ? (RectTransform)group : (RectTransform)group.Find("bg");
            var panelBounds = ScreenBounds(panel, panel.rect);
            var chip = (RectTransform)group.Find("2000");
            var chipBounds = ScreenBounds(chip, chip.rect);
            var texts = group.GetComponentsInChildren<Text>();
            for (int i = 0; i < texts.Length; i++)
            {
                var bounds = TextBounds(texts[i]);
                if (bounds.Overlaps(chipBounds)) throw new Exception("Guide text overlaps chip: " + texts[i].name);
                if (step == 4)
                {
                    var hand = (RectTransform)group.Find("hand");
                    if (bounds.Overlaps(ScreenBounds(hand, hand.rect))) throw new Exception("Guide hand covers text: " + texts[i].name);
                }
                if (!panelBounds.Contains(bounds.min) || !panelBounds.Contains(bounds.max)) throw new Exception("Guide text leaves panel: " + texts[i].name);
                for (int j = 0; j < i; j++) if (bounds.Overlaps(TextBounds(texts[j]))) throw new Exception("Guide text overlaps another label: " + texts[i].name);
            }
        }

        static void Click(Button button)
        {
            Render();
            var rect = button.targetGraphic.rectTransform;
            var pointer = new PointerEventData(EventSystem.current) { button = PointerEventData.InputButton.Left, position = RectTransformUtility.WorldToScreenPoint(session.worldCamera, rect.TransformPoint(rect.rect.center)) };
            var hits = new List<RaycastResult>(); EventSystem.current.RaycastAll(pointer, hits);
            if (hits.Count == 0 || ExecuteEvents.GetEventHandler<IPointerClickHandler>(hits[0].gameObject) != button.gameObject) throw new Exception("Guide native Button blocked");
            ExecuteEvents.Execute(button.gameObject, pointer, ExecuteEvents.pointerClickHandler);
        }

        static void Interaction()
        {
            if (phase == 1)
            {
                session.ChangeProfile("US", "B", true); session.Player.guideStep = 3; session.guideView.Show(3, session.Player.fakeMoney);
                Click(session.guideView.threeButton);
                if (session.Player.guideStep != 4 || !session.guideView.stepFour.activeInHierarchy) throw new Exception("Step 3 did not advance to step 4");
                phase = 2; next = EditorApplication.timeSinceStartup + .25; return;
            }
            Audit(4); Click(session.guideView.fourButton);
            if (session.Player.guideStep != 9999 || session.guideView.gameObject.activeSelf) throw new Exception("Start did not complete the guide");
            Finish(null);
        }

        static void Capture(string name)
        {
            var old = RenderTexture.active; RenderTexture.active = target;
            var tex = new Texture2D(target.width, target.height, TextureFormat.RGB24, false);
            tex.ReadPixels(new Rect(0, 0, target.width, target.height), 0, 0); tex.Apply();
            File.WriteAllBytes(Output + "/" + name, tex.EncodeToPNG());
            RenderTexture.active = old; UnityEngine.Object.DestroyImmediate(tex);
        }

        static void Finish(string error)
        {
            EditorApplication.update -= Tick; Application.logMessageReceived -= Log;
            if (error != null) errors.Add(error);
            if (session) session.worldCamera.targetTexture = null;
            if (target) UnityEngine.Object.DestroyImmediate(target);
            File.WriteAllText(Output + "/result.json", JsonUtility.ToJson(new Report { passed = errors.Count == 0, layoutCases = checks, countries = countries, errors = errors.ToArray(), interaction = error == null ? "Native Button: step 3 -> step 4 -> guide complete" : "Failed", unityVersion = Application.unityVersion }, true));
            EditorApplication.ExitPlaymode();
        }
    }
}
