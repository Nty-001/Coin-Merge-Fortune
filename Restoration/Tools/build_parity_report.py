"""Standalone local audit report. Uses actual Unity screenshots and saved validation reports."""
import base64
import datetime
import html
import json
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
V = ROOT / '07_Verification'
rows = [
    ('2000金币主链', '直接打开最高币奖励窗', '上升0.22s → 汇聚0.28s → 原GuangH_TX → 飞向入口0.55s → 恢复出币；不额外弹奖励窗', 'GameScene.playMerge1024Flow / finishMerge1024Flow'),
    ('最高币角标', '合成瞬间改变可见数量', '实际计数先保存；飞行到达入口后才刷新角标，并播放两次脉冲', 'GameScene.flyMerge1024PrefabToGetMoney'),
    ('现金领取', '只加余额', '三张原钞票错峰沿贝塞尔曲线飞向顶部，沿用原尺寸、速度范围及收集音效', 'RewardDialog / LuckyDrawRewardDialog.flyMoneyToGameScene'),
    ('顶部加金额', '没有浮动提示', '第三张钞票到达 → 图标放大回弹 → +金额上升70像素，1.8s淡出', 'GameScene.playFlyFakePlusMoney'),
    ('首奖评分', '缺少评分页面和触发', '首次普通奖励结算后再等1.5s出现原评分页，记录gameRateTimes', 'RewardDialog.firstShowScore'),
    ('500金币评分', '只保存首次标记', '首次合成500且gameRateScore≤3时显示评分；后续同币不重复触发', 'GameScene.onCreateMergedCoin'),
    ('评分交互', '未接入', '五个标准Button显示空/实星；4～5星使用原零基索引保存并走商店Mock', 'ScoreDialog.onitemRefresh / setCLosePage'),
    ('奖励标题和金额', '复活与双倍共用标题；普通标题有误，金额缺加号', '按34/35/110等原语言键分别显示，金额保留+并居中', 'RewardDialog.show'),
    ('奖励关闭', '普通/双倍可提前点遮罩关闭', '普通和双倍等1.5s自动关闭；复活、最高币预览、引导沿各自原规则', 'RewardDialog.onLoad / show'),
    ('奖励/失败/转盘表现', '部分页面缺进入缩放、光旋转及声音', '原BaseUI .35s backOut；奖励光每4s转一圈；恢复音效与复活按钮呼吸', 'BaseUI.pop / RewardDialog.show / FailDialog.show'),
    ('失败前动画', '冻结后固定等待', '触线金币染红；整盘按0.04s错峰做六段挤压回弹，末枚完成后显示失败窗；复活恢复颜色', 'GameScene.applyFailCoinRedTint / playFailBoardCoinAnimation'),
    ('危险线', '源节点始终隐藏', '静止/低速币堆达到玩法高度2/3时显示，0.18s分段闪现', 'GameScene.refreshDeadLineOpacity'),
    ('长短屏及GM', '固定物理相机，预览线未随短屏下移；尺寸切换后UI/GM可能偏离可见区', '750像素玩法宽度；短屏差值×0.4；地面贴合底栏；相机绘制前按真实Canvas几何重对齐，GM使用预制体右下角锚定', 'GameScene.adaptPlayAreaForBottomBar / 原Widget边距'),
    ('1_A未启动广告', '无填充也把窗口计数归零', '未启动保留第22次计数；实际启动后的回调才归零，SDK保持Mock', 'GameScene.checkShowReward / getADDouble'),
]
reports = [
    ('本轮生命周期', 'lifecycle_parity_validation.json'),
    ('合成与小手', 'merge_feedback_validation.json'),
    ('主玩法与复活', 'native_gameplay_validation.json'),
    ('GM与提现任务', 'gm_workflow_validation.json'),
    ('全部已接入菜单', 'recovered_menus_validation.json'),
    ('版本切换', 'version_variants_validation.json'),
    ('收益版加载与主界面', 'visuals_rewarded_validation.json'),
    ('基础版加载', 'visuals_packaged_validation.json'),
]
checks = []
for label, filename in reports:
    p = V / filename
    data = json.loads(p.read_text(encoding='utf-8-sig')) if p.exists() else {}
    checks.append({'name': label, 'file': filename, 'passed': data.get('passed', False), 'count': len(data.get('checks', []))})
def escape(s):
    return html.escape(str(s), quote=True)
def image(name):
    return 'data:image/png;base64,' + base64.b64encode((V / name).read_bytes()).decode()

