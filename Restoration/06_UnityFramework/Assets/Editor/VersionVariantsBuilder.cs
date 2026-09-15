using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace CoinMerge.Recovery.Editor
{
    public static class VersionVariantsBuilder
    {
        const string Runtime="Assets/Prefabs/Runtime/";
        static Font font;
        static readonly List<PackagedActionBinding> bindings=new List<PackagedActionBinding>();
        [Serializable] sealed class Point {public float[] values;}
        [Serializable] sealed class Polygon {public Point[] points;}
        [Serializable] sealed class DropTables {public PackagedDropTier[] firstAppear,appear;}
        [Serializable] sealed class LabelSettings {public bool _enableWrapText=true;public int overflow;}
        static Dictionary<int,GameObject> Nodes(GameObject root)
        {var map=new Dictionary<int,GameObject>();foreach(var n in root.GetComponentsInChildren<RecoveredNode>(true))map.Add(n.sourceObjectId,n.gameObject);return map;}
        internal static GameObject Source(string name,Transform parent)
        {
            var root=(GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Packaged/"+name+".prefab"),parent);
            PrefabUtility.UnpackPrefabInstance(root,PrefabUnpackMode.Completely,InteractionMode.AutomatedAction);root.SetActive(true);
            foreach(var n in root.GetComponentsInChildren<RecoveredNode>(true))foreach(var c in n.originalComponents)
            {
                if(c.type=="cc.Sprite")n.GetComponent<Image>().enabled=c.enabled;
                if(c.type=="cc.Label"||c.type=="cc.RichText")
                {
                    var text=n.GetComponent<Text>();text.enabled=c.enabled;
                    var settings=new LabelSettings();JsonUtility.FromJsonOverwrite(c.rawJson.Replace("\"_N$overflow\"","\"overflow\""),settings);
                    text.horizontalOverflow=settings.overflow!=0&&settings._enableWrapText?HorizontalWrapMode.Wrap:HorizontalWrapMode.Overflow;
                    text.resizeTextForBestFit=settings.overflow==2;text.resizeTextMinSize=1;text.resizeTextMaxSize=c.fontSize;
                    text.verticalOverflow=settings.overflow==2?VerticalWrapMode.Truncate:VerticalWrapMode.Overflow;
                    if(text.font&&text.font.lineHeight>0)text.lineSpacing=(float)c.lineHeight/c.fontSize*text.font.fontSize/text.font.lineHeight;
                }
            }
            foreach(var graphic in root.GetComponentsInChildren<Graphic>(true))graphic.raycastTarget=false;
            return root;
        }
        static Button Button(GameObject node)
        {var g=node.GetComponent<Graphic>();if(!g)throw new Exception("No visible button graphic: "+node.name);g.raycastTarget=true;var b=node.GetComponent<Button>()??node.AddComponent<Button>();b.targetGraphic=g;b.transition=Selectable.Transition.None;return b;}
        static void Bind(Dictionary<int,GameObject> n,int id,int action){bindings.Add(new PackagedActionBinding {button=Button(n[id]),action=action});}
        static Text TextAt(Dictionary<int,GameObject> n,int id)=>n[id].GetComponent<Text>();
        static string SpriteResource(Sprite sprite,string key)
        {
            if(!sprite)throw new Exception("Missing sprite "+key);
            string target="Assets/Resources/Packaged/"+key+".png";
            Directory.CreateDirectory(Path.GetDirectoryName(target));AssetDatabase.Refresh();
            if(!File.Exists(target))AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(sprite),target);
            return "Packaged/"+key;
        }
        static string SpriteUuid(string uuid,string key)
        {
            var import=JsonUtility.FromJson<SpriteImportModel>(File.ReadAllText("Assets/Resources/Recovered/sprite_import.json"));
            foreach(var entry in import.sprites)if(entry.uuid==uuid&&entry.variant=="Packaged")
                return SpriteResource(AssetDatabase.LoadAssetAtPath<Sprite>(entry.path),key);
            throw new Exception("Missing sprite uuid "+uuid);
        }
        static PackagedBalance Balance()
        {
            var balance=AssetDatabase.LoadAssetAtPath<PackagedBalance>("Assets/Config/Runtime/PackagedBalance.asset");
            if(!balance){balance=ScriptableObject.CreateInstance<PackagedBalance>();AssetDatabase.CreateAsset(balance,"Assets/Config/Runtime/PackagedBalance.asset");}
            var tables=JsonUtility.FromJson<DropTables>(File.ReadAllText("Assets/Config/Runtime/PackagedDropTables.json"));balance.firstAppear=tables.firstAppear;balance.appear=tables.appear;
            balance.coins=new PackagedCoinDefinition[11];
            for(int type=1;type<=11;type++)
            {
                var source=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Packaged/Block/block_"+type+".prefab");
                var nodes=Nodes(source);var rect=(RectTransform)nodes[1].transform;
                Polygon polygon=null;foreach(var c in nodes[1].GetComponent<RecoveredNode>().originalComponents)if(c.type=="cc.PhysicsPolygonCollider")polygon=JsonUtility.FromJson<Polygon>(c.rawJson);
                var points=new Vector2[polygon.points.Length];for(int p=0;p<points.Length;p++)points[p]=new Vector2(polygon.points[p].values[0],polygon.points[p].values[1])/balance.units;
                balance.coins[type-1]=new PackagedCoinDefinition {type=type,size=rect.sizeDelta,polygon=points,spritePath=SpriteResource(nodes[2].GetComponent<Image>().sprite,"Coins/"+type)};
            }
            EditorUtility.SetDirty(balance);return balance;
        }
        static PhysicsMaterial2D Material(string name,float friction,float bounce)
        {string path="Assets/Config/Runtime/"+name+".physicsMaterial2D";var value=AssetDatabase.LoadAssetAtPath<PhysicsMaterial2D>(path);if(!value){value=new PhysicsMaterial2D(name);AssetDatabase.CreateAsset(value,path);}value.friction=friction;value.bounciness=bounce;EditorUtility.SetDirty(value);return value;}
        [MenuItem("Coin Merge/Author content variants and GM popup")]
        public static void Run()
        {
            font=AssetDatabase.LoadAssetAtPath<Font>("Assets/Art/HotUpdate/Fonts/FZY4JW.ttf");
            var balance=Balance();AssetDatabase.SaveAssets();
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
            var coin=new GameObject("PackagedCoin");var coinScript=coin.AddComponent<PackagedCoin>();coinScript.body=coin.AddComponent<Rigidbody2D>();coinScript.body.simulated=false;
            coinScript.polygon=coin.AddComponent<PolygonCollider2D>();var visual=new GameObject("Sprite",typeof(SpriteRenderer));visual.transform.SetParent(coin.transform,false);coinScript.visual=visual.GetComponent<SpriteRenderer>();coinScript.visual.sortingOrder=0;
            var coinAsset=PrefabUtility.SaveAsPrefabAsset(coin,Runtime+"PackagedCoin.prefab");UnityEngine.Object.DestroyImmediate(coin);
            var root=new GameObject("RecoveredPackaged");var cameraRoot=new GameObject("WorldCamera",typeof(Camera));cameraRoot.transform.SetParent(root.transform,false);cameraRoot.transform.localPosition=new Vector3(0,0,-100);cameraRoot.tag="MainCamera";
            var camera=cameraRoot.GetComponent<Camera>();camera.orthographic=true;camera.orthographicSize=812/32f;camera.farClipPlane=200;camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=new Color(.12f,.16f,.26f);
            var ui=Source("Scene/GameScene",root.transform);var n=Nodes(ui);
            var canvas=n[4].GetComponent<Canvas>();canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=camera;canvas.planeDistance=100;canvas.sortingOrder=20;
            n[4].GetComponent<CanvasScaler>().referencePixelsPerUnit=32;
            var bg=n[38].AddComponent<Canvas>();bg.overrideSorting=true;bg.sortingOrder=-20;
            foreach(int id in new[]{1,13,14,23,24,42})n[id].SetActive(false);
            var boardRoot=new GameObject("PackagedBoard");boardRoot.transform.SetParent(root.transform,false);
            var board=boardRoot.AddComponent<PackagedBoard>();board.balance=balance;board.prefab=coinAsset.GetComponent<PackagedCoin>();board.deadline=n[39];
            board.freshMaterial=Material("PackagedCoinFresh",1,.2f);board.settledMaterial=Material("PackagedCoinSettled",1,0);
            board.coinRoot=new GameObject("Coins").transform;board.coinRoot.SetParent(boardRoot.transform,false);
            foreach(int id in new[]{13,14,23,24})
            {
                var source=n[id].GetComponent<RecoveredNode>();var rect=(RectTransform)source.transform;
                foreach(var c in source.originalComponents)if(c.type=="cc.PhysicsBoxCollider")
                {
                    var wall=new GameObject(source.name,typeof(BoxCollider2D));wall.transform.SetParent(boardRoot.transform,false);
                    wall.transform.localPosition=new Vector3(rect.anchoredPosition.x,(id==13||id==14?-812:rect.anchoredPosition.y)+812)/32f;
                    var box=wall.GetComponent<BoxCollider2D>();box.size=new Vector2(c.size[0],c.size[1])/32f;box.offset=new Vector2(c.offset[0],c.offset[1])/32f;
                    box.sharedMaterial=Material("PackagedWall",c.friction,c.restitution);
                }
            }
            var session=root.AddComponent<PackagedGameSession>();session.board=board;session.worldCamera=camera;session.home=n[2];session.map=n[3];
            cameraRoot.AddComponent<AudioListener>();var sound=new GameObject("PackagedAudio");sound.transform.SetParent(root.transform,false);session.audioCues=sound.AddComponent<PackagedAudio>();
            session.audioCues.music=sound.AddComponent<AudioSource>();session.audioCues.music.loop=true;session.audioCues.music.volume=.7f;session.audioCues.music.playOnAwake=false;
            session.audioCues.effects=sound.AddComponent<AudioSource>();session.audioCues.effects.playOnAwake=false;
            string[] audioNames={"BG","button","HeCheng","ShiBai"};session.audioCues.paths=new string[audioNames.Length];
            Directory.CreateDirectory("Assets/Resources/Packaged/Audio");AssetDatabase.Refresh();
            for(int i=0;i<audioNames.Length;i++){string path="Assets/Resources/Packaged/Audio/"+audioNames[i]+".mp3";if(!File.Exists(path))AssetDatabase.CopyAsset("Assets/Art/Packaged/Audio/Audio/"+audioNames[i]+".mp3",path);session.audioCues.paths[i]="Packaged/Audio/"+audioNames[i];}
            session.coinsText=TextAt(n,99);session.heartsText=TextAt(n,102);session.scoreText=TextAt(n,22);
            session.unlockedIconPath=SpriteUuid("6aa212c0-1a8a-4a5b-af7c-98df4e6f76e7","UI/Unlocked");session.lockedIconPath=SpriteUuid("6fa4fb6c-6f07-4b4b-ae5b-7955e06623a0","UI/Locked");
            bindings.Clear();int[] cards={6,11,7,8,9,10},prices={26,28,30,32,34,36},levels={69,74,78,83,88,93},amounts={72,74,81,86,91,96};
            session.modes=new PackagedModeBinding[6];
            for(int i=0;i<6;i++)
            {
                Bind(n,cards[i],100+i);
                // Source component references resolve explicitly against the metadata sourceId.
                var item=JsonUtility.FromJson<Item>(Array.Find(n[cards[i]].GetComponent<RecoveredNode>().originalComponents,c=>c.className=="Item").rawJson.Replace("$ref","reference"));
                Text level=null;foreach(var meta in ui.GetComponentsInChildren<RecoveredNode>(true))foreach(var c in meta.originalComponents)if(c.sourceId==item.lvel.reference)level=meta.GetComponent<Text>();
                session.modes[i]=new PackagedModeBinding {price=n[prices[i]],priceText=n[prices[i]].GetComponentInChildren<Text>(true),level=level,icon=n[44+i].GetComponent<Image>()};
            }
            Bind(n,50,3);Bind(n,51,4);Bind(n,17,5);Bind(n,18,6);Bind(n,40,7);Bind(n,41,8);
            AuthorDialogs(session,canvas.transform);
            session.actions=bindings.ToArray();
            var es=new GameObject("EventSystem",typeof(EventSystem),typeof(StandaloneInputModule));es.transform.SetParent(root.transform,false);
            var router=root.AddComponent<GameVersionRouter>();router.packaged=session;router.balance=AssetDatabase.LoadAssetAtPath<GameBalanceConfig>("Assets/Config/Runtime/GameBalance.asset");router.isRewarded=false;
            session.gm=AuthorGm(root.transform,camera,router);
            PrefabUtility.SaveAsPrefabAsset(root,Runtime+"RecoveredPackaged.prefab");EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(),"Assets/Scenes/RecoveredPackaged.unity");
            EditorSceneManager.OpenScene("Assets/Scenes/RecoveredMain.unity");
            var hot=UnityEngine.Object.FindObjectOfType<RecoveredGameSession>();
            if(hot.gm)UnityEngine.Object.DestroyImmediate(hot.gm.gameObject);
            router=hot.GetComponent<GameVersionRouter>()??hot.gameObject.AddComponent<GameVersionRouter>();router.rewarded=hot;router.balance=hot.board.Config;router.isRewarded=true;
            hot.gm=AuthorGm(hot.transform,hot.worldCamera,router);
            PrefabUtility.SaveAsPrefabAsset(hot.gameObject,Runtime+"RecoveredMain.prefab");EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
            var buildScenes=new List<EditorBuildSettingsScene>();
            if(File.Exists("Assets/Scenes/RecoveredLoading.unity"))buildScenes.Add(new EditorBuildSettingsScene("Assets/Scenes/RecoveredLoading.unity",true));
            buildScenes.Add(new EditorBuildSettingsScene("Assets/Scenes/RecoveredMain.unity",true));buildScenes.Add(new EditorBuildSettingsScene("Assets/Scenes/RecoveredPackaged.unity",true));
            EditorBuildSettings.scenes=buildScenes.ToArray();
            AssetDatabase.SaveAssets();Debug.Log("VERSION_VARIANTS_AUTHORED");
        }
        [Serializable] sealed class Reference {public int reference;}
        [Serializable] sealed class Item {public Reference lvel;}
        static Dictionary<int,GameObject> Dialog(string name,Transform parent,out GameObject root,int order)
        {
            root=Source("prefab/"+name,parent);var rect=(RectTransform)root.transform;rect.anchorMin=Vector2.zero;rect.anchorMax=Vector2.one;rect.offsetMin=rect.offsetMax=Vector2.zero;
            var layer=root.AddComponent<Canvas>();layer.overrideSorting=true;layer.sortingOrder=order;root.AddComponent<GraphicRaycaster>();
            var n=Nodes(root);
            foreach(var pair in n)if(pair.Value.name=="shadow")Button(pair.Value);
            root.SetActive(false);return n;
        }
        static void AuthorDialogs(PackagedGameSession s,Transform parent)
        {
            s.dialogs=new GameObject[7];Dictionary<int,GameObject> n;
            n=Dialog("GuideView",parent,out s.dialogs[0],100);
            // Source uses empty hit areas over rich-text links. Native Buttons own their visible labels.
            foreach(int id in new[]{11,12}){var link=n[id].AddComponent<Text>();link.font=font;link.fontSize=28;link.alignment=TextAnchor.MiddleCenter;link.color=new Color(.3f,.2f,.7f);link.text=id==11?"Privacy Policy":"User Agreement";}
            var paragraph=TextAt(n,10);paragraph.text="Before you begin playing, please take a moment to review our";
            paragraph.horizontalOverflow=HorizontalWrapMode.Wrap;paragraph.fontSize=30;paragraph.alignment=TextAnchor.MiddleCenter;
            paragraph.rectTransform.sizeDelta=new Vector2(580,108);paragraph.rectTransform.anchoredPosition=new Vector2(0,130);
            foreach(int id in new[]{11,12}){var rect=(RectTransform)n[id].transform;rect.anchoredPosition=new Vector2(rect.anchoredPosition.x,35);}
            var conclusion=Label("AgreementConclusion",n[2].transform,"Click 'Agree' after reading to start your game.",new Vector2(580,70),new Vector2(0,-40),26);conclusion.color=paragraph.color;conclusion.horizontalOverflow=HorizontalWrapMode.Overflow;
            Bind(n,5,1);Bind(n,8,2);Bind(n,11,12);Bind(n,12,13);
            n=Dialog("SettingView",parent,out s.dialogs[1],100);Bind(n,10,0);Bind(n,7,11);Bind(n,12,13);Bind(n,13,12);s.musicButton=n[7].GetComponent<Image>();s.musicKnob=(RectTransform)n[11].transform;
            n=Dialog("HonorView",parent,out s.dialogs[2],100);Bind(n,23,0);s.honors=new PackagedHonorBinding[7];
            for(int i=0;i<7;i++){Bind(n,3+i,200+i);s.honors[i]=new PackagedHonorBinding {price=n[16+i],finish=n[24+i*2],check=n[25+i*2]};}
            n=Dialog("InfoView",parent,out s.dialogs[3],100);Bind(n,9,0);s.infoCoins=TextAt(n,12);s.infoHearts=TextAt(n,15);
            n=Dialog("AddHeartView",parent,out s.dialogs[4],110);Bind(n,8,0);Bind(n,3,9);s.heartTime=TextAt(n,14);s.heartCount=TextAt(n,16);
            n=Dialog("FailureView",parent,out s.dialogs[5],100);Bind(n,8,7);Bind(n,11,7);Bind(n,6,10);
            n=Dialog("PolicyView",parent,out s.dialogs[6],120);Bind(n,9,0);s.policyImage=n[10].GetComponent<Image>();s.policyTitle=TextAt(n,5);
            s.policyPaths=new[]{SpriteUuid("419864ef-eb77-46f5-83d7-04ed1ee13d6c","UI/Privacy"),SpriteUuid("c3ba03e3-ceec-4e69-a0ed-67c2ef2b6811","UI/Agreement")};
            n=Dialog("ToastView",parent,out s.toast,150);s.toastText=s.toast.GetComponentInChildren<Text>(true);
        }
        static GameObject Rect(string name,Transform parent,Vector2 size,Vector2 position)
        {var go=new GameObject(name,typeof(RectTransform));go.transform.SetParent(parent,false);var rect=(RectTransform)go.transform;rect.sizeDelta=size;rect.anchoredPosition=position;return go;}
        static Text Label(string name,Transform parent,string value,Vector2 size,Vector2 position,int fontSize=26)
        {var go=Rect(name,parent,size,position);var text=go.AddComponent<Text>();text.font=font;text.fontSize=fontSize;text.text=value;text.color=Color.white;text.alignment=TextAnchor.MiddleCenter;text.raycastTarget=false;return text;}
        static Button Control(string name,Transform parent,string label,Vector2 size,Vector2 position,out Text text)
        {var go=Rect(name,parent,size,position);go.AddComponent<Image>().color=new Color(.2f,.29f,.46f);var b=Button(go);text=Label("Label",go.transform,label,size-Vector2.one*8,Vector2.zero,25);return b;}
        static VersionGmPanel AuthorGm(Transform parent,Camera camera,GameVersionRouter router)
        {
            var root=Rect("VersionGM",parent,new Vector2(750,1624),Vector2.zero);
            var canvas=root.AddComponent<Canvas>();canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=camera;canvas.planeDistance=100;canvas.sortingOrder=1000;
            var scaler=root.AddComponent<CanvasScaler>();scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;scaler.referenceResolution=new Vector2(750,1624);scaler.matchWidthOrHeight=1;root.AddComponent<GraphicRaycaster>();
            var panel=root.AddComponent<VersionGmPanel>();panel.router=router;
            panel.open=Control("OpenGM",root.transform,"GM",new Vector2(84,50),new Vector2(315,-580),out var unused);
            var popup=Rect("Popup",root.transform,new Vector2(750,1624),Vector2.zero);panel.popup=popup;
            var backdrop=Rect("Backdrop",popup.transform,new Vector2(750,1624),Vector2.zero);var br=(RectTransform)backdrop.transform;br.anchorMin=Vector2.zero;br.anchorMax=Vector2.one;br.offsetMin=br.offsetMax=Vector2.zero;
            backdrop.AddComponent<Image>().color=new Color(0,0,0,.62f);panel.backdrop=Button(backdrop);
            var card=Rect("Card",popup.transform,new Vector2(650,920),Vector2.zero);card.AddComponent<Image>().color=new Color(.085f,.12f,.21f);Button(card);
            Label("Title",card.transform,"GM / 版本测试",new Vector2(480,65),new Vector2(-35,390),34);
            panel.close=Control("Close",card.transform,"X",new Vector2(55,55),new Vector2(275,390),out unused);
            Label("ContentTitle",card.transform,"玩法版本（切换实际场景）",new Vector2(590,35),new Vector2(0,315));
            panel.baseVersion=Control("VersionA_Base",card.transform,"",new Vector2(280,58),new Vector2(-150,260),out panel.baseValue);
            panel.rewardedVersion=Control("VersionB_Rewards",card.transform,"",new Vector2(280,58),new Vector2(150,260),out panel.rewardedValue);
            Label("LocalVersionNames",card.transform,"A/B 为本地测试名，不代表原包买量归因",new Vector2(590,32),new Vector2(0,210),22);
            panel.content=Control("AutomaticRouting",card.transform,"",new Vector2(580,50),new Vector2(0,155),out panel.contentValue);
            Label("CohortTitle",card.transform,"原包 AB 标记（只读）",new Vector2(285,35),new Vector2(-150,85),23);
            Label("RegionTitle",card.transform,"国家 / 地区",new Vector2(280,35),new Vector2(150,85));
            panel.cohortValue=Label("OriginalCohortRecord",card.transform,"",new Vector2(295,58),new Vector2(-150,30),20);
            panel.regionPrevious=Control("PreviousCountry",card.transform,"<",new Vector2(58,58),new Vector2(50,30),out unused);
            panel.regionValue=Label("国家 / 地区",card.transform,"US",new Vector2(100,58),new Vector2(150,30));
            panel.regionNext=Control("NextCountry",card.transform,">",new Vector2(58,58),new Vector2(250,30),out unused);
            Label("ShareTitle",card.transform,"自动分流：本地测试比例",new Vector2(580,38),new Vector2(0,-40));
            panel.shareDown=Control("LessRewards",card.transform,"-",new Vector2(65,58),new Vector2(-250,-100),out unused);
            panel.shareUp=Control("MoreRewards",card.transform,"+",new Vector2(65,58),new Vector2(250,-100),out unused);
            panel.shareValue=Label("Share",card.transform,"",new Vector2(360,58),new Vector2(0,-100),23);
            panel.status=Label("Status",card.transform,"",new Vector2(590,145),new Vector2(0,-215),23);
            panel.reset=Control("ResetCurrentSave",card.transform,"清除当前版本的玩家存档",new Vector2(580,50),new Vector2(0,-320),out unused);
            panel.apply=Control("Apply",card.transform,"应用并重新进入",new Vector2(580,65),new Vector2(0,-400),out unused);
            popup.SetActive(false);
            // Standalone reusable UI asset; scene-specific router is wired in the scene, never through reflection.
            var saved=panel.router;panel.router=null;PrefabUtility.SaveAsPrefabAsset(root,Runtime+"VersionGM.prefab");panel.router=saved;
            return panel;
        }
        [MenuItem("Coin Merge/Update GM version selection only")]
        public static void RebuildGmOnly()
        {
            font=AssetDatabase.LoadAssetAtPath<Font>("Assets/Art/HotUpdate/Fonts/FZY4JW.ttf");
            foreach(string name in new[]{"RecoveredMain","RecoveredPackaged"})
            {
                string path=Runtime+name+".prefab";var prefab=PrefabUtility.LoadPrefabContents(path);
                try{ReplaceGm(prefab);PrefabUtility.SaveAsPrefabAsset(prefab,path);}finally{PrefabUtility.UnloadPrefabContents(prefab);}
                var scene=EditorSceneManager.OpenScene("Assets/Scenes/"+name+".unity");
                foreach(var root in scene.GetRootGameObjects())if(root.GetComponent<GameVersionRouter>())ReplaceGm(root);
                EditorSceneManager.SaveScene(scene);
            }
            AssetDatabase.SaveAssets();Debug.Log("GM_VERSION_SELECTION_UPDATED");
        }
        static void ReplaceGm(GameObject root)
        {
            var router=root.GetComponent<GameVersionRouter>();var previous=router.isRewarded?router.rewarded.gm:router.packaged.gm;
            if(previous)UnityEngine.Object.DestroyImmediate(previous.gameObject);
            var panel=AuthorGm(root.transform,router.isRewarded?router.rewarded.worldCamera:router.packaged.worldCamera,router);
            if(router.isRewarded)router.rewarded.gm=panel;else router.packaged.gm=panel;
        }
    }
}
