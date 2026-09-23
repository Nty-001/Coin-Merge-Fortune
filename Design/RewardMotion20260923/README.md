# 奖励弹窗国家钞票、独立动效与摇杆修复

已应用 `Restoration/12_UnityReskin_20260922` 当前场景与运行预制体。实际运行画面见 [预览](index.html)。未构建 APK，未安装或运行 MuMu。

- 普通奖励、双倍奖励按用户最新 1254 × 1254 参考图，统一为 719 × 719 画布并保留图片比例。标题、金额与双倍奖励顶部高清硬币按同一比例定位，去掉标题横向压缩。
- 普通、双倍、复活成功、最高面值合成和抽奖奖励恢复旋转光效。保留现有弹窗控制器，使用原版光效纹理及原版顺时针每秒 90 度的速度；新 UI 材质仅在蓝色内容区域合成光效。
- 普通、双倍、复活成功奖励的钞票拆成原生 Image 图层，加入现有 currencyIcons 国家切换绑定。26 个国家使用游戏已有对应钞票，金额计算不变。
- 4 张空白面板移除了原来烘焙在图片里的钞票、光束和星点。底板与唯一的旋转光效分开，保留原图透明轮廓、标题带和边框。
- 抽奖机摇杆以新机身右侧转轴为固定点，对齐杆顶和圆球，继续读取原 LaoHuJ_TX 动画的 ND_00 位移；圆球全程等比缩放并保留屏幕内边距。
- 提现提示框按照原版 SineInOut 上移 10（1 秒）、下移至 -10（2 秒）、回到起点（1 秒）循环。原余额阈值显隐和提示金额计算未改。
- 保留原奖励关闭、结算、音效、撒花、飞钞、复活缩放以及抽奖交互逻辑。

## 原始依据

- `Restoration/02_Gameplay/HotUpdate/Modules/RewardDialog.js:120`：角度归零，`by(4, {angle: -360}).repeatForever()`。
- `Restoration/02_Gameplay/HotUpdate/Modules/LuckyDrawRewardDialog.js:94`：相同旋转周期。
- `Restoration/02_Gameplay/HotUpdate/Modules/GameScene.js:382`：提示框三段正弦缓动，共 4 秒。

换皮后的光效颜色适配新蓝色面板；运动参数来自完整恢复的原 JS。空白 RGB 面板由内置 imagegen 编辑，提示词记录在 `asset-prompts.json`；Unity 使用原 Sprite 的 alpha 轮廓。钞票是现有高清国家资源组成的原生九张钞票堆，未重新生成或压缩钞票。

## 验证

Unity 2022.3.62f3c1 从 RecoveredLoading 完整启动，在隔离副本及临时存档命名空间运行。检查 1080 × 1920、1080 × 2340 的比例、转速、提示浮动、余额 500 显隐和原 1.5 秒奖励自动关闭；逐一切换 26 国，检查普通、双倍、复活和抽奖奖励钞票绑定；采样摇杆原动画的静止、按压、回位状态。最终检查数量见 `verification.json`。光效强度设为零的实机渲染图用于核对底板没有静态光束。

`verification.json` 记录结果；`reward-motion.patch` 记录真实 Scene / Prefab 修改。当前基础场景未纳入本仓库跟踪，因此保存迁移补丁，而不是提交整套生成的工程与缓存。`Before/` 本地回退备份不进入提交。

制作入口：`RewardMotionAuthor.Run`；独立验证入口：`RewardMotionReview.Run`。已应用的运行场景不依赖 Editor 制作器兜底。
