# 当前工程审查与下一步修复依据

结论：当前工程远未达到 1:1 复刻。此次只审查原工程，在独立副本中验证修复方向；未修改原工程 Assets、Packages 或 ProjectSettings，未完成 Git 提交/推送。

## Git 与工作范围

- 用户提供的远端为 `https://github.com/Nty-001/Coin-Merge-Fortune.git`。GitHub 只读 API 返回公开仓库、默认分支 main、size 0；这不代替实际 fetch 或 push 验证。
- 当前工作目录及检查过的父目录没有 `.git`。
- 用户要求使用本机图形 Git 工具。检测到 TortoiseGit 首次启动向导，窗口访问确认超时；没有擅自改用 git.exe 或 gh。
- 已准备根目录 `.gitignore`，排除 Unity 缓存、独立审查副本、工具依赖、设备原始采集、私人存档及含设备字段的汇总表。公开远端不包含这些本地数据。
- 231 MB 的完整 native 汇编原文保持本地；压缩副本可进入版本控制。此限制不删减本地还原证据。
- 已生成原工程 2,731 个文件的 SHA-256 基线：`original_project_manifest.json`。

## Unity 实际导入结果

指定的 Unity 2022.3.62f3c1 现在能正常启动。之前“许可证不可用”的结论已过时，但之前的验证记录作为历史记录保留。

首次导入原工程副本发现：

|检查|结果|
|原始 Prefab / Scene 数量|63 / 5|
|出现 Unity YAML 解析错误的文件|68|
|解析错误日志条数|5,520，包含重复载入产生的消息|
|LoadPrefabContents 失败|44 / 63|
|热更新主场景根节点数|23，原始模型应为一个场景根层级|
|热更新主场景 Button / NativeMergeBoard / NativeMergeCoin|0 / 0 / 0|
|热更新主场景 SpriteRenderer / Camera / EventSystem|0 / 0 / 0|
|当前启用的构建入口|MockFlow.unity|

没有 Missing Script 不代表玩法已实现：场景上挂载的是 RecoveredNode 数据记录组件，而非 GameScene 等原始行为的 C# 移植。

问题与修复试验：现有手工 YAML 序列化器输出的独立 `-` 列表项目可被通用 YAML 解析器接受，但 Unity 导入器报错，并修复/拆散组件和父子层级。在独立副本中，将 68 个文件重编码为 Unity 可识别的列表排版，逐对象比较字段值未改变后再次导入。结果 63 个 Prefab 全部通过 LoadPrefabContents，热更新主场景根节点恢复为 1。该修复只验证在副本，尚未写回原工程。

证据：`import_findings.json`、`unity_scene_audit.json`、`yaml_format_probe_manifest.json`、`unity_scene_audit_format_probe.json`。完整日志仅保留本地。

## 主流程差异与恢复顺序

以下是优先接入范围，不声称已完成全部分支的运行等价证明。原始完整 JS 函数体仍在 Restoration/02_Gameplay；`source_module_review_index.json` 只是辅助定位索引。

