using UnityEngine;
using UnityEngine.UI;
namespace CoinMerge.Recovery {
 // Ten native, editor-authored Image slots mirror the live player counter.
 public sealed class ApprovedCounterDigits : MonoBehaviour {
  public Text source; public Image[] slots; public Sprite[] digits;
  public float height=135,maxWidth=340,spacing=1;
  string last; bool initialized;
  void OnEnable(){initialized=false;Refresh();}
  void LateUpdate(){Refresh();}
  void OnDisable(){if(source)source.enabled=true;}
  void Refresh(){if(!source)return;string value=source.text;if(initialized&&last==value)return;initialized=true;last=value;
   bool valid=value.Length>0&&value.Length<=slots.Length;float width=0;
   for(int i=0;valid&&i<value.Length;i++){int n=value[i]-'0';if(n<0||n>9||!digits[n]){valid=false;break;}width+=height*digits[n].rect.width/digits[n].rect.height+(i==0?0:spacing);}
   source.enabled=!valid;float scale=width>maxWidth?maxWidth/width:1,x=-width*scale*.5f;
   for(int i=0;i<slots.Length;i++){var image=slots[i];image.enabled=valid&&i<value.Length;if(!image.enabled)continue;var sprite=digits[value[i]-'0'];image.sprite=sprite;float w=height*sprite.rect.width/sprite.rect.height*scale;image.rectTransform.sizeDelta=new Vector2(w,height*scale);image.rectTransform.anchoredPosition=new Vector2(x+w*.5f,0);x+=w+spacing*scale;}
  }
 }
}
