using UnityEngine;
namespace CoinMerge.Recovery
{
    [CreateAssetMenu(menuName="Coin Merge/Lifecycle visuals")]
    public sealed class LifecycleVisualConfig : ScriptableObject
    {
        public float highestRiseTime=.22f,highestGatherTime=.28f,highestFlyTime=.55f;
        public float highestRiseFraction=.35f,highestLift=120,highestGrow=1.08f,highestGatherScale=.72f;
        public Vector2 highestArc=new Vector2(180,110);
        public float highestFinalScale=.35f,highestPulseScale=1.12f,highestPulseUp=.12f,highestPulseDown=.1f;
        public string highestAnimation="idle1";
        public float moneyAppear=.12f,moneyGrow=.1f,moneyHold=.35f,moneyStagger=.16f;
        public float moneyStartScale=.504f,moneyAppearScale=.616f,moneyScale=.7f,moneyEndRatio=.35f;
        public Vector2[] moneyStarts={new Vector2(0,14),new Vector2(-26,-10),new Vector2(26,-12)};
        public Vector2[] moneyTargets={new Vector2(0,4),new Vector2(-12,8),new Vector2(12,6)};
        public float[] moneyLifts={170,145,150};
        public float moneyMinFlight=.48f,moneyMaxFlight=.72f,moneySpeed=760,moneyPulseUp=.16f,moneyPulseDown=.12f,moneyPulseScale=1.08f;
        public float plusMove=70,plusDuration=1.8f,rewardAutoClose=1.5f,ratingDelay=1.5f,glowSpeed=-90;
        public float failureStagger=.04f,failureWarningInterval=.2f,warningMovingLimit=40;
        public int failureStaggerModulo=6;
        public float[] failureTimes={.14f,.14f,.12f,.12f,.12f,.18f};
        public Vector2[] failureScales={new Vector2(1.16f,.8f),new Vector2(.9f,1.14f),new Vector2(1.1f,.88f),new Vector2(.94f,1.08f),new Vector2(1.04f,.96f),Vector2.one};
        public Color failureTint=new Color(1,75/255f,75/255f,1);
        public float warningStep=.18f,revivePulseTime=1,revivePulseScale=1.05f;
        public string[] sounds={"Lifecycle/Audio/fly_red_bag","Lifecycle/Audio/redbag_show","Lifecycle/Audio/reward","Lifecycle/Audio/guide_redbag_show","Lifecycle/Audio/collect","Lifecycle/Audio/fail"};
        public float FailureDuration(int count)
        {if(count==0)return 0;float result=Mathf.Min(count-1,failureStaggerModulo-1)*failureStagger;foreach(float t in failureTimes)result+=t;return result;}
    }
}
