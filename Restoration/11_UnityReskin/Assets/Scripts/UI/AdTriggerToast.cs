using UnityEngine;
using UnityEngine.UI;

namespace CoinMerge.Recovery
{
    // A request hint, not proof of an SDK impression or a completed video.
    // Authored above modal canvases; survives their immediate mock callbacks.
    public sealed class AdTriggerToast : MonoBehaviour
    {
        public RecoveredGameSession session;
        public CanvasGroup group;
        public RectTransform card;
        public Text label;
        public string rewardedMessage = "此处触发了激励广告";
        public string interstitialMessage = "此处触发了插屏广告";
        [Min(.5f)] public float duration = 2.8f;
        [Min(.01f)] public float fadeIn = .15f, fadeOut = .45f;
        public float rise = 32;
        Vector2 origin;
        float elapsed;
        bool showing;
        void Awake() { origin = card.anchoredPosition; group.alpha = 0; }
        void OnEnable() { session.Sdk.AdRequested += OnAdRequested; }
        void OnDisable()
        {
            session.Sdk.AdRequested -= OnAdRequested;
            showing = false; group.alpha = 0; card.anchoredPosition = origin;
        }
        void OnAdRequested(AdKind kind, string placement)
        {
            if (session.Profile == null || !session.Profile.rewardedVariant) return;
            label.text = kind == AdKind.Interstitial ? interstitialMessage : rewardedMessage;
            elapsed = 0; showing = true; group.alpha = 0; card.anchoredPosition = origin;
        }
        void Update()
        {
            if (!showing) return;
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            card.anchoredPosition = origin + Vector2.up * (rise * Mathf.SmoothStep(0, 1, t));
            group.alpha = Mathf.Min(Mathf.Clamp01(elapsed / fadeIn), Mathf.Clamp01((duration - elapsed) / fadeOut));
            if (elapsed >= duration) { showing = false; group.alpha = 0; }
        }
    }
}
