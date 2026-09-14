import json,hashlib,csv,zipfile,html,re,collections
from pathlib import Path
R=Path(__file__).resolve().parents[2];O=R/'Restoration'
def read(p):return json.loads(p.read_text(encoding='utf-8-sig'))
def write(p,x):p.parent.mkdir(parents=True,exist_ok=True);p.write_text(json.dumps(x,ensure_ascii=False,indent=2),encoding='utf-8')
def md(p,s):p.parent.mkdir(parents=True,exist_ok=True);p.write_text(s.strip()+'\n',encoding='utf-8')
native=[];il2cpp=[]
for p in (R/'export_20260914/apk').glob('*.apk'):
 with zipfile.ZipFile(p) as z:
  for entry in z.infolist():
   if entry.filename.endswith('.so'):native.append({'apk':p.name,'path':entry.filename,'bytes':entry.file_size,'classification':'Cocos engine, fully disassembled separately' if entry.filename.endswith('/libcocos2djs.so') else 'not included in reconstructed gameplay runtime'})
   if 'il2cpp' in entry.filename.lower() or 'global-metadata' in entry.filename.lower():il2cpp.append({'apk':p.name,'path':entry.filename})
write(O/'01_Evidence/engine_and_native_inventory.json',{'package':'com.mergecoin.cotune.tuneco','version':'1.6','engine':'Cocos Creator 2.x runtime','evidence':['assets/src/cocos2d-jsb.73f44.js','lib/arm64-v8a/libcocos2djs.so','complete JS module bundles and Cocos compiled asset format v1'],'il2cppEntries':il2cpp,'nativeLibraries':native})
modules=[]
for variant in ['Packaged','HotUpdate']:
 manifest=read(O/'02_Gameplay'/variant/'module_manifest.json')
 lines=[f'# {variant} 主玩法函数体索引','','每个模块文件含完整原始编译后的 JavaScript 函数体；依赖映射位于文件头。保留的变量名可能经过编译压缩。并非 TypeScript 原始工程。','','|模块|完整函数体|字节数|依赖|','|---|---|---:|---|']
 for m in manifest:
  file=O/'02_Gameplay'/variant/m['file'];m['sha256']=hashlib.sha256(file.read_bytes()).hexdigest();m['variant']=variant;modules.append(m)
  if m['category']=='gameplay/application module':lines.append(f"|{m['name']}|[{m['name']}]({m['file']})|{m['bodyBytes']}|{', '.join(map(str,m['dependencies'].values()))}|")
 md(O/'02_Gameplay'/variant/'README.md','\n'.join(lines))
