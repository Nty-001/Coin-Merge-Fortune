# Packaged 主玩法函数体索引

每个模块文件含完整原始编译后的 JavaScript 函数体；依赖映射位于文件头。保留的变量名可能经过编译压缩。并非 TypeScript 原始工程。

|模块|完整函数体|字节数|依赖|
|---|---|---:|---|
|AddHeartView|[AddHeartView](Modules/AddHeartView.js)|2367|BaseUI, EventCenter, GameEventConsts, LocalDataManager, TouchButton, UIManager, Utils, GameScene|
|AudioManager|[AudioManager](Modules/AudioManager.js)|644||
|BaseRecord|[BaseRecord](Modules/BaseRecord.js)|390||
|BaseUI|[BaseUI](Modules/BaseUI.js)|1778|UIManager|
|BlockUtils|[BlockUtils](Modules/BlockUtils.js)|1908|Block, PoolManager, Utils|
|Block|[Block](Modules/Block.js)|7519|SoundManager, BlockUtils, LevelManager, PoolManager, Wall|
|EventCenter|[EventCenter](Modules/EventCenter.js)|1478||
|FailureView|[FailureView](Modules/FailureView.js)|2117|BaseUI, LocalDataManager, SoundManager, TouchButton, UIManager, LevelManager, GameScene|
|GameEventConsts|[GameEventConsts](Modules/GameEventConsts.js)|591||
|GameMapLayer|[GameMapLayer](Modules/GameMapLayer.js)|4257|TouchButton, GameScene, LevelManager, PhysicsManager, Wall|
|GameMapTouch|[GameMapTouch](Modules/GameMapTouch.js)|1501||
|GameMenu|[GameMenu](Modules/GameMenu.js)|2102|BaseUI, EventCenter, GameEventConsts, LocalDataManager, TouchButton, UIManager|
|GameModel|[GameModel](Modules/GameModel.js)|2806|BaseRecord, EventCenter, GameEventConsts, LocalDataManager|
|GameScene|[GameScene](Modules/GameScene.js)|1782|BaseUI, EventCenter, GameEventConsts, LocalDataManager, SoundManager, UIManager, LevelManager|
|Game|[Game](Modules/Game.js)|721||
|GuideView|[GuideView](Modules/GuideView.js)|2162|BaseUI, EventCenter, GameEventConsts, LocalDataManager, TouchButton, UIManager|
|HomeView|[HomeView](Modules/HomeView.js)|1466|BaseUI, TouchButton, UIManager, Item|
|HonorItem|[HonorItem](Modules/HonorItem.js)|1684|LocalDataManager, TouchButton, UIManager|
|HonorView|[HonorView](Modules/HonorView.js)|1274|BaseUI, TouchButton, HonorItem|
|ImagesPanel|[ImagesPanel](Modules/ImagesPanel.js)|1404||
|InfoView|[InfoView](Modules/InfoView.js)|1502|BaseUI, LocalDataManager, TouchButton|
|Item|[Item](Modules/Item.js)|1869|ImagesPanel, LocalDataManager, TouchButton, UIManager, GameScene|
|LevelManager|[LevelManager](Modules/LevelManager.js)|12864|BaseUI, EventCenter, GameEventConsts, LocalDataManager, SoundManager, UIManager, Block, BlockUtils, GameMapLayer, GameMapTouch, LevelUtils|
|LevelUtils|[LevelUtils](Modules/LevelUtils.js)|873|LocalDataManager, Block, LevelManager, Utils|
|LoadAllResources|[LoadAllResources](Modules/LoadAllResources.js)|2602|ResManager, Singleton|
|LoadScene|[LoadScene](Modules/LoadScene.js)|2326|BaseUI, EventCenter, GameEventConsts, LoadAllResources, LocalDataManager, UIManager, BlockUtils|
|LocalDataManager|[LocalDataManager](Modules/LocalDataManager.js)|2628|Utils, GameModel, Singleton, UserDefault|
|PhysicsManager|[PhysicsManager](Modules/PhysicsManager.js)|1194||
|PolicyView|[PolicyView](Modules/PolicyView.js)|1409|BaseUI, ImagesPanel, TouchButton|
|PoolManager|[PoolManager](Modules/PoolManager.js)|893|Utils|
|ResManager|[ResManager](Modules/ResManager.js)|1540||
|SettingView|[SettingView](Modules/SettingView.js)|2389|BaseUI, LocalDataManager, SoundManager, TouchButton, UIManager|
|Singleton|[Singleton](Modules/Singleton.js)|328||
|SoundManager|[SoundManager](Modules/SoundManager.js)|1777|AudioManager, LoadAllResources, LocalDataManager|
|ToastView|[ToastView](Modules/ToastView.js)|1250|BaseUI|
|TouchButton|[TouchButton](Modules/TouchButton.js)|2368|SoundManager|
|UIManager|[UIManager](Modules/UIManager.js)|1767|BaseUI|
|UserDefault|[UserDefault](Modules/UserDefault.js)|536||
|Utils|[Utils](Modules/Utils.js)|1244|SoundManager|
|Wall|[Wall](Modules/Wall.js)|1340||
