using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace CoinMerge.Recovery {
 [AddComponentMenu("UI/Effects/Approved Text Depth")]
 public sealed class ApprovedTextDepth : BaseMeshEffect {
  public Color top=Color.white,bottom=new Color(.77f,.91f,1),shadow=new Color(0,.16f,.5f,.6f);public float depth=4;
  readonly List<UIVertex> source=new List<UIVertex>();readonly List<UIVertex> output=new List<UIVertex>();
  public override void ModifyMesh(VertexHelper vh){if(!IsActive())return;source.Clear();vh.GetUIVertexStream(source);if(source.Count==0)return;
   float lo=float.MaxValue,hi=float.MinValue;Color32 face=graphic.color;
   foreach(var v in source)if(v.color.Equals(face)){lo=Mathf.Min(lo,v.position.y);hi=Mathf.Max(hi,v.position.y);}
   output.Clear();foreach(var vertex in source){var v=vertex;v.position+=new Vector3(0,-depth,0);v.color=shadow;output.Add(v);}
   foreach(var vertex in source){var v=vertex;if(v.color.Equals(face)){float k=Mathf.InverseLerp(lo,hi,v.position.y);v.color=Color.Lerp(bottom,top,k);}output.Add(v);}
   vh.Clear();vh.AddUIVertexTriangleStream(output);
  }
 }
}
