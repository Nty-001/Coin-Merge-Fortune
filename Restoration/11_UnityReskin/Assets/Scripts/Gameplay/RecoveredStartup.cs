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
        public RecoveredLoadingView View {get;private set;}
        public float Progress {get;private set;}
        public bool Completed {get;private set;}
        public VersionProfile Profile {get;private set;}
        float fakeElapsed;
        IEnumerator Start()
        {
            var store=new PlayerStore(saveNamespace);Profile=store.LoadProfile(balance);
            VersionRouting.Initialize(Profile);store.SaveProfile(Profile);
            var viewRequest=Resources.LoadAsync<GameObject>(Profile.rewardedVariant?rewardedView:packagedView);
            yield return viewRequest;
            var prefab=viewRequest.asset as GameObject;
            if(!prefab)throw new System.InvalidOperationException("Missing authored startup prefab");
            View=Instantiate(prefab,transform).GetComponent<RecoveredLoadingView>();View.SetProgress(0);
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
                elapsed+=Time.deltaTime;
                if(Profile.rewardedVariant)SetProgress(Mathf.Lerp(stages[4],stages[5],loading.progress/.9f));
                else{Progress=Mathf.Clamp01(elapsed/Mathf.Max(.01f,packagedSeconds));View.SetProgress(Progress);}
                yield return null;
            }
            SetProgress(stages[5]);Completed=true;SetProgress(stages[6]);
            if(Profile.rewardedVariant)yield return new WaitForSeconds(completedHoldSeconds);
            GameVersionRouter.SetPendingNamespace(saveNamespace);loading.allowSceneActivation=true;
        }
        void Update()
        {
            if(!View||Completed||!Profile.rewardedVariant)return;
            fakeElapsed+=Time.deltaTime;
            if(fakeElapsed<fakeTickSeconds)return;fakeElapsed%=fakeTickSeconds;
            // LoadingScene.updateFakeProgress; monotonic display and 90% simulated cap.
            if(Progress<.9f)SetProgress(Mathf.Min(.9f,Progress+(Progress<.3f?.05f:Progress<.6f?.03f:Progress<.8f?.02f:.01f)));
        }
        public void SetProgress(float value)
        {
            if(Profile!=null&&!Profile.rewardedVariant&&!Completed)return;
            Progress=Mathf.Max(Progress,Mathf.Clamp01(value));if(View)View.SetProgress(Progress);
        }
    }
}
