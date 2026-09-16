using System;
using UnityEngine;
using UnityEngine.UI;
namespace CoinMerge.Recovery
{
    // GameScene's serialized getMoneyTipNode is iconContent (node 23), not its bell/background parent.
    public sealed class RecoveredNoticeTicker : MonoBehaviour
    {
        public Text label;
        public RectTransform content;
        [Min(.01f)] public float interval=4,moveDuration=.9f;
        public float distance=110;
        public int minimumDays=2,maximumDays=10;
        public string localeKey="109";
        RecoveredLocalization locale;
        WithdrawProduct[] products;
        Vector2 origin;
        float elapsed,moving;
        bool ready,returning,animating;
        public int MessageCount {get;private set;}
        public float Offset=>content.anchoredPosition.y-origin.y;
        void Awake(){origin=content.anchoredPosition;}
        public void Configure(RecoveredLocalization language,WithdrawProduct[] amounts)
        {
            locale=language;products=amounts;ready=true;Restart();
        }
        void OnEnable(){if(ready)Restart();}
        void OnDisable(){if(content)content.anchoredPosition=origin;animating=false;}
        public void Restart(){if(!ready)return;elapsed=moving=0;animating=returning=false;content.anchoredPosition=origin;RefreshMessage();}
        void Update(){Tick(Time.deltaTime);}
        public void Tick(float delta)
        {
            if(!ready)return;
            elapsed+=delta;
            bool started=elapsed>=interval;
            if(started){elapsed%=interval;moving=elapsed;returning=false;animating=true;content.anchoredPosition=origin;}
            if(!animating)return;
            if(!started)moving+=delta;
            if(!returning&&moving>=moveDuration){moving-=moveDuration;returning=true;RefreshMessage();}
            float t=Mathf.Clamp01(moving/moveDuration),ease=.5f-.5f*Mathf.Cos(Mathf.PI*t);
            content.anchoredPosition=origin+Vector2.up*(returning?distance*(ease-1):distance*ease);
            if(returning&&moving>=moveDuration){animating=false;content.anchoredPosition=origin;}
        }
        void RefreshMessage()
        {
            string user=((char)('A'+UnityEngine.Random.Range(0,26))).ToString()+"**"+
                (char)('A'+UnityEngine.Random.Range(0,26))+(char)('A'+UnityEngine.Random.Range(0,26));
            int days=UnityEngine.Random.Range(minimumDays,maximumDays+1);
            double amount=products!=null&&products.Length>0?products[UnityEngine.Random.Range(0,products.Length)].withdrawAmount:0;
            label.text=Format(locale,localeKey,user,days,amount);MessageCount++;
        }
        public static string Format(RecoveredLocalization language,string key,string user,int days,double amount)
        {
            return RecoveredWithdrawalRules.ReplaceFirst(RecoveredWithdrawalRules.ReplaceFirst(
                RecoveredWithdrawalRules.ReplaceFirst(language.Label(key),"%{0}",user),"%{1}",days.ToString(System.Globalization.CultureInfo.InvariantCulture)),"%{2}",language.Money(amount));
        }
    }
}
