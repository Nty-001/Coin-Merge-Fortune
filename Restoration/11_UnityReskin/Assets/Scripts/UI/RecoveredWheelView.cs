using System;
using UnityEngine;
using UnityEngine.UI;
namespace CoinMerge.Recovery
{
    [Serializable] public sealed class RecoveredWheelTimings
    {
        // LaoHuJ_TX idle duration 1.6667; source sp.Skeleton timeScale=2.
        public float introDuration=.83335f,resultDelay=.8f;
        public int revolutions=2;
        public float fastBoundary=.4f,slowBoundary=.7f,fastDelay=.05f,middleDelay=.12f,slowDelay=.15f,slowExtra=.35f;
        public double cashMultiplier=1.3;
    }
    [Serializable] public sealed class RecoveredWheelSlot
    {public GameObject selected,coin;public Image money;public Text coinAmount;}
    public sealed class RecoveredWheelView : MonoBehaviour
    {
        public Button draw;
        public Text drawLabel,countLabel,nextScoreLabel;
        public RecoveredWheelSlot[] slots;
        public GameBalanceConfig config;
        public NativeSkeletonPlayer backgroundAnimation;
        public event Action DrawRequested;
        public event Action<int> DrawCompleted;
        public bool IsSpinning {get;private set;}
        int result,step,totalSteps,phase;
        float wait;
        void Awake(){draw.onClick.AddListener(Request);backgroundAnimation.Completed+=OnIntroCompleted;}
        void OnIntroCompleted(){if(IsSpinning&&phase==0){phase=1;wait=0;}}
        void Request(){if(!IsSpinning)DrawRequested?.Invoke();}
        public void Show(PlayerProgress player,RecoveredLocalization locale)
        {
            IsSpinning=false;draw.gameObject.SetActive(true);nextScoreLabel.gameObject.SetActive(true);
            drawLabel.text=locale.Label("85");
            countLabel.text=RecoveredGameRules.AvailableSpins(config.rules.lotteryScores,player.gameTotalScore,player.currentLotteryCount)+locale.Label("46");
            int after=player.gameTotalScore-RecoveredGameRules.RequiredScore(config.rules.lotteryScores,player.currentLotteryCount);
            int need=Math.Max(0,RecoveredGameRules.RequiredScore(config.rules.lotteryScores,player.currentLotteryCount+1)-Math.Max(0,after));
            nextScoreLabel.text=locale.Label("45").Replace("%{0}",need.ToString());
            for(int i=0;i<slots.Length;i++)
            {
                var reward=config.rules.lotteryRewards[i];var slot=slots[i];slot.selected.SetActive(false);
                bool money=reward.type=="money";slot.money.gameObject.SetActive(money);slot.coin.SetActive(!money);
                if(money)locale.ApplyIcon(slot.money,reward.amount>5000?3:1);else slot.coinAmount.text="+"+reward.amount;
            }
            gameObject.SetActive(true);
        }
        public void Begin(int index)
        {
            if(IsSpinning)return;IsSpinning=true;result=index;step=0;phase=0;
            totalSteps=config.wheel.revolutions*slots.Length+index+1;wait=0;
            draw.gameObject.SetActive(false);nextScoreLabel.gameObject.SetActive(false);
            backgroundAnimation.Play("idle",false);
        }
        void Update()
        {
            if(!IsSpinning||phase==0)return;wait-=Time.deltaTime;if(wait>0)return;
            if(phase==2){IsSpinning=false;DrawCompleted?.Invoke(result);return;}
            phase=1;
            for(int i=0;i<slots.Length;i++)slots[i].selected.SetActive(i==step%slots.Length);
            step++;
            if(step>=totalSteps){phase=2;wait=config.wheel.resultDelay;return;}
            float fraction=(float)step/totalSteps;var t=config.wheel;
            wait=fraction<t.fastBoundary?t.fastDelay:fraction<t.slowBoundary?t.middleDelay:t.slowDelay+(fraction-t.slowBoundary)/(1-t.slowBoundary)*t.slowExtra;
        }
        void OnDestroy(){draw.onClick.RemoveListener(Request);backgroundAnimation.Completed-=OnIntroCompleted;}
    }
}
