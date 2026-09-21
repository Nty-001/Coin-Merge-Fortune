using System;
using UnityEngine;
using UnityEngine.UI;
namespace CoinMerge.Recovery
{
    public sealed class RecoveredRewardView : MonoBehaviour
    {
        public GameObject highestGroup,normalGroup,guideGroup,doubleGroup;
        public Text highestAmount,normalAmount,guideAmount,doubleAmount;
        public Button mask,highestClose,guideClose;
        public RecoveredGameSession session;
        public RectTransform[] glows;
        public Text highestTitle,normalTitle,guideTitle,doubleTitle;
        public RecoveredLocalization Locale {get;set;}
        public event Action Closed;
        public int Kind {get;private set;}
        public double Amount {get;private set;}
        float autoClose=-1;
        void Awake(){mask.onClick.AddListener(OnMask);highestClose.onClick.AddListener(Close);guideClose.onClick.AddListener(Close);}
        void OnMask(){if(Kind==1||Kind==4||(Kind==5&&session.Player.guideStep==2))Close();}
        public void Show(int kind,double amount)
        {
            Kind=kind;Amount=amount;
            highestGroup.SetActive(kind==4);normalGroup.SetActive(kind==3);guideGroup.SetActive(kind==5);doubleGroup.SetActive(kind==1||kind==2);
            highestTitle.text=Locale.Label("36");normalTitle.text=Locale.Label("110");guideTitle.text=Locale.Label("44");doubleTitle.text=Locale.Label(kind==1?"34":"35");
            string value="+"+Locale.Money(amount);
            foreach(var glow in glows)glow.localRotation=Quaternion.identity;
            session.lifecycle.PlaySound(kind==1||kind==2?0:kind==3?1:kind==4?2:3);
            highestAmount.text=normalAmount.text=guideAmount.text=doubleAmount.text=value;
            autoClose=kind==2||kind==3?session.lifecycle.config.rewardAutoClose:-1;gameObject.SetActive(true);
        }
        void Update(){Tick(Time.deltaTime);}
        public void Tick(float dt){foreach(var glow in glows)if(glow.gameObject.activeInHierarchy)glow.Rotate(0,0,session.lifecycle.config.glowSpeed*dt);if(autoClose>=0&&(autoClose-=dt)<=0)Close();}
        public void Close(){if(!gameObject.activeSelf)return;autoClose=-1;gameObject.SetActive(false);Closed?.Invoke();}
        void OnDestroy(){mask.onClick.RemoveListener(OnMask);highestClose.onClick.RemoveListener(Close);guideClose.onClick.RemoveListener(Close);}
    }
}
