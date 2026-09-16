using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
namespace CoinMerge.Recovery
{
    public sealed class RecoveredRatingView : MonoBehaviour
    {
        public RecoveredGameSession session;
        public Button close,confirm;
        public Button[] stars;
        public Sprite emptyStar,filledStar;
        public Text title,tips,confirmLabel;
        public int SelectedIndex {get;private set;}
        UnityAction[] callbacks;
        void Awake()
        {
            close.onClick.AddListener(Close);confirm.onClick.AddListener(Confirm);callbacks=new UnityAction[stars.Length];
            for(int i=0;i<stars.Length;i++){int index=i;callbacks[i]=()=>Select(index);stars[i].onClick.AddListener(callbacks[i]);}
        }
        public void Show()
        {
            title.text=session.Locale.Label("65");tips.text=session.Locale.Label("64");confirmLabel.text=session.Locale.Label("92");
            // ScoreDialog.show clears star opacity but does not reset its retained startIndex.
            foreach(var star in stars)star.image.sprite=emptyStar;gameObject.SetActive(true);
        }
        public void Select(int index)
        {SelectedIndex=index;for(int i=0;i<stars.Length;i++)stars[i].image.sprite=i<=index?filledStar:emptyStar;session.Save();}
        public void Confirm()
        {if(SelectedIndex>=3){session.Sdk.OpenMarket();session.Player.gameRateScore=SelectedIndex;}Close();}
        public void Close(){gameObject.SetActive(false);session.Save();}
        void OnDestroy()
        {close.onClick.RemoveListener(Close);confirm.onClick.RemoveListener(Confirm);if(callbacks!=null)for(int i=0;i<stars.Length;i++)stars[i].onClick.RemoveListener(callbacks[i]);}
    }
}
