using System;
using System.Collections.Generic;
using UnityEngine;
namespace CoinMerge.Recovery
{
    public sealed class PackagedBoard : MonoBehaviour
    {
        public PackagedBalance balance;public PackagedCoin prefab;public PhysicsMaterial2D freshMaterial,settledMaterial;
        public Transform coinRoot;public GameObject deadline;
        public bool GameOver {get;private set;}=true;
        public PackagedCoin Preview {get;private set;}
        public IReadOnlyList<PackagedCoin> Coins=>coins;
        public event Action<int> Merged;public event Action Failed;
        readonly List<PackagedCoin> coins=new List<PackagedCoin>(128);
        readonly Dictionary<Collider2D,PackagedCoin> lookup=new Dictionary<Collider2D,PackagedCoin>(128);
        readonly List<MergeJob> merges=new List<MergeJob>(32);
        struct MergeJob {public PackagedCoin first,second;public Vector2 position;public float remaining;public int type;}
        Sprite[] sprites;PackagedPlayer player;float previewDelay,schedule,physicsElapsed,failureDelay=-1;int frame;
        public Sprite SpriteFor(int type)=>sprites[type-1];
        public void Initialize(PackagedPlayer progress)
        {
            player=progress;
            if(sprites!=null)return;
            sprites=new Sprite[balance.coins.Length];
            for(int i=0;i<sprites.Length;i++)sprites[i]=Resources.Load<Sprite>(balance.coins[i].spritePath);
            for(int i=0;i<balance.poolCapacity;i++)CreateCoin();
        }
        PackagedCoin CreateCoin()
        {var coin=Instantiate(prefab,coinRoot);coins.Add(coin);lookup.Add(coin.polygon,coin);coin.Recycle();return coin;}
        public PackagedCoin Spawn(int type,Vector2 position,bool preview=false,bool animate=false)
        {
            PackagedCoin coin=null;for(int i=0;i<coins.Count;i++)if(!coins[i].alive){coin=coins[i];break;}
            if(!coin)coin=CreateCoin();coin.Configure(this,type,position,preview,animate);return coin;
        }
        public void StartGame()
        {
            Clear();GameOver=false;frame=0;schedule=physicsElapsed=0;failureDelay=-1;previewDelay=.01f;deadline.SetActive(false);
            Physics2D.gravity=new Vector2(0,balance.gravityPixels/balance.units);Physics2D.velocityIterations=balance.velocityIterations;
            Physics2D.positionIterations=balance.positionIterations;Physics2D.simulationMode=SimulationMode2D.Script;Physics2D.reuseCollisionCallbacks=true;
        }
        public void Clear(){merges.Clear();foreach(var coin in coins)coin.Recycle();Preview=null;GameOver=true;deadline.SetActive(false);}
        public void Move(float x)
        {if(!Preview||!Preview.alive||GameOver)return;if(Preview.Type==11)x=Mathf.Clamp(x,-125/balance.units,125/balance.units);Preview.transform.position=new Vector3(x,balance.previewY/balance.units,0);}
        public bool Drop()
        {
            if(GameOver||!Preview||!Preview.alive)return false;
            var coin=Preview;coin.preview=false;coin.body.position=coin.transform.position;coin.body.simulated=true;Preview=null;previewDelay=balance.dropGap;return true;
        }
        public void Tick(float dt)
        {
            if(failureDelay>=0){failureDelay-=dt;if(failureDelay<=0){failureDelay=-1;Failed?.Invoke();}return;}
            if(GameOver)return;
            if(!Preview&&(previewDelay-=dt)<=0)Preview=Spawn(balance.NextType(player,UnityEngine.Random.Range(0,1000000)),new Vector2(0,balance.previewY/balance.units),true);
            for(int i=merges.Count-1;i>=0;i--)
            {
                var job=merges[i];job.remaining-=dt;
                if(job.remaining>0){merges[i]=job;continue;}
                job.first.Recycle();job.second.Recycle();merges.RemoveAt(i);
                Spawn(job.type+1,job.position,false,true);Merged?.Invoke(job.type+1);
            }
            foreach(var coin in coins)if(coin.alive)coin.Tick(dt);
            physicsElapsed+=dt;while(physicsElapsed>=balance.fixedStep){Physics2D.Simulate(balance.fixedStep);physicsElapsed-=balance.fixedStep;}
            schedule+=dt;while(schedule>=balance.frameSchedule)
            {schedule-=balance.frameSchedule;frame++;if(frame%20==0)CheckFailure();if(frame%180==0)AutoMerge();if(frame>=999)frame=0;}
        }
        public void Contact(PackagedCoin coin,Collider2D other)
        {if(lookup.TryGetValue(other,out var target))TryMerge(coin,target);}
        public bool TryMerge(PackagedCoin a,PackagedCoin b)
        {
            if(GameOver||a==b||a.Type!=b.Type||!a.CanMerge||!b.CanMerge)return false;
            a.merging=b.merging=true;
            // Original type_11 branch stops here; onMergeTwoMax has no recovered caller.
            if(a.Type==11)return true;
            var lower=a.body.position.y<b.body.position.y?a:b;
            a.body.simulated=b.body.simulated=false;a.visual.enabled=b.visual.enabled=false;
            merges.Add(new MergeJob {first=a,second=b,type=a.Type,position=lower.body.position,remaining=.11f});return true;
        }
        void AutoMerge()
        {
            for(int type=1;type<=11;type++)
            {
                PackagedCoin previous=null;
                for(int i=coins.Count-1;i>=0;i--)
                {
                    var current=coins[i];if(!current.alive||current.preview||current.Type!=type)continue;
                    if(previous&&previous.CanMerge&&current.CanMerge)
                    {
                        float threshold=(type>=5&&type<=8?30:20)/balance.units;
                        // Source inner loop compares corresponding polygon vertices, not every pair.
                        var vertices=current.Definition.polygon;
                        for(int p=0;p<vertices.Length;p++)
                            if(Vector2.Distance(current.transform.TransformPoint(vertices[p]),previous.transform.TransformPoint(vertices[p]))<=threshold)
                                if(TryMerge(current,previous))return;
                    }
                    previous=current;
                }
            }
        }
        void CheckFailure()
        {
            int safe=0;
            for(int i=coins.Count-1;i>=0&&!GameOver;i--)
            {
                var coin=coins[i];if(!coin.alive||coin.preview)continue;
                bool above=coin.transform.position.y+coin.Definition.size.y*.5f/balance.units>=balance.deadlineY/balance.units;
                if(above){coin.overFrames+=20;deadline.SetActive(true);}else{coin.overFrames=0;if(++safe%3==0)deadline.SetActive(false);}
                if(coin.overFrames>balance.failureFrames)Fail();
            }
        }
        public void Fail()
        {
            if(GameOver)return;GameOver=true;merges.Clear();int total=0;
            foreach(var coin in coins)if(coin.alive){coin.Recycle();total++;}
            Preview=null;deadline.SetActive(false);failureDelay=.2f+.025f*total;
        }
    }
}
