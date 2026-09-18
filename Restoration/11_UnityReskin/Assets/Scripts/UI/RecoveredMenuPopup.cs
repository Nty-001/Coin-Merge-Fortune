using UnityEngine;
namespace CoinMerge.Recovery
{
    // BaseUI.pop is preserved by default; the verification page opts into a gentle fade/ease.
    public sealed class RecoveredMenuPopup : MonoBehaviour
    {
        public Transform content;public float duration=.35f,startScale=.5f;
        public bool smoothMotion;
        public CanvasGroup opacity;
        [Tooltip("Optional authored visual envelope for tall modal panels; full-screen pages keep width fit.")]
        public Vector2 fittedVisualSize;
        public RectTransform availableFrame;
        public float fitMargin=12;
        float elapsed;bool animating;
        void OnEnable(){if(!content)return;elapsed=0;animating=true;content.localScale=Vector3.one*startScale;if(smoothMotion&&opacity)opacity.alpha=0;}
        void Update(){Tick(Time.deltaTime);}
        float FitScale()
        {
            if(!availableFrame||fittedVisualSize.x<=0||fittedVisualSize.y<=0)return 1;
            var area=availableFrame.rect;
            return Mathf.Clamp01(Mathf.Min((area.width-2*fitMargin)/fittedVisualSize.x,(area.height-2*fitMargin)/fittedVisualSize.y));
        }
        public void Tick(float dt){if(!content)return;if(!animating){if(availableFrame)content.localScale=Vector3.one*FitScale();return;}elapsed+=dt;float t=Mathf.Clamp01(elapsed/duration)-1;
            float eased=smoothMotion?1+t*t*t:1+2.70158f*t*t*t+1.70158f*t*t;
            content.localScale=Vector3.one*(FitScale()*Mathf.LerpUnclamped(startScale,1,eased));
            if(smoothMotion&&opacity)opacity.alpha=Mathf.SmoothStep(0,1,Mathf.Clamp01(elapsed/duration));
            if(elapsed>=duration)animating=false;}
    }
}
