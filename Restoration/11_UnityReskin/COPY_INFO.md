# 换皮用独立副本

复制来源：`../06_UnityFramework`，复制日期：2026-09-16T20:14:46。

用 Unity Hub 添加本目录，使用 Unity 2022.3.62f3c1。启动入口为 `Assets/Scenes/RecoveredLoading.unity`，主玩法为 `Assets/Scenes/RecoveredMain.unity`。

已完整复制 Assets、Packages、ProjectSettings 与原说明文件，逐文件 SHA-256 校验通过。未复制可重建的 Library、Logs、Temp、UserSettings。副本 productName 为 CoinMergeFortune_Reskin，用于隔离测试存档。

已应用用户确认的 HOME_ART_ONLY_R2：主界面天空背景、玻璃面板、按钮及 11 级筹码。保留原来的布局、字体、交互区域、玩法和存档逻辑；没有增加白色落点圆点。主场景与主预制体各仅替换 13 个 Image.sprite 引用，另替换筹码 PNG 及三个共享筹码图集。运行时代码、已有导入设置没有变化。

Unity 2022.3.62f3c1 导入、编译和 Play Mode 43 项检查通过；验证了正常加载进入主界面、设置、规则、两类提现、Wheel 条件提示和 GM 弹窗按钮。真实渲染截图及检查记录位于 `Design/HomeR1/Verification`，测试金额和硬币摆放仅为隔离测试数据，不写入正式存档。原工程 4167 个文件的 SHA-256 仍与复制时一致。

后续换皮请只修改本目录；原工程仍保留在 `../06_UnityFramework`。复制记录见 COPY_MANIFEST.json。

2026-09-16 后续已按用户提供的参考图替换 Merge rules 和 Settings 两个弹窗。新增透明面板、独立按钮和开关；规则页使用指定 11 级筹码。仅这两个弹窗调整布局和字体，并修正弹窗与 GM 的 Canvas 排序；主界面 R2 保持不变。制作和验证说明见 `Design/POPUPS_README.md`，画面对照见 `Design/POPUPS_REVIEW.html`。
