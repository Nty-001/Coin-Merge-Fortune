# B 版视觉修复验收

Unity 项目：`Restoration/11_UnityReskin`。直接查看 `index.html`。

本轮完成审计 #02、#04、#05、#06、#07、#08、#09、#11、#14。仅修改收益版 RecoveredMain 的场景和预制体。Packaged/A 场景、预制体、加载素材及业务逻辑均未修改。

## 验证

- Unity 2022.3.62f3c1 Play Mode，独立临时存档。
- 连续切换全部 26 个国家；最终 10,080 次文字边界检查通过。
- 收益版页面、奖励、引导、GM、六档金额、账户提交、验证动画、下一任务、抽奖领取、评分按钮通过。
- 验证状态标题与说明在动画期间不重叠。
- 两份协议为原生 Text 段落；顶部、中段、底部均检查可读及可滚动。
- 字体容量错误和运行异常为 0；`After/play_mode.json` 是最终结果。
- 曾因人工停止 Play 中断一次检查；未作为最终结果使用。之后完整重跑通过。

## 实现

- `RecoveredTextFit.boundedRasterization` 仅在 B 版开启。一般文字使用 40 像素、大字使用 64 像素固定光栅字号；测量和最终渲染复用相同字形。协议正文固定 28 像素。适配通过布局宽度与几何缩放完成，避免每个整数字号都往字体贴图添加字形。
- 沿用 UGUI Text 的生成方式，保留所有顶点，包括最后一个字符；先生成文字，再应用圆形描边。外框不随语言伸缩。
- 绿色按钮白字蓝描边，包括表单运行时启用状态。
- 验证徽章读取原生骨骼动画的灰/橙/绿状态透明度和旋转；仅替换绘制层，不改变验证事件、时长、任务推进或 SDK mock。
- 协议基于原有 6 张文字图片转录，修正 OCR 字符与图片排版断字、更新产品名；未新增条款。文本位于 `Assets/Resources/BVisualFix/Privacy.txt` 和 `Terms.txt`。两份正文保持源内容语言（英语）。

## AI 素材

模式：内置 imagegen。已保存到项目 `Assets/Resources/BVisualFix/`：

- `StarSelected.png`、`StarEmpty.png`：384 × 384，透明 alpha。
- `StatusComplete.png`、`StatusWaiting.png`：384 × 384，透明 alpha。

先生成双图透明素材页，按两半分别裁切并等比例缩小，未拉伸形状。导出程序：`export_stars.cjs`。

星星最终提示词：

> Use case: stylized-concept. Asset type: transparent PNG sprite sheet for the Unity game Merge Stack Journey. Create exactly TWO rating star UI sprites, side by side, centered separately in the left and right halves of a wide canvas. Left: selected five-point star, glossy warm gold and lemon yellow, beveled rounded inflated toy shape, white specular highlight upper left, small orange rim and soft blue drop shadow. Right: unselected five-point star of EXACTLY the same silhouette, scale and front-facing orientation, glossy icy pale blue/silver with cyan-blue rim, upper-left white highlight, soft blue shadow. Match a cheerful premium casual mobile game with glossy sky blue glass panels and bright green candy buttons. Stars should be friendly and thick, clean rounded points, no faces, no letters, no numerals, no confetti, no panel, no other objects. Each star fits within its half with at least 12% transparent margins. Entire background must have genuine transparent alpha, never checkerboard. Smooth antialiased edges; no matte halo, no jagged fringe. Uniform scale and undistorted shapes. Save high-quality transparent artwork suitable for splitting into the two standalone sprites.

验证徽章最终提示词：

> Use case: stylized-concept. Asset type: two transparent UI status icons for Merge Stack Journey, matching glossy sky-blue glass panels and green candy buttons. A wide transparent sprite sheet, exactly two separate icons at equal scale, one centered in each half, each with 15% clear transparent margin. LEFT icon: circular puffy glossy green badge with a crisp white check mark and thin icy cyan glass bevel rim, white upper-left specular glint. RIGHT icon: circular puffy icy blue glass badge, with a centered white hourglass symbol with blue outline, a clean elegant bright cyan rim and white upper-left glint. Both frontal, circular, no perspective or distortion, soft small blue shadow only within icon bounds. Cheerful premium casual mobile game UI, smooth clean antialiased outline, high clarity at 64px. Genuine transparent alpha background, no checkerboard or solid matte, no text, no additional elements or connecting shadows. Use same outer silhouette and dimensions for both icons.

## 复验入口

在 Unity 打开本副本，停止 Play 后写入 `Temp/BVisualFixValidation.request`，内容为 `all-locales`。脚本会用临时存档执行 B 版检查，结束后清理该存档并回到 RecoveredLoading 场景。

`BVisualFixAuthor` 是定向编辑器制作工具，保存真实 Prefab/Scene；无需在游戏运行时执行。请勿用早期整包生成器覆盖后续美术。
