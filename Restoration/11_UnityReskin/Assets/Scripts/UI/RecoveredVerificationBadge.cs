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
        public bool smoothMotion;
        public float rotationSpeed=150,blendTime=.12f,completedStartScale=.94f;
        float waitingAlpha,completeAlpha,waitingVelocity,completeVelocity,angle;
        void OnEnable()
        {
            waitingAlpha=completeAlpha=waitingVelocity=completeVelocity=angle=0;
            if(!smoothMotion)return;
            waiting.color=complete.color=new Color(1,1,1,0);
            waiting.rectTransform.localRotation=Quaternion.identity;
            complete.rectTransform.localScale=Vector3.one*completedStartScale;
        }
        void LateUpdate()
        {
            var done=source.slots[completedSlot];float a=done.AttachmentIndex>=0?Mathf.Clamp01(done.a):0;
            var pending=source.slots[pendingSlot];var processing=source.slots[processingSlot];
            float b=Mathf.Max(pending.AttachmentIndex>=0?pending.a:0,processing.AttachmentIndex>=0?processing.a:0);
            if(smoothMotion)
            {
                float dt=Time.deltaTime;
                completeAlpha=Mathf.SmoothDamp(completeAlpha,a,ref completeVelocity,blendTime,Mathf.Infinity,dt);
                waitingAlpha=Mathf.SmoothDamp(waitingAlpha,Mathf.Clamp01(b)*(1-a),ref waitingVelocity,blendTime,Mathf.Infinity,dt);
                complete.color=new Color(1,1,1,completeAlpha);waiting.color=new Color(1,1,1,waitingAlpha);
                complete.rectTransform.localScale=Vector3.one*Mathf.Lerp(completedStartScale,1,completeAlpha);
                angle=Mathf.Repeat(angle-rotationSpeed*dt,360);waiting.rectTransform.localRotation=Quaternion.Euler(0,0,angle);
                return;
            }
            complete.color=new Color(1,1,1,a);
            waiting.color=new Color(1,1,1,Mathf.Clamp01(b)*(1-a));
            var bone=source.bones[processingBone];waiting.rectTransform.localRotation=Quaternion.Euler(0,0,bone.rotation-rotationOffset);
        }
    }
}
