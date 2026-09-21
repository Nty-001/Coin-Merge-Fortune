using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace CoinMerge.Recovery
{
    public enum AdOutcome { Completed, Cancelled, Unavailable, Failed }
    public enum AdKind { Rewarded, Interstitial }
    public sealed class WithdrawalRequest
    {
        public string RequestId, ProductId, Channel, Account;
        public decimal Amount;
        public int RequiredMerges, CurrentMerges;
    }
    public sealed class WithdrawalResult
    {
        public bool Accepted; public string RequestId, Status, Message;
    }
    public interface IAdFacade { Task<AdOutcome> ShowRewarded(string placement); Task<AdOutcome> ShowInterstitial(string placement); }
    public interface IMockAdPlayback {Task<AdOutcome> Play(AdKind kind,string placement,AdOutcome outcome);}
    public interface IWithdrawalFacade { Task<WithdrawalResult> Request(WithdrawalRequest request); IReadOnlyList<WithdrawalResult> History { get; } }
    /// <summary>Local deterministic mock. Never makes network requests or transfers money.</summary>
    public sealed class MockSdkFacade : IAdFacade, IWithdrawalFacade
    {
        public AdOutcome NextAdOutcome = AdOutcome.Completed;
        public IMockAdPlayback Playback {get;set;}
        public bool AdInProgress {get;private set;}
        public event Action<AdKind, string> AdRequested;
        public bool FailNextWithdrawal;
        public readonly List<string> Trace = new List<string>();
        readonly List<WithdrawalResult> history = new List<WithdrawalResult>();
        readonly Dictionary<string, WithdrawalResult> completed = new Dictionary<string, WithdrawalResult>();
        public IReadOnlyList<WithdrawalResult> History => history;
        public void OpenMarket(){Trace.Add("market.request:mock");}
        public Task<AdOutcome> ShowRewarded(string placement)
        {return Show(AdKind.Rewarded,placement);}
        public Task<AdOutcome> ShowInterstitial(string placement)
        {return Show(AdKind.Interstitial,placement);}
        async Task<AdOutcome> Show(AdKind kind,string placement)
        {
            if(AdInProgress)return AdOutcome.Unavailable;
            var outcome=NextAdOutcome;AdInProgress=true;
            try
            {
                Trace.Add((kind==AdKind.Rewarded?"ad.request:":"interstitial.request:")+placement);
                AdRequested?.Invoke(kind,placement);
                if(outcome!=AdOutcome.Unavailable&&Playback!=null)outcome=await Playback.Play(kind,placement,outcome);
                Trace.Add("ad.result:"+outcome);return outcome;
            }
            finally{AdInProgress=false;}
        }
        public Task<WithdrawalResult> Request(WithdrawalRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (string.IsNullOrWhiteSpace(request.RequestId)) throw new ArgumentException("RequestId is required");
            if (completed.TryGetValue(request.RequestId, out var previous)) return Task.FromResult(previous);
            Trace.Add("withdraw.request:" + request.RequestId);
            string error = request.Amount <= 0 ? "invalid_amount" : string.IsNullOrWhiteSpace(request.Account) ? "account_required" :
                request.CurrentMerges < request.RequiredMerges ? "merge_condition_not_met" : FailNextWithdrawal ? "mock_server_error" : null;
            FailNextWithdrawal = false;
            var result = new WithdrawalResult { RequestId = request.RequestId, Accepted = error == null,
                Status = error == null ? "mock_pending" : "rejected", Message = error ?? "Local mock only; no money transfer" };
            completed[request.RequestId] = result; history.Add(result); Trace.Add("withdraw.result:" + result.Status);
            return Task.FromResult(result);
        }
    }
    /// <summary>Business trigger boundary for subsequent exact C# ports of the recovered JS.</summary>
    public sealed class BusinessFlow
    {
        readonly IAdFacade ads; readonly IWithdrawalFacade withdrawals;
        public decimal RewardBalance { get; private set; }
        public BusinessFlow(IAdFacade ads, IWithdrawalFacade withdrawals) { this.ads = ads; this.withdrawals = withdrawals; }
        public async Task<bool> WatchAdForReward(string placement, decimal reward)
        {
            if (reward < 0) throw new ArgumentOutOfRangeException(nameof(reward));
            if (await ads.ShowRewarded(placement) != AdOutcome.Completed) return false;
            RewardBalance += reward; return true;
        }
        public Task<WithdrawalResult> TriggerWithdrawal(WithdrawalRequest request) => withdrawals.Request(request);
    }
}
