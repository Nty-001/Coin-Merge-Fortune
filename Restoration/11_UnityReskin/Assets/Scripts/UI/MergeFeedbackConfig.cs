using UnityEngine;
namespace CoinMerge.Recovery
{
    [CreateAssetMenu(menuName="Coin Merge/Merge Feedback")]
    public sealed class MergeFeedbackConfig : ScriptableObject
    {
        public float chainDelay=.5f,comboGapY=105,appearGrow=.08f,appearSettle=.02f,timesDelay=.1f,hold=.5f,shrink=.2f,praiseFade=.5f;
        public int starCount=12,initialStarCapacity=48,initialBurstCapacity=4;
        public float starDelay=.06f,scatterRadius=56,flightDuration=1.12f,targetSpread=20,scatterDuration=.14f;
        public Vector2 durationJitter=new Vector2(-.08f,.16f),starScale=new Vector2(.5f,.68f),peakMultiplier=new Vector2(1.18f,1.38f);
        public Vector2 normalAlpha=new Vector2(125,190),brightAlpha=new Vector2(205,255),glowAlpha=new Vector2(170,225);
        public Vector2 curveSway=new Vector2(55,110),nearHeight=new Vector2(120,180),farHeight=new Vector2(80,145),spinDegrees=new Vector2(160,300);
        public float glowChance=.3f,normalChance=.7f,glowScale=1.28f,fadeStart=.72f,finalScale=.38f,glowFinalScale=.5f;
        public Color glowTint=new Color(1,246/255f,174/255f,1);
        public int[] coinValues={1,2,5,10,20,50,100,200,500,1000,2000};
        public float[] burstScales={.5f,.5f,.7f,1,1,1,1.1f,1.1f,1.1f,1.25f,1.25f};
        // Source requests idel/idel1, neither exists. The prefab's already-playing idle1
        // survives that failed request in the original engine and completes once.
        public string burstAnimation="idle1";
        public string[] praisePaths,timesPaths,mergeAudioPaths,comboAudioPaths;
        public string comboPath,starPath;
        public float BurstScale(int value){for(int i=0;i<coinValues.Length;i++)if(coinValues[i]==value)return burstScales[i];return 1;}
    }
}
