"""Build the local, offline visual audit. Does not modify game assets."""
import json
import shutil
from html import escape
from pathlib import Path
from struct import unpack

ROOT = Path(__file__).resolve().parent
PROJECT = ROOT.parents[1]
REPO = PROJECT.parents[1]

refs = {
    "home": "codex-clipboard-ba7beba0-dcbb-451d-aea0-92ee5dcdcc50.png",
    "loading": "codex-clipboard-f74414b8-6186-454e-becc-fcf4584eefd8.png",
    "coin": "codex-clipboard-ba0c16ee-7dc1-478d-b57f-23de1c557d5c.png",
    "cash": "codex-clipboard-d0159860-1401-47ce-bb63-c515bfe6cde5.jpg",
}
(ROOT / "References").mkdir(exist_ok=True)
for key, name in refs.items():
    src = Path("C:/Users/001/AppData/Local/Temp") / name
    dst = ROOT / "References" / (key + src.suffix)
    if src.exists() and not dst.exists():
        shutil.copyfile(src, dst)

issues = []
def issue(id, priority, group, title, observation, basis, action, shots, evidence="当前运行截图", location=""):
    issues.append(dict(id=id, priority=priority, group=group, title=title,
        observation=observation, basis=basis, action=action, shots=shots,
        evidence=evidence, location=location))

# Each focus is [left, top, width, height], normalized to the unmodified image.
def shot(file, label, focus=None):
    return dict(file=file, label=label, focus=focus)

issue("01", "P1", "A 自然量", "自然量入口和玩法场景仍是旧皮肤",
    "入口仍显示 COIN MERGE FORTUNE、旧金币和纯深蓝底；玩法仍使用保险柜背景。新云朵弹窗打开后，内外像两套游戏。",
    "当前启动页及已确认的主界面采用 MERGE STACK JOURNEY、天空和彩色筹码；A 版尚未同步。A/B 的功能区别应保留，美术语言需要一致。",
    "先补自然量入口、玩法背景、顶部状态条及装饰币；保留关卡、体力、解锁的原有逻辑。",
    [shot("Current/A_home.png", "A 版入口"), shot("Current/A_gameplay.png", "A 版玩法"), shot("Current/US_home.png", "B 版新皮肤")],
    location="RecoveredPackaged.prefab / Home、map、顶部状态栏")
issue("02", "P1", "B 提现", "提现验证：状态标题与说明相互重叠",
    "真实账户提交后的第三个节点中，In progress 与 Withdrawal will arrive after approval 上下挤在一起。",
    "这是文本节点之间的碰撞。单独检查每个文本框是否溢出，无法发现此问题；本次 US 前台完整流程可见。",
    "在固定外框内重新分配第三行标题和说明的区域，设置间距与多语言最小字号，并检查两段文字的相交。",
    [shot("Current/US_verification_flow.png", "实际验证流程 · 第三节点", [.1,.60,.8,.16])],
    evidence="通过真实按钮提交 → 原生验证动画取样",
    location="GameRealTXYZ/content/bg/Layout/New3/stageTips3、stageTips4")
issue("03", "P1", "A 自然量", "荣誉页：Level 1 与 achieved 重叠",
    "已完成的第一行同时显示 Level 1 和 achieved，位置几乎一致；绿色完成图标还在右侧。",
    "A 版荣誉页运行截图与活跃文本清单均确认这两段文字同时出现。",
    "固定每行尺寸，拆分等级名和完成状态的文本区域；检查所有已完成、未完成行。",
    [shot("Current/A_dialog_2.png", "荣誉页首行", [.1,.25,.8,.12])],
    location="RecoveredPackaged/HonorView/.../content/bg/level、ff")
issue("04", "P1", "B 抽奖", "抽奖机底部出现无意义的“146”",
    "机身下方显示 146；这不是积分或奖金的正常排版。用户实机截图也曾出现同样数字。",
    "当前代码把可抽次数与缺失的本地化 key 46 拼接。原 Cocos 预制体把 drawTimesTxt 放在默认隐藏的 redBg 下；复刻后却露出了它。",
    "按原版恢复计数节点的隐藏层级与显示条件；不要只改翻译来掩盖多出来的元素。",
    [shot("Current/US_machine.png", "当前抽奖机 · 底部局部", [.12,.53,.76,.26])],
    evidence="当前截图 + 原 JS 完整方法 + 原预制体 active=false",
    location="RecoveredWheelView.cs:35；原 LuckDrawDialog.js:setDrawLabel；f6fc17bd…json 对象 24 / 64")
