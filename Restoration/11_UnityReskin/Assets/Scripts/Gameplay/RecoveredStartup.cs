using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace CoinMerge.Recovery
{
    // A real build-entry scene. The same local SDK/configuration policy runs on Editor and devices.
    public sealed class RecoveredStartup : MonoBehaviour
    {
        public GameBalanceConfig balance;
        public string saveNamespace="coinmerge.recovered.v1";
        public string rewardedView="Startup/RewardedLoading",packagedView="Startup/PackagedLoading";
        public float fakeTickSeconds=.1f,completedHoldSeconds=.3f,packagedSeconds=2;
        public float[] stages={.15f,.25f,.55f,.7f,.85f,.99f,1};
        [Header("Loading presentation")]
        [Min(0)] public float initialZeroHoldSeconds=.2f;
        [Min(.01f)] public float displayDurationSeconds=1.8f;
        [Range(.01f,.1f)] public float maximumDisplayDeltaSeconds=.05f;
        public RecoveredLoadingView View {get;private set;}
        public float Progress {get;private set;}
        public float TargetProgress {get;private set;}
        public bool Completed {get;private set;}
        public VersionProfile Profile {get;private set;}
        float fakeElapsed,zeroHoldElapsed;
        bool firstFrameRendered,sceneReady;
        IEnumerator Start()
        {
            var store=new PlayerStore(saveNamespace);Profile=store.LoadProfile(balance);
            VersionRouting.Initialize(Profile);store.SaveProfile(Profile);
            var viewRequest=Resources.LoadAsync<GameObject>(Profile.rewardedVariant?rewardedView:packagedView);
            yield return viewRequest;
            var prefab=viewRequest.asset as GameObject;
            if(!prefab)throw new System.InvalidOperationException("Missing authored startup prefab");
            View=Instantiate(prefab,transform).GetComponent<RecoveredLoadingView>();View.SetProgress(0);
            // Do not advance the display until the canvas has presented its initial zero.
            Canvas.willRenderCanvases+=FirstFrame;
            yield return null;
            SetProgress(stages[0]); // Country/profile: existing local facade, no live attribution request.
            store.LoadPlayer();SetProgress(stages[1]);
            if(balance.rules==null)throw new System.InvalidOperationException("Missing recovered configuration");
            SetProgress(stages[2]);
            var language=new RecoveredLocalization(Profile.country);SetProgress(stages[3]);
            yield return null;
            SetProgress(stages[4]);
            var loading=SceneManager.LoadSceneAsync(VersionRouting.Scene(Profile));
            if(loading==null)throw new System.InvalidOperationException("Gameplay scene is not in build settings");
            loading.allowSceneActivation=false;
            float elapsed=0;
            while(loading.progress<.9f||!Profile.rewardedVariant&&elapsed<packagedSeconds)
            {
                elapsed+=Time.unscaledDeltaTime;
                if(Profile.rewardedVariant)SetProgress(Mathf.Lerp(stages[4],stages[5],loading.progress/.9f));
                else TargetProgress=Mathf.Max(TargetProgress,Mathf.Clamp01(elapsed/Mathf.Max(.01f,packagedSeconds)));
                yield return null;
            }
            SetProgress(stages[5]);sceneReady=true;SetProgress(stages[6]);
            while(Progress<1)yield return null;
            Completed=true;
            // Both variants present a completed bar before activating the loaded scene.
            yield return new WaitForSecondsRealtime(completedHoldSeconds);
            GameVersionRouter.SetPendingNamespace(saveNamespace);loading.allowSceneActivation=true;
        }
        void Update()
        {
            if(!View||Completed)return;
            if(Profile.rewardedVariant&&!sceneReady)
            {
                fakeElapsed+=Time.unscaledDeltaTime;
                if(fakeElapsed>=Mathf.Max(.01f,fakeTickSeconds))
                {
                    fakeElapsed%=Mathf.Max(.01f,fakeTickSeconds);
                    // Keep the recovered simulated 90% cap, independently of presentation.
                    if(TargetProgress<.9f)SetProgress(Mathf.Min(.9f,TargetProgress+(TargetProgress<.3f?.05f:TargetProgress<.6f?.03f:TargetProgress<.8f?.02f:.01f)));
                }
            }
            if(!firstFrameRendered)return;
            // An import/IO hitch must not consume the entire visible loading animation.
            float delta=Mathf.Min(Time.unscaledDeltaTime,Mathf.Max(.001f,maximumDisplayDeltaSeconds));
            if(zeroHoldElapsed<initialZeroHoldSeconds){zeroHoldElapsed+=delta;return;}
            float target=sceneReady?TargetProgress:Mathf.Min(TargetProgress,.99f);
            Progress=Mathf.MoveTowards(Progress,target,delta/Mathf.Max(.01f,displayDurationSeconds));View.SetProgress(Progress);
        }
        void FirstFrame(){if(!View)return;firstFrameRendered=true;Canvas.willRenderCanvases-=FirstFrame;}
        void OnDestroy(){Canvas.willRenderCanvases-=FirstFrame;}
        public void SetProgress(float value)
        {
            if(Profile!=null&&!Profile.rewardedVariant&&!sceneReady)return;
            TargetProgress=Mathf.Max(TargetProgress,Mathf.Clamp01(value));
        }
    }
}
