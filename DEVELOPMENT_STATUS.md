# 当前开发状态

初始资料和 Unity 框架已备份到用户指定 GitHub 仓库：
`31ae71f36dd0a9ed0dd7925d6621ce5fb7ec4600`（main）。本地与远端提交号核验一致。

第一轮修复：Unity YAML 列表排版已修复，68 个文件的序列化字段逐项核对未改变。
指定 Unity 2022.3.62f3c1 实际导入并加载 63 个预制体和 5 个场景，结构校验全部通过。
验证入口：`CoinMerge.Recovery.Editor.RecoveredAssetValidation.Run`。

完整主流程、原生金币玩法、引导、奖励/提现业务面板、地区/AB GM 和视觉特效仍在移植范围内，不能标为 100% 复刻。
商业 SDK 保持 facade/mock。原始代码和配置仍是移植依据。

第二轮进展：增加原始数据字段兼容的玩家状态模型、可编辑 GameBalance ScriptableObject 和规则实现。
配置包含 11 种金币、11 组投币规则、6 组收益配置、26 个地区。C# 实现通过 900 个原 JS 计算结果的差分用例和 9 项状态检查。
规则已实现但尚未全部接到场景 UI；不能据此声称主流程已经可玩或视觉一致。

设备采集与私人存档仅保存在本机；公开仓库包含代码、资源、可公开配置、工具和检查记录。
231 MB 原生汇编通过 `Restoration/02_Gameplay/NativeCocosAssembly/text.asm.gz` 无损备份，解压 SHA-256 与原文相同。
