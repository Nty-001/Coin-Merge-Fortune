using System;
using UnityEngine;
using UnityEngine.UI;
namespace CoinMerge.Recovery
{
    public sealed class RecoveredGuideView : MonoBehaviour
    {
        public GameObject stepZero,stepOne,stepThree,stepFour,mask;
        public Button oneButton,threeButton,fourButton,backdropButton;
        public Text balance;
        public RecoveredLocalization Locale {get;set;}
        public event Action Advanced;
        void Awake(){oneButton.onClick.AddListener(Advance);threeButton.onClick.AddListener(Advance);fourButton.onClick.AddListener(Advance);backdropButton.onClick.AddListener(Advance);}
        void Advance(){Advanced?.Invoke();}
        public void Show(int step,double money)
        {
            stepZero.SetActive(step==0);stepOne.SetActive(step==1);stepThree.SetActive(step==3);stepFour.SetActive(step==4);
            mask.SetActive(step!=0);balance.text=Locale.Money(money);
            gameObject.SetActive(step==0||step==1||step==3||step==4);
        }
        void OnDestroy(){oneButton.onClick.RemoveListener(Advance);threeButton.onClick.RemoveListener(Advance);fourButton.onClick.RemoveListener(Advance);backdropButton.onClick.RemoveListener(Advance);}
    }
}
