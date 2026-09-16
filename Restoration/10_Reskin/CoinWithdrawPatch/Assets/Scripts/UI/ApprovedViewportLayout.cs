using UnityEngine;
namespace CoinMerge.Recovery {
 // Resize only an authored layout surface; all controls are serialized prefabs.
 public sealed class ApprovedViewportLayout : MonoBehaviour {
  public Camera targetCamera; public RectTransform surface; public bool fullPage;
  public Vector2 referenceSize=new Vector2(941,1672); public float worldWidth=750;
  int lastW,lastH; Canvas owner;
  void OnEnable(){owner=GetComponentInParent<Canvas>();Camera.onPreCull+=Before;Refresh();}
  void OnDisable(){Camera.onPreCull-=Before;}
  void Before(Camera c){if(c==targetCamera||owner&&c==owner.worldCamera)Refresh();}
  void LateUpdate(){Refresh();}
  void Refresh(){if(!surface)return;var cam=targetCamera?targetCamera:owner?owner.worldCamera:null;int w=cam?cam.pixelWidth:Screen.width,h=cam?cam.pixelHeight:Screen.height;if(w<1||h<1||w==lastW&&h==lastH)return;lastW=w;lastH=h;
   float visibleH=worldWidth*h/w,scale=worldWidth/referenceSize.x;
   if(!fullPage)scale=Mathf.Min(scale,visibleH/referenceSize.y);
   surface.localScale=Vector3.one*scale;surface.sizeDelta=new Vector2(referenceSize.x,fullPage?visibleH/scale:referenceSize.y);
  }
 }
}
