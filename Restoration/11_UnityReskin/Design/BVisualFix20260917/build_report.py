from pathlib import Path
import json,html,re
p=Path(__file__).parent
r=json.loads((p/'After/play_mode.json').read_text(encoding='utf-8-sig'))
old='../VisualAudit20260917/Current/'
items=[
('02','提现验证标题重叠','标题、说明分别占用独立文字区域；动画弹出过程中也检查交叠。','US_verification_flow.png','US_verification_flow.png'),
('04','抽奖机底部 146','恢复原始隐藏状态，保留抽奖次数计算与奖励链路。','US_machine.png','US_machine.png'),
('05','连续切换国家字体贴图不足','B 版使用固定光栅字号生成字形，以几何缩放适配文字框；大标题使用较高清晰度档位。','JP_home.png','JP_home.png'),
('06','顶部提现提示过小','允许最多两行自然断行，按原有外框宽高适配。','US_home.png','US_home.png'),
('07','通知栏字重与断行','统一两行布局；日、韩、泰、孟加拉文字补齐回退字重，保留通知滚动和金额高亮。','JP_home.png','JP_home.png'),
('08','绿色按钮文字不统一','主流程绿色按钮使用白字、蓝描边；账户表单运行时启用状态同样保持此样式。','US_account_valid.png','US_account_valid.png'),
('09','验证与任务内层旧控件','费用卡、状态卡、任务卡与进度条统一为玻璃蓝；验证状态图标沿用原始时间线。','US_next_task_flow.png','US_next_task_flow.png'),
('11','协议旧名称和文字长图','两份协议转换为原生 Text 段落与 ScrollRect，产品名称替换为 Merge Stack Journey。正文依据现有六张图转录，未新增条款。','US_page_2.png','Privacy_0.0.png'),
('14','评分星星扁平旧样式','新增透明底立体金色选中星与冰蓝未选中星，保留五级点击状态。','US_rating.png','US_rating_selected.png')]
def figure(src,label):
 return f'<button class="shot" data-src="{src}" aria-label="放大{label}"><img src="{src}" loading="lazy" alt="{label}"><span>{label} ↗</span></button>'
