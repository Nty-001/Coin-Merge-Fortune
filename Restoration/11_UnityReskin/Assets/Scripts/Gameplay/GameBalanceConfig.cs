using UnityEngine;
namespace CoinMerge.Recovery
{
    [CreateAssetMenu(menuName="Coin Merge/Recovered balance configuration")]
    public sealed class GameBalanceConfig : ScriptableObject
    {
        public RecoveredRulesData rules;
        [Min(1)] public float pixelsPerUnit=32;
        [Min(8)] public int initialCoinPoolCapacity=64;
        [Min(1)] public int velocityIterations=10,positionIterations=10;
        [Range(.005f,.02f)] public float physicsStep=1f/60f;
        [Range(1,12)] public int maxPhysicsStepsPerFrame=6;
        public string defaultCountry="US",defaultCohort="B";
        public bool defaultRewardedVariant=true;
        public RecoveredWheelTimings wheel=new RecoveredWheelTimings();
    }
}
