using System;
using UnityEngine;
namespace CoinMerge.Recovery
{
    [Serializable] public sealed class PackagedWeight {public int id,weigh;}
    [Serializable] public sealed class PackagedDropTier {public int threshold;public PackagedWeight[] entries;}
    [Serializable] public sealed class PackagedCoinDefinition
    {public int type;public string spritePath;public Vector2 size;public Vector2[] polygon;}
    public sealed class PackagedBalance : ScriptableObject
    {
        public float units=32,gravityPixels=-320,dropGravity=80,settledGravity=4,settledDrag=.5f,dropGap=.35f;
        public int poolCapacity=64,velocityIterations=2,positionIterations=2,heartSeconds=1200,failureFrames=300;
        public int modePrice=1000,heartPrice=100,mergeReward=10,maxHearts=10;
        public float fixedStep=1f/60,frameSchedule=.016f,previewY=262,deadlineY=131.188f;
        public PackagedDropTier[] firstAppear,appear;
        public PackagedCoinDefinition[] coins;
        public string[] modeNames={"beginner","simple","normal","hard","limit","hell"};
        public int NextType(PackagedPlayer player,int draw)
        {
            var tiers=player.passlevel[0]<=0?firstAppear:appear;int value=player.passlevel[0]<=0?player.mergedMaxLv:player.passlevel[0];
            var tier=tiers[0];for(int i=1;i<tiers.Length;i++)if(tiers[i].threshold<=value)tier=tiers[i];
            int total=0;foreach(var e in tier.entries)total+=e.weigh;
            int weight=1+(int)((long)Mathf.Clamp(draw,0,999999)*total/1000000);
            foreach(var e in tier.entries){if(e.weigh<=0)continue;if(weight<=e.weigh)return e.id;weight-=e.weigh;}return 1;
        }
    }
    [Serializable] public sealed class PackagedPlayer
    {
        public bool NewUser=true,open_bgm=true,open_music=true;
        public int coins,heart=10,mergedMaxLv=1,mergeCount;
        public int[] passStaus={1,0,0,0,0,0},passlevel={0,0,0,0,0,0},achieveStaus={1,0,0,0,0,0};
        public bool Spend(int amount){if(coins<amount)return false;coins-=amount;return true;}
        public bool SpendHeart(){if(heart<=0)return false;heart--;return true;}
    }
}
