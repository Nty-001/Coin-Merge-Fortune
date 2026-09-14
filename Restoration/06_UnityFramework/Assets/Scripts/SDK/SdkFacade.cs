using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace CoinMerge.Recovery
{
    public enum AdOutcome { Completed, Cancelled, Unavailable, Failed }
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
    public interface IWithdrawalFacade { Task<WithdrawalResult> Request(WithdrawalRequest request); IReadOnlyList<WithdrawalResult> History { get; } }
    /// <summary>Local deterministic mock. Never makes network requests or transfers money.</summary>
    public sealed class MockSdkFacade : IAdFacade, IWithdrawalFacade
    {
        public AdOutcome NextAdOutcome = AdOutcome.Completed;
        public bool FailNextWithdrawal;
        public readonly List<string> Trace = new List<string>();
        readonly List<WithdrawalResult> history = new List<WithdrawalResult>();
        readonly Dictionary<string, WithdrawalResult> completed = new Dictionary<string, WithdrawalResult>();
        public IReadOnlyList<WithdrawalResult> History => history;
        public Task<AdOutcome> ShowRewarded(string placement)
        { Trace.Add("ad.request:" + placement); Trace.Add("ad.result:" + NextAdOutcome); return Task.FromResult(NextAdOutcome); }
        public Task<AdOutcome> ShowInterstitial(string placement)
        { Trace.Add("interstitial.request:" + placement); return Task.FromResult(NextAdOutcome); }
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
