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
        [Tooltip("Use one glyph raster size for this B-version label; fitting changes geometry, not the font atlas.")]
        public bool boundedRasterization;
        [Min(0)] public int maximumLines;
        [Min(16)] public int rasterFontSize=40;
        public int FittedFontSize {get;private set;}
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
                int middle=(low+high)/2;float ratio=boundedRasterization?(float)rasterFontSize/middle:1;
                settings.fontSize=boundedRasterization?rasterFontSize:middle;settings.generationExtents=size*ratio;measure.Populate(label.text,settings);
                BoundsOf(measure.verts,label.pixelsPerUnit,boundedRasterization?0:4,out var min,out var max);
                if((maximumLines==0||measure.lineCount<=maximumLines)&&(max.x-min.x)/ratio<=size.x+.25f&&(max.y-min.y)/ratio<=size.y+.25f){chosen=middle;low=middle+1;}else high=middle-1;
            }
            label.resizeTextForBestFit=false;label.horizontalOverflow=wrapping;label.verticalOverflow=VerticalWrapMode.Overflow;
            bool changed=FittedFontSize!=chosen;FittedFontSize=chosen;
            int actualSize=boundedRasterization?rasterFontSize:chosen;
            if(label.fontSize!=actualSize)label.fontSize=actualSize;
            else if(changed)label.SetVerticesDirty();
            fitting=false;
        }
        static void BoundsOf(System.Collections.Generic.IList<UIVertex> vertices,float units,int trailingVertices,out Vector2 min,out Vector2 max)
        {
            min=new Vector2(float.MaxValue,float.MaxValue);max=new Vector2(float.MinValue,float.MinValue);
            for(int i=0;i<vertices.Count-trailingVertices;i++){Vector2 p=vertices[i].position/units;min=Vector2.Min(min,p);max=Vector2.Max(max,p);}
            if(vertices.Count<=trailingVertices)min=max=Vector2.zero;
        }
        public override void ModifyMesh(VertexHelper vertices)
        {
            if(!IsActive()||vertices.currentVertCount==0)return;
            if(!label)label=GetComponent<Text>();var rect=label.rectTransform.rect;rect.min+=padding;rect.max-=padding;
            if(boundedRasterization&&measure!=null)
            {
                // The component is authored before outline/gradient effects. All languages share
                // the same raster size, including the temporary measurements used for wrapping.
                float ratio=(float)rasterFontSize/Mathf.Max(1,FittedFontSize>0?FittedFontSize:maximumFontSize);
                var settings=label.GetGenerationSettings(rect.size*ratio);settings.fontSize=rasterFontSize;
                settings.resizeTextForBestFit=false;settings.horizontalOverflow=singleLine?HorizontalWrapMode.Overflow:HorizontalWrapMode.Wrap;
                settings.verticalOverflow=VerticalWrapMode.Overflow;settings.generateOutOfBounds=true;
                measure.Invalidate();measure.Populate(label.text,settings);vertices.Clear();var source=measure.verts;
                float units=1/(label.pixelsPerUnit*ratio);
                for(int i=0;i<source.Count;i++)
                {var vertex=source[i];vertex.position*=units;vertices.AddVert(vertex);if((i&3)==3){vertices.AddTriangle(i-3,i-2,i-1);vertices.AddTriangle(i-1,i,i-3);}}
                if(vertices.currentVertCount==0)return;
            }
            var min=new Vector2(float.MaxValue,float.MaxValue);var max=new Vector2(float.MinValue,float.MinValue);UIVertex v=default;
            for(int i=0;i<vertices.currentVertCount;i++){vertices.PopulateUIVertex(ref v,i);min=Vector2.Min(min,v.position);max=Vector2.Max(max,v.position);}
            var size=max-min;float scale=Mathf.Min(1,rect.width/Mathf.Max(.01f,size.x),rect.height/Mathf.Max(.01f,size.y));
            var centre=(min+max)*.5f;var half=size*(scale*.5f);var adjusted=new Vector2(Mathf.Clamp(centre.x,rect.xMin+half.x,rect.xMax-half.x),Mathf.Clamp(centre.y,rect.yMin+half.y,rect.yMax-half.y));
            if(scale>=1&&Vector2.SqrMagnitude(adjusted-centre)<.0001f)return;
            for(int i=0;i<vertices.currentVertCount;i++){vertices.PopulateUIVertex(ref v,i);Vector2 p=((Vector2)v.position-centre)*scale+adjusted;v.position=new Vector3(p.x,p.y,v.position.z);vertices.SetUIVertex(v,i);}
        }
    }
}
