# 功能差异核对：Unity 换皮工程与原游戏

日期：2026-09-21。对象：`Restoration/11_UnityReskin`，重点为用户截图对应的收益版 `RecoveredMain`。依据为恢复的完整 Cocos JS 函数体、原始 CoinItem 预制体引用、当前 C# 与 Prefab/Scene；本轮在 MuMu 实测新增虚线。其他条目为代码核对结论，不冒充逐项实机复现。基础版只做入口与实现范围检查，未穷尽全部关卡、国家和服务器响应。

**本轮只补齐按住对齐虚线。以下差异均未修改，待用户决定。**

## 已完成：按住对齐虚线

- 原规则来自 `GameScene.onTouchStart/onTouchMove/updatePreviewPosition/drawGuideLine/dropCoin`（原文件约 1704–1805 行）。按下即可显示，不额外增加长按延迟；拖动跟随预览币；松手投币隐藏。
- 半径 5 像素、间距 30 像素、白色 alpha=180；从预览币底部向下绘制，按圆形接触关系计算落点，遇到下方硬币就缩短，不简单贯穿地面。
- 实现使用真实世界空间 SpriteRenderer 节点，保存在 RecoveredMain.prefab 与 RecoveredMain.unity。Dot.png、颜色、大小、间距均可在资源/Inspector 中调整；无运行时新建虚线节点。
- Unity 专项 19 项检查通过；MuMu 按住/松手截图验证通过。APK 构建成功，0 errors、12 warnings，已覆盖安装。
- APK：`Restoration/Builds/Android/MergeStackJourney_DropGuide_20260921.apk`。
- SHA256：`8005343F04471B4B1CC4F796B11024535F5A82010E200672C946755E8AE2AA37`。
- 专项记录：`drop_guide_validation_20260921.txt`。设备截图只保留在忽略的 Builds/Android 目录。

## 建议优先由用户决定的体验差异

| 编号 | 功能 | 原游戏代码行为 | 当前 Unity 行为及影响 |
|---|---|---|---|
| 1 | 震动反馈 | CoinItem 的普通碰撞效果与合成都在 open_vibrate 开启时调用 N_vibrate。 | 设置页会保存并显示震动开关，但运行代码没有震动调用。开关实际不产生震动，需真机验收，模拟器不能代替触感测试。 |
| 2 | 投币音效 | GameScene.dropCoin 在有效投币时播放 sfx_bom。 | NativeMergeBoard.Dropped 只有会话 OnDrop 订阅；该流程处理奖励/引导，没有播放投币音效。已有菜单与合成音效不能替代这一触发。 |
| 3 | 普通碰撞效果 | CoinItem.onBeginContact 在首次普通接触时执行 playCollisionEffect；原 CoinItem 预制体的 collisionEffect 引用非空。 | NativeMergeCoin 的碰撞回调只维护物理状态并尝试合成。当前爆发/星星来自合成事件，普通未合成的碰撞缺少原有效果。 |
| 4 | 复活清除过程 | removeTopTwoThirdCoins 实际选取约三分之一、保护最高面值；选中的硬币按 0.04 秒错峰、0.12 秒缩小淡出，全部结束后恢复投币。 | NativeMergeBoard.Revive 的数量与保护规则一致，但立即回收硬币并立即恢复。缺少清除动画，复活后可以更早投币。 |

证据定位：

- 1：`02_Gameplay/HotUpdate/Modules/CoinItem.js:376–405`；`11_UnityReskin/Assets/Scripts/UI/RecoveredMainMenus.cs:66`；`Assets/Scripts/Gameplay/NativeMergeCoin.cs:84`；`NativeMergeBoard.CompleteMerge`。
- 2：`02_Gameplay/HotUpdate/Modules/GameScene.js:1788`；`11_UnityReskin/Assets/Scripts/Gameplay/RecoveredGameSession.cs:121`。
- 3：`02_Gameplay/HotUpdate/Modules/CoinItem.js:290–383`；`05_PrefabModel/HotUpdate/Decoded/8450e79a-6034-4f88-8bb0-c0e00e2d31c1.json:156`；`NativeMergeCoin.OnCollisionEnter2D`；`RecoveredMergeFeedback.OnMerge`。
- 4：`02_Gameplay/HotUpdate/Modules/GameScene.js:662–739`；`11_UnityReskin/Assets/Scripts/Gameplay/NativeMergeBoard.cs:229`。

