# 抽奖摇杆与 2000 筹码动画修复

工程：`Restoration/11_UnityReskin`。原工程 `06_UnityFramework` 未修改。

## 原版依据

- `Restoration/02_Gameplay/HotUpdate/Modules/LuckDrawDialog.js` 完整函数体：积分足够且未抽奖时进入 `playingDraw`，播放 `bgSpine.idle` 一次；完成回调后启动奖格轮转，停到目标奖格后等待 800 ms 弹出奖励。
- `Restoration/04_Assets/HotUpdate/SkeletalReusable/LaoHuJ_TX/Timelines/idle.json`：ND_00 为球头，Y 位移 -84.46，最大等比缩放 1.048。0.3333～0.6667 秒下压，0.6667～1 秒保持，1～1.6667 秒按原曲线回弹。
- 原 `LuckDrawDialog` 预制体的 Skeleton 参数 `timeScale=2`，实际时间如下。

| 游戏时间 | 动作 |
|---|---|
| 0～0.16665 s | 初始姿势 |
| 0.16665～0.33335 s | 下压 |
| 0.33335～0.5 s | 保持 |
| 0.5～0.83335 s | 回弹 |
| 随后 | 奖格轮转 → 命中目标 → 等待 0.8 s → 奖励页 |

## 实现

- `RecoveredWheelLever` 直接跟随现有原版 Unity AnimationClip 采样的 ND_00 骨骼，未创建第二个动画计时器。球头等比缩放，金属杆独立压缩；不同外观的运动距离按球头与轴座距离换算。
- 机身、金属杆、球头是透明 PNG 与预制体中的独立 Image。原绿色球头像素直接复用。没有新增点击区域；仍由原抽奖 Button 和积分判断触发。
- 每次打开抽奖页恢复原始姿势，避免上一轮动画残留。
- `GuangH_TX` 的 C1 是旧 2000 硬币层。仅隐藏这一层，以 `Gameplay/Coins/2000` 替代，保留原骨骼的缩放、颜色、星光、光圈和动画完成回调。弹窗中原指向 `RulesReskin/Chip2000` 的展示也统一引用该资源。
- 基于原生 UGUI/Animation，无新增第三方插件。每帧仅更新少量已绑定 Transform，未在 Update 中加载资源或创建对象。

## 验证

打开 `REVIEW.html` 查看姿势对照、庆祝动画截图及结果。`Verification/play_mode.json` 为真实 Unity Play 验证结果。
验证在独立测试存档中运行：积分不足、空闲不动、源曲线采样、重复点击、两次抽奖及奖励领取、1000+1000 合成后庆祝/飞入计数器/恢复玩法。

重跑入口：Unity 菜单 `Coin Merge / Reskin / Restore prize machine lever and highest chip` 重新写入部件。完整验证由 `PrizeAnimationReskinValidation.Run` 调用。

## 美术来源与可复现处理

使用内置 imagegen 对现有 `UnifiedReskin/Machine.png` 编辑，输出 `GeneratedBody.png` 与 `GeneratedParts.png`。最终项目资源位于 `Assets/Resources/WheelReskin`；`prepare.cjs` 进行透明边界清理与部件裁切，源文件不覆盖。

提示词要点：
1. 只移除右侧绿色球头和银色杆，补齐固定蓝色轴座，保持原画布、云朵、玻璃高光、空深蓝内框、顶部绿灯及机身比例，输出真实透明背景。
2. 在透明画布上将右侧绿色球头与倾斜银色杆分别拆出，不带机身、底座、文字和棋盘格，边缘抗锯齿。
最终球头采用原已认可素材的精确裁切，而非生成图中的替代球头。
