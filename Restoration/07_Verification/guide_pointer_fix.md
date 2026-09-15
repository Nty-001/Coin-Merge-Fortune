# 引导第 3、4 步无法点击

原始 `GuideDialog.onLoad/showGuide` 使用全屏 clickArea 推进 1→2、3→4、4→9999。
恢复的原始预制体中，第 3 步的 money（节点 6）以及第 4 步的 btn1（节点 14）均为 inactive。
先前 Unity 实现将第 3 步 Button 绑定到 money 的子节点 18，第 4 步绑定到 14，无法被真实指针命中。
之前直接调用 onClick.Invoke 的检查没有经过 GraphicRaycaster，因此漏掉了此问题。

修复后，标准 Button 分别绑定在实际可见卡片（节点 27、32、2）和现有可见黑色遮罩（节点 7）上。
所有装饰 Graphic 不参与指针命中。无需新增透明点击面或运行时创建 UI；事件由代码绑定。
第 0 步不显示遮罩，仍保留原有投币引导行为。

验证使用 Unity 2022.3.62f3c1 真正 Play Mode，在 750×1624 竖屏渲染面上，经 EventSystem.RaycastAll 取得最上层命中对象，
验证该对象对应预期 Button 后发送标准 pointerDown、pointerUp、pointerClick 事件。
覆盖第 1 步卡片、第 3 步卡片/遮罩、第 4 步卡片/遮罩，检查引导最终进入 9999；完整原生玩法回归 29 项通过。
这是运行时 UI 命中验证，不是物理鼠标硬件自动化；截图保存在 native_guide_step3.png 和 native_guide_step4.png。

修复同时写入 Runtime/RecoveredMain.prefab 和 Scenes/RecoveredMain.unity。
已运行的旧场景需退出 Play 后重新打开 RecoveredMain 场景再运行。原有玩家存档保持不变。
