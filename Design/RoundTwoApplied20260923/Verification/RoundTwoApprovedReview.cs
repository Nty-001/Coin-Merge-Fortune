using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
namespace CoinMerge.Recovery.Editor
{
    [InitializeOnLoad] public static class RoundTwoApprovedReview
    {
        const string Key="RoundTwoApprovedReview",Prefix="coinmerge.round.two.disposable";
        static string Output=>Path.GetFullPath("RoundTwoApprovedReview");
        static RecoveredGameSession s;static Camera cam,bg;static RenderTexture rt,back;
        static int index;static double next,deadline;static readonly List<Entry> entries=new List<Entry>();
        [Serializable] sealed class Entry{public string name,scope;public string[] texts,resources;public string file;}
        [Serializable] sealed class Report{public string error;public Entry[] screens;}
        static RoundTwoApprovedReview(){EditorApplication.playModeStateChanged+=State;}
        public static void Run(){RoundTwoApprovedAuthor.Run();Start();}
        static void Start()
        {
            Directory.CreateDirectory(Output);SessionState.SetBool(Key,true);SessionState.SetString(Key+".error","");
            var store=new PlayerStore(Prefix);store.ResetPlayer();store.ResetProfile();store.Save(new PlayerProgress{guideStep=9999,fakeMoney=482.32,coin1024Number=503,open_bgm=true,open_music=true,open_vibrate=true});store.SaveProfile(new VersionProfile{country="US",cohort="B",rewardedVariant=true});
            
            EditorSceneManager.OpenScene("Assets/Scenes/RecoveredMain.unity");
            UnityEngine.Object.FindObjectOfType<RecoveredGameSession>().saveNamespace=Prefix;
            EditorApplication.EnterPlaymode();
        }
        static void State(PlayModeStateChange state)
        {
            if(!SessionState.GetBool(Key,false))return;
            if(state==PlayModeStateChange.EnteredPlayMode){index=-1;next=EditorApplication.timeSinceStartup+2;deadline=next+150;entries.Clear();Application.logMessageReceived+=Log;EditorApplication.update+=Tick;}
            if(state==PlayModeStateChange.EnteredEditMode){SessionState.SetBool(Key,false);var store=new PlayerStore(Prefix);store.ResetPlayer();store.ResetProfile();EditorApplication.Exit(SessionState.GetString(Key+".error","").Length==0?0:1);}
        }
        static void Log(string message,string trace,LogType type){if(type==LogType.Error||type==LogType.Exception)SessionState.SetString(Key+".error",message+"\n"+trace);}
        static void Tick()
        {
            if(EditorApplication.timeSinceStartup<next)return;next=EditorApplication.timeSinceStartup+4;
            try
            {
                if(EditorApplication.timeSinceStartup>deadline)throw new TimeoutException();var error=SessionState.GetString(Key+".error","");if(error.Length>0)throw new Exception(error);

                if(index<0)
                {
                    s=UnityEngine.Object.FindObjectOfType<RecoveredGameSession>();if(!s||s.Player==null)return;s.automaticInput=false;s.rewardView.enabled=false;cam=s.worldCamera;
                    var viewport=cam.GetComponent<DeviceSafeViewport>();if(viewport)viewport.enabled=false;
                    var root=s.transform;var b=root.Find("SafeAreaBackdrop");if(b)bg=b.GetComponent<Camera>();
                    rt=new RenderTexture(1080,1920,24);cam.targetTexture=rt;cam.rect=new Rect(0,0,1,1);
                    if(bg){back=new RenderTexture(1080,1920,24);bg.targetTexture=back;}
                    if(viewport)foreach(var scaler in viewport.scalers){scaler.scaleFactor=1.44f;scaler.GetComponent<Canvas>().scaleFactor=1.44f;}
                    index=0;return;
                }
                Rewarded();
                index++;
            }catch(Exception e){Finish(e.ToString());}
        }
        static void Rewarded()
        {
            var m=s.menus;var g=s.guideView;
            m.CloseAll();s.rewardView.gameObject.SetActive(false);s.failView.gameObject.SetActive(false);s.rating.gameObject.SetActive(false);s.wheelView.gameObject.SetActive(false);s.wheelRewardView.gameObject.SetActive(false);g.gameObject.SetActive(false);
            if(index==11){rt.Release();rt.height=2340;rt.Create();if(back){back.Release();back.height=2340;back.Create();}}
            if(index<=4||index==11||index==12)
            {
                bool br=index==0||index==1||index==11;bool filled=index==1||index==3;string country=br?"BR":index==4?"TH":"ID";
                s.ChangeProfile(country,"B",true);s.Player.raccountName="";s.Player.rfullName="";s.Player.rdocumentId="";
                m.Show(4);m.Act(10);int target=br?6:7;if(m.CurrentPage!=target)throw new Exception("Native account entry failed");var f=m.forms[br?1:2];
                f.account.text="";f.fullName.text="";if(f.taxId)f.taxId.text="";
                if(filled){f.account.text=br?"review@example.com":"081234567890";f.fullName.text="Review User";if(f.taxId)f.taxId.text="12345678900";}
                if(f.confirm.sprite.name!=(filled?"GreenButton":"DisabledButton"))throw new Exception("Account button state mismatch");
                if(f.platforms[0].sprite.name!=(br?"PagBankCard":index==4?"PayPalCard":"DanaCard"))throw new Exception("Country platform mismatch");
                Capture(index==0?"06_brazil":index==1?"Brazil_filled":index==2?"07_phone":index==3?"Phone_filled":index==4?"Phone_TH":index==11?"Tall_brazil":"Tall_phone",m.pages[target]);return;
            }
            if(index>=5&&index<=8||index==13||index==14)
            {
                s.ChangeProfile(index==14?"JP":"US","B",true);int step=index==5?0:index==6?1:index==7||index==14?3:4;
                s.Player.guideStep=step;g.Show(step,s.Player.fakeMoney);g.AlignCashGuide();
                if(step==4&&g.coinContent)g.coinContent.position+=g.coinTarget.position-g.coinAnchor.position;
                Capture(index==5?"11_guide_drag":index==6?"12_guide_merge":index==7?"13_guide_balance":index==8?"14_guide_coin":index==13?"Tall_coin_guide":"Cash_guide_JP",g.gameObject);return;
            }
            if(index==9||index==10)
            {
                s.ChangeProfile("US","B",true);s.Player.guideStep=9999;g.gameObject.SetActive(false);m.Act(2);m.ruleAnimation.EvaluateAt("animation",index==9?0:3);m.ruleAnimation.GetComponent<NativeRulesCoinImages>().Refresh();
                Capture(index==9?"Rules_reveal_start":"09_rules",m.pages[1]);
                if(index==10){int count=0;foreach(var coin in m.ruleAnimation.GetComponent<NativeRulesCoinImages>().coins)if(coin.image.enabled&&coin.image.color.a>.9f)count++;if(count!=11)throw new Exception("Expected eleven fully revealed coins, got "+count);m.Act(0);if(m.CurrentPage>=0)throw new Exception("Rule close failed");}return;
            }
            if(index==15)
            {
                s.ChangeProfile("US","B",true);s.Player.guideStep=1;g.Show(1,0);g.oneButton.onClick.Invoke();if(s.Player.guideStep!=2||!s.rewardView.gameObject.activeSelf)throw new Exception("Merge tutorial did not show newbie reward");
                s.rewardView.gameObject.SetActive(false);s.Player.guideStep=3;g.Show(3,0);g.threeButton.onClick.Invoke();if(s.Player.guideStep!=4)throw new Exception("Cash tutorial advance failed");
                g.fourButton.onClick.Invoke();if(s.Player.guideStep!=9999||g.gameObject.activeSelf)throw new Exception("Coin tutorial finish failed");
                Finish(null);
            }
        }
        static void Capture(string name,GameObject scope)
        {
            Canvas.ForceUpdateCanvases();foreach(var popup in UnityEngine.Object.FindObjectsOfType<RecoveredMenuPopup>())popup.Tick(1);Canvas.ForceUpdateCanvases();if(s)s.playfieldLayout.Refresh();
            if(bg){bg.Render();Graphics.Blit(back,rt);}cam.Render();var old=RenderTexture.active;RenderTexture.active=rt;var image=new Texture2D(rt.width,rt.height,TextureFormat.RGB24,false);image.ReadPixels(new Rect(0,0,rt.width,rt.height),0,0);image.Apply();File.WriteAllBytes(Path.Combine(Output,name+".png"),image.EncodeToPNG());RenderTexture.active=old;UnityEngine.Object.DestroyImmediate(image);
            var texts=new List<string>();foreach(var t in scope.GetComponentsInChildren<Text>())if(t.enabled&&!string.IsNullOrEmpty(t.text))texts.Add(t.text);
            var art=new HashSet<string>();foreach(var loader in UnityEngine.Object.FindObjectsOfType<RecoveredMenuArt>(true))if(loader.images!=null)foreach(var b in loader.images)if(b.image&&b.image.enabled&&b.image.gameObject.activeInHierarchy&&b.image.transform.IsChildOf(scope.transform))art.Add(b.resourcePath);
            entries.Add(new Entry{name=name,scope=scope.name,texts=texts.ToArray(),resources=new List<string>(art).ToArray(),file=name+".png"});
        }
        static void Finish(string error)
        {
            EditorApplication.update-=Tick;Application.logMessageReceived-=Log;SessionState.SetString(Key+".error",error??"");File.WriteAllText(Path.Combine(Output,"rewarded.json"),JsonUtility.ToJson(new Report{error=error,screens=entries.ToArray()},true));EditorApplication.ExitPlaymode();
        }
    }
}
