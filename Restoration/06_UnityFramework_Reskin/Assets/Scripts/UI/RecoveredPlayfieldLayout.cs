using UnityEngine;
namespace CoinMerge.Recovery
{
    [DefaultExecutionOrder(-100)]
    public sealed class RecoveredPlayfieldLayout : MonoBehaviour
    {
        public NativeMergeBoard board;
        public Camera worldCamera;
        public RectTransform gameArea,bottom,warning,upper,background,canvasRoot,gmButton;
        public float upperTopGap=.734f;
        public Vector2 designSize=new Vector2(750,1624);
        public float basePreviewY=558.466f,baseDeadlineY=403.407f,shortScreenShift=.4f;
        int lastWidth,lastHeight;
        Matrix4x4 lastCanvasMatrix;
        Vector2 lastCanvasSize;
        void OnEnable(){Camera.onPreCull+=BeforeCameraCull;}
        void OnDisable(){Camera.onPreCull-=BeforeCameraCull;}
        void BeforeCameraCull(Camera camera){if(camera==worldCamera)Refresh();}
        void LateUpdate(){Refresh();}
        public void Refresh()
        {
            int width=worldCamera.pixelWidth,height=worldCamera.pixelHeight;
            if(width<=0||height<=0)return;
            if(width==lastWidth&&height==lastHeight&&lastCanvasMatrix==canvasRoot.localToWorldMatrix&&lastCanvasSize==canvasRoot.rect.size)return;
            lastWidth=width;lastHeight=height;
            float visibleHeight=designSize.x*height/width;
            worldCamera.orthographicSize=visibleHeight/(2*board.Units);
            worldCamera.aspect=(float)width/height;
            Canvas.ForceUpdateCanvases();
            // Cocos refreshChildrenWidgetAlignment anchors the upper/bottom regions to
            // the visible frame. Unity's camera-target rect can differ from Screen.height.
            // Project the actual viewport instead of assuming a 1624-high canvas.
            float depth=Vector3.Dot(canvasRoot.position-worldCamera.transform.position,worldCamera.transform.forward);
            Vector3 lo=worldCamera.ViewportToWorldPoint(new Vector3(0,0,depth)),hi=worldCamera.ViewportToWorldPoint(new Vector3(1,1,depth));
            Vector3 localLo=canvasRoot.InverseTransformPoint(lo),localHi=canvasRoot.InverseTransformPoint(hi);
            float center=(localLo.x+localHi.x)*.5f;
            upper.position=canvasRoot.TransformPoint(new Vector3(center,localHi.y-upper.rect.height*(1-upper.pivot.y)-upperTopGap));
            bottom.position=canvasRoot.TransformPoint(new Vector3(center,localLo.y+bottom.rect.height*bottom.pivot.y));
            background.position=canvasRoot.TransformPoint(new Vector3(center,(localLo.y+localHi.y)*.5f));
            background.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,localHi.x-localLo.x+2);
            background.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,localHi.y-localLo.y+2);
            var position=gameArea.position;position.z=board.transform.position.z;board.transform.position=position;
            float shift=Mathf.Min(0,visibleHeight-designSize.y)*shortScreenShift;
            board.previewLine.localPosition=new Vector3(0,(basePreviewY+shift)/board.Units);
            board.deadLine.localPosition=new Vector3(0,(baseDeadlineY+shift)/board.Units);
            var floor=bottom.TransformPoint(new Vector3(0,bottom.rect.yMax));floor.z=board.ground.position.z;board.ground.position=new Vector3(board.transform.position.x,floor.y,floor.z);
            warning.position=board.deadLine.position;
            if(board.Preview)
            {var preview=board.Preview;var point=new Vector3(preview.Position.x,board.previewLine.position.y,board.transform.position.z);preview.body.position=point;preview.transform.position=point;}
            foreach(var coin in board.Coins)if(!coin.IsPreview&&!coin.IsMerging&&coin.Position.y<board.ground.position.y+coin.Radius+2/board.Units)
            {coin.body.position=new Vector2(coin.Position.x,board.ground.position.y+coin.Radius+2/board.Units);coin.body.velocity=Vector2.zero;coin.body.angularVelocity=0;}
            lastCanvasMatrix=canvasRoot.localToWorldMatrix;lastCanvasSize=canvasRoot.rect.size;
        }
    }
}
