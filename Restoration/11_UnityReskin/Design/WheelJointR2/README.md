# 绿色球头接缝修复

目标工程：`Restoration/11_UnityReskin`。

原 `Knob.png` 底部带有一小段旧银色杆头，和独立的 `Shaft.png` 重叠，造成连接处露边。本次清除该残留，让独立杆身延伸至完整绿色球头后方。

- 使用内置 imagegen 清理球头切图，再缩放回原 144 × 145 RGBA 画布。
- 保留资源 GUID、透明背景、预制体尺寸、枢轴、动画曲线与抽奖触发时序。
- `BeforeKnob.png` 为修复前图片；`GeneratedKnob.png` 为生成原图；`ApprovedKnob.png` 为实际替换图。
- `prepare.cjs` 将替换图写入 `Assets/Resources/WheelReskin/Knob.png` 并更新资源清单。R1 脚本也改为复用干净切图。

## 验证

Unity 2022.3.62f3c1 实际 Play 验证通过，结果见 `Verification/play_mode.json`。检查 101 个原动画采样点，并查看 6 个姿势的渲染接缝特写；球头保持等比，下压和回弹连接连续。两次完整抽奖及奖励领取、2000 筹码庆祝流程通过。

截图来自 Unity Camera 的 RenderTexture，未修饰。使用独立测试存档。本次未进行 Android 设备安装验证。

打开 `REVIEW.html` 查看切图前后对照和姿势特写。可通过 `Temp/PrizeAnimationReskinValidate.request` 触发仅验证流程，不重新生成预制体。

## 生成记录

工具：内置 imagegen。参考输入为修复前 `Knob.png`。

提示要求：只移除绿色球体底部残留的银灰金属短杆；尽量保留绿色球体、白色高光、轮廓大小和位置；底部为平滑完整的绿色弧线；不添加杆身、轴座、边框、背景、棋盘格或外部阴影；输出真实透明背景和抗锯齿边缘。生成后检查并缩放至原资源画布。
