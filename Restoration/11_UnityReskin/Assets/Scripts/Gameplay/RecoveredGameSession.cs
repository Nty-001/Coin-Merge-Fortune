using System;
using System.Globalization;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace CoinMerge.Recovery
{
    public sealed class RecoveredGameSession : MonoBehaviour
    {
        public NativeMergeBoard board;
        public RecoveredCoinFeedback coinFeedback;
        public MockAdPlaybackView adPlayback;
        public VersionGmPanel gm;
        public RecoveredMainMenus menus;
        public RecoveredNoticeTicker notice;
        public RecoveredMergeFeedback mergeFeedback;
        public RecoveredIdleGuide idleGuide;
        public RecoveredLifecycleFeedback lifecycle;
        public RecoveredRatingView rating;
        public RecoveredPlayfieldLayout playfieldLayout;
        public Camera worldCamera;
        public Text moneyText,bubbleText,progressText,remainingText,highestText;
        public Image progressFill,nextImage;
        public GameObject bubble,dropGuide;
        public RecoveredRewardView rewardView;
        public RecoveredFailView failView;
        public RecoveredGuideView guideView;
        public RecoveredWheelView wheelView;
        public RecoveredWheelRewardView wheelRewardView;
        public Button wheelButton;
        public LocalizedLabelBinding[] localizedLabels;
        public CurrencyIconBinding[] currencyIcons;
        public RecoveredLocalization Locale {get;private set;}
        public VersionProfile Profile {get;private set;}
        public PlayerProgress Player {get;private set;}
        public MockSdkFacade Sdk {get;}=new MockSdkFacade();
        public bool AdShowing {get;private set;}
        public bool CanShowIdleGuide=>initialized&&!board.GameOver&&!AdShowing&&rewardDelay<0&&wheelDelay<0&&!(gm&&gm.IsOpen)&&!(menus&&menus.IsOpen)&&!rewardView.gameObject.activeSelf&&!failView.gameObject.activeSelf&&!wheelView.gameObject.activeSelf&&!wheelRewardView.gameObject.activeSelf&&!guideView.gameObject.activeSelf&&!rating.gameObject.activeSelf;
        PlayerStore store;
        float saveElapsed,rewardDelay=-1,guideDelay=-1,wheelDelay=-1,ratingDelay=-1;
        bool firstStrong;
        int pendingReward;
        bool inputStarted,initialized;
        public bool IsBoardPointerHeld=>isActiveAndEnabled&&inputStarted&&!applicationPaused&&!applicationUnfocused&&!(gm&&gm.IsOpen);
        bool applicationPaused,applicationUnfocused,discardResumeFrame;
        public bool IsApplicationSuspended=>applicationPaused||applicationUnfocused;
        float physicsElapsed;
        double displayedMoney=double.NaN;
        public bool automaticInput=true;
        public string saveNamespace="coinmerge.recovered.v1";
        void Start(){Initialize(new PlayerStore(saveNamespace));}
        public void Initialize(PlayerStore playerStore)
        {
            if(initialized)return;
            store=playerStore;Player=store.LoadPlayer();Profile=store.LoadProfile(board.Config);
            ApplyLocale();
            Player.RecordLoginDay(PlayerClock.Today(Player),board.Config.rules.flow.validLoginMergeCount);
            board.Changed+=Refresh;board.Dropped+=OnDrop;board.Merged+=OnMerged;board.Failed+=OnFailed;board.HighestCoinCreated+=OnHighest;
            board.FailureStarted+=OnFailureStarted;
            rewardView.Closed+=OnRewardClosed;failView.ReviveRequested+=OnRevive;failView.RestartRequested+=OnRestart;guideView.Advanced+=OnGuideAdvanced;
            wheelButton.onClick.AddListener(OnWheelButton);wheelView.DrawRequested+=OnWheelDraw;wheelView.DrawCompleted+=OnWheelCompleted;wheelRewardView.ClaimRequested+=OnWheelClaim;
            Physics2D.gravity=new Vector2(0,board.Config.rules.physics.gravityPixels/board.Units);
            Physics2D.velocityIterations=board.Config.velocityIterations;Physics2D.positionIterations=board.Config.positionIterations;
            Physics2D.simulationMode=SimulationMode2D.Script;Physics2D.reuseCollisionCallbacks=true;
            if(mergeFeedback)mergeFeedback.Initialize(this);lifecycle.Initialize();
            coinFeedback.Initialize();Sdk.Playback=adPlayback;
            playfieldLayout.Refresh();board.Initialize(Player);initialized=true;guideView.Show(Player.guideStep,Player.fakeMoney);
            // Step 1 opens only after the first merge, as GameScene does.
            if(Player.guideStep==1)guideView.gameObject.SetActive(false);
            Refresh();Save();
        }
        void Update(){Tick(Time.deltaTime);}
        public void Tick(float dt)
        {
            if(!initialized)return;
            if(applicationPaused||applicationUnfocused)return;
            if(AdShowing){board.InputBlocked=true;inputStarted=false;physicsElapsed=0;return;}
            // Android may deliver both focus and pause callbacks, in either order.
            // Never feed the elapsed background time into the live physics world.
            if(discardResumeFrame){discardResumeFrame=false;physicsElapsed=0;return;}
            if(dt<=0||float.IsNaN(dt)||float.IsInfinity(dt))return;
            if(gm&&gm.IsOpen){board.InputBlocked=true;inputStarted=false;physicsElapsed=0;return;}
            float step=Mathf.Clamp(board.Config.physicsStep,.005f,.02f);
            int maxSteps=Mathf.Clamp(board.Config.maxPhysicsStepsPerFrame,1,12);
            board.InputBlocked=board.Reviving||AdShowing||rating.gameObject.activeSelf||(menus&&menus.IsOpen)||rewardView.gameObject.activeSelf||failView.gameObject.activeSelf||wheelView.gameObject.activeSelf||wheelRewardView.gameObject.activeSelf||(guideView.gameObject.activeSelf&&Player.guideStep!=0);
            if(automaticInput)ReadBoardInput();
            physicsElapsed=Mathf.Min(physicsElapsed+dt,step*maxSteps);
            for(int i=0;i<maxSteps&&physicsElapsed>=step;i++)
            {
                board.Tick(step);
                if(!board.GameOver)Physics2D.Simulate(step);
                physicsElapsed-=step;
            }
            if(ratingDelay>=0&&(ratingDelay-=dt)<=0){ratingDelay=-1;rating.Show();Player.gameRateTimes=1;Save();}
            if(guideDelay>=0&&(guideDelay-=dt)<=0){guideDelay=-1;guideView.Show(Player.guideStep,Player.fakeMoney);}
            if(rewardDelay>=0&&(rewardDelay-=dt)<=0)
            {
                if(board.InputBlocked)rewardDelay=board.Config.rules.flow.popupDelay;
                else{rewardDelay=-1;ShowDropReward();}
            }
            if(wheelDelay>=0&&(wheelDelay-=dt)<=0)
            {
                if(board.GameOver)wheelDelay=-1;
                else if(board.InputBlocked||rewardDelay>=0)wheelDelay=board.Config.rules.flow.popupDelay;
                else{wheelDelay=-1;wheelView.Show(Player,Locale);}
            }
            saveElapsed+=dt;
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
                inputStarted=worldCamera.pixelRect.Contains(point)&&!overUi&&!board.InputBlocked&&world.y>=board.ground.position.y&&world.y<=board.previewLine.position.y+2;
            }
            if(!inputStarted)return;
            if(held||up)board.MovePreview(worldCamera.ScreenToWorldPoint(point).x);
            if(up){inputStarted=false;board.RequestDrop();}
        }
        void OnDrop()
        {
            if(idleGuide)idleGuide.ResetIdle();
            if(Player.guideStep==0){Player.guideStep=1;guideView.gameObject.SetActive(false);}
            int stage=RecoveredGameRules.RewardDropStage(Player,board.Config.rules.flow);
            if(stage>0&&rewardDelay<0){pendingReward=stage;rewardDelay=board.Config.rules.flow.popupDelay;}
        }
        void OnMerged(int value)
        {
            if(Player.guideStep==1)guideDelay=.2f;
            if(value==500&&!Player.firstMergeIcon500){Player.firstMergeIcon500=true;if(Player.gameRateScore<=3)rating.Show();Save();}
        }
        async void ShowDropReward()
        {
            if(board.GameOver)return;
            if(pendingReward==2)
            {
                board.InputBlocked=true;
                var outcome=await ShowGameplayAd("1_A");
                if(!this)return;
                // HWLshowAd=false never invokes either callback; the original window stays at 22.
                if(outcome!=AdOutcome.Unavailable)Player.windowsCointimes=0;
                if(outcome==AdOutcome.Completed)ShowReward(2);else board.InputBlocked=false;
            }
            else {ShowReward(3);firstStrong=Player.dropCointimes==board.Config.rules.flow.firstRewardDrop;}
        }
        public void ShowReward(int kind)
        {
            if(board.GameOver&&kind!=1)return;firstStrong=false;
            var cash=RecoveredGameRules.CashConfiguration(board.Config.rules,Profile.country);
            double amount=kind==5?cash.guideMoney:kind==4?1:RecoveredGameRules.CalculateCash(Player.fakeMoney,cash,NativeMergeBoard.Sample())*(kind==2?2:1);
            rewardView.Show(kind,amount);board.InputBlocked=true;
        }
        void OnRewardClosed()
        {
            int kind=rewardView.Kind;
            if(board.GameOver&&kind!=1)return;
            if(kind!=4){Player.fakeMoney+=rewardView.Amount;lifecycle.FlyMoney(rewardView.Amount);}
            if(firstStrong){ratingDelay=lifecycle.config.ratingDelay;firstStrong=false;}
            if(kind==1)board.Revive();
            if(kind==4)board.FinishHighestCoinFlow();
            if(kind==5&&Player.guideStep==2){Player.guideStep=3;guideView.Show(3,Player.fakeMoney);}
            board.InputBlocked=board.Reviving;Refresh();Save();
        }
        void OnGuideAdvanced()
        {
            if(Player.guideStep==1){Player.guideStep=2;guideView.gameObject.SetActive(false);ShowReward(5);}
            else if(Player.guideStep==3){Player.guideStep=4;guideView.Show(4,Player.fakeMoney);}
            else if(Player.guideStep==4){Player.guideStep=9999;guideView.gameObject.SetActive(false);}
            Refresh();Save();
        }
        void OnHighest(NativeMergeCoin coin){lifecycle.BeginHighest(coin);}
        void OnFailureStarted(NativeMergeCoin cause){rewardDelay=guideDelay=wheelDelay=-1;rewardView.gameObject.SetActive(false);wheelView.gameObject.SetActive(false);wheelRewardView.gameObject.SetActive(false);}
        void OnFailed(){rewardDelay=guideDelay=wheelDelay=-1;rewardView.gameObject.SetActive(false);wheelView.gameObject.SetActive(false);wheelRewardView.gameObject.SetActive(false);failView.Show(Player);Save();}
        async void OnRevive()
        {
            if(AdShowing)return;
            failView.revive.interactable=false;
            var outcome=await ShowGameplayAd("3_A");
            if(!this)return;
            if(outcome==AdOutcome.Completed){failView.gameObject.SetActive(false);ShowReward(1);}
            else failView.revive.interactable=true;
        }
        void OnRestart(){board.ResetAfterFailure();lifecycle.ResetVisuals();if(mergeFeedback)mergeFeedback.ResetScore();if(idleGuide)idleGuide.ResetIdle();if(notice)notice.Restart();Save();}
        void OnWheelButton()
        {
            // Original GameScene's canLottery branch is empty; progress refresh schedules the popup.
            int required=RecoveredGameRules.RequiredScore(board.Config.rules.lotteryScores,Player.currentLotteryCount);
            if(Player.gameTotalScore<required)
            {
                string message=Locale.Label("45").Replace("%{0}",(required-Player.gameTotalScore).ToString());
                if(menus)menus.ShowToast(message);else remainingText.text=message;
            }
        }
        void OnWheelDraw()
        {
            if(board.GameOver||Player.gameTotalScore<RecoveredGameRules.RequiredScore(board.Config.rules.lotteryScores,Player.currentLotteryCount))return;
            int result=gmNextWheelIndex>=0?gmNextWheelIndex:RecoveredGameRules.PickLottery(board.Config.rules.lotteryRewards,NativeMergeBoard.Sample());
            gmNextWheelIndex=-1;wheelView.Begin(result);
        }
        void OnWheelCompleted(int index)
        {
            if(board.GameOver)return;
            RecoveredGameRules.SpendSpin(Player,board.Config.rules.lotteryScores);wheelView.gameObject.SetActive(false);
            wheelRewardView.Show(index,Player,board.Config,Locale,NativeMergeBoard.Sample());Refresh();Save();
        }
        async void OnWheelClaim()
        {
            if(wheelRewardView.Settled||wheelRewardView.WatchingAd)return;
            if(wheelRewardView.RequiresAd)
            {
                wheelRewardView.WatchingAd=true;var outcome=await ShowGameplayAd("2_A");
                if(!this)return;wheelRewardView.WatchingAd=false;
                // Both original success and error callbacks settle; a request that never starts does not.
                if(outcome==AdOutcome.Unavailable)return;
            }
            wheelRewardView.Settled=true;wheelRewardView.gameObject.SetActive(false);
            if(board.GameOver)return;
            Player.fakeMoney+=wheelRewardView.Cash;Player.coin1024Number+=wheelRewardView.Coins;
            if(wheelRewardView.Cash>0)lifecycle.FlyMoney(wheelRewardView.Cash);
            Refresh();Save();
        }
        async Task<AdOutcome> ShowGameplayAd(string placement)
        {
            if(AdShowing)return AdOutcome.Unavailable;
            AdOutcome outcome;AdShowing=true;
            inputStarted=false;board.CancelPendingDrop();physicsElapsed=0;board.InputBlocked=true;
            try{outcome=await Sdk.ShowRewarded(placement);}finally{AdShowing=false;discardResumeFrame=true;}
            if(!this)return outcome;
            // HWL.addadnum -> PlayData.add_show_video, on the successful mock callback.
            if(outcome==AdOutcome.Completed){Player.watch_video_count++;Save();}
            return outcome;
        }
        public void ChangeProfile(string country,string cohort,bool rewarded)
        {
            Profile.country=RecoveredGameRules.NormalizeCountry(country,board.Config.rules.supportedCountries);
            Profile.cohort=cohort=="A"?"A":"B";Profile.rewardedVariant=rewarded;Profile.contentMode=rewarded?2:1;Profile.cohortMode=cohort=="A"?1:2;store.SaveProfile(Profile);ApplyLocale();Refresh();
        }
        public void ResetPlayerKeepingProfile()
        {
            if(menus)menus.CloseAll();
            store.ResetPlayer();Player=store.LoadPlayer();rewardDelay=guideDelay=wheelDelay=-1;
            rewardView.gameObject.SetActive(false);failView.gameObject.SetActive(false);
            wheelView.gameObject.SetActive(false);wheelRewardView.gameObject.SetActive(false);
            board.Initialize(Player);lifecycle.ResetVisuals();rating.Close();ratingDelay=-1;guideView.Show(Player.guideStep,Player.fakeMoney);Save();Refresh();
            if(mergeFeedback)mergeFeedback.ResetScore();if(idleGuide)idleGuide.ResetIdle();
            if(menus)menus.audioCues.SetMusic(Player.open_bgm);
            if(notice)notice.Restart();
        }
        public void Save(){if(!initialized)return;board.Capture();store.Save(Player);}
        int gmNextWheelIndex=-1;
        public void GmSetNextWheel(int index){gmNextWheelIndex=Mathf.Clamp(index,0,board.Config.rules.lotteryRewards.Length-1);}
        public void GmRefresh(){Refresh();if(menus)menus.Refresh();Save();}
        public void RefreshPresentation(){Refresh();}
        public bool GmCanTrigger=>initialized&&!AdShowing&&!board.GameOver&&!(menus&&menus.IsOpen)&&!rewardView.gameObject.activeSelf&&!wheelView.gameObject.activeSelf&&!wheelRewardView.gameObject.activeSelf&&!guideView.gameObject.activeSelf&&!rating.gameObject.activeSelf;
        public void GmPrepareDrop(int count)
        {Player.guideStep=9999;guideView.gameObject.SetActive(false);Player.windowsCointimes=Math.Max(0,count-1);Player.dropCointimes=Math.Max(Player.dropCointimes,count-1);rewardDelay=-1;GmRefresh();}
        void ApplyLocale()
        {
            Locale=new RecoveredLocalization(Profile.country);displayedMoney=double.NaN;
            foreach(var binding in localizedLabels)binding.label.text=Locale.Label(binding.key);
            foreach(var binding in currencyIcons)Locale.ApplyIcon(binding.image,binding.type);
            rewardView.Locale=Locale;guideView.Locale=Locale;
            if(notice)notice.Configure(Locale,RecoveredGameRules.CashConfiguration(board.Config.rules,Profile.country).new_Fake_products);
        }
        void Refresh()
        {
            if(Player==null)return;
            if(displayedMoney!=Player.fakeMoney)
            {
                displayedMoney=Player.fakeMoney;moneyText.text=Locale.Money(Player.fakeMoney);
                var cash=RecoveredGameRules.CashConfiguration(board.Config.rules,Profile.country);
                double remaining=Math.Max(0,cash.real_products[0].withdrawAmount-Player.fakeMoney);
                bubble.SetActive(remaining>0);bubbleText.text=Locale.Label("56").Replace("%{0}",Locale.Money(remaining));
            }
            int required=RecoveredGameRules.RequiredScore(board.Config.rules.lotteryScores,Player.currentLotteryCount);
            int displayScore=mergeFeedback?mergeFeedback.ScoreForDisplay(Player.gameTotalScore):Player.gameTotalScore;
            progressText.text=displayScore+"/"+required;
            progressFill.fillAmount=Mathf.Clamp01((float)displayScore/required);
            int spins=RecoveredGameRules.AvailableSpins(board.Config.rules.lotteryScores,displayScore,Player.currentLotteryCount);
            if(spins>0&&!board.GameOver&&wheelDelay<0&&!wheelView.gameObject.activeSelf)wheelDelay=board.Config.rules.flow.popupDelay;
            remainingText.text=Locale.Label("45").Replace("%{0}",Math.Max(0,required-displayScore).ToString());
            highestText.text=lifecycle.HighestForDisplay(Player.coin1024Number).ToString();nextImage.sprite=board.SpriteFor(Player.savedNextCoinValue);
        }
        public void OnApplicationPause(bool paused)
        {
            applicationPaused=paused;ResetFrameAfterLifecycleChange();
            if(paused&&initialized){Player.windowsCointimes=0;Save();}
        }
        public void OnApplicationFocus(bool focused)
        {applicationUnfocused=!focused;ResetFrameAfterLifecycleChange();}
        void ResetFrameAfterLifecycleChange()
        {
            inputStarted=false;physicsElapsed=0;discardResumeFrame=true;
            if(board)board.CancelPendingDrop();
        }
        void OnApplicationQuit(){Save();}
        void OnDestroy()
        {
            if(!initialized)return;Save();
            board.Changed-=Refresh;board.Dropped-=OnDrop;board.Merged-=OnMerged;board.Failed-=OnFailed;board.HighestCoinCreated-=OnHighest;
            board.FailureStarted-=OnFailureStarted;
            rewardView.Closed-=OnRewardClosed;failView.ReviveRequested-=OnRevive;failView.RestartRequested-=OnRestart;guideView.Advanced-=OnGuideAdvanced;
            wheelButton.onClick.RemoveListener(OnWheelButton);wheelView.DrawRequested-=OnWheelDraw;wheelView.DrawCompleted-=OnWheelCompleted;wheelRewardView.ClaimRequested-=OnWheelClaim;
        }
    }
}
