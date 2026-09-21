using System;
using System.Globalization;
namespace CoinMerge.Recovery
{
    // A per-save test offset. Device time and elapsed animation/ad timers are never changed.
    public static class PlayerClock
    {
        public static DateTime Now(PlayerProgress player)=>DateTime.Now.AddSeconds(player.gmTimeOffsetSeconds);
        public static string Today(PlayerProgress player)=>Now(player).ToString("yyyy-MM-dd",CultureInfo.InvariantCulture);
        public static void Advance(PlayerProgress player,double hours,int dailyMerges)
        {
            if(double.IsNaN(hours)||double.IsInfinity(hours)||hours<0||hours>24*366)throw new ArgumentOutOfRangeException(nameof(hours));
            player.gmTimeOffsetSeconds=Math.Min(player.gmTimeOffsetSeconds+hours*3600,86400d*3660);
            player.RecordLoginDay(Today(player),dailyMerges);
        }
    }
}
