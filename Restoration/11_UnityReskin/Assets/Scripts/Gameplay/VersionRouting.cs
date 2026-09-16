using System;
using UnityEngine;
namespace CoinMerge.Recovery
{
    // Bundle routing is a LOCAL test policy. The recovered RandomAB label does not choose a bundle.
    public static class VersionRouting
    {
        public const int Automatic=0, Packaged=1, Rewarded=2;
        public static string OriginalCohort(string clientId)
        {
            if(string.IsNullOrEmpty(clientId))return "B";
            char tail=clientId[clientId.Length-1];return tail>='5'&&tail<='9'?"B":"A";
        }
        public static bool OriginalCoinGoalA(string clientId)
        {if(string.IsNullOrEmpty(clientId))return false;char c=clientId[clientId.Length-1];return c>='0'&&c<='4';}
        public static void Initialize(VersionProfile profile)
        {
            if(profile.routingSchema==0)
            {
                profile.contentMode=profile.rewardedVariant?Rewarded:Packaged;
                profile.cohortMode=profile.cohort=="A"?1:2;
                var bytes=Guid.NewGuid().ToByteArray();
                profile.localBucket=(bytes[0]*256+bytes[1])%10000;
                profile.localTail=bytes[2]%10;profile.rewardedShare=50;profile.routingSchema=1;
            }
            Resolve(profile);
        }
        public static void Resolve(VersionProfile profile)
        {
            profile.contentMode=Mathf.Clamp(profile.contentMode,0,2);
            profile.cohortMode=Mathf.Clamp(profile.cohortMode,0,2);
            profile.rewardedShare=Mathf.Clamp(profile.rewardedShare,0,100);
            profile.localBucket=Mathf.Clamp(profile.localBucket,0,9999);profile.localTail=Mathf.Clamp(profile.localTail,0,9);
            profile.rewardedVariant=profile.contentMode==Rewarded||(profile.contentMode==Automatic&&profile.localBucket<profile.rewardedShare*100);
            profile.cohort=profile.cohortMode==0?OriginalCohort(profile.localTail.ToString()):profile.cohortMode==1?"A":"B";
        }
        public static VersionProfile Copy(VersionProfile profile)=>JsonUtility.FromJson<VersionProfile>(JsonUtility.ToJson(profile));
        public static string Scene(VersionProfile profile)=>profile.rewardedVariant?"RecoveredMain":"RecoveredPackaged";
    }
}
