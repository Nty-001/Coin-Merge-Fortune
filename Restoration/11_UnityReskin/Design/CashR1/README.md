# 提现页换皮 · Cash R1

目标：把图一的现金提现页改为图二的蓝天玻璃风格。仅应用在 `Restoration/11_UnityReskin` 副本。

打开 `REVIEW.html` 可查看参考图、Unity 实际渲染、长屏、俄文、日文和透明素材。截图中的 488.49 是独立测试存档；正式页面使用玩家实际余额。

## 已替换

- 天空和彩虹、蓝色玻璃标题栏、绿色返回按钮。
- 用户确认的 1000 / 2000 / 500 筹码装饰。
- 选中绿色卡片、未选中蓝色卡片、动态绿色进度条。
- 底部玻璃栏、独立绿色提现按钮、原生文字渐变和描边。
- Button 的 RectTransform 比例与 PNG 比例一致；文字及进度仍由原有逻辑更新。

## 保留

金额与国家配置、货币格式、提现任务顺序、选中档位、滚动、金额不足提示、满足条件后进入激活流程、SDK mock 均沿用现有实现。没有把参考图片中的文字/金额做成整页截图覆盖游戏。

## 验证

Unity 2022.3.62f3c1 Play Mode：106 项检查通过，覆盖 941×1672、1080×2340、US / RU / JP、真实 EventSystem 点击、滚动、档位切换、进度数值、条件不足和完成分支、返回与 GM。

`scope_audit.json` 确认场景和预制体仅改动 `GameFakeWDDialog` 子树，原工程 4167 个源文件保持原哈希。运行时仅新增文字颜色效果，并给提现页已有自适应布局增加可序列化边距。

## 维护位置

- 场景：`Assets/Scenes/RecoveredMain.unity`；通常从 `RecoveredLoading.unity` 启动。
- 预制体：`Assets/Prefabs/Runtime/RecoveredMain.prefab` → 菜单中的 `GameFakeWDDialog`。
- 素材：`Assets/Resources/CashReskin/`。HeroSky / Footer 是完整背景面，其余控件使用透明边缘；筹码复用 `RulesReskin` 中已确认的透明 PNG。
- 编辑器重建：`Coin Merge → Reskin → Apply reference Cash withdrawal`。
- 回归验证：`Coin Merge → Reskin → Validate reference Cash withdrawal`。使用专用临时存档，不覆盖玩家存档。
- `prepare.cjs` 从 Target、BlankReference、FooterSource 确定性裁切 PNG。FooterSource 仅用图像工具清除底栏上的按钮，防止背景重复包含按钮。

参考是栅格效果图，工程文字采用 Unity 原生 Text、现有字体及渐变描边，保持动态数值和多语言；字形和高光并非逐像素相同。对照页提供真实渲染便于继续视觉验收。
