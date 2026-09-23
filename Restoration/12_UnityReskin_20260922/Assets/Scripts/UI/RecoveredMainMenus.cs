using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
namespace CoinMerge.Recovery
{
    [Serializable] public sealed class MenuActionBinding {public Button button;public int action;}
    [Serializable] public sealed class FakeWithdrawRow
    {public GameObject root,selected,unselected;public Text amount,condition,remaining;public Image fill;public double progress;}
    [Serializable] public sealed class CoinWithdrawRow
    {public GameObject root,selected,enough;public Text[] amounts;}
    [Serializable] public sealed class AccountForm
    {
        public InputField account,fullName,taxId;public Image confirm;public Outline confirmOutline;
        public string enabledSprite,disabledSprite;public Color enabledOutline=new Color(7/255f,113/255f,50/255f),disabledOutline=new Color(84/255f,84/255f,84/255f);
        public Image[] platforms;public GameObject[] selected;public string[] names,platformPaths;
    }
    [DefaultExecutionOrder(100)] public sealed class RecoveredMainMenus : MonoBehaviour
    {
        public const int Settings=0,Rules=1,Policy=2,Fake=3,Coin=4,Email=5,Brazil=6,Phone=7,Verify=8,NextStage=9,Active=10;
        public RecoveredGameSession session;
        public GameObject[] pages;
        public MenuActionBinding[] actions;public LocalizedLabelBinding[] labels;public CurrencyIconBinding[] currencies;
        public GameObject bgmOn,bgmOff,vibrateOn,vibrateOff,privacyScroll,termsScroll,coinReady,coinBlocked,toast;
        public Text policyTitle,fakeBalance,coinCount,coinPercent,coinHint,stageAmount,stagePercent,stageHint,verifyAmount,verifyCommission,verifyCredited,toastText;
        public Image coinFill,stageFill;
        public Image policyPlate;
        public string privacyPlateResource,termsPlateResource;
        public FakeWithdrawRow[] fakeRows;public CoinWithdrawRow[] coinRows;
        public AccountForm[] forms;
        public RecoveredVerificationView verification;public NativeSkeletonPlayer ruleAnimation;
        public PackagedAudio audioCues;
        public bool IsOpen=>stack.Count>0;
        public int CurrentPage=>stack.Count==0?-1:stack.Peek();
        public int SelectedCash {get;private set;}public int SelectedCoin {get;private set;}
        public bool CoinConditionsMet {get;private set;}
        readonly Stack<int> stack=new Stack<int>(12);
        UnityAction[] callbacks;UnityAction<string>[] edits;float toastRemaining,requestCooldown;string oldPlatform;bool initialized;
        PlayerProgress Player=>session.Player;
        RecoveredLocalization Locale=>session.Locale;
        CashGroup Group=>RecoveredGameRules.CashConfiguration(session.board.Config.rules,session.Profile.country);
        void Awake()
        {
            callbacks=new UnityAction[actions.Length];for(int i=0;i<actions.Length;i++){int code=actions[i].action;callbacks[i]=()=>Act(code);actions[i].button.onClick.AddListener(callbacks[i]);}
            edits=new UnityAction<string>[forms.Length];
            for(int i=0;i<forms.Length;i++){int index=i;edits[i]=value=>RefreshAccount(index);forms[i].account.onValueChanged.AddListener(edits[i]);if(forms[i].fullName)forms[i].fullName.onValueChanged.AddListener(edits[i]);if(forms[i].taxId)forms[i].taxId.onValueChanged.AddListener(edits[i]);}
            verification.Finished+=FinishVerification;
        }
        void Start(){initialized=true;ApplyLocale();audioCues.SetMusic(Player.open_bgm);}
        void ApplyLocale(){foreach(var x in labels)x.label.text=Locale.Label(x.key);foreach(var x in currencies)Locale.ApplyIcon(x.image,x.type);}
        public void Act(int code)
        {
            if(!initialized)return;
            if(code>=2000){SelectPlatform(code-2000);return;}
            if(code>=1100){SelectedCoin=code-1100;RefreshCoin();return;}
            if(code>=1000){SelectedCash=code-1000;RefreshFake();return;}
            audioCues.Effect(1,Player.open_music);
            switch(code)
            {
                case 0:Close();break;
                case 1:Show(Settings);break;
                case 2:Show(Rules);ruleAnimation.Play("animation",false);break;
                case 3:SelectedCash=0;Show(Fake);break;
                case 4:SelectedCoin=0;Show(Coin);break;
                case 5:Player.open_bgm=Player.open_music=!Player.open_bgm;audioCues.SetMusic(Player.open_bgm);session.Save();RefreshSettings();break;
                case 6:Player.open_vibrate=!Player.open_vibrate;session.Save();RefreshSettings();break;
                case 7:ShowPolicy(false);break;
                case 8:ShowPolicy(true);break;
                case 9:if(fakeRows[SelectedCash].progress==1)Show(Active);else ShowToast(fakeRows[SelectedCash].remaining.text);break;
                case 10:BeginCoinRequest();break;
                case 11:SubmitAccount(0);break;
                case 12:SubmitAccount(1);break;
                case 13:SubmitAccount(2);break;
                case 14:audioCues.Effect(2,Player.open_music);break; // Original unavailable button only plays no_click.
            }
        }
        public void Show(int page)
        {
            if(CurrentPage==page)return;
            stack.Push(page);pages[page].GetComponent<Canvas>().sortingOrder=200+stack.Count;
            pages[page].SetActive(true);ApplyLocale();Refresh();
        }
        public void Close()
        {
            if(!IsOpen)return;pages[stack.Pop()].SetActive(false);
            if(IsOpen){ApplyLocale();Refresh();}
        }
        public void CloseAll(){while(IsOpen)Close();}
        public void Refresh()
        {
            if(Player==null)return;
            if(CurrentPage==Settings)RefreshSettings();
            if(CurrentPage==Fake)RefreshFake();if(CurrentPage==Coin)RefreshCoin();
        }
        void RefreshSettings(){bgmOn.SetActive(Player.open_bgm);bgmOff.SetActive(!Player.open_bgm);vibrateOn.SetActive(Player.open_vibrate);vibrateOff.SetActive(!Player.open_vibrate);}
        void ShowPolicy(bool terms){Show(Policy);privacyScroll.SetActive(!terms);termsScroll.SetActive(terms);policyTitle.text=Locale.Label(terms?"6":"7");if(policyPlate)policyPlate.sprite=Resources.Load<Sprite>(terms?termsPlateResource:privacyPlateResource);}
        void RefreshFake()
        {
            fakeBalance.text=Locale.Money(Player.fakeMoney);var products=Group.real_products;
            for(int i=0;i<fakeRows.Length;i++)
            {
                var row=fakeRows[i];row.root.SetActive(i<products.Length);if(i>=products.Length)continue;
                var p=products[i];int stage=RecoveredGameRules.FakeWithdrawConditionStage(Player,p);double numerator,denominator;
                row.amount.text=Locale.RealMoney(p.withdrawAmount);
                if(stage==0){numerator=Player.fakeMoney;denominator=p.withdrawAmount;row.condition.text=Format("55",Locale.Money(p.withdrawAmount));row.remaining.text=Format("56",Locale.Money(Math.Max(p.withdrawAmount-Player.fakeMoney,0)));}
                else if(stage==1){numerator=Player.coin1024Number;denominator=p.condition_merge;row.condition.text=Format("57",Number(p.condition_merge));row.remaining.text=Format("58",Number(Math.Max(p.condition_merge-Player.coin1024Number,0)));}
                else if(stage==2){numerator=Player.loginDays;denominator=p.condition_login_days;row.condition.text=Format("59",Number(p.condition_login_days),Number(p.condition_daily_merge));row.remaining.text=Format("60",Number(Math.Max(p.condition_login_days-Player.loginDays,0)),Number(p.condition_daily_merge));}
                else{numerator=Player.watch_video_count;denominator=p.condition_video;row.condition.text=Format("61",Number(p.condition_video));row.remaining.text=Format("62",Number(Math.Max(p.condition_video-Player.watch_video_count,0)));}
                row.progress=Math.Min(numerator/denominator,1);Fill(row.fill,row.progress);row.selected.SetActive(i==SelectedCash);row.unselected.SetActive(i!=SelectedCash);
            }
        }
        void RefreshCoin()
        {
            coinCount.text=Player.coin1024Number.ToString();var products=Group.new_Fake_products;
            for(int i=0;i<coinRows.Length;i++)
            {
                var row=coinRows[i];row.root.SetActive(i<products.Length);if(i>=products.Length)continue;
                foreach(var text in row.amounts)text.text=Locale.Money(products[i].withdrawAmount);
                // Original GameWithdrawItem.refreshEnoughBGSprite has an empty body.
                row.selected.SetActive(i==SelectedCoin);row.enough.SetActive(false);
            }
            double progress=RecoveredWithdrawalRules.Progress(Player,products[SelectedCoin],Player.newFakeMoneyWithdraw[SelectedCoin]);
            Fill(coinFill,progress);if(coinPercent)coinPercent.text=Percent(progress);
            coinHint.text=StepHint(false);CoinConditionsMet=progress>=1;coinReady.SetActive(CoinConditionsMet);coinBlocked.SetActive(!CoinConditionsMet);
        }
        string StepHint(bool popup)
        {
            var c=Group.new_Fake_products[SelectedCoin];int step=Player.newFakeMoneyWithdraw[SelectedCoin];
            if(step==0)return RecoveredWithdrawalRules.ReplaceFirst(Format("106",Number(c.condition_merge)),"%{0}",Number(Math.Max(c.condition_merge-Player.coin1024Number,0)));
            if(step==1)return Format("107",Number(Math.Max(c.condition_ad-Player.watch_video_count,0)));
            if(step==2){double remaining=Math.Max(c.withdrawAmount-Player.fakeMoney,0);return Format("56",popup?Locale.Money(remaining):Number(remaining));}
            if(step==3)return Format("60",Number(Math.Max(c.condition_login_days-Player.loginDays,0)),Number(c.condition_daily_merge));
            return step==4?Format("61",Number(Math.Max(c.condition_video-Player.watch_video_count,0))):Locale.Label("112");
        }
        void BeginCoinRequest()
        {
            if(requestCooldown>0||!CoinConditionsMet)return;requestCooldown=3;
            string country=session.Profile.country;
            if(RecoveredWithdrawalRules.HasCachedAccount(Player,country)){BeginVerification();return;}
            int form=country=="BR"?1:RecoveredWithdrawalRules.PhoneCountry(country)?2:0;
            Show(Email+form);PrepareAccount(form);
        }
        void PrepareAccount(int index)
        {
            var form=forms[index];oldPlatform=Player.realSelectPlatform;
            form.account.text=Player.raccountName;if(form.fullName)form.fullName.text=Player.rfullName;if(form.taxId)form.taxId.text=Player.rdocumentId;
            ((Text)form.account.placeholder).text=string.IsNullOrEmpty(Player.raccountName)?Locale.Label("19"):Player.raccountName;
            if(form.fullName)((Text)form.fullName.placeholder).text=string.IsNullOrEmpty(Player.rfullName)?Locale.Label("18"):Player.rfullName;
            if(form.taxId)((Text)form.taxId.placeholder).text=string.IsNullOrEmpty(Player.rdocumentId)?Locale.Label("20"):Player.rdocumentId;
            if(index>0)RefreshPlatforms(form);
            RefreshAccount(index);
        }
        void RefreshPlatforms(AccountForm form)
        {
            // Lists are authored from recovered cached/default country rules, never a fabricated backend response.
            string prefix=session.Profile.country+":";int count=0;bool found=false;
            foreach(var name in form.names)if(name.StartsWith(prefix,StringComparison.Ordinal)){if(name.Substring(prefix.Length)==Player.realSelectPlatform)found=true;count++;}
            if(!found)foreach(var name in form.names)if(name.StartsWith(prefix,StringComparison.Ordinal)){Player.realSelectPlatform=name.Substring(prefix.Length);oldPlatform=Player.realSelectPlatform;break;}
            int slot=0;
            for(int i=0;i<form.names.Length;i++)if(form.names[i].StartsWith(prefix,StringComparison.Ordinal))
            {form.platforms[slot].gameObject.SetActive(true);form.platforms[slot].sprite=Resources.Load<Sprite>(form.platformPaths[i]);form.selected[slot].SetActive(form.names[i].Substring(prefix.Length)==Player.realSelectPlatform);slot++;}
            for(int i=slot;i<form.platforms.Length;i++)form.platforms[i].gameObject.SetActive(false);session.Save();
        }
        void SelectPlatform(int index)
        {
            int formIndex=CurrentPage-Email;if(formIndex<1||formIndex>2)return;var form=forms[formIndex];string prefix=session.Profile.country+":";int slot=0;
            foreach(var name in form.names)if(name.StartsWith(prefix,StringComparison.Ordinal)){if(slot++==index){Player.realSelectPlatform=name.Substring(prefix.Length);break;}}
            RefreshPlatforms(form);
        }
        void RefreshAccount(int index)
        {
            var f=forms[index];bool complete=!string.IsNullOrWhiteSpace(f.account.text)&&(index==0||f.fullName.text.Length>0)&&(index!=1||f.taxId.text.Length>0);
            f.confirm.sprite=Resources.Load<Sprite>(complete?f.enabledSprite:f.disabledSprite);if(f.confirmOutline)f.confirmOutline.effectColor=complete?f.enabledOutline:f.disabledOutline;
        }
        void SubmitAccount(int index)
        {
            var f=forms[index];string account=f.account.text,normalized="",kind="";bool valid;
            if(index==0)valid=RecoveredWithdrawalRules.Email(account,out normalized);
            else if(index==1)valid=(Player.realSelectPlatform=="PIX"?RecoveredWithdrawalRules.Pix(account,out kind):RecoveredWithdrawalRules.Email(account,out normalized))&&RecoveredWithdrawalRules.Name(f.fullName.text)&&RecoveredWithdrawalRules.TaxId(f.taxId.text);
            else valid=RecoveredWithdrawalRules.Phone(account,Player.realSelectPlatform)&&RecoveredWithdrawalRules.Name(f.fullName.text);
            if(!valid){ShowToast(Locale.Label("14"));return;}
            Player.raccountName=index==0?normalized:account;Player.rfullName=index==0?"":f.fullName.text;Player.rdocumentId=index==1?f.taxId.text:"";
            if(kind.Length>0)Player.raccountType=kind;session.Save();Close();
            if(index>0&&(oldPlatform!=Player.realSelectPlatform||!CoinConditionsMet))return;
            BeginVerification();
        }
        void BeginVerification()
        {
            Show(Verify);var product=Group.new_Fake_products[SelectedCoin];verifyAmount.text=Locale.RealMoney(product.withdrawAmount);
            verifyCommission.text=Locale.Label("21")+": 0";verifyCredited.text=Locale.Label("22")+": "+verifyAmount.text;verification.Begin();
        }
        void FinishVerification()
        {
            if(CurrentPage!=Verify)return;
            var product=Group.new_Fake_products[SelectedCoin];int step=RecoveredWithdrawalRules.Advance(Player,product,Player.newFakeMoneyWithdraw[SelectedCoin]);
            Player.newFakeMoneyWithdraw[SelectedCoin]=step;session.Save();Close();
            if(step>=5)Show(Active);
            else{Show(NextStage);stageAmount.text=Locale.RealMoney(product.withdrawAmount);double progress=RecoveredWithdrawalRules.Progress(Player,product,step);Fill(stageFill,progress);stagePercent.text=Percent(progress);stageHint.text=StepHint(true);}
        }
        public void ShowToast(string text){toastText.text=text;toast.SetActive(true);toastRemaining=2;}
        string Format(string key,string first,string second=null){string s=RecoveredWithdrawalRules.ReplaceFirst(Locale.Label(key),"%{0}",first);return second==null?s:RecoveredWithdrawalRules.ReplaceFirst(s,"%{1}",second);}
        static string Number(double value)=>value.ToString("G",CultureInfo.InvariantCulture);
        static string Percent(double value)=>double.IsNaN(value)?"NaN%":Math.Floor(value*100).ToString(CultureInfo.InvariantCulture)+"%";
        static void Fill(Image image,double value){image.fillAmount=double.IsNaN(value)?0:(float)value;}
        void Update(){if(requestCooldown>0)requestCooldown-=Time.deltaTime;if(toastRemaining>0&&(toastRemaining-=Time.deltaTime)<=0)toast.SetActive(false);}
        void OnDestroy()
        {
            verification.Finished-=FinishVerification;
            if(callbacks!=null)for(int i=0;i<actions.Length;i++)actions[i].button.onClick.RemoveListener(callbacks[i]);
            if(edits!=null)for(int i=0;i<forms.Length;i++){forms[i].account.onValueChanged.RemoveListener(edits[i]);if(forms[i].fullName)forms[i].fullName.onValueChanged.RemoveListener(edits[i]);if(forms[i].taxId)forms[i].taxId.onValueChanged.RemoveListener(edits[i]);}
        }
    }
}