issue("05", "P1", "全局", "连续切换国家时，动态字体贴图容量不足",
    "本次连续遍历 26 个国家，完成 AR 后、继续检查时中断：Nunito-Black 的字形无法全部放入 4096 贴图。",
    "这是连续多语言/多页面压力测试中的实际错误；单独的 US 完整流程通过。不能据此断言每个国家冷启动都会失败。",
    "优先检查重复字号、字形请求量和 fallback 策略；按语种拆分或采用官方字体资源方案，再复测连续切换。",
    [], evidence="multi_locale_stress.json · 6461 次文本节点检查后捕获错误",
    location="Nunito-Black / RecoveredTextFit 与多语言字体加载")
issue("06", "P2", "B 主界面", "顶部提现提示被压成过小的一行",
    "US 提示 Still $11.5 remaining to withdraw 被压成单行，文本框上下留白明显。此次运行字号为 17，框高 60。",
    "原游戏及你确认的主界面效果图都是两行、易读的提示；当前虽然没有越框，阅读层级已经不同。",
    "保持气泡大小，允许两行显示，设置可读的最小字号与行距；金额变化及长语言一起验收。",
    [shot("Current/US_home.png", "当前 · 提示过小", [0,0,1,.15]), shot("Original/home.png", "原游戏 · 两行提示", [0,.04,1,.10])],
    evidence="当前与原游戏截图；US_home.json 的 bubbleLabel",
    location="GameScene/up/bubble/bubbleLabel")
issue("07", "P2", "多语言", "通知栏的语言字重和断行不一致",
    "英文为较粗的圆体；日文通知明显更细、更小，样本中末尾单字落到第三行。当前布局优先塞进框，未保持阅读质量。",
    "同一通知条在 US、RU、JP 下应维持一致的信息层级。此次 JP 样本字号 19，US / RU 为 26。",
    "统一语种字体的视觉字重；在固定条高内控制两行及断行，避免孤字和过度缩小。",
    [shot("Current/US_home.png", "英文通知", [0,.07,1,.09]), shot("Current/JP_home.png", "日文通知", [0,.07,1,.09])],
    location="NoticeViewport/iconContent/getMoneyTipLabel / 本地化字形与 RecoveredTextFit")
issue("08", "P2", "全局", "同类绿色按钮使用三种文字样式",
    "主页按钮是白字蓝描边；奖励 Claim、规则 OK 等是白字绿描边；账户和任务 Confirm、A 版 Agree / Retry 为纯蓝字。",
    "同样的亮绿玻璃按钮缺少统一的文字规范，造成页面切换时风格跳变。按钮背景本身已基本同属新风格。",
    "统一主按钮的字面颜色、描边颜色/厚度、字重与内边距；禁用态单列规则，保留现有按钮比例。",
    [shot("Current/US_home.png", "白字蓝描边", [.70,.91,.29,.07]), shot("Current/US_reward_4.png", "白字绿描边", [.17,.65,.65,.07]), shot("Current/US_next_task_flow.png", "纯蓝字", [.17,.60,.66,.11])],
    location="主页 Withdraw / Wheel、RewardView、RecoveredMainMenus、PackagedGameSession 的按钮 Text")
issue("09", "P2", "B 提现", "验证和任务链仍夹着旧式内层控件",
    "新云朵外框内仍保留灰紫色费用卡、扁平绿勾/橙色等待点、旧灰蓝进度槽。任务文本、进度条与底卡还显得挤靠。",
    "这些元素和已确认的提现页玻璃卡片、发光进度条不是同一套材质。当前费用卡仍引用旧 MenuArt 资源。",
    "补齐验证节点、费用卡和任务进度条的小组件；按同一圆角、高光和间距规范配置，不改任务条件。",
    [shot("Current/US_verification_flow.png", "验证内层", [.1,.34,.8,.40]), shot("Current/US_next_task_flow.png", "任务内层", [.1,.42,.8,.13])],
    location="GameRealTXYZ/Layout/bg；GameRealWDTXTips；MenuArt/d921927577ac55a1bd05eef37accc2ad.png")
issue("10", "P2", "A 自然量", "自然量弹窗只统一了外壳，内层还有旧元素",
    "设置仍用小型扁平开关；My Info 两条灰紫底之间露出白矩形；体力倒计时仍是黄底棕边小标签，荣誉列表也有白底残留。",
    "B 版设置已经是大玻璃开关与图标卡。A 版这些旧控件与新云朵弹窗并置，差异很明显。",
    "复用 B 版开关和基础卡样式，清除不必要白色背景；为体力标签补同系列样式。不要将 A 版通用金币误替换成 2000 筹码。",
    [shot("Current/A_dialog_1.png", "A 设置 · 旧开关", [.1,.46,.8,.17]), shot("Current/A_dialog_3.png", "My Info · 白块", [.15,.45,.7,.16]), shot("Current/A_dialog_4.png", "旧倒计时标签", [.57,.64,.36,.06])],
    location="Packaged SettingView、InfoView、HeartView、HonorView")
