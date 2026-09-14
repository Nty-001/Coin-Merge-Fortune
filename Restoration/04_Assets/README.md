# 可复用美术与音频

- `Packaged/Sprites` 与 `HotUpdate/Sprites`：432 张独立 PNG，保留透明背景，按原 offset/originalSize 还原画布。
- `sprites.json`：UUID、逻辑名称、原图集、rect、rotated、offset、originalSize、capInsets，可用于重新打图集和九宫格设置。
- `Native`：490 个原 native 资源文件，保留原始字节。
- `Audio`、`Fonts`：按可恢复的逻辑路径命名的音频和字体，含私有缓存中解压出的 FZY4JW.ttf。
- `ContactSheets`：独立图片总览。
- `SkeletalReusable`：10 套骨骼数据的可读 skeleton.json、atlas、纹理、182 张图集 region 图片和分动画时间轴。
- `AnimationSource`：3 个 Cocos 动画片段的原始曲线数据。

源骨骼动画包含 mesh、clipping、deform。它们的所有源数据已保留，但尚未完成使用 Unity 原生 SkinnedMeshRenderer/Animator 的逐项等价实现。框架不依赖 Spine 等第三方运行时；不要把源数据完整导出误认为动画移植已经完成。
