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
    // Explicit asset authoring operation. Never called by runtime or scene initialization.
    public static class NativeGameplayBuilder
    {
        const string RuntimePath="Assets/Prefabs/Runtime";
        [Serializable] sealed class LabelSettings {public bool _enableWrapText=true;public int overflow;}
        [Serializable] sealed class CoinLoaderSettings {public int coin_type=1;}
        static readonly List<LocalizedLabelBinding> Labels=new List<LocalizedLabelBinding>();
        static void Label(Dictionary<int,GameObject> nodes,int id,string key)
        {
            var text=Component<Text>(nodes,id);Labels.Add(new LocalizedLabelBinding {label=text,key=key});
            text.text=new RecoveredLocalization("US").Label(key);
        }
        static Dictionary<int,GameObject> Nodes(GameObject root)
        {
            var nodes=new Dictionary<int,GameObject>();
            foreach(var n in root.GetComponentsInChildren<RecoveredNode>(true))nodes.Add(n.sourceObjectId,n.gameObject);
            return nodes;
        }
        static T Component<T>(Dictionary<int,GameObject> nodes,int id) where T:UnityEngine.Component
        {
            var component=nodes[id].GetComponent<T>();
            if(!component)throw new InvalidOperationException("Missing "+typeof(T).Name+" source node "+id);
            return component;
        }
        static Button Button(GameObject node)
        {
            var graphic=node.GetComponent<Graphic>();
            if(!graphic)throw new InvalidOperationException("Button must be on its visible source graphic: "+node.name);
            graphic.raycastTarget=true;
            var button=node.GetComponent<Button>()??node.AddComponent<Button>();button.targetGraphic=graphic;
            button.transition=Selectable.Transition.None;return button;
        }
        static GameObject Source(string path,Transform parent)
        {
            var source=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/HotUpdate/"+path+".prefab");
            if(!source)throw new InvalidOperationException("Missing original prefab: "+path);
            var root=(GameObject)PrefabUtility.InstantiatePrefab(source,parent);
            PrefabUtility.UnpackPrefabInstance(root,PrefabUnpackMode.Completely,InteractionMode.AutomatedAction);
            foreach(var node in root.GetComponentsInChildren<RecoveredNode>(true))
                foreach(var component in node.originalComponents)
                {
                    if(component.type=="cc.Sprite")node.GetComponent<Image>().enabled=component.enabled;
                    if(component.type=="cc.Label"||component.type=="cc.RichText")
                    {
                        var text=node.GetComponent<Text>();text.enabled=component.enabled;
                        var settings=new LabelSettings();JsonUtility.FromJsonOverwrite(component.rawJson.Replace("\"_N$overflow\"","\"overflow\""),settings);
                        text.horizontalOverflow=settings.overflow!=0&&settings._enableWrapText?HorizontalWrapMode.Wrap:HorizontalWrapMode.Overflow;
                        text.resizeTextForBestFit=settings.overflow==2;text.resizeTextMinSize=1;text.resizeTextMaxSize=component.fontSize;
                        text.verticalOverflow=settings.overflow==2?VerticalWrapMode.Truncate:VerticalWrapMode.Overflow;
                        if(text.font&&text.font.lineHeight>0)text.lineSpacing=(float)component.lineHeight/component.fontSize*text.font.fontSize/text.font.lineHeight;
                    }
                }
            return root;
        }
        static Dictionary<int,GameObject> Dialog(string name,Transform parent,out GameObject root)
        {
            root=Source("GameDialog/"+name,parent);
            var rect=(RectTransform)root.transform;rect.anchorMin=Vector2.zero;rect.anchorMax=Vector2.one;rect.offsetMin=rect.offsetMax=Vector2.zero;
            var layer=root.AddComponent<Canvas>();layer.overrideSorting=true;layer.sortingOrder=100;
            root.AddComponent<GraphicRaycaster>();
            return Nodes(root);
        }
        [MenuItem("Coin Merge/Author native main gameplay assets")]
        public static void Run()
        {
            Labels.Clear();
            Directory.CreateDirectory(RuntimePath);Directory.CreateDirectory("Assets/Config/Runtime");AssetDatabase.Refresh();
            var config=AssetDatabase.LoadAssetAtPath<GameBalanceConfig>("Assets/Config/Runtime/GameBalance.asset");
            config.pixelsPerUnit=32;config.velocityIterations=10;config.positionIterations=10;
            config.defaultCountry="US";config.defaultCohort="B";config.defaultRewardedVariant=true;
            EditorUtility.SetDirty(config);AssetDatabase.SaveAssets();RecoveredRulesValidation.Run();
            var material=Material("CoinPhysics",config.rules.physics.friction,config.rules.physics.restitution);
            var groundMaterial=Material("GroundPhysics",.65f,0);var wallMaterial=Material("WallPhysics",0,0);
            var coinRoot=new GameObject("NativeCoin");
            var body=coinRoot.AddComponent<Rigidbody2D>();body.simulated=false;body.sleepMode=RigidbodySleepMode2D.StartAwake;
            var solid=coinRoot.AddComponent<CircleCollider2D>();solid.sharedMaterial=material;
            var visualRoot=new GameObject("Sprite",typeof(SpriteRenderer));visualRoot.transform.SetParent(coinRoot.transform,false);
            var sensorRoot=new GameObject("MergeSensor",typeof(CircleCollider2D),typeof(NativeCoinSensor));sensorRoot.transform.SetParent(coinRoot.transform,false);
            var coin=coinRoot.AddComponent<NativeMergeCoin>();coin.body=body;coin.solid=solid;coin.visual=visualRoot.GetComponent<SpriteRenderer>();
            coin.sensor=sensorRoot.GetComponent<CircleCollider2D>();coin.sensor.isTrigger=true;coin.sensor.density=0;sensorRoot.GetComponent<NativeCoinSensor>().coin=coin;
            var coinAsset=PrefabUtility.SaveAsPrefabAsset(coinRoot,RuntimePath+"/NativeCoin.prefab");UnityEngine.Object.DestroyImmediate(coinRoot);

            var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
            var root=new GameObject("RecoveredMain");
            var cameraRoot=new GameObject("WorldCamera",typeof(Camera));cameraRoot.transform.SetParent(root.transform,false);
            cameraRoot.transform.localPosition=new Vector3(0,0,-100);cameraRoot.tag="MainCamera";
            var camera=cameraRoot.GetComponent<Camera>();camera.orthographic=true;camera.orthographicSize=812/config.pixelsPerUnit;
            camera.nearClipPlane=.1f;camera.farClipPlane=200;camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=new Color(.12f,.16f,.26f);
            var ui=Source("scene/GameScene",root.transform);var nodes=Nodes(ui);
            var canvas=Component<Canvas>(nodes,1);canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=camera;canvas.planeDistance=100;canvas.sortingOrder=20;
            var scale=Component<CanvasScaler>(nodes,1);scale.referencePixelsPerUnit=32;
            // Separate canvas sorting preserves original background order around world-space sprites.
            foreach(int id in new[]{22,32}){var layer=nodes[id].AddComponent<Canvas>();layer.overrideSorting=true;layer.sortingOrder=-20;}
            foreach(int id in new[]{12,18,30,35,61,63,65})nodes[id].SetActive(false);
            nodes[10].SetActive(false);nodes[11].SetActive(false);
            Component<Text>(nodes,25).text="Withdraw";Component<Text>(nodes,34).text="Wheel";Component<Text>(nodes,56).text="Next";
            Label(nodes,25,"8");Label(nodes,34,"91");Label(nodes,56,"43");
            // Retain source placeholder for rolling notice; actual random notice formatter is pending.
            Component<Text>(nodes,39).text="";
            var progress=Component<Image>(nodes,69);progress.type=Image.Type.Filled;progress.fillMethod=Image.FillMethod.Horizontal;progress.fillOrigin=0;
            var obsoleteSlider=nodes[14].GetComponent<Slider>();if(obsoleteSlider)UnityEngine.Object.DestroyImmediate(obsoleteSlider);
            var boardRoot=new GameObject("NativeBoard");boardRoot.transform.SetParent(root.transform,false);
            boardRoot.transform.localPosition=new Vector3(0,(-209.435f+51.377f)/config.pixelsPerUnit,0);
            var board=boardRoot.AddComponent<NativeMergeBoard>();board.config=config;board.coinPrefab=coinAsset.GetComponent<NativeMergeCoin>();board.widthPixels=750;
            board.coinContainer=new GameObject("Coins").transform;board.coinContainer.SetParent(boardRoot.transform,false);
            board.ground=Marker(boardRoot.transform,"Ground",new Vector2(0,-455.064f),config.pixelsPerUnit);
            var groundCollider=board.ground.gameObject.AddComponent<BoxCollider2D>();groundCollider.size=new Vector2(750,30)/config.pixelsPerUnit;groundCollider.offset=new Vector2(0,-15)/config.pixelsPerUnit;groundCollider.sharedMaterial=groundMaterial;
            board.previewLine=Marker(boardRoot.transform,"PreviewLine",new Vector2(0,558.466f),config.pixelsPerUnit);
            board.deadLine=Marker(boardRoot.transform,"DeadLine",new Vector2(0,403.407f),config.pixelsPerUnit);
            float bottom=-455.064f-100,top=558.466f+100,height=top-bottom,center=bottom+height*.5f;
            board.leftWall=Wall(boardRoot.transform,"WallLeft",-385,center,height,config.pixelsPerUnit,wallMaterial);
            board.rightWall=Wall(boardRoot.transform,"WallRight",385,center,height,config.pixelsPerUnit,wallMaterial);
            var session=root.AddComponent<RecoveredGameSession>();session.board=board;session.worldCamera=camera;
            session.moneyText=Component<Text>(nodes,40);session.bubbleText=Component<Text>(nodes,45);session.progressText=Component<Text>(nodes,33);
            session.remainingText=Component<Text>(nodes,75);session.highestText=Component<Text>(nodes,48);session.progressFill=progress;
            session.nextImage=Component<Image>(nodes,54);session.bubble=nodes[8];session.dropGuide=nodes[13];
            BindReward(session,canvas.transform);BindFail(session,canvas.transform);BindGuide(session,canvas.transform);
            BindWheel(session,canvas.transform);session.wheelButton=Button(nodes[73]);
            session.localizedLabels=Labels.ToArray();
            var currency=new List<CurrencyIconBinding>();
            foreach(var meta in root.GetComponentsInChildren<RecoveredNode>(true))
                foreach(var component in meta.originalComponents)
                    if(component.className=="LoaderCoinSprite")
                    {
                        var setting=new CoinLoaderSettings();JsonUtility.FromJsonOverwrite(component.rawJson,setting);
                        currency.Add(new CurrencyIconBinding {image=meta.GetComponent<Image>(),type=setting.coin_type});
                    }
            session.currencyIcons=currency.ToArray();
            var es=new GameObject("EventSystem",typeof(EventSystem),typeof(StandaloneInputModule));es.transform.SetParent(root.transform,false);
            PrefabUtility.SaveAsPrefabAsset(root,RuntimePath+"/RecoveredMain.prefab");
            EditorSceneManager.SaveScene(scene,"Assets/Scenes/RecoveredMain.unity");
            EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene("Assets/Scenes/RecoveredMain.unity",true),new EditorBuildSettingsScene("Assets/Scenes/MockFlow.unity",false)};
            AssetDatabase.SaveAssets();
            Debug.Log("NATIVE_GAMEPLAY_ASSETS_AUTHORED; play validation remains required");
        }
        static PhysicsMaterial2D Material(string name,float friction,float bounce)
        {
            string path="Assets/Config/Runtime/"+name+".physicsMaterial2D";
            var material=AssetDatabase.LoadAssetAtPath<PhysicsMaterial2D>(path);
            if(!material){material=new PhysicsMaterial2D(name);AssetDatabase.CreateAsset(material,path);}
            material.friction=friction;material.bounciness=bounce;EditorUtility.SetDirty(material);return material;
        }
        static Transform Marker(Transform parent,string name,Vector2 pixels,float units)
        {var node=new GameObject(name).transform;node.SetParent(parent,false);node.localPosition=pixels/units;return node;}
        static Collider2D Wall(Transform parent,string name,float x,float y,float height,float units,PhysicsMaterial2D material)
        {var node=Marker(parent,name,new Vector2(x,y),units);var collider=node.gameObject.AddComponent<BoxCollider2D>();collider.size=new Vector2(20,height)/units;collider.sharedMaterial=material;return collider;}
        static void BindReward(RecoveredGameSession session,Transform parent)
        {
            var n=Dialog("RewardDialog",parent,out var root);var view=root.AddComponent<RecoveredRewardView>();session.rewardView=view;
            view.highestGroup=n[5];view.normalGroup=n[6];view.guideGroup=n[7];view.doubleGroup=n[9];
            view.highestAmount=Component<Text>(n,37);view.normalAmount=Component<Text>(n,47);view.guideAmount=Component<Text>(n,52);view.doubleAmount=Component<Text>(n,43);
            view.mask=Button(n[10]);view.highestClose=Button(n[16]);view.guideClose=Button(n[11]);
            Component<Text>(n,17).text="Collect";Component<Text>(n,29).text="Collect";
            Label(n,17,"37");Label(n,29,"37");Label(n,15,"36");Label(n,20,"35");Label(n,23,"36");Label(n,27,"44");
            root.SetActive(false);
        }
        static void BindFail(RecoveredGameSession session,Transform parent)
        {
            var n=Dialog("FailDialog",parent,out var root);var view=root.AddComponent<RecoveredFailView>();session.failView=view;
            view.revive=Button(n[6]);view.close=Button(n[7]);view.restart=Button(n[44]);
            view.scoreText=Component<Text>(n,24);view.mergesText=Component<Text>(n,33);view.bestText=Component<Text>(n,30);
            Component<Text>(n,18).text="Revive";Component<Text>(n,44).text="Restart";Component<Text>(n,12).text="Game over";
            Label(n,18,"33");Label(n,12,"29");
            root.SetActive(false);
        }
        static void BindGuide(RecoveredGameSession session,Transform parent)
        {
            var n=Dialog("GuideDialog",parent,out var root);var view=root.AddComponent<RecoveredGuideView>();session.guideView=view;
            view.stepZero=n[3];view.stepOne=n[9];view.stepThree=n[10];view.stepFour=n[12];view.mask=n[7];
            view.balance=Component<Text>(n,35);GuideClickRepair.Configure(view);
            Component<Text>(n,26).text="Drag left or right to drop coins";
            Component<Text>(n,29).text="Merge matching coins";Component<Text>(n,30).text="Collect rewards";
            Component<Text>(n,19).text="Withdraw";Component<Text>(n,34).text="Collect your reward";
            Label(n,26,"84");Label(n,29,"86");Label(n,30,"87");Label(n,34,"88");Label(n,19,"8");Label(n,37,"89");Label(n,38,"90");
            root.SetActive(false);
        }
        static void BindWheel(RecoveredGameSession session,Transform parent)
        {
            var n=Dialog("LuckDrawDialog",parent,out var root);var view=root.AddComponent<RecoveredWheelView>();session.wheelView=view;
            view.config=session.board.Config;view.draw=Button(n[12]);view.drawLabel=Component<Text>(n,25);view.countLabel=Component<Text>(n,64);view.nextScoreLabel=Component<Text>(n,66);
            view.slots=new RecoveredWheelSlot[8];
            for(int i=0;i<8;i++)view.slots[i]=new RecoveredWheelSlot {selected=n[33+4*i],coin=n[16+i],money=Component<Image>(n,35+4*i),coinAmount=Component<Text>(n,34+4*i)};
            root.SetActive(false);
            n=Dialog("LuckyDrawRewardDialog",parent,out root);var reward=root.AddComponent<RecoveredWheelRewardView>();session.wheelRewardView=reward;
            reward.claim=Button(n[3]);reward.money=Component<Image>(n,13);reward.coin=n[11];reward.glow=(RectTransform)n[6].transform;
            reward.amountLabel=Component<Text>(n,16);reward.hintLabel=Component<Text>(n,19);reward.titleLabel=Component<Text>(n,7);reward.claimLabel=Component<Text>(n,8);
            root.SetActive(false);
        }
    }
}
