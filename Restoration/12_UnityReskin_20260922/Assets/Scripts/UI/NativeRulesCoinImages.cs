using System;
using UnityEngine;
using UnityEngine.UI;

namespace CoinMerge.Recovery
{
    [Serializable] public sealed class NativeRulesCoinBinding
    {
        public Image image;
        public NativeSkeletonBone bone;
        public NativeSkeletonSlot slot;
    }

    // High-resolution UI artwork follows the recovered native skeleton's pose and reveal channels.
    [DefaultExecutionOrder(200)]
    public sealed class NativeRulesCoinImages : MonoBehaviour
    {
        public NativeRulesCoinBinding[] coins=Array.Empty<NativeRulesCoinBinding>();
        void LateUpdate(){Refresh();}
        public void Refresh()
        {
            foreach(var c in coins)
            {
                var b=c.bone;var r=c.image.rectTransform;
                r.anchoredPosition=new Vector2(b.WorldX,b.WorldY);
                r.localRotation=Quaternion.Euler(0,0,Mathf.Atan2(b.C,b.A)*Mathf.Rad2Deg);
                r.localScale=new Vector3(Mathf.Sqrt(b.A*b.A+b.C*b.C),Mathf.Sqrt(b.B*b.B+b.D*b.D),1);
                c.image.color=c.slot.Tint;
                c.image.enabled=c.slot.AttachmentIndex>=0;
            }
        }
    }
}