issue("11", "P2", "全局", "协议内容仍是旧名称和整张文字图片",
    "新启动页名为 MERGE STACK JOURNEY，但隐私内容仍写 Coin Merge Fortune。A 版正文带明显白底，B 版也沿用旧正文风格。",
    "运行时仍按 policyPaths 加载正文 Sprite。这种文字图片不能像其他文本一样适配字体、字号和语言。此项只审核显示与名称一致性。",
    "确认产品名称后，统一正文呈现和可读字号；优先改为正常文本/滚动内容，文案内容另按最终版本确认。",
    [shot("Current/US_page_2.png", "B 版协议正文", [.12,.22,.76,.49]), shot("Current/A_dialog_6.png", "A 版旧名称及白底", [.12,.25,.76,.49])],
    location="PrivacyPolicyView；PackagedGameSession.Policy / policyPaths")
issue("12", "P2", "B 提现", "提现标题和金额尚未达到确认图的立体字效果",
    "当前标题/金额采用平面填色与较粗的均匀描边；确认图的字面高光、内阴影和投影层次更丰富。布局接近，字体质感仍有距离。",
    "这是相对你批准的换皮图的差异，不是要求恢复原游戏旧配色。现金和筹码提现页都有同类现象。",
    "统一可本地化的标题、金额字材质；按确认图控制描边和阴影，避免靠加粗描边替代立体质感。",
    [shot("Current/US_coin_blocked_reference.png", "当前 · 标题", [0,0,1,.11]), shot("References/coin.png", "确认图 · 标题", [0,0,1,.11])],
    evidence="当前渲染与用户确认图对比",
    location="CoinReskin 提现页标题/金额 Text；现金页 Your cash / 金额 Text")
issue("13", "P2", "A 自然量", "难度入口图标压住了文字，玩法分数字号过小",
    "beginner 的箭头盖住单词尾部，normal 的锁压住名称；玩法上方分数条很大，beginner Score0 却缩得很小。",
    "实际 A 版入口/开始玩法截图可见。外框内适配通过，不代表与同一按钮上的图标互不遮挡。",
    "为名称、价格、解锁图标划定独立区域；为分数条设置合适的文本区域和字号。按钮外框不变。",
    [shot("Current/A_home.png", "箭头 / 锁压字", [0,.53,1,.24]), shot("Current/A_gameplay.png", "分数条字号", [.10,.11,.80,.09])],
    location="Packaged Home 的 modes；map 的 scoreText")
issue("14", "P2", "B 其他", "评分星星仍是扁平旧图标",
    "评分弹窗已是云朵玻璃外框，但未选中的星星仍是平灰色旧图标，与新按钮和标题材质不一致。",
    "这是小组件换皮遗漏；本轮点击星星、关闭按钮均通过，未发现交互阻塞。",
    "补齐同系列的未选中/选中星星，两态保持相同尺寸与中心点。",
    [shot("Current/US_rating.png", "评分弹窗", [.08,.27,.84,.43])],
    location="ScoreDialog / RecoveredRatingView.stars")

