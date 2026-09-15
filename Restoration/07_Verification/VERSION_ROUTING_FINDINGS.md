# 内容版本、AB 与流量归因核对

核对对象：本机导出的 APK v1.6（versionCode 16）、设备已下载的 app_coin 热更新资源、完整恢复的 JS 模块和缓存配置。没有向服务端发送设备标识；没有修改 MuMu 中的原应用。

## 能确定的事实

| 机制 | 原始证据 | 结论 |
| --- | --- | --- |
| APK 内置内容 | `02_Gameplay/Packaged/Modules/GameScene.js`、`HomeView.js`、`Item.js`、`GameModel.js`、`LevelManager.js` | 首页有六个模式、金币与体力，消耗体力进入合成玩法；模式用金币解锁。 |
| 下载后的收益内容 | `02_Gameplay/HotUpdate/Modules/GameScene.js`、`PlayData.js`、提现及转盘模块 | 现金奖励、提现入口、转盘和广告业务分支属于另一套已落盘内容。 |
| 原包 AB 标记 | `HotUpdate/Modules/NativeCall.js` 的完整 `checkClientEndingWith`；`LoadingScene.js` 调用设备 ID 初始化 | 尾字符 5–9 写 RandomAB=B，其余非空字符写 A；空 ID 写 B 并返回 false。不是按所有字符的奇偶随机分组。 |
| 旧金币目标组 | `NativeCall.checkClientEndingWithCoin` 与 `Cached.normalized.json` 的 `coin_mubiaoA` | 本地缓存 A 列表为 0–4；浏览器直接返回 true；原生端无 ID 返回 false。被旧版 `UI_TopRewardLayer`、`FlyAnimation`、`NewGamePlayData` 引用。 |
| AB 是否决定当前两个完整主界面 | 全部已恢复热更新模块中搜索 `RandomAB`，再核对当前 `GameScene` / `PlayData` | 找到标记写入，未找到当前玩法读取该标记切换两套主界面的证据。旧模块还存在，不等于当前入口执行它。 |
| 买量 / 自然量是否决定内容包 | 已恢复 APK / runtime DEX、归因标志与本地运行记录 | 未建立可验证的内容包选择条件。`attribution_state` 是归因上报相关状态；`h_i_x/e/y` 涉及广告设置，不能据此认定买量或自然量。 |

**因此：确认有 AB 分组代码，也确认有“基础内置包 / 收益热更新包”；尚不能认定它们分别就是“自然量版 / 买量版”。不应把 AB 标签、内容版本和广告归因混成同一开关。**

## 本轮 Unity 实现

- `RecoveredMain.unity`：已有收益版运行场景。保持现有广告 facade/mock、引导、奖励、转盘链路。
- `RecoveredPackaged.unity`：使用原始基础版页面与美术，接入协议、模式首页、解锁、体力、世界空间金币、原始多边形碰撞体、出币配置、合成计分、失败重试、信息/体力/设置/荣誉/政策弹窗。音乐开关连接原生 AudioSource 和原包 BG 音频。
- `VersionGM.prefab`：650 × 920 的居中卡片，参考画布 750 × 1624，中文界面。仅遮罩覆盖全屏，内容卡片不全屏；右下侧 GM 按钮可打开，X/遮罩可取消，应用才保存并重进场景。
- 玩法版本提供明确的“A：基础版 / B：收益版”两个按钮，应用后切换实际场景；A/B 是本地测试名称，不代表已证实原游戏买量归因。自动比例分流与 26 国地区独立设置；原包 AB 标签改为只读，明确注明不切换界面。
- 自动内容分流是**新增的本地测试策略**：每个安装生成稳定的 0–9999 测试桶，按设置的收益版比例分配；不是冒充已还原的原服务端买量规则。默认依旧为 US / 收益版 / B。
- 自动 AB 用本地生成的稳定数字尾号复现原函数；不读取或上传真实设备标识。没有虚构 RandomAB 对当前主玩法的 UI 影响。
- 现有收益进度、基础版进度、GM profile 三个存储键独立。GM 清除当前玩家存档保留分流；切回另一版本恢复它自己的进度。全部应用数据被系统删除后，回到工程内置的 US 收益版默认值。
- Editor 与设备使用同一套 PlayerPrefs、资源加载、分流与场景逻辑，无 Editor 专用核心逻辑分支。

## 验证

- `version_variants_validation.json`：74 项实际 Unity Play Mode 检查，包括 EventSystem 指针命中、版本场景加载、独立存档、清档持久化、地区/比例按钮、基础版多边形物理合成、解锁、体力购买、失败重试和音乐开关。
- `version_rules_validation.json`：25 组原始 AB 函数结果、495 组原始基础版出币函数结果，C# 与原 JS 对照通过。合成输入均为 synthetic-test，无真实设备 ID。
- `probe_version_rules.cjs` 执行恢复的完整原始方法体生成结果，输出保留方法 SHA-256，非仅看函数签名或调用链。
- 原 Cocos 默认重力 -320 世界单位/秒²、32 世界单位/米，依据 [Cocos 官方物理文档](https://docs.cocos.com/creator/2.4/manual/zh/physics/physics/physics-manager.html)。基础版源代码设置累积固定步长 1/60、速度/位置求解 2/2；收益版仍保留它自己的设置。
- 页面截图：`version_gm_popup.png`、`version_gm_rewards_jp.png`、`version_packaged_home.png`、`version_packaged_game.png`、`version_packaged_agreement.png`。

## 不能计为 100% 复刻的部分

- 原服务端的流量归因和内容包下发条件没有完整证据，本轮没有伪造配置真值。
- `LevelManager.onMergeTwoMax` / VictoryView 没有已找到的当前调用方；type_11 的合并分支只锁定合并状态。保留该事实，没有捏造通关表或结算页面。
- 基础版原协议链接依赖空点击区域。按用户“可见标准 Button”要求改为真实文字按钮，保留政策图与含义，排版属于明确的原生适配差异。
- 基础版启动进度场景、全部细粒度补间/爆破时序、音效触发时刻、跨 Cocos/Unity 物理轨迹与全部页面逐像素对照尚未完成。现有收益版提现完整业务面板等早先记录的缺口也不能因加入 GM 而标为已完成。
- 原始完整函数体和资源继续保留，以上运行验证证明本轮分流与可玩分支，不证明全部生命周期 100% 一致。

## 打开与复验

Unity Hub 添加 `Restoration/06_UnityFramework` 文件夹，使用 2022.3.62f3c1；打开 `Assets/Scenes/RecoveredMain.unity` 后 Play。

在游戏内点击右下侧 GM。选择内容版本、AB 与国家后点击“应用并重新进入”。已有正在运行的旧场景需要先停止 Play，再重新打开该场景。

资产制作菜单 `Coin Merge/Author content variants and GM popup`；验证入口 `CoinMerge.Recovery.Editor.VersionVariantsValidation.Run`。实际 Play Mode 验证自动使用独立临时命名空间并清理，不触碰用户存档。

2026-09-15 GM 易用性修正：此前可点击的原包 AB 标记没有场景分支，容易被误认为玩法切换。现提供明确的 A/B 玩法按钮，并显示“当前 / 应用后”状态；实际指针点击 A→基础场景、B→收益场景、取消与存档恢复均验证通过。
