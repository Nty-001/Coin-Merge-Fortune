using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace CoinMerge.Recovery.Editor
{
    [InitializeOnLoad] public static class PrizeAnimationReskinValidation
    {
        const string Key="CoinMerge.PrizeAnimation.Validation",Prefix="coinmerge.prizeanimation.disposable";
        static string Output=>SessionState.GetString(Key+".output","Design/WheelLeverR1/Verification");
        static RecoveredGameSession session;static RecoveredWheelLever lever;static Camera camera;static RenderTexture target;
        static int step,checks,completions,lastHighest,chosen;static float start,peak;static double next;
        static bool sawRise,sawEffect,sawFlight;static readonly List<string> passed=new List<string>();
        [Serializable] sealed class Report{public bool passed;public string error,unityVersion;public int checks;public string[] scenarios;public float leverTravel,sourceSeconds,gameSeconds;}
        static PrizeAnimationReskinValidation(){EditorApplication.playModeStateChanged+=State;}
        public static void Run(string output="Design/WheelLeverR1/Verification")
        {
            SessionState.SetString(Key+".output",output);
            Directory.CreateDirectory(Output);SessionState.SetBool(Key,true);SessionState.SetString(Key+".error","");
            var store=new PlayerStore(Prefix);store.ResetPlayer();store.ResetProfile();store.Save(new PlayerProgress{guideStep=9999,fakeMoney=488.49,coin1024Number=50,gameTotalScore=0});
            store.SaveProfile(new VersionProfile{country="US",cohort="B",rewardedVariant=true});
            EditorSceneManager.OpenScene("Assets/Scenes/RecoveredMain.unity");var s=UnityEngine.Object.FindObjectOfType<RecoveredGameSession>();s.saveNamespace=Prefix;s.automaticInput=false;EditorApplication.EnterPlaymode();
        }
        static void State(PlayModeStateChange state)
        {
            if(!SessionState.GetBool(Key,false))return;
            if(state==PlayModeStateChange.EnteredPlayMode){session=null;step=checks=completions=0;passed.Clear();sawRise=sawEffect=sawFlight=false;next=EditorApplication.timeSinceStartup+.8;EditorApplication.isPaused=false;Time.timeScale=1;Application.logMessageReceived+=Log;EditorApplication.update+=Tick;}
            if(state==PlayModeStateChange.EnteredEditMode){Time.timeScale=1;var store=new PlayerStore(Prefix);store.ResetPlayer();store.ResetProfile();SessionState.SetBool(Key,false);EditorSceneManager.OpenScene("Assets/Scenes/RecoveredMain.unity");}
        }
        static void Log(string message,string stack,LogType type){if(type==LogType.Error||type==LogType.Exception)SessionState.SetString(Key+".error",message+"\n"+stack);}
        static void Completed(int index){completions++;Require(index==chosen,"Winner changed");}
        static void Tick()
        {
            if(EditorApplication.timeSinceStartup<next)return;
            try
            {
                var error=SessionState.GetString(Key+".error","");if(error.Length>0)throw new Exception(error);
                if(!session)
                {
                    session=UnityEngine.Object.FindObjectOfType<RecoveredGameSession>();Require(session&&session.Locale!=null,"Session initialization");
                    camera=session.worldCamera;target=new RenderTexture(1080,2340,24);camera.targetTexture=target;Canvas.ForceUpdateCanvases();session.playfieldLayout.Refresh();
                    lever=session.wheelView.GetComponentInChildren<RecoveredWheelLever>(true);Require(lever,"Lever prefab binding");session.wheelView.DrawCompleted+=Completed;
                }
                if(step==0)
                {
                    session.wheelView.Show(session.Player,session.Locale);session.wheelView.draw.onClick.Invoke();Require(!session.wheelView.IsSpinning,"Insufficient points started draw");
                    start=Time.time;step=1;return;
                }
                if(step==1)
                {
                    Require(Mathf.Abs(lever.knob.anchoredPosition.y-lever.knobRest.y)<.01f,"Idle lever moved");
                    if(Time.time-start<1)return;
                    passed.Add("Insufficient score and idle: no lever movement");
                    SourceSamples();Capture("machine_idle.png");
                    session.wheelView.backgroundAnimation.ResetPose("idle");lever.ApplyPose();
                    session.Player.gameTotalScore=RecoveredGameRules.RequiredScore(session.board.Config.rules.lotteryScores,session.Player.currentLotteryCount);
                    chosen=0;for(int i=0;i<session.board.Config.rules.lotteryRewards.Length;i++)if(session.board.Config.rules.lotteryRewards[i].type!="money"){chosen=i;break;}
                    BeginDraw();step=2;return;
                }
                if(step==2||step==4)
                {
                    float elapsed=Time.time-start;peak=Mathf.Max(peak,lever.PressedFraction);
                    if(elapsed<.80f)Require(Selected()<0,"Reward selection started before lever returned");
                    // Leave one rendered frame around the exact source boundaries (0.33335 / 0.5 s).
                    if(elapsed>.37f&&elapsed<.46f)Require(lever.PressedFraction>.99f,"Lever did not hold: elapsed="+elapsed+" pressed="+lever.PressedFraction);
                    if(session.wheelRewardView.gameObject.activeSelf)
                    {
                        Require(peak>.95f,"Live lever never reached pressed pose");Require(lever.PressedFraction<.005f,"Lever did not return");Require(completions==(step==2?1:2),"Draw completed more than once");
                        Require(session.Player.gameTotalScore==0,"Points spent incorrectly");Require(session.Player.currentLotteryCount==completions,"Draw count changed incorrectly");
                        Require(Selected()==chosen,"Wrong selected reward");
                        Capture(step==2?"draw_coin_reward.png":"draw_cash_reward.png");passed.Add("Live draw "+completions+": lever -> selection -> reward, duplicate click ignored");
                        session.wheelRewardView.claim.onClick.Invoke();start=Time.time;step=step==2?3:5;return;
                    }
                    Require(elapsed<18,"Draw never completed");return;
                }
                if(step==3)
                {
                    if(session.wheelRewardView.gameObject.activeSelf){Require(Time.time-start<10,"Coin claim never completed");return;}
                    session.Player.gameTotalScore=RecoveredGameRules.RequiredScore(session.board.Config.rules.lotteryScores,session.Player.currentLotteryCount);
                    for(int i=0;i<session.board.Config.rules.lotteryRewards.Length;i++)if(session.board.Config.rules.lotteryRewards[i].type=="money"){chosen=i;break;}
                    BeginDraw();step=4;return;
                }
                if(step==5)
                {
                    if(session.wheelRewardView.gameObject.activeSelf){Require(Time.time-start<10,"Cash claim never completed");return;}
                    session.lifecycle.ResetVisuals();lastHighest=session.Player.coin1024Number;
                    var point=session.board.ground.position+Vector3.up*5;
                    var a=session.board.Spawn(1000,point+Vector3.left*.3f);var b=session.board.Spawn(1000,point+Vector3.right*.3f);
                    session.board.Merge(a,b);start=Time.time;step=6;return;
                }
                if(step==6)
                {
                    var flow=session.lifecycle;
                    if(flow.HighestPhase==1)sawRise=true;
                    if(flow.HighestPhase==2)
                    {
                        var sprite=flow.highestEffect.GetComponentInChildren<RecoveredSkeletalSprite>();
                        Require(sprite&&sprite.image.sprite==session.board.SpriteFor(2000),"Highest animation still uses old coin");
                        Require(flow.highestEffect.graphic.hiddenSlots.Length==1&&flow.highestEffect.graphic.hiddenSlots[0]==21,"Glow layers changed");
                        Require(sprite.image.preserveAspect&&!sprite.image.raycastTarget,"Chip aspect / input changed");
                        if(!sawEffect&&Time.time-start>1){Capture("highest_2000_celebration.png");sawEffect=true;}
                    }
                    if(flow.HighestPhase==3&&!sawFlight){Capture("highest_2000_flight.png");sawFlight=true;}
                    if(sawFlight&&flow.HighestPhase==0)
                    {
                        Require(sawRise&&sawEffect,"Highest animation stages skipped");Require(session.Player.coin1024Number==lastHighest+1,"Highest reward counted incorrectly");Require(!session.board.HighestFlowActive,"Highest flow left board blocked");
                        passed.Add("1000 + 1000 -> approved 2000 chip -> source celebration -> counter flight -> gameplay resumes");
                        session.ShowReward(4);Capture("highest_reward_popup.png");
                        foreach(var image in session.rewardView.highestGroup.GetComponentsInChildren<UnityEngine.UI.Image>())
                            if(image.sprite&&AssetDatabase.GetAssetPath(image.sprite).EndsWith("/2000.png"))Require(image.sprite==session.board.SpriteFor(2000),"Highest popup source differs");
                        passed.Add("Highest reward popup uses approved chip resource");Finish(null);return;
                    }
                    Require(Time.time-start<15,"Highest animation did not finish");
                }
            }
            catch(Exception e){Finish(e.ToString());}
        }
        static void BeginDraw()
        {
            session.GmSetNextWheel(chosen);session.wheelView.Show(session.Player,session.Locale);session.wheelView.draw.onClick.Invoke();session.wheelView.draw.onClick.Invoke();
            Require(session.wheelView.IsSpinning,"Valid draw not started");start=Time.time;peak=0;
        }
        static int Selected(){for(int i=0;i<session.wheelView.slots.Length;i++)if(session.wheelView.slots[i].selected.activeSelf)return i;return -1;}
        static void SourceSamples()
        {
            var player=session.wheelView.backgroundAnimation;
            Require(Mathf.Abs(player.timeScale-2)<.001f,"Source playback speed changed");
            for(int i=0;i<=100;i++)
            {
                float t=i*1.6667f/100;player.EvaluateAt("idle",t);lever.ApplyPose();
                float y=lever.sourceKnob.y;Require(y>=502.21f&&y<=586.69f,"Source lever travel outside range");
                Require(Mathf.Abs(lever.knob.localScale.x-lever.knob.localScale.y)<.0001f,"Knob stretched");
                Require(Mathf.Abs(lever.knob.anchoredPosition.x-lever.knobRest.x)<.0001f,"Knob horizontal drift");
            }
            var frames=new[]{0f,.3333f,.6667f,1f,1.33335f,1.6667f};
            for(int i=0;i<frames.Length;i++){player.EvaluateAt("idle",frames[i]);lever.ApplyPose();Capture("lever_pose_"+i+".png");}
            player.ResetPose("idle");lever.ApplyPose();passed.Add("101 source clip samples: 1.6667 s / timeScale 2; round knob, fixed X, pull and return");
        }
        static void Require(bool condition,string message){checks++;if(!condition)throw new Exception(message);}
        static void Capture(string filename)
        {
            Canvas.ForceUpdateCanvases();camera.Render();var previous=RenderTexture.active;RenderTexture.active=target;
            var image=new Texture2D(target.width,target.height,TextureFormat.RGB24,false);image.ReadPixels(new Rect(0,0,target.width,target.height),0,0);image.Apply();File.WriteAllBytes(Output+"/"+filename,image.EncodeToPNG());
            if(filename.StartsWith("lever_pose_",StringComparison.Ordinal))
            {
                var detail=new Texture2D(210,390,TextureFormat.RGB24,false);detail.ReadPixels(new Rect(870,target.height-1040,210,390),0,0);detail.Apply();File.WriteAllBytes(Output+"/joint_"+filename,detail.EncodeToPNG());UnityEngine.Object.DestroyImmediate(detail);
            }
            RenderTexture.active=previous;UnityEngine.Object.DestroyImmediate(image);
        }
        static void Finish(string error)
        {
            EditorApplication.update-=Tick;Application.logMessageReceived-=Log;if(session)session.wheelView.DrawCompleted-=Completed;
            if(camera)camera.targetTexture=null;if(target)UnityEngine.Object.DestroyImmediate(target);
            File.WriteAllText(Output+"/play_mode.json",JsonUtility.ToJson(new Report{passed=error==null,error=error,unityVersion=Application.unityVersion,checks=checks,scenarios=passed.ToArray(),leverTravel=lever?lever.travel:0,sourceSeconds=1.6667f,gameSeconds=.83335f},true));
            Debug.Log(error==null?"PRIZE_ANIMATION_RESKIN_VALIDATED "+checks:error);EditorApplication.ExitPlaymode();
        }
    }
}
