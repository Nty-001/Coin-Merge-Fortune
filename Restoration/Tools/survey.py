import json,sqlite3,collections
from pathlib import Path
R=Path(__file__).resolve().parents[2]
out=R/'Restoration'
for variant,base in [('Packaged',R/'export_20260914/unpacked/base/assets'),('HotUpdate',R/'export_20260914/unpacked/runtime_adaljkjf')]:
    for folder in (base/'assets').iterdir():
        configs=list(folder.glob('config.*.json'))
        if not configs:continue
        c=json.loads(configs[0].read_text())
        print(variant,folder.name,'paths',len(c.get('paths',{})),'types',c.get('types'), 'scenes',c.get('scenes'))
        for k,v in c.get('paths',{}).items():
            if c['types'][v[1]] in ['cc.JsonAsset','cc.Prefab','cc.AnimationClip','sp.SkeletonData']:print(' ',k,v,c['uuids'][int(k)])
        for p in folder.glob('import/*/*.json'):
            j=json.loads(p.read_text(encoding='utf-8'))
            if len(j)==6:
                print(' PACK',p.name,'shared classes',str(j[3])[:250],'sections',len(j[5]),'first',str(j[5][0])[:300])
                break
c=sqlite3.connect(R/'export_20260914/snapshot/data/user/0/com.mergecoin.cotune.tuneco/databases/jsb.sqlite')
target=out/'03_Configuration/CapturedDevice';target.mkdir(parents=True,exist_ok=True)
for k,v in c.execute('select key,value from data'):
    try:j=json.loads(v)
    except ValueError:j=v
    (target/(k+'.json')).write_text(json.dumps(j,ensure_ascii=False,indent=2),encoding='utf-8')
    if k=='coinGameData': print('SERVER CONFIG ROOT',list(j.keys()))
