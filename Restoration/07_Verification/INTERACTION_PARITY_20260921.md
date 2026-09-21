# 五项交互补齐 — 2026-09-21

对象：`11_UnityReskin` 收益版 RecoveredMain。沿用 Unity 原生玩法预制体、代码绑定按钮与本地 SDK Mock。其他审计差异不在本次修改范围。

## 实现与原始依据

- **震动**：CoinItem.js 的 playCollisionEffect / merge 在 open_vibrate 开启时调用 `N_vibrate(1,20,10,255)`。Unity 对应首次实体接触与成功合成事件，Android 原生 Vibrator 单次 20 ms、幅度 255；Android 8 以下使用兼容单次接口。加入 VIBRATE 权限与官方 Android JNI 模块，无商业 SDK、无反射。模拟器无法证明真实手机的触感；iOS 仍使用系统标准震动，未声明其与 Android 参数等同。
- **投币音效**：GameScene.dropCoin 使用原始 sfx_bom；新增投币 AudioSource，按 open_music 开关播放。原 wav 内容直接复用。
- **普通碰撞效果**：CoinItem.onBeginContact 首次实体接触触发一次；采用原碰撞 Skeleton UUID `1059e560-1ac0-4260-80ef-2291746bdd38` 的 PZ_TX/idle、速度 4、原偏移与面值缩放。按硬币对象代次追踪并池化回收，合成或移除时不残留。原 stopCollisionEffect 虽安排 0.2 秒淡出，merge 随即销毁两个父硬币；本实现也随回收终止附属效果。
- **复活**：GameScene.removeTopTwoThirdCoins 实际为选取向上取整的三分之一并保护最高面值，按顶部排序，间隔 0.04 秒，每枚 0.12 秒缩小并淡出。等待全部完成后恢复物理与投币；重复请求不重启动画；全部为最高币时直接恢复。参数保存在 LifecycleVisuals.asset。
- **广告等待**：游戏实际 1_A 投币、2_A 转盘、3_A 复活都等待本地播放视图的异步回调。默认模拟 5 秒，可在 MockAdPlaybackView 调整；这不是原 SDK 广告时长的精确复刻。等待时锁定玩法、暂停正在播放的声音，切后台停止倒计时；关闭时按当前音效/音乐设置恢复。Completed 等待计时结束且用户关闭；提前关闭为 Cancelled；Failed 等待后返回；Unavailable 不展示。保持原奖励分支，包括转盘的错误回调仍结算。广告层是真实序列化 Canvas 与标准 Button，没有广告网络或付款能力。

## 验证

Unity 2022.3.62f3c1 在隔离副本导入、生成资源并运行实际 RecoveredMain 场景。专项 41 项检查记录见 `interaction_parity_validation_20260921.txt`：原音效与碰撞资源、真实物理接触/合成、开关、对象池、复活选择与动画时序、后台暂停、广告并发/取消/失败/不可用、三个业务入口、音频暂停恢复均通过。前后台物理回归也通过，见 `interaction_resume_regression_20260921.txt`。

Android 双 ABI APK 构建成功，0 errors、12 warnings。设备首轮发现 Unity 默认 androidlib 模板没有编译 Java src，已补全显式 Gradle sourceSets、构建工具版本与混淆保留规则，并重新构建覆盖安装。

MuMu 最终包验证：正常进入游戏、实体碰撞不产生 JNI 异常；系统振动服务确实记录本游戏 `Step=20ms(amplitude=1.00)`；VIBRATE 权限已授予。实际第 22 次投币进入广告等待页，成功关闭按钮在等待期间禁用，倒计时后启用，关闭后进入双倍奖励。设备截图留在忽略的 Builds/Android，不提交设备标识或原始系统日志。以上不能替代手机震动触感验收。

测试 APK：`Restoration/Builds/Android/MergeStackJourney_InteractionParity_20260921.apk`，已覆盖安装到 MuMu，未清除玩家数据。

SHA256：`DF5B6B61299B0899EE5E3002D11A89A88EF6D32D1D79EF6B69556CBAD3323A1F`。
