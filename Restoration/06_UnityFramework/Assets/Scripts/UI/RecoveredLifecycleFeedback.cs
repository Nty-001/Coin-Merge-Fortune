using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace CoinMerge.Recovery
{
    public sealed class RecoveredLifecycleFeedback : MonoBehaviour
    {
        public RecoveredGameSession session;
        public LifecycleVisualConfig config;
        public NativeSkeletonPlayer highestEffect;
        public RectTransform cashLayer,moneyTarget,highestTarget,plus;
        public Text plusText;
        public CanvasGroup plusAlpha,highestAlpha;
        public Image moneyPrefab;
        public AudioSource sound;
        public Image warning;
        public RectTransform gameArea;
        public int HighestPhase {get;private set;}
        public int ActiveMoneyCount=>flights.Count;
        public int DisplayedHighest {get;private set;}
        public int CompletedMoneyBatches {get;private set;}
        NativeMergeCoin highestCoin;
        Vector3 coinStart,coinMid,coinEnd,effectStart,plusStart;
        float highestTime,highestPulse=-1,moneyPulse=-1,plusTime=-1,failTime=-1,warningTime=-1,warningPoll;
        AudioClip[] clips;
        sealed class Batch{public int remaining;public double amount;}
        sealed class Flight{public Image image;public Vector2 start,target,a,b;public float time,duration,delay;public int index;public Batch batch;public bool soundPlayed;}
        readonly List<Flight> flights=new List<Flight>(12);
        readonly Stack<Flight> pool=new Stack<Flight>(12);
        readonly List<NativeMergeCoin> failing=new List<NativeMergeCoin>(128);
        public void Initialize()
        {
            clips=new AudioClip[config.sounds.Length];plusStart=plus.localPosition;effectStart=highestEffect.transform.localPosition;
            DisplayedHighest=session.Player.coin1024Number;plus.gameObject.SetActive(false);highestEffect.gameObject.SetActive(false);warning.gameObject.SetActive(false);
            for(int i=0;i<6;i++)pool.Push(CreateFlight());
            session.board.FailureStarted+=BeginFailure;highestEffect.Completed+=HighestAnimationComplete;
        }
        Flight CreateFlight(){var img=Instantiate(moneyPrefab,cashLayer);img.gameObject.SetActive(false);return new Flight {image=img};}
        public void PlaySound(int index)
        {if(!session.Player.open_music||session.AdShowing)return;if(!clips[index])clips[index]=Resources.Load<AudioClip>(config.sounds[index]);if(clips[index])sound.PlayOneShot(clips[index]);}
        public void BeginHighest(NativeMergeCoin coin)
        {
            if(highestCoin&&highestCoin.IsAlive)session.board.Remove(highestCoin);
            highestEffect.gameObject.SetActive(false);
            highestCoin=coin;HighestPhase=1;highestTime=0;coinStart=coin.transform.position;
            coinEnd=highestEffect.transform.parent.TransformPoint(effectStart);coinEnd.z=coinStart.z;
            coinMid=new Vector3(Mathf.Lerp(coinStart.x,coinEnd.x,config.highestRiseFraction),Mathf.Max(coinStart.y,coinEnd.y)+config.highestLift/session.board.Units,coinStart.z);
        }
        void HighestAnimationComplete(){if(HighestPhase!=2||session.board.GameOver)return;HighestPhase=3;highestTime=0;}
        public int HighestForDisplay(int value){if(HighestPhase==0)DisplayedHighest=value;return DisplayedHighest;}
        public void FlyMoney(double amount)
        {
            var batch=new Batch {remaining=3,amount=amount};var target=(Vector2)cashLayer.InverseTransformPoint(moneyTarget.position);
            // cashLayer fills the visible canvas, so its pivot is the original screen-center origin.
            for(int i=0;i<3;i++)
            {
                var f=pool.Count>0?pool.Pop():CreateFlight();f.time=0;f.index=i;f.batch=batch;f.soundPlayed=false;
                f.start=config.moneyStarts[i];f.target=target+config.moneyTargets[i];float lift=config.moneyLifts[i],top=Mathf.Max(f.start.y,f.target.y)+lift;
                f.a=new Vector2(Mathf.Lerp(f.start.x,f.target.x,.18f),top);f.b=new Vector2(Mathf.Lerp(f.start.x,f.target.x,.72f),top-.2f*lift);
                f.duration=Mathf.Clamp(Vector2.Distance(f.start,f.target)/config.moneySpeed,config.moneyMinFlight,config.moneyMaxFlight);
                f.delay=config.moneyAppear+config.moneyGrow+config.moneyHold+i*config.moneyStagger;
                f.image.sprite=session.Locale.Icon(1);f.image.gameObject.SetActive(true);flights.Add(f);SetMoney(f,config.moneyStartScale,0,f.start);
            }
        }
        static float Out(float t)=>Mathf.Sin(Mathf.Clamp01(t)*Mathf.PI*.5f);
        static float In(float t)=>1-Mathf.Cos(Mathf.Clamp01(t)*Mathf.PI*.5f);
        static float Both(float t)=>(1-Mathf.Cos(Mathf.Clamp01(t)*Mathf.PI))*.5f;
        static Vector2 Bezier(Vector2 p,Vector2 a,Vector2 b,Vector2 end,float t){float u=1-t;return u*u*u*p+3*u*u*t*a+3*u*t*t*b+t*t*t*end;}
        static void SetMoney(Flight f,float scale,float alpha,Vector2 point){f.image.rectTransform.anchoredPosition=point;f.image.transform.localScale=Vector3.one*scale;f.image.color=new Color(1,1,1,alpha);}
        void Update(){if(session.Player!=null)Tick(Time.deltaTime);}
        public void Tick(float dt)
        {
            if(HighestPhase!=0&&!session.board.GameOver)TickHighest(dt);
            if(highestPulse>=0){highestPulse+=dt;float period=config.highestPulseUp+config.highestPulseDown;float t=highestPulse%period;highestTarget.localScale=Vector3.one*(t<config.highestPulseUp?Mathf.Lerp(1,config.highestPulseScale,Out(t/config.highestPulseUp)):Mathf.Lerp(config.highestPulseScale,1,In((t-config.highestPulseUp)/config.highestPulseDown)));if(highestPulse>=period*2){highestPulse=-1;highestTarget.localScale=Vector3.one;}}
            if(moneyPulse>=0){moneyPulse+=dt;moneyTarget.localScale=Vector3.one*(moneyPulse<config.moneyPulseUp?Mathf.Lerp(1,config.moneyPulseScale,Out(moneyPulse/config.moneyPulseUp)):Mathf.Lerp(config.moneyPulseScale,1,In((moneyPulse-config.moneyPulseUp)/config.moneyPulseDown)));if(moneyPulse>=config.moneyPulseUp+config.moneyPulseDown){moneyPulse=-1;moneyTarget.localScale=Vector3.one;}}
            if(plusTime>=0){plusTime+=dt;float t=Out(plusTime/config.plusDuration);plus.localPosition=plusStart+Vector3.up*(config.plusMove*t);plusAlpha.alpha=1-t;if(plusTime>=config.plusDuration){plusTime=-1;plus.gameObject.SetActive(false);plus.localPosition=plusStart;}}
            for(int i=flights.Count-1;i>=0;i--)
            {
                var f=flights[i];f.time+=dt;
                if(f.time<config.moneyAppear)SetMoney(f,Mathf.Lerp(config.moneyStartScale,config.moneyAppearScale,Out(f.time/config.moneyAppear)),Out(f.time/config.moneyAppear),f.start);
                else if(f.time<config.moneyAppear+config.moneyGrow)SetMoney(f,Mathf.Lerp(config.moneyAppearScale,config.moneyScale,Out((f.time-config.moneyAppear)/config.moneyGrow)),1,f.start);
                else if(f.time<f.delay)SetMoney(f,config.moneyScale,1,f.start);
                else
                {
                    if(!f.soundPlayed){f.soundPlayed=true;if(f.index==0)PlaySound(4);}
                    float t=Mathf.Clamp01((f.time-f.delay)/f.duration);SetMoney(f,Mathf.Lerp(config.moneyScale,config.moneyScale*config.moneyEndRatio,Both(t)),1,Bezier(f.start,f.a,f.b,f.target,t));
                    if(t>=1){f.image.gameObject.SetActive(false);flights.RemoveAt(i);pool.Push(f);if(--f.batch.remaining==0){CompletedMoneyBatches++;moneyPulse=0;plusTime=0;plusText.text="+"+session.Locale.Money(f.batch.amount);plus.localPosition=plusStart;plusAlpha.alpha=1;plus.gameObject.SetActive(true);}}
                }
            }
            if(failTime>=0)TickFailure(dt);
            warningPoll-=dt;if(warningPoll<=0){warningPoll=config.failureWarningInterval;RefreshWarning();}
            if(warningTime>=0){warningTime+=dt;float t=warningTime/config.warningStep;float alpha=t<1?Mathf.Lerp(0,1,t):t<2?Mathf.Lerp(1,170/255f,t-1):Mathf.Lerp(170/255f,1,t-2);var c=warning.color;c.a=alpha;warning.color=c;}
        }
        void TickHighest(float dt)
        {
            highestTime+=dt;
            if(HighestPhase==1)
            {
                if(highestTime<config.highestRiseTime){float t=Out(highestTime/config.highestRiseTime);highestCoin.transform.position=Vector3.Lerp(coinStart,coinMid,t);highestCoin.transform.localScale=Vector3.one*Mathf.Lerp(1,config.highestGrow,t);}
                else {float t=Both((highestTime-config.highestRiseTime)/config.highestGatherTime);highestCoin.transform.position=Vector3.Lerp(coinMid,coinEnd,t);highestCoin.transform.localScale=Vector3.one*Mathf.Lerp(config.highestGrow,config.highestGatherScale,t);highestCoin.visual.color=new Color(1,1,1,1-t);}
                if(highestTime>=config.highestRiseTime+config.highestGatherTime){session.board.Remove(highestCoin);highestCoin=null;HighestPhase=2;highestTime=0;highestEffect.transform.localPosition=effectStart;highestEffect.transform.localScale=Vector3.one;highestAlpha.alpha=1;highestEffect.gameObject.SetActive(true);highestEffect.Play(config.highestAnimation,false);}
            }
            else if(HighestPhase==3)
            {
                var end=(Vector2)highestEffect.transform.parent.InverseTransformPoint(highestTarget.position);float top=Mathf.Max(effectStart.y,end.y),t=Mathf.Clamp01(highestTime/config.highestFlyTime);
                highestEffect.transform.localPosition=Bezier(effectStart,new Vector2(effectStart.x,top+config.highestArc.x),new Vector2(end.x,top+config.highestArc.y),end,t);
                highestEffect.transform.localScale=Vector3.one*Mathf.Lerp(1,config.highestFinalScale,Both(t));highestAlpha.alpha=1-Both(t);
                if(t>=1){HighestPhase=0;highestEffect.gameObject.SetActive(false);highestEffect.transform.localPosition=effectStart;highestEffect.transform.localScale=Vector3.one;highestPulse=0;DisplayedHighest=session.Player.coin1024Number;session.board.FinishHighestCoinFlow();session.RefreshPresentation();session.Save();}
            }
        }
        void BeginFailure(NativeMergeCoin cause)
        {
            CancelHighest();failing.Clear();failTime=0;
            foreach(var coin in session.board.Coins)if(!coin.IsPreview){failing.Add(coin);if(coin==cause)coin.visual.color=config.failureTint;}
        }
        void TickFailure(float dt)
        {
            failTime+=dt;for(int i=0;i<failing.Count;i++)
            {
                var coin=failing[i];if(!coin||!coin.IsAlive)continue;float time=failTime-i%config.failureStaggerModulo*config.failureStagger;Vector2 scale=Vector2.one,from=Vector2.one;
                for(int s=0;s<config.failureTimes.Length&&time>0;s++){float duration=config.failureTimes[s];if(time<duration){float t=time/duration;float ease=s==0?Out(t):s==config.failureTimes.Length-1?BackOut(t):Both(t);scale=Vector2.LerpUnclamped(from,config.failureScales[s],ease);break;}time-=duration;from=scale=config.failureScales[s];}
                coin.visual.transform.localScale=new Vector3(scale.x,scale.y,1);
            }
            if(failTime>=config.FailureDuration(failing.Count)){failTime=-1;foreach(var coin in failing)if(coin&&coin.IsAlive)coin.visual.transform.localScale=Vector3.one;failing.Clear();}
        }
        static float BackOut(float t){float a=t-1;return 1+2.70158f*a*a*a+1.70158f*a*a;}
        void RefreshWarning()
        {
            float top=float.NegativeInfinity;foreach(var coin in session.board.Coins)if(!coin.IsPreview&&!coin.IsMerging){var v=coin.body.velocity*session.board.Units;if(coin.body.simulated&&(Mathf.Abs(v.x)>config.warningMovingLimit||Mathf.Abs(v.y)>config.warningMovingLimit))continue;top=Mathf.Max(top,coin.Position.y+coin.HalfHeight);}
            float ceiling=gameArea.TransformPoint(new Vector3(0,gameArea.rect.yMax)).y;float ground=session.board.ground.position.y;
            bool show=top>=ground+(ceiling-ground)*2/3;
            if(show&&!warning.gameObject.activeSelf){warning.gameObject.SetActive(true);warningTime=0;}
            else if(!show){warning.gameObject.SetActive(false);warningTime=-1;}
        }
        void CancelHighest(){HighestPhase=0;highestCoin=null;highestEffect.gameObject.SetActive(false);highestEffect.transform.localPosition=effectStart;highestEffect.transform.localScale=Vector3.one;}
        public void ResetVisuals()
        {
            CancelHighest();failing.Clear();failTime=-1;DisplayedHighest=session.Player.coin1024Number;
            warningPoll=0;warningTime=-1;warning.gameObject.SetActive(false);
            foreach(var f in flights){f.image.gameObject.SetActive(false);pool.Push(f);}flights.Clear();plusTime=moneyPulse=highestPulse=-1;plus.gameObject.SetActive(false);moneyTarget.localScale=highestTarget.localScale=Vector3.one;
        }
        void OnDestroy(){if(session&&session.board)session.board.FailureStarted-=BeginFailure;if(highestEffect)highestEffect.Completed-=HighestAnimationComplete;}
    }
}
