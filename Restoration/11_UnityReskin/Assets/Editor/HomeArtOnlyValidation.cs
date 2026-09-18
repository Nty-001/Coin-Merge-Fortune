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
    // Disposable Play Mode fixture; never saves a scene, prefab, or production player.
    [InitializeOnLoad]
    public static class HomeArtOnlyValidation
    {
        const string Key = "CoinMerge.HomeArtR2.Validation";
        const string Prefix = "coinmerge.home.art.r2.disposable";
        static readonly List<string> checks = new List<string>();
        static RecoveredGameSession session;
        static Camera camera;
        static RenderTexture target;
        static int phase;
        static double next, deadline;
        static string Output => Path.GetFullPath("Design/HomeR1/Verification");
        [Serializable] sealed class Report
        {
            public bool passed;
            public string error, unityVersion;
            public string[] checks;
        }
        static HomeArtOnlyValidation() { EditorApplication.playModeStateChanged += State; }
        public static void Run()
        {
            Directory.CreateDirectory(Output);
            SessionState.SetBool(Key, true);
            SessionState.SetString(Key + ".error", "");
            var store = new PlayerStore(Prefix);
            store.ResetPlayer(); store.ResetProfile();
            store.Save(new PlayerProgress { guideStep = 5, fakeMoney = 446.43, gameTotalScore = 128, currentLotteryCount = 1 });
            store.SaveProfile(new VersionProfile { country = "US", cohort = "B", rewardedVariant = true });
            EditorSceneManager.OpenScene("Assets/Scenes/RecoveredLoading.unity");
            UnityEngine.Object.FindObjectOfType<RecoveredStartup>().saveNamespace = Prefix;
            EditorApplication.EnterPlaymode();
        }
        static void State(PlayModeStateChange state)
        {
            if (!SessionState.GetBool(Key, false)) return;
            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                checks.Clear(); phase = 0; next = 0; deadline = EditorApplication.timeSinceStartup + 90;
                Application.logMessageReceived += Log; EditorApplication.update += Tick;
            }
            if (state == PlayModeStateChange.EnteredEditMode)
            {
                var store = new PlayerStore(Prefix); store.ResetPlayer(); store.ResetProfile();
                SessionState.SetBool(Key, false);
                EditorApplication.Exit(SessionState.GetString(Key + ".error", "").Length == 0 ? 0 : 1);
            }
        }
        static void Log(string message, string stack, LogType type)
        {
            if (type == LogType.Error || type == LogType.Exception) SessionState.SetString(Key + ".error", message + "\n" + stack);
        }
        static void Require(bool condition, string description)
        {
            if (!condition) throw new Exception(description);
            checks.Add(description);
        }
        static void Tick()
        {
            if (EditorApplication.timeSinceStartup < next) return;
            next = EditorApplication.timeSinceStartup + .6;
            try
            {
                if (EditorApplication.timeSinceStartup > deadline) throw new Exception("R2 validation timeout");
                string error = SessionState.GetString(Key + ".error", "");
                if (error.Length > 0) throw new Exception(error);
                switch (phase)
                {
                    case 0:
                        session = UnityEngine.Object.FindObjectOfType<RecoveredGameSession>();
                        if (!session || session.Player == null) return;
                        Require(session.saveNamespace == Prefix, "Loading reaches main through normal startup with disposable save");
                        session.automaticInput = false; camera = session.worldCamera;
                        Resize(750, 1624); phase++; break;
                    case 1:
                        Require(session.Player.fakeMoney == 446.43 && session.bubble.activeSelf && session.bubbleText.text.Contains("53.56"), "Balance hint retains source truncation: " + session.bubbleText.text);
                        Require(session.notice.label.text.Length > 0, "Native rotating notice is populated");
                        foreach (int value in new[] { 1, 2, 5, 10, 20, 50, 100, 200, 500, 1000, 2000 })
                            Require(session.board.SpriteFor(value), "Denomination sprite loads: " + value);
                        Capture("home_us_initial.png");
                        PopulateArtFixture(); phase++; break;
                    case 2:
                        Capture("home_us_r2_750x1624.png"); ClickAction(1); phase++; break;
                    case 3:
                        Require(session.menus.CurrentPage == 0, "Settings opens through native Button hit test");
                        Capture("settings_r2.png"); session.menus.CloseAll(); ClickAction(2); phase++; next += 2; break;
                    case 4:
                        Require(session.menus.CurrentPage == 1, "Rules opens through native Button hit test");
                        Capture("rules_r2.png"); session.menus.CloseAll(); ClickAction(3); phase++; break;
                    case 5:
                        Require(session.menus.CurrentPage == 3, "Cash withdrawal opens through native Button hit test");
                        session.menus.CloseAll(); ClickAction(4); phase++; break;
                    case 6:
                        Require(session.menus.CurrentPage == 4, "2000-chip withdrawal opens through native Button hit test");
                        session.menus.CloseAll(); session.Player.gameTotalScore = 0; session.GmRefresh(); Click(session.wheelButton);
                        Require(session.menus.toast.activeSelf, "Wheel keeps original insufficient-points response");
                        Click(session.gm.open); phase++; break;
                    case 7:
                        Require(session.gm.IsOpen, "GM popup opens through native Button hit test");
                        Click(session.gm.close); Require(!session.gm.IsOpen, "GM close receives native pointer event");
                        session.ChangeProfile("JP", "B", true); phase++; break;
                    case 8:
                        Require(session.moneyText.text.Length > 0 && session.notice.label.text.Length > 0, "JP native localization remains populated");
                        Capture("home_jp_r2.png");
                        session.ChangeProfile("US", "B", true); session.Player.gameTotalScore = 128; session.GmRefresh();
                        Resize(941, 1672); phase++; break;
                    case 9:
                        session.idleGuide.ResetIdle(); PopulateArtFixture(); phase++; break;
                    case 10:
                        Capture("home_us_r2_941x1672.png"); Finish(null); break;
                }
            }
            catch (Exception e) { Finish(e.ToString()); }
        }
        static void PopulateArtFixture()
        {
            var board = session.board;
            for (int i = board.Coins.Count - 1; i >= 0; i--) if (!board.Coins[i].IsPreview) board.Remove(board.Coins[i]);
            // Fixed positions are only screenshot fixture data; no serialized gameplay state changes.
            int[] values = { 5, 500, 20, 10, 100, 20, 10, 2, 1 };
            float[] xs = { -310, -115, 105, 305, 145, -300, -230, 0, 325 };
            float[] ys = { 65, 135, 75, 58, 290, 285, 415, 450, 400 };
            for (int i = 0; i < values.Length; i++)
            {
                var coin = board.Spawn(values[i], new Vector3(board.transform.position.x + xs[i] / board.Units,
                    board.ground.position.y + ys[i] / board.Units, board.transform.position.z));
                coin.Freeze();
            }
            session.idleGuide.ResetIdle();
        }
        static void Resize(int width, int height)
        {
            camera.targetTexture = null;
            if (target) UnityEngine.Object.DestroyImmediate(target);
            target = new RenderTexture(width, height, 24); camera.targetTexture = target;
            Canvas.ForceUpdateCanvases(); session.playfieldLayout.Refresh();
        }
        static void ClickAction(int action)
        {
            foreach (var binding in session.menus.actions)
                if (binding.action == action && binding.button.gameObject.activeInHierarchy) { Click(binding.button); return; }
            throw new Exception("Missing main action " + action);
        }
        static void Click(Button button)
        {
            Require(button && button.gameObject.activeInHierarchy && button.IsInteractable(), "Visible interactable Button: " + button.name);
            Require(button.onClick.GetPersistentEventCount() == 0, "Code-bound Button event: " + button.name);
            Canvas.ForceUpdateCanvases(); camera.Render();
            var rect = button.targetGraphic.rectTransform;
            var pointer = new PointerEventData(EventSystem.current) { button = PointerEventData.InputButton.Left,
                position = RectTransformUtility.WorldToScreenPoint(camera, rect.TransformPoint(rect.rect.center)) };
            var hits = new List<RaycastResult>(); EventSystem.current.RaycastAll(pointer, hits);
            var actual = hits.Count > 0 ? ExecuteEvents.GetEventHandler<IPointerClickHandler>(hits[0].gameObject) : null;
            Require(actual == button.gameObject, "Unobstructed pointer hit: " + button.name);
            ExecuteEvents.Execute(actual, pointer, ExecuteEvents.pointerDownHandler);
            ExecuteEvents.Execute(actual, pointer, ExecuteEvents.pointerUpHandler);
            ExecuteEvents.Execute(actual, pointer, ExecuteEvents.pointerClickHandler);
        }
        static void Capture(string name)
        {
            Canvas.ForceUpdateCanvases(); camera.Render(); var previous = RenderTexture.active; RenderTexture.active = target;
            var image = new Texture2D(target.width, target.height, TextureFormat.RGB24, false);
            image.ReadPixels(new Rect(0, 0, target.width, target.height), 0, 0); image.Apply();
            File.WriteAllBytes(Path.Combine(Output, name), image.EncodeToPNG());
            RenderTexture.active = previous; UnityEngine.Object.DestroyImmediate(image);
        }
        static void Finish(string error)
        {
            EditorApplication.update -= Tick; Application.logMessageReceived -= Log;
            if (camera) camera.targetTexture = null;
            if (target) UnityEngine.Object.DestroyImmediate(target);
            SessionState.SetString(Key + ".error", error ?? "");
            File.WriteAllText(Path.Combine(Output, "play_mode.json"), JsonUtility.ToJson(new Report {
                passed = error == null, error = error, unityVersion = Application.unityVersion, checks = checks.ToArray() }, true));
            Debug.Log(error == null ? "HOME_ART_R2_VALIDATED " + checks.Count : error);
            EditorApplication.ExitPlaymode();
        }
    }
}
