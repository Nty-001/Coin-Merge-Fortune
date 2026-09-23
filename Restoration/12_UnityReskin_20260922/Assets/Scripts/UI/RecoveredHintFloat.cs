using UnityEngine;
namespace CoinMerge.Recovery
{
    // GameScene.setLabel: sineInOut to y+10 in 1s, y-10 in 2s, then y in 1s.
    [RequireComponent(typeof(RectTransform))]
    public sealed class RecoveredHintFloat : MonoBehaviour
    {
        public Vector2 restPosition;
        public float amplitude=10,riseTime=1,fallTime=2,returnTime=1;
        RectTransform target;float elapsed;
        public float Offset=>target?target.anchoredPosition.y-restPosition.y:0;
        void Awake(){target=(RectTransform)transform;}
        void OnEnable(){if(!target)target=(RectTransform)transform;elapsed=0;target.anchoredPosition=restPosition;}
        void Update(){Tick(Time.deltaTime);}
        public void Tick(float dt)
        {
            elapsed=Mathf.Repeat(elapsed+Mathf.Max(0,dt),riseTime+fallTime+returnTime);
            float y=elapsed<riseTime?Mathf.Lerp(0,amplitude,Ease(elapsed/riseTime)):
                elapsed<riseTime+fallTime?Mathf.Lerp(amplitude,-amplitude,Ease((elapsed-riseTime)/fallTime)):
                Mathf.Lerp(-amplitude,0,Ease((elapsed-riseTime-fallTime)/returnTime));
            target.anchoredPosition=restPosition+Vector2.up*y;
        }
        static float Ease(float t){return .5f-.5f*Mathf.Cos(Mathf.PI*Mathf.Clamp01(t));}
        void OnDisable(){if(target)target.anchoredPosition=restPosition;}
    }
}
