using System;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace CoinMerge.Recovery.Editor
{
    public static class RecoveredRulesValidation
    {
        [Serializable] public sealed class VectorSet { public RuleVector[] vectors; }
        [Serializable] public sealed class RuleVector { public string kind,country;public int maximum,watched,count,score;public double balance,sample,expected; }
        [Serializable] public sealed class Result {public string status;public int originalJsCases,stateChecks;public string scope;}
        public static void Run()
        {
            var rules=JsonUtility.FromJson<RecoveredRulesData>(File.ReadAllText("Assets/Config/Runtime/RecoveredRules.json"));
            var vectors=JsonUtility.FromJson<VectorSet>(File.ReadAllText("../07_Verification/original_rule_vectors.json"));
            foreach(var v in vectors.vectors)
            {
                double actual;
                switch(v.kind)
                {
                    case "drop":actual=RecoveredGameRules.PickDrop(rules,v.maximum,v.sample);break;
                    case "lottery":actual=RecoveredGameRules.PickLottery(rules.lotteryRewards,v.sample);break;
                    case "cash":actual=RecoveredGameRules.CalculateCash(v.balance,RecoveredGameRules.CashConfiguration(rules,v.country),v.sample);break;
                    case "video":actual=RecoveredGameRules.VideoQ(rules.videoRewards,v.watched,v.sample);break;
                    case "spins":actual=RecoveredGameRules.AvailableSpins(rules.lotteryScores,v.score,v.count);break;
                    case "score":actual=RecoveredGameRules.RequiredScore(rules.lotteryScores,v.count);break;
                    default:throw new InvalidOperationException("Unknown vector kind");
                }
                if(Math.Abs(actual-v.expected)>1e-8)throw new InvalidOperationException("Original JS difference: "+v.kind+" country="+v.country+" balance="+v.balance+" sample="+v.sample+" expected="+v.expected+" actual="+actual);
            }
            int checks=0;
            var progress=new PlayerProgress();
            Require(!progress.RecordLoginDay("2026-09-14",5),"Opening game is not a qualifying login day");checks++;
            for(int i=0;i<4;i++)progress.AddHighestCoinMerge("2026-09-14",5);
            Require(progress.loginDays==0,"Four merges do not qualify");checks++;
            progress.AddHighestCoinMerge("2026-09-14",5);Require(progress.loginDays==1,"Fifth merge qualifies");checks++;
            progress.AddHighestCoinMerge("2026-09-14",5);Require(progress.loginDays==1,"Same day is not counted twice");checks++;
            Require(!progress.RecordLoginDay("2026-09-15",5)&&progress.today1024NumberCoin==0,"New day resets merge requirement");checks++;
            progress.gameTotalScore=150;progress.roundScore=100;progress.fakeMoney=234.28;
            progress.ResetBoardAfterFailure();Require(progress.roundScore==0&&progress.gameTotalScore==150&&progress.fakeMoney==234.28,"Failure reset retains progress and balance");checks++;
            progress=new PlayerProgress {savedCoinsScore=19};progress.NormalizeScoreFields(false,false,false);
            Require(progress.roundScore==19&&progress.gameTotalScore==19,"Legacy score migration");checks++;
            Require(RecoveredGameRules.ClientCohort(null)=="B"&&RecoveredGameRules.ClientCohort("fixture-4")=="A"&&RecoveredGameRules.ClientCohort("fixture-5")=="B","Original client suffix cohorts");checks++;
            Require(RecoveredGameRules.NormalizeCountry(" cn ",rules.supportedCountries)=="US","Original CN normalization");checks++;
            Directory.CreateDirectory("Assets/Config/Runtime");
            const string assetPath="Assets/Config/Runtime/GameBalance.asset";
            var asset=AssetDatabase.LoadAssetAtPath<GameBalanceConfig>(assetPath);
            if(asset==null){asset=ScriptableObject.CreateInstance<GameBalanceConfig>();AssetDatabase.CreateAsset(asset,assetPath);}
            asset.rules=rules;EditorUtility.SetDirty(asset);
            foreach(var coin in rules.coins)
            {
                var importer=AssetImporter.GetAtPath("Assets/Resources/"+coin.spritePath+".png") as TextureImporter;
                if(importer==null)throw new InvalidOperationException("Missing coin texture: "+coin.spritePath);
                importer.textureType=TextureImporterType.Sprite;importer.spriteImportMode=SpriteImportMode.Single;
                importer.spritePixelsPerUnit=asset.pixelsPerUnit;importer.mipmapEnabled=false;importer.alphaIsTransparency=true;
                importer.SaveAndReimport();
            }
            AssetDatabase.SaveAssets();
            File.WriteAllText("../07_Verification/csharp_original_rule_comparison.json",JsonUtility.ToJson(new Result {status="passed",originalJsCases=vectors.vectors.Length,stateChecks=checks,scope="Rules and state transitions only; gameplay scene/visual parity is a separate check."},true));
            Debug.Log("ORIGINAL_JS_RULE_COMPARISON_PASSED "+vectors.vectors.Length+" vectors; "+checks+" state checks");
        }
        static void Require(bool condition,string label){if(!condition)throw new InvalidOperationException(label);}
    }
}
