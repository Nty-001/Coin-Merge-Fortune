using UnityEngine;
namespace CoinMerge.Recovery
{
    /// <summary>Lossless source metadata, not a replacement for the original behavior.</summary>
    public sealed class RecoveredNode : MonoBehaviour
    {
        public string sourceUuid, variant;
        public int sourceObjectId;
        [TextArea(2,12)] public string originalNodeJson;
        public ComponentModel[] originalComponents;
    }
}
