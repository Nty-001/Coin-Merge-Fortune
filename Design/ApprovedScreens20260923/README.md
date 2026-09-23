# 六个审核通过界面的换皮落地

用户已批准本批六个设计稿。正式工程：`Restoration/12_UnityReskin_20260922`。没有打包或安装 MuMu。

应用范围：游戏结束／复活、九宫格抽奖、抽奖领奖、2000 合成成功、复活成功奖励、评分弹窗。继续使用原生控件、原弹窗根布局和按钮位置；皇冠等装饰通过独立美术层绘制。分数、金额、奖品数量和积分条件均来自原控制器。

`index.html` 展示 Unity 实际运行截图，可切换 1080×1920、1080×2340，与审核设计稿对照。截图使用隔离测试存档；抽奖结果和金额为测试示例。实际抽奖保留原生弹出缩放、拉杆动作、奖格高亮和领奖逻辑。

## 资源与应用

- `Assets/Resources/ApprovedScreens`：九张 PNG，Unity 原生 sprite rect 去除透明导出留白，不重采样；RGBA32 无压缩、无 mipmap。2000 硬币继续引用用户提供的 1254px 原图。
- `Assets/Editor/ApprovedScreensAuthor.cs`：显式迁移工具，写入真实 Prefab 和 Scene。菜单 `Coin Merge > Reskin > Apply six approved popup designs`。
- `Assets/Editor/ApprovedScreensRequest.cs`：当前编辑器的固定本地请求入口，仅应用资源，不构建。
- `approved-screens.patch`：原 Prefab/Scene 到本次最终文件的修改记录。整体基线工程不重复纳入本次提交。
- `asset-prompts.json` / `asset-manifest.json`：从审核稿提取组件美术的生成记录。

## 验证

使用 Unity 2022.3.62f3c1，在隔离工程从原 RecoveredLoading 生命周期进入 Play Mode。

- 六个界面分别检查 1080×1920、1080×2340。
- 应用前后六个弹窗根节点及原按钮的锚点、尺寸、位置、缩放、旋转保持一致。
- 原生按钮无 Inspector 持久化回调，仍由代码绑定；文字和奖励数据动态更新。
- 评分选择、确认和关闭，游戏结束的重开与视频复活均通过。
- 复活使用已有本地模拟广告，播放结束点击原关闭按钮后进入原奖励分支。
- 抽奖拉杆压下与复位、奖格高亮、积分不足拦截、金币／现金领奖以及重复点击不重复发奖通过。
- 所有新增图像已实际导入并验证 RGBA32；原高清 2000 硬币引用验证通过。

详细结果见 `verification.json`。既有物理密度警告与本次图像替换无关；没有更改游戏规则、SDK mock 或原存档。
