using UnityEngine;
namespace CoinMerge.Recovery
{
    public sealed class NativeMergeBoard : MonoBehaviour
    {
        public NativeMergeCoin coinPrefab; public int score;
        public NativeMergeCoin Spawn(int value, Vector3 position)
        {
            var coin=Instantiate(coinPrefab,position,Quaternion.identity,transform);coin.Configure(value,this);return coin;
        }
        public void Merge(NativeMergeCoin first, NativeMergeCoin second)
        {
            var position=(first.transform.position+second.transform.position)*.5f;
            int value=MergeRules.Upgrade(first.value);
            Destroy(first.gameObject);Destroy(second.gameObject);
            Spawn(value,position);score+=MergeRules.MergeScores[MergeRules.Index(value)];
        }
    }
}
