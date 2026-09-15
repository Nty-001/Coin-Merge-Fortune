using UnityEngine;
namespace CoinMerge.Recovery
{
    // BaseUI.pop: source content scales from .5 to 1 over .35 seconds with backOut.
    public sealed class RecoveredMenuPopup : MonoBehaviour
    {
        public Transform content;public float duration=.35f,startScale=.5f;
        float elapsed;bool animating;
        void OnEnable(){if(!content)return;elapsed=0;animating=true;content.localScale=Vector3.one*startScale;}
        void Update(){Tick(Time.deltaTime);}
        public void Tick(float dt){if(!animating)return;elapsed+=dt;float t=Mathf.Clamp01(elapsed/duration)-1;
            float eased=1+2.70158f*t*t*t+1.70158f*t*t;content.localScale=Vector3.one*Mathf.LerpUnclamped(startScale,1,eased);if(elapsed>=duration)animating=false;}
    }
}
