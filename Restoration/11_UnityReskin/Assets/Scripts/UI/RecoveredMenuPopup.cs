using UnityEngine;
namespace CoinMerge.Recovery
{
    // BaseUI.pop is preserved by default; the verification page opts into a gentle fade/ease.
    public sealed class RecoveredMenuPopup : MonoBehaviour
    {
        public Transform content;public float duration=.35f,startScale=.5f;
        public bool smoothMotion;
        public CanvasGroup opacity;
        float elapsed;bool animating;
        void OnEnable(){if(!content)return;elapsed=0;animating=true;content.localScale=Vector3.one*startScale;if(smoothMotion&&opacity)opacity.alpha=0;}
        void Update(){Tick(Time.deltaTime);}
        public void Tick(float dt){if(!animating)return;elapsed+=dt;float t=Mathf.Clamp01(elapsed/duration)-1;
            float eased=smoothMotion?1+t*t*t:1+2.70158f*t*t*t+1.70158f*t*t;
            content.localScale=Vector3.one*Mathf.LerpUnclamped(startScale,1,eased);
            if(smoothMotion&&opacity)opacity.alpha=Mathf.SmoothStep(0,1,Mathf.Clamp01(elapsed/duration));
            if(elapsed>=duration)animating=false;}
    }
}
