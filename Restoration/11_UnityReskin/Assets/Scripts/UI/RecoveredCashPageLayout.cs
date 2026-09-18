using UnityEngine;
namespace CoinMerge.Recovery
{
    // GameFakeWDDialog.adaptLayout, applied identically on device and in the Editor.
    public sealed class RecoveredCashPageLayout : MonoBehaviour
    {
        public RectTransform content,top,bottom,scroll,viewport;
        public float topHeight=456,bottomHeight=168,gap=8,minScrollHeight=300;
        [Tooltip("Use the prefab's visual margin instead of the recovered adaptive inset.")]
        public bool useAuthoredMargin;
        public float authoredMargin;
        void OnEnable(){Apply();}
        void OnRectTransformDimensionsChange(){if(content&&top)Apply();}
        void Apply()
        {
            float h=((RectTransform)transform).rect.height,w=((RectTransform)transform).rect.width;
            float margin=useAuthoredMargin?authoredMargin:h<=1400?6:h<=1624?12:20;
            top.anchoredPosition=new Vector2(0,-topHeight*.5f-margin);bottom.anchoredPosition=new Vector2(0,bottomHeight*.5f+margin);
            float topEdge=h*.5f-topHeight-margin-gap,bottomEdge=-h*.5f+bottomHeight+margin+gap;
            scroll.anchoredPosition=new Vector2(0,topEdge);scroll.sizeDelta=new Vector2(w,Mathf.Max(minScrollHeight,topEdge-bottomEdge));
        }
    }
}
