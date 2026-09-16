using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace CoinMerge.Recovery.Editor
{
    // Scoped asset authoring: no runtime branch, gameplay parameter, or shared sprite edits.
    public static class RulesReskinAuthor
    {
        const float Scale = 750f / 1024;
        const string Art = "Assets/Resources/RulesReskin/";
        static readonly int[] Values = { 1, 2, 5, 10, 20, 50, 100, 200, 500, 1000, 2000 };
        static readonly float[] X = { 201, 341, 490, 644, 795, 893, 782, 625, 474, 325, 185 };
        static readonly float[] Y = { 861, 861, 861, 861, 861, 998, 1074, 1074, 1074, 1074, 1074 };
        static readonly float[] Diameter = { 96, 116, 121, 128, 132, 116, 145, 145, 141, 145, 148 };
        static NativeSkeletonData data;
        [MenuItem("Coin Merge/Reskin/Apply reference Merge Rules")]
        public static void Run()
        {
            if (EditorApplication.isPlaying) throw new InvalidOperationException("Stop Play before authoring rules assets.");
            var previous = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            if (previous.isDirty)
            {
                Directory.CreateDirectory("Temp/RulesReskinBackup");
                EditorSceneManager.SaveScene(previous, "Temp/RulesReskinBackup/BeforeAuthoring.unity", true);
            }
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            foreach (var file in Directory.GetFiles(Art, "*.png"))
            {
                var importer = (TextureImporter)AssetImporter.GetAtPath(file.Replace('\\','/'));
                importer.textureType = TextureImporterType.Sprite; importer.spriteImportMode = SpriteImportMode.Single;
                importer.spritePixelsPerUnit = 32; importer.mipmapEnabled = false;
                importer.alphaIsTransparency = true; importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.maxTextureSize = 2048; importer.filterMode = FilterMode.Bilinear; importer.wrapMode = TextureWrapMode.Clamp;
                var settings = new TextureImporterSettings(); importer.ReadTextureSettings(settings);
                settings.spriteMeshType = SpriteMeshType.FullRect; importer.SetTextureSettings(settings); importer.SaveAndReimport();
            }
            AuthorAnimation();
            const string path = "Assets/Prefabs/Runtime/RecoveredMain.prefab";
            var root = PrefabUtility.LoadPrefabContents(path);
            try { Apply(root); PrefabUtility.SaveAsPrefabAsset(root,path); }
            finally { PrefabUtility.UnloadPrefabContents(root); }
            var scene = EditorSceneManager.OpenScene("Assets/Scenes/RecoveredMain.unity");
            foreach (var item in scene.GetRootGameObjects()) if (item.GetComponent<RecoveredGameSession>()) Apply(item);
            EditorSceneManager.SaveScene(scene); AssetDatabase.SaveAssets();
            Debug.Log("RULES_RESKIN_AUTHORED: scoped native Buttons/text, uniform chips, original animation timing retained");
        }
        static void AuthorAnimation()
        {
            var original = AssetDatabase.LoadAssetAtPath<NativeSkeletonData>("Assets/Resources/Skeletal/Data/HeChengSM_TX.asset");
            data = AssetDatabase.LoadAssetAtPath<NativeSkeletonData>(Art + "RulesSequence.asset");
            if (!data) { data = ScriptableObject.CreateInstance<NativeSkeletonData>(); AssetDatabase.CreateAsset(data, Art + "RulesSequence.asset"); }
            EditorUtility.CopySerialized(original, data); data.name = "RulesSequence"; data.texturePath = "RulesReskin/ChipAtlas";
            // The generated panel supplies the decorative arrow. Keep the original reveal clip and hide only its old arrow/clipping surface.
            foreach (var attachment in data.attachments)
            {
                if (attachment.name == "ZZ") { attachment.type = 0; attachment.color = new float[] { 1,1,1,0 }; continue; }
                if (attachment.name == "11_11") { attachment.color = new float[] { 1,1,1,0 }; continue; }
                int number = int.Parse(attachment.name.Substring(3)); int index = 10 - number;
                float r = Diameter[index] * .5f;
                attachment.positions = new[] { -r,-r,-r,r,r,r,r,-r };
                float u0 = (index % 4 * 320f + 8) / 1280, u1 = (index % 4 * 320f + 312) / 1280;
                float v1 = 1 - (index / 4 * 320f + 8) / 960, v0 = 1 - (index / 4 * 320f + 312) / 960;
                attachment.uvs = new[] { u0,v0,u0,v1,u1,v1,u1,v0 };
                attachment.triangles = new[] { 0,1,2,2,3,0 };
            }
            var sourceClip = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/Resources/Skeletal/Clips/HeChengSM_TX/animation.anim");
            var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(Art + "RulesSequence.anim");
            if (!clip) { clip = new AnimationClip(); AssetDatabase.CreateAsset(clip, Art + "RulesSequence.anim"); }
            EditorUtility.CopySerialized(sourceClip,clip); clip.name = "animation";
            // Change only the visual pose coordinates; key times, easing, scale pulses and opacity remain original.
            foreach (var binding in AnimationUtility.GetCurveBindings(clip))
            {
                if (binding.type != typeof(NativeSkeletonBone) || (binding.propertyName != "x" && binding.propertyName != "y")) continue;
                for (int index = 0; index < Values.Length; index++)
                {
                    string name = "11_" + (10-index).ToString("00");
                    if (!binding.path.EndsWith("_" + name,StringComparison.Ordinal)) continue;
                    var bone = Array.Find(original.bones, b => b.name == name);
                    float delta = binding.propertyName == "x" ? X[index] - 512 - bone.x : 997 - Y[index] - bone.y;
                    var curve = AnimationUtility.GetEditorCurve(clip,binding); var keys = curve.keys;
                    for (int k=0;k<keys.Length;k++) keys[k].value += delta;
                    curve.keys=keys; AnimationUtility.SetEditorCurve(clip,binding,curve);
                }
            }
            for (int i=0;i<Values.Length;i++)
            {
                var bone=Array.Find(data.bones,b=>b.name=="11_"+(10-i).ToString("00"));bone.x=X[i]-512;bone.y=997-Y[i];
            }
            data.animations[0].resourcePath="RulesReskin/RulesSequence";
            data.animations[0].deforms=Array.Empty<NativeDeformAnimation>();
            EditorUtility.SetDirty(data);EditorUtility.SetDirty(clip);
        }
        static void Rect(RectTransform r, RectTransform parent, float x,float y,float width,float height)
        {
            r.SetParent(parent,false);r.anchorMin=r.anchorMax=r.pivot=new Vector2(.5f,.5f);
            r.localRotation=Quaternion.identity;r.localScale=Vector3.one;
            r.anchoredPosition=new Vector2(x+width*.5f-512,768-y-height*.5f)*Scale;
            r.sizeDelta=new Vector2(width,height)*Scale;
        }
        static void Style(Text text,int size,Color color,float outline,Color stroke, bool bold=false)
        {
            text.font=AssetDatabase.LoadAssetAtPath<Font>("Assets/Art/PopupFonts/Nunito-"+(bold?"Black":"ExtraBold")+".ttf");
            text.fontSize=Mathf.RoundToInt(size*Scale);text.fontStyle=FontStyle.Normal;
            text.color=color;text.resizeTextForBestFit=true;text.resizeTextMaxSize=text.fontSize;
            text.resizeTextMinSize=Mathf.RoundToInt(text.fontSize*.7f);text.lineSpacing=1;
            text.horizontalOverflow=HorizontalWrapMode.Wrap;text.verticalOverflow=VerticalWrapMode.Truncate;
            var effect=text.GetComponent<RecoveredRoundOutline>();
            if(outline>0){if(!effect)effect=text.gameObject.AddComponent<RecoveredRoundOutline>();effect.enabled=true;effect.effectColor=stroke;effect.effectDistance=Vector2.one*(outline*Scale);}
            else if(effect)effect.enabled=false;
        }
        static void Apply(GameObject root)
        {
            var menus=root.GetComponent<RecoveredGameSession>().menus;var page=menus.pages[1];
            // Nested popup canvases must honor the existing Show() stack order (201, 202, ...).
            foreach(var popup in menus.pages)EnableSorting(popup.GetComponent<Canvas>());
            var nodes=new Dictionary<int,RectTransform>();
            foreach(var node in page.GetComponentsInChildren<RecoveredNode>(true))nodes[node.sourceObjectId]=(RectTransform)node.transform;
            var content=nodes[2];var loader=page.GetComponent<RecoveredMenuArt>();var bindings=new List<MenuImageBinding>(loader.images);
            void ArtFor(Image image,string name)
            {
                var binding=bindings.Find(x=>x.image==image);
                if(binding==null){binding=new MenuImageBinding{image=image};bindings.Add(binding);}
                binding.resourcePath="RulesReskin/"+name;
                image.sprite=null;image.type=Image.Type.Simple;image.preserveAspect=true;image.color=Color.white;image.enabled=true;
            }
            Rect(nodes[10],content,27,299,970,1149);ArtFor(nodes[10].GetComponent<Image>(),"Panel");
            nodes[6].GetComponent<Image>().enabled=false;nodes[9].GetComponent<Image>().enabled=false;
            Rect(nodes[5],content,267,333,500,108);nodes[5].GetComponent<Text>().alignment=TextAnchor.MiddleCenter;
            Style(nodes[5].GetComponent<Text>(),79,Color.white,6,new Color(.0f,.23f,.76f),true);
            Rect(nodes[6],content,62,491,902,196);
            var desc=nodes[12];desc.anchorMin=desc.anchorMax=desc.pivot=new Vector2(.5f,.5f);desc.anchoredPosition=Vector2.zero;desc.sizeDelta=new Vector2(790,173)*Scale;
            desc.GetComponent<Text>().alignment=TextAnchor.MiddleLeft;
            Style(desc.GetComponent<Text>(),43,new Color(.02f,.29f,.72f),0,Color.clear);
            Rect(nodes[17],content,102,702,710,76);nodes[17].GetComponent<Text>().alignment=TextAnchor.MiddleLeft;
            Style(nodes[17].GetComponent<Text>(),55,new Color(0,.27f,.93f),2.5f,Color.white,true);
            Rect(nodes[3],content,198,1217,638,181);ArtFor(nodes[3].GetComponent<Image>(),"Confirm");
            var ok=nodes[8];ok.anchorMin=ok.anchorMax=ok.pivot=new Vector2(.5f,.5f);ok.anchoredPosition=new Vector2(0,4)*Scale;ok.sizeDelta=new Vector2(490,137)*Scale;
            ok.GetComponent<Text>().alignment=TextAnchor.MiddleCenter;Style(ok.GetComponent<Text>(),103,Color.white,6,new Color(0,.32f,.07f),true);
            Rect(nodes[7],content,858,327,110,108);
            var close=nodes[16];close.anchorMin=close.anchorMax=close.pivot=new Vector2(.5f,.5f);close.anchoredPosition=Vector2.zero;close.sizeDelta=new Vector2(110,108)*Scale;
            ArtFor(close.GetComponent<Image>(),"Close");
            // Replace the old single fan sprite with fixed, serialized images using the exact user's chips.
            var fan=nodes[9];Rect(fan,content,0,0,1024,1536);fan.SetAsFirstSibling();
            int[] fanValues={5,5,20,10,500,100,2000};
            float[] fx={58,838,125,729,249,575,375},fy={232,225,174,178,138,130,73},fd={132,114,164,160,203,204,257};
            float[] angles={17,-17,15,-12,13,-10,0};
            for(int i=0;i<fanValues.Length;i++)
            {
                string name="ReferenceChip"+i;var child=fan.Find(name);Image image;
                if(child)image=child.GetComponent<Image>();else{var go=new GameObject(name,typeof(RectTransform),typeof(CanvasRenderer),typeof(Image));image=go.GetComponent<Image>();}
                Rect(image.rectTransform,fan,fx[i],fy[i],fd[i],fd[i]);image.rectTransform.localRotation=Quaternion.Euler(0,0,angles[i]);
                ArtFor(image,"Chip"+fanValues[i]);image.raycastTarget=false;
            }
            float[] dx={267,411,562,715,885,866,701,548,398,251};
            float[] dy={860,860,860,860,917,1079,1079,1079,1079,1079};
            for(int i=0;i<dx.Length;i++)
            {
                string name="SequenceDot"+i;var child=content.Find(name);Image image;
                if(child)image=child.GetComponent<Image>();else{var go=new GameObject(name,typeof(RectTransform),typeof(CanvasRenderer),typeof(Image));image=go.GetComponent<Image>();}
                Rect(image.rectTransform,content,dx[i]-12,dy[i]-12,24,24);ArtFor(image,"PathDot");image.raycastTarget=false;
                image.transform.SetSiblingIndex(nodes[14].GetSiblingIndex());
            }
            // Only the small GM entry sits below modal masks. Its popup retains its original topmost canvas.
            var gm=root.GetComponentInChildren<VersionGmPanel>(true);
            if(gm)
            {
                var entryRaycaster=gm.open.GetComponent<GraphicRaycaster>();if(entryRaycaster)UnityEngine.Object.DestroyImmediate(entryRaycaster);
                var entryCanvas=gm.open.GetComponent<Canvas>();if(entryCanvas)UnityEngine.Object.DestroyImmediate(entryCanvas);
                gm.GetComponent<Canvas>().sortingOrder=100;
                var popupCanvas=gm.popup.GetComponent<Canvas>();if(!popupCanvas)popupCanvas=gm.popup.AddComponent<Canvas>();
                popupCanvas.sortingOrder=1000;EnableSorting(popupCanvas);
                if(!gm.popup.GetComponent<GraphicRaycaster>())gm.popup.AddComponent<GraphicRaycaster>();
            }
            page.GetComponent<Canvas>().sortingOrder=201;
            // Coordinates in the visual reference, sampled by the existing native animation.
            Rect(nodes[14],content,0,229,1024,1536);
            var player=menus.ruleAnimation;player.dataPath="RulesReskin/RulesSequence";
            player.transform.localScale=Vector3.one*Scale;
            foreach(var bone in player.bones)
            {
                var model=Array.Find(data.bones,b=>bone.name.EndsWith("_"+b.name,StringComparison.Ordinal));
                if(model!=null){bone.x=model.x;bone.y=model.y;}
            }
            loader.images=bindings.ToArray();
            // The existing Buttons and action=0 callbacks remain intact and stay on their visible images.
            foreach(int id in new[]{3,16}){var button=nodes[id].GetComponent<Button>();if(!button||button.targetGraphic!=nodes[id].GetComponent<Image>())throw new Exception("Rules Button binding changed");}
        }
        static void EnableSorting(Canvas canvas)
        {
            // Inactive prefab contents do not initialize the native Canvas hierarchy. Author its serialized override explicitly.
            var serialized=new SerializedObject(canvas);serialized.FindProperty("m_OverrideSorting").boolValue=true;serialized.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