coverage = [
    ("启动", "加载 0 / 中间 / 100%", "已检查", "三轮均从 0% 开始；新图为已批准换皮", "Startup/After_startup_0_middle.png"),
    ("B 主界面", "余额 / 提现提示 / 通知 / Next / Wheel", "有差异", "06、07、08", "Current/US_home.png"),
    ("B 引导", "步骤 0、1、3、4", "已检查", "US / RU / JP 截图；云朵底、2000 筹码已接入", "Current/US_guide_1.png"),
    ("B 玩法", "合成星星 / Combo / 15 秒闲置手", "已检查", "40 项效果与触发检查通过；英文赞语保留原图", "Effects/merge_amazing_combo.png"),
    ("B 菜单", "设置", "已检查", "新玻璃开关、红色关闭钮已统一", "Current/US_page_0.png"),
    ("B 菜单", "合成规则", "已检查", "原生动画末帧 11 级筹码齐全；末帧取样不等于全程视频比对", "Current/US_rules_source_end.png"),
    ("B 菜单", "隐私正文", "有差异", "11；条款正文未另做全段视觉核对", "Current/US_page_2.png"),
    ("B 提现", "现金金额列表（含滚动）", "有差异", "08、12；金额样本与原机进度不同不计为错误", "Current/US_page_3.png"),
    ("B 提现", "2000 筹码提现：未满足 / 已满足", "已检查", "六档选择、两态按钮与原生点击通过", "Current/US_coin_blocked_reference.png"),
    ("B 提现", "邮箱账户与提交", "有差异", "08；使用本地虚拟测试邮箱，无外部提交", "Current/US_account_valid.png"),
    ("B 提现", "BR / 手机账户表单外观", "部分检查", "只检查表单外观；直接 Show 未准备渠道图，空渠道位不作为缺图结论", "Current/BR_page_6.png"),
    ("B 提现", "验证 → 首个广告任务", "有差异", "02、09；真实按钮及原生动画进入任务", "Current/US_next_task_flow.png"),
    ("B 提现", "激活提示", "有差异", "08；通用按钮字体需统一", "Current/US_page_10.png"),
    ("B 奖励", "复活 / 双倍 / 普通 / 最高级 / 新手", "已检查", "五种奖励面板均取样；新钱堆和 2000 图已显示", "Current/US_reward_4.png"),
    ("B 抽奖", "抽奖机 → 结果 → 领奖", "有差异", "04、08；旋转高亮、结束和领奖流程通过", "Current/US_machine.png"),
    ("B 抽奖", "现金 / 筹码两类奖品弹窗", "已检查", "均为新云朵样式", "Current/US_machine_coin_reward.png"),
    ("B 其他", "失败 / 复活", "有差异", "08；按钮文字需统一", "Current/US_fail.png"),
    ("B 其他", "评分", "有差异", "14；星星选择与关闭可操作", "Current/US_rating.png"),
    ("GM", "三个页签", "已检查", "调试面板按现有功能保留，不要求与原游戏一致", "Current/GM_0.png"),
    ("A 自然量", "入口与玩法", "有差异", "01、13", "Current/A_home.png"),
    ("A 自然量", "隐私引导 / 设置", "有差异", "08、10", "Current/A_dialog_1.png"),
    ("A 自然量", "荣誉 / 信息", "有差异", "03、10", "Current/A_dialog_2.png"),
    ("A 自然量", "体力 / 失败 / 协议", "有差异", "08、10、11", "Current/A_dialog_4.png"),
    ("多语言", "26 国家连续遍历", "未全部完成", "05；在 AR 之后发生字体错误，后续国家不记为通过", "Current/JP_home.png"),
]

def dimensions(path):
    data = (ROOT / path).read_bytes()
    if data[:8] == b'\x89PNG\r\n\x1a\n':
        return unpack('>II', data[16:24])
    # Only PNG files are used for cropped figures.
    return (941, 1672)

def figure(s):
    file, label, focus = s['file'], s['label'], s.get('focus')
    assert (ROOT / file).exists(), file
    if focus:
        w, h = dimensions(file)
        x, y, cw, ch = focus
        view = f'{x*w:g} {y*h:g} {cw*w:g} {ch*h:g}'
        media = f'<svg viewBox="{view}" role="img" aria-label="{escape(label)}"><image href="{file}" width="{w}" height="{h}"/></svg>'
    else:
        media = f'<img src="{file}" alt="{escape(label)}" loading="lazy">'
    return f'<figure><a class="zoom" href="{file}" data-caption="{escape(label)}">{media}</a><figcaption>{escape(label)} · 点击看原图</figcaption></figure>'

cards = []
for it in issues:
    shots = ''.join(figure(s) for s in it['shots'])
    if not shots:
        shots = '<pre class="error">Failed to update dynamic font (Nunito-Black) texture;\nall the needed characters do not fit onto a single texture\n(max size 4096).</pre><a href="multi_locale_stress.json">查看压力测试记录 ↗</a>'
    cards.append(f'''<article id="issue-{it['id']}" class="issue" data-priority="{it['priority']}" data-group="{escape(it['group'])}">
      <div class="issue-top"><span class="badge {it['priority'].lower()}">{it['priority']}</span><span>{escape(it['group'])}</span><span class="number">#{it['id']}</span></div>
      <h3>{escape(it['title'])}</h3><p class="observation">{escape(it['observation'])}</p>
      <div class="shots">{shots}</div>
      <div class="why"><b>判断依据</b><p>{escape(it['basis'])}</p></div>
      <div class="action"><b>建议处理</b><p>{escape(it['action'])}</p></div>
      <details><summary>证据与工程定位</summary><p>{escape(it['evidence'])}</p><code>{escape(it['location'])}</code></details>
    </article>''')