write(O/'02_Gameplay/module_body_inventory.json',modules)
md(O/'02_Gameplay/README.md',r'''
# 玩法代码阅读路径

这里保留完整函数体，不以空方法、签名或 Dummy.dll 代替代码。`Packaged` 与 `HotUpdate` 是两套不同的玩法资源，均已拆出。

|功能|APK 内置玩法|下载的热更新玩法|
|---|---|---|
|进入与加载|LoadScene、HomeView、GameScene|LoadingScene、GameScene|
|投放与碰撞合成|LevelManager、Block、BlockUtils、PhysicsManager|GameScene、CoinItem|
|关卡/掉落配置|LocalDataManager、LevelUtils|GameManagement、GameUtils、GameScene|
|存档|GameModel、LocalDataManager、UserDefault|PlayData、NewGamePlayData、GameLocalData|
|转盘与奖励|详见模块清单|LuckDrawDialog、LuckyDrawRewardDialog、RewardDialog|
|广告触发与回调|游戏调用接口保留|HWL_TStool.HWLshowAd、window.XSSdkCallback|
|提现界面与校验|无对应完整提现系统|GameRealWD*、GameFakeWD*、GameWDValidate*、GameWithdraw*|
|提现请求组装|无对应链路|ServerConfig.sendCash/httpCashSend、HWL_TStool.AESUtil|

所有主玩法模块见两套目录的 README。`AlgorithmDependencies` 保留请求加密等逻辑依赖的 37 个原始算法模块，不含商业广告 SDK 的平台实现。原始 Android 商业 SDK DEX/so 未加入 Unity 框架。

## 实际存在的 native 库

原 APK 不含 libil2cpp.so 或 global-metadata.dat，不能对不存在的文件声称完成 IL2CPP 还原。
`NativeCocosAssembly/text.asm` 包含实际 libcocos2djs.so 的完整 .text 汇编；plt.asm、__lcxx_override.asm 覆盖其他可执行段。
functions.csv 提供函数地址、符号、大小和函数体在汇编文件中的字节偏移；branch_references.csv 提供直接/间接跳转记录。
coverage.json 记录逐段覆盖字节数。`.byte` 是原字节保留，包含代码段中的字符串/数据及未识别字；不能视为已理解的高级逻辑。
这不是原始 C++/TypeScript 源码，也不能解析出所有运行时计算得到的间接调用目标。
''')
md(O/'05_PrefabModel/README.md',r'''
# 场景与预制体模型

两套玩法合计 63 个源场景/预制体，包含 1,127 个 Node/Scene 节点。
Decoded/*.json 保留解码对象列表、根对象、源路径与资源 UUID。

- `$ref`：同一解码文件中 objects 数组的下标。
- `$asset`：跨文件的资源 UUID，查相同 UUID 的 Decoded 文件或 Art aliases.json。
- `__type__`：原始 Cocos 类名或压缩后的脚本类 ID；module_manifest.json 可映射到完整脚本。
- `data`：SpriteFrame、Texture2D 等自定义序列化对象的原始有效载荷。
- `StaticPreviews`：按源坐标和可见性绘制的静态参考图；场景根按进入时激活。没有执行玩法、Widget、骨骼动画或运行时文本，因此不是原游戏运行截图，也不是 Unity 画面一致性证明。

解析器依据原包 cocos2d-jsb.73f44.js 中的 deserialize-compiled 模块 254 实现，支持全部出现的数据类型和 Texture2D 特殊 pack。结构校验结果见 07_Verification。
''')
md(O/'04_Assets/README.md',r'''
# 可复用美术与音频

- `Packaged/Sprites` 与 `HotUpdate/Sprites`：432 张独立 PNG，保留透明背景，按原 offset/originalSize 还原画布。
- `sprites.json`：UUID、逻辑名称、原图集、rect、rotated、offset、originalSize、capInsets，可用于重新打图集和九宫格设置。
- `Native`：490 个原 native 资源文件，保留原始字节。
- `Audio`、`Fonts`：按可恢复的逻辑路径命名的音频和字体，含私有缓存中解压出的 FZY4JW.ttf。
- `ContactSheets`：独立图片总览。
- `SkeletalReusable`：10 套骨骼数据的可读 skeleton.json、atlas、纹理、182 张图集 region 图片和分动画时间轴。
- `AnimationSource`：3 个 Cocos 动画片段的原始曲线数据。

源骨骼动画包含 mesh、clipping、deform。它们的所有源数据已保留，但尚未完成使用 Unity 原生 SkinnedMeshRenderer/Animator 的逐项等价实现。框架不依赖 Spine 等第三方运行时；不要把源数据完整导出误认为动画移植已经完成。
''')
md(O/'06_UnityFramework/README.md',r'''
# Unity 2022.3.62f3c1 基础框架

目录可交给 Unity Hub 作为基础工程打开。它是后续移植框架，不是完成了 1:1 行为和画面验证的游戏。

## 已提供

- Assets/Prefabs：63 个源场景/预制体对应的 Unity YAML 预制体，节点坐标、层级、尺寸、锚点、旋转和资源引用来自原数据。
- Assets/Scenes：两套玩法的 4 个静态场景图，以及 MockFlow.unity。
- Assets/Art：源纹理、432 张独立图片、字体、音频和骨骼原始数据/独立 region。
- Assets/Config：默认配置、实际缓存归一化配置、代码默认值和硬币参数。
- Assets/Scripts/Core：数据 DTO 与 RecoveredNode 原始属性保留组件。
- Assets/Scripts/Gameplay：原生 Rigidbody2D/Collider2D 的合成框架和已核实的币值、半径、积分、权重抽样规则。物理单位及整体手感尚未与 Cocos 校准。
- Assets/Scripts/SDK：IAdFacade/IWithdrawalFacade 与本地 mock。广告成功、失败、取消、不可用以及提现条件、错误和幂等路径可测试，不联网、不执行真实付款。
- Assets/Scripts/UI/MockFlowPanel：MockFlow 场景的按钮驱动触发器。
- Assets/Editor/RecoveryBuilder：许可证激活后可在编辑器内用官方 PrefabUtility 重建并运行检查。
- Packages/manifest.json：仅 Unity 官方内置模块与 com.unity.ugui；没有第三方商业 SDK 程序集。

## 验证状态与未完成项

独立 C# 编译通过（引用指定编辑器的 UnityEngine/UnityEditor/官方 UGUI）。11 项纯 C# 规则和 mock 检查通过。生成的 YAML 结构、内部对象引用与 GUID 检查通过。
指定编辑器因缺少有效许可证退出，未执行编辑器导入、Play Mode 或截图比对。见 ../07_Verification/unity_build.log。

RecoveredNode.originalComponents 保留所有原字段，但它不是原玩法脚本的 C# 实现。自定义玩法类，以及尚未等价实现的 Widget/Layout/EditBox/ProgressBar/部分遮罩/骨骼行为，见 native_yaml_unported_components.json。原始完整函数体在 ../02_Gameplay；这些行为仍需逐项移植、绑定与校验。
源 Scene 的序列化 _active=false 是待激活状态，生成框架时仅将场景根激活，原值仍保存在 originalNodeJson。

## 后续使用

1. 在 Unity Hub 为指定版本激活许可证，再打开此目录。
2. 先打开 Assets/Scenes/MockFlow.unity，检查本地广告和提现按钮路径。
3. 执行菜单 Coin Merge > Build recovered prefabs and mock scene，可用官方 API 重建，并检查生成日志。
4. 按 module_body_inventory、field_values 和 component_migration 的映射逐项移植真实玩法；不要把框架的 mock 规则当作服务器真规则。
5. 对照原 MuMu 画面执行逐控件、动画和物理轨迹测试。当前未声称达到 1:1。
''')
coverage=read(O/'02_Gameplay/NativeCocosAssembly/coverage.json')
art=read(O/'07_Verification/art_summary.json')
unported=read(O/'07_Verification/native_yaml_unported_components.json')
write(O/'07_Verification/completeness.json',{'claim100PercentReplica':False,'jsGameplayModulesWithBodies':143,'algorithmDependencyModules':37,'decodedCocosAssets':1029,'sourcePrefabsAndScenes':63,'sourceNodes':1127,'spriteExports':432,'nativeAssetFiles':490,'nativeFunctionSymbols':coverage['functionSymbols'],'nativeExecutableBytesCovered':sum(x['coveredBytes'] for x in coverage['sections']),'nativePseudoDataBytes':sum(x['pseudoDataBytes'] for x in coverage['sections']),'unityPrefabs':63,'unityScenes':5,'unportedComponentOccurrences':len(unported),'originalJsMockPaths':'passed','csharpCompile':'passed','csharpMockChecks':11,'editorImport':'blocked_no_license','liveServerConfig':'blocked_pending_explicit_client_id_authorization','cachedServerConfig':'recovered_from_device','remaining':['Additional live/other-client/other-country server configs are unverified.','Some field semantics require further manual verification; exact values and code references are preserved.','Custom gameplay and dynamic UI behavior need C# ports and prefab binding.','Spine mesh/clipping/deform and exact animations need Unity-native conversion.','Unity import/Play Mode/visual and physics parity tests remain unperformed.','Native disassembly preserves code bytes but is not source-level semantic recovery.']})
md(O/'README.md',r'''
# Coin Merge Fortune 还原资料与 Unity 迁移框架

**此交付提供可读原包资料、完整已取得玩法函数体、实际缓存配置、美术资源和 Unity 基础框架。尚未达到或验证“100% 完整复刻”。**

原包 com.mergecoin.cotune.tuneco 1.6 使用 Cocos Creator，不是 Unity IL2CPP。安装包中没有 libil2cpp.so 或 global-metadata.dat。

|目录|内容|
|---|---|
|01_Evidence|引擎、原 APK 与 native 库核验|
|02_Gameplay|内置 40 + 热更新 103 个完整 JS 玩法模块、37 个算法依赖；实际 libcocos2djs.so 全可执行段汇编与函数体索引|
|03_Configuration|原始表、设备缓存真值、原代码补齐后的生效值、逐叶字段真实值/语义/引用、默认与缓存差异|
|04_Assets|432 张独立图片、490 个原 native 文件、音频、字体、10 套骨骼动画源资料及 region 导出|
|05_PrefabModel|63 个源场景/预制体完整对象图、UUID 引用与静态预览|
|06_UnityFramework|63 个 Unity YAML 预制体、5 个场景、原生基础脚本、官方包依赖、本地广告/提现 facade/mock|
|07_Verification|完整度、反汇编覆盖率、结构/引用检查、原 JS mock 检查、C# 编译/逻辑检查及编辑器失败日志|
|Tools|可复现的拆包、解码、导出、生成、校验脚本|

优先打开 `index.html` 浏览模块、配置、美术与预制体。原始导出仍在上一级 export_20260914，未改写。模拟器应用未因本次还原任务而修改。

## 已验证

- 143 个玩法模块保留完整编译函数体，未用空方法或签名代替；广告与提现触发逻辑包含在内。
- 1,029 个 Cocos 资源解码，解析错误 0；2,025 个跨资源引用检查未发现缺失目标。
- 432 张独立图片导出无错误，裁切和布局元数据保留。
- libcocos2djs.so 全部可执行段的 16,881,296 字节均进入汇编/原字节输出；40,962 个函数符号可定位函数体。仍有 184,200 字节以 .byte 形式保留，不声称这些字节已全部解释为指令。
- 原 JS 广告请求/原生回调/成功与失败路径，以及提现请求构造、原始加密、历史查询和错误路径，通过本地隔离 mock 检查。
- Unity C# 源码独立编译通过，11 项 C# 逻辑检查通过。原生 YAML 对象、GUID 和引用检查通过。

## 不能标成 100% 的具体原因

1. 在线配置 GET 需要将当前客户端 ID 发往游戏配置域名，自动审批拒绝，等待明确授权；已保留设备中的真实缓存和与默认表的 17 处差异。未虚构未取得的服务器值。
2. 部分字段语义尚未独立确认，CSV 明确标注，并给出实际值与使用代码位置。
3. Unity 中的自定义玩法、动态 UI、骨骼 mesh/clipping/deform 等尚未全部移植和绑定。完整源字段及函数体已保留，待移植项逐条列出。
4. 指定 Unity 编辑器缺少有效许可证，未能执行导入、Play Mode、画面对比或物理一致性验证。已生成的 YAML 不是编辑器验证通过的承诺。
5. native 汇编覆盖不等于原始 C++ 源码还原，也不能凭静态分析得出所有间接调用的运行时目标。

没有真实创建并完成整个复刻游戏。框架按 Unity 原生组件与预制体目录组织，商业 SDK 实现排除，只保留玩法触发接口与可测试的本地 mock。

## 技术参考

序列化解析主要依据原包内引擎 deserialize-compiled 模块 254；公开接口参考 [Cocos 2.4 Details](https://docs.cocos.com/creator/2.4/api/en/classes/Details.html)。编辑器生成器使用 [Unity PrefabUtility.SaveAsPrefabAsset](https://docs.unity.cn/2021.2/Documentation/ScriptReference/PrefabUtility.SaveAsPrefabAsset.html)，并引用指定安装目录内的官方程序集做独立编译检查。
''')
# Standalone report; all links are local and no external scripts are loaded.
chunks=['<!doctype html><meta charset="utf-8"><title>Coin Merge Fortune 还原资料</title><style>body{font:16px system-ui;background:#101a30;color:#e7edf7;margin:0 auto;padding:36px;max-width:1280px}a{color:#8ed7ff}h1{font-size:32px}h2{margin-top:36px}.notice{background:#28334b;padding:20px;border-left:4px solid #ffce75}.grid{display:grid;grid-template-columns:repeat(auto-fill,minmax(180px,1fr));gap:14px}.card{background:#1b2840;padding:12px;border-radius:10px}.card img{width:100%;height:240px;object-fit:contain}input{width:100%;box-sizing:border-box;padding:14px;background:#203451;color:white;border:1px solid #45617e;font-size:16px}.module{display:inline-block;margin:5px;padding:7px;background:#223451;border-radius:5px}small{color:#a1b7d1}table{border-collapse:collapse}td{padding:8px;border-bottom:1px solid #31415c}</style><h1>Coin Merge Fortune · 可读资料与迁移框架</h1><div class="notice">原包为 Cocos，不含 IL2CPP。原始玩法函数体和资源已整理；Unity 1:1 行为、动画和画面尚未完成验证。<a href="README.md">先读完整度说明</a></div><p>143 个玩法模块 · 432 张独立图片 · 63 个源预制体/场景 · 40,962 个 native 函数符号</p><input id="filter" placeholder="按模块名、预制体名或关键词筛选"><h2>入口</h2><p><a href="03_Configuration/README.md">字段说明</a> · <a href="03_Configuration/field_values.csv">配置真实值 CSV</a> · <a href="03_Configuration/EffectiveRuntime/Cached.normalized.json">当前客户端生效配置</a> · <a href="06_UnityFramework/README.md">Unity 框架</a> · <a href="07_Verification/completeness.json">未完成项</a></p>']
for variant in ['Packaged','HotUpdate']:
 chunks.append('<h2>'+variant+' 玩法函数体</h2><div>')
 for m in modules:
  if m['variant']!=variant or m['category']!='gameplay/application module':continue
  url='02_Gameplay/'+variant+'/'+m['file'];chunks.append(f'<a class="module searchable" href="{html.escape(url)}">{html.escape(m["name"])}</a>')
 chunks.append('</div><h2>'+variant+' 场景与预制体初始态预览</h2><p><small>仅静态序列化状态；未执行运行时脚本、布局器和动画。</small></p><div class="grid">')
 for p in read(O/'05_PrefabModel'/variant/'prefab_inventory.json'):
  chunks.append(f'<div class="card searchable"><img loading="lazy" src="05_PrefabModel/{variant}/StaticPreviews/{p["uuid"]}.png"><b>{html.escape(p["name"])}</b><p><a href="05_PrefabModel/{variant}/Decoded/{p["uuid"]}.json">完整对象图</a> · {p["objects"]} 个对象</p></div>')
 chunks.append('</div><h2>'+variant+' 美术总览</h2><div class="grid">')
 for p in sorted((O/'04_Assets'/variant/'ContactSheets').glob('*.jpg')):
  rel=p.relative_to(O).as_posix();chunks.append(f'<a href="{rel}"><img style="width:100%" loading="lazy" src="{rel}"></a>')
 chunks.append('</div>')
chunks.append('<h2>Native 实际函数体</h2><p>完整 text.asm 约 231 MB，建议文本编辑器打开。<a href="02_Gameplay/NativeCocosAssembly/functions.csv">函数体位置索引</a> · <a href="02_Gameplay/NativeCocosAssembly/coverage.json">覆盖率</a></p><script>document.querySelector("#filter").addEventListener("input",e=>{let q=e.target.value.toLowerCase();document.querySelectorAll(".searchable").forEach(x=>x.style.display=x.textContent.toLowerCase().includes(q)?"":"none")});</script>')
(O/'index.html').write_text(''.join(chunks),encoding='utf-8')
print('Wrote report, source index, engine evidence, completeness record and local HTML browser.')
