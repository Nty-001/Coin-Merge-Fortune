using UnityEngine;
using UnityEngine.UI;
namespace CoinMerge.Recovery
{
    // Reskin only: the recovered clip owns state, timing and completion events.
    [DefaultExecutionOrder(150)] public sealed class RecoveredVerificationBadge : MonoBehaviour
    {
        public NativeSkeletonPlayer source;
        public Image waiting,complete;
        public int completedSlot=5,pendingSlot=6,processingSlot=7,processingBone=2;
        public float rotationOffset=88.9415f;
        void LateUpdate()
        {
            var done=source.slots[completedSlot];float a=done.AttachmentIndex>=0?Mathf.Clamp01(done.a):0;
            complete.color=new Color(1,1,1,a);
            var pending=source.slots[pendingSlot];var processing=source.slots[processingSlot];
            float b=Mathf.Max(pending.AttachmentIndex>=0?pending.a:0,processing.AttachmentIndex>=0?processing.a:0);
            waiting.color=new Color(1,1,1,Mathf.Clamp01(b)*(1-a));
            var bone=source.bones[processingBone];waiting.rectTransform.localRotation=Quaternion.Euler(0,0,bone.rotation-rotationOffset);
        }
    }
}
