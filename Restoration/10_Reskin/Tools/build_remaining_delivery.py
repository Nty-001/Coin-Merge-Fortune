from pathlib import Path
import json,html
from PIL import Image,ImageDraw
ROOT=Path(__file__).resolve().parents[3]
P=ROOT/'paid_ui_work/reskin_20260916'
runtime='07_Verification/RemainingRuntime/'
rows=[
('01','金币提现','01_coin_withdraw',['01_coin_locked','01_coin_ready'],'menus_coin_locked.png'),
('02','美国账号表单','02_account_us',['02_account_us'],'menus_email.png'),
('03','巴西账号表单','03_account_br',['03_account_br'],'menus_brazil.png'),
('04','印尼账号表单','04_account_id',['04_account_id'],'menus_indonesia.png'),
('05','提现校验、条件与限额','05_withdraw_flow',['05_verification','05_next_condition','05_daily_limit'],'menus_next_condition.png'),
('06','用户协议与隐私政策','06_legal',['06_terms','06_privacy'],'menus_terms.png'),
('07','启动加载','07_loading',['07_loading'],'startup_rewarded.png'),
('08','失败与复活','08_game_over',['08_game_over'],None),
('09','评分','09_rating',['09_rating'],None),
('10','奖励弹窗','10_reward_states',['10_highest_reward','10_newbie_reward','10_normal_reward','10_double_reward'],None),
('11','抽奖奖励','11_wheel_reward',['11_wheel_cash','11_wheel_coin'],None),
('12','新手引导','12_guide',['12_guide_move','12_guide_merge','12_guide_cash','12_guide_coin'],'native_guide_step3.png'),
('13','轻提示与状态组件','13_feedback',['13_toast','02_account_us','01_coin_ready'],None),
('14','复活奖励','14_resurrected',['14_resurrected'],None)]
doc='''<!doctype html><html lang="zh-CN"><meta charset="utf-8"><meta name="viewport" content="width=device-width,initial-scale=1"><title>剩余界面 Unity 换皮验收</title><style>body{margin:0;background:#eaf6ff;color:#143650;font:16px/1.65 system-ui,"Microsoft YaHei",sans-serif}header{padding:30px;background:#123e64;color:white}h1{margin:0;font-size:28px}main{padding:24px;max-width:1500px;margin:auto}a{color:#1477b5}nav{display:flex;flex-wrap:wrap;gap:10px}nav a{background:white;padding:5px 12px;border-radius:8px;text-decoration:none}section{background:white;padding:22px;border-radius:16px;margin:24px 0}.cols{display:grid;grid-template-columns:1fr 2fr;gap:24px}.shots{display:flex;flex-wrap:wrap;align-items:start;gap:10px}.shots a{width:calc(50% - 10px);max-width:300px}.shots img,.concept{max-width:100%;height:auto;border-radius:9px}h2{margin:0}h3{font-size:17px}.note{padding:15px;border-left:4px solid #2ca8e6;background:#d8edfc}small{color:#466b83}details{margin:16px 0}details img{max-height:600px;max-width:100%}@media(max-width:750px){.cols{grid-template-columns:1fr}.shots a{width:calc(50% - 10px)}main{padding:12px}}</style><header><h1>剩余界面换皮 · Unity 实际运行验收</h1><p>已按“全部通过”的审核结果接入独立工程；本页右侧是 Unity PlayMode 截图。</p></header><main><p class="note">工程：UnityRemainingWorkingCopy；主场景：Assets/Scenes/RecoveredMain.unity。253 项检查通过。运行图保留原有动态文案、完整协议、地区支付标识和布局约束；测试金额及随机广播与效果图可能不同。鼠标点击可查看原尺寸。未进行手机实机验证。</p><nav>'''
for num,title,*_ in rows:doc+=f'<a href="#p{num}">{num} {title}</a>'
doc+='</nav>'
for num,title,concept,files,before in rows:
 doc+=f'<section id="p{num}"><h2>{num} · {title}</h2><div class="cols"><div><h3>已通过的风格效果图</h3><a href="RemainingUI_Review_R1/{concept}.png"><img class="concept" src="RemainingUI_Review_R1/{concept}.png" loading="lazy"></a></div><div><h3>Unity 实际运行</h3><div class="shots">'
 for f in files:
  assert (P/runtime/(f+'.png')).exists(),f
  doc+=f'<a href="{runtime}{f}.png"><img src="{runtime}{f}.png" loading="lazy"><small>{f}</small></a>'
 doc+='</div></div></div>'
 if before:doc+=f'<details><summary>查看替换前运行图</summary><img src="RemainingUI_Before/{before}" loading="lazy"></details>'
 doc+='</section>'
doc+='<section><h2>切图边缘检查</h2><p>检查了透明蒙版、底色去除和深底显示效果；原图可放大查看。</p><a href="remaining_alpha_qa.png"><img class="concept" src="remaining_alpha_qa.png"></a></section></main></html>'
(P/'remaining_delivery.html').write_text(doc,encoding='utf-8')
files=['01_coin_locked','03_account_br','08_game_over','09_rating','10_highest_reward','07_loading']
sheet=Image.new('RGB',(1500,590),'#163047');draw=ImageDraw.Draw(sheet)
for i,f in enumerate(files):
 im=Image.open(P/runtime/(f+'.png'));im.thumbnail((250,550));sheet.paste(im,(250*i,30));draw.text((250*i+8,8),f,fill='white')
sheet.save(P/'remaining_delivery_overview.png')
print('Delivery review and actual-runtime overview saved.')
