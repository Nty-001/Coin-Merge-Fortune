using System;
using UnityEngine;
using UnityEngine.UI;
namespace CoinMerge.Recovery
{
    public sealed class RecoveredFailView : MonoBehaviour
    {
        public Button revive,close,restart;
        public Text scoreText,mergesText,bestText;
        public event Action ReviveRequested,RestartRequested;
        void Awake(){revive.onClick.AddListener(OnRevive);close.onClick.AddListener(OnRestart);restart.onClick.AddListener(OnRestart);}
        void OnRevive(){ReviveRequested?.Invoke();}
        void OnRestart(){gameObject.SetActive(false);RestartRequested?.Invoke();}
        public void Show(PlayerProgress player)
        {scoreText.text=player.roundScore.ToString();mergesText.text="×"+player.coin1024Number;bestText.text=player.histroyMaxScore.ToString();revive.interactable=true;gameObject.SetActive(true);}
        void OnDestroy(){revive.onClick.RemoveListener(OnRevive);close.onClick.RemoveListener(OnRestart);restart.onClick.RemoveListener(OnRestart);}
    }
}