|玩家阶段/触发|原始实现依据|当前缺失与必须接入的行为|
|启动与配置选择|HotUpdate/LoadingScene.js: onLoad、startLoading、loadGameDataConfig、checkupdate|地区和客户端获取超时、版本变化后的配置覆盖、缓存优先与后台刷新、本地表 fallback、更新提示、资源进度、进入 GameScene|
|进入或恢复棋盘|GameScene.js: onLoad、loadGameSceneData、createInitialBottomCoins、setupPreviewQueue|真实存档模型、初始 [1,2,5,2,1]、当前/下个金币、位置和角度、分数、引导状态、BGM|
|拖动与释放|GameScene.js: onTouchStart/Move/End、getPreviewXByTouchEvent、dropCoin|落点限制、引导线、生成锁及延迟释放、按落差设置物理状态、投币次数、下一枚 .3 秒延迟、失败检查|
|金币接触与合并|CoinItem.js；GameScene.js: onCreateMergedCoin|碰撞与合并 sensor、合并锁、值升级、物理参数、出生位置与缩放、合并连击、音效、奖励计分|
|引导|GuideDialog.js；GameScene.js: setshowGuide；RewardDialog.js|步骤 0/1/2/3/4/9999 的真实条件与持久化、奖励转场、屏幕适配、按钮与金额高亮位置|
|闲置与屏幕适配|GameScene.js: update、adaptHomeForFullScreen、adaptPlayAreaForBottomBar|15 秒闲置手势、宽高比与底栏适配、地面/预览线/失败线和金币位置修正|
|合并视觉反馈|GameScene.js: playCoinSpawnTween、playMergeStarFlyEffect、showMergeComboUI|生成缩放、连击位置/透明度、星星散开与飞入、星星到达后再刷新进度，不能提前加视觉数值|
|2000 金币事件|GameScene.js: playMerge1024Flow、finishMerge1024Flow|名称虽含 1024，应按实际值与计数逻辑移植；特效、飞往按钮、按钮 pulse、奖励面板、恢复预览|
|抽奖进度与轮盘|GameUtils.js: getLotteryScoreConfig/checkLotteryStatus；LuckDrawDialog.js；LuckyDrawRewardDialog.js|分段积分门槛、次数、奖励权重、两圈后停格、分段变速、广告成功/失败、奖励结算与回场|
|投币奖励与强制广告触发|GameScene.js: checkShowReward/getADDouble；RewardDialog.js|first/second/thirdAndlookADDouble 配置阈值、延迟弹窗、首强奖励、showType 1~5、失败不发奖励|
|失败判定|GameScene.js: checkGameOver/updatePendingDeadLineFailCoin/triggerGameOver|静止确认、最大等待、预览及合并状态排除、停止输入和合并、取消待弹窗、变红/冻结/失败动画、分数保存|
|广告复活与无广告重开|FailDialog.js；RewardDialog.js；GameScene.js: removeTopTwoThirdCoins/resetGameAfterFailWithoutAd|广告锁、取消/失败后恢复按钮、成功奖励、清除顶部金币、物理恢复；关闭失败框后的完整重置|
|余额与两套提现入口|GameFakeWDDialog.js、GameRealWDDialog.js、GameRealWDItem.js 及账户/记录模块|进度、条件、币种、地区平台、表单验证、记录、成功/失败反馈及返回路径；商业 SDK 继续 facade/mock|
|顶部滚动提现文案|GameScene.js: startGetMoneyTipScroll/refreshGetMoneyTipText|原代码生成的随机用户名和金额、滚动时序、富文本排版，不把其当成真实用户交易数据|
|外围入口解锁|GameScene.js: setTaskBtnShowOrHide；HtmlDialog.js|原流程依赖返回任务状态决定可见性；使用现有 mock 边界提供可测试返回值，不虚构真实服务器任务|
|设置、规则与评分|SettingDialog.js、mergeRuleDialog.js、ScoreDialog.js|原流程中的显示/关闭、音乐音效持久化和评分触发。SDK 跳转保留 mock 处理|
|切后台、退出和重新进入|GameScene.js: saveBoardState/flushBoardStateToStorage/onGameHide/onDestroy；GameLocalData.js/PlayData.js|棋盘与预览持久化、后台刷新、窗口计数重置、取消延时回调及事件解绑|
|国家和 A/B GM|GameManagement.js、NativeCall.js、DebugDialog.js|Unity 中尚无控制器。默认 US 收益/广告测试版本；国家、实验分组与玩家存档分开管理；不能把 client 尾号 A/B 等同于自然量/买量|

## 视觉与开发规则差异

- 当前源场景金币采用 UI 图片的基础转换，尚无运行时 SpriteRenderer 游戏金币。需要世界空间金币 Prefab、明确单位换算、物理材质和相机，再通过 UI 展示 HUD。
- 原始 63 个节点图不是 63 个已实现功能。自定义控件、Widget/Layout、动态文字、遮罩、动画与骨骼表现均需实际移植和绑定。
- 当前合并示例在合并路径 Instantiate/Destroy、在碰撞路径 GetComponent，且半径/分数等参数写死。应先忠实提取配置，再缓存组件与复用金币实例。
- 原触摸脚本由 Unity 标准 Button 接替 UI 交互，代码绑定事件；拖动投币属于玩法输入，不能用不可见伪按钮替代整个棋盘。
- 不把代码生成全部静态 UI 当作运行时方案。编辑器制作/转换后保存的 Prefab 才是运行载体。
- Shader、粒子、网格、AnimationClip、音频和字体需逐项对照原始资源；原包是 Cocos 2D，不能凭空声称 Unity 场景烘焙参数与原包一致。
- 原生骨骼/网格转换推荐保留骨骼和关键帧，以 Unity AnimationClip 与原生网格组件承载；复杂 deform/clipping 需额外实现与验证。整段特效烘成全尺寸逐帧图片只适合有限小特效，不适合默认替代：例如 1024×1024 RGBA、30 fps、4 秒约 480 MiB 未压缩纹理，显著增加内存和加载量。骨骼方案内存更低，但仍需测量每帧蒙皮、裁剪和 GC 成本。

## 国家与配置证据边界

已保存 BR、ID、TH、VN 的区域字段以及 group1~6/groupNew1~6。当前缓存没有独立的 US、MY、PH 区域对象；原代码有全局合并和默认值逻辑。按原逻辑回退，不把缺失对象补成未经取得的“服务器真值”。`configuration_availability.json` 记录可用性。

本轮没有调用游戏服务器，也没有改变 MuMu 中的安装包、存档或分流状态。此前向游戏域名发送真实客户端 ID 的审批问题没有因提供 Git 地址而获得授权。

## 下一轮必须先完成

1. 完成 TortoiseGit 初始化与窗口访问，或由用户明确允许使用本机 git.exe；读取远端并保存当前版本的提交与推送检查点。
2. 把已在副本验证的 YAML 修复写回生成器与原工程，运行全部 Prefab/Scene 导入门槛，提交推送。
3. 优先移植启动/配置/存档/引导/投币/合并/失败重开主链，并建立标准 Button、世界空间金币 Prefab 与配置资源，逐段和原函数体/真机画面核对。
4. 在主链上补齐奖励、抽奖、提现业务触发、GM 和视觉时序；商业 SDK 保持现有 mock，不扩展无关功能。

全局 skill 已安装：`C:/Users/001/.codex/skills/unity-native-prefab-development/SKILL.md`，工作目录中保留可审查副本。
