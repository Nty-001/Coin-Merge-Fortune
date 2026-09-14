---
name: unity-native-prefab-development
description: Implement or review Unity gameplay and UI using the user's native, prefab-first engineering rules, including obfuscation compatibility, code-bound events, resource loading, device parity, and hot-path performance. Apply to Unity project development and fixes.
---

# Unity 原生与预制体开发规则

本 skill 保存用户对 Unity 开发的全局要求。进行 Unity 功能开发、修复或代码审查时应用；不把某个项目的包名、地区、Git 地址或逆向结论变成通用规则。

## Obfuz 兼容与事件绑定

- 禁止用反射调用方法或字段。确实不可替代时，在被调用处和调用处按需要添加 `[Obfuz.ObfuzIgnore]`，明确保留范围。默认选择显式接口、类型化引用和直接调用。
- 不依赖 `enum.ToString()` 做逻辑判断、存档 key、配置 key 或反射匹配。使用明确稳定的常量或映射。
- Unity 事件通过代码绑定和解除绑定。禁止用 Inspector 拖拽方式配置持久化事件回调；序列化组件引用仍然允许。
- 不为满足兼容性要求擅自添加 Obfuz 或其他商业程序集。无反射的代码无需新增 Obfuz 依赖；若项目已有 Obfuz，则遵循已有集成方式。

## Prefab-First

- 优先通过 Prefab、Scene、Inspector、ScriptableObject 表达层级、组件、布局、样式和参数。
- 运行时代码负责状态控制、事件绑定、数据刷新、显隐、实例化已配置的 Prefab、播放动画及音效。
- 禁止用运行时代码大量创建静态 UI、GameObject 层级、布局、颜色、字体等结构。不要把应由策划或美术调整的参数写死在逻辑中。
- 可以用 Editor 工具生成并保存真实 Prefab、Scene、配置资源；运行时必须使用生成的资源，不能依赖 Editor 生成器兜底。
- 使用 Unity 原生功能和标准游戏工程结构。插件程序集及包使用 Unity 官方提供的实现；没有授权时不引入商业 SDK、第三方 UI 或动画运行库。

## 资源引用

- 组件、Prefab 和轻量配置可用 `[SerializeField]`。
- 大图片、音效、特效、模型等尽量通过 Resources 或 Unity 官方 Addressables 按路径/地址加载，避免启动时强引用整个资源集合。
- 控制资源生命周期，避免重复加载和无界缓存。选择简单且符合当前项目规模的加载方式。

## Editor 与真机一致

- 禁止通过 `Application.isEditor`、`#if UNITY_EDITOR` 等改变核心逻辑、初始化流程、资源加载流程、数据路径或异常处理。
- Editor 应复现 Android/iOS 的同一运行流程。修复根因，不添加仅在 Editor 生效的静默成功或替代数据兜底。
- 允许不改变运行结果的 Debug Log、Gizmos、Profiler 代码；纯编辑器资产制作工具放在 Editor 程序集。

## UI 与游戏元素

- 所有可点击 UI 使用 Unity 标准 `Button` 或 UI Toolkit `Button`。禁止用 Image + Collider、`OnMouseDown`、`OnPointerDown`、自定义 Raycast、空物体点击区域等替代 Button。
- Prefab 中清楚表达按钮视觉与交互，不能用与可见按钮分离的伪点击区域。
- 核心玩法对象使用 GameObject、SpriteRenderer、Mesh、物理组件及相应 Prefab。不能用 UI 元素直接充当核心玩法对象；UI 负责界面展示与交互。

## 性能与方案选择

- 轻量、低频逻辑优先保持简单可读，不做无依据的过度优化。
- 在 Update、LateUpdate、FixedUpdate、高频回调和大循环中，避免 LINQ、频繁 new、Find、FindObjectOfType、频繁 GetComponent、字符串拼接、频繁 Instantiate/Destroy 和重复 UI Layout rebuild。
- 对高频对象缓存引用、复用缓冲区，按实际负载使用对象池。考虑 CPU、GC、常驻内存和加载峰值。
- 采用高性能任务或高成本实现前，主动说明风险，给出成本更低的替代方案，明确推荐哪个方案、较高消耗是否可接受，以及继续采用高消耗方案时 CPU / GC / 内存 / 加载的影响。不要默认采用高消耗实现。

## 验证与交付

- 用真实 Prefab/Scene 检查静态结构和引用，再验证运行行为；生成了资源文件不等于已通过 Unity 导入或 Play Mode。
- 对复刻任务按原始函数体、资源、配置和实机表现核对行为；缺失证据要标注，不能把 stub、mock 或仅保留元数据的组件算成已实现。
- 既有用户指令要求保留 SDK facade/mock 时保持该边界，游戏内触发逻辑与商业 SDK 实现分开处理。
