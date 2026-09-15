using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace CoinMerge.Recovery
{
    // Native UGUI mesh effect; Outline's public API and serialized references stay compatible.
    // Cocos stroke width is a radius. Four diagonal copies produce a larger, visibly forked contour.
    [AddComponentMenu("UI/Effects/Recovered Round Outline")]
    public sealed class RecoveredRoundOutline : Outline
    {
        [SerializeField,Range(8,24)] int samples=16;
        readonly List<UIVertex> vertices=new List<UIVertex>();
        public override void ModifyMesh(VertexHelper helper)
        {
            if(!IsActive())return;
            vertices.Clear();helper.GetUIVertexStream(vertices);
            int count=vertices.Count;if(count==0)return;
            int directions=Mathf.Clamp(samples,8,24);
            int capacity=count*(directions+1);if(vertices.Capacity<capacity)vertices.Capacity=capacity;
            float radius=Mathf.Max(Mathf.Abs(effectDistance.x),Mathf.Abs(effectDistance.y));
            int begin=0,end=count;
            for(int i=0;i<directions;i++)
            {
                float angle=i*(Mathf.PI*2/directions);
                ApplyShadowZeroAlloc(vertices,effectColor,begin,end,Mathf.Cos(angle)*radius,Mathf.Sin(angle)*radius);
                begin=end;end=vertices.Count;
            }
            helper.Clear();helper.AddUIVertexTriangleStream(vertices);vertices.Clear();
        }
    }
}
