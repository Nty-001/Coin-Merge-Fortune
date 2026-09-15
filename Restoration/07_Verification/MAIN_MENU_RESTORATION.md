# 主界面按钮与提现页面恢复记录（2026-09-15）

Unity 工程：`Restoration/06_UnityFramework`。运行入口：`Assets/Scenes/RecoveredMain.unity`。

本轮解决收益版主场景只有投币可操作、外围入口没有绑定运行逻辑的问题。原游戏为 Cocos；下面的来源均为 `02_Gameplay/HotUpdate/Modules` 内恢复的完整 JS 函数体，并非函数签名或 DLL 占位。

| 可见入口 | 已接入的原始路径 | 返回与条件 |
| --- | --- | --- |
| 设置齿轮 | SettingDialog → PrivacyPolicyView 的隐私/用户协议页面 | 音乐/音效、震动开关保存；文档可滚动，关闭回到设置 |
| 规则按钮 | mergeRuleDialog | 原币种说明骨骼动画；确认与关闭返回游戏 |
| 顶部 Withdraw | GameFakeWDDialog → 6 个 GameFakeWDItem | 金额、合成数、有效登录天数、视频数按原顺序判断；未满足提示条件，满足打开 GameRealWDActiveTips |
| 2000 金币图标 | GameRealWDDialog → 6 个 GameWithdrawItem | 每个档位独立保存条件阶段；未达标灰色按钮仅播放原 no_click 音效 |
| 金币提现继续 | GameRealWDAccount / BR / ID | US 邮箱、BR 账户/姓名/CPF、ID 等手机账户分支；保留原格式校验与已保存账户判断 |
| 账户确认 | GameRealTXYZ → GameRealWDTXTips / GameRealWDActiveTips | 原验证骨骼及提示出现次序；跳到首个未满足条件，或进入原最终提示 |
| Wheel（积分不足） | GameScene 的不足积分提示 | 显示原文案 Toast；达标自动弹出和领奖链路保留 |

US 首档现金提现要求 100 次最高币合成；金币提现的第一阶段要求 50 次。两者不能共用一套条件。`GameWithdrawItem.refreshEnoughBGSprite` 原函数体为空，所以没有擅自增加“余额不足”覆盖样式。

## 工程实现

- 静态层级来自恢复的原 Prefab，并保存到真实 `RecoveredMain.prefab` 及场景。六档列表为预制体中的实体节点，运行时不拼装静态 UI。
- 可见图形上的 Unity Button 由代码绑定。设置/规则/提现父节点里的原空点击区域没有继续作为伪按钮。
- 金币入口原生骨骼 Graphic 的 raycastTarget 曾在嵌套 prefab 保存时丢失。制作时展开骨骼 prefab 后保存实际字段，真实指针命中测试覆盖此问题。
- Unity InputField、ScrollRect、Image 进度条、原生 AnimationClip、AudioSource；没有加入商业 SDK 或第三方骨骼程序集。
- 大图按页面 Resources 路径加载，关闭页面清理图形引用；没有把全部新页面图片强引用到启动入口。未将“引用清理”当作纹理已经卸载的证明。
- 新增 BaseUI.pop 的 .35 秒 backOut 缩放；现金提现页按原 adaptLayout 的边距/顶部/底部/滚动区域计算。
- 原无 SpriteFrame 的空 Sprite 不绘制白块；原禁用的文字 Shadow/Outline 保持禁用。
- 成功的广告 mock 回调执行 `HWL.addadnum → PlayData.add_show_video` 对应计数并保存，失败/不可用不增加成功观看计数。
- SDK 沿用现有 facade/mock。当前 GameRealTXYZ 活跃函数走本地条件阶段与提示，不伪造支付成功、不扣除原函数未扣除的余额、不发起真实转账。

## 验证

- `recovered_menus_validation.json`：130 项 Unity Play Mode 检查。覆盖真实 EventSystem 指针命中、两套六档选择、滚动、音乐/震动存档、账户验证、原生验证动画完成、阶段持久化、两套不同门槛、BR/ID 页面与关闭后恢复玩法输入。
- `withdrawal_rules_comparison.json`：952 组原 JS 与 C# 对照（260 个账户校验、260 个真实金额格式、432 个六组配置阶段判断）。输入均为合成测试数据。
- `probe_withdrawal_rules.cjs`：离线执行完整原方法体，产物 `Assets/Config/Runtime/WithdrawalVectors.json` 保留函数体 SHA-256。修复了 C# `$` 与 JavaScript 末尾换行匹配不同的问题。
- `native_gameplay_validation.json`：31 项主玩法/引导/转盘/广告复活回归通过。
- `unity_asset_validation.json`：85 个 prefab/scene 结构检查通过，无缺失脚本。
- `version_variants_validation.json`：菜单接入后的 GM、实际内容切换和独立存档回归 74 项通过。
- `menus_*.png` 是本轮 Unity 运行截图，不是原 MuMu 截图，也不是逐像素一致性证明。

## 仍不能宣称 100% 的项目

- 商业 SDK 仍为 mock；流量归因与内容下发条件仍缺乏完整证据，MuMu 原应用没有被本轮修改。
- 当前缓存没有各国 `withdrawal_platform` 字段；BR/ID 使用原函数 Pagbank/DANA 默认值。TH/MY/VN/PH 原默认为 PayPal，但手机校验函数没有 PayPal 分支。保留失败判断，没有杜撰服务端平台列表或把任意输入当成功。远端平台表需进一步取得证据。
- 原 BR 序列化平台数组重复指向同一组件；当前默认单 Pagbank 页面可用，没有冒充已经恢复双平台远端配置。
- 原 GameRealTXYZ 只有继续文字的关闭事件，本轮没有把未绑定的装饰关闭图标当成可用入口；原 help/record/task 隐藏入口保持原当前条件。
- 字体系统替换、UGUI 描边与 Cocos 的栅格化差异、全部页面逐帧视觉、评级触发/滚动广播/最大币飞行等早先缺口仍需继续核对。现有检查不证明跨引擎物理轨迹或全部生命周期已 1:1。

## 复验入口

- 制作：`CoinMerge.Recovery.Editor.RecoveredMainMenusBuilder.Run`（显式重写主菜单资产）。
- 菜单 Play Mode：`CoinMerge.Recovery.Editor.RecoveredMenusValidation.Run`。
- 原函数对照：`CoinMerge.Recovery.Editor.WithdrawalRulesValidation.Run`。
- 所有 Play Mode 验证用临时独立存档命名空间，不清理用户游戏进度。若 Unity 当前仍处于旧 Play 实例，先停止 Play，再重新打开 RecoveredMain 场景运行。
