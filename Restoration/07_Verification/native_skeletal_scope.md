# 原生骨骼转换范围与验证

输入：`04_Assets/HotUpdate/SkeletalSource` 的 10 套原始 JSON、图集和 PNG。
输出：`05_PrefabModel/NativeSkeletal` 保留可读 authoring 数据；Unity 的 `Resources/Skeletal` 保存数据资产、图集、14 段 AnimationClip；
`Prefabs/Runtime/Skeletal` 保存 10 个原生预制体。没有把第三方骨骼 DLL、JS 或程序集放入 Unity。

骨骼平移、旋转、缩放、剪切，slot 颜色/附件切换，权重网格和顶点变形均来自原始数据。
原动画的分段曲线（包括原 3.8 实现的贝塞尔离散节点）转换为 Unity AnimationCurve；没有用整段逐帧图片替代动画。
运行时由 Unity Animation 播放原生 AnimationClip，UGUI Mesh 绘制装饰特效，使用原生 Shader 表达普通/叠加混合和 UI 裁剪。
核心金币玩法仍使用世界空间 SpriteRenderer/Rigidbody2D。每个骨骼实例缓存顶点和裁剪缓冲，图集按 Resources 路径延迟加载。
复杂动画仍有逐帧骨骼矩阵、网格裁剪和 Canvas 网格更新成本；尚未完成 Android 设备帧耗时/加载峰值测试。

数值参考仅在本机离线 authoring 工具中运行官方 Spine 3.8 参考解析器（固定 SHA-256）；工具本身没有进入 Unity 或公开仓库。
原始 loading_4 旧版 skins/curve 结构做了显式格式归一化。数值报告比较 70 个姿态：
22,280 个顶点坐标、12,300 个矩阵分量，最大误差 0.00048828125 像素；颜色最大误差约 6.97e-7。
报告：`native_skeletal_validation.json`。采样姿态通过不等于每一个时间点均经过比较。

Unity 2022.3.62f3c1 实际 GPU 渲染 14 段动画各一帧，全部有可见输出且无渲染异常；记录在
`native_skeletal_render_validation.json` 与 `NativeSkeletalRenders`。这项检查不是与原应用截图的像素差分。
10 个预制体加原有框架合计 75 个预制体、6 个场景通过结构检查；接入后的实际 Play Mode 29 项回归通过。

已接回主场景中的 AnNiu_TX、奖励页的 DJB_TX/CaiDai_TX、转盘中的 LaoHuJ_TX/PiaoDai_TX、转盘奖励彩带。
Source 组件为空资源（如 FailDialog 节点 15）继续保持为空，没有填入猜测素材。
奖励页 ATTACHED_NODE 挂点按原骨骼层级同步；这些挂点对应的原始动画没有剪切通道。
转盘使用 LaoHuJ_TX 的真实完成回调进入选格，不再只依赖估算等待时间。

尚待完成：启动加载流程的动画接入、主玩法碰撞/最高金币飞行/奖励飞钱全链路触发、全部页面时序及原画面逐帧对照、设备性能验证。