rows = ''.join(f'<tr><td>{escape(group)}</td><td><a class="zoom" data-caption="{escape(name)}" href="{img}">{escape(name)} ↗</a></td><td><span class="state">{state}</span></td><td>{escape(note)}</td></tr>' for group,name,state,note,img in coverage)
thumbs = ''.join(figure(shot(f,l)) for f,l in [
    ('Startup/After_startup_0_middle.png','启动'),('Current/US_home.png','B 主界面'),('Current/US_rules_source_end.png','规则'),
    ('Current/US_coin_blocked_reference.png','筹码提现'),('Current/US_machine.png','抽奖'),('Current/A_home.png','A 版入口')])
passed = json.loads((ROOT/'Current/play_mode.json').read_text(encoding='utf-8'))
startup = json.loads((ROOT/'Startup/startup_after.json').read_text(encoding='utf-8'))
effects = json.loads((ROOT/'Effects/merge_feedback_validation.json').read_text(encoding='utf-8'))
summary = dict(date="2026-09-17", project="Restoration/11_UnityReskin", issues=issues,
    startup=[{k:v for k,v in r.items() if k!='frames'} for r in startup['runs']],
    current_flow_passed=passed['passed'],text_checks=passed['textChecks'],effects_passed=effects['passed'],
    effect_checks=len(effects['checks']), all_locale_passed=False,
    scope="视觉审计；页面状态取样与部分真实事件链；未覆盖原机全部长周期提现任务、广告 SDK 和所有动画逐帧对齐")
(ROOT/'audit_summary.json').write_text(json.dumps(summary,ensure_ascii=False,indent=2),encoding='utf-8')