cards=''.join(f'<article id="fix-{n}"><div class="tag">审计 #{n} · 已修复</div><h2>{title}</h2><p>{desc}</p><div class="compare">{figure(old+before,"修复前")}{figure("After/"+after,"修复后")}</div></article>' for n,title,desc,before,after in items)
checks=''.join('<li>'+html.escape(x)+'</li>' for x in r['checks'])
extra=''.join(figure('After/'+name,label) for name,label in [('US_verification_flow.png','新验证状态'),('US_machine.png','抽奖机'),('US_home.png','英语主界面'),('RU_home.png','俄语主界面'),('JP_home.png','日语主界面'),('Terms_1.0.png','用户协议顶部'),('Terms_0.0.png','用户协议底部'),('Privacy_1.0.png','隐私政策顶部')])
page='''<!doctype html><html lang="zh-CN"><meta charset="utf-8"><meta name="viewport" content="width=device-width,initial-scale=1"><title>B 版视觉问题修复验收</title>
<style>*{box-sizing:border-box}body{margin:0;background:#edf7ff;color:#173653;font:16px/1.65 system-ui,"Microsoft YaHei",sans-serif}header,main{max-width:1180px;margin:auto;padding:34px 24px}header{padding-bottom:12px}h1{font-size:36px;line-height:1.25;margin:10px 0 15px}h2{font-size:21px;margin:5px 0}p{margin:8px 0 18px}.eyebrow{color:#176cac;font-weight:750;letter-spacing:1px}.scope{background:#dcecff;border-left:4px solid #1687db;padding:15px;border-radius:8px}.metrics{display:grid;grid-template-columns:repeat(4,1fr);gap:14px;margin:26px 0}.metric{background:white;border:1px solid #cde4f5;border-radius:16px;padding:20px}.metric b{font-size:32px;color:#087b65;display:block;line-height:1.2}.metric span{font-size:14px;color:#486779}.grid{display:grid;grid-template-columns:1fr 1fr;gap:22px}article{background:white;border:1px solid #cfe4f4;border-radius:18px;padding:24px;box-shadow:0 7px 20px #13467106}.tag{color:#07866f;font-size:13px;font-weight:700}.compare{display:grid;grid-template-columns:1fr 1fr;gap:12px}.shot{appearance:none;border:1px solid #d4e6f3;border-radius:12px;background:#dfedfa;padding:8px;cursor:zoom-in;min-width:0;color:#245978;font:inherit}.shot img{width:100%;height:330px;object-fit:contain;display:block}.shot span{display:block;font-size:13px;padding-top:6px}.gallery{display:grid;grid-template-columns:repeat(4,1fr);gap:12px}.gallery .shot img{height:300px}details{background:white;padding:20px;border:1px solid #d0e5f5;border-radius:14px;margin:24px 0}summary{cursor:pointer;font-weight:700}li{margin:7px 0;overflow-wrap:anywhere}dialog{border:0;border-radius:14px;background:#122331;padding:42px 14px 14px;max-width:96vw;max-height:96vh}dialog::backdrop{background:#051524dc}dialog img{max-width:90vw;max-height:83vh;object-fit:contain;display:block}#close{position:absolute;right:12px;top:6px;border:0;background:transparent;color:white;font-size:24px;cursor:pointer}a{color:#086bb0}footer{padding:18px 0;color:#5b7485;font-size:13px}@media(max-width:800px){.grid{grid-template-columns:1fr}.metrics{grid-template-columns:1fr 1fr}.gallery{grid-template-columns:1fr 1fr}h1{font-size:28px}.shot img{height:300px}}</style>
<header><div class="eyebrow">MERGE STACK JOURNEY / B 版收益版</div><h1>9 项视觉问题修复验收</h1><p>2026-09-17 · Unity 2022.3.62f3c1 · 副本 11_UnityReskin</p><div class="scope">本轮仅处理所选 B 版问题。A 版自然量页面未修改。外框与按钮的既有尺寸保持，文字适配内部空间。商业 SDK 继续使用现有 mock。</div>
<div class="metrics"><div class="metric"><b>9 / 9</b><span>所选问题完成</span></div><div class="metric"><b>COUNTRIES</b><span>连续切换国家</span></div><div class="metric"><b>TEXTCOUNT</b><span>文字检查次数</span></div><div class="metric"><b>0</b><span>运行异常 / 文字越框</span></div></div></header>
<main><div class="grid">CARDS</div><h2 style="margin-top:35px">更多运行截图</h2><p>点击图片查看原始分辨率。</p><div class="gallery">EXTRA</div><details><summary>验证范围和结果</summary><p>使用独立临时存档：26 个国家、主界面和各收益版页面、奖励弹窗、引导、GM、六档金额选择、账户提交、验证动画、下一任务、抽奖领取与评分按钮。另检查两份协议的顶部、中段和底部。未调用真实广告、支付或提现服务。</p><ul>CHECKS</ul><a href="After/play_mode.json">完整自动验证记录</a></details><details><summary>实现与素材记录</summary><p>原生预制体与 Text/ScrollRect/Button；字体适配功能仅对 B 版序列化开启。验证标题和说明在动画期间按渲染边界检查交叠。星星与状态徽章由内置 imagegen 生成并导出透明 PNG；提示词和导出记录见 <a href="README.md">README</a>。协议文字保留现有内容，只整理段落、去掉图片换行连字符并更新产品名称。</p><a href="../VisualAudit20260917/index.html">原始问题审计</a></details><footer>截图为本地 Unity Play Mode 的固定测试状态，不代表真实支付、广告填充或服务端到账结果。原图与无关用户修改未纳入本次修改。</footer></main>
<dialog id="viewer"><button id="close" aria-label="关闭">×</button><img id="full" alt="放大运行截图"></dialog><script>const d=document.querySelector('#viewer'),f=document.querySelector('#full');document.querySelectorAll('[data-src]').forEach(b=>b.onclick=()=>{f.src=b.dataset.src;d.showModal()});document.querySelector('#close').onclick=()=>d.close();d.onclick=e=>{if(e.target===d)d.close()};</script></html>'''
page=page.replace('COUNTRIES',str(len(r['countries']))).replace('TEXTCOUNT',f"{r['textChecks']:,}").replace('CARDS',cards).replace('EXTRA',extra).replace('CHECKS',checks)
if not r['passed']:raise RuntimeError('Cannot publish a passed report while validation failed')
(p/'index.html').write_text(page,encoding='utf-8')
refs=re.findall(r'(?:src|href|data-src)="([^"#]+)"',page)
missing=[x for x in refs if not (p/x).exists()]
(p/'report_validation.json').write_text(json.dumps({'linksChecked':len(refs),'missing':missing,'runtimePassed':r['passed']},indent=2),encoding='utf-8')
print(json.dumps({'links':len(refs),'missing':missing,'textChecks':r['textChecks']}))