## 条件分支与此前保留的本地模拟差异

这些差异不等同于本轮发现的普通 gameplay bug。仓库明确要求商业 SDK 保持 mock，本轮继续遵守。

| 编号 | 功能 | 原游戏代码行为 | 当前 Unity 行为 / 判定边界 |
|---|---|---|---|
| 5 | 广告等待、失败与取消 | HWLshowAd 通过原生 SDK 启动，维护 adstart、暂停/恢复音乐，等待回调；可能拒绝或失败。 | MockSdkFacade 默认立即 Completed，可通过测试入口选择其他结果，但无真实广告播放过程与等待期。原游戏内奖励分支应保留；这属于既定 mock 边界。 |
| 6 | 评分后跳商店 | ScoreDialog 对零基索引 >=3（4–5 星）调用 gotoMarket。 | 相同星级条件调用 OpenMarket，但只记录 market.request:mock，不跳转真实商店。属于现有 mock 行为。 |
| 7 | 国家、配置与分流来源 | LoadingScene 请求国家与设备信息，GameData 使用缓存/服务器/本地表及超时回退；原 cohort 读取 clientId。 | RecoveredStartup 从本地保存的 Profile/GameBalance 读取；VersionRouting 的自动测试分流采用本地随机桶，可由 GM 设置。新安装默认 US/B；不会随服务器配置或真实地区自动变化。测试政策差异，不建议擅自接入旧服务。 |
| 8 | 版本更新提示 | LoadingScene.checkupdate 在当前版本命中配置列表时显示可选更新或强制更新；强制更新阻止继续进入。 | 启动流程没有相应版本检查与更新弹窗。属于明确的条件分支缺失；本轮未证明原游戏当前配置会触发更新提示。 |

证据定位：`02_Gameplay/HotUpdate/Modules/HWL_TStool.js:85`、`ScoreDialog.js:setCLosePage`、`LoadingScene.js:65–160,315–336`、`UpdateDialog.js:show`；Unity 对应 `SDK/SdkFacade.cs:30–34`、`UI/RecoveredRatingView.cs:28`、`Gameplay/RecoveredStartup.cs:25`、`Gameplay/VersionRouting.cs:16`。

## 没有误报为缺失的内容

- **回前台广告 7_A**：原 HWL_TStool.pasttime 在恢复的完整函数体中恒返回 -1，这个条件目前不会触发。不把工程没有自动弹该广告列为功能 bug。
- **复活移除数量**：原方法名虽然叫 removeTopTwoThirdCoins，实际是 ceil(数量/3)，并保护最高面值；Unity 的数量规则与它一致。
- **最高币与提现提示**：当前已有合成最高币、飞行动画、角标、评分触发和提现条件页。不能把已有逻辑概括为未实现；本轮也没有证据证明原流程满足条件后必定实际付款。
- **普通合成、出币队列与存档**：保留了对应流程；此前的前后台硬币修复仍在。跨引擎的随机碰撞轨迹不能据此宣称逐帧完全一致。
- 换皮后的背景、图标、字体以及现有 GM/广告调试提示不是这份功能差异清单中的视觉缺陷。

## 仍需专门场景验证的范围

- 原合并接触回调会立即禁用本次接触，Unity 主要标记待合并并在下一固定小步回收；是否在高堆连续合成时产生可见手感差异，需要对同一布局实测，不直接定为已复现 bug。
- 完整基础版关卡、所有国家账号输入组合、任务全部完成后的服务器响应，以及长期跨日/断网/进程被系统杀死等场景，不能凭本轮静态核对宣布完全一致。

建议先决定是否补齐 1–4；5–8 单独决定产品需求与服务边界。上述条目均保持原状。
