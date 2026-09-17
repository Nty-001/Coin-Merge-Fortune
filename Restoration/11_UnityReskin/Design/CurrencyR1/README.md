# 各国家新钞票统一

标准：用户在 `Assets/Resources/Localization/Currency/1` 更新的 26 张透明 PNG。原图像素和文件字节不改动；国家与货币编号沿用当前工程配置。

- 主界面、引导、飞行动画使用对应国家单捆原图。
- 现金奖励、提现提示、抽奖格子和抽奖现金奖励使用同一套原图。小堆 / 大堆由真实预制体中的 3 / 9 张 Unity Image 组成；不生成新的钞票位图。
- 每张 Image 开启 preserveAspect。堆叠容器 FitInParent 锁定整体比例，图片不拦截按钮点击。
- 26 个本地化 JSON 的三个显示角色均映射到 `Currency/1`。旧 `Currency/2` 和 `Currency/3` 素材保留供历史对照，当前运行链路不再读取。
- 保持金额格式、国家规则、奖励条件、按钮和业务链路。仅为不同展示角色配置新美术及堆叠。

## 后续美术维护

替换 `Currency/1/<编号>.png` 即可更新该货币的全部展示角色；保留 `.meta`。堆叠布局可编辑 `Assets/Prefabs/Runtime/CurrencySmallPile.prefab` 和 `CurrencyLargePile.prefab`。

## 验收

打开 `REVIEW.html` 切换国家查看用户原图及 Unity 真实运行截图。运行验证使用独立临时存档，覆盖 26 个国家 × 21 种页面 / 事件情景；检查当前国家的图片、单张 / 成堆切换、图片渲染比例及点击穿透。

Editor 菜单：`Coin Merge > Reskin > Validate localized banknotes`。这是验证工具，实际游戏不依赖 Editor。

`prepare.cjs` 记录原图校验和并更新本地化资源路径；`CurrencyReskinAuthor` 保存原生预制体及主场景；`audit.cjs` 只读核对原图与配置差异；`review.cjs` 根据通过的测试生成验收页。保留 Unity 原生保存的序列化格式。
