using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace CoinMerge.Recovery
{
    public sealed class RecoveredMergeFeedback : MonoBehaviour
    {
        public MergeFeedbackConfig config;
        public RectTransform effectsRoot;
        public RectTransform burstRoot;
        public Image praise,combo,times;
        public MergeStarImage starPrefab;
        public NativeSkeletonPlayer burstPrefab;
        public AudioSource sound;
        public RecoveredGameSession session;
        readonly Stack<Star> spareStars=new Stack<Star>(64);
        readonly List<Star> stars=new List<Star>(128);
        readonly Stack<Batch> spareBatches=new Stack<Batch>(16);
        readonly Stack<NativeSkeletonPlayer> spareBursts=new Stack<NativeSkeletonPlayer>(8);
        readonly List<NativeSkeletonPlayer> bursts=new List<NativeSkeletonPlayer>(16);
        readonly List<float> burstRemaining=new List<float>(16);
        Sprite[] praiseSprites,timesSprites;Sprite starSprite;
        AudioClip[] mergeSounds,comboSounds;
        float chainRemaining=-1,comboElapsed=-1,timesOffset,starBaseScale=1,burstDuration;
        int chain,displayedScore,latestScore;Vector2 lastPoint;
        bool initialized,wasOver;
        public int LastComboCount {get;private set;}
        public int ActiveStarCount=>stars.Count;
        public int ActiveBurstCount=>bursts.Count;
        public int DisplayedScore=>displayedScore;
        public int CreatedStars {get;private set;}
        public int CreatedBursts {get;private set;}
        public int AwardedFlights {get;private set;}
        sealed class Batch{public int score,remaining;public bool applied;}
        sealed class Star
        {
            public MergeStarImage image;public RectTransform rect;public Batch batch;
            public Vector2 origin,scatter,c1,c2,target;public float age,delay,duration,startScale,peak,endScale,alpha,angle,spin;public Color tint;
        }
        public void Initialize(RecoveredGameSession owner)
        {
            if(initialized)return;session=owner;displayedScore=latestScore=owner.Player.gameTotalScore;
            praiseSprites=Sprites(config.praisePaths);timesSprites=Sprites(config.timesPaths);starSprite=Resources.Load<Sprite>(config.starPath);
            combo.sprite=Resources.Load<Sprite>(config.comboPath);combo.rectTransform.sizeDelta=combo.sprite.rect.size;
            mergeSounds=Sounds(config.mergeAudioPaths);comboSounds=Sounds(config.comboAudioPaths);
            timesOffset=times.rectTransform.anchoredPosition.x-combo.rectTransform.anchoredPosition.x;
            starBaseScale=starPrefab.transform.localScale.x;
            var data=Resources.Load<NativeSkeletonData>(burstPrefab.dataPath);
            foreach(var animation in data.animations)if(animation.name==config.burstAnimation)burstDuration=animation.duration;
            if(burstDuration<=0||!starSprite)throw new InvalidOperationException("Missing original merge effect resources");
            for(int i=0;i<config.initialStarCapacity;i++)spareStars.Push(CreateStar());
            for(int i=0;i<config.initialBurstCapacity;i++){spareBursts.Push(CreateBurst());spareBatches.Push(new Batch());}
            HideCombo();owner.board.MergePresented+=OnMerge;initialized=true;
        }
        static Sprite[] Sprites(string[] paths){var result=new Sprite[paths.Length];for(int i=0;i<paths.Length;i++){result[i]=Resources.Load<Sprite>(paths[i]);if(!result[i])throw new InvalidOperationException("Missing merge sprite "+paths[i]);}return result;}
        static AudioClip[] Sounds(string[] paths){var result=new AudioClip[paths.Length];for(int i=0;i<paths.Length;i++){result[i]=Resources.Load<AudioClip>(paths[i]);if(!result[i])throw new InvalidOperationException("Missing merge audio "+paths[i]);}return result;}
        Star CreateStar(){var image=Instantiate(starPrefab,effectsRoot);image.sprite=starSprite;image.gameObject.SetActive(false);CreatedStars++;return new Star {image=image,rect=image.rectTransform};}
        NativeSkeletonPlayer CreateBurst(){var player=Instantiate(burstPrefab,burstRoot);player.playOnEnable=false;player.Initialize();player.gameObject.SetActive(false);CreatedBursts++;return player;}
        void PlaySound(AudioClip clip){if(session.Player.open_music&&!session.AdShowing)sound.PlayOneShot(clip);}
        void OnMerge(int value,Vector2 worldPoint,int score)
        {
            if(session.board.GameOver)return;
            chain++;chainRemaining=config.chainDelay;latestScore=score;
            PlaySound(mergeSounds[Mathf.Min(chain-1,mergeSounds.Length-1)]);
            lastPoint=worldPoint;
            var burst=spareBursts.Count>0?spareBursts.Pop():CreateBurst();
            burst.transform.position=worldPoint;burst.transform.localScale=Vector3.one*config.BurstScale(value);
            burst.gameObject.SetActive(true);burst.Play(config.burstAnimation,false);bursts.Add(burst);burstRemaining.Add(burstDuration);
            StartStars(worldPoint,score);
        }
        static float Range(Vector2 range)=>UnityEngine.Random.Range(range.x,range.y);
        void StartStars(Vector2 worldPoint,int score)
        {
            var batch=spareBatches.Count>0?spareBatches.Pop():new Batch();batch.score=score;batch.remaining=config.starCount;batch.applied=false;
            Vector2 origin=effectsRoot.InverseTransformPoint(worldPoint);
            var fill=session.progressFill.rectTransform;var rect=fill.rect;
            Vector2 target=effectsRoot.InverseTransformPoint(fill.TransformPoint(new Vector3(rect.xMin+rect.width*session.progressFill.fillAmount,0,0)));
            for(int i=0;i<config.starCount;i++)
            {
                var star=spareStars.Count>0?spareStars.Pop():CreateStar();star.batch=batch;star.origin=origin;
                float angle=UnityEngine.Random.value*Mathf.PI*2,radius=UnityEngine.Random.Range(10,config.scatterRadius);
                star.scatter=origin+new Vector2(Mathf.Cos(angle),Mathf.Sin(angle))*radius;
                star.target=target+new Vector2(UnityEngine.Random.Range(-config.targetSpread,config.targetSpread),UnityEngine.Random.Range(-config.targetSpread*.5f,config.targetSpread*.5f));
                star.duration=config.flightDuration+Range(config.durationJitter);star.startScale=starBaseScale*Range(config.starScale);star.peak=star.startScale*Range(config.peakMultiplier);
                float style=UnityEngine.Random.value;bool glow=style<config.glowChance;
                star.tint=glow?config.glowTint:Color.white;star.alpha=Range(glow?config.glowAlpha:style<config.normalChance?config.normalAlpha:config.brightAlpha)/255;
                star.image.additive=glow;star.endScale=starBaseScale*(glow?config.glowFinalScale:config.finalScale);if(glow)star.peak*=config.glowScale;
                float sway=(i%2==0?1:-1)*Range(config.curveSway),height=Range(Vector2.Distance(star.scatter,star.target)<260?config.nearHeight:config.farHeight),top=Mathf.Max(star.scatter.y,star.target.y)+height;
                star.c1=new Vector2(Mathf.Lerp(star.scatter.x,star.target.x,.22f)+sway,top);
                star.c2=new Vector2(Mathf.Lerp(star.scatter.x,star.target.x,.72f)-.35f*sway,top-.18f*height);
                star.age=0;star.delay=i*config.starDelay;star.angle=UnityEngine.Random.Range(0,360f);star.spin=Range(config.spinDegrees);
                star.image.gameObject.SetActive(true);Apply(star,origin,star.startScale,0,star.angle);stars.Add(star);
            }
        }
        static void Apply(Star star,Vector2 position,float scale,float alpha,float angle)
        {star.rect.anchoredPosition=position;star.rect.localScale=new Vector3(scale,scale,1);star.rect.localRotation=Quaternion.Euler(0,0,angle);var color=star.tint;color.a=alpha;star.image.color=color;}
        void Update(){Tick(Time.deltaTime);}
        public void Tick(float dt)
        {
            if(!initialized)return;
            if(session.board.GameOver){if(!wasOver){Clear();session.RefreshPresentation();}wasOver=true;return;}wasOver=false;
            float comboStep=dt;
            if(chainRemaining>=0&&(chainRemaining-=dt)<=0){if(chain>=2)comboStep=-chainRemaining;ShowCombo(chain);chainRemaining=-1;chain=0;}
            if(comboElapsed>=0){comboElapsed+=comboStep;AnimateCombo();}
            for(int i=bursts.Count-1;i>=0;i--){burstRemaining[i]-=dt;if(burstRemaining[i]<=0){var burst=bursts[i];burst.gameObject.SetActive(false);spareBursts.Push(burst);bursts.RemoveAt(i);burstRemaining.RemoveAt(i);}}
            for(int i=stars.Count-1;i>=0;i--)
            {
                var star=stars[i];star.age+=dt;float age=star.age-star.delay;if(age<0)continue;
                if(age<config.scatterDuration){float t=Mathf.Sin(age/config.scatterDuration*Mathf.PI*.5f);Apply(star,Vector2.LerpUnclamped(star.origin,star.scatter,t),Mathf.LerpUnclamped(star.startScale,star.peak,t),star.alpha*t,star.angle);continue;}
                float fly=age-config.scatterDuration,timer=Mathf.Clamp01(fly/star.duration),u=1-timer;
                Vector2 point=u*u*u*star.scatter+3*u*u*timer*star.c1+3*u*timer*timer*star.c2+timer*timer*timer*star.target;
                float fade=timer<config.fadeStart?0:1-Mathf.Cos((timer-config.fadeStart)/(1-config.fadeStart)*Mathf.PI*.5f);
                Apply(star,point,Mathf.LerpUnclamped(star.peak,star.endScale,fade),star.alpha*(1-fade),star.angle+star.spin*(.5f-.5f*Mathf.Cos(timer*Mathf.PI)));
                if(fly<star.duration)continue;
                if(!star.batch.applied){star.batch.applied=true;displayedScore=Mathf.Max(displayedScore,star.batch.score);AwardedFlights++;session.RefreshPresentation();}
                ReleaseStar(i);
            }
        }
        void ReleaseStar(int index){var star=stars[index];star.image.gameObject.SetActive(false);if(--star.batch.remaining==0)spareBatches.Push(star.batch);star.batch=null;spareStars.Push(star);stars.RemoveAt(index);}
        public int ScoreForDisplay(int actual)
        {
            if(actual!=latestScore){ClearFlights();displayedScore=latestScore=actual;}
            return displayedScore;
        }
        void ShowCombo(int count)
        {
            if(count<2)return;LastComboCount=count;
            praise.sprite=praiseSprites[Mathf.Min(count-2,3)];praise.rectTransform.sizeDelta=praise.sprite.rect.size;
            times.sprite=timesSprites[Mathf.Min(count,8)-2];times.rectTransform.sizeDelta=times.sprite.rect.size;
            PlaySound(comboSounds[Mathf.Min(count-2,3)]);
            var parent=(RectTransform)combo.transform.parent;Vector2 point=parent.InverseTransformPoint(lastPoint);
            float half=Mathf.Max(combo.rectTransform.rect.width,times.rectTransform.rect.width)*.5f;
            point.x=Mathf.Clamp(point.x-timesOffset*.5f,-parent.rect.width*parent.pivot.x+half,parent.rect.width*(1-parent.pivot.x)-half-timesOffset);
            point.y=Mathf.Clamp(point.y+config.comboGapY,-parent.rect.height*parent.pivot.y+config.comboGapY*.5f,parent.rect.height*(1-parent.pivot.y)-config.comboGapY*.5f);
            combo.rectTransform.anchoredPosition=point;times.rectTransform.anchoredPosition=point+Vector2.right*timesOffset;
            comboElapsed=0;AnimateCombo();
        }
        float Appear(float time){return time<config.appearGrow?Mathf.Lerp(.2f,1.12f,time/config.appearGrow):Mathf.Lerp(1.12f,1,(time-config.appearGrow)/config.appearSettle);}
        static void SetImage(Image image,bool active,float scale,float alpha){image.gameObject.SetActive(active);image.rectTransform.localScale=new Vector3(scale,scale,1);var color=image.color;color.a=alpha;image.color=color;}
        void AnimateCombo()
        {
            float appear=config.appearGrow+config.appearSettle,timesStart=appear+config.timesDelay,holdEnd=timesStart+appear+config.hold,hideEnd=holdEnd+config.shrink;
            float hide=1-Mathf.Cos(Mathf.Clamp01((comboElapsed-holdEnd)/config.shrink)*Mathf.PI*.5f);
            SetImage(combo,comboElapsed<hideEnd,comboElapsed<appear?Appear(comboElapsed):Mathf.Lerp(1,.2f,hide),1-hide);
            SetImage(times,comboElapsed>=timesStart&&comboElapsed<hideEnd,comboElapsed<timesStart+appear?Appear(Mathf.Max(0,comboElapsed-timesStart)):Mathf.Lerp(1,.2f,hide),1-hide);
            float fade=Mathf.Sin(Mathf.Clamp01((comboElapsed-hideEnd)/config.praiseFade)*Mathf.PI*.5f);
            SetImage(praise,comboElapsed<hideEnd+config.praiseFade,comboElapsed<appear?Appear(comboElapsed):1+fade,1-fade);
            if(comboElapsed>=hideEnd+config.praiseFade)comboElapsed=-1;
        }
        void HideCombo(){comboElapsed=-1;SetImage(praise,false,1,1);SetImage(combo,false,1,1);SetImage(times,false,1,1);}
        void ClearFlights(){for(int i=stars.Count-1;i>=0;i--)ReleaseStar(i);}
        public void Clear()
        {
            chainRemaining=-1;chain=0;HideCombo();ClearFlights();
            for(int i=bursts.Count-1;i>=0;i--){bursts[i].gameObject.SetActive(false);spareBursts.Push(bursts[i]);}bursts.Clear();burstRemaining.Clear();
            if(session&&session.Player!=null)displayedScore=latestScore=session.Player.gameTotalScore;
        }
        public void ResetScore(){Clear();displayedScore=latestScore=session.Player.gameTotalScore;}
        void OnDestroy(){if(initialized&&session&&session.board)session.board.MergePresented-=OnMerge;}
    }
}
