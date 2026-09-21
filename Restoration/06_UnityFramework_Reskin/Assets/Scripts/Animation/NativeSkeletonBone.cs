using UnityEngine;
namespace CoinMerge.Recovery
{
    // Authored bone pose channels are animated by a native Unity AnimationClip.
    public sealed class NativeSkeletonBone : MonoBehaviour
    {
        public float x,y,rotation,scaleX=1,scaleY=1,shearX,shearY;
        public int parentIndex=-1;
        public float A {get;private set;}
        public float B {get;private set;}
        public float C {get;private set;}
        public float D {get;private set;}
        public float WorldX {get;private set;}
        public float WorldY {get;private set;}
        public void Evaluate(NativeSkeletonBone parent)
        {
            float rx=(rotation+shearX)*Mathf.Deg2Rad,ry=(rotation+90+shearY)*Mathf.Deg2Rad;
            float a=Mathf.Cos(rx)*scaleX,b=Mathf.Cos(ry)*scaleY,c=Mathf.Sin(rx)*scaleX,d=Mathf.Sin(ry)*scaleY;
            if(parent==null){A=a;B=b;C=c;D=d;WorldX=x;WorldY=y;return;}
            A=parent.A*a+parent.B*c;B=parent.A*b+parent.B*d;
            C=parent.C*a+parent.D*c;D=parent.C*b+parent.D*d;
            WorldX=parent.A*x+parent.B*y+parent.WorldX;WorldY=parent.C*x+parent.D*y+parent.WorldY;
        }
        public Vector2 TransformPoint(float px,float py)=>new Vector2(A*px+B*py+WorldX,C*px+D*py+WorldY);
    }
}
