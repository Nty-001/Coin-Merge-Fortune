using UnityEngine;
using UnityEngine.UI;
namespace CoinMerge.Recovery
{
    public sealed class RecoveredLoadingView : MonoBehaviour
    {
        public Image fill;
        public Text percentage;
        int displayed=-1;
        public void SetProgress(float progress)
        {
            fill.fillAmount=Mathf.Clamp01(progress);
            int value=Mathf.FloorToInt(fill.fillAmount*100+.5f);
            if(percentage&&value!=displayed){displayed=value;percentage.text=value+"%";}
        }
    }
}
