# 商业化测试指南与 GM（2026-09-15）

交付入口：[离线 HTML 测试指南](../09_TestGuide/index.html)。包含奖励时间轴、广告回调矩阵、按国家/档位切换的提现任务图、活跃日模拟、GM 截图和 38 项可记录结果的人工用例。无需服务器或安装依赖。

## Unity 使用

正式工程：`Restoration/06_UnityFramework`。停止 Play，重新打开 `Assets/Scenes/RecoveredLoading.unity` 再运行，右下角 GM → B 收益版 / US。重新打开场景是为了载入已更新的序列化按钮引用，不需要清存档。

GM 保持非全屏弹窗，三个页签为版本、提现/时间、事件/广告。新增 40 个代码绑定的标准 Button 操作：

- 选择六档提现产品与五项任务，准备差 1 / 刚好达标，再走实际申请与验证动画。
- 当前存档游戏时间 +12h / +24h、恢复偏移；跨日后仍要累计五次最高金币合成才增加有效天数。
- 设置现金、最高币、成功广告数，清测试账户、跳过引导，直接进入两类提现页面。
- 准备第 10/16/22 次真实投币；切换 Mock 成功、取消、无填充、失败；触发真实最高币合成与失败复活。
- 指定下一次正常转盘结果、准备差 1 分；覆盖仅消耗一次，长期概率表不修改。

准备操作会替换当前测试存档相关计数；时间偏移随该存档保存，不修改系统时钟，也不快放动画、广告或防重复点击计时。直接预览奖励会正常结算，但不能代替广告链测试。

## 验证与本轮修复

- [gm_workflow_validation.json](gm_workflow_validation.json)：57 项 Unity Play Mode 检查通过，包括五项任务的差 1 → 达标 → 账户验证 → 下一阶段、跨日第五次合成、保存重载、1_A 四种结果、2_A 无填充/错误回调、3_A 失败重试和成功复活、真实最高币合成及转盘覆盖只生效一次。
- 检查实际 EventSystem 指针命中的标准 Button，不以单独调用 onClick 代替可点击性验证。测试发现 FailDialog 误绑停用的 anim/videobtn，并将可见复活按钮误当关闭按钮；已按源节点 7/13/37 修复，同时恢复活动布局的文字和最高币累计显示。
- [html_guide_validation.json](html_guide_validation.json)：15 项离线源代码检查通过，含完整脚本语法、内嵌报告新鲜度、38 个用例、72 组地区组/档位/路线配置和活跃日模拟逻辑。
- HTML 的本地浏览器打开操作被浏览器安全策略阻止；没有绕过策略，也没有声称完成浏览器渲染验收。GM 和复活截图来自 Unity 实际渲染。
- 使用独立 `08_ValidationUnity` 工程与 `coinmerge.gm.workflow.disposable` 存档；测试后清除测试命名空间。正式场景继续保留 `coinmerge.recovered.v1`，未运行自动测试修改用户玩家状态。

## 证据边界

当前主链并没有启用 40/60 秒定时插屏；回前台 `pasttime` 返回 -1。有效天数不是简单等待 24h。提现任务 2 和任务 5 共用成功广告累计数，任务 5 真值为 200000。最终 Active Tips 不是支付到账。

HTML 明确列出首奖评分链、复活奖励标题、1_A 未发起时计数差异，以及 SDK/远程任务墙响应缺失；未把这些标成已完成。商业 SDK 保持 Mock，不联网执行广告投放或付款。

## 重现本轮检查

仅在独立验证工程运行 Unity `-batchmode -executeMethod CoinMerge.Recovery.Editor.GmWorkflowValidation.Run`（不加 `-quit`；测试完成后自行退出）。不要在正式编辑器的当前场景直接跑它。

HTML 重新生成：`python Restoration/Tools/build_test_guide.py`；离线校验：`node Restoration/Tools/validate_test_guide.cjs`。生成器直接读取已恢复配置、当前测试报告和验证截图。
