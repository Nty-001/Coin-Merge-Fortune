import json,re,csv,collections
from pathlib import Path
R=Path(__file__).resolve().parents[2];O=R/'Restoration';C=O/'03_Configuration'
def read(p):return json.loads(p.read_text(encoding='utf-8-sig'))
def write(p,v):p.parent.mkdir(parents=True,exist_ok=True);p.write_text(json.dumps(v,ensure_ascii=False,indent=2),encoding='utf-8')
meanings={
'appearRule':'已通关后的掉落权重表；按通关等级选不大于当前等级的最大键。',
'firstAppearRule':'首次局掉落权重表；未通关时按已合成最高等级选档。',
'weigh':'候选掉落权重，原拼写为 weigh；过滤非正权重后，以权重和为随机区间。',
'level_list':'以棋盘中当前最高币值选档；二维数组第 0 行为候选币值，第 1 行为对应权重。',
'lotteryConfig':'转盘奖励槽配置；奖励类别、数量及权重见对应槽对象。',
'lotteryScoreConfig':'按当前累计抽奖次数选取一次抽奖所需积分的分段表。',
'lotteryCount':'累计抽奖次数的分段起点。','requiredScore':'该抽奖次数档位下单次抽奖消耗的积分。',
'drawRewardStrong':'转盘奖励强引导策略开关，具体分支见 LuckyDrawRewardDialog。',
'newJumpRewardtimes':'新版跳奖间隔表；区间键和数组位置必须结合使用函数阅读。',
'jumpRewardtimes':'跳奖间隔表，保留原结构；使用位置附后。',
'thirdjumpRewardtimes':'第三段跳奖策略；first/second/thirdAndlookADDouble 为原代码阶段名。',
'openScoreTimes':'评分弹窗触发阈值配置。',
'group1':'按国家选择的旧版虚拟提现档位组 1。','group2':'旧版虚拟提现档位组 2。','group3':'旧版虚拟提现档位组 3。','group4':'旧版虚拟提现档位组 4。','group5':'旧版虚拟提现档位组 5。','group6':'旧版虚拟提现档位组 6。',
'groupNew1':'按国家选择的新版虚拟提现档位组 1。','groupNew2':'新版虚拟提现档位组 2。','groupNew3':'新版虚拟提现档位组 3。','groupNew4':'新版虚拟提现档位组 4。','groupNew5':'新版虚拟提现档位组 5。','groupNew6':'新版虚拟提现档位组 6。',
'global':'normalizeBlastConfig 补齐的全局默认组；未必存在于原始服务器响应中。',
'guide_coins':'引导阶段硬币数量/奖励参数；见 GameScene 的实际用途。','show_CN':'中国地区展示控制配置。',
'not_force':'非强制更新版本配置。','force_version':'强制更新版本配置。',
'coin_mubiaoA':'客户端 ID 尾号属于组 A 的字符串列表。','coin_mubiaoB':'客户端 ID 尾号属于组 B 的字符串列表。',
'cashTime':'现金倒计时分段数组，原始各列由 CashTimeDialog 使用。','cashCoins':'现金弹窗/进度相关数量阈值。',
'tixian_products':'全局提现展示档位列表。','withdrawAmount':'提现档位的展示/目标金额。','withdrawCount':'提现次数条件/配置，见对应判断。',
'condition_2':'原条件字段 2；实际条件类型以 GameWDValidate 模块使用分支为准。','condition_3':'原条件字段 3；实际条件类型以 GameWDValidate 模块使用分支为准。',
'condition_4':'原条件字段 4；实际条件类型以 GameWDValidate 模块使用分支为准。','condition_5':'原条件字段 5；实际条件类型以 GameWDValidate 模块使用分支为准。',
'real_products':'真实提现展示的商品/档位配置；这不代表服务器实际会付款。',
'condition_merge':'累计合成条件阈值。','condition_daily_merge':'当日合成条件阈值。','condition_login_days':'有效登录天数条件阈值。',
'fee':'配置给出的手续费数值；单位与所在国家/通道一致。','cash':'该产品配置金额，保留原币种上下文。','credited':'配置给出的到账金额。',
'channel':'渠道标识。','channels':'国家下的提现渠道配置字典。','ranges':'按数值区间选用参数的分段表。','le':'区间下界原字段，包含关系由代码中的比较符决定。','lh':'区间上界原字段，包含关系由代码中的比较符决定。','ratio':'该区间使用的比例/除数，见实际计算表达式。',
'real_init_coin':'真实奖励余额初始化默认值。','real_addvideo':'按已看视频数量选取新增奖励区间。','reward_multiplier':'奖励倍率分段配置。',
'reward_coins':'奖励余额阈值数组。','collect_progress_data':'收集进度参数。','ui_delay_data':'界面延时参数。',
'gravityScale':'Cocos 刚体重力缩放；移植 Unity 需同时匹配世界单位与 Physics2D.gravity。',
'linearDamping':'线性阻尼系数。','dropGaps':'普通投放后下一次预览出现的间隔秒数。','dropGaps2':'另一投放间隔参数，保留原字段。','failedFrame':'越界失败累计帧阈值。','smartDrop':'智能投放开关。','scale':'按硬币类型的缩放映射。',
'upgradeMap':'硬币合成升级映射，2000 的结果仍为 2000。','radiusList':'硬币碰撞半径，单位为 Cocos 设计像素。','offsetList':'硬币碰撞体偏移，单位为 Cocos 设计像素。','score':'按合成结果币值产生的积分映射。','valueToIndex':'硬币币值到精灵数组索引。','valueToScale':'硬币币值到美术缩放系数。'}
sources=[]
meanings.update({
'base_name':'存档记录的原类型名称。','fakeMoney':'虚拟提现余额；与真实奖励余额分开保存。','money':'金额/余额字段；具体是哪种余额由所在对象决定，须结合引用表达式。','gold':'金币余额/计数字段，见 PlayData 使用。','UseRevenue':'累计/使用的广告收益记录，保留原拼写。','addVodeoshowCoin':'观看视频后用于显示增加金币的存档量，保留原拼写。','ecmp':'原存档广告收益参数，名称拼写为 ecmp；不自动等同于任何服务端结算指标。',
'watch_video_count':'累计观看视频广告次数。','coin1024Number':'原代码沿用的高阶币累计合成计数名，实际触发币值必须按 GameScene 判断。','today1024NumberCoin':'当天高阶币合成计数，参与有效登录日判定。','today1024NumberCoinDate':'当天合成计数所属日期；日期变化后重置该计数。','histroyMaxScore':'历史最高分，原字段拼写保留。','gameTotalScore':'当前可用于转盘的累计积分，抽奖时扣除。','roundScore':'当前局积分。','loginDays':'满足游戏条件的有效登录天数；不是仅打开应用的自然日数。','lastLoginDate':'最后计入有效登录的日期。','isNewLoginDay':'本次是否形成新的有效登录日。','hasCurrentRoundStats':'是否已有当前局统计。','currentRoundStartHistroyMaxScore':'本局开始时的历史最高分快照。','currentRoundCoin1024Number':'本局高阶币合成计数。','hasSavedGameScene':'是否已有可恢复棋盘快照。','value':'所在对象的原值；棋盘 savedCoins 中表示币值，其他对象须结合上级路径。','x':'坐标/偏移的 X 分量。','y':'坐标/偏移的 Y 分量。','angle':'保存的旋转角度。','savedCurrentCoinValue':'保存的当前投放币值。','savedNextCoinValue':'保存的下一个预览币值。','savedPreviewX':'保存的预览币水平位置。','savedCoinsScore':'棋盘/当前局保存积分。','savedDrawScore':'用于恢复抽奖进度的积分镜像。','raccountName':'用户输入的提现账户名/账号。','rfullName':'用户输入的提现收款人姓名。','rdocumentId':'用户输入的提现证件字段。','raccountType':'用户选择的提现账户类型。','realSelectPlatform':'当前选择的真实提现渠道。','playerCurrentStep':'玩家步骤存档，当前只在初始化中出现，未确认额外消费者。','guideStep':'新手引导步骤记录。','currentLotteryCount':'累计已完成抽奖次数。','stronewardTimes':'原始强奖励次数存档字段；当前仅初始化出现，语义未进一步确认。','dropCointimes':'投放硬币次数记录。','windowsCointimes':'弹窗金币次数/触发记录，见对应引用。','watch_video_Singlecount':'当前局/阶段的视频观看次数。','real_watch_video_Singlecount':'真实奖励链路的当前阶段视频次数。','open_music':'音效开关。','open_bgm':'背景音乐开关。','open_vibrate':'振动开关。','gameRateScore':'评分弹窗记录分值。','gameRateTimes':'评分弹窗触发次数记录。','firstMergeIcon500':'首次合成 500 币的状态标记。','newFakeMoneyWithdraw':'新版虚拟提现状态/档位记录。',
'id':'所在列表中对象/产品的标识符。','coin':'金币数量/门槛，具体用途由所在配置对象决定。','condition_video':'视频观看次数条件。','condition_ad':'广告次数条件。','condition_coin':'硬币/金币数量条件。','exchangeRate':'表中配置的汇率值；本次恢复的 JS 未找到直接字段引用，不能确认被使用。','currencySymbol':'配置币种符号；是否使用见字段引用。','currency':'配置币种代码/名称；是否使用见字段引用。','amount':'奖励槽的数量，与 type 联合解释。','probability':'随机奖励的权重，实际按权重和抽样。','index':'对象的索引；不同上级配置不共享语义。','type':'对象类别/奖励类型/账户类型之一，由所在路径限定。','countries':'国家列表配置。','guideMoney':'引导奖励金额配置。','baseMinReward':'基础最小奖励参数。','newGuideReward':'新版引导奖励配置。','min':'范围下限。','max':'范围上限。','version_code':'配置中的普通版本门槛。','force_version_code':'配置中的强制更新版本门槛。','first':'该分段配置的第一阶段。','second':'该分段配置的第二阶段。','thirdAndlookADDouble':'第三阶段与看广告翻倍阶段的跳奖配置。','success_target_levels':'胜利/目标合成等级列表，见游戏成功判断。','reward_base_min':'奖励基础下限。','coinSprites':'原节点上绑定的币图数组。','collisionAnimName':'碰撞效果动画名称。','_value':'CoinItem 内部币值。','_isMerging':'硬币正在合成的互斥标志。','_isPreview':'是否为待投放预览币。','isColliding':'是否正在接触碰撞体。','collisionCount':'当前碰撞接触计数。','shouldRelaxDropPhysics':'是否需要过渡到放松投放物理状态。','shouldSettleDropPhysics':'是否需要进入稳定堆叠物理状态。','settleDropContactElapsed':'稳定接触已累计时间。','currentDropGravityScale':'本次投放当前使用的重力缩放。','currentDropLinearDamping':'本次投放当前使用的线性阻尼。','mergePausedForGameOver':'结束状态下暂停合成的静态标志。','SOLID_COLLIDER_TAG':'实体碰撞体标签。','MERGE_SENSOR_COLLIDER_TAG':'合成检测传感器碰撞体标签。','MERGE_SENSOR_EXTRA_RADIUS':'合成检测传感器额外半径，设计像素。','DROP_GRAVITY_BASE':'投放重力缩放基础量。','DROP_GRAVITY_DISTANCE_BONUS':'投放距离参与的额外重力参数。','SETTLE_GRAVITY_SCALE':'稳定状态重力缩放。','DROP_SPEED_BASE':'投放初速度基础参数。','DROP_SPEED_DISTANCE_BONUS':'投放距离参与的初速度增量参数。','DROP_SPEED_FALLBACK':'无法正常估算距离时的速度兜底值。','MIN_EFFECTIVE_DROP_DISTANCE':'有效投放距离下限。','DROP_DAMPING_MULTIPLIER':'投放时阻尼倍率。','SETTLE_LINEAR_DAMPING':'稳定状态线性阻尼。','DROP_CONTACT_SETTLE_DELAY':'接触后切换稳定状态的延迟秒数。','ANGULAR_DAMPING':'角阻尼系数。','COIN_DENSITY':'硬币密度基础参数。','COIN_DENSITY_RADIUS_BASE':'密度计算使用的参考半径。','COIN_DENSITY_MIN':'硬币密度最小值。','COIN_DENSITY_MAX':'硬币密度最大值。','COIN_RESTITUTION':'硬币恢复系数/弹性。','COIN_FRICTION':'硬币摩擦系数。','qMin':'该视频奖励阶段的随机比例下界。','qMax':'该视频奖励阶段的随机比例上界。','edit1':'本地化配置中第一个输入框文案/格式字段。','edit2':'本地化配置中第二个输入框文案/格式字段。','country':'当前国家代码或本地化中的国家文案，按来源区分。'})
for variant in ['Packaged','HotUpdate']:
    for p in (O/'02_Gameplay'/variant/'Modules').glob('*.js'):
        sources.extend((str(p.relative_to(O)).replace('\\','/'),n,line) for n,line in enumerate(p.read_text(encoding='utf-8').splitlines(),1))
