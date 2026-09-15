using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
namespace CoinMerge.Recovery
{
    [Serializable] public sealed class RecoveredLabel {public string key,value;}
    [Serializable] public sealed class RecoveredLocale {public string country,currency;public int id;public RecoveredLabel[] labels;public string[] icons;}
    [Serializable] public sealed class LocalizedLabelBinding {public Text label;public string key;}
    [Serializable] public sealed class CurrencyIconBinding {public Image image;public int type=1;}
    public sealed class RecoveredLocalization
    {
        public RecoveredLocale Data {get;}
        readonly Dictionary<string,string> labels=new Dictionary<string,string>(128);
        readonly Sprite[] icons=new Sprite[3];
        public RecoveredLocalization(string normalizedCountry)
        {
            var asset=Resources.Load<TextAsset>("Localization/Text/"+normalizedCountry);
            if(!asset)throw new InvalidOperationException("Missing recovered locale "+normalizedCountry);
            Data=JsonUtility.FromJson<RecoveredLocale>(asset.text);
            foreach(var entry in Data.labels)labels.Add(entry.key,entry.value);
        }
        public string Label(string key)
        {
            // Lab.getlab returns the key itself when a locale omits it (e.g. US 46).
            return labels.TryGetValue(key,out var value)&&!string.IsNullOrEmpty(value)?value.Replace("</c>","</color>"):key;
        }
        public Sprite Icon(int type)
        {
            int index=type-1;if(index<0||index>=icons.Length)throw new ArgumentOutOfRangeException(nameof(type));
            if(!icons[index])icons[index]=Resources.Load<Sprite>(Data.icons[index]);
            if(!icons[index])throw new InvalidOperationException("Missing currency sprite "+Data.icons[index]);
            return icons[index];
        }
        public string Money(double amount)=>FormatMoney(amount,Data.id,Data.currency);
        // GameManagement.getRealMonstr differs from the main balance formatter in several countries.
        public string RealMoney(double amount)
        {
            int id=Data.id;string symbol=Data.currency;
            if(id==2){string three=EcmaFixed(amount,3);return symbol+TrimZeros(three.Substring(0,three.Length-1));}
            string value=TrimZeros(EcmaFixed(Math.Floor(amount*100)/100,id==1||id==16?0:2));
            if(id==3)return Group(value.Replace('.',','),','," ")+symbol;
            value=Group(value,'.',",");return id==5||id==16?value+symbol:symbol+value;
        }
        public static string FormatMoney(double amount,int countryId,string symbol)
        {
            // GameManagement.getmonstr, including its truncation before decimal rounding.
            if(countryId==13)
            {
                string fixedThree=EcmaFixed(amount,3);
                return symbol+TrimZeros(fixedThree.Substring(0,fixedThree.Length-1));
            }
            bool integer=countryId==1||countryId==12||countryId==16||countryId==17||countryId==19||countryId==20||countryId==24||countryId==25;
            string value=TrimZeros(EcmaFixed(Math.Floor(amount*100)/100,integer?0:2));
            char decimalPoint='.';string group=",";
            if(countryId==17||countryId==22||countryId==28){value=value.Replace('.',',');decimalPoint=',';group=countryId==28?".":" ";}
            else if(countryId==19||countryId==20||countryId==23)group=".";
            value=Group(value,decimalPoint,group);
            return countryId==15||countryId==16||countryId==17||countryId==22||countryId==24?value+symbol:symbol+value;
        }
        // Exact IEEE-754 rational rounding preserves JavaScript Number.toFixed halfway behavior.
        static string EcmaFixed(double value,int places)
        {
            if(double.IsNaN(value)||double.IsInfinity(value))throw new ArgumentOutOfRangeException(nameof(value));
            bool negative=value<0;value=Math.Abs(value);
            long bits=BitConverter.DoubleToInt64Bits(value);
            int exponent=(int)((bits>>52)&2047);
            BigInteger numerator=bits&0xFFFFFFFFFFFFFL;
            if(exponent!=0)numerator+=BigInteger.One<<52;
            int shift=exponent==0?-1074:exponent-1023-52;
            numerator*=BigInteger.Pow(10,places);BigInteger denominator=BigInteger.One;
            if(shift>=0)numerator<<=shift;else denominator<<=-shift;
            BigInteger rounded=BigInteger.DivRem(numerator,denominator,out var remainder);
            if(remainder*2>=denominator)rounded++;
            string text=rounded.ToString(CultureInfo.InvariantCulture);
            if(places>0){text=text.PadLeft(places+1,'0');text=text.Insert(text.Length-places,".");}
            return negative&&rounded!=0?"-"+text:text;
        }
        static string TrimZeros(string value)
        {return value.IndexOf('.')<0?value:value.TrimEnd('0').TrimEnd('.');}
        static string Group(string value,char decimalPoint,string separator)
        {
            int end=value.IndexOf(decimalPoint);if(end<0)end=value.Length;
            int start=value.StartsWith("-",StringComparison.Ordinal)?1:0;
            var result=new StringBuilder(value.Length+8);
            for(int i=0;i<value.Length;i++)
            {if(i>start&&i<end&&(end-i)%3==0)result.Append(separator);result.Append(value[i]);}
            return result.ToString();
        }
    }
}
