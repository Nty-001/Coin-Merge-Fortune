using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
namespace CoinMerge.Recovery.Editor
{
    [InitializeOnLoad] public static class CurrencyReskinValidation
    {
        const string Key="CoinMerge.CurrencyReskin.Validation",Prefix="coinmerge.currency.reskin.disposable",Output="Design/CurrencyR1/Verification";
        static RecoveredGameSession session;static Camera camera;static RenderTexture target;
        static int countryIndex,step,checks;static double next;static string[] countries;static readonly List<string> passed=new List<string>();
        [Serializable] sealed class Report {public bool passed;public string error,unityVersion;public int checks;public string[] countries,scenarios;}
        static CurrencyReskinValidation(){EditorApplication.playModeStateChanged+=State;}
        [MenuItem("Coin Merge/Reskin/Validate localized banknotes")]
        public static void Run()
        {
            Directory.CreateDirectory(Output);SessionState.SetBool(Key,true);SessionState.SetString(Key+".error","");
            var store=new PlayerStore(Prefix);store.ResetPlayer();store.ResetProfile();store.Save(new PlayerProgress{guideStep=5,fakeMoney=488.49,coin1024Number=50,gameTotalScore=128});store.SaveProfile(new VersionProfile{country="US",cohort="B",rewardedVariant=true});
            EditorSceneManager.OpenScene("Assets/Scenes/RecoveredMain.unity");var s=UnityEngine.Object.FindObjectOfType<RecoveredGameSession>();s.saveNamespace=Prefix;s.automaticInput=false;EditorApplication.EnterPlaymode();
        }
        static void State(PlayModeStateChange state)
        {
            if(!SessionState.GetBool(Key,false))return;
            if(state==PlayModeStateChange.EnteredPlayMode){session=null;passed.Clear();countryIndex=step=checks=0;next=EditorApplication.timeSinceStartup+.8;EditorApplication.isPaused=false;Time.timeScale=1;Application.logMessageReceived+=Log;EditorApplication.update+=Tick;}
            if(state==PlayModeStateChange.EnteredEditMode){Time.timeScale=1;var store=new PlayerStore(Prefix);store.ResetPlayer();store.ResetProfile();SessionState.SetBool(Key,false);EditorSceneManager.OpenScene("Assets/Scenes/RecoveredMain.unity");}
        }
        static void Log(string message,string stack,LogType type){if(type==LogType.Error||type==LogType.Exception)SessionState.SetString(Key+".error",message+"\n"+stack);}
        static void Tick()
        {
            if(EditorApplication.timeSinceStartup<next)return;
            try
            {
                string error=SessionState.GetString(Key+".error","");if(error.Length>0)throw new Exception(error);
                if(!session)
                {
                    session=UnityEngine.Object.FindObjectOfType<RecoveredGameSession>();if(!session||session.Locale==null)throw new Exception("Session did not initialize");
                    camera=session.worldCamera;target=new RenderTexture(1080,2340,24);camera.targetTexture=target;Time.timeScale=0;Canvas.ForceUpdateCanvases();session.playfieldLayout.Refresh();
                    var list=new List<string>{"US","RU","JP"};foreach(string c in session.board.Config.rules.supportedCountries)if(!list.Contains(c))list.Add(c);countries=list.ToArray();
                }
                if(countryIndex>=countries.Length){Finish(null);return;}
                Hide();string country=countries[countryIndex];
                if(step==0)
                {
                    session.lifecycle.ResetVisuals();session.ChangeProfile(country,"B",true);session.RefreshPresentation();
                    var locale=session.Locale;var expected=Resources.Load<Sprite>(locale.Data.icons[0]);
                    for(int type=1;type<=3;type++){Require(locale.Icon(type)==expected,"Currency role still uses old art "+country+" / "+type);Require(locale.Data.icons[type-1].StartsWith("Localization/Currency/1/",StringComparison.Ordinal),"Old locale path");}
                }
                string name;
                if(step==0)name="home";
                else if(step<=11){session.menus.Show(step-1);name="page_"+(step-1);}
                else if(step<=16){session.rewardView.Show(step-11,6.72);name="reward_"+(step-11);}
                else if(step==17){session.wheelView.Show(session.Player,session.Locale);name="machine";}
                else if(step==18||step==19)
                {
                    int found=-1;bool pile=step==19;
                    for(int i=0;i<session.board.Config.rules.lotteryRewards.Length;i++){var r=session.board.Config.rules.lotteryRewards[i];if(r.type=="money"&&(r.amount>5000)==pile){found=i;break;}}
                    Require(found>=0,"Missing cash reward test case");session.wheelRewardView.Show(found,session.Player,session.board.Config,session.Locale,.4);name=pile?"machine_cash_pile":"machine_cash_single";
                }
                else {session.lifecycle.FlyMoney(6.72);session.lifecycle.Tick(.23f);name="cash_flight";}
                foreach(var popup in session.GetComponentsInChildren<RecoveredMenuPopup>())popup.Tick(1);
                Canvas.ForceUpdateCanvases();camera.Render();Audit(country+" / "+name);
                // Every country has a real reward screenshot, and representative locales cover all roles/screens.
                if(step==13||(countryIndex<3&&(step==0||step==3||step==12||step==16||step>=17)))Capture(country+"_"+name+".png");
                passed.Add(country+" / "+name);step++;
                if(step>20){step=0;countryIndex++;File.WriteAllText(Output+"/progress.txt",country+" complete / "+countryIndex+" of "+countries.Length);}
                next=EditorApplication.timeSinceStartup+.02;
            }
            catch(Exception e){Finish(e.ToString());}
        }
        static void Hide(){session.menus.CloseAll();session.rewardView.gameObject.SetActive(false);session.wheelView.gameObject.SetActive(false);session.wheelRewardView.gameObject.SetActive(false);session.guideView.gameObject.SetActive(false);session.rating.gameObject.SetActive(false);session.failView.gameObject.SetActive(false);}
        static void Audit(string context)
        {
            Sprite expected=session.Locale.Icon(1);
            foreach(var binding in session.currencyIcons)Check(binding.image,binding.type,expected,context);
            // Menus refresh their locale when opened; closed pages may retain the last country's cached sprite.
            foreach(var binding in session.menus.currencies)if(binding.image.gameObject.activeInHierarchy)Check(binding.image,binding.type,expected,context);
            if(session.wheelView.gameObject.activeSelf)
                for(int i=0;i<session.wheelView.slots.Length;i++){var r=session.board.Config.rules.lotteryRewards[i];if(r.type=="money")Check(session.wheelView.slots[i].money,r.amount>5000?3:1,expected,context);}
            foreach(var image in session.GetComponentsInChildren<Image>())
            {
                if(!image.enabled||!image.sprite)continue;string path=AssetDatabase.GetAssetPath(image.sprite);
                Require(!path.Contains("Localization/Currency/2/")&&!path.Contains("Localization/Currency/3/"),context+" old visible art "+path);
                if(!path.Contains("Localization/Currency/1/"))continue;
                Require(image.sprite==expected,context+" wrong country: "+path);Require(image.preserveAspect,context+" banknote stretched: "+image.name);
                var mesh=image.canvasRenderer.GetMesh();if(mesh&&mesh.vertexCount==4)
                {
                    var vertices=mesh.vertices;var uv=mesh.uv;Vector2 min=uv[0],max=uv[0];var bounds=new Bounds(vertices[0],Vector3.zero);
                    for(int i=1;i<4;i++){bounds.Encapsulate(vertices[i]);min=Vector2.Min(min,uv[i]);max=Vector2.Max(max,uv[i]);}
                    // Image removes transparent sprite padding; compare its actual UV rectangle, not the padded canvas.
                    float actual=bounds.size.x/Mathf.Max(.001f,bounds.size.y),source=(max.x-min.x)*image.sprite.texture.width/Mathf.Max(.001f,(max.y-min.y)*image.sprite.texture.height);
                    Require(Mathf.Abs(actual-source)<.025f,context+" rendered banknote aspect differs: "+image.name+" actual="+actual+" source="+source);
                }
                Require(!image.raycastTarget,context+" banknote blocks buttons");
            }
        }
        static void Check(Image image,int type,Sprite expected,string context)
        {
            Require(image.sprite==expected,context+" stale binding "+image.name);
            if(type==1){Require(image.enabled,context+" hidden single banknote");return;}
            var view=image.GetComponent<RecoveredCurrencyArtwork>();Require(view&&view.CurrentType==type,context+" missing pile role");Require(!image.enabled,context+" single note drawn behind pile");
            var notes=type==2?view.smallNotes:view.largeNotes;Require(notes.Length==(type==2?3:9),context+" wrong pile size");
            foreach(var note in notes)Require(note.sprite==expected,context+" stale banknote inside pile");
        }
        static void Require(bool condition,string message){checks++;if(!condition)throw new Exception(message);}
        static void Capture(string filename){var previous=RenderTexture.active;RenderTexture.active=target;var image=new Texture2D(target.width,target.height,TextureFormat.RGB24,false);image.ReadPixels(new Rect(0,0,target.width,target.height),0,0);image.Apply();File.WriteAllBytes(Output+"/"+filename,image.EncodeToPNG());RenderTexture.active=previous;UnityEngine.Object.DestroyImmediate(image);}
        static void Finish(string error)
        {
            EditorApplication.update-=Tick;Application.logMessageReceived-=Log;if(camera)camera.targetTexture=null;if(target)UnityEngine.Object.DestroyImmediate(target);
            File.WriteAllText(Output+"/play_mode.json",JsonUtility.ToJson(new Report{passed=error==null,error=error,unityVersion=Application.unityVersion,checks=checks,countries=countries,scenarios=passed.ToArray()},true));Debug.Log(error==null?"CURRENCY_RESKIN_VALIDATED "+checks:error);EditorApplication.ExitPlaymode();
        }
    }
}
