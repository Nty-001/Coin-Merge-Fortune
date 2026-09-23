using System;
using UnityEngine;

namespace CoinMerge.Recovery
{
    // Animates authored decorative Images; never spawns gameplay coins or delays startup.
    public sealed class LoadingCoinBurst : MonoBehaviour
    {
        [Serializable] public sealed class Flight
        {
            public RectTransform rect;
            public CanvasGroup opacity;
            public Vector2 destination;
            [Min(0)] public float delay;
            public float initialAngle;
        }

        public Flight[] flights = Array.Empty<Flight>();
        public Vector2 origin;
        [Min(.01f)] public float duration = .76f;
        [Min(0)] public float bobAmplitude = 4;
        [Min(0)] public float bobFrequency = 1.8f;
        float elapsed;

        void OnEnable() { elapsed = 0; EvaluateAt(0); }
        void Update() { elapsed += Time.unscaledDeltaTime; EvaluateAt(elapsed); }

        public void EvaluateAt(float time)
        {
            for (int i = 0; i < flights.Length; i++)
            {
                var flight = flights[i];
                if (!flight.rect || !flight.opacity) continue;
                float t = Mathf.Clamp01((time - flight.delay) / Mathf.Max(.01f, duration));
                float remaining = 1 - t;
                float travel = 1 - remaining * remaining * remaining;
                float after = Mathf.Max(0, time - flight.delay - duration);
                float bob = Mathf.Sin(after * bobFrequency) * bobAmplitude;
                flight.rect.anchoredPosition = Vector2.LerpUnclamped(origin, flight.destination, travel) + new Vector2(0, bob);
                flight.rect.localScale = Vector3.one * Mathf.Lerp(.16f, 1, travel);
                flight.rect.localRotation = Quaternion.Euler(0, 0, flight.initialAngle * remaining * remaining);
                flight.opacity.alpha = Mathf.Clamp01(t * 6);
            }
        }
    }
}
