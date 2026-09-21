using UnityEngine;
namespace CoinMerge.Recovery
{
    // Authored world-space dots; no runtime hierarchy construction or allocations.
    [DefaultExecutionOrder(100)]
    public sealed class RecoveredDropGuide : MonoBehaviour
    {
        public RecoveredGameSession session;
        public SpriteRenderer[] dots;
        [Min(1)] public float spacingPixels=30;
        public int VisibleDots {get;private set;}
        void LateUpdate(){Refresh(session.IsBoardPointerHeld);}
        public void Refresh(bool held)
        {
            var board=session.board;var preview=board.Preview;
            int count=0;
            if(held&&preview&&!board.GameOver&&!board.InputBlocked)
            {
                float bottom=preview.Position.y-preview.Radius;
                float hit=board.GetDropLandingBottom();
                float spacing=Mathf.Max(1,spacingPixels)/board.Units;
                for(float y=bottom;y>hit&&count<dots.Length;y-=spacing)
                {
                    var dot=dots[count++];dot.transform.position=new Vector3(preview.Position.x,y,board.transform.position.z);
                    dot.enabled=true;
                }
            }
            for(int i=count;i<VisibleDots;i++)dots[i].enabled=false;
            VisibleDots=count;
        }
        void OnDisable(){for(int i=0;i<dots.Length;i++)if(dots[i])dots[i].enabled=false;VisibleDots=0;}
    }
}
