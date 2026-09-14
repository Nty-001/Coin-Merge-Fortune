# Coin Merge Fortune 还原资料与 Unity 迁移框架

**此交付提供可读原包资料、完整已取得玩法函数体、实际缓存配置、美术资源和 Unity 基础框架。尚未达到或验证“100% 完整复刻”。**

原包 com.mergecoin.cotune.tuneco 1.6 使用 Cocos Creator，不是 Unity IL2CPP。安装包中没有 libil2cpp.so 或 global-metadata.dat。

|目录|内容|
|---|---|
|01_Evidence|引擎、原 APK 与 native 库核验|
|02_Gameplay|内置 40 + 热更新 103 个完整 JS 玩法模块、37 个算法依赖；实际 libcocos2djs.so 全可执行段汇编与函数体索引|
|03_Configuration|原始表、设备缓存真值、原代码补齐后的生效值、逐叶字段真实值/语义/引用、默认与缓存差异|
|04_Assets|432 张独立图片、490 个原 native 文件、音频、字体、10 套骨骼动画源资料及 region 导出|
|05_PrefabModel|63 个源场景/预制体完整对象图、UUID 引用与静态预览|
|06_UnityFramework|63 个 Unity YAML 预制体、5 个场景、原生基础脚本、官方包依赖、本地广告/提现 facade/mock|
|07_Verification|完整度、反汇编覆盖率、结构/引用检查、原 JS mock 检查、C# 编译/逻辑检查及编辑器失败日志|
|Tools|可复现的拆包、解码、导出、生成、校验脚本|

优先打开 `index.html` 浏览模块、配置、美术与预制体。原始导出仍在上一级 export_20260914，未改写。模拟器应用未因本次还原任务而修改。

## 已验证

- 143 个玩法模块保留完整编译函数体，未用空方法或签名代替；广告与提现触发逻辑包含在内。
- 1,029 个 Cocos 资源解码，解析错误 0；2,025 个跨资源引用检查未发现缺失目标。
- 432 张独立图片导出无错误，裁切和布局元数据保留。
- libcocos2djs.so 全部可执行段的 16,881,296 字节均进入汇编/原字节输出；40,962 个函数符号可定位函数体。仍有 184,200 字节以 .byte 形式保留，不声称这些字节已全部解释为指令。
- 原 JS 广告请求/原生回调/成功与失败路径，以及提现请求构造、原始加密、历史查询和错误路径，通过本地隔离 mock 检查。
- Unity C# 源码独立编译通过，11 项 C# 逻辑检查通过。已修复 Unity YAML 序列化排版，68 个文件的字段值与备份保持一致；指定 Unity 编辑器实际导入及 63 个预制体、5 个场景的结构检查通过。此结果不等于主玩法或视觉已完成。

## 不能标成 100% 的具体原因

1. 在线配置 GET 需要将当前客户端 ID 发往游戏配置域名，自动审批拒绝，等待明确授权；已保留设备中的真实缓存和与默认表的 17 处差异。未虚构未取得的服务器值。
2. 部分字段语义尚未独立确认，CSV 明确标注，并给出实际值与使用代码位置。
3. Unity 中的自定义玩法、动态 UI、骨骼 mesh/clipping/deform 等尚未全部移植和绑定。完整源字段及函数体已保留，待移植项逐条列出。
4. 指定 Unity 编辑器现已可用；主玩法 19 项实际 Play Mode 检查通过，覆盖物理合成、存档、引导领奖、广告复活和失败重开。完整画面、主流程及跨引擎轨迹一致性仍未完成。历史日志中的许可证失败不代表当前状态。
5. native 汇编覆盖不等于原始 C++ 源码还原，也不能凭静态分析得出所有间接调用的运行时目标。

当前已制作原生可运行主场景 RecoveredMain.unity，尚未完成整个复刻游戏。完整规则差分与本轮实现边界见 07_Verification/native_gameplay_scope.md。框架按 Unity 原生组件与预制体目录组织，商业 SDK 实现排除，只保留玩法触发接口与可测试的本地 mock。

## 技术参考

序列化解析主要依据原包内引擎 deserialize-compiled 模块 254；公开接口参考 [Cocos 2.4 Details](https://docs.cocos.com/creator/2.4/api/en/classes/Details.html)。编辑器生成器使用 [Unity PrefabUtility.SaveAsPrefabAsset](https://docs.unity.cn/2021.2/Documentation/ScriptReference/PrefabUtility.SaveAsPrefabAsset.html)，并引用指定安装目录内的官方程序集做独立编译检查。
