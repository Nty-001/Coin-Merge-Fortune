# Unity 2022.3.62f3c1 基础框架

最新测试入口：[商业化 HTML 指南](../09_TestGuide/index.html)。GM 已增加提现任务边界、游戏时间 +12h/+24h、广告异常、真实合成/复活和下一次转盘指定工具；[本轮验证与使用说明](../07_Verification/GM_TEST_GUIDE.md)。停止 Play 并重新打开加载场景后使用更新后的 GM。

目录可交给 Unity Hub 作为基础工程打开。它是后续移植框架，不是完成了 1:1 行为和画面验证的游戏。

## 已提供

- Assets/Prefabs：63 个源场景/预制体对应的 Unity YAML 预制体，节点坐标、层级、尺寸、锚点、旋转和资源引用来自原数据。
- Assets/Scenes：原始两套玩法的 4 个静态场景图、MockFlow.unity、完整启动入口 RecoveredLoading.unity，以及收益版 RecoveredMain.unity 和基础版 RecoveredPackaged.unity。
- Assets/Art：源纹理、432 张独立图片、字体、音频和骨骼原始数据/独立 region。
- Assets/Config：默认配置、实际缓存归一化配置、代码默认值和硬币参数。
- Assets/Scripts/Core：数据 DTO 与 RecoveredNode 原始属性保留组件。
- Assets/Scripts/Gameplay：原生物理金币对象池、合成与投币队列、存档、失败判断、可编辑 GameBalance、原始收益规则和玩家状态。32 像素/米来自实际引擎，跨引擎轨迹尚未完成对比。
- Assets/Scripts/SDK：IAdFacade/IWithdrawalFacade 与本地 mock。广告成功、失败、取消、不可用以及提现条件、错误和幂等路径可测试，不联网、不执行真实付款。
- Assets/Scripts/UI/MockFlowPanel：MockFlow 场景的按钮驱动触发器。
- Assets/Editor/RecoveryBuilder：许可证激活后可在编辑器内用官方 PrefabUtility 重建并运行检查。
- Packages/manifest.json：仅 Unity 官方内置模块与 com.unity.ugui；没有第三方商业 SDK 程序集。

## 验证状态与未完成项

独立 C# 编译通过（引用指定编辑器的 UnityEngine/UnityEditor/官方 UGUI）。11 项纯 C# 规则和 mock 检查通过。生成的 YAML 结构、内部对象引用与 GUID 检查通过。
指定编辑器目前已可用。900 个原 JS 规则差分用例、9 项状态检查通过；原生主玩法场景通过 19 项实际 Play Mode 检查。导入及运行报告见 ../07_Verification。旧 unity_build.log 的许可证问题为历史状态。

RecoveredNode.originalComponents 保留所有原字段，但它不是原玩法脚本的 C# 实现。自定义玩法类，以及尚未等价实现的 Widget/Layout/EditBox/ProgressBar/部分遮罩/骨骼行为，见 native_yaml_unported_components.json。原始完整函数体在 ../02_Gameplay；这些行为仍需逐项移植、绑定与校验。
源 Scene 的序列化 _active=false 是待激活状态，生成框架时仅将场景根激活，原值仍保存在 originalNodeJson。

## 后续使用

1. 在指定版本 Unity 中打开此目录。
2. 打开 Assets/Scenes/RecoveredLoading.unity 后点 Play，按已保存版本进入对应加载页和玩法。RecoveredMain.unity / RecoveredPackaged.unity 可直接调试玩法；主游戏的设置、规则、广告和提现分支已接通，商业 SDK 使用本地 mock。
3. 新原生资产制作入口为 Coin Merge > Author native main gameplay assets；Play Mode 校验入口为 Validate native gameplay in Play Mode。制作操作会重写 Runtime 预制体及 RecoveredMain 场景，手工修改需先提交。旧基线生成器也会重写其目标，不要直接覆盖后续开发。
4. 按 module_body_inventory、field_values 和 component_migration 的映射逐项移植真实玩法；不要把框架的 mock 规则当作服务器真规则。
5. 对照原 MuMu 画面执行逐控件、动画和物理轨迹测试。当前未声称达到 1:1。

较早阶段差异清单见 ../07_Verification/native_gameplay_scope.md；后续状态以下方更新及根目录 DEVELOPMENT_STATUS.md 为准。MuMu 原应用未修改。

## 当前可运行入口（2026-09-15 更新）

Unity Hub 添加本目录后，打开 `Assets/Scenes/RecoveredLoading.unity`，从原加载页进入当前选择的玩法版本。`RecoveredMain.unity` 仍可直接运行以调试主玩法。右下 GM 弹窗可切换 A 基础版 / B 收益版及国家；应用会加载实际场景，清玩家存档保留版本选择。A/B 是本地测试名称，不等于已经证实原游戏的买量/自然量规则。

收益版主界面的设置、规则、顶部 Withdraw、2000 金币按钮及 11 个关联页面已接入；支持原金额/合成/视频/有效登录条件、账户输入、验证动画、阶段存档及关闭返回。Wheel 积分不足显示原提示。所有广告继续使用本地 mock，成功回调会累计观看次数；不执行真实提现。

主界面修复记录与剩余差异见 `../07_Verification/MAIN_MENU_RESTORATION.md`；分流证据见 `../07_Verification/VERSION_ROUTING_FINDINGS.md`。
本轮恢复了铃铛旁原随机滚动播报、统一修正文字圆周描边，并接入 A/B 原加载页。Withdraw 旁提示依原规则在达到首档门槛后隐藏。来源、时序、性能与测试边界见 `../07_Verification/HOME_VISUALS_AND_STARTUP.md`。
130 项菜单 Play Mode、952 组原函数差分、31 项主玩法回归和 74 项版本回归通过。不能据此宣称全部视觉及生命周期已 100% 一致。

如果编辑器仍显示旧运行实例，停止 Play 后重新打开 `Assets/Scenes/RecoveredMain.unity`。不需要清玩家存档。

2026-09-15 已在实际打开的正式编辑器中完成这次重新载入，并逐个点击设置、规则、两类提现、档位切换和 GM 验证。再次停止／Play 后入口仍正常。外部同步场景后，单纯重新 Play 可能继续使用内存里的旧场景；开发交付需要同时验证已打开的编辑器，详情见上述主界面修复记录。
