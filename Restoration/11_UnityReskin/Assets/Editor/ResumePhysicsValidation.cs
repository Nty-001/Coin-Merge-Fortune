using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace CoinMerge.Recovery.Editor
{
    [InitializeOnLoad]
    public static class ResumePhysicsValidation
    {
        const string Key="CoinMerge.ResumeValidation",Prefix="coinmerge.resume.disposable";
        static readonly List<string> checks=new List<string>();
        static int result;
        static ResumePhysicsValidation(){EditorApplication.playModeStateChanged+=State;}
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
            if(state==PlayModeStateChange.EnteredEditMode)
            {SessionState.SetBool(Key,false);var store=new PlayerStore(Prefix);store.ResetPlayer();store.ResetProfile();EditorApplication.Exit(result);}
        }
        static void Require(bool condition,string message)
        {if(!condition)throw new Exception(message);checks.Add(message);}
        static void Check()
        {
            string error="";checks.Clear();
            try
            {
                var s=UnityEngine.Object.FindObjectOfType<RecoveredGameSession>();s.Initialize(new PlayerStore(Prefix));s.enabled=false;
                s.OnApplicationPause(false);s.OnApplicationFocus(true);s.Tick(0);
                var board=s.board;
                var coin=board.Spawn(50,new Vector3(0,board.ground.position.y+4));
                Physics2D.SyncTransforms();
                for(int i=0;i<360;i++)s.Tick(1f/60f);
                var position=coin.Position;var velocity=coin.body.velocity;float rotation=coin.body.rotation;
                bool sleeping=coin.body.IsSleeping();int count=board.Coins.Count;
                for(int cycle=0;cycle<20;cycle++)
                {
                    s.OnApplicationFocus(false);s.OnApplicationPause(true);s.Tick(30);
                    s.OnApplicationPause(false);s.Tick(30);
                    Require(coin.Position==position&&coin.body.velocity==velocity,"Pause/focus overlap preserves body state, cycle "+cycle);
                    s.OnApplicationFocus(true);s.Tick(30);
                    Require(coin.Position==position&&coin.body.rotation==rotation&&coin.body.IsSleeping()==sleeping&&board.Coins.Count==count,"Resume frame preserves position, rotation, sleep and count, cycle "+cycle);
                }
                s.OnApplicationPause(true);s.OnApplicationFocus(false);s.OnApplicationFocus(true);s.Tick(30);
                Require(coin.Position==position,"Focus-first resume stays suspended until pause ends");
                s.OnApplicationPause(false);s.Tick(30);
                Require(coin.Position==position,"Pause-first suspension also discards the resume frame");
                s.Tick(1f/60f);Require(Vector2.Distance(position,coin.Position)<.01f,"Settled coin remains stable after normal simulation resumes");
                board.Remove(coin);
                var start=new Vector3(0,board.ground.position.y+8);
                coin=board.Spawn(50,start);Physics2D.SyncTransforms();
                s.OnApplicationFocus(true);s.Tick(0);s.Tick(10);
                Vector2 stalledPosition=coin.Position,stalledVelocity=coin.body.velocity;
                board.Remove(coin);coin=board.Spawn(50,start);Physics2D.SyncTransforms();
                s.OnApplicationFocus(true);s.Tick(0);
                for(int i=0;i<board.Config.maxPhysicsStepsPerFrame;i++)s.Tick(board.Config.physicsStep);
                Require(Vector2.Distance(stalledPosition,coin.Position)<.001f&&Vector2.Distance(stalledVelocity,coin.body.velocity)<.001f,"Ten-second stall matches bounded fixed steps instead of one oversized physics step");
                var movingPosition=coin.Position;var movingVelocity=coin.body.velocity;
                s.OnApplicationPause(true);s.Tick(10);s.OnApplicationPause(false);s.Tick(10);
                Require(coin.Position==movingPosition&&coin.body.velocity==movingVelocity,"In-flight coin keeps velocity through pause/resume");
                s.Tick(board.Config.physicsStep);Require(coin.Position.y<movingPosition.y,"In-flight coin continues falling normally after resume");
                board.Remove(coin);board.Initialize(s.Player);board.InputBlocked=false;
                Require(board.RequestDrop(),"Drop can be queued during preview spawn animation");
                s.OnApplicationFocus(false);s.OnApplicationFocus(true);s.Tick(10);
                for(int i=0;i<60;i++)s.Tick(1f/60f);
                Require(board.Preview&&board.Preview.IsPreview,"Interrupted queued input does not drop a coin after resume");
                Require(board.RequestDrop()&&!board.Preview,"Fresh input still drops after resume");
                s.Player.savedCoins=Array.Empty<SavedCoin>();board.Initialize(s.Player);
                var pile=new List<NativeMergeCoin>();
                int[] values={10,50,100,200,20,5};
                float y=board.ground.position.y;
                foreach(int value in values)
                {
                    var c=board.Spawn(value,new Vector3(0,y+2));
                    c.body.position=new Vector2(0,y+c.Radius+.03f);y+=2*c.Radius+.03f;pile.Add(c);
                }
                Physics2D.SyncTransforms();for(int i=0;i<360;i++)s.Tick(1f/60f);
                var positions=new Vector2[pile.Count];var velocities=new Vector2[pile.Count];
                for(int i=0;i<pile.Count;i++){positions[i]=pile[i].Position;velocities[i]=pile[i].body.velocity;}
                s.OnApplicationPause(true);s.OnApplicationFocus(false);s.Tick(60);
                s.OnApplicationFocus(true);s.OnApplicationPause(false);s.Tick(.333333f);
                for(int i=0;i<pile.Count;i++)Require(pile[i].Position==positions[i]&&pile[i].body.velocity==velocities[i],"Stack coin retains exact body state after resume: "+values[i]);
                for(int i=0;i<30;i++)s.Tick(1f/60f);
                foreach(var c in pile)Require(c.IsAlive&&Vector2.Distance(c.Position,positions[pile.IndexOf(c)])<.1f,"Settled stack does not scatter after resume: "+c.value);
                s.Save();Require(new PlayerStore(Prefix).LoadPlayer().hasSavedGameScene,"Board remains saveable after lifecycle transitions");
            }
            catch(Exception ex){error=ex.ToString();result=1;}
            File.WriteAllText("resume-physics-validation.txt",(result==0?"PASS":"FAIL")+"\n"+string.Join("\n",checks)+"\n"+error);
            Debug.Log("RESUME_PHYSICS_VALIDATION "+(result==0?"PASS":"FAIL")+" "+error);
            EditorApplication.ExitPlaymode();
        }
    }
}
