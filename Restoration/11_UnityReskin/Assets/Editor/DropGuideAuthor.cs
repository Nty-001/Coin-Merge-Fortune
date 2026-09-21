using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace CoinMerge.Recovery.Editor
{
    public static class DropGuideAuthor
    {
        const string DotPath="Assets/Resources/GameplayGuide/Dot.png";
        static void Apply(GameObject root,Sprite sprite)
        {
            var session=root.GetComponent<RecoveredGameSession>();
            var guide=root.GetComponentInChildren<RecoveredDropGuide>(true);
            if(!guide)
            {
                var go=new GameObject("DropAlignmentGuide");go.transform.SetParent(session.board.transform,false);
                guide=go.AddComponent<RecoveredDropGuide>();guide.session=session;guide.dots=new SpriteRenderer[128];
                for(int i=0;i<guide.dots.Length;i++)
                {
                    var dot=new GameObject("Dot"+i.ToString("D3"));dot.transform.SetParent(go.transform,false);
                    var renderer=dot.AddComponent<SpriteRenderer>();renderer.sprite=sprite;renderer.color=new Color32(255,255,255,180);
                    renderer.sortingLayerID=session.board.coinPrefab.visual.sortingLayerID;
                    renderer.sortingOrder=session.board.coinPrefab.visual.sortingOrder-1;
                    dot.transform.localScale=Vector3.one*(10/session.board.Units);
                    renderer.enabled=false;guide.dots[i]=renderer;
                }
            }
            guide.spacingPixels=30;
        }
        public static void Author()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(DotPath));
            var texture=new Texture2D(32,32,TextureFormat.RGBA32,false);
            for(int y=0;y<32;y++)for(int x=0;x<32;x++)
            {
                float distance=Vector2.Distance(new Vector2(x+.5f,y+.5f),new Vector2(16,16));
                texture.SetPixel(x,y,new Color(1,1,1,Mathf.Clamp01(16-distance)));
            }
            texture.Apply();File.WriteAllBytes(DotPath,texture.EncodeToPNG());Object.DestroyImmediate(texture);
            AssetDatabase.ImportAsset(DotPath);
            var importer=(TextureImporter)AssetImporter.GetAtPath(DotPath);importer.textureType=TextureImporterType.Sprite;
            importer.spritePixelsPerUnit=32;importer.mipmapEnabled=false;importer.alphaIsTransparency=true;importer.textureCompression=TextureImporterCompression.Uncompressed;importer.SaveAndReimport();
            var sprite=AssetDatabase.LoadAssetAtPath<Sprite>(DotPath);
            const string prefab="Assets/Prefabs/Runtime/RecoveredMain.prefab";
            var root=PrefabUtility.LoadPrefabContents(prefab);
            try{Apply(root,sprite);PrefabUtility.SaveAsPrefabAsset(root,prefab);}finally{PrefabUtility.UnloadPrefabContents(root);}
            var scene=EditorSceneManager.OpenScene("Assets/Scenes/RecoveredMain.unity");
            foreach(var go in scene.GetRootGameObjects())if(go.GetComponent<RecoveredGameSession>())Apply(go,sprite);
            EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();
        }
    }
}
