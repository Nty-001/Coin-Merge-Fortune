using System;
using CoinMerge.Recovery;
class MockFlowChecks
{
    static int count;
    static void Check(bool result,string message){if(!result)throw new Exception(message);count++;Console.WriteLine("PASS "+message);}
    static void Main()
    {
        var sdk=new MockSdkFacade();var flow=new BusinessFlow(sdk,sdk);
        Check(flow.WatchAdForReward("reward",2m).Result&&flow.RewardBalance==2m,"completed ad grants reward once");
        foreach(var outcome in new[]{AdOutcome.Cancelled,AdOutcome.Failed,AdOutcome.Unavailable}){
            sdk.NextAdOutcome=outcome;Check(!flow.WatchAdForReward("reward",3m).Result&&flow.RewardBalance==2m,outcome+" does not grant reward");}
        var req=new WithdrawalRequest{RequestId="accepted",Amount=1m,Account="fixture",RequiredMerges=5,CurrentMerges=5};
        Check(flow.TriggerWithdrawal(req).Result.Accepted,"withdrawal trigger returns mock pending");
        flow.TriggerWithdrawal(req).Wait();Check(sdk.History.Count==1,"same withdrawal id is idempotent");
        Check(!flow.TriggerWithdrawal(new WithdrawalRequest{RequestId="bad",Amount=1m,Account="fixture",RequiredMerges=5,CurrentMerges=4}).Result.Accepted,"condition failure reaches rejected branch");
        Check(!flow.TriggerWithdrawal(new WithdrawalRequest{RequestId="account",Amount=1m}).Result.Accepted,"missing account rejected");
        sdk.FailNextWithdrawal=true;Check(!flow.TriggerWithdrawal(new WithdrawalRequest{RequestId="server",Amount=1m,Account="fixture"}).Result.Accepted,"mock server error reaches failure branch");
        Check(MergeRules.Upgrade(2)==5&&MergeRules.Upgrade(2000)==2000,"recovered upgrade map");
        Check(MergeRules.PickWeighted(new[]{1,2},new[]{50f,50f},.499)==1&&MergeRules.PickWeighted(new[]{1,2},new[]{50f,50f},.5)==2,"original weighted boundary semantics");
        Console.WriteLine("TOTAL="+count+"; local mock checks only, no network used");
    }
}
