using System;
using UnityEngine;
namespace CoinMerge.Recovery
{
    [Serializable] public sealed class VersionProfile
    {
        public string country,cohort;
        public bool rewardedVariant;
        public int routingSchema,contentMode,cohortMode,localBucket,localTail,rewardedShare;
    }
    [Serializable] public sealed class PlayerSaveEnvelope
    {
        public int schema=1;
        public PlayerProgress player=new PlayerProgress();
    }
    // Identical on every platform. GM profile is separate from player reset.
    public sealed class PlayerStore
    {
        readonly string playerKey,profileKey;
        public PlayerStore(string prefix="coinmerge.recovered.v1")
        {playerKey=prefix+".player";profileKey=prefix+".profile";}
        public PlayerProgress LoadPlayer()
        {
            if(!PlayerPrefs.HasKey(playerKey))return new PlayerProgress();
            var envelope=new PlayerSaveEnvelope();
            JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString(playerKey),envelope);
            if(envelope.schema!=1||envelope.player==null)throw new InvalidOperationException("Unsupported player save schema.");
            return envelope.player;
        }
        public void Save(PlayerProgress player)
        {
            PlayerPrefs.SetString(playerKey,JsonUtility.ToJson(new PlayerSaveEnvelope {player=player}));
            PlayerPrefs.Save();
        }
        public VersionProfile LoadProfile(GameBalanceConfig config)
        {
            var profile=new VersionProfile {country=config.defaultCountry,cohort=config.defaultCohort,rewardedVariant=config.defaultRewardedVariant};
            if(PlayerPrefs.HasKey(profileKey))JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString(profileKey),profile);
            profile.country=RecoveredGameRules.NormalizeCountry(profile.country,config.rules.supportedCountries);
            if(profile.cohort!="A"&&profile.cohort!="B")profile.cohort=config.defaultCohort;
            return profile;
        }
        public void SaveProfile(VersionProfile profile)
        {PlayerPrefs.SetString(profileKey,JsonUtility.ToJson(profile));PlayerPrefs.Save();}
        public void ResetPlayer(){PlayerPrefs.DeleteKey(playerKey);PlayerPrefs.Save();}
        public void ResetProfile(){PlayerPrefs.DeleteKey(profileKey);PlayerPrefs.Save();}
    }
}
