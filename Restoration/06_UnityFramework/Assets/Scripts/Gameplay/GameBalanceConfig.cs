using UnityEngine;
namespace CoinMerge.Recovery
{
    [CreateAssetMenu(menuName="Coin Merge/Recovered balance configuration")]
    public sealed class GameBalanceConfig : ScriptableObject
    {
        public RecoveredRulesData rules;
        [Min(1)] public float pixelsPerUnit=100;
        [Min(8)] public int initialCoinPoolCapacity=64;
    }
}
