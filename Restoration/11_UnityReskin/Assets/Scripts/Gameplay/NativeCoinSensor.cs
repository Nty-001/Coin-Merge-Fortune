using UnityEngine;
namespace CoinMerge.Recovery
{
    // Reject sensor/sensor pairs exactly as CoinItem.onBeginContact does.
    public sealed class NativeCoinSensor : MonoBehaviour
    {
        public NativeMergeCoin coin;
        void OnTriggerEnter2D(Collider2D other)
        {if(!other.isTrigger&&coin.IsAlive)coin.Board.TryMergeContact(coin,other);}
    }
}
