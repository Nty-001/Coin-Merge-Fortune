using System;
using UnityEngine;
using UnityEngine.UI;
namespace CoinMerge.Recovery
{
    [Serializable] public sealed class MenuImageBinding {public Image image;public string resourcePath;}
    public sealed class RecoveredMenuArt : MonoBehaviour
    {
        public MenuImageBinding[] images;
        void OnEnable(){foreach(var item in images)if(item.image)item.image.sprite=Resources.Load<Sprite>(item.resourcePath);}
        void OnDisable(){foreach(var item in images)if(item.image)item.image.sprite=null;}
    }
}
