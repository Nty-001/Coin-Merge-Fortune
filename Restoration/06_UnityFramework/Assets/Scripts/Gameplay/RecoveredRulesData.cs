using System;
namespace CoinMerge.Recovery
{
    [Serializable] public sealed class DropRule { public int threshold; public int[] values; public double[] weights; }
    [Serializable] public sealed class LotteryScoreRule { public int lotteryCount; public int requiredScore; }
    [Serializable] public sealed class LotteryReward { public int index; public string type; public double amount; public double probability; }
    [Serializable] public sealed class CashRange { public double le,lh,ratio; }
    [Serializable] public sealed class CashFluctuation { public double min,max; }
    [Serializable] public sealed class WithdrawProduct
    {
        public int id,condition_merge,condition_daily_merge,condition_login_days,condition_video,condition_ad;
        public double condition_coin,withdrawAmount;
    }
    [Serializable] public sealed class CashGroup
    {
        public string id;
        public string[] countries;
        public double baseMinReward,newGuideReward,guideMoney;
        public CashRange[] ranges;
        public CashFluctuation fluctuation;
        public WithdrawProduct[] real_products,new_Fake_products;
    }
    [Serializable] public sealed class VideoRewardRule { public int min,max; public double qMin,qMax; }
    [Serializable] public sealed class CoinDefinition
    {
        public int value,upgrade,mergeScore;
        public float radiusPixels,visualWidthPixels,visualHeightPixels,dropDamping;
        public string spritePath;
    }
    [Serializable] public sealed class PhysicsRules
    {
        public float gravityPixels,mergeSensorExtraRadius,dropGravityBase,dropGravityDistanceBonus,settleGravity;
        public float dropSpeedBase,dropSpeedDistanceBonus,dropSpeedFallback,minEffectiveDropDistance,dropDistanceRange;
        public float dropDampingMultiplier,minimumDropDamping,settleLinearDamping,contactSettleDelay,angularDamping;
        public float density,densityRadiusBase,densityMin,densityMax,restitution,friction;
    }
    [Serializable] public sealed class FlowRules
    {
        public int[] initialBottomValues;
        public int firstRewardDrop,secondRewardDrop,adRewardDrop,drawRewardStrong,validLoginMergeCount;
        public float saveInterval,previewDelay,spawnDuration,spawnInitialScale,spawnUnlockBuffer,popupDelay,idleGuideDelay;
        public float failStillVelocity,failStillAngularVelocity,failStillDuration,failMaxWait,failAnimationDuration;
        public float mergeSpawnLiftPixels;
    }
    [Serializable] public sealed class RecoveredRulesData
    {
        public string evidence;
        public string[] supportedCountries;
        public DropRule[] drops;
        public LotteryScoreRule[] lotteryScores;
        public LotteryReward[] lotteryRewards;
        public CashGroup[] cashGroups;
        public VideoRewardRule[] videoRewards;
        public CoinDefinition[] coins;
        public PhysicsRules physics;
        public FlowRules flow;
    }
}
