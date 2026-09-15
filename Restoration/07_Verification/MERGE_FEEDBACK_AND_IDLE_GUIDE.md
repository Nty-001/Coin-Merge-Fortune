# 合成反馈与闲置小手

2026-09-15，正式工程 `Restoration/06_UnityFramework`。停止 Play 后重新打开 `Assets/Scenes/RecoveredLoading.unity` 或 `RecoveredMain.unity`，载入更新后的预制体引用，无需清存档。

## 合成表现

依据原始 `GameScene.js` 的完整函数 `onCreateMergedCoin`、`playMergeStarFlyEffect`、`showMergeComboUI` 与原场景资源绑定：

| 条件 | 表现 |
|---|---|
| 每次同值金币成功合成 | 合成原音效、XX_TX 爆光、12 颗星星 |
| 距上次合成不足 0.5 秒又合成 | 连击数累加并重置 0.5 秒计时 |
| 合成停止 0.5 秒，累计只有 1 次 | 不出现英文评价 |
| 累计 2 / 3 / 4 次 | Good / Great / Amazing，Combo ×2 / ×3 / ×4 |
| 累计 5 次及以上 | Unbelievable；倍数最高显示 ×8；音效最高取 combo_5 |

英文、Combo 和倍数均使用原 PNG，不用字体重新绘制。按原图像像素尺寸设置，避免 Unity Sprite PPU 与 Canvas 比例导致缩小。

英文和 Combo 先用 0.08 + 0.02 秒弹入，再等 0.1 秒弹入倍数；停留后用 0.2 秒缩小淡出 Combo 和倍数，英文另用 0.5 秒放大淡出。Combo 位于最后一次合成位置上方 105 像素，并按原公式限制在屏幕内。

星星每颗间隔 0.06 秒，先用 0.14 秒散开，再沿原三次贝塞尔轨迹飞向进度条当前填充位置。基础飞行时间 1.12 秒，原随机偏移 −0.08～+0.16 秒；30% 为淡黄色叠加发光，其余使用普通透明混合。旋转、大小、透明度范围保存在 `MergeFeedback.asset`。

实际积分立即保存，第一颗星星到达后才刷新显示积分与转盘触发判断；每批只刷新一次。重开或失败清理旧飞行动画，避免旧回调影响新状态。

### 爆光动画的源数据问题

原 prefab 名为 `PZ_TX`，实际引用的是 UUID `c80b42c1-1e4d-4ece-a55c-7c6a5616b57c` 的 **XX_TX** 骨骼。原 JS 请求 `idel` / `idel1`，资源实际只有 `idle` / `idle1`。原引擎找不到动画时返回，不替换正在播放的默认轨道；本轮显式使用原 prefab 默认的 `idle1` 首次播放周期，没有创建假的同名动画。原请求名称、实际资源名称均保留在源对照报告中。

## 白色小手

这是主界面 **15 秒闲置提示**，不是只在新手阶段出现：

1. 游戏正在主界面，未结束，没有广告、待弹奖励、待弹转盘或已打开/加载中的业务弹窗时累计时间。
2. 满 15 秒后显示原小手和左右箭头。`handslip` 原动画循环长 3.016667 秒，按原关键帧左右滑动。
3. 任意触摸开始立即隐藏并重置计时；弹窗、GM、失败等状态也隐藏并重置。
4. 小手与箭头不参与 UI 点击拦截；不改变金币操作。它的计时不受 GM 日历 +12h/+24h 影响。

## 验证与性能

- `merge_feedback_validation.json`：真实 Physics2D 碰撞、星星延迟计分、连击图片/时序、复用以及闲置边界的 Unity Play Mode 检查。
- `merge_feedback_source_reference.json`：执行原 GameScene 完整模块，27 项原逻辑/配置核对通过。
- `merge_feedback_regression.json`：原 GM/提现/复活 57 项及主玩法 31 项回归通过；资产结构检查另见 `unity_asset_validation.json`。
- 可视结果：`merge_amazing_combo.png`、`merge_unbelievable_combo.png`、`merge_single_stars.png`、`idle_hand_15_seconds.png`。

使用已保存的原生 Prefab、AnimationClip、UGUI 装饰层和 AudioSource。玩法金币继续使用世界空间 SpriteRenderer/Physics2D。预热 48 个星星和 4 个爆光实例，繁忙时按需扩展并回收；重复八连击检查不再产生新实例。资源按 Resources 路径加载，未加入第三方运行库。极长连击仍会按可见粒子数量增加 CPU/网格开销，没有静默丢弃星星来伪装一致性。

截图使用合成的独立测试存档、750×1624 画面。随机轨迹和不同屏幕比例不等于与 MuMu 的同一帧像素完全一致；本轮核对的是原资源、规则、关键帧与 Unity 实际渲染。
