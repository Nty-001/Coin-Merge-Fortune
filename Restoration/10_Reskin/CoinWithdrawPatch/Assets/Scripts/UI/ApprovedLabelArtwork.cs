using UnityEngine;
using UnityEngine.UI;
namespace CoinMerge.Recovery {
 // Static approved lettering follows the existing label's value and visibility.
 // Other languages and changed product amounts use the live native Text.
 public sealed class ApprovedLabelArtwork : MonoBehaviour {
  public Text label; public Image artwork; public string matchingText;
  string last; bool initialized;
  void OnEnable(){initialized=false;Refresh();}
  void LateUpdate(){Refresh();}
  void Refresh(){if(!label||!artwork)return;string value=label.text;if(initialized&&value==last)return;initialized=true;last=value;bool match=value==matchingText;artwork.enabled=match;label.enabled=!match;}
  void OnDisable(){if(label)label.enabled=true;}
 }
}
