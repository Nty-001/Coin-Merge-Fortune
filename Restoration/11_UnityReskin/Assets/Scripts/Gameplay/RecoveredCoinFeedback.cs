using System.Collections.Generic;
using UnityEngine;
namespace CoinMerge.Recovery
{
    public sealed class RecoveredCoinFeedback : MonoBehaviour
    {
        public RecoveredGameSession session;
        public AudioSource dropAudio;
        public string dropSoundPath="CoinFeedback/sfx_bom";
        public NativeSkeletonPlayer collisionPrefab;
        public RectTransform effectRoot;
        public Vector2 offsetPixels=new Vector2(-.847f,-52.498f);
        public float animationSpeed=4;
        public int vibrationMilliseconds=20,vibrationAmplitude=255,initialCapacity=8;
        public float[] valueScales={.9f,.9f,1.1f,1.1f,1.3f,1.3f,2,2,2,2,2};
        public int DropSounds {get;private set;}
        public int HapticRequests {get;private set;}
        public int CollisionEffects {get;private set;}
        public int ActiveEffects=>active.Count;
        AudioClip clip;float duration;bool initialized;
        sealed class Effect{public NativeSkeletonPlayer player;public NativeMergeCoin coin;public int generation;public float elapsed,scale;}
        readonly Stack<Effect> pool=new Stack<Effect>(16);
        readonly List<Effect> active=new List<Effect>(32);
        public void Initialize()
        {
            if(initialized)return;initialized=true;
            var data=Resources.Load<NativeSkeletonData>(collisionPrefab.dataPath);
            foreach(var animation in data.animations)if(animation.name=="idle")duration=animation.duration;
            clip=Resources.Load<AudioClip>(dropSoundPath);
            for(int i=0;i<initialCapacity;i++)pool.Push(Create());
            session.board.Dropped+=OnDrop;session.board.Merged+=OnMerge;session.board.CoinContact+=OnContact;
        }
        Effect Create(){var player=Instantiate(collisionPrefab,effectRoot);player.playOnEnable=false;player.gameObject.SetActive(false);return new Effect{player=player};}
        void OnDrop(){if(session.Player.open_music&&!session.AdShowing&&clip){dropAudio.PlayOneShot(clip);DropSounds++;}}
        void Pulse(){if(session.Player.open_vibrate&&!session.AdShowing){HapticRequests++;NativeHaptics.Pulse(vibrationMilliseconds,vibrationAmplitude);}}
        void OnMerge(int value){Pulse();}
        void OnContact(NativeMergeCoin coin)
        {
            Pulse();var effect=pool.Count>0?pool.Pop():Create();
            effect.coin=coin;effect.generation=coin.Generation;effect.elapsed=0;effect.scale=1;
            var definitions=session.board.Config.rules.coins;
            for(int i=0;i<definitions.Length;i++)if(definitions[i].value==coin.value){effect.scale=valueScales[Mathf.Min(i,valueScales.Length-1)];break;}
            effect.player.gameObject.SetActive(true);effect.player.ResetPose("idle");active.Add(effect);CollisionEffects++;
            Position(effect);
        }
        void Position(Effect effect)
        {
            var coin=effect.coin;float units=session.board.Units;
            effect.player.transform.position=coin.transform.TransformPoint(new Vector3(offsetPixels.x/units,offsetPixels.y/units));
            effect.player.transform.rotation=coin.transform.rotation;
            float factor=effectRoot.InverseTransformVector(coin.transform.TransformVector(Vector3.right/units)).magnitude;
            effect.player.transform.localScale=Vector3.one*(effect.scale*factor);
        }
        void LateUpdate(){Tick(Time.deltaTime);}
        public void Tick(float dt)
        {
            if(session.IsApplicationSuspended||session.AdShowing)return;
            for(int i=active.Count-1;i>=0;i--)
            {
                var effect=active[i];effect.elapsed+=Mathf.Max(0,dt)*animationSpeed;
                if(!effect.coin.IsAlive||effect.coin.IsMerging||effect.coin.Generation!=effect.generation||session.board.GameOver||effect.elapsed>=duration)
                {effect.player.gameObject.SetActive(false);active.RemoveAt(i);pool.Push(effect);continue;}
                Position(effect);effect.player.EvaluateAt("idle",effect.elapsed);
            }
        }
        void OnDestroy(){if(initialized&&session&&session.board){session.board.Dropped-=OnDrop;session.board.Merged-=OnMerge;session.board.CoinContact-=OnContact;}}
    }
}
