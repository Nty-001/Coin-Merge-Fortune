using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace CoinMerge.Recovery
{
    /// <summary>Authored outline, short extrusion and edge highlight for live popup labels.</summary>
    [AddComponentMenu("UI/Effects/Popup Lettering")]
    public sealed class RecoveredPopupLettering : BaseMeshEffect
    {
        public Color outline=new Color(.02f,.1f,.35f),depthColor=new Color(.01f,.05f,.2f),highlight=new Color(1,1,1,.8f);
        [Min(0)] public float radius=4,depth=4,edge=.8f;
        readonly List<UIVertex> source=new List<UIVertex>();
        readonly List<UIVertex> output=new List<UIVertex>();
        public override void ModifyMesh(VertexHelper mesh)
        {
            if(!IsActive()||mesh.currentVertCount==0)return;
            source.Clear();output.Clear();mesh.GetUIVertexStream(source);
            int capacity=source.Count*26;if(output.Capacity<capacity)output.Capacity=capacity;
            Ring(depthColor,new Vector2(0,-depth));Ring(outline,Vector2.zero);
            Copy(new Vector2(0,edge),highlight);output.AddRange(source);mesh.Clear();mesh.AddUIVertexTriangleStream(output);
        }
        void Ring(Color tint,Vector2 shift)
        {for(int i=0;i<12;i++){float a=i*Mathf.PI/6;Copy(shift+new Vector2(Mathf.Cos(a),Mathf.Sin(a))*radius,tint);}}
        void Copy(Vector2 shift,Color tint)
        {
            for(int i=0;i<source.Count;i++)
            {
                var v=source[i];v.position+=new Vector3(shift.x,shift.y,0);Color32 c=tint;c.a=(byte)(v.color.a*tint.a);v.color=c;output.Add(v);
            }
        }
    }
}
