using System;
using UnityEngine;
using UnityEngine.UI;
namespace CoinMerge.Recovery
{
    public sealed class RecoveredWheelRewardView : MonoBehaviour
    {
        public Button claim;
        public Image money;
        public GameObject coin;
        public Text amountLabel,hintLabel,titleLabel,claimLabel;
        public RectTransform glow;
        public float glowDegreesPerSecond=-90;
        public event Action ClaimRequested;
        public bool Settled {get;set;}
        public bool WatchingAd {get;set;}
        public bool RequiresAd {get;private set;}
        public double Cash {get;private set;}
        public int Coins {get;private set;}
        void Awake(){claim.onClick.AddListener(Request);}
        void Request(){if(!Settled&&!WatchingAd)ClaimRequested?.Invoke();}
        public void Show(int index,PlayerProgress player,GameBalanceConfig config,RecoveredLocalization locale,double randomSample)
        {
            var reward=config.rules.lotteryRewards[index];Settled=WatchingAd=false;
            RequiresAd=player.currentLotteryCount%config.rules.flow.drawRewardStrong==0;
            bool isMoney=reward.type=="money";coin.SetActive(!isMoney);money.gameObject.SetActive(isMoney);
            Cash=0;Coins=0;
            if(isMoney)
            {
                locale.ApplyIcon(money,reward.amount>5000?3:1);
                Cash=RecoveredGameRules.CalculateCash(player.fakeMoney,RecoveredGameRules.CashConfiguration(config.rules,locale.Data.country),randomSample)*config.wheel.cashMultiplier;
                amountLabel.text=locale.Money(Cash);
            }
            else {Coins=(int)reward.amount;amountLabel.text="+"+Coins;}
            int required=RecoveredGameRules.RequiredScore(config.rules.lotteryScores,player.currentLotteryCount);
            hintLabel.text=locale.Label("45").Replace("%{0}",Math.Max(0,required-player.gameTotalScore).ToString());
            titleLabel.text=locale.Label("38");claimLabel.text=locale.Label("37");
            glow.localRotation=Quaternion.identity;gameObject.SetActive(true);
        }
        void Update(){glow.Rotate(0,0,glowDegreesPerSecond*Time.deltaTime);}
        void OnDestroy(){claim.onClick.RemoveListener(Request);}
    }
}
