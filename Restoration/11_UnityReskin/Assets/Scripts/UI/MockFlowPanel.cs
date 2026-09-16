using UnityEngine;
using UnityEngine.UI;
namespace CoinMerge.Recovery
{
    public sealed class MockFlowPanel : MonoBehaviour
    {
        public Text status; public Button rewardButton, withdrawButton, cancelButton;
        MockSdkFacade sdk;BusinessFlow flow;
        void Awake()
        {
            sdk=new MockSdkFacade();flow=new BusinessFlow(sdk,sdk);
            rewardButton.onClick.AddListener(Reward);withdrawButton.onClick.AddListener(Withdraw);
            cancelButton.onClick.AddListener(()=>{sdk.NextAdOutcome=AdOutcome.Cancelled;Reward();});
            status.text="Local mock / no network or money transfer";
        }
        async void Reward()
        {
            bool granted=await flow.WatchAdForReward("whitepack_reward",1m);
            status.text="Reward granted: "+granted+" / mock balance: "+flow.RewardBalance;
            sdk.NextAdOutcome=AdOutcome.Completed;
        }
        async void Withdraw()
        {
            var result=await flow.TriggerWithdrawal(new WithdrawalRequest {RequestId=System.Guid.NewGuid().ToString(),ProductId="fixture",Channel="mock",Account="local-test",Amount=1m});
            status.text="Withdrawal: "+result.Status+"\n"+result.Message;
        }
    }
}
