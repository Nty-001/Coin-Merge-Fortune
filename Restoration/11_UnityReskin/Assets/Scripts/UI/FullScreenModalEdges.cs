using UnityEngine;
using UnityEngine.UI;

namespace CoinMerge.Recovery
{
    // The gameplay camera is inset for cutouts. Extend existing modal dimming onto
    // the full-window backdrop, without dimming the center twice or moving UI.
    [DefaultExecutionOrder(1000)]
    public sealed class FullScreenModalEdges : MonoBehaviour
    {
        public Camera contentCamera, backdropCamera;
        public Image[] sourceMasks;
        public Image top, bottom, left, right;
        Rect previousViewport;
        float previousAlpha=-1;

        void OnEnable(){Camera.onPreCull+=BeforeRender;}
        void OnDisable(){Camera.onPreCull-=BeforeRender;SetAlpha(0);}
        void LateUpdate(){if(contentCamera)Refresh(contentCamera.rect);}
        void BeforeRender(Camera camera){if(camera==backdropCamera&&contentCamera)Refresh(contentCamera.rect);}

        public void Refresh(Rect viewport)
        {
            if(viewport!=previousViewport)
            {
                // Four non-overlapping regions. Normalized anchors also handle side cutouts.
                Place(bottom,Vector2.zero,new Vector2(1,viewport.yMin));
                Place(top,new Vector2(0,viewport.yMax),Vector2.one);
                Place(left,new Vector2(0,viewport.yMin),new Vector2(viewport.xMin,viewport.yMax));
                Place(right,new Vector2(viewport.xMax,viewport.yMin),new Vector2(1,viewport.yMax));
                previousViewport=viewport;
            }
            float transmission=1;
            foreach(var mask in sourceMasks)
                if(mask&&mask.isActiveAndEnabled)
                    transmission*=1-Mathf.Clamp01(mask.color.a*mask.canvasRenderer.GetAlpha()*mask.canvasRenderer.GetInheritedAlpha());
            SetAlpha(1-transmission);
        }
        static void Place(Image image,Vector2 min,Vector2 max)
        {
            var rect=image.rectTransform;rect.anchorMin=min;rect.anchorMax=max;
            rect.offsetMin=rect.offsetMax=Vector2.zero;
        }
        void SetAlpha(float alpha)
        {
            if(Mathf.Approximately(alpha,previousAlpha))return;
            previousAlpha=alpha;var color=new Color(0,0,0,alpha);
            top.color=bottom.color=left.color=right.color=color;
        }
    }
}
