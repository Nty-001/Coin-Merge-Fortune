# Merge Stack Journey

当前正在维护的 Unity 换皮工程位于 **`Restoration/11_UnityReskin`**。

## 打开工程

1. 安装 Unity **2022.3.62f3c1**，并安装 Android Build Support（SDK、NDK、OpenJDK）。
2. 在 Unity Hub 中添加 `Restoration/11_UnityReskin` 文件夹。
3. 等待首次资源导入，打开 `Assets/Scenes/RecoveredLoading.unity`，点击 Play 从加载页进入游戏。

游戏名称：**Merge Stack Journey**  
Android 包名：**com.pizza.mergejourney**  
当前交付范围：**B 版收益版**。

工程包含 `Assets`、`Packages` 和 `ProjectSettings`。`Library` 等生成缓存、Android 工具链、构建产物、设备记录及签名密钥不上传。旧版本和恢复资料保留在其他 `Restoration` 子目录，日常修改使用上述换皮工程。

## Android 构建

项目内的 `Assets/Editor/AndroidApkBuild.cs` 是批处理构建入口：
`CoinMerge.Recovery.Editor.AndroidApkBuild.Run`。设置 `COINMERGE_APK_OUTPUT` 为目标 APK 的完整路径；可通过 `COINMERGE_ANDROID_TOOLCHAIN` 指定包含 `SDK`、`NDK`、`OpenJDK` 的本地工具链目录。建议在独立工程副本中执行。

当前配置为竖屏、IL2CPP、ARMv7 + ARM64、最低 Android API 22、目标 API 34。该入口生成本地测试签名 APK。商业广告及提现 SDK 保持模拟接口，保留游戏内流程。

最近的界面修改包括按原版规则进行宽度适配、安全区域避让，以及弹窗遮罩覆盖安全视口之外的屏幕边缘。验证记录见 `Restoration/11_UnityReskin/Design/DeviceAdaptation20260918/README.md`。