html = '''<!doctype html><html lang="zh-CN"><head><meta charset="utf-8"><meta name="viewport" content="width=device-width,initial-scale=1"><title>从启动到全页面 · 视觉差异清单</title>
<style>
:root{--ink:#10294d;--muted:#65768e;--blue:#1463d6;--line:#dce6f2;--bg:#f4f7fc;--pink:#ad2640}*{box-sizing:border-box}html{scroll-behavior:smooth}body{margin:0;background:var(--bg);color:var(--ink);font:15px/1.7 "Segoe UI","Microsoft YaHei",sans-serif}a{color:var(--blue);text-decoration:none}a:hover{text-decoration:underline}button,input,select{font:inherit}button,select{cursor:pointer}header{background:#0c2345;color:#fff;padding:42px max(24px,calc((100vw - 1240px)/2)) 32px}header .eyebrow{font-size:12px;letter-spacing:2px;color:#83caff}h1{font-size:36px;line-height:1.3;margin:12px 0 16px}header p{max-width:850px;color:#c9d9ec}h2{font-size:25px;margin:34px 0 15px}h3{font-size:22px;line-height:1.45;margin:12px 0}.sub{color:var(--muted);font-size:13px}.wrap{max-width:1290px;margin:auto;padding:0 24px 55px}.stats{display:grid;grid-template-columns:repeat(4,1fr);gap:15px;margin-top:26px}.stat{border:1px solid #375473;border-radius:14px;padding:13px 18px;background:#142e51}.stat strong{font-size:28px;display:block}.stat span{color:#b9cee5;font-size:13px}.nav{position:sticky;top:0;z-index:10;border-bottom:1px solid var(--line);background:#ffffffed;backdrop-filter:blur(12px);padding:12px 24px;display:flex;gap:24px;justify-content:center;flex-wrap:wrap}.note{background:#e9f2ff;border-left:4px solid #2178dc;padding:15px 20px;border-radius:0 12px 12px 0;margin:24px 0}.strip{display:grid;grid-template-columns:repeat(6,1fr);gap:12px}.strip figure{margin:0}.strip img{height:245px;max-width:100%;object-fit:contain;background:#e6edf6;border-radius:10px}.strip figcaption{font-size:12px;color:var(--muted)}.flow{display:flex;gap:8px;flex-wrap:wrap;margin:20px 0}.flow a{background:#fff;border:1px solid #ccdbee;padding:8px 15px;border-radius:25px}.controls{display:flex;gap:12px;flex-wrap:wrap;align-items:center;margin:20px 0}.controls input,.controls select{border:1px solid #c7d6e9;border-radius:8px;background:#fff;padding:9px 12px}.controls input{flex:1;min-width:200px}.controls #count{min-width:90px;color:var(--muted)}.issues{display:grid;grid-template-columns:repeat(2,minmax(0,1fr));gap:22px;align-items:start}.issue{background:#fff;border:1px solid var(--line);border-radius:16px;padding:23px;scroll-margin-top:80px}.issue[hidden]{display:none}.issue-top{display:flex;gap:12px;align-items:center;font-size:13px;color:var(--muted)}.number{margin-left:auto}.badge{display:inline-block;font-weight:800;border-radius:5px;padding:2px 10px}.p1{background:#ffe8eb;color:#b3203c}.p2{background:#fff0cf;color:#946008}.observation{margin:8px 0 16px}.shots{display:flex;gap:12px;align-items:flex-start;flex-wrap:wrap;background:#f3f6fb;padding:12px;border-radius:10px}.shots figure{margin:0;flex:1;min-width:140px}.shots img{display:block;max-height:340px;width:100%;object-fit:contain}.shots svg{width:100%;display:block;background:#dfebf7;max-height:350px}.shots figcaption{font-size:11px;color:#54677f;margin-top:5px}.why,.action{display:grid;grid-template-columns:66px 1fr;gap:10px;margin-top:16px;font-size:13px}.why b,.action b{color:#385d88}.why p,.action p{margin:0}.action{background:#eef7f3;padding:12px;border-radius:7px}.action b{color:#25724e}details{margin-top:16px;font-size:12px;color:#63738a}summary{cursor:pointer}code{overflow-wrap:anywhere;white-space:normal}.error{max-width:100%;overflow:auto;font:12px/1.6 Consolas,monospace;color:#a8263f}.passes{display:grid;grid-template-columns:repeat(2,1fr);gap:18px}.pass{background:#fff;border:1px solid var(--line);padding:20px;border-radius:12px}.pass b{color:#207955}.pass p{margin:8px 0;color:#52677f;font-size:14px}.tablewrap{overflow:auto;background:white;border:1px solid var(--line);border-radius:12px}table{width:100%;border-collapse:collapse;min-width:770px}th,td{text-align:left;padding:12px 15px;border-bottom:1px solid var(--line);font-size:13px}th{background:#eaf1fa;color:#355a86}td:first-child{white-space:nowrap}.state{white-space:nowrap;font-size:12px}.limits{background:#fff;border:1px solid var(--line);padding:20px;border-radius:12px}.limits li{margin:8px 0}.evidence{display:flex;gap:14px;flex-wrap:wrap}footer{color:#75869e;font-size:12px;margin-top:30px}.lightbox{padding:0;border:0;border-radius:12px;background:#111c30;color:white;max-width:95vw;width:900px;max-height:95vh}.lightbox::backdrop{background:#091426df}.lightbar{display:flex;justify-content:space-between;gap:12px;padding:12px 18px;align-items:center;position:sticky;top:0;background:#111c30}.lightbar button{background:#fff;color:#10294d;border:0;border-radius:5px;padding:5px 14px}.lightbox img{display:block;max-width:100%;max-height:80vh;margin:auto;object-fit:contain}.lightbox .full{display:block;color:#b4d7ff;padding:10px 18px}.section{scroll-margin-top:76px}.toplist{display:grid;grid-template-columns:repeat(3,1fr);gap:12px}.toplist a{display:block;border:1px solid #ccdbee;border-radius:10px;background:white;padding:13px 15px}.toplist small{display:block;color:var(--muted)}@media(max-width:800px){h1{font-size:28px}.stats{grid-template-columns:repeat(2,1fr)}.issues,.passes{grid-template-columns:1fr}.strip{grid-template-columns:repeat(3,1fr)}.strip img{height:200px}.toplist{grid-template-columns:1fr}.nav{gap:14px;font-size:13px}header{padding:30px 20px}.wrap{padding:0 16px 35px}.issue{padding:18px}}@media print{.nav,.controls,.lightbox{display:none}.issues{display:block}.issue{break-inside:avoid;margin:16px 0}.shots img{max-height:230px}.wrap{max-width:none}header{background:#fff;color:#10294d}header p,.stat span{color:#52677f}.stat{background:#fff;border-color:#ccc}.strip{display:none}}
</style></head><body>
<header><div class="eyebrow">VISUAL AUDIT / 2026.09.17 / UNITY 2022.3.62f3c1</div><h1>从启动页到游戏内<br>视觉差异与风格遗漏清单</h1><p>检查对象：11_UnityReskin 副本。以原游戏确认行为，以你批准的天空 / 玻璃 / 彩色筹码美术确认风格。本轮只整理，未修改游戏运行逻辑、Prefab 或美术。</p><div class="stats"><div class="stat"><strong>14 项</strong><span>整理出的待处理事项</span></div><div class="stat"><strong>5 项 P1</strong><span>明显遗漏 / 排版错误 / 运行风险</span></div><div class="stat"><strong>3 / 3</strong><span>启动测试均从 0% 开始</span></div><div class="stat"><strong>725 + 40</strong><span>文本检查 / 效果与引导检查</span></div></div></header>
<nav class="nav"><a href="#overview">总览</a><a href="#issues">差异清单</a><a href="#kept">已统一 / 原样保留</a><a href="#coverage">逐页覆盖</a><a href="#method">范围与证据</a></nav>
<main class="wrap"><section id="overview" class="section"><h2>先看这三个地方</h2><div class="toplist"><a href="#issue-01"><b>① 自然量页面换皮遗漏</b><small>旧 Logo、保险柜、旧金币仍在</small></a><a href="#issue-02"><b>② 两处文字互相压住</b><small>提现验证与 A 版荣誉页</small></a><a href="#issue-04"><b>③ 抽奖机“146”露出</b><small>原版隐藏节点被显示出来</small></a></div><div class="note">“与原游戏不同”不全是问题：新启动画、天空背景、彩色筹码、云朵弹窗是你确认的换皮方向。需要处理的是遗漏、错误和风格不一致，而不是把新美术改回旧游戏。</div><div class="strip">__THUMBS__</div><div class="flow"><a href="#kept">启动</a><span>→</span><a href="#issue-06">主界面 / 引导</a><span>→</span><a href="#issue-08">菜单 / 奖励</a><span>→</span><a href="#issue-04">抽奖</a><span>→</span><a href="#issue-02">提现 / 任务</a><span>↘</span><a href="#issue-01">A 自然量分支</a></div></section>
<section id="issues" class="section"><h2>差异清单</h2><p class="sub">P1：优先处理，含 1 项压力测试风险。P2：统一视觉与可读性。图片可点开；局部放大不改动原始截图。</p><div class="controls"><select id="priority" aria-label="优先级"><option value="">全部优先级</option><option>P1</option><option>P2</option></select><select id="group" aria-label="页面分类"><option value="">全部页面</option><option value="B">B 版页面</option><option value="A">A 自然量</option><option value="global">全局 / 多语言</option></select><input id="query" type="search" aria-label="搜索差异" placeholder="搜索：字体、抽奖、提现、自然量…"><span id="count">14 / 14 项</span></div><div class="issues">__CARDS__</div></section>
<section id="kept" class="section"><h2>已经接入，以及不应误判的差异</h2><div class="passes"><div class="pass"><b>✓ 新加载页与 0% 起步</b><p>三轮首帧均为 0%，0% 可见约 0.46–0.60 秒；完整观察约 2.70–2.84 秒。进度变化连续，未复现一按 Play 就从 80% 多开始。</p><a class="zoom" data-caption="启动 0%" href="Startup/After_startup_0_zero.png">0%</a> · <a class="zoom" data-caption="启动中间帧" href="Startup/After_startup_0_middle.png">中间帧</a> · <a class="zoom" data-caption="启动完成" href="Startup/After_startup_0_full.png">100%</a></div><div class="pass"><b>✓ 新筹码和钱堆已进入主要奖励页</b><p>规则末帧显示 11 级彩色筹码；最高级奖励与抽奖结果使用蓝金 2000 筹码；US / RU / JP 奖励样本使用对应新钱堆。本轮没有发现这些样本退回旧白金 2000。</p><a class="zoom" data-caption="2000 奖励" href="Current/US_reward_4.png">查看 2000 奖励</a></div><div class="pass"><b>✓ 星星、Combo、白手触发存在</b><p>真实碰撞触发星星；连击按原逻辑选择英文。15 秒合格闲置后显示小手，触摸或弹窗会隐藏。40 项相关检查通过。</p><a class="zoom" data-caption="15 秒闲置小手 · 测试摆币已冻结" href="Effects/idle_hand_15_seconds.png">查看闲置小手</a></div><div class="pass"><b>△ 合成英文仍是原版美术：暂按保留</b><p>Amazing 的红黄字、Combo 的橙白字与蓝色玻璃 UI 不完全同系，但此前明确要求还原原版合成表现；它是风格差异，不算漏接功能。若以后统一，可只换字图，保留触发和时序。</p><a class="zoom" data-caption="原版英文赞语仍保留" href="Effects/merge_amazing_combo.png">查看合成赞语</a></div></div></section>
<section id="coverage" class="section"><h2>从启动到分支页面的检查覆盖</h2><p class="sub">点击页面名查看当前样本。这里的“已检查”表示所述状态已核对，不代表全生命周期已完成 1:1 验收。</p><div class="tablewrap"><table><thead><tr><th>阶段</th><th>页面 / 状态</th><th>结果</th><th>说明</th></tr></thead><tbody>__ROWS__</tbody></table></div></section>
<section id="method" class="section"><h2>证据和判断边界</h2><div class="limits"><ul><li><b>当前画面：</b>本机 Unity 2022.3.62f3c1 中运行副本，独立测试存档；主要截图 1080×2340，筹码提现流程 941×1672，特效取样 750×1624。未用效果图冒充运行截图。</li><li><b>原游戏：</b>本次在 MuMu 实际观察主界面、设置、规则、现金提现、筹码提现；抽奖计数差异另由完整恢复 JS 和原 Cocos 预制体证明。06_UnityFramework 的截图不作为原游戏截图。</li><li><b>测试数据：</b>为查看样式，部分页面直接显示并注入金额/积分，所以不同截图的余额、轮次、奖励金额不作为数值偏差；BR/手机表单的空渠道位、直接 Show 页的默认文字不作为故障结论。</li><li><b>动画：</b>提现与抽奖采用真实按钮进入并等待原生动画；规则另外取了源动画末帧，特效测试使用受控时间推进。未完成原机与副本所有动画逐帧同步对比。</li><li><b>排版：</b>US 前台完整流程中 725 次文本检查没有检测到越过各自框的网格；人工仍发现文本互相重叠。不能将“无单框越界”当成“视觉无问题”。</li><li><b>尚未完成：</b>原机后续全部长周期提现任务、全部国家冷启动/真机 DPI 与画幅、广告 SDK 展示、声音与逐帧节奏；多国连续遍历在字体错误后终止。不能宣称 100% 对齐。</li><li><b>已排除：</b>窗口失焦导致的验证取样超时，前台复测已通过；2000 入口图标在动画中短时留白，原机也可见；GM 是用户要求的调试入口。</li></ul><div class="evidence"><a href="audit_summary.json">结构化差异清单</a><a href="Current/play_mode.json">US 前台流程记录</a><a href="Startup/startup_after.json">启动逐帧进度</a><a href="Effects/merge_feedback_validation.json">特效与手势检查</a><a href="multi_locale_stress.json">多语言中断记录</a></div></div></section><footer>本地视觉审计 · 2026-09-17 · 副本路径：Restoration/11_UnityReskin<br>原游戏窗口截图保存在本地 Original 目录。此页离线可用，无外部图片或脚本请求。</footer></main>
<dialog class="lightbox" id="lightbox"><div class="lightbar"><span id="caption"></span><button id="closebox" type="button">关闭 ×</button></div><img id="fullimage" alt=""><a id="originallink" class="full" target="_blank" rel="noopener">打开完整原图 ↗</a></dialog>
<script>
const cards=[...document.querySelectorAll('.issue')],p=document.querySelector('#priority'),g=document.querySelector('#group'),q=document.querySelector('#query');
function filter(){let n=0;for(const c of cards){let gp=c.dataset.group,matchGroup=!g.value||(g.value==='global'?gp==='全局'||gp==='多语言':gp.startsWith(g.value));c.hidden=!!(p.value&&c.dataset.priority!==p.value)||!matchGroup||!c.textContent.toLowerCase().includes(q.value.trim().toLowerCase());if(!c.hidden)n++;}document.querySelector('#count').textContent=n+' / '+cards.length+' 项';}
[p,g,q].forEach(el=>el.addEventListener('input',filter));
document.querySelectorAll('a[href^="#issue-"]').forEach(a=>a.addEventListener('click',()=>{p.value='';g.value='';q.value='';filter();}));
const box=document.querySelector('#lightbox'),fi=document.querySelector('#fullimage');document.querySelectorAll('a.zoom').forEach(a=>a.addEventListener('click',e=>{e.preventDefault();fi.src=a.href;fi.alt=a.dataset.caption||'运行截图';document.querySelector('#caption').textContent=fi.alt;document.querySelector('#originallink').href=a.href;box.showModal();}));document.querySelector('#closebox').addEventListener('click',()=>box.close());box.addEventListener('click',e=>{if(e.target===box)box.close();});
</script></body></html>'''
html = html.replace('__THUMBS__',thumbs).replace('__CARDS__',''.join(cards)).replace('__ROWS__',rows)
(ROOT/'index.html').write_text(html,encoding='utf-8')
print(json.dumps(dict(report=str(ROOT/'index.html'),issues=len(issues),priorities={p:sum(i['priority']==p for i in issues) for p in ('P1','P2')},coverage=len(coverage)),ensure_ascii=False))
