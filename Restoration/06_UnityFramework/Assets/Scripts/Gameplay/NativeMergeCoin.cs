using UnityEngine;
namespace CoinMerge.Recovery
{
    /// <summary>Unity physics scaffold; physics parity with Cocos is not yet verified.</summary>
    [RequireComponent(typeof(Rigidbody2D),typeof(CircleCollider2D))]
    public sealed class NativeMergeCoin : MonoBehaviour
    {
        public int value=1; public float pixelsPerUnit=100;
        public NativeMergeBoard board;
        bool merging;
        public void Configure(int newValue, NativeMergeBoard owner)
        {
            value=newValue;board=owner;
            GetComponent<CircleCollider2D>().radius=MergeRules.RadiusPixels[MergeRules.Index(value)]/pixelsPerUnit;
        }
        void OnCollisionEnter2D(Collision2D collision)
        {
            var other=collision.gameObject.GetComponent<NativeMergeCoin>();
            if(other==null || other.value!=value || merging || other.merging || board==null || GetInstanceID()>other.GetInstanceID())return;
            merging=other.merging=true;board.Merge(this,other);
        }
    }
}
