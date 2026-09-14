using System;
namespace CoinMerge.Recovery
{
    [Serializable] public sealed class SavedCoin
    {
        public int value;
        public float x,y,angle,scale=1;
    }
    // Field spelling preserves PlayData.js, including its original misspellings.
    [Serializable] public sealed class PlayerProgress
    {
        public string base_name="PlayData";
        public double money,UseRevenue,fakeMoney;
        public int addVodeoshowCoin=1,gold=100,ecmp=10;
        public int watch_video_count,coin1024Number,today1024NumberCoin,histroyMaxScore,gameTotalScore,roundScore,loginDays;
        public string today1024NumberCoinDate="",lastLoginDate="";
        public bool isNewLoginDay,hasCurrentRoundStats,hasSavedGameScene;
        public int currentRoundStartHistroyMaxScore,currentRoundCoin1024Number;
        public SavedCoin[] savedCoins=Array.Empty<SavedCoin>();
        public int savedCurrentCoinValue=1,savedNextCoinValue=1,savedCoinsScore,savedDrawScore;
        public float savedPreviewX;
        public string raccountName="",rfullName="",rdocumentId="",raccountType="",realSelectPlatform="PayPal";
        public int playerCurrentStep,guideStep,currentLotteryCount,stronewardTimes,dropCointimes,windowsCointimes;
        public int[] watch_video_Singlecount=new int[8],real_watch_video_Singlecount=new int[16],newFakeMoneyWithdraw=new int[6];
        public bool open_music=true,open_vibrate=true,open_bgm=true,firstMergeIcon500;
        public int gameRateScore,gameRateTimes;

        public void NormalizeScoreFields(bool hasTotal,bool hasRound,bool hasDraw)
        {
            int oldBoard=Math.Max(0,savedCoinsScore),oldDraw=Math.Max(0,hasDraw?savedDrawScore:oldBoard);
            roundScore=Math.Max(0,hasRound?roundScore:oldBoard);
            gameTotalScore=Math.Max(0,hasTotal?gameTotalScore:oldDraw);
            savedCoinsScore=roundScore;savedDrawScore=gameTotalScore;
        }
        public bool RecordLoginDay(string today,int requiredMerges)
        {
            if(today1024NumberCoinDate!=today){today1024NumberCoinDate=today;today1024NumberCoin=0;}
            isNewLoginDay=lastLoginDate!=today&&today1024NumberCoin>=requiredMerges;
            if(isNewLoginDay){loginDays=Math.Max(0,loginDays)+1;lastLoginDate=today;}
            return isNewLoginDay;
        }
        public void AddHighestCoinMerge(string today,int requiredMerges)
        {
            if(today1024NumberCoinDate!=today){today1024NumberCoinDate=today;today1024NumberCoin=0;}
            today1024NumberCoin++;
            coin1024Number++;
            currentRoundCoin1024Number=Math.Max(0,currentRoundCoin1024Number)+1;
            RecordLoginDay(today,requiredMerges);
        }
        public void ResetRoundStats()
        {
            hasCurrentRoundStats=true;currentRoundStartHistroyMaxScore=histroyMaxScore;currentRoundCoin1024Number=0;
        }
        public void ResetBoardAfterFailure()
        {
            // GameScene.resetGameAfterFailWithoutAd retains cumulative draw score and balance.
            hasSavedGameScene=true;savedCoins=Array.Empty<SavedCoin>();roundScore=0;savedCoinsScore=0;
            savedPreviewX=0;savedDrawScore=gameTotalScore;
        }
    }
}
