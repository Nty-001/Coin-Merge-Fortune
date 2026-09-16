"""Package verified visual changes; never writes to the original Unity project."""
from pathlib import Path
import csv, hashlib, json, shutil
ROOT=Path(__file__).resolve().parents[3]
LOCAL=ROOT/'paid_ui_work/reskin_20260916'
BASE=ROOT/'Restoration/06_UnityFramework'
WORK=LOCAL/'UnityRemainingWorkingCopy'
OUT=ROOT/'Restoration/10_Reskin'
def digest(p):return hashlib.sha256(p.read_bytes()).hexdigest()
with (LOCAL/'baseline_sha256.csv').open(encoding='utf-8-sig',newline='') as f: rows=list(csv.DictReader(f))
errors=[r['path'] for r in rows if not (BASE/r['path']).exists() or digest(BASE/r['path']).lower()!=r['sha256'].lower()]
assert not errors,errors
changes=[]
for p in sorted((WORK/'Assets').rglob('*')):
 if not p.is_file():continue
 rel=p.relative_to(WORK)
 if rel.as_posix().startswith(('Assets/Editor/Reskin','Assets/Editor/RemainingReskin')):continue
 original=BASE/rel
 after=digest(p);before=digest(original) if original.exists() else None
 if before==after:continue
 assert p.suffix in ('.png','.meta','.prefab','.unity'),f'Unexpected runtime change: {rel}'
 target=OUT/'Patch'/rel;target.parent.mkdir(parents=True,exist_ok=True);shutil.copy2(p,target)
 changes.append(dict(path=rel.as_posix(),before=before,after=after,bytes=p.stat().st_size))
tests={}
for name,file in [('menus','recovered_menus_validation.json'),('gameplay','native_gameplay_validation.json'),('reskin','ReskinRuntime/checks.json'),('lifecycle','lifecycle_parity_validation.json'),('visuals','visuals_rewarded_validation.json'),('remaining','RemainingRuntime/checks.json')]:
 data=json.loads((LOCAL/'07_Verification'/file).read_text(encoding='utf-8-sig'))
 assert data['passed'],data
 tests[name]={'passed':True,'checks':len(data['checks'])}
 verify=OUT/'Verification';verify.mkdir(exist_ok=True)
 (verify/(name+'.json')).write_text(json.dumps(data,ensure_ascii=False,indent=2),encoding='utf-8')
report={'originalFilesVerified':len(rows),'originalChanges':errors,'runtimeCodeChanged':False,'tests':tests,'patchFileCount':len(changes),'files':changes}
(OUT/'manifest.json').write_text(json.dumps(report,ensure_ascii=False,indent=2),encoding='utf-8')
pages=[('主界面','01_home.png'),('抽奖','02_wheel.png'),('提现','03_cash.png'),('设置','04_settings_on.png'),('合成规则','05_rules_complete.png')]
html='''<!doctype html><html lang="zh-CN"><meta charset="utf-8"><title>Unity 换皮运行验收</title><style>body{background:#102637;color:#e6f5ff;font:16px system-ui;margin:32px}h1{font-size:26px}section{margin:40px 0}div{display:flex;gap:20px}figure{margin:0;width:min(42vw,375px)}img{width:100%;height:auto}figcaption{padding:10px 0;color:#a6d6ef}a{color:#80daff}</style><h1>五页实际 Unity 运行对比</h1><p>左：原版工程副本；右：本次换皮工程。750 × 1624，真实 PlayMode 截图；同一测试配置，随机广播与物理落位可能不同。</p><p>硬币按图五的 11 面额样式重制，保留现有布局、动态文字和原生合成逻辑。此页面不是参考图贴图模拟。</p>'''
for title,f in pages:
 html+=f'<section><h2>{title}</h2><div><figure><figcaption>原版</figcaption><a href="BaselineCapture/07_Verification/ReskinRuntime/{f}"><img src="BaselineCapture/07_Verification/ReskinRuntime/{f}"></a></figure><figure><figcaption>换皮实际运行</figcaption><a href="07_Verification/ReskinRuntime/{f}"><img src="07_Verification/ReskinRuntime/{f}"></a></figure></div></section>'
html+='<p>编辑工程：UnityRemainingWorkingCopy；场景：Assets/Scenes/RecoveredMain.unity。点击截图查看原尺寸。</p></html>'
(LOCAL/'review_final.html').write_text(html,encoding='utf-8')
print(json.dumps({k:v for k,v in report.items() if k!='files'},ensure_ascii=False,indent=2))
