using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
namespace CoinMerge.Recovery.Editor
{
    public static class RecoveredVisualsBuilder
    {
        [Serializable] sealed class RichSettings {public int fontSize=28,lineHeight=30,maxWidth=540;}
        [MenuItem("Coin Merge/Restore notice, round text strokes and startup")]
        public static void Run()
        {
            UpgradeOutlineAssets();
            const string path="Assets/Prefabs/Runtime/RecoveredMain.prefab";
            var root=PrefabUtility.LoadPrefabContents(path);
            try{AuthorNotice(root);PrefabUtility.SaveAsPrefabAsset(root,path);}finally{PrefabUtility.UnloadPrefabContents(root);}
            var main=EditorSceneManager.OpenScene("Assets/Scenes/RecoveredMain.unity");
            foreach(var item in main.GetRootGameObjects())if(item.GetComponent<RecoveredGameSession>())AuthorNotice(item);
            EditorSceneManager.SaveScene(main);
            AuthorLoading();AssetDatabase.SaveAssets();
            Debug.Log("RECOVERED_NOTICE_OUTLINES_STARTUP_AUTHORED");
        }
        static void UpgradeOutlineAssets()
        {
            // Change only the script GUID. Existing fileIDs, widths, colors, enabled flags and form references survive.
            string guid=AssetDatabase.AssetPathToGUID("Assets/Scripts/UI/RecoveredRoundOutline.cs");
            if(string.IsNullOrEmpty(guid))throw new Exception("Round outline script not imported");
            int changed=0;
            foreach(string directory in new[]{"Assets/Prefabs","Assets/Scenes","Assets/Resources/Startup"})
            {
                if(!Directory.Exists(directory))continue;
                foreach(string file in Directory.GetFiles(directory,"*",SearchOption.AllDirectories))
                {
                    if(!file.EndsWith(".prefab",StringComparison.Ordinal)&&!file.EndsWith(".unity",StringComparison.Ordinal))continue;
                    string old=File.ReadAllText(file),updated=old.Replace("e19747de3f5aca642ab2be37e372fb86",guid);
                    if(old==updated)continue;File.WriteAllText(file,updated,new UTF8Encoding(false));changed++;
                }
            }
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            Debug.Log("ROUND_OUTLINE_ASSETS_UPDATED "+changed);
        }
        static void AuthorNotice(GameObject root)
        {
            var session=root.GetComponent<RecoveredGameSession>();
            var nodes=new Dictionary<int,GameObject>();string uuid=session.moneyText.GetComponent<RecoveredNode>().sourceUuid;
            foreach(var node in root.GetComponentsInChildren<RecoveredNode>(true))if(node.sourceUuid==uuid)nodes.Add(node.sourceObjectId,node.gameObject);
            var text=nodes[39].GetComponent<Text>();var settings=new RichSettings();
            foreach(var c in nodes[39].GetComponent<RecoveredNode>().originalComponents)
                if(c.type=="cc.RichText")JsonUtility.FromJsonOverwrite(c.rawJson.Replace("_N$fontSize","fontSize").Replace("_N$lineHeight","lineHeight").Replace("_N$maxWidth","maxWidth"),settings);
            text.fontSize=settings.fontSize;text.resizeTextForBestFit=false;text.horizontalOverflow=HorizontalWrapMode.Wrap;
            text.verticalOverflow=VerticalWrapMode.Overflow;text.alignment=TextAnchor.MiddleCenter;text.supportRichText=true;text.raycastTarget=false;
            text.lineSpacing=(float)settings.lineHeight/settings.fontSize*text.font.fontSize/text.font.lineHeight;
            text.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,settings.maxWidth);
            text.text="";
            var notice=nodes[23].GetComponent<RecoveredNoticeTicker>()??nodes[23].AddComponent<RecoveredNoticeTicker>();
            notice.content=(RectTransform)nodes[23].transform;notice.label=text;notice.interval=4;notice.moveDuration=.9f;notice.distance=110;
            notice.minimumDays=2;notice.maximumDays=10;notice.localeKey="109";session.notice=notice;
        }
        static void AuthorLoading()
        {
            Directory.CreateDirectory("Assets/Resources/Startup");AssetDatabase.Refresh();
            var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
            var root=new GameObject("RecoveredStartup");var startup=root.AddComponent<RecoveredStartup>();
            startup.balance=AssetDatabase.LoadAssetAtPath<GameBalanceConfig>("Assets/Config/Runtime/GameBalance.asset");
            var cameraRoot=new GameObject("StartupCamera",typeof(Camera));cameraRoot.transform.SetParent(root.transform,false);
            var camera=cameraRoot.GetComponent<Camera>();camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=new Color(.12f,.16f,.26f);camera.cullingMask=0;
            BuildView(true);BuildView(false);
            EditorSceneManager.SaveScene(scene,"Assets/Scenes/RecoveredLoading.unity");
            EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene("Assets/Scenes/RecoveredLoading.unity",true),
                new EditorBuildSettingsScene("Assets/Scenes/RecoveredMain.unity",true),new EditorBuildSettingsScene("Assets/Scenes/RecoveredPackaged.unity",true)};
        }
        static void BuildView(bool rewarded)
        {
            var view=rewarded?NativeGameplayBuilder.Source("scene/LoadingScene",null):VersionVariantsBuilder.Source("Scene/LoadScene",null);
            var nodes=NativeGameplayBuilder.Nodes(view);var canvas=nodes[1].GetComponent<Canvas>();
            canvas.renderMode=RenderMode.ScreenSpaceOverlay;canvas.sortingOrder=1000;
            foreach(var camera in view.GetComponentsInChildren<Camera>(true))UnityEngine.Object.DestroyImmediate(camera);
            foreach(var graphic in view.GetComponentsInChildren<Graphic>(true))graphic.raycastTarget=false;
            var old=rewarded?nodes[2].GetComponent<Slider>():null;if(old)UnityEngine.Object.DestroyImmediate(old);
            var loading=view.AddComponent<RecoveredLoadingView>();loading.fill=nodes[rewarded?9:8].GetComponent<Image>();
            loading.fill.type=Image.Type.Filled;loading.fill.fillMethod=Image.FillMethod.Horizontal;loading.fill.fillOrigin=0;loading.fill.fillAmount=0;
            if(rewarded){loading.percentage=nodes[5].GetComponent<Text>();loading.percentage.text="0%";}
            PrefabUtility.SaveAsPrefabAsset(view,"Assets/Resources/Startup/"+(rewarded?"RewardedLoading":"PackagedLoading")+".prefab");
            UnityEngine.Object.DestroyImmediate(view);
        }
    }
}
