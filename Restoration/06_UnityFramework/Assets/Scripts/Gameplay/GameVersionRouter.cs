using UnityEngine;
using UnityEngine.SceneManagement;
namespace CoinMerge.Recovery
{
    [DefaultExecutionOrder(-1000)]
    public sealed class GameVersionRouter : MonoBehaviour
    {
        public GameBalanceConfig balance;
        public RecoveredGameSession rewarded;
        public PackagedGameSession packaged;
        public bool isRewarded;
        public string SaveNamespace=>rewarded?rewarded.saveNamespace:packaged.saveNamespace;
        public VersionProfile Profile {get;private set;}
        public bool Changing {get;private set;}
        PlayerStore store;
        static string pendingNamespace;
        void Awake()
        {
            if(pendingNamespace!=null){if(rewarded)rewarded.saveNamespace=pendingNamespace;if(packaged)packaged.saveNamespace=pendingNamespace;pendingNamespace=null;}
            store=new PlayerStore(SaveNamespace);Profile=store.LoadProfile(balance);
            VersionRouting.Initialize(Profile);store.SaveProfile(Profile);
            if(Profile.rewardedVariant!=isRewarded)
            {
                Changing=true;
                if(rewarded)rewarded.enabled=false;if(packaged)packaged.enabled=false;
                pendingNamespace=SaveNamespace;SceneManager.LoadScene(VersionRouting.Scene(Profile));
            }
        }
        public void Apply(VersionProfile draft)
        {
            if(Changing)return;
            VersionRouting.Resolve(draft);draft.country=RecoveredGameRules.NormalizeCountry(draft.country,balance.rules.supportedCountries);
            if(rewarded)rewarded.Save();if(packaged)packaged.Save();
            Profile=VersionRouting.Copy(draft);store.SaveProfile(Profile);Changing=true;
            pendingNamespace=SaveNamespace;SceneManager.LoadScene(VersionRouting.Scene(Profile));
        }
        public void ResetCurrentPlayer()
        {
            if(rewarded)rewarded.ResetPlayerKeepingProfile();
            if(packaged)packaged.ResetPlayerKeepingProfile();
        }
    }
}
