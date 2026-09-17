# 加载页换肤验收

应用工程：`Restoration/11_UnityReskin`。Unity 入口场景：`Assets/Scenes/RecoveredLoading.unity`。

- 标题、筹码、礼盒和彩虹直接使用用户图二，原始像素保留在 `Assets/Resources/LoadingReskin/Illustration.png`。
- 进度条外框和填充拆为透明 PNG。两张图片按同一个比例缩放；外框 RectTransform 固定，进度通过 Unity Image 的 Horizontal Filled 裁剪显示，未按进度缩放图片。
- 保留 `RecoveredLoadingView.SetProgress`、百分比、原有 `loading...` / `Loading...` 文字，以及 `RecoveredStartup` 的分流与目标场景；显示进度使用下面的新时序。
- 941×1672 按原图显示。长屏底部使用原图末端云朵等比例镜像延展，主体画面和进度条不拉伸。
- 两套真实资源预制体：`Assets/Resources/Startup/RewardedLoading.prefab`、`PackagedLoading.prefab`。没有新增第三方程序集或运行时 UI 构建逻辑。

验证使用一次性存档命名空间，不覆盖玩家存档。`Verification/play_mode.json` 记录两条真实启动链路和 24 张渲染截图的检查结果；`REVIEW.html` 可选择版本、屏幕比例、进度查看实际效果。

## 启动进度时序修复

本地初始化之前在相邻帧直接设置 0%、70%、85%、100%，跳过了玩家应看到的中间过程。原版存在异步网络与资源阶段，副本的本地 facade 很快就完成相应阶段。

当前把目标进度与显示进度分开：首次 Canvas 渲染显示 0%，可见停留参数为 0.2 秒，随后以 1.8 秒走完整条的速度追赶目标。单帧显示时间最多计入 0.05 秒，避免资源加载卡帧造成大跳变。100% 只在场景确实加载完成后显示，停留 0.3 秒后进入对应游戏场景。以上参数保存在加载场景 Inspector，Editor 与设备使用同一运行代码。

`StartupProgressValidation` 观察真实 Canvas 渲染帧，不手动设定进度；覆盖收益版连续两次 Play、基础版一次 Play。修复前后逐帧记录为 `Verification/startup_before.json`、`startup_after.json`，时间曲线见 `STARTUP_TIMING.html`。原有 24 张静态截图只证明外观与几何正确，不证明首次显示时序。

## 素材与工具

进度条通过内置 `image_gen` 生成，源文件 `GeneratedBars.png`。`prepare.cjs` 只负责分离两个透明元素、移除边缘杂点和裁取原图底部云朵，不改变进度条宽高比。`materials.json` 记录尺寸、透明像素与 SHA-256。

生成提示词：

> Create a production Unity loading progress bar sprite sheet, matching the supplied reference's polished cheerful sky blue, white glossy glass and golden yellow 3D casual-game style. Reference image is ONLY a style/color reference; do not reproduce the poster. Output exactly TWO standalone horizontal capsule-shaped elements, each with aspect ratio approximately 10:1, centered, separated vertically by ample fully transparent spacing. TOP: complete empty loading track with glossy cyan blue outer rim, a thin white inner glint, deep royal blue inset interior, crisp smooth rounded ends; width about 1300 pixels and height 130 pixels. BOTTOM: standalone luminous golden yellow progress fill capsule with subtle orange bottom shading and clean white upper highlight, width about 1260 pixels and height 88 pixels; no outer track attached. Front orthographic view, perfectly straight horizontal bars, no perspective or deformation. Actual transparent RGBA background around and between elements, no checkerboard pattern, no text, no numbers, no logo, no icons, no sparkles outside edges, no drop shadow halo. High quality antialiased smooth edges suitable for Unity native Image, clean evenly rounded capsules. Layout in a wide landscape canvas; generous transparent margins around each asset.
