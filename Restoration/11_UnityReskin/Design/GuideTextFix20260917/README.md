# 收益版新手引导叠字修复

问题是布局重叠，不是字符编码或字体贴图不足。`step4/content/bg/step2Label1`（说明）与 `step2Label2`（Start）都被拉伸为覆盖整个卡片，两段文字在中央重叠，说明也覆盖了左侧筹码。

已在真实 Prefab 和 Scene 中恢复三个独立区域：左侧 2000 筹码、右侧说明、右下角 Start；同时移开会遮住 Start 的白色小手。卡片外框、筹码尺寸和原有 Button 事件保持。B 版制作工具再次执行时也会保留该布局。

原始布局依据：`Restoration/05_PrefabModel/HotUpdate/Decoded/972e4d70-4ec4-4d93-bd4b-e446ece8f428.json` 中节点 37/38，说明与 Start 原本使用独立文本框。

验证：Unity 2022.3.62f3c1 Play Mode，26 个国家 × 2 种竖屏比例（1080×2340、941×1672）× 引导步骤 1/3/4，共 156 项通过。检查文字互相遮挡、筹码遮挡、白手遮挡和卡片边界；原生 Button 的步骤 3 → 4 → 完成链路通过。临时存档与用户存档隔离，结束后清理。

此前文字检查只覆盖各自文本框边界，没有捕捉两个文本框之间的重叠；本轮新增了实际渲染范围交叠检查。

- 修复前：`Before.png`
- 修复后：`US_step4_941.png`、`RU_step4_941.png`、`JP_step4_941.png`
- 相邻引导：`US_step1_941.png`、`US_step3_941.png`
- 最终机器验证结果：`result.json`

复验：Unity 停止 Play、脚本导入完成后，执行菜单 `Coin Merge / Reskin / Repair and validate guide text layout`。只定向制作 B 版引导布局，使用临时存档复验，最后回到加载场景。无需清除用户存档。
