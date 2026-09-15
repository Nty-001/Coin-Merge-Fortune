using UnityEngine;
namespace CoinMerge.Recovery
{
    public sealed class RecoveredIdleGuide : MonoBehaviour
    {
        public RecoveredGameSession session;
        public GameObject guide;
        public Animation handAnimation;
        public float delay=15;
        public float Elapsed {get;private set;}
        bool suspended;
        void Awake(){ResetIdle();}
        void Update()
        {
            bool touched=Input.GetMouseButtonDown(0);
            for(int i=0;i<Input.touchCount&&!touched;i++)touched=Input.GetTouch(i).phase==TouchPhase.Began;
            Tick(Time.deltaTime,touched);
        }
        public void Tick(float dt,bool touched)
        {
            if(touched||suspended||!session.CanShowIdleGuide){ResetIdle();return;}
            Elapsed+=Mathf.Max(0,dt);
            if(Elapsed>=delay&&!guide.activeSelf){guide.SetActive(true);handAnimation.Play();}
        }
        public void ResetIdle(){Elapsed=0;if(guide)guide.SetActive(false);}
        void OnApplicationPause(bool paused){suspended=paused;ResetIdle();}
        void OnDisable(){ResetIdle();}
    }
}
