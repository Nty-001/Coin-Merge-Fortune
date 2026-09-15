# 播报、提现提示、描边和加载页（2026-09-15）

正式 Unity 工程为 `Restoration/06_UnityFramework`，启动入口为 `Assets/Scenes/RecoveredLoading.unity`。本轮不修改 MuMu 原应用或用户玩家存档。商业 SDK 沿用本地 facade/mock。

## 铃铛旁播报

依据 `02_Gameplay/HotUpdate/Modules/GameScene.js` 的 `startGetMoneyTipScroll`、`scrollGetMoneyTip`、`refreshGetMoneyTipText` 及其全部随机生成函数实现。读取了场景序列化绑定：`getMoneyTipNode` 实际引用 `iconContent`（节点 23），不是包含铃铛的外层节点。因此使用 110 单位默认距离，只移动文字容器；父节点 17 的原 606 × 94 遮罩裁剪内容，铃铛位置固定。

- 进入游戏立即生成文案；每 4 秒开始一轮，0.9 秒向上移出、更新内容并移到下方，再用 0.9 秒移回；两段均为 sineInOut。
- 姓名按 `A**BC` 的三枚随机大写字母生成；天数为 2–10 的闭区间；金额随机选当前地区 `new_Fake_products` 档位。
- 文案来自语言表键 109，金额使用原 `getmonstr` 对应格式，保留黄色富文本。它是原客户端生成的模拟播报，不是已核实的真实他人提现记录。
- 字号 28、行高 30、最大宽度 540 来自原 RichText 序列化实值。原 `fitGetMoneyTipLabel` 没有在此链路调用，没有擅自启用其另一套尺寸。
- 失败重新开始和清当前版本玩家进度会重启播报时间；离开或禁用对象后停止更新。固定节点与引用保存在真实 Prefab/Scene 中，无运行时创建静态 UI。

## Withdraw 旁提示

原 `setFakeMoney` 的规则已存在，并非缺少文字：余额小于当前地区 `real_products[0].withdrawAmount` 时显示语言键 56，达到或超过门槛后隐藏。保留这个规则，不强制常显。

测试覆盖 US 精确余额 434.77、220、500、23569.29：前两者显示剩余金额，后两者隐藏。截图的两位小数余额不能确定内部完整小数，因此它和剩余金额相加可能有一分钱的显示差异；仍使用原金额截断规则。

## 原生圆周描边

UGUI 默认 Outline 在四个对角方向复制文字，原 6 单位描边会扩到约 8.49 的对角距离，产生截图中的尖角和分叉。本轮 `RecoveredRoundOutline` 继承官方 UGUI Outline，用 16 个圆周方向形成 6 单位半径，保留最后一层文字填充。

转换保留原组件 fileID、启用状态、颜色、宽度及账户表单的 Outline 引用，覆盖已有两版 Prefab/Scene；规则标题、OK、设置、提现页、进度数字和加载百分比共用该修复，没有改变无描边正文。没有添加第三方程序集或材质依赖。

一个启用描边的文字网格最多扩展为原顶点数的 17 倍，代价高于默认四方向，但只在 UI 需要重建文字网格时处理，复用列表；没有逐帧生成静态层级或字符串。未宣称这种跨引擎栅格化和抗锯齿已经逐像素一致。

## 原加载入口

- B：原 `HotUpdate/scene/LoadingScene` 的背景、Logo、进度条、百分比和 loading 文案，保存为 `Resources/Startup/RewardedLoading.prefab`。
- A：原 `Packaged/Scene/LoadScene` 的另一套背景、Logo、进度条及 Loading 文案，保存为 `Resources/Startup/PackagedLoading.prefab`；采用原 2 秒进度时间。
- `RecoveredLoading` 是构建第一场景，读取已保存版本选择，按路径只加载对应 Prefab，再用 Unity 异步场景加载进入 `RecoveredMain` 或 `RecoveredPackaged`，传递同一存档命名空间。
- B 沿用原 15/25/55/70/85/99/100% 阶段、模拟进度单调递增至 90% 的上限，以及完成后 0.3 秒等待。场景加载阶段根据实际 `AsyncOperation.progress` 更新。
- 原联网取地区、远端更新、SDK 等待继续使用现有本地数据边界，因此离线加载耗时不保证与联网原应用相同。本轮没有伪造服务器更新检查，也没有新增真实支付或身份归因调用。
- 直接打开 `RecoveredMain` 是跳过启动流程的玩法调试入口；完整启动测试应从 `RecoveredLoading` Play，与构建入口一致。

## 验证与复现

`RecoveredVisualsBuilder.Run` 制作资产，`RecoveredVisualsValidation.Run` / `RunPackaged` 验证两套启动、播报和文字效果。`RecoveredMenusValidation.Run` 回归实际按钮命中、账户验证、提现条件和返回链路。报告和 Unity 渲染图在本目录；所有自动流程使用独立测试存档。

本轮结果：收益版 25 项、基础版 5 项、菜单 130 项通过；88 个 Prefab/Scene（含两个新的 Resources 加载 Prefab）结构检查通过。`visuals_checkpoint_sync.json` 记录从验证副本同步到正式工程的 57 个文件及 SHA-256。规则图在原 3.3333 秒骨骼动画完成后截取，不把中间展开帧当成最终布局。

外部同步资产后，需要在用户已打开的正式编辑器重新载入场景并实际运行，不能只以验证副本成功作为正式窗口已经更新的证据。

正式窗口补验：用户手动停止旧 Play 并运行加载入口后，已观察到 `06_UnityFramework` 进入 `RecoveredPackaged` 的分数/爱心界面；加载日志确认进入该场景。当前正式存档的 GM 选择为 US、基础版 A，所以此结果符合已保存选择。没有清除或改写用户的版本选择、玩家进度。自动窗口控制在激活阶段返回 `failed to activate captured window`，因此正式窗口的收益版播报和规则弹窗尚未完成鼠标复验；其运行结果以上述独立 Play Mode 检查及截图为依据。

## 奖励金额居中修复（2026-09-15）

`Double Rewards` 金额节点使用固定的 77.44 宽文本框，UGUI 左对齐溢出使较长金额向右偏移。已将双倍/复活、普通奖励和最高金币奖励的金额改为 `MiddleCenter`，引导奖励原本已居中；保留原坐标、字号、颜色、金额值及奖励逻辑。同步修改 `RecoveredMain` 场景、运行预制体和 `NativeGameplayBuilder`，避免重建后回退。独立 Unity Play Mode 渲染了 `$6.72` 和 `$1,234.56`，已逐图确认居中，日志无编译或运行异常。截图为本目录 `reward_amount_centered.png` 和 `reward_amount_centered_long.png`，使用独立测试存档；没有操作用户正在运行的场景。
