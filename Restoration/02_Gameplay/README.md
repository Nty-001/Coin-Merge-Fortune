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
