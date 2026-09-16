using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
namespace CoinMerge.Recovery.Editor
{
    public static class RecoveredMainMenusBuilder
    {
        static readonly List<MenuActionBinding> actions=new List<MenuActionBinding>();
        static readonly List<LocalizedLabelBinding> labels=new List<LocalizedLabelBinding>();
        static readonly List<CurrencyIconBinding> currencies=new List<CurrencyIconBinding>();
        static readonly Dictionary<string,string> resources=new Dictionary<string,string>();
        [Serializable] sealed class CoinSettings {public int coin_type=1;}
        static T At<T>(Dictionary<int,GameObject> n,int id) where T:Component=>n[id].GetComponent<T>();
        static Text Text(Dictionary<int,GameObject> n,int id)=>At<Text>(n,id);
        static void Label(Dictionary<int,GameObject> n,int id,string key)
        {var t=Text(n,id);if(!t)throw new Exception("Missing text "+id);t.text=new RecoveredLocalization("US").Label(key);labels.Add(new LocalizedLabelBinding {label=t,key=key});}
        static Button Bind(GameObject node,int action)
        {
            var graphic=node.GetComponent<Graphic>();if(!graphic||!graphic.enabled)throw new Exception("No visible graphic on Button "+node.name);
            graphic.raycastTarget=true;var b=node.GetComponent<Button>()??node.AddComponent<Button>();b.targetGraphic=graphic;b.transition=Selectable.Transition.None;
            b.navigation=new Navigation {mode=Navigation.Mode.None};actions.Add(new MenuActionBinding {button=b,action=action});return b;
        }
        static string Resource(Sprite sprite)
        {
            if(!sprite)throw new Exception("Missing source sprite");string source=AssetDatabase.GetAssetPath(sprite);
            if(resources.TryGetValue(source,out var existing))return existing;
            if(source.StartsWith("Assets/Resources/",StringComparison.Ordinal))return source.Substring(17,source.Length-21);
            string name=AssetDatabase.AssetPathToGUID(source),target="Assets/Resources/MenuArt/"+name+Path.GetExtension(source);
            if(!File.Exists(target)&&!AssetDatabase.CopyAsset(source,target))throw new Exception("Cannot copy "+source);
            string path="MenuArt/"+name;resources.Add(source,path);return path;
        }
        static string Uuid(string uuid)
        {
            var model=JsonUtility.FromJson<SpriteImportModel>(File.ReadAllText("Assets/Resources/Recovered/sprite_import.json"));
            foreach(var e in model.sprites)if(e.variant=="HotUpdate"&&e.uuid==uuid)return Resource(AssetDatabase.LoadAssetAtPath<Sprite>(e.path));
            throw new Exception("Unknown sprite "+uuid);
        }
        static string Platform(string name)
        {
            string[] files=Directory.GetFiles("Assets/Art/HotUpdate/Sprites/texture/CashPlat",name+"_1__*.png");
            if(files.Length!=1)throw new Exception("Platform art "+name);return Resource(AssetDatabase.LoadAssetAtPath<Sprite>(files[0].Replace('\\','/')));
        }
        static void DeferArt(GameObject page)
        {
            var values=new List<MenuImageBinding>();
            foreach(var img in page.GetComponentsInChildren<Image>(true))if(img.sprite)
            {values.Add(new MenuImageBinding {image=img,resourcePath=Resource(img.sprite)});img.sprite=null;}
            page.AddComponent<RecoveredMenuArt>().images=values.ToArray();
        }
        static Dictionary<int,GameObject> Page(string name,Transform parent,int content,out GameObject root)
        {
            root=NativeGameplayBuilder.Source("GameDialog/"+name,parent);root.SetActive(false);
            var rect=(RectTransform)root.transform;rect.anchorMin=Vector2.zero;rect.anchorMax=Vector2.one;rect.offsetMin=rect.offsetMax=Vector2.zero;
            var canvas=root.AddComponent<Canvas>();canvas.overrideSorting=true;canvas.sortingOrder=201;canvas.additionalShaderChannels=AdditionalCanvasShaderChannels.TexCoord1;
            root.AddComponent<GraphicRaycaster>();var n=NativeGameplayBuilder.Nodes(root);
            foreach(var graphic in root.GetComponentsInChildren<Graphic>(true))graphic.raycastTarget=false;
            foreach(var image in root.GetComponentsInChildren<Image>(true))if(!image.sprite)image.enabled=false; // An unassigned Cocos Sprite draws nothing; a null Unity Image would draw a white quad.
            foreach(var meta in root.GetComponentsInChildren<RecoveredNode>(true))foreach(var c in meta.originalComponents)
                if(c.className=="LoaderCoinSprite"){var settings=new CoinSettings();JsonUtility.FromJsonOverwrite(c.rawJson,settings);currencies.Add(new CurrencyIconBinding {image=meta.GetComponent<Image>(),type=settings.coin_type});}
            if(content>0)root.AddComponent<RecoveredMenuPopup>().content=n[content].transform;
            foreach(var img in root.GetComponentsInChildren<Image>(true))if(img.name.ToLowerInvariant().Contains("mask")||img.name=="sprite_nask_bg")img.raycastTarget=true;
            return n;
        }
        static void Scroll(Dictionary<int,GameObject> n,int scrollId,int viewId,int contentId)
        {
            var s=At<ScrollRect>(n,scrollId);s.content=(RectTransform)n[contentId].transform;s.viewport=(RectTransform)n[viewId].transform;
            s.horizontal=false;s.vertical=true;s.scrollSensitivity=40;
            s.viewport.anchorMin=Vector2.zero;s.viewport.anchorMax=Vector2.one;s.viewport.offsetMin=s.viewport.offsetMax=Vector2.zero;
            s.content.anchorMin=s.content.anchorMax=new Vector2(.5f,1);s.content.pivot=new Vector2(.5f,1);s.content.anchoredPosition=Vector2.zero;
            // The source images themselves receive scroll/drag events through their ScrollRect ancestor.
            foreach(var image in s.content.GetComponentsInChildren<Image>(true))image.raycastTarget=true;
        }
        static Image Fill(Dictionary<int,GameObject> n,int rootId,int fillId)
        {
            var slider=At<Slider>(n,rootId);if(slider)UnityEngine.Object.DestroyImmediate(slider);
            var image=At<Image>(n,fillId);image.type=Image.Type.Filled;image.fillMethod=Image.FillMethod.Horizontal;image.fillOrigin=0;return image;
        }
        static InputField Input(Dictionary<int,GameObject> n,int id)
        {
            var root=n[id];var field=root.GetComponent<InputField>()??root.AddComponent<InputField>();
            var text=root.transform.Find("TEXT_LABEL").GetComponent<Text>();var placeholder=root.transform.Find("PLACEHOLDER_LABEL").GetComponent<Text>();
            var bg=root.transform.Find("BACKGROUND_SPRITE").GetComponent<Image>();bg.transform.SetAsFirstSibling();bg.raycastTarget=true;
            text.gameObject.SetActive(true);text.text="";text.supportRichText=false;placeholder.supportRichText=false;
            field.textComponent=text;field.placeholder=placeholder;field.targetGraphic=bg;field.characterLimit=500;field.contentType=InputField.ContentType.Standard;
            field.lineType=InputField.LineType.SingleLine;field.transition=Selectable.Transition.None;return field;
        }
        static AccountForm Form(Dictionary<int,GameObject> n,int account,int full,int tax,int confirm,int caption)
        {
            return new AccountForm {account=Input(n,account),fullName=full>0?Input(n,full):null,taxId=tax>0?Input(n,tax):null,
                confirm=At<Image>(n,confirm),confirmOutline=At<Outline>(n,caption),enabledSprite=Uuid("cd716fcd-072f-4d7b-b158-c11cbee228aa"),disabledSprite=Uuid("fb19ae04-bd29-45ff-858a-cdf97cdd08ed"),
                platforms=Array.Empty<Image>(),selected=Array.Empty<GameObject>(),names=Array.Empty<string>(),platformPaths=Array.Empty<string>()};
        }
        [MenuItem("Coin Merge/Author original main menu chains")]
        public static void Run()
        {
            Directory.CreateDirectory("Assets/Resources/MenuArt");Directory.CreateDirectory("Assets/Resources/MenuAudio");AssetDatabase.Refresh();
            const string prefab="Assets/Prefabs/Runtime/RecoveredMain.prefab";
            var root=PrefabUtility.LoadPrefabContents(prefab);try{Author(root);PrefabUtility.SaveAsPrefabAsset(root,prefab);}finally{PrefabUtility.UnloadPrefabContents(root);}
            var scene=EditorSceneManager.OpenScene("Assets/Scenes/RecoveredMain.unity");
            foreach(var candidate in scene.GetRootGameObjects())if(candidate.GetComponent<RecoveredGameSession>())Author(candidate);
            EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();
            Debug.Log("RECOVERED_MAIN_MENUS_AUTHORED");
        }
        static void Author(GameObject root)
        {
            actions.Clear();labels.Clear();currencies.Clear();
            var session=root.GetComponent<RecoveredGameSession>();var canvas=session.moneyText.GetComponentInParent<Canvas>();
            if(session.menus){foreach(var page in session.menus.pages)UnityEngine.Object.DestroyImmediate(page);UnityEngine.Object.DestroyImmediate(session.menus.toast);UnityEngine.Object.DestroyImmediate(session.menus.audioCues.gameObject);UnityEngine.Object.DestroyImmediate(session.menus);}
            var menus=root.AddComponent<RecoveredMainMenus>();session.menus=menus;menus.session=session;menus.pages=new GameObject[11];menus.forms=new AccountForm[3];
            var main=new Dictionary<int,GameObject>();string uuid=session.moneyText.GetComponent<RecoveredNode>().sourceUuid;
            foreach(var meta in canvas.GetComponentsInChildren<RecoveredNode>(true))if(meta.sourceUuid==uuid)main.Add(meta.sourceObjectId,meta.gameObject);
            var coinGraphic=main[47].GetComponentInChildren<NativeSkeletonGraphic>(true);
            if(PrefabUtility.IsAnyPrefabInstanceRoot(coinGraphic.gameObject))PrefabUtility.UnpackPrefabInstance(coinGraphic.gameObject,PrefabUnpackMode.Completely,InteractionMode.AutomatedAction);
            coinGraphic.rectTransform.sizeDelta=((RectTransform)main[47].transform).sizeDelta;
            Bind(main[52],1);Bind(main[53],2);Bind(main[42],3);Bind(coinGraphic.gameObject,4);
            var n=Page("SettingDialog",canvas.transform,2,out menus.pages[0]);
            Bind(n[27],0);Bind(n[19],5);Bind(n[20],5);Bind(n[22],6);Bind(n[23],6);Bind(n[28],8);Bind(n[29],7);
            menus.bgmOn=n[8];menus.bgmOff=n[9];menus.vibrateOn=n[13];menus.vibrateOff=n[14];Label(n,15,"3");Label(n,6,"4");Label(n,11,"5");Label(n,28,"6");Label(n,29,"7");
            n=Page("mergeRuleDialog",canvas.transform,2,out menus.pages[1]);Bind(n[3],0);Bind(n[16],0);
            Label(n,5,"1");Label(n,12,"2");Label(n,17,"28");Label(n,8,"92");menus.ruleAnimation=n[14].GetComponentInChildren<NativeSkeletonPlayer>(true);
            n=Page("PrivacyPolicyView",canvas.transform,7,out menus.pages[2]);Bind(n[25],0);menus.policyTitle=Text(n,13);menus.privacyScroll=n[5];menus.termsScroll=n[6];
            Scroll(n,5,8,2);Scroll(n,6,10,3);
            n=Page("GameFakeWDDialog",canvas.transform,2,out menus.pages[3]);Bind(n[16],0);Bind(n[25],9);Label(n,17,"8");Label(n,20,"53");Label(n,12,"8");Label(n,14,"8");menus.fakeBalance=Text(n,22);At<Image>(n,10).raycastTarget=true;
            Scroll(n,7,8,9);var list=(RectTransform)n[9].transform;list.sizeDelta=new Vector2(750,6*319+5*8);
            menus.fakeRows=new FakeWithdrawRow[6];
            for(int i=0;i<6;i++)
            {
                var row=NativeGameplayBuilder.Source("GameDialog/GameFakeWDItem",list);var rowNodes=NativeGameplayBuilder.Nodes(row);
                foreach(var g in row.GetComponentsInChildren<Graphic>(true))g.raycastTarget=false;
                var rect=(RectTransform)row.transform;rect.anchorMin=rect.anchorMax=new Vector2(.5f,1);rect.anchoredPosition=new Vector2(0,-159.5f-i*327);
                Bind(rowNodes[3],1000+i);Bind(rowNodes[5],1000+i);
                menus.fakeRows[i]=new FakeWithdrawRow {root=row,selected=rowNodes[3],unselected=rowNodes[5],amount=Text(rowNodes,11),condition=Text(rowNodes,13),remaining=Text(rowNodes,15),fill=Fill(rowNodes,4,8)};
            }
            var layout=menus.pages[3].AddComponent<RecoveredCashPageLayout>();layout.content=(RectTransform)n[2].transform;layout.top=(RectTransform)n[3].transform;layout.bottom=(RectTransform)n[5].transform;layout.scroll=(RectTransform)n[7].transform;layout.viewport=(RectTransform)n[8].transform;
            layout.content.anchorMin=Vector2.zero;layout.content.anchorMax=Vector2.one;layout.content.offsetMin=layout.content.offsetMax=Vector2.zero;
            layout.top.anchorMin=layout.top.anchorMax=new Vector2(.5f,1);layout.bottom.anchorMin=layout.bottom.anchorMax=new Vector2(.5f,0);
            layout.scroll.anchorMin=layout.scroll.anchorMax=new Vector2(.5f,.5f);layout.scroll.pivot=new Vector2(.5f,1);
            n=Page("GameRealWDDialog",canvas.transform,0,out menus.pages[4]);Bind(n[34],0);Bind(n[57],10);Bind(n[58],14);At<Image>(n,23).raycastTarget=true;
            Label(n,10,"8");Label(n,28,"8");Label(n,29,"8");Label(n,14,"9");Label(n,17,"10");Label(n,53,"11");Label(n,6,"105");
            menus.coinCount=Text(n,16);menus.coinHint=Text(n,55);menus.coinFill=Fill(n,21,50);menus.coinReady=n[9];menus.coinBlocked=n[22];Scroll(n,18,19,20);
            list=(RectTransform)n[20].transform;list.sizeDelta=new Vector2(700,3*129+2*8);menus.coinRows=new CoinWithdrawRow[6];
            for(int i=0;i<6;i++)
            {
                var row=NativeGameplayBuilder.Source("GameDialog/GameWithdrawItem",list);var rn=NativeGameplayBuilder.Nodes(row);foreach(var g in row.GetComponentsInChildren<Graphic>(true))g.raycastTarget=false;
                var rect=(RectTransform)row.transform;rect.anchorMin=rect.anchorMax=new Vector2(.5f,1);rect.anchoredPosition=new Vector2(-180+350*(i%2),-64.5f-137*(i/2));
                Bind(rn[2],1100+i);Bind(rn[4],1100+i);Bind(rn[3],1100+i);
                menus.coinRows[i]=new CoinWithdrawRow {root=row,selected=rn[2],enough=rn[3],amounts=new[]{Text(rn,5),Text(rn,6),Text(rn,7)}};
            }
            n=Page("GameRealWDAccount",canvas.transform,5,out menus.pages[5]);Bind(n[16],0);Bind(n[7],11);Label(n,9,"13");Label(n,13,"108");Label(n,19,"96");Label(n,26,"14");menus.forms[0]=Form(n,3,0,0,7,13);
            n=Page("GameRealWDAccountBR",canvas.transform,0,out menus.pages[6]);Bind(n[24],0);Bind(n[8],12);Label(n,11,"13");Label(n,20,"108");Label(n,27,"96");Label(n,34,"95");Label(n,41,"97");Label(n,48,"14");menus.forms[1]=Form(n,3,4,5,8,20);
            var br=menus.forms[1];br.platforms=new[]{At<Image>(n,7)};br.selected=new[]{n[26]};br.names=new[]{"BR:Pagbank"};br.platformPaths=new[]{Platform("Pagbank")};br.platforms[0].enabled=true;n[25].SetActive(false);Bind(n[7],2000);
            n=Page("GameRealWDAccountID",canvas.transform,6,out menus.pages[7]);Bind(n[23],0);Bind(n[18],13);Label(n,9,"13");Label(n,19,"108");Label(n,32,"96");Label(n,39,"95");Label(n,46,"14");menus.forms[2]=Form(n,3,4,0,18,19);
            var phone=menus.forms[2];phone.platforms=new[]{At<Image>(n,11),At<Image>(n,12),At<Image>(n,13)};phone.selected=new[]{n[24],n[26],n[28]};
            // Missing remote withdrawal_platform values use exactly GameManagement.getPlatformSpr's defaults.
            phone.names=new[]{"ID:DANA","TH:PayPal","MY:PayPal","VN:PayPal","PH:PayPal"};phone.platformPaths=new[]{Platform("DANA"),Platform("PayPal"),Platform("PayPal"),Platform("PayPal"),Platform("PayPal")};
            n[7].SetActive(false);for(int i=0;i<3;i++){phone.platforms[i].enabled=true;Bind(phone.platforms[i].gameObject,2000+i);}
            n=Page("GameRealTXYZ",canvas.transform,6,out menus.pages[8]);Bind(n[13],0);n[19].SetActive(false); // Source has no close-button handler here; continue is the close action.
            Label(n,12,"23");Label(n,30,"22");Label(n,35,"23");Label(n,40,"24");Label(n,42,"111");Label(n,14,"108");
            menus.verifyAmount=Text(n,16);menus.verifyCommission=Text(n,23);menus.verifyCredited=Text(n,25);
            var verify=menus.pages[8].AddComponent<RecoveredVerificationView>();menus.verification=verify;
            verify.first=n[28].GetComponentInChildren<NativeSkeletonPlayer>(true);verify.second=n[33].GetComponentInChildren<NativeSkeletonPlayer>(true);verify.stamp=n[38].GetComponentInChildren<NativeSkeletonPlayer>(true);
            verify.first.playOnEnable=verify.second.playOnEnable=verify.stamp.playOnEnable=false;verify.tips=new[]{n[30].transform,n[35].transform,n[40].transform,n[42].transform};verify.continueGroup=n[13];
            n=Page("GameRealWDTXTips",canvas.transform,4,out menus.pages[9]);Bind(n[7],0);Bind(n[8],0);Label(n,6,"13");Label(n,9,"108");menus.stageAmount=Text(n,18);menus.stagePercent=Text(n,10);menus.stageHint=Text(n,16);menus.stageFill=Fill(n,3,21);
            n=Page("GameRealWDActiveTips",canvas.transform,2,out menus.pages[10]);Bind(n[11],0);Bind(n[3],0);Label(n,4,"15");Label(n,8,"92");Label(n,12,"27");
            var toast=NativeGameplayBuilder.Source("CommonPrefab/Toast",canvas.transform);var tn=NativeGameplayBuilder.Nodes(toast);menus.toast=toast;menus.toastText=Text(tn,3);
            var tr=(RectTransform)toast.transform;tr.anchorMin=tr.anchorMax=new Vector2(.5f,.5f);tr.anchoredPosition=Vector2.zero;
            var toastCanvas=toast.AddComponent<Canvas>();toastCanvas.overrideSorting=true;toastCanvas.sortingOrder=900;foreach(var g in toast.GetComponentsInChildren<Graphic>(true))g.raycastTarget=false;toast.SetActive(false);
            var sound=new GameObject("MainMenuAudio");sound.transform.SetParent(root.transform,false);menus.audioCues=sound.AddComponent<PackagedAudio>();menus.audioCues.music=sound.AddComponent<AudioSource>();menus.audioCues.effects=sound.AddComponent<AudioSource>();
            menus.audioCues.music.loop=true;menus.audioCues.music.playOnAwake=menus.audioCues.effects.playOnAwake=false;
            if(!session.worldCamera.GetComponent<AudioListener>())session.worldCamera.gameObject.AddComponent<AudioListener>();
            string[] sounds={"bgm_game","click","no_click"};menus.audioCues.paths=new string[3];
            for(int i=0;i<3;i++){string target="Assets/Resources/MenuAudio/"+sounds[i]+".mp3";if(!File.Exists(target))AssetDatabase.CopyAsset("Assets/Art/HotUpdate/Audio/Audio/"+sounds[i]+".mp3",target);menus.audioCues.paths[i]="MenuAudio/"+sounds[i];}
            menus.actions=actions.ToArray();menus.labels=labels.ToArray();menus.currencies=currencies.ToArray();
            foreach(var page in menus.pages)DeferArt(page);DeferArt(toast);
        }
    }
}
