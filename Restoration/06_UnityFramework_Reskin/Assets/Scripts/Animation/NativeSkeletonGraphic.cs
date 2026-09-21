using UnityEngine;
using UnityEngine.UI;
namespace CoinMerge.Recovery
{
    // Unity UGUI mesh surface for decorative skeletal UI. Gameplay coins remain world-space sprites.
    public sealed class NativeSkeletonGraphic : MaskableGraphic
    {
        NativeSkeletonPlayer player;
        Texture2D atlas;
        Vector2[] world,clipPoints;
        readonly Vector2[] triangleClip=new Vector2[3];
        ClipVertex[] bufferA,bufferB;
        struct ClipVertex {public Vector2 point,uv;}
        public override Texture mainTexture=>atlas?atlas:Texture2D.whiteTexture;
        public void Bind(NativeSkeletonPlayer owner)
        {
            player=owner;atlas=Resources.Load<Texture2D>(owner.Data.texturePath);
            if(!atlas)throw new System.InvalidOperationException("Missing skeletal atlas: "+owner.Data.texturePath);
            world=new Vector2[owner.MaxVertices];clipPoints=new Vector2[owner.MaxVertices];
            bufferA=new ClipVertex[owner.MaxVertices+8];bufferB=new ClipVertex[owner.MaxVertices+8];SetAllDirty();
        }
        protected override void OnPopulateMesh(VertexHelper output)
        {
            output.Clear();if(player==null||!player.Data)return;
            NativeAttachmentModel clipping=null;int clippingCount=0;
            for(int slotIndex=0;slotIndex<player.slots.Length;slotIndex++)
            {
                var slot=player.slots[slotIndex];int index=slot.AttachmentIndex;
                if(index<0){if(clipping!=null&&slotIndex==clipping.endSlot)clipping=null;continue;}
                var attachment=player.Data.attachments[index];
                if(attachment.type==2)
                {clipping=attachment;clippingCount=player.FillWorldVertices(index,clipPoints);continue;}
                player.FillWorldVertices(index,world);
                var a=attachment.color;Color tint=slot.Tint*new Color(a[0],a[1],a[2],a[3])*color;
                float additive=player.Data.slots[slotIndex].blend==1?1:0;
                if(tint.a>0)
                    for(int t=0;t<attachment.triangles.Length;t+=3)
                    {
                        var va=Vertex(attachment,attachment.triangles[t]);var vb=Vertex(attachment,attachment.triangles[t+1]);var vc=Vertex(attachment,attachment.triangles[t+2]);
                        if(clipping==null){Emit(output,va,vb,vc,tint,additive);continue;}
                        if(clipping.clipConvex)Clip(output,va,vb,vc,clipPoints,clippingCount,tint,additive);
                        else for(int i=0;i<clipping.clipTriangles.Length;i+=3)
                        {
                            triangleClip[0]=clipPoints[clipping.clipTriangles[i]];triangleClip[1]=clipPoints[clipping.clipTriangles[i+1]];triangleClip[2]=clipPoints[clipping.clipTriangles[i+2]];
                            Clip(output,va,vb,vc,triangleClip,3,tint,additive);
                        }
                    }
                if(clipping!=null&&slotIndex==clipping.endSlot)clipping=null;
            }
        }
        ClipVertex Vertex(NativeAttachmentModel attachment,int index)
        {return new ClipVertex {point=world[index],uv=new Vector2(attachment.uvs[index*2],attachment.uvs[index*2+1])};}
        void Clip(VertexHelper output,ClipVertex a,ClipVertex b,ClipVertex c,Vector2[] polygon,int length,Color tint,float additive)
        {
            float area=0;for(int i=0;i<length;i++)area+=Cross(polygon[i],polygon[(i+1)%length]);
            if(Mathf.Abs(area)<.00001f)return;
            float direction=area>0?1:-1;
            bufferA[0]=a;bufferA[1]=b;bufferA[2]=c;int count=3;var input=bufferA;var result=bufferB;
            for(int edge=0;edge<length&&count>0;edge++)
            {
                Vector2 start=polygon[edge],line=polygon[(edge+1)%length]-start;int next=0;
                var previous=input[count-1];float previousDistance=Cross(line,previous.point-start)*direction;
                for(int i=0;i<count;i++)
                {
                    var current=input[i];float distance=Cross(line,current.point-start)*direction;
                    bool inside=distance>=0,previousInside=previousDistance>=0;
                    if(inside!=previousInside)
                    {
                        float fraction=previousDistance/(previousDistance-distance);
                        result[next++]=new ClipVertex {point=Vector2.LerpUnclamped(previous.point,current.point,fraction),uv=Vector2.LerpUnclamped(previous.uv,current.uv,fraction)};
                    }
                    if(inside)result[next++]=current;
                    previous=current;previousDistance=distance;
                }
                count=next;var swap=input;input=result;result=swap;
            }
            for(int i=1;i<count-1;i++)Emit(output,input[0],input[i],input[i+1],tint,additive);
        }
        static float Cross(Vector2 a,Vector2 b)=>a.x*b.y-a.y*b.x;
        static void Emit(VertexHelper output,ClipVertex a,ClipVertex b,ClipVertex c,Color tint,float additive)
        {
            int first=output.currentVertCount;Add(output,a,tint,additive);Add(output,b,tint,additive);Add(output,c,tint,additive);output.AddTriangle(first,first+1,first+2);
        }
        static void Add(VertexHelper output,ClipVertex source,Color tint,float additive)
        {
            var vertex=UIVertex.simpleVert;vertex.position=source.point;vertex.uv0=source.uv;vertex.uv1=new Vector4(additive,0,0,0);vertex.color=tint;output.AddVert(vertex);
        }
    }
}
