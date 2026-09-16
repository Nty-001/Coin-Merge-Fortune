using System;
namespace CoinMerge.Recovery
{
    // Direct ports of the recovered function bodies. Random samples are injected for differential tests.
    public static class RecoveredGameRules
    {
        public static string NormalizeCountry(string country,string[] supported)
        {
            if(string.IsNullOrEmpty(country))return "US";
            string value=country.Trim().ToUpperInvariant();
            if(value=="CN")return "US";
            for(int i=0;i<supported.Length;i++)if(supported[i]==value)return value;
            return "US";
        }
        public static string ClientCohort(string client)
        {
            if(string.IsNullOrEmpty(client))return "B";
            char last=client[client.Length-1];return last>='5'&&last<='9'?"B":"A";
        }
        public static CashGroup CashConfiguration(RecoveredRulesData rules,string country)
        {
            for(int i=0;i<rules.cashGroups.Length;i++)
                for(int j=0;j<rules.cashGroups[i].countries.Length;j++)
                    if(rules.cashGroups[i].countries[j]==country)return rules.cashGroups[i];
            return rules.cashGroups[0];
        }
        public static CoinDefinition Coin(RecoveredRulesData rules,int value)
        {
            for(int i=0;i<rules.coins.Length;i++)if(rules.coins[i].value==value)return rules.coins[i];
            throw new ArgumentOutOfRangeException(nameof(value),"Coin value not present in original configuration.");
        }
        public static int PickDrop(RecoveredRulesData rules,int highestBoardValue,double sample)
        {
            CheckSample(sample);
            DropRule row=rules.drops[0];
            for(int i=0;i<rules.drops.Length&&rules.drops[i].threshold<=highestBoardValue;i++)row=rules.drops[i];
            double total=0,cumulative=0;
            for(int i=0;i<row.weights.Length;i++)total+=row.weights[i];
            double roll=sample*total;
            for(int i=0;i<row.values.Length;i++){cumulative+=row.weights[i];if(roll<cumulative)return row.values[i];}
            return row.values[row.values.Length-1];
        }
        public static int PickLottery(LotteryReward[] rows,double sample)
        {
            CheckSample(sample);double total=0,cumulative=0;
            for(int i=0;i<rows.Length;i++)total+=rows[i].probability;
            double roll=sample*total;
            // Original lottery uses <=; coin drops use <. Keep the boundary difference.
            for(int i=0;i<rows.Length;i++){cumulative+=rows[i].probability;if(roll<=cumulative)return i;}
            return 0;
        }
        public static int RequiredScore(LotteryScoreRule[] rows,int count)
        {
            for(int i=rows.Length-1;i>=0;i--)if(count>=rows[i].lotteryCount)return rows[i].requiredScore;
            return rows[0].requiredScore;
        }
        public static int AvailableSpins(LotteryScoreRule[] rows,int score,int count)
        {
            int spins=0;
            while(true){int required=RequiredScore(rows,count);if(required<=0)throw new InvalidOperationException("Lottery score must be positive.");if(score<required)return spins;spins++;score-=required;count++;}
        }
        public static int SpendSpin(PlayerProgress player,LotteryScoreRule[] rows)
        {
            int count=Math.Max(0,player.currentLotteryCount),required=RequiredScore(rows,count);
            player.gameTotalScore=Math.Max(0,player.gameTotalScore-required);
            player.savedDrawScore=player.gameTotalScore;player.currentLotteryCount=count+1;
            return player.gameTotalScore;
        }
        public static double CalculateCash(double balance,CashGroup group,double sample)
        {
            CheckSample(sample);double target=-1;
            for(int i=0;i<group.real_products.Length;i++)if(balance<group.real_products[i].withdrawAmount){target=group.real_products[i].withdrawAmount;break;}
            if(target<0)return Math.Floor(balance+.5)/100;
            double ratio=20;
            if(group.ranges!=null&&group.ranges.Length>0)
            {
                ratio=group.ranges[group.ranges.Length-1].ratio;
                for(int i=0;i<group.ranges.Length;i++)if(balance>=group.ranges[i].le&&balance<group.ranges[i].lh){ratio=group.ranges[i].ratio;break;}
            }
            double reward=(target-balance)/ratio*(group.fluctuation.min+sample*(group.fluctuation.max-group.fluctuation.min));
            return Math.Floor(Math.Max(reward,group.baseMinReward)*100+.5)/100;
        }
        public static double VideoQ(VideoRewardRule[] rows,int watched,double sample)
        {
            CheckSample(sample);var row=rows[rows.Length-1];
            for(int i=0;i<rows.Length;i++)if(watched>=rows[i].min&&watched<=rows[i].max){row=rows[i];break;}
            return Math.Floor((row.qMin+sample*(row.qMax-row.qMin))*100+.5)/100;
        }
        public static int RewardDropStage(PlayerProgress player,FlowRules flow)
        {
            player.dropCointimes++;
            if(player.windowsCointimes>flow.adRewardDrop)player.windowsCointimes=0;else player.windowsCointimes++;
            if(player.windowsCointimes==flow.firstRewardDrop)return player.dropCointimes==flow.firstRewardDrop?4:1;
            if(player.windowsCointimes==flow.secondRewardDrop)return 1;
            return player.windowsCointimes==flow.adRewardDrop?2:0;
        }
        public static int FakeWithdrawConditionStage(PlayerProgress player,WithdrawProduct product)
        {
            // GameFakeWDItem.refresh progresses to the video condition after valid login days.
            // condition_daily_merge is explanatory text; GameUtils records qualifying days separately.
            if(player.fakeMoney<product.withdrawAmount)return 0;
            if(player.coin1024Number<product.condition_merge)return 1;
            if(player.loginDays<product.condition_login_days)return 2;
            return 3;
        }
        static void CheckSample(double sample)
        {
            if(double.IsNaN(sample)||sample<0||sample>=1)throw new ArgumentOutOfRangeException(nameof(sample));
        }
    }
}
