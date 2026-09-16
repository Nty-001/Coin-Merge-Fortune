using UnityEngine;
using UnityEngine.UI;
namespace CoinMerge.Recovery {
 public sealed class ApprovedProgressFill : MonoBehaviour {
  public Image fill;public float fullWidth;float last=-1;
  void OnEnable(){last=-1;Refresh();}void LateUpdate(){Refresh();}
  void Refresh(){if(!fill||Mathf.Approximately(last,fill.fillAmount))return;last=fill.fillAmount;fill.enabled=last>0;fill.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,Mathf.Max(1,fullWidth*last));}
 }
}
