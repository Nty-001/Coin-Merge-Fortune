using UnityEngine;
using UnityEngine.UI;
namespace CoinMerge.Recovery
{
    // Uses the project's native UI shader's per-vertex additive channel.
    public sealed class MergeStarImage : Image
    {
        public bool additive;
        protected override void OnPopulateMesh(VertexHelper output)
        {
            base.OnPopulateMesh(output);if(!additive)return;
            var vertex=UIVertex.simpleVert;
            for(int i=0;i<output.currentVertCount;i++){output.PopulateUIVertex(ref vertex,i);vertex.uv1=new Vector4(1,0,0,0);output.SetUIVertex(vertex,i);}
        }
    }
}
