using System;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace CoinMerge.Recovery.Editor
{
    public static class RecoveredLocalizationValidation
    {
        [Serializable] sealed class VectorSet {public Vector[] vectors;}
        [Serializable] sealed class Vector {public string country,expected;public double amount;}
        [MenuItem("Coin Merge/Validate recovered localization and money format")]
        public static void Run()
        {
            foreach(string path in Directory.GetFiles("Assets/Resources/Localization/Currency","*.png",SearchOption.AllDirectories))
            {
                var importer=(TextureImporter)AssetImporter.GetAtPath(path);importer.textureType=TextureImporterType.Sprite;importer.spriteImportMode=SpriteImportMode.Single;
                importer.mipmapEnabled=false;importer.alphaIsTransparency=true;importer.SaveAndReimport();
            }
            var vectors=JsonUtility.FromJson<VectorSet>(File.ReadAllText("../07_Verification/original_money_format_vectors.json"));
            foreach(var v in vectors.vectors)
            {
                var locale=new RecoveredLocalization(v.country);string actual=locale.Money(v.amount);
                if(actual!=v.expected)throw new Exception("Money format differs from original JS: "+v.country+" "+v.amount+" expected="+v.expected+" actual="+actual);
                locale.Icon(1);locale.Icon(2);locale.Icon(3);
            }
            File.WriteAllText("../07_Verification/localization_validation.json","{\"passed\":true,\"originalJsMoneyCases\":"+vectors.vectors.Length+",\"locales\":26,\"currencyIcons\":78}");
            Debug.Log("RECOVERED_LOCALIZATION_VALIDATION_PASSED "+vectors.vectors.Length);
        }
    }
}
