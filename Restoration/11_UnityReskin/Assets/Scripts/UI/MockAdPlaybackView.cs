using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
namespace CoinMerge.Recovery
{
    // Authored local stand-in for a native full-screen ad. No ad SDK or network.
    public sealed class MockAdPlaybackView : MonoBehaviour,IMockAdPlayback
    {
        public RecoveredGameSession session;
        public GameObject panel;
        public Text countdown,placementLabel;
        public Button finish,cancel;
        [Min(.1f)] public float playbackSeconds=5;
        public AudioSource[] audioToPause;
        bool[] resumeAudio;
        TaskCompletionSource<AdOutcome> pending;
        AdOutcome requested;
        float elapsed;
        int displayed=-1;
        public bool Playing=>pending!=null;
        public bool Ready=>Playing&&elapsed>=playbackSeconds;
        void Awake()
        {
            resumeAudio=new bool[audioToPause.Length];panel.SetActive(false);
            finish.onClick.AddListener(Finish);cancel.onClick.AddListener(Cancel);
        }
        public Task<AdOutcome> Play(AdKind kind,string placement,AdOutcome outcome)
        {
            if(Playing)return Task.FromResult(AdOutcome.Unavailable);
            if(outcome==AdOutcome.Unavailable)return Task.FromResult(outcome);
            pending=new TaskCompletionSource<AdOutcome>();var task=pending.Task;
            requested=outcome;elapsed=0;displayed=-1;finish.interactable=false;
            placementLabel.text=kind==AdKind.Rewarded?"激励广告 · 本地模拟":"插屏广告 · 本地模拟";
            for(int i=0;i<audioToPause.Length;i++)
            {resumeAudio[i]=audioToPause[i]&&audioToPause[i].isPlaying;if(resumeAudio[i])audioToPause[i].Pause();}
            panel.SetActive(true);RefreshCountdown();return task;
        }
        void Update(){Tick(Mathf.Min(Time.unscaledDeltaTime,.1f));}
        public void Tick(float dt)
        {
            if(!Playing||session.IsApplicationSuspended)return;
            elapsed+=Mathf.Max(0,dt);RefreshCountdown();
            if(Ready&&requested!=AdOutcome.Completed)Resolve(requested);
        }
        void RefreshCountdown()
        {
            int seconds=Mathf.CeilToInt(Mathf.Max(0,playbackSeconds-elapsed));
            if(displayed!=seconds){displayed=seconds;countdown.text=seconds>0?"模拟播放中，剩余 "+seconds+" 秒":"播放完成，可以关闭广告";}
            finish.interactable=Ready;
        }
        public void Finish(){if(Ready)Resolve(requested);}
        public void Cancel(){if(Playing)Resolve(AdOutcome.Cancelled);}
        void Resolve(AdOutcome outcome)
        {
            var task=pending;if(task==null)return;pending=null;panel.SetActive(false);
            for(int i=0;i<audioToPause.Length;i++)
                if(resumeAudio[i]&&audioToPause[i]&&session.Player!=null)
                {
                    bool allowed=audioToPause[i]==session.menus.audioCues.music?session.Player.open_bgm:session.Player.open_music;
                    if(allowed)audioToPause[i].UnPause();else audioToPause[i].Stop();
                    resumeAudio[i]=false;
                }
            task.TrySetResult(outcome);
        }
        void OnDisable(){Resolve(AdOutcome.Cancelled);}
        void OnDestroy(){finish.onClick.RemoveListener(Finish);cancel.onClick.RemoveListener(Cancel);}
    }
}
