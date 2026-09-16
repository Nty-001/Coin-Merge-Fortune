using System;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace CoinMerge.Recovery.Editor
{
    public static class VersionRulesValidation
    {
        [Serializable] sealed class Tail {public string input,cohort;}
        [Serializable] sealed class Drop {public int pass,max,draw,type;}
        [Serializable] sealed class Vectors {public Tail[] tails;public Drop[] drops;public string abBodySha256,dropBodySha256;}
        [Serializable] sealed class Report {public bool passed;public int sourceTailVectors,sourceDropVectors;public string abBodySha256,dropBodySha256;}
        public static void Run()
        {
            var data=JsonUtility.FromJson<Vectors>(File.ReadAllText("Assets/Config/Runtime/VersionRuleVectors.json"));
            var balance=AssetDatabase.LoadAssetAtPath<PackagedBalance>("Assets/Config/Runtime/PackagedBalance.asset");
            foreach(var tail in data.tails)if(VersionRouting.OriginalCohort(tail.input)!=tail.cohort)throw new Exception("AB tail differs: "+tail.input);
            foreach(var drop in data.drops){var player=new PackagedPlayer {mergedMaxLv=drop.max};player.passlevel[0]=drop.pass;if(balance.NextType(player,drop.draw)!=drop.type)throw new Exception("Packaged drop differs: "+JsonUtility.ToJson(drop));}
            File.WriteAllText("../07_Verification/version_rules_validation.json",JsonUtility.ToJson(new Report {passed=true,sourceTailVectors=data.tails.Length,sourceDropVectors=data.drops.Length,abBodySha256=data.abBodySha256,dropBodySha256=data.dropBodySha256},true));
            Debug.Log("VERSION_SOURCE_RULES_VALIDATED "+data.tails.Length+" tails + "+data.drops.Length+" drop vectors");
        }
    }
}
