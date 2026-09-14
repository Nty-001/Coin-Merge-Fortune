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
        public event Action Closed;
        public int Kind {get;private set;}
        public double Amount {get;private set;}
        float autoClose=-1;
        void Awake(){mask.onClick.AddListener(Close);highestClose.onClick.AddListener(Close);guideClose.onClick.AddListener(Close);}
        public void Show(int kind,double amount)
        {
            Kind=kind;Amount=amount;
            highestGroup.SetActive(kind==4);normalGroup.SetActive(kind==3);guideGroup.SetActive(kind==5);doubleGroup.SetActive(kind==1||kind==2);
            string value=amount.ToString("F2",System.Globalization.CultureInfo.InvariantCulture);
            highestAmount.text=normalAmount.text=guideAmount.text=doubleAmount.text=value;
            autoClose=kind==2||kind==3?1.5f:-1;gameObject.SetActive(true);
        }
        void Update(){if(autoClose>=0&&(autoClose-=Time.deltaTime)<=0)Close();}
        public void Close(){if(!gameObject.activeSelf)return;autoClose=-1;gameObject.SetActive(false);Closed?.Invoke();}
        void OnDestroy(){mask.onClick.RemoveListener(Close);highestClose.onClick.RemoveListener(Close);guideClose.onClick.RemoveListener(Close);}
    }
}
