using System;
namespace CoinMerge.Recovery
{
    /// <summary>Recovered values from HotUpdate CoinItem.js; sampling from GameScene.getRandomDropValue.</summary>
    public static class MergeRules
    {
        public static readonly int[] Values = {1,2,5,10,20,50,100,200,500,1000,2000};
        public static readonly float[] RadiusPixels = {27,32,55,57.5f,71.5f,85,103,121,131,140,192};
        public static readonly int[] MergeScores = {0,1,1,2,3,4,5,6,7,8,9};
        public static int Index(int value) { int i=Array.IndexOf(Values,value); if(i<0) throw new ArgumentException("Unknown coin value"); return i; }
        public static int Upgrade(int value) => Values[Math.Min(Index(value)+1,Values.Length-1)];
        public static int PickWeighted(int[] values, float[] weights, double unitRandom)
        {
            if(values == null || weights == null || values.Length==0 || values.Length != weights.Length) throw new ArgumentException("Invalid weighted row");
            if(unitRandom<0 || unitRandom>=1) throw new ArgumentOutOfRangeException(nameof(unitRandom));
            double total=0; foreach(float w in weights) { if(w<0) throw new ArgumentException("Negative weight"); total+=w; }
            if(total<=0) throw new ArgumentException("Zero total weight");
            double roll=unitRandom*total, cumulative=0;
            for(int i=0;i<values.Length;i++){ cumulative+=weights[i]; if(roll<cumulative)return values[i]; }
            return values[values.Length-1];
        }
    }
}
