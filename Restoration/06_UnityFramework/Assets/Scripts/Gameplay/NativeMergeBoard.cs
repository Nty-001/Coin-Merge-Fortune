using System;
using System.Collections.Generic;
using UnityEngine;
namespace CoinMerge.Recovery
{
    public sealed class NativeMergeBoard : MonoBehaviour
    {
        public GameBalanceConfig config;
        public NativeMergeCoin coinPrefab;
        public Transform coinContainer,ground,previewLine,deadLine;
        public Collider2D leftWall,rightWall;
        public float widthPixels=750;
        public int score=>player==null?0:player.roundScore;
        public GameBalanceConfig Config=>config;
        public float Units=>config.pixelsPerUnit;
        public bool GameOver {get;private set;}
        public bool InputBlocked {get;set;}
        public NativeMergeCoin Preview=>preview;
        public IReadOnlyList<NativeMergeCoin> Coins=>active;
        public PlayerProgress Player=>player;
        public event Action Changed,Dropped,Failed;
        public event Action<int> Merged;
        public event Action<int,Vector2,int> MergePresented;
        public event Action<NativeMergeCoin> HighestCoinCreated;
        readonly List<NativeMergeCoin> active=new List<NativeMergeCoin>(128);
        readonly Stack<NativeMergeCoin> pool=new Stack<NativeMergeCoin>(128);
        readonly Dictionary<Collider2D,NativeMergeCoin> lookup=new Dictionary<Collider2D,NativeMergeCoin>(256);
        readonly Dictionary<int,Sprite> sprites=new Dictionary<int,Sprite>(16);
        readonly Queue<MergePair> merges=new Queue<MergePair>(64);
        readonly List<NativeMergeCoin> reviveOrder=new List<NativeMergeCoin>(128);
        struct MergePair {public NativeMergeCoin first,second;}
        PlayerProgress player;
        NativeMergeCoin preview,pendingFail,pendingHighest;
        float previewWait=-1,checkWait=-1,pendingElapsed,pendingStill,failWait=-1;
        bool pendingDrop,initialized;
        public void Initialize(PlayerProgress state)
        {
            if(!initialized)
            {
                foreach(var c in config.rules.coins)
                {
                    var sprite=Resources.Load<Sprite>(c.spritePath);
                    if(sprite==null)throw new InvalidOperationException("Missing original coin sprite: "+c.spritePath);
                    sprites.Add(c.value,sprite);
                }
                for(int i=0;i<config.initialCoinPoolCapacity;i++)pool.Push(CreatePooled());
                initialized=true;
            }
            ClearBoard();player=state;GameOver=false;InputBlocked=false;
            previewWait=checkWait=failWait=-1;pendingDrop=false;pendingFail=pendingHighest=null;
            if(player.hasSavedGameScene)
            {
                foreach(var saved in player.savedCoins)
                {
                    var coin=Spawn(saved.value,transform.TransformPoint(new Vector3(saved.x/Units,saved.y/Units,0)));
                    coin.body.rotation=saved.angle;coin.transform.localScale=Vector3.one*(saved.scale==0?1:saved.scale);
                    if(coin.Position.y<ground.position.y+coin.Radius+2/Units)
                        coin.body.position=new Vector2(coin.Position.x,ground.position.y+coin.Radius+2/Units);
                }
            }
            else
            {
                var initial=config.rules.flow.initialBottomValues;
                float step=widthPixels/(initial.Length+1)/Units;
                for(int i=0;i<initial.Length;i++)
                {
                    var c=RecoveredGameRules.Coin(config.rules,initial[i]);
                    Spawn(c.value,new Vector3(transform.position.x-widthPixels*.5f/Units+step*(i+1),ground.position.y+(c.visualWidthPixels*.5f+10)/Units));
                }
                player.ResetRoundStats();NewQueue();
            }
            CreatePreview();Capture();Changed?.Invoke();
        }
        NativeMergeCoin CreatePooled()
        {
            var coin=Instantiate(coinPrefab,coinContainer);coin.Initialize(this);
            lookup.Add(coin.solid,coin);lookup.Add(coin.sensor,coin);coin.Recycle();return coin;
        }
        public Sprite SpriteFor(int value)=>sprites[value];
        public bool IsSideWall(Collider2D collider)=>collider==leftWall||collider==rightWall;
        public NativeMergeCoin Spawn(int value,Vector3 position)=>Spawn(value,position,false,false,false);
        NativeMergeCoin Spawn(int value,Vector2 position,bool isPreview,bool animate,bool merged)
        {
            var coin=pool.Count>0?pool.Pop():CreatePooled();
            coin.Configure(value,position,isPreview,animate,merged);active.Add(coin);return coin;
        }
        public void Remove(NativeMergeCoin coin)
        {
            if(!coin||!coin.IsAlive)return;
            active.Remove(coin);coin.Recycle();pool.Push(coin);
            if(preview==coin)preview=null;
        }
        void ClearBoard(){for(int i=active.Count-1;i>=0;i--)Remove(active[i]);merges.Clear();}
        int RandomDrop()
        {
            int highest=0;
            for(int i=0;i<active.Count;i++){var c=active[i];if(!c.IsPreview&&!c.IsMerging&&c.value>highest)highest=c.value;}
            return RecoveredGameRules.PickDrop(config.rules,highest,Sample());
        }
        internal static double Sample()=>Math.Min(UnityEngine.Random.value,.9999999999999999);
        void NewQueue(){player.savedCurrentCoinValue=RandomDrop();player.savedNextCoinValue=RandomDrop();player.savedPreviewX=0;}
        void CreatePreview()
        {
            if(GameOver||pendingFail||pendingHighest)return;
            if(preview)Remove(preview);
            preview=Spawn(player.savedCurrentCoinValue,new Vector2(transform.position.x+player.savedPreviewX/Units,previewLine.position.y),true,true,false);
            MovePreview(preview.Position.x);previewWait=-1;Changed?.Invoke();
        }
        public void MovePreview(float worldX)
        {
            if(!preview||GameOver||InputBlocked||pendingFail)return;
            float half=widthPixels*.5f/Units;
            worldX=Mathf.Clamp(worldX,transform.position.x-half+preview.Radius,transform.position.x+half-preview.Radius);
            preview.body.position=new Vector2(worldX,previewLine.position.y);
            preview.transform.position=new Vector3(worldX,previewLine.position.y,transform.position.z);
            player.savedPreviewX=(worldX-transform.position.x)*Units;
        }
        public bool RequestDrop()
        {
            if(GameOver||InputBlocked||pendingFail||pendingHighest||!preview)return false;
            if(preview.SpawnLocked){pendingDrop=true;return true;}
            float bottom=preview.Position.y-preview.Radius,hit=ground.position.y;
            for(int i=0;i<active.Count;i++)
            {
                var c=active[i];if(c==preview||c.IsPreview||c.IsMerging)continue;
                float r=preview.Radius+c.Radius,dx=Mathf.Abs(c.Position.x-preview.Position.x);
                if(dx>r)continue;
                float y=c.Position.y+Mathf.Sqrt(Mathf.Max(0,r*r-dx*dx))-preview.Radius;
                if(y<bottom&&y>hit)hit=y;
            }
            preview.Release(Mathf.Max(0,bottom-hit)*Units);preview=null;pendingDrop=false;
            player.savedCurrentCoinValue=player.savedNextCoinValue;player.savedNextCoinValue=RandomDrop();
            previewWait=config.rules.flow.previewDelay;checkWait=1;
            Capture();Dropped?.Invoke();Changed?.Invoke();return true;
        }
        public void TryMergeContact(NativeMergeCoin first,Collider2D collider)
        {
            if(!lookup.TryGetValue(collider,out var second))return;
            Merge(first,second);
        }
        public void Merge(NativeMergeCoin first,NativeMergeCoin second)
        {
            if(GameOver||!first.IsAlive||!second.IsAlive||first==second||first.IsPreview||second.IsPreview||first.IsMerging||second.IsMerging||first.value!=second.value)return;
            if(RecoveredGameRules.Coin(config.rules,first.value).upgrade==first.value)return;
            first.IsMerging=second.IsMerging=true;
            merges.Enqueue(new MergePair {first=first,second=second});
        }
        void CompleteMerge(MergePair pair)
        {
            var a=pair.first;var b=pair.second;
            if(!a.IsAlive||!b.IsAlive)return;
            var next=RecoveredGameRules.Coin(config.rules,RecoveredGameRules.Coin(config.rules,a.value).upgrade);
            Vector2 point=Mathf.Abs(a.Position.y-b.Position.y)*Units<=2?(a.Position+b.Position)*.5f:(a.Position.y<b.Position.y?a.Position:b.Position);
            float bottom=Mathf.Min(a.Position.y-a.HalfHeight,b.Position.y-b.HalfHeight),radius=next.visualWidthPixels*.5f/Units;
            point.y=Mathf.Max(bottom+radius+1/Units,ground.position.y+radius+2/Units)+config.rules.flow.mergeSpawnLiftPixels/Units;
            float half=widthPixels*.5f/Units;
            point.x=Mathf.Clamp(point.x,transform.position.x-half+radius,transform.position.x+half-radius);
            Remove(a);Remove(b);
            var result=Spawn(next.value,point,false,true,true);
            player.roundScore+=next.mergeScore;player.gameTotalScore+=next.mergeScore;
            player.savedCoinsScore=player.roundScore;player.savedDrawScore=player.gameTotalScore;
            if(next.upgrade==next.value)pendingHighest=result;
            MergePresented?.Invoke(next.value,point-Vector2.up*config.rules.flow.mergeSpawnLiftPixels/Units,player.gameTotalScore);
            Merged?.Invoke(next.value);Changed?.Invoke();
        }
        public void Tick(float dt)
        {
            if(player==null)return;
            if(GameOver){if(failWait>=0&&(failWait-=dt)<=0){failWait=-1;Failed?.Invoke();}return;}
            while(merges.Count>0)CompleteMerge(merges.Dequeue());
            for(int i=0;i<active.Count;i++)active[i].Tick(dt);
            if(pendingHighest&&!pendingHighest.SpawnLocked&&!pendingHighest.IsMerging)
            {
                pendingHighest.IsMerging=true;previewWait=-1;
                player.AddHighestCoinMerge(PlayerClock.Today(player),config.rules.flow.validLoginMergeCount);
                HighestCoinCreated?.Invoke(pendingHighest);Changed?.Invoke();
            }
            if(InputBlocked)return;
            if(pendingDrop&&preview&&!preview.SpawnLocked)RequestDrop();
            if(previewWait>=0&&(previewWait-=dt)<=0)CreatePreview();
            if(checkWait>=0&&(checkWait-=dt)<=0){checkWait=-1;CheckGameOver();}
            UpdatePendingFailure(dt);
        }
        public void FinishHighestCoinFlow()
        {if(pendingHighest)Remove(pendingHighest);pendingHighest=null;if(!preview)previewWait=config.rules.flow.previewDelay;Capture();Changed?.Invoke();}
        public void CheckGameOver()
        {
            if(GameOver||InputBlocked)return;
            for(int i=0;i<active.Count;i++)
            {
                var c=active[i];if(c.IsPreview||c.IsMerging||c.Position.y<deadLine.position.y)continue;
                if(c.IsStill(config.rules.flow.failStillVelocity,config.rules.flow.failStillAngularVelocity)){TriggerFailure();return;}
                if(!pendingFail){pendingFail=c;pendingElapsed=pendingStill=0;pendingDrop=false;previewWait=-1;}
            }
        }
        void UpdatePendingFailure(float dt)
        {
            if(!pendingFail)return;
            if(!pendingFail.IsAlive||pendingFail.IsMerging||pendingFail.Position.y<deadLine.position.y)
            {pendingFail=null;CheckGameOver();if(!pendingFail&&!preview)previewWait=config.rules.flow.previewDelay;return;}
            pendingElapsed+=dt;
            pendingStill=pendingFail.IsStill(config.rules.flow.failStillVelocity,config.rules.flow.failStillAngularVelocity)?pendingStill+dt:0;
            if(pendingElapsed>=config.rules.flow.failMaxWait||pendingStill>=config.rules.flow.failStillDuration)TriggerFailure();
        }
        public void TriggerFailure()
        {
            if(GameOver)return;GameOver=true;pendingFail=null;pendingDrop=false;merges.Clear();
            if(preview)Remove(preview);
            for(int i=0;i<active.Count;i++)active[i].Freeze();
            player.histroyMaxScore=Mathf.Max(player.histroyMaxScore,player.roundScore);
            failWait=config.rules.flow.failAnimationDuration;Capture();Changed?.Invoke();
        }
        public void ResetAfterFailure()
        {
            ClearBoard();player.ResetBoardAfterFailure();NewQueue();pendingFail=pendingHighest=null;
            GameOver=InputBlocked=false;previewWait=checkWait=failWait=-1;CreatePreview();Capture();Changed?.Invoke();
        }
        public int Revive()
        {
            reviveOrder.Clear();int highest=0;
            for(int i=0;i<active.Count;i++)if(!active[i].IsPreview){reviveOrder.Add(active[i]);highest=Mathf.Max(highest,active[i].value);}
            reviveOrder.Sort(TopFirst);
            int target=Mathf.Max(1,Mathf.CeilToInt(reviveOrder.Count/3f)),removed=0;
            for(int i=0;i<reviveOrder.Count&&removed<target;i++)if(reviveOrder[i].value<highest){Remove(reviveOrder[i]);removed++;}
            GameOver=InputBlocked=false;pendingFail=pendingHighest=null;failWait=checkWait=-1;
            for(int i=0;i<active.Count;i++){active[i].IsMerging=false;active[i].SetupPhysics(false);}
            if(!preview)CreatePreview();Capture();Changed?.Invoke();return removed;
        }
        static int TopFirst(NativeMergeCoin a,NativeMergeCoin b)=>(b.Position.y+b.HalfHeight).CompareTo(a.Position.y+a.HalfHeight);
        public void Capture()
        {
            if(player==null)return;
            int count=0;for(int i=0;i<active.Count;i++)if(!active[i].IsPreview&&!active[i].IsMerging)count++;
            var saved=new SavedCoin[count];int k=0;
            for(int i=0;i<active.Count;i++)
            {
                var c=active[i];if(c.IsPreview||c.IsMerging)continue;
                Vector3 local=transform.InverseTransformPoint(c.transform.position);
                saved[k++]=new SavedCoin {value=c.value,x=local.x*Units,y=local.y*Units,angle=c.body.rotation,scale=c.transform.localScale.x};
            }
            player.savedCoins=saved;player.hasSavedGameScene=true;
            player.savedCoinsScore=player.roundScore;player.savedDrawScore=player.gameTotalScore;
        }
    }
}
