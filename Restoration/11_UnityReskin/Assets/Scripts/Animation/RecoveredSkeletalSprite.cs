using UnityEngine;
using UnityEngine.UI;
namespace CoinMerge.Recovery
{
    // Replaces the final C1 region in GuangH_TX; the existing native clip still animates its bone and tint.
    [DefaultExecutionOrder(100)]
    public sealed class RecoveredSkeletalSprite : MonoBehaviour
    {
        public NativeSkeletonPlayer player;
        public int boneIndex,slotIndex;
        public Image image;
        public string resourcePath;
        void OnEnable(){image.sprite=Resources.Load<Sprite>(resourcePath);ApplyPose();}
        void OnDisable(){image.sprite=null;}
        void LateUpdate(){ApplyPose();}
        public void ApplyPose()
        {
            var bone=player.bones[boneIndex];var slot=player.slots[slotIndex];
            var rect=image.rectTransform;
            rect.anchoredPosition=new Vector2(bone.WorldX,bone.WorldY);
            rect.localRotation=Quaternion.Euler(0,0,Mathf.Atan2(bone.C,bone.A)*Mathf.Rad2Deg);
            float x=Mathf.Sqrt(bone.A*bone.A+bone.C*bone.C),y=Mathf.Sqrt(bone.B*bone.B+bone.D*bone.D);
            if(bone.A*bone.D-bone.B*bone.C<0)y=-y;
            rect.localScale=new Vector3(x,y,1);
            image.color=slot.Tint;image.enabled=slot.AttachmentIndex>=0;
        }
    }
}
