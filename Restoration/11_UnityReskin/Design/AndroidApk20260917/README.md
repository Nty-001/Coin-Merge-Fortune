# B 版 Android 测试 APK

- 输出：Restoration/Builds/Android/CoinMergeFortune_B_20260917.apk
- 大小：64,525,256 字节（61.5 MiB）。
- SHA-256：AD43DDB87B57E433E96BE722C66F747CDE47B31ED5F7781D7CA66BC77492B70E
- Unity：2022.3.62f3c1；完整 BuildPipeline 成功，0 个错误、12 个警告。
- 包名：com.coinmergefortune.reskin；版本 1.0 / versionCode 1。
- ARM64 + ARMv7，Android 5.1 起，目标 API 34，竖屏。
- 默认 B 收益版 / US；保留 GM、广告与提现模拟接口。Android debug 签名，用于本地安装测试。
- 已验证：v1/v2 签名通过；模拟器安装和冷启动通过；进入 B 版主界面；滑动投币后分数增加，新手引导继续。
- 尚未验证：实体手机完整流程。模拟器有图形格式探测日志，既有物理 density 警告仍在；本次未改动玩法。

构建入口：Assets/Editor/AndroidApkBuild.cs，batchmode 下调用 CoinMerge.Recovery.Editor.AndroidApkBuild.Run。
必须传入 COINMERGE_APK_OUTPUT；可通过 COINMERGE_ANDROID_TOOLCHAIN 指定 SDK、NDK、OpenJDK 的父目录。
在独立构建副本执行，避免更改当前美术工程的 Android 参数。构建副本与工具位于 Restoration/Builds，均不纳入版本控制。
本机 Java 下载依赖需使用从 Windows 既有根证书库复制的 PKCS12 信任库；保持 HTTPS 验证开启。构建日志与模拟器截图仅保留本地 Builds/Android。