table = ''.join('<tr><td><b>'+escape(a)+'</b></td><td>'+escape(b)+'</td><td>'+escape(c)+'</td><td><code>'+escape(d)+'</code></td></tr>' for a,b,c,d in rows)
results = ''.join('<a class="check" href="../07_Verification/'+escape(c['file'])+'"><b>'+escape(c['name'])+'</b><span>'+('通过' if c['passed'] else '待处理')+' · '+str(c['count'])+' 项</span></a>' for c in checks)
gallery = ''.join('<figure><img src="'+image(name)+'" alt="'+escape(title)+'"><figcaption>'+escape(title)+'</figcaption></figure>' for title,name in [('2000金币原光环','parity_highest_effect.png'),('原评分页','parity_rating.png'),('现金飞向顶部','parity_cash_flight.png'),('短屏完整显示','parity_main_short.png')])
page = '''<!doctype html><html lang="zh-CN"><meta charset="utf-8"><meta name="viewport" content="width=device-width,initial-scale=1"><title>原版差异核对 · Coin Merge Fortune</title><style>
*{box-sizing:border-box}body{margin:0;background:#f2f5f8;color:#203047;font:15px/1.65 "Segoe UI","Microsoft YaHei",sans-serif}main{max-width:1260px;margin:auto;padding:36px 26px 60px}h1{font-size:32px;margin:8px 0}h2{font-size:22px;margin-top:30px}p{max-width:950px}.eyebrow{color:#168065;font-weight:700}.lead{color:#617085}.bar,.card{padding:20px 24px;border-radius:12px;background:white;border:1px solid #dce4eb}.bar{border-left:5px solid #168065}.stats{display:grid;grid-template-columns:repeat(3,1fr);gap:12px;margin:22px 0}.check{display:flex;justify-content:space-between;gap:14px;padding:16px;background:white;border:1px solid #dce4eb;border-radius:10px;color:inherit;text-decoration:none}.check span{color:#168065}.scroll{overflow:auto;background:white;border-radius:12px}table{width:100%;border-collapse:collapse}th,td{text-align:left;padding:15px 16px;border-bottom:1px solid #dce4eb;vertical-align:top}th{background:#e8edf3;font-size:13px;color:#5c6b80}td:first-child{width:14%}td:nth-child(2){width:23%;color:#6d7c8e}td:nth-child(3){width:42%}code{font-size:12px;overflow-wrap:anywhere}.gallery{display:grid;grid-template-columns:repeat(4,1fr);gap:18px}figure{margin:0;background:white;padding:10px;border-radius:12px;border:1px solid #dce4eb}figure img{width:100%;height:340px;object-fit:contain;background:#172338}figcaption{padding:10px 4px;font-size:13px}a{color:#235b9b}.card li{margin:10px 0}.note{font-size:13px;color:#617085}.links{display:flex;gap:18px;flex-wrap:wrap}@media(max-width:850px){.stats{grid-template-columns:1fr 1fr}.gallery{grid-template-columns:1fr 1fr}main{padding:22px 15px}td{min-width:190px}}@media print{body{background:white}main{padding:0}.card,figure{break-inside:avoid}.gallery img{height:240px}.scroll{overflow:visible}table{font-size:11px}.links{display:none}}
</style><main><div class="eyebrow">SOURCE PARITY AUDIT · 2026.09.15</div><h1>这轮发现的差异，逐项修正</h1><p class="lead">依据本机恢复的完整 Cocos JS、场景绑定和原资源，修正 Unity 中可确认的行为与视觉差异。下面的图片来自独立 Unity 验证工程的真实渲染。</p>
<div class="bar"><b>14 类明确差异已处理。</b>停止 Play，重新打开 <code>Assets/Scenes/RecoveredLoading.unity</code> 再运行，不需要清玩家存档。商业 SDK 继续使用现有 Mock。</div>
<div class="stats">__RESULTS__</div><h2>改了什么</h2><div class="scroll"><table><thead><tr><th>环节</th><th>修正前</th><th>现在按原逻辑执行</th><th>完整源函数</th></tr></thead><tbody>__TABLE__</tbody></table></div>
<h2>实际 Unity 画面</h2><div class="gallery">__GALLERY__</div><p class="note">测试画面使用独立合成存档；静态截图不代表与 MuMu 同一随机帧的逐像素比较。</p>
<h2>这份检查不等于全游戏100%证明</h2><div class="card"><ul><li>基础版保留现有实现并做版本/启动回归；其所有细粒度补间尚未完成与原设备的逐帧验收。</li><li>跨 Cocos/Unity 的物理轨迹、字体栅格化、全部地区与分辨率组合，尚无完整的同输入、同帧比对记录。</li><li>真实归因分流、远程任务墙、缺失的提现平台响应仍需有效原始证据；没有编造服务端真值。</li><li>当前原主链没有调用的旧商业模块不会仅因文件存在就接入。广告、商店与支付桥接保持 Mock。</li></ul></div>
<h2>测试入口</h2><div class="links"><a href="index.html">打开完整商业化测试指南</a><a href="../07_Verification/lifecycle_source_reference.json">原JS执行对照</a><a href="../07_Verification/LIFECYCLE_PARITY_AUDIT.md">实现与验证说明</a></div><p class="note">离线文件，无外部脚本或联网请求。生成时间：__DATE__</p></main></html>'''
page = page.replace('__RESULTS__',results).replace('__TABLE__',table).replace('__GALLERY__',gallery).replace('__DATE__',escape(datetime.datetime.now().isoformat(timespec='minutes')))
out = ROOT / '09_TestGuide/parity.html'
out.write_text(page,encoding='utf-8')
print('Wrote',out,'with',len(rows),'audited changes')
