using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace CoinMerge.Recovery.Editor
{
    [InitializeOnLoad] public static class InteractionParityValidation
    {
        const string Key="CoinMerge.InteractionValidation",Prefix="coinmerge.interaction.disposable";
        static readonly List<string> checks=new List<string>();static int result;
        static InteractionParityValidation(){EditorApplication.playModeStateChanged+=State;}
        public static void Run()
        {
            var store=new PlayerStore(Prefix);store.ResetPlayer();store.ResetProfile();
            store.Save(new PlayerProgress{guideStep=9999,hasSavedGameScene=true,currentLotteryCount=100});
            EditorSceneManager.OpenScene("Assets/Scenes/RecoveredMain.unity");
            var s=UnityEngine.Object.FindObjectOfType<RecoveredGameSession>();s.saveNamespace=Prefix;s.automaticInput=false;
            SessionState.SetBool(Key,true);EditorApplication.EnterPlaymode();
        }
        static void State(PlayModeStateChange state)
        {
            if(!SessionState.GetBool(Key,false))return;
            if(state==PlayModeStateChange.EnteredPlayMode)EditorApplication.delayCall+=Check;
            if(state==PlayModeStateChange.EnteredEditMode){SessionState.SetBool(Key,false);var store=new PlayerStore(Prefix);store.ResetPlayer();store.ResetProfile();EditorApplication.Exit(result);}
        }
        static void Require(bool value,string message){if(!value)throw new Exception(message);checks.Add(message);}
        static void Empty(RecoveredGameSession s)
        {s.Player.savedCoins=new SavedCoin[0];s.Player.hasSavedGameScene=true;s.board.Initialize(s.Player);s.lifecycle.ResetVisuals();}
        static void Check()
        {
            result=0;checks.Clear();string error="";
            try
            {
                var s=UnityEngine.Object.FindObjectOfType<RecoveredGameSession>();s.Initialize(new PlayerStore(Prefix));s.enabled=false;
                s.OnApplicationFocus(true);s.OnApplicationPause(false);
                var b=s.board;var f=s.coinFeedback;var ad=s.adPlayback;ad.enabled=false;f.enabled=false;
                Require(f&&ad&&ad.finish&&ad.cancel&&ad.panel,"Authored scene contains feedback, blocking mock canvas and bound Buttons");
                Require(Resources.Load<AudioClip>(f.dropSoundPath),"Original sfx_bom drop clip resolves at runtime");
                var data=Resources.Load<NativeSkeletonData>(f.collisionPrefab.dataPath);
                Require(data.sourceUuid=="1059e560-1ac0-4260-80ef-2291746bdd38"&&f.animationSpeed==4,"Original collision skeleton and playback speed");
                Require(f.vibrationMilliseconds==20&&f.vibrationAmplitude==255,"Original Android 20 ms / 255 pulse parameters");
                Empty(s);s.Player.open_music=true;b.Tick(.4f);int sound=f.DropSounds;b.RequestDrop();
                Require(f.DropSounds==sound+1,"Drop triggers original sound");
                b.Tick(.4f);s.Player.open_music=false;b.RequestDrop();Require(f.DropSounds==sound+1,"Music preference suppresses drop sound");
                Empty(s);s.Player.open_vibrate=true;var c=b.Spawn(1,new Vector3(0,b.ground.position.y+1));int vibration=f.HapticRequests,effects=f.CollisionEffects;
                for(int i=0;i<90;i++){b.Tick(1f/60);Physics2D.Simulate(1f/60);}
                Require(f.CollisionEffects==effects+1&&f.HapticRequests==vibration+1,"Real non-merge ground collision triggers effect and vibration once");
                for(int i=0;i<90;i++){b.Tick(1f/60);Physics2D.Simulate(1f/60);}
                Require(f.CollisionEffects==effects+1,"Resting contacts do not replay collision effect");
                f.Tick(2);Require(f.ActiveEffects==0,"Finished collision effect returns to pool");
                Empty(s);s.Player.open_vibrate=false;b.Spawn(2,new Vector3(0,b.ground.position.y+1));
                for(int i=0;i<90;i++){b.Tick(1f/60);Physics2D.Simulate(1f/60);}
                Require(f.HapticRequests==vibration+1&&f.CollisionEffects==effects+2,"Vibration off preserves collision visuals and recycled coin can retrigger");
                Empty(s);s.Player.open_vibrate=true;b.Spawn(1,new Vector3(-.1f,b.ground.position.y+1));b.Spawn(1,new Vector3(.1f,b.ground.position.y+1));
                int merges=0;b.Merged+=value=>merges++;
                for(int i=0;i<10;i++){b.Tick(1f/60);Physics2D.Simulate(1f/60);}
                Require(merges>0&&f.HapticRequests>vibration+1,"Real merge also triggers enabled haptic feedback");
                Empty(s);var coins=new NativeMergeCoin[6];
                for(int i=0;i<6;i++)coins[i]=b.Spawn(i==0?100:1,new Vector3(0,b.ground.position.y+1+i));
                b.TriggerFailure();int count=b.Coins.Count,removed=b.Revive();
                Require(removed==2&&b.Coins.Count==count&&b.Reviving&&b.InputBlocked,"Revive selects top third without immediate deletion and blocks input");
                b.Tick(.02f);Require(coins[5].transform.localScale.x<1&&coins[5].visual.color.a<1&&coins[4].transform.localScale.x==1,"First coin shrinks/fades while second waits its 0.04 s stagger");
                Require(b.Revive()==0,"Repeated revive cannot restart an active removal");
                b.Tick(.11f);Require(!coins[5].IsAlive&&coins[4].IsAlive&&b.Reviving,"First coin completes before second");
                b.Tick(.04f);Require(b.Coins.Count==5&&coins[0].IsAlive&&!b.GameOver&&!b.Reviving&&b.Preview,"Animation completion preserves highest coin and resumes play (removed objects may be pooled as preview)");
                Empty(s);b.Spawn(100,new Vector3(0,b.ground.position.y+1));b.TriggerFailure();Require(b.Revive()==0&&!b.Reviving&&!b.GameOver,"Highest-only board resumes without a stuck animation");
                Empty(s);s.Player.open_music=true;ad.playbackSeconds=5;
                var task=s.Sdk.ShowRewarded("validation");Require(!task.IsCompleted&&ad.Playing&&!ad.finish.interactable,"Mock ad starts pending and cannot close successfully early");
                ad.Finish();Require(!task.IsCompleted,"Early successful-close request ignored");
                var concurrent=s.Sdk.ShowRewarded("concurrent");Require(concurrent.IsCompleted&&concurrent.Result==AdOutcome.Unavailable,"Concurrent ads are rejected");
                s.OnApplicationPause(true);ad.Tick(8);Require(!ad.Ready,"Background time does not advance playback");s.OnApplicationPause(false);
                ad.Tick(4.9f);Require(!task.IsCompleted&&!ad.Ready,"No reward before playback duration");ad.Tick(.2f);
                Require(ad.Ready&&!task.IsCompleted,"Playback completion waits for user close");ad.Finish();Require(task.IsCompleted&&task.Result==AdOutcome.Completed&&!s.Sdk.AdInProgress,"Close resolves success and releases ad lock");
                task=s.Sdk.ShowRewarded("cancel");ad.Cancel();Require(task.IsCompleted&&task.Result==AdOutcome.Cancelled,"Early cancel returns cancellation");
                s.Sdk.NextAdOutcome=AdOutcome.Failed;task=s.Sdk.ShowRewarded("failure");s.Sdk.NextAdOutcome=AdOutcome.Completed;ad.Tick(5);
                Require(task.IsCompleted&&task.Result==AdOutcome.Failed,"Failure resolves after waiting and uses request-time outcome");
                s.Sdk.NextAdOutcome=AdOutcome.Unavailable;task=s.Sdk.ShowRewarded("missing");Require(task.IsCompleted&&task.Result==AdOutcome.Unavailable&&!ad.Playing,"Unavailable ad never opens playback");
                s.Sdk.NextAdOutcome=AdOutcome.Completed;b.TriggerFailure();s.failView.Show(s.Player);int watched=s.Player.watch_video_count;
                s.failView.revive.onClick.Invoke();Require(s.AdShowing&&ad.Playing&&b.InputBlocked,"Real revive button enters waiting state");
                s.Tick(10);Require(b.GameOver&&!b.Reviving&&s.Player.watch_video_count==watched,"Gameplay and reward remain frozen during ad");
                ad.Cancel();Require(!s.AdShowing&&s.failView.revive.interactable&&s.Player.watch_video_count==watched,"Cancelled revive can retry without reward");
                s.failView.revive.onClick.Invoke();ad.Tick(5);ad.Finish();Require(!s.AdShowing&&s.rewardView.gameObject.activeSelf&&s.Player.watch_video_count==watched+1,"Successful revive ad follows original reward popup branch exactly once");
                s.rewardView.Close();Require(!b.GameOver,"Closing revive reward resumes eligible board");
                Empty(s);s.GmPrepareDrop(22);b.Tick(.4f);b.RequestDrop();s.Tick(.01f);s.Tick(1);
                Require(s.AdShowing&&ad.Playing&&s.Player.windowsCointimes==22,"22nd drop enters original 1_A waiting branch");
                ad.Tick(5);ad.Finish();Require(s.rewardView.gameObject.activeSelf&&s.rewardView.Kind==2&&s.Player.windowsCointimes==0,"Completed drop ad opens double reward and resets counter");s.rewardView.Close();
                Empty(s);s.Player.currentLotteryCount=b.Config.rules.flow.drawRewardStrong;
                s.wheelRewardView.Show(0,s.Player,b.Config,s.Locale,.5);double cash=s.Player.fakeMoney;int tokens=s.Player.coin1024Number;
                s.wheelRewardView.claim.onClick.Invoke();Require(s.AdShowing&&s.wheelRewardView.WatchingAd&&!s.wheelRewardView.Settled,"Wheel reward enters original 2_A waiting branch");
                Require(s.Player.fakeMoney==cash&&s.Player.coin1024Number==tokens,"Wheel reward cannot settle while ad is pending");
                s.Sdk.NextAdOutcome=AdOutcome.Failed;ad.Cancel();Require(s.wheelRewardView.Settled&&!s.wheelRewardView.WatchingAd,"Wheel error/cancel callback retains original settlement behavior");
                s.Sdk.NextAdOutcome=AdOutcome.Completed;s.Player.open_music=true;
                f.dropAudio.clip=Resources.Load<AudioClip>(f.dropSoundPath);f.dropAudio.loop=true;f.dropAudio.Play();
                Require(f.dropAudio.isPlaying,"Audio test starts an actual playing source");
                task=s.Sdk.ShowRewarded("audio");Require(!f.dropAudio.isPlaying,"Ad pauses currently playing audio");ad.Cancel();
                Require(f.dropAudio.isPlaying,"Ad close resumes allowed audio");
                task=s.Sdk.ShowRewarded("muted");s.Player.open_music=false;ad.Cancel();Require(!f.dropAudio.isPlaying,"Ad close respects sound switch changed while pending");
            }
            catch(Exception ex){error=ex.ToString();result=1;}
            File.WriteAllText("interaction-parity-validation.txt",(result==0?"PASS":"FAIL")+"\n"+string.Join("\n",checks)+"\n"+error);
            Debug.Log("INTERACTION_PARITY_VALIDATION "+(result==0?"PASS":"FAIL")+" "+error);EditorApplication.ExitPlaymode();
        }
    }
}
