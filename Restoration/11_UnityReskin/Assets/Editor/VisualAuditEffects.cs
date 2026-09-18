using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
namespace CoinMerge.Recovery.Editor
{
    [InitializeOnLoad] public static class VisualAuditEffects
    {
        const string Key="CoinMerge.VisualAudit.Effects",Prefix="coinmerge.visualaudit.effects.disposable";
        static RecoveredGameSession session;static RecoveredMergeFeedback effects;static RenderTexture target;
        static readonly List<string> checks=new List<string>();static double next;
        [Serializable] sealed class Report{public bool passed;public string error;public string[] checks;}
        static VisualAuditEffects(){EditorApplication.playModeStateChanged+=State;EditorApplication.update+=Poll;} static void Poll(){const string request="Temp/VisualAuditEffects.request";if(!File.Exists(request)||SessionState.GetBool(Key,false)||EditorApplication.isCompiling||EditorApplication.isUpdating)return;if(EditorApplication.isPlaying){EditorApplication.ExitPlaymode();return;}if(EditorApplication.isPlayingOrWillChangePlaymode)return;File.Delete(request);Directory.CreateDirectory("Design/VisualAudit20260917/Effects");Run();}
        public static void Run()
        {
            var store=new PlayerStore(Prefix);store.ResetPlayer();store.ResetProfile();
            store.Save(new PlayerProgress {guideStep=9999,hasSavedGameScene=true,currentLotteryCount=100,savedCurrentCoinValue=1,savedNextCoinValue=2});
            store.SaveProfile(new VersionProfile {country="US",cohort="B",rewardedVariant=true});
            EditorSceneManager.OpenScene("Assets/Scenes/RecoveredMain.unity");var game=UnityEngine.Object.FindObjectOfType<RecoveredGameSession>();game.saveNamespace=Prefix;game.automaticInput=false;
            SessionState.SetBool(Key,true);SessionState.SetString(Key+".error","");EditorApplication.EnterPlaymode();
        }
        static void State(PlayModeStateChange state)
        {
            if(!SessionState.GetBool(Key,false))return;
            if(state==PlayModeStateChange.EnteredPlayMode){checks.Clear();next=EditorApplication.timeSinceStartup+1;EditorApplication.update+=RunChecks;Application.logMessageReceived+=Log;}
            if(state==PlayModeStateChange.EnteredEditMode){var store=new PlayerStore(Prefix);store.ResetPlayer();store.ResetProfile();SessionState.SetBool(Key,false);EditorSceneManager.OpenScene("Assets/Scenes/RecoveredLoading.unity");}
        }
        static void Log(string text,string stack,LogType type){if(type==LogType.Error||type==LogType.Exception)SessionState.SetString(Key+".error",text+"\n"+stack);}
        static void Require(bool condition,string reason){if(!condition)throw new Exception(reason);checks.Add(reason);}
        static void Merge(int value=1,float x=0)
        {
            var board=session.board;foreach(var coin in board.Coins)coin.body.simulated=false;
            float r=RecoveredGameRules.Coin(board.Config.rules,value).radiusPixels/board.Units;
            Vector3 point=new Vector3(board.transform.position.x+x/board.Units,board.ground.position.y+400/board.Units,0);
            int count=effects.ActiveStarCount,score=session.Player.roundScore;
            var a=board.Spawn(value,point-Vector3.right*r*.75f);var b=board.Spawn(value,point+Vector3.right*r*.75f);a.body.gravityScale=b.body.gravityScale=0;
            Physics2D.SyncTransforms();Physics2D.Simulate(.02f);board.Tick(.25f);
            if(effects.ActiveStarCount!=count+12||session.Player.roundScore<=score)throw new Exception("Real physics contact did not produce one merge/star batch");
            foreach(var coin in board.Coins)coin.body.simulated=false;
        }
        static void Advance(float seconds){effects.Tick(seconds);}
        static void Chain(int count)
        {for(int i=0;i<count;i++){Merge(1,-100+i*25);if(i<count-1)Advance(.1f);}Advance(.51f);}
        static void RunChecks()
        {
            if(EditorApplication.timeSinceStartup<next)return;EditorApplication.update-=RunChecks;
            try
            {
                session=UnityEngine.Object.FindObjectOfType<RecoveredGameSession>();effects=session.mergeFeedback;Time.timeScale=0;
                target=new RenderTexture(750,1624,24);session.worldCamera.targetTexture=target;Canvas.ForceUpdateCanvases();session.worldCamera.Render();
                Require(effects&&session.idleGuide,"Formal scene has prefab-bound merge feedback and idle guide components");
                Require(effects.CreatedStars==48&&effects.CreatedBursts==4,"Small reusable effect pools are prewarmed");
                int before=session.Player.gameTotalScore;Merge();
                Require(effects.ActiveStarCount==12&&effects.ActiveBurstCount==1,"A real equal-coin physics collision creates 12 stars and original XX_TX burst");
                Require(effects.DisplayedScore==before&&session.Player.gameTotalScore>before,"Player score saves immediately but displayed draw score waits for star arrival");
                Advance(.5f);Require(!effects.praise.gameObject.activeSelf,"A single merge does not show a combo rating");Capture("merge_single_stars.png");
                Advance(.95f);Require(effects.DisplayedScore==session.Player.gameTotalScore&&effects.AwardedFlights==1,"First arriving star updates progress once per merge batch");
                Advance(2);Require(effects.ActiveStarCount==0&&effects.ActiveBurstCount==0,"Stars and burst return to pools after completing their animations");
                for(int i=0;i<4;i++){Merge(i==3?10:1,-110+i*60);if(i<3)Advance(.1f);}
                Advance(.499f);Require(!effects.praise.gameObject.activeSelf,"Combo stays hidden until 0.5 seconds after the final merge");
                Advance(.002f);Require(effects.LastComboCount==4&&effects.praise.sprite==Resources.Load<Sprite>(effects.config.praisePaths[2]),"Four linked merges select original Amazing artwork");
                Require(effects.praise.rectTransform.sizeDelta==effects.praise.sprite.rect.size&&effects.combo.rectTransform.sizeDelta==effects.combo.sprite.rect.size,"Praise and Combo preserve original pixel dimensions independently of Unity sprite pixels-per-unit");
                Require(!effects.times.gameObject.activeSelf,"Multiplier waits for both appearance tweens and the 0.1 second gap");
                Advance(.23f);Require(effects.times.gameObject.activeSelf&&effects.times.sprite==Resources.Load<Sprite>(effects.config.timesPaths[2]),"Original x4 artwork appears after its staggered delay");
                Capture("merge_amazing_combo.png");Advance(.77f);
                Require(!effects.combo.gameObject.activeSelf&&effects.praise.gameObject.activeSelf,"Combo shrinks first while praise begins its separate fade/grow phase");
                Advance(.51f);Require(!effects.praise.gameObject.activeSelf,"Praise disappears after its additional 0.5 second fade");Advance(2);
                foreach(int count in new[]{2,3,5,8})
                {
                    Chain(count);int index=Math.Min(count-2,3);
                    Require(effects.praise.sprite==Resources.Load<Sprite>(effects.config.praisePaths[index]),count+" merges select the correct original rating sprite");
                    Require(effects.praise.transform.localScale.x<.5f,count+"-merge praise starts its appearance tween only after the idle-chain timeout");
                    Advance(.23f);Require(effects.times.sprite==Resources.Load<Sprite>(effects.config.timesPaths[Math.Min(count,8)-2]),count+" merges select the correct multiplier sprite");
                    if(count==8)Capture("merge_unbelievable_combo.png");Advance(3);
                }
                int created=effects.CreatedStars,bursts=effects.CreatedBursts;Chain(8);Advance(3);
                Require(effects.CreatedStars==created&&effects.CreatedBursts==bursts,"A repeated eight-merge chain reuses every star and burst without new instances");
                var idle=session.idleGuide;idle.ResetIdle();idle.Tick(14.99f,false);
                Require(!idle.guide.activeSelf,"Idle hand stays hidden before 15 seconds");idle.Tick(.02f,false);
                Require(idle.guide.activeSelf,"Idle hand and arrows appear after 15 eligible idle seconds");
                var hand=idle.handAnimation;var state=hand[hand.clip.name];state.time=1.5f;hand.Sample();
                var position=((RectTransform)hand.transform).anchoredPosition;
                Require(Mathf.Abs(position.x-132.281f)<.01f&&Mathf.Abs(position.y-270)<.01f,"Native hand clip matches original 1.5-second key position");
                Require(Mathf.Abs(hand.clip.length-3.0166667f)<.001f&&hand.clip.wrapMode==WrapMode.Loop,"Original hand motion loops over 3.016667 seconds");Capture("idle_hand_15_seconds.png");
                foreach(var graphic in idle.guide.GetComponentsInChildren<UnityEngine.UI.Graphic>(true))if(graphic.raycastTarget)throw new Exception("Idle hand blocks gameplay raycasts");
                Require(true,"Hand and arrows are decorative and never consume UI raycasts");
                idle.Tick(0,true);Require(!idle.guide.activeSelf&&idle.Elapsed==0,"Touch-start immediately hides the idle hand and resets elapsed time");
                idle.Tick(15,false);session.menus.Act(1);idle.Tick(20,false);
                Require(!idle.guide.activeSelf&&idle.Elapsed==0,"Settings popup blocks idle guidance and resets its timer");session.menus.CloseAll();idle.Tick(14,false);
                Require(!idle.guide.activeSelf,"Closing a popup starts a fresh 15-second idle period");
                session.gm.Show();idle.Tick(20,false);Require(!idle.guide.activeSelf&&idle.Elapsed==0,"GM popup also blocks idle guidance");session.gm.Hide();
                session.ShowReward(3);idle.Tick(20,false);Require(!idle.guide.activeSelf&&idle.Elapsed==0,"Reward popup suppresses idle guidance");session.rewardView.Close();
                session.GmPrepareDrop(10);session.board.RequestDrop();session.board.Tick(.3f);idle.Tick(20,false);
                Require(!idle.guide.activeSelf&&idle.Elapsed==0,"A pending reward popup suppresses idle guidance before it opens");
                Merge();session.board.TriggerFailure();effects.Tick(.01f);idle.Tick(20,false);
                Require(!idle.guide.activeSelf&&effects.ActiveStarCount==0&&effects.ActiveBurstCount==0,"Game over clears active merge effects and hides the hand");
                session.ResetPlayerKeepingProfile();Require(effects.DisplayedScore==session.Player.gameTotalScore&&effects.ActiveStarCount==0,"Resetting a save clears delayed visual score and star callbacks");
                string error=SessionState.GetString(Key+".error","");if(error.Length>0)throw new Exception(error);Finish(null);
            }
            catch(Exception error){Finish(error.ToString());}
        }
        static void Capture(string name)
        {
            foreach(var player in effects.burstRoot.GetComponentsInChildren<NativeSkeletonPlayer>())player.EvaluateAt(effects.config.burstAnimation,.22f);
            Canvas.ForceUpdateCanvases();session.worldCamera.Render();var previous=RenderTexture.active;RenderTexture.active=target;
            var texture=new Texture2D(750,1624,TextureFormat.RGB24,false);texture.ReadPixels(new Rect(0,0,750,1624),0,0);texture.Apply();File.WriteAllBytes("Design/VisualAudit20260917/Effects/"+name,texture.EncodeToPNG());RenderTexture.active=previous;UnityEngine.Object.DestroyImmediate(texture);
        }
        static void Finish(string error)
        {
            Time.timeScale=1;Application.logMessageReceived-=Log;SessionState.SetString(Key+".error",error??"");
            File.WriteAllText("Design/VisualAudit20260917/Effects/merge_feedback_validation.json",JsonUtility.ToJson(new Report {passed=error==null,error=error,checks=checks.ToArray()},true));
            if(session)session.worldCamera.targetTexture=null;if(target)UnityEngine.Object.DestroyImmediate(target);
            Debug.Log(error??"MERGE_FEEDBACK_IDLE_HAND_VALIDATED "+checks.Count);EditorApplication.ExitPlaymode();
        }
    }
}