fields={}
def walk(x,path='$',key=None):
    if isinstance(x,dict):
        for k,v in x.items():yield from walk(v,path+'.'+k,k)
    elif isinstance(x,list):
        for i,v in enumerate(x):yield from walk(v,path+f'[{i}]',key)
        if not x:yield path,key,x
    else:yield path,key,x
rows=[]
for p in sorted(C.rglob('*.json')):
    if p.name in ['request_response_metadata.json']:continue
    if p.relative_to(C).parts[0] not in ['Packaged','HotUpdate','CapturedDevice','EffectiveRuntime','ServerCapture']:continue
    relative=str(p.relative_to(C)).replace('\\','/');obj=read(p)
    for path,key,value in walk(obj):
        if key not in fields:
            matches=[]
            if key and not key.isdigit():
                pattern=re.compile(r'(?<![A-Za-z0-9_$])'+re.escape(key)+r'(?![A-Za-z0-9_$])')
                matches=[{'file':f,'line':n,'expression':l} for f,n,l in sources if pattern.search(l)]
            fields[key]={'meaning':meanings.get(key,'未单独确认语义；请结合所在上级配置和代码引用，不推断为服务器规则。'),'references':matches}
        item=fields[key]
        meaning=item['meaning']
        if relative.endswith('/language.json'):meaning='国家/语言分组下的本地化文案；由 Lab.getlab 和 GameManagement 读取。字段键保留用于按原文索引。'
        elif key and key.isdigit():meaning='数字档位/产品/文案索引键；含义由上级配置限定。数值本身是原始真值。'
        elif key and re.fullmatch(r'\d+-\d+',key):meaning='原配置的数值区间键；区间边界取法见使用该上级表的函数。'
        rows.append({'source':relative,'path':path,'field':key,'jsonType':type(value).__name__,'actualValue':json.dumps(value,ensure_ascii=False),'meaning':meaning,'referenceCount':len(item['references']),'references':'; '.join(f"{x['file']}:{x['line']}" for x in item['references'][:10])})
