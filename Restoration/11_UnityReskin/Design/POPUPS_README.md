# 规则与设置弹窗换皮

直接打开 `11_UnityReskin`，从 `Assets/Scenes/RecoveredLoading.unity` 运行。主场景和 `Assets/Prefabs/Runtime/RecoveredMain.prefab` 已同步保存。

## 美术结构

| 界面 | 独立透明 PNG | 原生可交互内容 |
|---|---|---|
| Merge rules | 云朵玻璃面板、关闭、确认按钮、11 级筹码、筹码图集、路径圆点 | 标题/说明/OK 为 Text；关闭/确认仍为 Button；筹码使用原有 3.3333 秒 AnimationClip 的时序 |
| Settings | 云朵玻璃面板、红色关闭按钮、灰色关闭态/绿色开启态开关 | 标题/标签/协议文字为 Text；开关、关闭和协议入口仍为 Button |

PNG 使用 RGBA 真透明，不把棋盘格当作透明底。按钮使用 Simple Image + preserveAspect，矩形宽高比与对应 PNG 一致，弹窗动画仅等比缩放。面板中的云朵、齿轮、音乐图标和手机图标属于固定装饰。图片按原有 RecoveredMenuArt 机制按需加载，未加入运行时创建 UI 的代码。

英文使用项目内 Nunito 字体（许可证见 `Assets/Art/PopupFonts/OFL.txt`），保留原字体作为 CJK fallback；中文/日文内容仍通过原有本地化数据填充。参考图是栅格效果图，字体渲染及生成面板的细部不宣称像素级完全一致。

## 验证

- `RulesR1/Verification/play_mode.json`：真实 EventSystem 射线点击规则入口、X、OK；检查筹码、比例、原动画时长和英文/日文渲染。
- `SettingsR1/Verification/play_mode.json`：真实点击设置入口、音乐开关、震动开关、协议入口与返回、关闭、GM；检查存档和 AudioSource。
- `popup_scope_audit.json`：对照主界面 R2 交付提交，检查弹窗范围与原工程文件哈希。
- `POPUPS_REVIEW.html`：参考图、Unity 真实渲染及透明素材对照。

验证使用专用存档命名空间并在结束后清除。不会把测试图里的金额、地区或音乐关闭状态写入正式玩家存档。

## 制作记录

使用内置 image_gen 编辑已准备的空白参考面板：只移除需要独立交互的按钮和原筹码扇形，保留云朵、玻璃边框、面板分区和路径。提示要求透明背景、不改布局、不添加文字、不重绘指定筹码。原始生成结果保存在每个界面的 GeneratedPanel.png；这些是制作源文件，实际工程使用经过边缘裁切和 alpha 处理的 Resources 下 PNG。

Settings 的红色关闭和两个开关直接从用户图二按原像素裁切，Rules 关闭/确认从空白参考素材裁切；统一使用抗锯齿透明蒙版。规则筹码直接复用用户指定筹码表的 11 张已分离图片，并验证新上传筹码表与源表的解码像素完全一致。

制作脚本为 `RulesR1/prepare.cjs`、`SettingsR1/prepare.cjs`；Editor 菜单 `Coin Merge/Reskin` 可重新保存和验证。不要用旧全量 YAML 生成器覆盖后续资源。

测试结束后编辑器停留在加载场景。原 `06_UnityFramework` 未修改。
