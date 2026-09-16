using System;
using System.Globalization;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace CoinMerge.Recovery.Editor
{
    public static class WithdrawalRulesValidation
    {
        [Serializable] sealed class Validator {public string method,input,normalized;public bool valid;}
        [Serializable] sealed class Money {public string country,expected;public double amount;}
        [Serializable] sealed class Stage {public string country,progress,hint;public int current,coin,videos,days,next;public double money;public bool ready;}
        [Serializable] sealed class Vectors {public Validator[] validators;public Money[] money;public Stage[] stages;}
        [Serializable] sealed class Report {public bool passed;public int validators,money,stages;public string scope;}
        public static void Run()
        {
            var all=JsonUtility.FromJson<Vectors>(File.ReadAllText("Assets/Config/Runtime/WithdrawalVectors.json"));
            foreach(var v in all.validators)
            {
                bool result;string normalized="",kind="";
                switch(v.method)
                {
                    case "validateEmail":result=RecoveredWithdrawalRules.Email(v.input,out normalized);break;
                    case "validateAccount":result=RecoveredWithdrawalRules.Name(v.input);break;
                    case "validatePhone08":result=RecoveredWithdrawalRules.Phone(v.input,"DANA");break;
                    case "validatePhone10":result=RecoveredWithdrawalRules.Phone(v.input,"Truemoney");break;
                    case "validatePhone10Or11":result=RecoveredWithdrawalRules.Phone(v.input,"TNG");break;
                    case "validatePhone84":result=RecoveredWithdrawalRules.Phone(v.input,"ZaloPay");break;
                    case "validatePhone09":result=RecoveredWithdrawalRules.Phone(v.input,"GCash");break;
                    case "validatePhone55":RecoveredWithdrawalRules.Pix(v.input,out kind);result=kind=="P";break;
                    case "validateCPFJ":result=RecoveredWithdrawalRules.TaxId(v.input);break;
                    case "validatePix":RecoveredWithdrawalRules.Pix(v.input,out kind);result=kind=="B";break;
                    default:throw new Exception(v.method);
                }
                if(result!=v.valid||normalized!=v.normalized)throw new Exception("Source validator mismatch "+v.method+" fixture "+Array.IndexOf(all.validators,v)+" expected "+v.valid+" actual "+result);
            }
            foreach(var v in all.money)
            {string actual=new RecoveredLocalization(v.country).RealMoney(v.amount);if(actual!=v.expected)throw new Exception("RealMoney difference "+v.country+" "+v.amount+" expected "+v.expected+" actual "+actual);}
            var balance=AssetDatabase.LoadAssetAtPath<GameBalanceConfig>("Assets/Config/Runtime/GameBalance.asset");
            foreach(var v in all.stages)
            {
                var product=RecoveredGameRules.CashConfiguration(balance.rules,v.country).new_Fake_products[0];
                var player=new PlayerProgress {coin1024Number=v.coin,watch_video_count=v.videos,fakeMoney=v.money,loginDays=v.days};
                int step=RecoveredWithdrawalRules.Advance(player,product,v.current);double p=RecoveredWithdrawalRules.Progress(player,product,v.current);
                double expected=v.progress=="NaN"?double.NaN:double.Parse(v.progress,CultureInfo.InvariantCulture);
                if(step!=v.next||double.IsNaN(p)!=double.IsNaN(expected)||Math.Abs(p-expected)>1e-12||(p>=1)!=v.ready)throw new Exception("Original withdrawal stage differs "+v.country+" step "+v.current);
            }
            File.WriteAllText("../07_Verification/withdrawal_rules_comparison.json",JsonUtility.ToJson(new Report {passed=true,validators=all.validators.Length,money=all.money.Length,stages=all.stages.Length,scope="Complete recovered AccountCheckManager validators, GameManagement.getRealMonstr, GameRealWDDialog.setProgress and GameRealTXYZ.showNextWithdrawResult run offline on synthetic fixtures."},true));
            Debug.Log("WITHDRAWAL_SOURCE_COMPARISON_PASSED "+(all.validators.Length+all.money.Length+all.stages.Length));
        }
    }
}