with (C/'field_values.csv').open('w',newline='',encoding='utf-8-sig') as f:
    w=csv.DictWriter(f,fieldnames=rows[0].keys());w.writeheader();w.writerows(rows)
write(C/'field_code_references.json',fields)
a=read(C/'HotUpdate/config/data.json');b=read(C/'CapturedDevice/coinGameData.json')
flatA={p:v for p,k,v in walk(a)};flatB={p:v for p,k,v in walk(b)}
diff=[{'path':p,'packaged':flatA.get(p,{'missing':True}),'cachedServer':flatB.get(p,{'missing':True})} for p in sorted(flatA.keys()|flatB.keys()) if flatA.get(p,object())!=flatB.get(p,object())]
write(C/'packaged_vs_cached_diff.json',diff)
lines=['# 配置字段与真实值阅读说明','','配置按来源分别保存；不能把代码补入的默认值当作服务器响应。','','- Packaged：APK 内置玩法的 next_config。','- HotUpdate：下载资源包的 data、detailsData、language。','- CapturedDevice：设备 jsb.sqlite 中的实际缓存值，含 GameData 和存档。','- EffectiveRuntime：执行原始规范化函数后的实际有效值，以及直接从代码提取的常量。','- ServerCapture：在线请求结果、时间和错误/响应；成功与否见元数据。','','`field_values.csv` 逐项列出所有叶字段真实值、类型、语义及代码位置；`field_code_references.json` 保留完整引用列表。未确认的语义明确标注，未虚构字段解释。','','## 核心玩法配置','']
for k,v in meanings.items():lines.append(f'- `{k}`：{v}')
lines+=['','## 已观察到的差异',f'包内默认表与设备缓存表有 {len(diff)} 个叶路径值不同或仅在一侧存在，详见 packaged_vs_cached_diff.json。','','## 关卡含义','内置玩法使用通关等级和最高合成等级选择掉落档位；热更新玩法以棋盘中当前最高币值选择 level_list 档位。它们不是已证实存在的固定地图关卡表。GameManagement.maxLevel=1418 是代码常量，不能据此虚构 1418 张关卡配置。']
(C/'README.md').write_text('\n'.join(lines),encoding='utf-8')
print({'leafValues':len(rows),'fieldNames':len(fields),'differences':len(diff)})
