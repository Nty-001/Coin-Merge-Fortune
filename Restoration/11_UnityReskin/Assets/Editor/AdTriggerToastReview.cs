using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace CoinMerge.Recovery.Editor
{
    [InitializeOnLoad]
    public static class AdTriggerToastReview
    {
        const string Key = "AdTriggerToastReview.running", Prefix = "coinmerge.adhints.disposable", Output = "../Android/AdHintsReview";
        static RecoveredGameSession session;
        static AdTriggerToast hint;
        static int phase, checks, result;
        static double next;
        static RenderTexture surface;
        static AdTriggerToastReview() { EditorApplication.playModeStateChanged += State; }
        public static void Run()
        {
            Directory.CreateDirectory(Output); EditorSceneManager.OpenScene("Assets/Scenes/RecoveredMain.unity");
            var s = UnityEngine.Object.FindObjectOfType<RecoveredGameSession>(); s.saveNamespace = Prefix; s.automaticInput = false;
            var store = new PlayerStore(Prefix); store.ResetPlayer(); store.ResetProfile();
            store.Save(new PlayerProgress { guideStep = 9999, fakeMoney = 220 });
            store.SaveProfile(new VersionProfile { country = "US", cohort = "B", rewardedVariant = true, contentMode = 2, cohortMode = 2 });
            SessionState.SetBool(Key, true); EditorApplication.EnterPlaymode();
        }
        static void State(PlayModeStateChange state)
        {
            if (!SessionState.GetBool(Key, false)) return;
            if (state == PlayModeStateChange.EnteredPlayMode) { phase = checks = 0; next = EditorApplication.timeSinceStartup + 1; EditorApplication.update += Tick; }
            if (state == PlayModeStateChange.EnteredEditMode)
            {
                SessionState.SetBool(Key, false); var store = new PlayerStore(Prefix); store.ResetPlayer(); store.ResetProfile(); EditorApplication.Exit(result);
            }
        }
        static void Require(bool condition, string message) { if (!condition) throw new Exception(message); checks++; }
        static void Tick()
        {
            if (EditorApplication.timeSinceStartup < next) return;
            next = EditorApplication.timeSinceStartup + .6;
            try
            {
                switch (phase++)
                {
                    case 0:
                        session = UnityEngine.Object.FindObjectOfType<RecoveredGameSession>(); hint = session.GetComponentInChildren<AdTriggerToast>();
                        Require(hint && hint.group.alpha == 0, "Hint starts hidden");
                        Require(!hint.group.blocksRaycasts && !hint.label.raycastTarget, "Hint never blocks Buttons");
                        foreach (char c in hint.rewardedMessage + hint.interstitialMessage) Require(hint.label.font.HasCharacter(c), "Missing Chinese glyph: " + c);
                        session.worldCamera.GetComponent<DeviceSafeViewport>().enabled = false;
                        surface = new RenderTexture(750, 1624, 24); session.worldCamera.targetTexture = surface; session.worldCamera.rect = new Rect(0, 0, 1, 1);
                        foreach (var scaler in session.worldCamera.GetComponent<DeviceSafeViewport>().scalers) scaler.scaleFactor = 1;
                        session.ShowReward(2); Require(hint.label.text.Length == 0, "Reward preview must not claim an ad was requested"); session.rewardView.gameObject.SetActive(false);
                        session.GmPrepareDrop(22); session.board.InputBlocked = false;
                        Require(session.board.RequestDrop(), "Real drop accepted"); next = EditorApplication.timeSinceStartup + 1.2; break;
                    case 1:
                        Require(session.Sdk.Trace.Contains("ad.request:1_A"), "22nd drop requests 1_A");
                        Require(hint.label.text == hint.rewardedMessage && hint.group.alpha > .5f, "Double reward hint visible above popup"); Capture("double-reward.png");
                        session.rewardView.mask.onClick.Invoke(); session.board.TriggerFailure(); next = EditorApplication.timeSinceStartup + 1.5; break;
                    case 2:
                        Require(session.failView.gameObject.activeSelf, "Failure popup opened");
                        int watched = session.Player.watch_video_count;
                        session.Sdk.NextAdOutcome = AdOutcome.Unavailable; session.failView.revive.onClick.Invoke();
                        Require(session.Player.watch_video_count == watched && session.failView.gameObject.activeSelf, "Unavailable ad still keeps original failure flow");
                        session.Sdk.NextAdOutcome = AdOutcome.Completed; session.failView.revive.onClick.Invoke();
                        Require(session.Player.watch_video_count == watched + 1 && session.Sdk.Trace.Contains("ad.request:3_A"), "Successful revive still counts once"); break;
                    case 3:
                        Require(hint.group.alpha > .5f && hint.label.text == hint.rewardedMessage, "Revive hint survives instant mock callback"); Capture("revive.png");
                        session.rewardView.mask.onClick.Invoke(); next = EditorApplication.timeSinceStartup + 3.2; break;
                    case 4:
                        Require(hint.group.alpha == 0, "Hint fades out");
                        session.Player.currentLotteryCount = session.board.Config.rules.flow.drawRewardStrong;
                        session.wheelRewardView.Show(0, session.Player, session.board.Config, session.Locale, .5);
                        Require(session.wheelRewardView.RequiresAd, "Draw reward needs ad");
                        session.wheelRewardView.claim.onClick.Invoke(); int traces = session.Sdk.Trace.Count;
                        session.wheelRewardView.claim.onClick.Invoke();
                        Require(session.Sdk.Trace.Count == traces && session.Sdk.Trace.Contains("ad.request:2_A"), "Draw claim requests once; repeated claim does not"); break;
                    case 5:
                        Require(hint.group.alpha > .5f && hint.label.text == hint.rewardedMessage, "Draw hint visible"); Capture("draw-reward.png");
                        watched = session.Player.watch_video_count;
                        session.Sdk.ShowInterstitial("validation_only");
                        Require(hint.label.text == hint.interstitialMessage && session.Player.watch_video_count == watched, "Interstitial boundary is distinct and never adds rewarded count");
                        Time.timeScale = 0; break;
                    case 6:
                        Require(hint.group.alpha > .5f, "Animation uses unscaled time"); Capture("interstitial-interface-only.png"); next = EditorApplication.timeSinceStartup + 3.2; break;
                    case 7:
                        Require(hint.group.alpha == 0, "Hint expires even when paused"); Time.timeScale = 1;
                        session.ChangeProfile("US", "A", false); hint.label.text = ""; session.Sdk.ShowRewarded("validation_a_only");
                        Require(hint.label.text.Length == 0, "A version presentation unchanged"); Finish(null); break;
                }
            }
            catch (Exception e) { Finish(e.ToString()); }
        }
        static void Capture(string name)
        {
            Canvas.ForceUpdateCanvases();
            var backdrop = session.transform.Find("SafeAreaBackdrop").GetComponent<Camera>();
            var previousTarget = backdrop.targetTexture; backdrop.targetTexture = surface; backdrop.Render(); backdrop.targetTexture = previousTarget;
            session.worldCamera.Render(); var old = RenderTexture.active; RenderTexture.active = surface;
            var image = new Texture2D(surface.width, surface.height, TextureFormat.RGB24, false); image.ReadPixels(new Rect(0, 0, surface.width, surface.height), 0, 0); image.Apply();
            File.WriteAllBytes(Output + "/" + name, image.EncodeToPNG()); UnityEngine.Object.DestroyImmediate(image); RenderTexture.active = old;
        }
        static void Finish(string error)
        {
            EditorApplication.update -= Tick; Time.timeScale = 1; result = error == null ? 0 : 1;
            if (session) session.worldCamera.targetTexture = null; if (surface) UnityEngine.Object.DestroyImmediate(surface);
            File.WriteAllText(Output + "/result.txt", (error ?? "PASS") + "\nChecks: " + checks + "\nInterstitial exercised at SDK boundary only; no gameplay trigger added.");
            EditorApplication.ExitPlaymode();
        }
    }
}
