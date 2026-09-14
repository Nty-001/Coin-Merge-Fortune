# 场景与预制体模型

两套玩法合计 63 个源场景/预制体，包含 1,127 个 Node/Scene 节点。
Decoded/*.json 保留解码对象列表、根对象、源路径与资源 UUID。

- `$ref`：同一解码文件中 objects 数组的下标。
- `$asset`：跨文件的资源 UUID，查相同 UUID 的 Decoded 文件或 Art aliases.json。
- `__type__`：原始 Cocos 类名或压缩后的脚本类 ID；module_manifest.json 可映射到完整脚本。
- `data`：SpriteFrame、Texture2D 等自定义序列化对象的原始有效载荷。
- `StaticPreviews`：按源坐标和可见性绘制的静态参考图；场景根按进入时激活。没有执行玩法、Widget、骨骼动画或运行时文本，因此不是原游戏运行截图，也不是 Unity 画面一致性证明。

解析器依据原包 cocos2d-jsb.73f44.js 中的 deserialize-compiled 模块 254 实现，支持全部出现的数据类型和 Texture2D 特殊 pack。结构校验结果见 07_Verification。
