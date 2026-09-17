using UnityEngine;
namespace CoinMerge.Recovery
{
    // The recovered LaoHuJ_TX clip is the only animation clock. Artwork follows its ND_00 pose.
    [DefaultExecutionOrder(100)]
    public sealed class RecoveredWheelLever : MonoBehaviour
    {
        public NativeSkeletonBone sourceKnob;
        public RectTransform knob,shaft;
        public Vector2 knobRest,shaftRest;
        public float sourceRestY,sourcePressedY,travel,shaftCompressedScale;
        public float PressedFraction {get;private set;}
        void LateUpdate(){ApplyPose();}
        public void ApplyPose()
        {
            PressedFraction=Mathf.InverseLerp(sourceRestY,sourcePressedY,sourceKnob.y);
            knob.anchoredPosition=knobRest+Vector2.down*(PressedFraction*travel);
            knob.localScale=Vector3.one*sourceKnob.scaleX; // Ball remains round; original pulse is uniform.
            shaft.anchoredPosition=shaftRest;
            shaft.localScale=new Vector3(1,Mathf.Lerp(1,shaftCompressedScale,PressedFraction),1);
        }
    }
}
