# HotUpdate 主玩法函数体索引

每个模块文件含完整原始编译后的 JavaScript 函数体；依赖映射位于文件头。保留的变量名可能经过编译压缩。并非 TypeScript 原始工程。

|模块|完整函数体|字节数|依赖|
|---|---|---:|---|
|AccountCheckManager|[AccountCheckManager](Modules/AccountCheckManager.js)|1849||
|AccountDetailsView|[AccountDetailsView](Modules/AccountDetailsView.js)|5756|BaseUI, GameLocalData, NewGamePlayData, TouchButton, UIManagerNew, GameManagement, Lab|
|AdaptNotchScreen|[AdaptNotchScreen](Modules/AdaptNotchScreen.js)|1769||
|AudioManager|[AudioManager](Modules/AudioManager.js)|790||
|BaoXiangFly|[BaoXiangFly](Modules/BaoXiangFly.js)|2293|FlyAnimation, GameLocalData, NewGamePlayData, TouchButton, HWL_TStool, GameManagement|
|BaoXiangOkSpin|[BaoXiangOkSpin](Modules/BaoXiangOkSpin.js)|2274|TouchButton, NativeCall|
|BaseRecord|[BaseRecord](Modules/BaseRecord.js)|412||
|BaseScene|[BaseScene](Modules/BaseScene.js)|389||
|BaseTodayDataRecord|[BaseTodayDataRecord](Modules/BaseTodayDataRecord.js)|5349|DateUtils, GameLocalData, NewGamePlayData|
|BaseUI|[BaseUI](Modules/BaseUI.js)|4477|EventManager, LinkScript, UIConfig, UIManagerNew|
|CashTimeDialog|[CashTimeDialog](Modules/CashTimeDialog.js)|3938|BaseUI, FlyAnimation, GameLocalData, NewGamePlayData, SoundManager, TouchButton, HWL_TStool, GameManagement, Lab|
|ChinaDialog|[ChinaDialog](Modules/ChinaDialog.js)|1516|BaseUI, TouchButton, Lab|
|CircleProgress|[CircleProgress](Modules/CircleProgress.js)|1679||
|CoinItem|[CoinItem](Modules/CoinItem.js)|13864|GameLocalData, PlayData, ServerConfig|
|CollectDialog|[CollectDialog](Modules/CollectDialog.js)|4552|BaseUI, FlyAnimation, GameLocalData, NewGamePlayData, SoundManager, TouchButton, UIConfig, UIManagerNew, GameManagement, Lab, CollectSuccessDialog|
|CollectSuccessDialog|[CollectSuccessDialog](Modules/CollectSuccessDialog.js)|2892|BaseUI, FlyAnimation, GameLocalData, NewGamePlayData, TouchButton, UIConfig, UIManagerNew, HWL_TStool, GameManagement, Lab|
|CommonInterface|[CommonInterface](Modules/CommonInterface.js)|166||
|CongratulationsViewNew|[CongratulationsViewNew](Modules/CongratulationsViewNew.js)|115||
|Controller|[Controller](Modules/Controller.js)|246||
|DateUtils|[DateUtils](Modules/DateUtils.js)|2207||
|DebugComponent|[DebugComponent](Modules/DebugComponent.js)|1981|GameLocalData, NewGamePlayData, TouchButton, UIConfig, UIManagerNew|
|DebugDialog|[DebugDialog](Modules/DebugDialog.js)|7153|BaseUI, GameLocalData, PlayData, TouchButton, GameScene, GameManagement|
|EventCenter|[EventCenter](Modules/EventCenter.js)|1627|Singleton, MKNotice|
|EventManager|[EventManager](Modules/EventManager.js)|1090||
|FailDialog|[FailDialog](Modules/FailDialog.js)|4073|BaseUI, FlyAnimation, GameLocalData, PlayData, SoundManager, TouchButton, HWL_TStool, Lab, RewardDialog|
|FlyAnimation|[FlyAnimation](Modules/FlyAnimation.js)|6339|NativeCall, UI_TopRewardLayer, EventCenter, GameEventConsts, GameLocalData, NewGamePlayData, SoundManager, UIConfig, UIManagerNew|
|FontView|[FontView](Modules/FontView.js)|1357|GameManagement|
|GameClickManager|[GameClickManager](Modules/GameClickManager.js)|1772|ResManager, Singleton, GameClickSpine|
|GameClickSpine|[GameClickSpine](Modules/GameClickSpine.js)|1076|UIUtils, GameClickManager|
|GameEventConsts|[GameEventConsts](Modules/GameEventConsts.js)|792||
|GameFakeWDDialog|[GameFakeWDDialog](Modules/GameFakeWDDialog.js)|5912|BaseUI, GameLocalData, PlayData, TouchButton, UIManagerNew, GameManagement, Lab, GameRealWDActiveTips, GameFakeWDItem|
|GameFakeWDItem|[GameFakeWDItem](Modules/GameFakeWDItem.js)|3113|GameLocalData, PlayData, GameManagement, Lab|
|GameLocalData|[GameLocalData](Modules/GameLocalData.js)|2949|PlayData, Singleton, UserDefaultManager|
|GameManagement|[GameManagement](Modules/GameManagement.js)|19400|FlyAnimation, GameLocalData, NewGamePlayData, PlayData, ServerConfig|
|GameObjectFlyTo|[GameObjectFlyTo](Modules/GameObjectFlyTo.js)|1013|GameManagement|
|GameRealTXYZ|[GameRealTXYZ](Modules/GameRealTXYZ.js)|9521|BaseUI, GameLocalData, NewGamePlayData, PlayData, TouchButton, UIConfig, UIManagerNew, GameManagement, Lab, UI_TopRewardLayer, GameRealWDActiveTips, GameRealWDDialog|
|GameRealWDADIDTips|[GameRealWDADIDTips](Modules/GameRealWDADIDTips.js)|1790|BaseUI, TouchButton, HWL_TStool, Lab, NativeCall|
|GameRealWDAccountBR|[GameRealWDAccountBR](Modules/GameRealWDAccountBR.js)|7814|BaseUI, GameLocalData, PlayData, TouchButton, UIConfig, UIManagerNew, ServerConfig, GameManagement, Lab, AccountCheckManager, GameRealWDDialog|
|GameRealWDAccountID|[GameRealWDAccountID](Modules/GameRealWDAccountID.js)|7167|BaseUI, GameLocalData, PlayData, TouchButton, UIConfig, UIManagerNew, ServerConfig, GameManagement, Lab, AccountCheckManager, GameRealWDDialog|
|GameRealWDAccount|[GameRealWDAccount](Modules/GameRealWDAccount.js)|3398|BaseUI, GameLocalData, PlayData, TouchButton, UIConfig, UIManagerNew, Lab, AccountCheckManager|
|GameRealWDActiveTips|[GameRealWDActiveTips](Modules/GameRealWDActiveTips.js)|1922|BaseUI, TouchButton, Lab|
|GameRealWDDialog|[GameRealWDDialog](Modules/GameRealWDDialog.js)|9273|BaseUI, GameLocalData, PlayData, SoundManager, TouchButton, UIConfig, UIManagerNew, GameManagement, Lab, NativeCall, GameWithdrawItem|
|GameRealWDEmailOccupy|[GameRealWDEmailOccupy](Modules/GameRealWDEmailOccupy.js)|1656|BaseUI, TouchButton, Lab|
|GameRealWDHelp|[GameRealWDHelp](Modules/GameRealWDHelp.js)|1272|BaseUI, TouchButton, Lab|
|GameRealWDItem|[GameRealWDItem](Modules/GameRealWDItem.js)|1851|GameLocalData, NewGamePlayData, GameManagement, Lab|
|GameRealWDRecordItem|[GameRealWDRecordItem](Modules/GameRealWDRecordItem.js)|4503|TouchButton, GameManagement, Lab|
|GameRealWDRecord|[GameRealWDRecord](Modules/GameRealWDRecord.js)|5188|BaseUI, TouchButton, ServerConfig, Lab, GameRealWDRecordItem|
|GameRealWDTXSuccess|[GameRealWDTXSuccess](Modules/GameRealWDTXSuccess.js)|1823|BaseUI, TouchButton, GameManagement, Lab|
|GameRealWDTXTips|[GameRealWDTXTips](Modules/GameRealWDTXTips.js)|3977|BaseUI, GameLocalData, PlayData, TouchButton, GameManagement, Lab|
|GameScene|[GameScene](Modules/GameScene.js)|73952|GameLocalData, PlayData, SoundManager, TouchButton, UIManagerNew, CoinItem, HWL_TStool, ServerConfig, DebugDialog, GameManagement, Lab, GameRealWDDialog, GameUtils, FailDialog, GameFakeWDDialog, GuideDialog, HtmlDialog, LuckDrawDialog, RewardDialog, ScoreDialog, SettingDialog, mergeRuleDialog|
|GameUtils|[GameUtils](Modules/GameUtils.js)|7339|GameLocalData, PlayData, Singleton, GameManagement|
|GameWDValidate1|[GameWDValidate1](Modules/GameWDValidate1.js)|2233|BaseUI, TouchButton, UIConfig, UIManagerNew, Lab|
|GameWDValidate2|[GameWDValidate2](Modules/GameWDValidate2.js)|4561|BaseUI, GameLocalData, NewGamePlayData, TouchButton, UIManagerNew, GameManagement, Lab, GameRealWDDialog|
|GameWithdrawDialog|[GameWithdrawDialog](Modules/GameWithdrawDialog.js)|9223|BaseUI, GameLocalData, NewGamePlayData, ResManager, TouchButton, UIConfig, UIManagerNew, GameManagement, Lab, GameWithdrawItem|
|GameWithdrawItem|[GameWithdrawItem](Modules/GameWithdrawItem.js)|1458|GameManagement|
|GuideDialog|[GuideDialog](Modules/GuideDialog.js)|9101|BaseUI, GameLocalData, PlayData, TouchButton, GameScene, GameManagement, Lab, RewardDialog|
|GuidePropsDialog|[GuidePropsDialog](Modules/GuidePropsDialog.js)|2020|BaseUI, TouchButton, Lab|
|HWL_TStool|[HWL_TStool](Modules/HWL_TStool.js)|14003|GameLocalData, PlayData, SoundManager, UIManagerNew, Lab, ServerConfig, 12|
|HtmlBtn|[HtmlBtn](Modules/HtmlBtn.js)|3173|TouchButton, UIConfig, UIManagerNew, ServerConfig|
|HtmlDialog|[HtmlDialog](Modules/HtmlDialog.js)|3264|BaseUI, TouchButton, UIManagerNew, ServerConfig, Lab, HtmlItem|
|HtmlItem|[HtmlItem](Modules/HtmlItem.js)|6868|GameLocalData, PlayData, SoundManager, TouchButton, UIManagerNew, GameScene, ServerConfig, Lab, NativeCall, GameUtils, HtmlBtn|
|ImagesPanel|[ImagesPanel](Modules/ImagesPanel.js)|1534||
|JBlab|[JBlab](Modules/JBlab.js)|1636||
|Lab|[Lab](Modules/Lab.js)|1587|GameManagement|
|LanguageDialog|[LanguageDialog](Modules/LanguageDialog.js)|2598|BaseUI, EventCenter, GameEventConsts, TouchButton, GameManagement, Lab|
|LinkScript|[LinkScript](Modules/LinkScript.js)|740|UIConfig|
|LoadAllResources|[LoadAllResources](Modules/LoadAllResources.js)|2880|ResManager, Singleton|
|LoaderCoinSprite|[LoaderCoinSprite](Modules/LoaderCoinSprite.js)|1306|GameManagement|
|LoaderRealCoinSprite|[LoaderRealCoinSprite](Modules/LoaderRealCoinSprite.js)|1451|GameManagement|
|LoadingScene|[LoadingScene](Modules/LoadingScene.js)|10064|GameLocalData, LoadAllResources, UIConfig, UIManagerNew, ServerConfig, GameManagement, NativeCall|
|LogUtils|[LogUtils](Modules/LogUtils.js)|947||
|LuckDrawDialog|[LuckDrawDialog](Modules/LuckDrawDialog.js)|4884|BaseUI, GameLocalData, PlayData, SoundManager, TouchButton, UIManagerNew, GameScene, GameManagement, Lab, GameUtils, LuckyDrawRewardDialog|
|LuckyDrawRewardDialog|[LuckyDrawRewardDialog](Modules/LuckyDrawRewardDialog.js)|6392|BaseUI, GameLocalData, PlayData, SoundManager, TouchButton, GameScene, HWL_TStool, GameManagement, Lab, GameUtils|
|MKNoticeConsts|[MKNoticeConsts](Modules/MKNoticeConsts.js)|778|MKNotice, GameEventConsts|
|MKNotice|[MKNotice](Modules/MKNotice.js)|677||
|NativeCall|[NativeCall](Modules/NativeCall.js)|2895|ServerConfig, GameManagement|
|NewGamePlayData|[NewGamePlayData](Modules/NewGamePlayData.js)|4245|ServerConfig, GameManagement, NativeCall, GameLocalData|
|PlatformLoader|[PlatformLoader](Modules/PlatformLoader.js)|1799|GameLocalData, PlayData, ServerConfig, GameManagement|
|PlayData|[PlayData](Modules/PlayData.js)|2471|GameLocalData|
|PreviewSpine|[PreviewSpine](Modules/PreviewSpine.js)|105||
|PrivacyPolicyView|[PrivacyPolicyView](Modules/PrivacyPolicyView.js)|1660|BaseUI, TouchButton, Lab|
|ResManager|[ResManager](Modules/ResManager.js)|2307||
|RewardDialog|[RewardDialog](Modules/RewardDialog.js)|7150|BaseUI, GameLocalData, PlayData, SoundManager, TouchButton, GameScene, GameManagement, Lab, GameUtils, GuideDialog, ScoreDialog|
|RewardRealToast|[RewardRealToast](Modules/RewardRealToast.js)|1599|BaseUI, GameLocalData, PlayData|
|RewardToast|[RewardToast](Modules/RewardToast.js)|1479|GameManagement, BaseUI|
|ScoreDialog|[ScoreDialog](Modules/ScoreDialog.js)|2725|BaseUI, GameLocalData, PlayData, TouchButton, ServerConfig, Lab|
|ServerConfig|[ServerConfig](Modules/ServerConfig.js)|15621|GameManagement, HWL_TStool|
|SettingDialog|[SettingDialog](Modules/SettingDialog.js)|4470|BaseUI, GameLocalData, PlayData, SoundManager, TouchButton, UIConfig, UIManagerNew, Lab|
|Singleton|[Singleton](Modules/Singleton.js)|328||
|SoundManager|[SoundManager](Modules/SoundManager.js)|3082|AudioManager, GameLocalData, LoadAllResources, PlayData|
|SuccessDialog|[SuccessDialog](Modules/SuccessDialog.js)|5160|BaseUI, FlyAnimation, GameLocalData, NewGamePlayData, SoundManager, TouchButton, HWL_TStool, GameManagement, Lab|
|Toast|[Toast](Modules/Toast.js)|2134|BaseUI|
|TouchButton|[TouchButton](Modules/TouchButton.js)|2368|SoundManager|
|UIConfig|[UIConfig](Modules/UIConfig.js)|292||
|UIManagerNew|[UIManagerNew](Modules/UIManagerNew.js)|4185|UIConfig|
|UIUtils|[UIUtils](Modules/UIUtils.js)|1619|LogUtils, ResManager, Singleton|
|UI_TopRewardLayer|[UI_TopRewardLayer](Modules/UI_TopRewardLayer.js)|12181|EventCenter, GameEventConsts, GameLocalData, NewGamePlayData, SoundManager, TouchButton, UIConfig, UIManagerNew, GameManagement, Lab, NativeCall, SettingDialog|
|UpdateDialog|[UpdateDialog](Modules/UpdateDialog.js)|1913|BaseUI, TouchButton, Lab, NativeCall|
|UserDefaultManager|[UserDefaultManager](Modules/UserDefaultManager.js)|543||
|WinRewardView|[WinRewardView](Modules/WinRewardView.js)|3151|BaseUI, FlyAnimation, GameLocalData, NewGamePlayData, SoundManager, TouchButton, GameManagement, Lab|
|WithDrawPlatfrom|[WithDrawPlatfrom](Modules/WithDrawPlatfrom.js)|10389||
|mergeRuleDialog|[mergeRuleDialog](Modules/mergeRuleDialog.js)|1913|BaseUI, TouchButton, Lab|
|ts|[ts](Modules/ts.js)|1832||
