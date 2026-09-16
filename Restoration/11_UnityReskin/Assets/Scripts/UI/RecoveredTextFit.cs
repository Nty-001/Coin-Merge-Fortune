using UnityEngine;
using UnityEngine.UI;
namespace CoinMerge.Recovery
{
    /// <summary>Fit complete localized text to a fixed authored rect, including stroke and font bearings.</summary>
    [RequireComponent(typeof(Text))]
    public sealed class RecoveredTextFit : BaseMeshEffect, ICanvasElement
    {
        [Min(1)] public int maximumFontSize=40,minimumFontSize=8;
        public Vector2 padding=new Vector2(3,3);
        public bool singleLine;
        Text label;TextGenerator measure;bool fitting;
        protected override void OnEnable()
        {
            base.OnEnable();if(!label)label=GetComponent<Text>();if(measure==null)measure=new TextGenerator();
            label.RegisterDirtyVerticesCallback(Queue);Queue();
        }
        protected override void OnDisable(){if(label)label.UnregisterDirtyVerticesCallback(Queue);CanvasUpdateRegistry.UnRegisterCanvasElementForRebuild(this);base.OnDisable();}
        protected override void OnDestroy(){if(measure!=null){((System.IDisposable)measure).Dispose();measure=null;}base.OnDestroy();}
        protected override void OnRectTransformDimensionsChange(){base.OnRectTransformDimensionsChange();Queue();}
        void Queue(){if(!fitting&&IsActive())CanvasUpdateRegistry.TryRegisterCanvasElementForLayoutRebuild(this);}
        public void Rebuild(CanvasUpdate update){if(update==CanvasUpdate.PostLayout)Fit();}
        public void LayoutComplete(){}
        public void GraphicUpdateComplete(){}
        bool ICanvasElement.IsDestroyed(){return this==null;}
        void Fit()
        {
            if(!label||measure==null||!IsActive())return;
            var size=label.rectTransform.rect.size-padding*2;if(size.x<=0||size.y<=0)return;
            fitting=true;
            var wrapping=singleLine?HorizontalWrapMode.Overflow:HorizontalWrapMode.Wrap;
            var settings=label.GetGenerationSettings(size);settings.resizeTextForBestFit=false;settings.horizontalOverflow=wrapping;settings.verticalOverflow=VerticalWrapMode.Overflow;settings.generateOutOfBounds=true;
            int low=Mathf.Max(1,minimumFontSize),high=Mathf.Max(low,maximumFontSize),chosen=low;
            while(low<=high)
            {
                int middle=(low+high)/2;settings.fontSize=middle;measure.Populate(label.text,settings);
                BoundsOf(measure.verts,label.pixelsPerUnit,out var min,out var max);
                if(max.x-min.x<=size.x+.25f&&max.y-min.y<=size.y+.25f){chosen=middle;low=middle+1;}else high=middle-1;
            }
            label.resizeTextForBestFit=false;label.horizontalOverflow=wrapping;label.verticalOverflow=VerticalWrapMode.Overflow;
            if(label.fontSize!=chosen)label.fontSize=chosen;
            fitting=false;
        }
        static void BoundsOf(System.Collections.Generic.IList<UIVertex> vertices,float units,out Vector2 min,out Vector2 max)
        {
            min=new Vector2(float.MaxValue,float.MaxValue);max=new Vector2(float.MinValue,float.MinValue);
            for(int i=0;i<vertices.Count-4;i++){Vector2 p=vertices[i].position/units;min=Vector2.Min(min,p);max=Vector2.Max(max,p);}
            if(vertices.Count<=4)min=max=Vector2.zero;
        }
        public override void ModifyMesh(VertexHelper vertices)
        {
            if(!IsActive()||vertices.currentVertCount==0)return;
            if(!label)label=GetComponent<Text>();var rect=label.rectTransform.rect;rect.min+=padding;rect.max-=padding;
            var min=new Vector2(float.MaxValue,float.MaxValue);var max=new Vector2(float.MinValue,float.MinValue);UIVertex v=default;
            for(int i=0;i<vertices.currentVertCount;i++){vertices.PopulateUIVertex(ref v,i);min=Vector2.Min(min,v.position);max=Vector2.Max(max,v.position);}
            var size=max-min;float scale=Mathf.Min(1,rect.width/Mathf.Max(.01f,size.x),rect.height/Mathf.Max(.01f,size.y));
            var centre=(min+max)*.5f;var half=size*(scale*.5f);var adjusted=new Vector2(Mathf.Clamp(centre.x,rect.xMin+half.x,rect.xMax-half.x),Mathf.Clamp(centre.y,rect.yMin+half.y,rect.yMax-half.y));
            if(scale>=1&&Vector2.SqrMagnitude(adjusted-centre)<.0001f)return;
            for(int i=0;i<vertices.currentVertCount;i++){vertices.PopulateUIVertex(ref v,i);Vector2 p=((Vector2)v.position-centre)*scale+adjusted;v.position=new Vector3(p.x,p.y,v.position.z);vertices.SetUIVertex(v,i);}
        }
    }
}
