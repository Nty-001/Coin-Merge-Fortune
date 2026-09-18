using UnityEngine;
using UnityEngine.UI;
namespace CoinMerge.Recovery
{
    // Presentation-only UGUI effect. Rebuilds only when the native Text mesh changes.
    [AddComponentMenu("UI/Effects/Recovered Text Vertical Gradient")]
    public sealed class RecoveredTextVerticalGradient : BaseMeshEffect
    {
        public Color top=Color.white,bottom=new Color(.7f,.9f,1);
        public override void ModifyMesh(VertexHelper helper)
        {
            if(!IsActive()||helper.currentVertCount==0)return;
            var vertex=new UIVertex();float low=float.MaxValue,high=float.MinValue;
            Color32 source=graphic.color;
            for(int i=0;i<helper.currentVertCount;i++)
            {
                helper.PopulateUIVertex(ref vertex,i);
                if(vertex.color.r!=source.r||vertex.color.g!=source.g||vertex.color.b!=source.b)continue;
                low=Mathf.Min(low,vertex.position.y);high=Mathf.Max(high,vertex.position.y);
            }
            float height=high-low;if(height<=0)return;
            for(int i=0;i<helper.currentVertCount;i++)
            {
                helper.PopulateUIVertex(ref vertex,i);
                if(vertex.color.r!=source.r||vertex.color.g!=source.g||vertex.color.b!=source.b)continue;
                Color32 tint=Color.Lerp(bottom,top,(vertex.position.y-low)/height);tint.a=vertex.color.a;vertex.color=tint;helper.SetUIVertex(vertex,i);
            }
        }
    }
}
