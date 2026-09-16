using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;
using UnityEditor.SceneManagement;
namespace CoinMerge.Recovery.Editor
{
    public static class RecoveryBuilder
    {
        static readonly Dictionary<string,Sprite> Sprites=new Dictionary<string,Sprite>();
        static readonly List<string> Warnings=new List<string>();
        static Font FontAsset;
        static string Key(string variant,string uuid)=>variant+":"+uuid;
        [MenuItem("Coin Merge/Build recovered prefabs and mock scene")]
        public static void BuildAll()
        {
            Directory.CreateDirectory("Assets/Prefabs");Directory.CreateDirectory("Assets/Scenes");
            AssetDatabase.Refresh();ImportSprites();
            FontAsset=AssetDatabase.FindAssets("t:Font",new[]{"Assets/Art"}).Select(AssetDatabase.GUIDToAssetPath).Select(AssetDatabase.LoadAssetAtPath<Font>).FirstOrDefault();
            if(FontAsset==null)FontAsset=Resources.GetBuiltinResource<Font>("Arial.ttf");
            var paths=Directory.GetFiles("Assets/Resources/Recovered","*.json").Where(p=>!p.EndsWith("sprite_import.json")).OrderBy(p=>p).ToArray();
            int count=0;
            foreach(var path in paths)
            {
                var model=JsonUtility.FromJson<RecoveryModel>(File.ReadAllText(path));
                var root=BuildModel(model);
                string target="Assets/Prefabs/"+model.variant+"/"+Clean(model.name.Replace(".fire",""))+".prefab";
                Directory.CreateDirectory(Path.GetDirectoryName(target));
                PrefabUtility.SaveAsPrefabAsset(root,target,out bool success);
                if(!success)throw new Exception("Prefab save failed: "+target);
                UnityEngine.Object.DestroyImmediate(root);count++;
            }
            BuildNativeCoin();BuildMockScene();RunSmokeTests();
            File.WriteAllLines("../07_Verification/unity_conversion_warnings.txt",Warnings);
            File.WriteAllText("../07_Verification/unity_build_result.json","{\"prefabs\":"+count+",\"warnings\":"+Warnings.Count+",\"status\":\"generated_and_smoke_tested\"}");
            AssetDatabase.SaveAssets();AssetDatabase.Refresh();Debug.Log("RECOVERY_BUILD_OK prefabs="+count);
        }
        static string Clean(string s)
        {foreach(char c in Path.GetInvalidFileNameChars())if(c!='/'&&c!='\\')s=s.Replace(c,'_');return s;}
        static void ImportSprites()
        {
            var list=JsonUtility.FromJson<SpriteImportModel>(File.ReadAllText("Assets/Resources/Recovered/sprite_import.json"));
            foreach(var entry in list.sprites)
            {
                var importer=AssetImporter.GetAtPath(entry.path) as TextureImporter;
                if(importer==null)throw new Exception("Missing sprite "+entry.path);
                importer.textureType=TextureImporterType.Sprite;importer.spriteImportMode=SpriteImportMode.Single;
                importer.spritePixelsPerUnit=100;importer.mipmapEnabled=false;importer.alphaIsTransparency=true;
                importer.textureCompression=TextureImporterCompression.Uncompressed;
                importer.maxTextureSize=8192;
                // Cocos capInsets: left, top, right, bottom. Unity: left, bottom, right, top.
                var b=entry.border;importer.spriteBorder=new Vector4(b[0],b[3],b[2],b[1]);
                importer.SaveAndReimport();Sprites[Key(entry.variant,entry.uuid)]=AssetDatabase.LoadAssetAtPath<Sprite>(entry.path);
            }
        }
        static Vector2 V2(float[] a,Vector2 fallback)=>a!=null&&a.Length>=2?new Vector2(a[0],a[1]):fallback;
        static GameObject BuildModel(RecoveryModel m)
        {
            var wrapper=new GameObject(Path.GetFileName(m.name),typeof(RectTransform));
            var objects=new Dictionary<int,GameObject>();
            foreach(var n in m.nodes)objects[n.id]=new GameObject(n.name,typeof(RectTransform));
            foreach(var n in m.nodes)
            {
                var go=objects[n.id];var rect=(RectTransform)go.transform;
                var parent=objects.ContainsKey(n.parent)?objects[n.parent].transform:wrapper.transform;
                rect.SetParent(parent,false);rect.pivot=V2(n.pivot,new Vector2(.5f,.5f));
                var pr=parent as RectTransform;rect.anchorMin=rect.anchorMax=pr!=null?pr.pivot:new Vector2(.5f,.5f);
                rect.sizeDelta=V2(n.size,Vector2.zero);rect.anchoredPosition3D=new Vector3(n.position[0],n.position[1],n.position[2]);
                rect.localRotation=new Quaternion(n.rotation[0],n.rotation[1],n.rotation[2],n.rotation[3]);
                rect.localScale=new Vector3(n.scale[0],n.scale[1],n.scale[2]);go.SetActive(n.active);
                var meta=go.AddComponent<RecoveredNode>();meta.sourceUuid=m.uuid;meta.variant=m.variant;meta.sourceObjectId=n.id;meta.originalNodeJson=n.rawJson;meta.originalComponents=n.components;
            }
            foreach(var n in m.nodes)
                for(int i=0;i<n.children.Length;i++)if(objects.TryGetValue(n.children[i],out var child))child.transform.SetSiblingIndex(i);
            foreach(var n in m.nodes)
            {
                var go=objects[n.id];var color=new Color(n.color[0],n.color[1],n.color[2],n.color[3]);
                foreach(var c in n.components)
                {
                    try{AddComponent(go,c,m.variant,objects,color);}
                    catch(Exception ex){Warnings.Add(m.variant+"/"+m.name+"/"+n.name+" "+c.type+": "+ex.Message);}
                }
            }
            return wrapper;
        }
        static void AddComponent(GameObject go,ComponentModel c,string variant,Dictionary<int,GameObject> nodes,Color color)
        {
            switch(c.type)
            {
                case "cc.Canvas":
                    var canvas=go.AddComponent<Canvas>();canvas.renderMode=RenderMode.ScreenSpaceOverlay;
                    var scale=go.AddComponent<CanvasScaler>();scale.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;scale.referenceResolution=new Vector2(750,1624);scale.matchWidthOrHeight=0;
                    go.AddComponent<GraphicRaycaster>();break;
                case "cc.Sprite":
                    var image=go.GetComponent<Image>()??go.AddComponent<Image>();
                    if(!string.IsNullOrEmpty(c.sprite)&&Sprites.TryGetValue(Key(variant,c.sprite),out var sprite))image.sprite=sprite;
                    image.color=color;image.raycastTarget=false;image.enabled=c.enabled;
                    image.type=c.spriteType==1?Image.Type.Sliced:c.spriteType==2?Image.Type.Tiled:c.spriteType==3?Image.Type.Filled:Image.Type.Simple;
                    image.fillAmount=c.fillRange;image.fillMethod=c.fillType==0?Image.FillMethod.Horizontal:c.fillType==1?Image.FillMethod.Vertical:Image.FillMethod.Radial360;break;
                case "cc.Label":case "cc.RichText":
                    var text=go.GetComponent<Text>()??go.AddComponent<Text>();text.font=FontAsset;text.text=c.text;text.fontSize=c.fontSize;text.color=color;
                    text.supportRichText=c.type=="cc.RichText";text.alignment=(TextAnchor)(Mathf.Clamp(c.verticalAlign,0,2)*3+Mathf.Clamp(c.horizontalAlign,0,2));
                    text.horizontalOverflow=HorizontalWrapMode.Wrap;text.verticalOverflow=VerticalWrapMode.Overflow;text.raycastTarget=false;text.enabled=c.enabled;break;
                case "cc.LabelOutline":if(go.GetComponent<Graphic>()!=null){var outline=go.AddComponent<Outline>();outline.effectDistance=new Vector2(2,-2);}break;
                case "cc.LabelShadow":if(go.GetComponent<Graphic>()!=null)go.AddComponent<Shadow>();break;
                case "cc.Button":
                    var button=go.AddComponent<Button>();button.targetGraphic=go.GetComponent<Graphic>();
                    if(button.targetGraphic!=null)button.targetGraphic.raycastTarget=true;break;
                case "cc.BlockInputEvents":
                    var blocker=go.GetComponent<Graphic>();if(blocker==null){var img=go.AddComponent<Image>();img.color=Color.clear;blocker=img;}blocker.raycastTarget=true;break;
                case "cc.Mask":go.AddComponent<RectMask2D>();break;
                case "cc.ScrollView":
                    var scroll=go.AddComponent<ScrollRect>();if(nodes.TryGetValue(c.content,out var content))scroll.content=content.transform as RectTransform;
                    if(nodes.TryGetValue(c.viewport,out var view))scroll.viewport=view.transform as RectTransform;scroll.horizontal=false;break;
                case "cc.EditBox":
                    var input=go.AddComponent<InputField>();input.textComponent=go.GetComponentInChildren<Text>(true);break;
                case "cc.ProgressBar":
                    var slider=go.AddComponent<Slider>();slider.interactable=false;break;
                case "cc.RigidBody":
                    var rb=go.AddComponent<Rigidbody2D>();rb.bodyType=c.bodyType==0?RigidbodyType2D.Static:c.bodyType==1?RigidbodyType2D.Kinematic:RigidbodyType2D.Dynamic;
                    rb.gravityScale=c.gravityScale;rb.drag=c.linearDamping;rb.angularDrag=c.angularDamping;break;
                case "cc.PhysicsCircleCollider":case "cc.CircleCollider":
                    var circle=go.AddComponent<CircleCollider2D>();circle.radius=c.radius;circle.offset=V2(c.offset,Vector2.zero);circle.isTrigger=c.sensor;break;
                case "cc.PhysicsBoxCollider":case "cc.BoxCollider":
                    var box=go.AddComponent<BoxCollider2D>();box.size=V2(c.size,new Vector2(100,100));box.offset=V2(c.offset,Vector2.zero);box.isTrigger=c.sensor;break;
                case "cc.Animation":go.AddComponent<Animator>();break;
                // Widget/Layout/polygon/Spine/custom scripts retain exact source metadata.
                // Their runtime semantic ports are explicitly listed in component_migration.json.
            }
        }
        static void BuildNativeCoin()
        {
            var go=new GameObject("NativeMergeCoin",typeof(SpriteRenderer),typeof(Rigidbody2D),typeof(CircleCollider2D),typeof(NativeMergeCoin));
            go.GetComponent<Rigidbody2D>().collisionDetectionMode=CollisionDetectionMode2D.Continuous;
            go.GetComponent<CircleCollider2D>().radius=.27f;
            var entry=JsonUtility.FromJson<SpriteImportModel>(File.ReadAllText("Assets/Resources/Recovered/sprite_import.json")).sprites.FirstOrDefault(s=>s.variant=="HotUpdate"&&s.path.Contains("coin1/1__"));
            if(entry!=null)go.GetComponent<SpriteRenderer>().sprite=Sprites[Key(entry.variant,entry.uuid)];
            Directory.CreateDirectory("Assets/Prefabs/Framework");PrefabUtility.SaveAsPrefabAsset(go,"Assets/Prefabs/Framework/NativeMergeCoin.prefab");UnityEngine.Object.DestroyImmediate(go);
        }
        static Text MakeText(Transform parent,string name,string value,Vector2 pos,Vector2 size)
        {
            var go=new GameObject(name,typeof(RectTransform),typeof(Text));go.transform.SetParent(parent,false);
            var rect=(RectTransform)go.transform;rect.sizeDelta=size;rect.anchoredPosition=pos;
            var text=go.GetComponent<Text>();text.font=FontAsset;text.fontSize=30;text.alignment=TextAnchor.MiddleCenter;text.text=value;text.color=Color.white;return text;
        }
        static Button MakeButton(Transform parent,string label,Vector2 pos)
        {
            var go=new GameObject(label,typeof(RectTransform),typeof(Image),typeof(Button));go.transform.SetParent(parent,false);
            var rect=(RectTransform)go.transform;rect.sizeDelta=new Vector2(580,100);rect.anchoredPosition=pos;
            go.GetComponent<Image>().color=new Color(.14f,.35f,.65f);MakeText(go.transform,"Label",label,Vector2.zero,new Vector2(560,90));return go.GetComponent<Button>();
        }
        static void BuildMockScene()
        {
            var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
            var canvas=new GameObject("MockFlowCanvas",typeof(RectTransform),typeof(Canvas),typeof(CanvasScaler),typeof(GraphicRaycaster));
            canvas.GetComponent<Canvas>().renderMode=RenderMode.ScreenSpaceOverlay;
            var scaler=canvas.GetComponent<CanvasScaler>();scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;scaler.referenceResolution=new Vector2(750,1624);
            new GameObject("EventSystem",typeof(EventSystem),typeof(StandaloneInputModule));
            var panel=canvas.AddComponent<MockFlowPanel>();panel.status=MakeText(canvas.transform,"Status","Local mock",new Vector2(0,350),new Vector2(700,240));
            panel.rewardButton=MakeButton(canvas.transform,"Watch reward ad (mock)",new Vector2(0,100));
            panel.cancelButton=MakeButton(canvas.transform,"Cancel reward ad (mock)",new Vector2(0,-40));
            panel.withdrawButton=MakeButton(canvas.transform,"Request withdrawal (mock)",new Vector2(0,-180));
            PrefabUtility.SaveAsPrefabAsset(canvas,"Assets/Prefabs/Framework/MockFlowCanvas.prefab");
            EditorSceneManager.SaveScene(scene,"Assets/Scenes/MockFlow.unity");
            EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene("Assets/Scenes/MockFlow.unity",true)};
        }
        [MenuItem("Coin Merge/Run facade and rule checks")]
        public static void RunSmokeTests()
        {
            var sdk=new MockSdkFacade();var flow=new BusinessFlow(sdk,sdk);
            if(!flow.WatchAdForReward("reward",2m).Result||flow.RewardBalance!=2m)throw new Exception("Reward completion failed");
            sdk.NextAdOutcome=AdOutcome.Cancelled;if(flow.WatchAdForReward("reward",3m).Result||flow.RewardBalance!=2m)throw new Exception("Cancel incorrectly grants reward");
            sdk.NextAdOutcome=AdOutcome.Failed;if(flow.WatchAdForReward("reward",3m).Result||flow.RewardBalance!=2m)throw new Exception("Failure incorrectly grants reward");
            var req=new WithdrawalRequest{RequestId="test-1",Amount=1m,Account="mock",RequiredMerges=2,CurrentMerges=2};
            if(!flow.TriggerWithdrawal(req).Result.Accepted)throw new Exception("Mock withdrawal failed");
            flow.TriggerWithdrawal(req).Wait();if(sdk.History.Count!=1)throw new Exception("Duplicate withdrawal recorded");
            var bad=new WithdrawalRequest{RequestId="test-2",Amount=1m,Account="mock",RequiredMerges=2,CurrentMerges=1};
            if(flow.TriggerWithdrawal(bad).Result.Accepted)throw new Exception("Missing condition accepted");
            if(MergeRules.Upgrade(2)!=5||MergeRules.Upgrade(2000)!=2000)throw new Exception("Upgrade mismatch");
            if(MergeRules.PickWeighted(new[]{1,2},new[]{50f,50f},.499)!=1||MergeRules.PickWeighted(new[]{1,2},new[]{50f,50f},.5)!=2)throw new Exception("Weight boundary mismatch");
            File.WriteAllLines("../07_Verification/unity_smoke_tests.txt",new[]{"PASS: completed ad grants reward","PASS: cancelled and failed ads do not grant reward","PASS: withdrawal accepted and recorded locally","PASS: repeated request is idempotent","PASS: failed condition rejects withdrawal","PASS: recovered merge progression","PASS: weighted boundary selection","NOTE: mock behavior, not proof of remote service equivalence"}.Concat(sdk.Trace));
        }
    }
}
