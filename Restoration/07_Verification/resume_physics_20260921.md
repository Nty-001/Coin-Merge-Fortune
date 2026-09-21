# Android 前后台切换硬币物理修复（2026-09-21）

## 问题与修改

用户录屏约 10 秒和 14 秒切换应用，返回后上层硬币明显弹起。运行中的 RecoveredMain 使用 RecoveredGameSession，原实现每帧调用 Physics2D.Simulate(Time.deltaTime)，暂停回调仅存档。长帧直接作为单次物理步长，会影响接触求解的稳定性。

- 同时跟踪 pause 和 focus，两者都恢复后才允许棋盘继续更新。
- 生命周期切换时清除物理时间余量，丢弃恢复首帧的棋盘时间；保留刚体位置、速度、角度和休眠状态，不重建硬币。
- 棋盘更新与物理模拟使用固定小步；GameBalance 可调整，默认 1/60 秒、每帧最多 6 步。超额时间不积压到后续帧。
- 清除被打断的拖动和等待预览动画结束的投币请求，恢复后接受新的输入。
- 保留原始 GameScene.onGameHide 的存档和 windowsCointimes 重置行为，商业 SDK 继续使用现有 mock。

## 验证

Unity 2022.3.62f3c1 在隔离副本运行 ResumePhysicsValidation.Run：62 项检查通过，详见 resume_physics_20260921.txt。包含两种生命周期回调顺序、20 次反复暂停恢复、10 秒长帧的小步预算、运动中硬币速度保留、整堆硬币恢复后稳定、恢复后的投币和存档。

Android IL2CPP ARMv7 + ARM64 构建成功，0 errors，12 warnings。生成本地测试 APK：
`Restoration/Builds/Android/MergeStackJourney_ResumeFix_20260921.apk`

SHA256：`24B2823AFD7581FC28701B39D71BA1DFFEACFE72E26D2B77171DA9197DF82BEB`

MuMu Android 15：覆盖安装成功，未卸载应用或清除存档。使用正常投币准备棋盘，连续 3 次打开系统设置、停留 2 秒、返回游戏；900×1600 截图中的稳定硬币区域 (0,1050)-(900,1355) 三次均逐像素一致。恢复后再次投币仍正常。

模拟器截图、录屏解码帧、构建副本和日志保留在已忽略的 Builds 目录，不纳入公共提交。实测场景为本次正常投币形成的棋盘，并非对录屏中原始高堆存档的逐帧重放。
