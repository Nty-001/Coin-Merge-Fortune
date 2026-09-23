using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace CoinMerge.Recovery.Editor
{
    public static class RewardCoinClarityAuthor
    {
        [Serializable] sealed class Source { public Skin[] skins; }
        [Serializable] sealed class Skin { public string name; public Attachments attachments; }
        [Serializable] sealed class Attachments { public CoinSlot an_00; }
        [Serializable] sealed class CoinSlot { public CoinMesh an_00; }
        [Serializable] sealed class CoinMesh { public float[] uvs; }

        [MenuItem("Coin Merge/Reskin/Use full resolution reward coin")]
        public static void Run()
        {
            if(EditorApplication.isPlayingOrWillChangePlaymode)
                throw new InvalidOperationException("Author the reward coin in Edit Mode.");
            foreach(int value in new[]{1,2,5,10,20,50,100,200,500,1000,2000})
                Lossless("Assets/Resources/Gameplay/Coins/"+value+".png");
            Lossless("Assets/Resources/RulesReskin/Chip2000.png");
            var source=JsonUtility.FromJson<Source>(File.ReadAllText("Assets/Art/HotUpdate/SkeletalReusable/AnNiu_TX/skeleton.json"));
            float[] raw=null;
            foreach(var skin in source.skins)if(skin.name=="default")raw=skin.attachments.an_00.an_00.uvs;
            if(raw==null||raw.Length%2!=0)throw new InvalidOperationException("Missing original an_00 mesh UVs.");
            var uvs=new Vector2[raw.Length/2];
            // Source mesh UVs use a top-left origin, before atlas packing rotation.
            // Sample the upright full-resolution replacement with Unity's bottom-left origin.
            for(int i=0;i<uvs.Length;i++)uvs[i]=new Vector2(raw[i*2],1-raw[i*2+1]);
            foreach(string path in new[]{"Assets/Prefabs/Runtime/RecoveredMain.prefab","Assets/Prefabs/Runtime/Skeletal/AnNiu_TX.prefab"})
            {
                var root=PrefabUtility.LoadPrefabContents(path);
                try{Apply(root,uvs);PrefabUtility.SaveAsPrefabAsset(root,path);}
                finally{PrefabUtility.UnloadPrefabContents(root);}
            }
            var scene=EditorSceneManager.OpenScene("Assets/Scenes/RecoveredMain.unity");
            foreach(var root in scene.GetRootGameObjects())if(root.GetComponent<RecoveredGameSession>())Apply(root,uvs);
            EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();
            Debug.Log("REWARD_COIN_CLARITY_AUTHORED");
        }

        static void Lossless(string path)
        {
            var importer=(TextureImporter)AssetImporter.GetAtPath(path);
            float ppu=importer.spritePixelsPerUnit;
            importer.textureCompression=TextureImporterCompression.Uncompressed;
            importer.crunchedCompression=false;
            importer.mipmapEnabled=false;
            importer.npotScale=TextureImporterNPOTScale.None;
            importer.maxTextureSize=2048;
            var settings=importer.GetDefaultPlatformTextureSettings();
            settings.format=TextureImporterFormat.RGBA32;
            settings.textureCompression=TextureImporterCompression.Uncompressed;
            settings.crunchedCompression=false;settings.maxTextureSize=2048;
            importer.SetPlatformTextureSettings(settings);
            foreach(string platform in new[]{"Standalone","Android","iPhone"})importer.ClearPlatformTextureSettings(platform);
            importer.SaveAndReimport();
            if(importer.spritePixelsPerUnit!=ppu)throw new InvalidOperationException("Coin size must not change.");
        }

        static void Apply(GameObject root,Vector2[] uvs)
        {
            int changed=0;
            foreach(var player in root.GetComponentsInChildren<NativeSkeletonPlayer>(true))
            {
                if(player.dataPath!="Skeletal/Data/AnNiu_TX")continue;
                var data=Resources.Load<NativeSkeletonData>(player.dataPath);
                int index=Array.FindIndex(data.attachments,item=>item.name=="an_00");
                if(index<0||data.attachments[index].VertexCount!=uvs.Length)throw new InvalidOperationException("Coin mesh differs from source.");
                player.graphic.replacementAttachment=index;
                player.graphic.replacementTexturePath="RulesReskin/Chip2000";
                player.graphic.replacementUVs=(Vector2[])uvs.Clone();
                changed++;
            }
            if(changed!=1)throw new InvalidOperationException("Expected one home withdrawal icon, found "+changed);
        }
    }
}
