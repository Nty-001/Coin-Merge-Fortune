using System;
using System.Text.RegularExpressions;
namespace CoinMerge.Recovery
{
    public static class RecoveredWithdrawalRules
    {
        static readonly Regex EmailPattern=new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}\z");
        public static bool Email(string value,out string normalized)
        {
            normalized="";
            if(string.IsNullOrWhiteSpace(value)||value.Length>254||!EmailPattern.IsMatch(value))return false;
            string local=value.Split('@')[0];
            if(local.Length>64||value.Contains("..")||local.StartsWith(".",StringComparison.Ordinal)||local.EndsWith(".",StringComparison.Ordinal))return false;
            normalized=value.ToLowerInvariant().Trim();return true;
        }
        public static bool Name(string value)=>Regex.IsMatch(value??"",@"^[a-zA-Z0-9\s-,.]{3,100}\z");
        public static bool TaxId(string value)=>Regex.IsMatch(value??"",@"^\d{11}\z",RegexOptions.ECMAScript);
        public static bool Pix(string value,out string kind)
        {
            kind="";
            if(Email(value,out _))kind="E";
            else if(Regex.IsMatch(value??"",@"^\+55\d{7,12}\z",RegexOptions.ECMAScript))kind="P";
            else if(TaxId(value))kind="C";
            else if(Regex.IsMatch(value??"",@"^[a-zA-Z0-9-]{36}\z"))kind="B";
            return kind.Length>0;
        }
        public static bool Phone(string value,string platform)
        {
            string pattern=platform=="DANA"||platform=="OVO"?@"^08[0-9\s-]{9,14}\z":platform=="Truemoney"?@"^0[0-9]{9}\z":
                platform=="TNG"?@"^[0-9]{10,11}\z":platform=="ZaloPay"?@"^84[0-9]{9}\z":
                platform=="GCash"||platform=="Graboay"||platform=="Paymaya"?@"^09[0-9]{9}\z":null;
            return pattern!=null&&Regex.IsMatch(value??"",pattern);
        }
        public static bool PhoneCountry(string country)=>country=="ID"||country=="TH"||country=="MY"||country=="VN"||country=="PH";
        public static bool HasCachedAccount(PlayerProgress player,string country)
        {return !string.IsNullOrWhiteSpace(player.raccountName)&&(country!="BR"||!string.IsNullOrWhiteSpace(player.rdocumentId))&&
            ((country!="BR"&&!PhoneCountry(country))||!string.IsNullOrWhiteSpace(player.rfullName));}
        public static bool ConditionMet(PlayerProgress p,WithdrawProduct c,int step)
        {
            return step==0?p.coin1024Number>=c.condition_merge:step==1?p.watch_video_count>=c.condition_ad:
                step==2?p.fakeMoney>=c.withdrawAmount:step==3?p.loginDays>=c.condition_login_days:step!=4||p.watch_video_count>=c.condition_video;
        }
        public static int Advance(PlayerProgress p,WithdrawProduct c,int current)
        {
            int next=Math.Max(0,current);if(next<5&&ConditionMet(p,c,next))next++;
            for(int i=next;i<5;i++)if(!ConditionMet(p,c,i))return i;
            return 5;
        }
        public static double Progress(PlayerProgress p,WithdrawProduct c,int step)
        {
            double value=step==0?(double)p.coin1024Number/c.condition_merge:step==1?(double)p.watch_video_count/c.condition_ad:
                step==2?p.fakeMoney/c.withdrawAmount:step==3?(c.condition_login_days>0?(double)p.loginDays/c.condition_login_days:1):
                step==4?(double)p.watch_video_count/c.condition_video:1;
            return Math.Min(1,Math.Max(0,value));
        }
        public static string ReplaceFirst(string text,string key,string value)
        {int at=text.IndexOf(key,StringComparison.Ordinal);return at<0?text:text.Substring(0,at)+value+text.Substring(at+key.Length);}
    }
}
