using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace CoinMerge.Recovery
{
    public sealed class RecoveredGameSession : MonoBehaviour
    {
        public NativeMergeBoard board;
        public Camera worldCamera;
        public Text moneyText,bubbleText,progressText,remainingText,highestText;
        public Image progressFill,nextImage;
        public GameObject bubble,dropGuide;
        public RecoveredRewardView rewardView;
        public RecoveredFailView failView;
        public RecoveredGuideView guideView;
        public VersionProfile Profile {get;private set;}
        public PlayerProgress Player {get;private set;}
        public MockSdkFacade Sdk {get;}=new MockSdkFacade();
        PlayerStore store;
        float saveElapsed,rewardDelay=-1,guideDelay=-1;
        int pendingReward;
        bool inputStarted,initialized;
        public bool automaticInput=true;
        public string saveNamespace="coinmerge.recovered.v1";
        void Start(){Initialize(new PlayerStore(saveNamespace));}
        public void Initialize(PlayerStore playerStore)
        {
            if(initialized)return;
            store=playerStore;Player=store.LoadPlayer();Profile=store.LoadProfile(board.Config);
            Player.RecordLoginDay(DateTime.Now.ToString("yyyy-MM-dd"),board.Config.rules.flow.validLoginMergeCount);
            board.Changed+=Refresh;board.Dropped+=OnDrop;board.Merged+=OnMerged;board.Failed+=OnFailed;board.HighestCoinCreated+=OnHighest;
            rewardView.Closed+=OnRewardClosed;failView.ReviveRequested+=OnRevive;failView.RestartRequested+=OnRestart;guideView.Advanced+=OnGuideAdvanced;
            Physics2D.gravity=new Vector2(0,board.Config.rules.physics.gravityPixels/board.Units);
            Physics2D.velocityIterations=board.Config.velocityIterations;Physics2D.positionIterations=board.Config.positionIterations;
            Physics2D.simulationMode=SimulationMode2D.Script;Physics2D.reuseCollisionCallbacks=true;
            board.Initialize(Player);initialized=true;guideView.Show(Player.guideStep,Player.fakeMoney);
            // Step 1 opens only after the first merge, as GameScene does.
            if(Player.guideStep==1)guideView.gameObject.SetActive(false);
            Refresh();Save();
        }
        void Update()
        {
            if(!initialized)return;
            board.InputBlocked=rewardView.gameObject.activeSelf||failView.gameObject.activeSelf||(guideView.gameObject.activeSelf&&Player.guideStep!=0);
            if(automaticInput)ReadBoardInput();
            board.Tick(Time.deltaTime);
            if(!board.GameOver)Physics2D.Simulate(Time.deltaTime);
            if(guideDelay>=0&&(guideDelay-=Time.deltaTime)<=0){guideDelay=-1;guideView.Show(Player.guideStep,Player.fakeMoney);}
            if(rewardDelay>=0&&(rewardDelay-=Time.deltaTime)<=0)
            {
                if(board.InputBlocked)rewardDelay=board.Config.rules.flow.popupDelay;
                else{rewardDelay=-1;ShowDropReward();}
            }
            saveElapsed+=Time.deltaTime;
            if(saveElapsed>=board.Config.rules.flow.saveInterval){saveElapsed=0;Save();}
        }
        void ReadBoardInput()
        {
            bool down=Input.GetMouseButtonDown(0),held=Input.GetMouseButton(0),up=Input.GetMouseButtonUp(0);
            Vector2 point=Input.mousePosition;int pointer=-1;
            if(Input.touchCount>0){Touch t=Input.GetTouch(0);point=t.position;pointer=t.fingerId;down=t.phase==TouchPhase.Began;up=t.phase==TouchPhase.Ended||t.phase==TouchPhase.Canceled;held=!up;}
            if(down)
            {
                bool overUi=EventSystem.current!=null&&(pointer<0?EventSystem.current.IsPointerOverGameObject():EventSystem.current.IsPointerOverGameObject(pointer));
                Vector3 world=worldCamera.ScreenToWorldPoint(point);
                inputStarted=!overUi&&!board.InputBlocked&&world.y>=board.ground.position.y&&world.y<=board.previewLine.position.y+2;
            }
            if(!inputStarted)return;
            if(held||up)board.MovePreview(worldCamera.ScreenToWorldPoint(point).x);
            if(up){inputStarted=false;board.RequestDrop();}
        }
        void OnDrop()
        {
            if(Player.guideStep==0){Player.guideStep=1;guideView.gameObject.SetActive(false);}
            int stage=RecoveredGameRules.RewardDropStage(Player,board.Config.rules.flow);
            if(stage>0&&rewardDelay<0){pendingReward=stage;rewardDelay=board.Config.rules.flow.popupDelay;}
        }
        void OnMerged(int value)
        {
            if(Player.guideStep==1)guideDelay=.2f;
            if(value==500&&!Player.firstMergeIcon500)Player.firstMergeIcon500=true;
        }
        async void ShowDropReward()
        {
            if(board.GameOver)return;
            if(pendingReward==2)
            {
                board.InputBlocked=true;
                var outcome=await Sdk.ShowRewarded("1_A");Player.windowsCointimes=0;
                if(outcome==AdOutcome.Completed)ShowReward(2);else board.InputBlocked=false;
            }
            else ShowReward(3);
        }
        public void ShowReward(int kind)
        {
            var cash=RecoveredGameRules.CashConfiguration(board.Config.rules,Profile.country);
            double amount=kind==5?cash.guideMoney:kind==4?1:RecoveredGameRules.CalculateCash(Player.fakeMoney,cash,NativeMergeBoard.Sample())*(kind==2?2:1);
            rewardView.Show(kind,amount);board.InputBlocked=true;
        }
        void OnRewardClosed()
        {
            int kind=rewardView.Kind;
            if(kind!=4)Player.fakeMoney+=rewardView.Amount;
            if(kind==1)board.Revive();
            if(kind==4)board.FinishHighestCoinFlow();
            if(kind==5&&Player.guideStep==2){Player.guideStep=3;guideView.Show(3,Player.fakeMoney);}
            board.InputBlocked=false;Refresh();Save();
        }
        void OnGuideAdvanced()
        {
            if(Player.guideStep==1){Player.guideStep=2;guideView.gameObject.SetActive(false);ShowReward(5);}
            else if(Player.guideStep==3){Player.guideStep=4;guideView.Show(4,Player.fakeMoney);}
            else if(Player.guideStep==4){Player.guideStep=9999;guideView.gameObject.SetActive(false);}
            Refresh();Save();
        }
        void OnHighest(NativeMergeCoin coin){ShowReward(4);}
        void OnFailed(){rewardDelay=guideDelay=-1;failView.Show(Player);Save();}
        async void OnRevive()
        {
            failView.revive.interactable=false;
            var outcome=await Sdk.ShowRewarded("3_A");
            if(outcome==AdOutcome.Completed){failView.gameObject.SetActive(false);ShowReward(1);}
            else failView.revive.interactable=true;
        }
        void OnRestart(){board.ResetAfterFailure();Save();}
        public void ChangeProfile(string country,string cohort,bool rewarded)
        {
            Profile.country=RecoveredGameRules.NormalizeCountry(country,board.Config.rules.supportedCountries);
            Profile.cohort=cohort=="A"?"A":"B";Profile.rewardedVariant=rewarded;store.SaveProfile(Profile);Refresh();
        }
        public void ResetPlayerKeepingProfile()
        {
            store.ResetPlayer();Player=store.LoadPlayer();rewardDelay=guideDelay=-1;
            rewardView.gameObject.SetActive(false);failView.gameObject.SetActive(false);
            board.Initialize(Player);guideView.Show(Player.guideStep,Player.fakeMoney);Save();Refresh();
        }
        public void Save(){if(!initialized)return;board.Capture();store.Save(Player);}
        void Refresh()
        {
            if(Player==null)return;
            moneyText.text="$"+Player.fakeMoney.ToString("F2",CultureInfo.InvariantCulture);
            var cash=RecoveredGameRules.CashConfiguration(board.Config.rules,Profile.country);
            double remaining=Math.Max(0,cash.real_products[0].withdrawAmount-Player.fakeMoney);
            bubble.SetActive(remaining>0);bubbleText.text="Still $"+remaining.ToString("F2",CultureInfo.InvariantCulture)+"\nremaining to withdraw";
            int required=RecoveredGameRules.RequiredScore(board.Config.rules.lotteryScores,Player.currentLotteryCount);
            progressText.text=Player.gameTotalScore+"/"+required;
            progressFill.fillAmount=Mathf.Clamp01((float)Player.gameTotalScore/required);
            int spins=RecoveredGameRules.AvailableSpins(board.Config.rules.lotteryScores,Player.gameTotalScore,Player.currentLotteryCount);
            remainingText.text=spins>0?spins+" spins available":Math.Max(0,required-Player.gameTotalScore)+" more pts to spin";
            highestText.text=Player.coin1024Number.ToString();nextImage.sprite=board.SpriteFor(Player.savedNextCoinValue);
            dropGuide.SetActive(Player.guideStep==0&&!guideView.gameObject.activeSelf);
        }
        void OnApplicationPause(bool paused){if(paused&&initialized){Player.windowsCointimes=0;Save();}}
        void OnApplicationQuit(){Save();}
        void OnDestroy()
        {
            if(!initialized)return;Save();
            board.Changed-=Refresh;board.Dropped-=OnDrop;board.Merged-=OnMerged;board.Failed-=OnFailed;board.HighestCoinCreated-=OnHighest;
            rewardView.Closed-=OnRewardClosed;failView.ReviveRequested-=OnRevive;failView.RestartRequested-=OnRestart;guideView.Advanced-=OnGuideAdvanced;
        }
    }
}
