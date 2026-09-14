# Unity 2022.3.62f3c1 基础框架

目录可交给 Unity Hub 作为基础工程打开。它是后续移植框架，不是完成了 1:1 行为和画面验证的游戏。

## 已提供

- Assets/Prefabs：63 个源场景/预制体对应的 Unity YAML 预制体，节点坐标、层级、尺寸、锚点、旋转和资源引用来自原数据。
- Assets/Scenes：原始两套玩法的 4 个静态场景图、MockFlow.unity，以及当前原生主玩法入口 RecoveredMain.unity。
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
2. 打开 Assets/Scenes/RecoveredMain.unity 运行当前已接通的金币、引导与复活链路；完整广告/提现 mock 演示仍在 MockFlow.unity。
3. 新原生资产制作入口为 Coin Merge > Author native main gameplay assets；Play Mode 校验入口为 Validate native gameplay in Play Mode。制作操作会重写 Runtime 预制体及 RecoveredMain 场景，手工修改需先提交。旧基线生成器也会重写其目标，不要直接覆盖后续开发。
4. 按 module_body_inventory、field_values 和 component_migration 的映射逐项移植真实玩法；不要把框架的 mock 规则当作服务器真规则。
5. 对照原 MuMu 画面执行逐控件、动画和物理轨迹测试。当前未声称达到 1:1。

完整差异清单见 ../07_Verification/native_gameplay_scope.md。地区/版本的独立持久化已实现，但完整 GM 界面和自然量/收益场景切换尚未接通，也未修改 MuMu 原应用。
