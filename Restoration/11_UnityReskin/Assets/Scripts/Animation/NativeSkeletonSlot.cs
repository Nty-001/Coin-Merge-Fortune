using UnityEngine;
namespace CoinMerge.Recovery
{
    public sealed class NativeSkeletonSlot : MonoBehaviour
    {
        public float r=1,g=1,b=1,a=1,attachment=-1;
        public int AttachmentIndex=>Mathf.RoundToInt(attachment);
        public Color Tint=>new Color(r,g,b,a);
        [System.NonSerialized] public float[] deform;
        [System.NonSerialized] public int deformAttachment=-1;
    }
}
