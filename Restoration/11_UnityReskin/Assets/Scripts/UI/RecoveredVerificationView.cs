using System;
using UnityEngine;
namespace CoinMerge.Recovery
{
    public sealed class RecoveredVerificationView : MonoBehaviour
    {
        public NativeSkeletonPlayer first,second,stamp;
        public Transform[] tips;
        public GameObject continueGroup;
        public float initialDelay=1,stageDelay=.3f,tipDuration=.4f,stampDelay=.38f,resultDelay=.5f;
        public CanvasGroup[] tipOpacity;
        public Vector2 tipEnterOffset=new Vector2(0,-8);
        public event Action Finished;
        readonly float[] starts={-1,-1,-1,-1};
        float clock,at;int phase=-1;
        Vector3[] tipPositions;
        bool SmoothTips=>tipOpacity!=null&&tipOpacity.Length==tips.Length;
        void Awake(){first.Completed+=FirstDone;second.Completed+=SecondDone;tipPositions=new Vector3[tips.Length];for(int i=0;i<tips.Length;i++)tipPositions[i]=tips[i].localPosition;}
        public void Begin()
        {
            clock=0;phase=0;at=initialDelay;continueGroup.SetActive(false);
            first.transform.parent.gameObject.SetActive(false);second.transform.parent.gameObject.SetActive(false);stamp.transform.parent.gameObject.SetActive(false);
            for(int i=0;i<tips.Length;i++)
            {
                tips[i].localScale=SmoothTips?Vector3.one:Vector3.zero;starts[i]=-1;
                if(SmoothTips){tipOpacity[i].alpha=0;tips[i].localPosition=tipPositions[i]+(Vector3)tipEnterOffset;}
            }
        }
        void FirstDone(){if(phase!=2)return;starts[1]=clock;phase=3;at=clock+stageDelay;}
        void SecondDone(){if(phase!=4)return;starts[2]=starts[3]=clock;phase=5;at=clock+stageDelay;}
        void Update()
        {
            if(phase<0)return;clock+=Time.deltaTime;
            for(int i=0;i<tips.Length;i++)if(starts[i]>=0)
            {
                float progress=Mathf.Clamp01((clock-starts[i])/tipDuration),t=progress-1;
                if(SmoothTips)
                {
                    tipOpacity[i].alpha=Mathf.SmoothStep(0,1,progress);
                    float remaining=1-progress;
                    tips[i].localPosition=tipPositions[i]+(Vector3)tipEnterOffset*(remaining*remaining*remaining);
                }
                else{float size=1+2.70158f*t*t*t+1.70158f*t*t;tips[i].localScale=Vector3.one*size;}
            }
            if(clock<at)return;
            switch(phase)
            {
                case 0:starts[0]=clock;phase=1;at=clock+stageDelay;break;
                case 1:first.transform.parent.gameObject.SetActive(true);first.Play("1",false);phase=2;break;
                case 3:second.transform.parent.gameObject.SetActive(true);second.Play("1",false);phase=4;break;
                case 5:stamp.transform.parent.gameObject.SetActive(true);stamp.Play("2",false);phase=6;at=clock+stampDelay;break;
                case 6:stamp.Play("3",true);continueGroup.SetActive(true);continueGroup.transform.localScale=Vector3.one;phase=7;at=clock+resultDelay;break;
                case 7:phase=-1;Finished?.Invoke();break;
            }
        }
        void OnDisable()
        {
            phase=-1;
            if(!SmoothTips||tipPositions==null)return;
            for(int i=0;i<tips.Length;i++){tips[i].localPosition=tipPositions[i];tips[i].localScale=Vector3.one;tipOpacity[i].alpha=1;}
        }
        void OnDestroy(){first.Completed-=FirstDone;second.Completed-=SecondDone;}
    }
}
