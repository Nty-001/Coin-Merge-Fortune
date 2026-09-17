# 2026-09-17 视觉审计

打开 `index.html`。离线可用，14 项差异支持搜索、按优先级/页面筛选、点击查看原图。

本次没有修改运行逻辑、预制体或美术。`Assets/Editor/VisualAudit*.cs` 为独立取样工具，使用专用可清除的测试存档，并在结束后回到加载场景。

- `Current/play_mode.json`：US 前台完整流程与 A 版页面取样，725 次文本检查通过。文本框互相重叠仍由人工发现，不应将此结果解释为视觉无误。
- `Startup/startup_after.json`：两次 B 版、一次 A 版启动，首个可见进度均为 0%。
- `Effects/merge_feedback_validation.json`：合成、连击及 15 秒闲置引导的 40 项检查。
- `multi_locale_stress.json`：连续遍历多国时动态字体贴图容量不足的失败证据。
- `audit_summary.json`：结构化差异项与修正方向。
- `Current/*.json`：对应截图的活跃图片/字体/文本清单。

取样请求分别为项目 `Temp/VisualAuditStartup.request`、`Temp/VisualAuditPages.request`、`Temp/VisualAuditEffects.request`。页面请求默认只跑 US 全流程和 A 版取样；内容包含 `all-locales` 才连续遍历 26 国。需要 Unity 保持前台，避免失焦暂停动画造成误判。

`Original/` 是本机 MuMu 原游戏窗口截图，按仓库规则不提交。此机器上报告包含原机对照；在新机器检出后，需要重新采集该目录，才能查看原机截图。版本化的当前画面均来自专用测试存档。效果截图中摆币/时间推进是受控测试，不表示玩家实际堆叠状态。

报告覆盖状态与未覆盖内容见 HTML 最后一节。原版长周期提现任务、广告 SDK、声音和所有动画逐帧比对没有在本轮完成；批准的换皮设计与需修复差异分开记录。

生成：`python build_report.py`。校验：`python validate_report.py`。
