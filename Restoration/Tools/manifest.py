import json,hashlib,csv
from pathlib import Path
R=Path(__file__).resolve().parents[1]
rows=[]
for p in sorted(R.rglob('*')):
    if not p.is_file():continue
    rel=p.relative_to(R).as_posix()
    if rel.startswith('Tools/python_deps/') or '__pycache__/' in rel or rel in ['07_Verification/file_manifest.csv','07_Verification/delivery_size.json']:continue
    if '/Library/' in rel or '/Temp/' in rel or '/Logs/' in rel or '/obj/' in rel:continue
    h=hashlib.sha256()
    with p.open('rb') as f:
        for chunk in iter(lambda:f.read(1024*1024),b''):h.update(chunk)
    rows.append({'path':rel,'bytes':p.stat().st_size,'sha256':h.hexdigest()})
with (R/'07_Verification/file_manifest.csv').open('w',newline='',encoding='utf-8-sig') as f:
    w=csv.DictWriter(f,fieldnames=rows[0].keys());w.writeheader();w.writerows(rows)
stats={'files':len(rows),'bytes':sum(r['bytes'] for r in rows),'excludes':['tool dependency packages','Unity generated Library/Temp/Logs','manifest itself']}
(R/'07_Verification/delivery_size.json').write_text(json.dumps(stats,indent=2),encoding='utf-8');print(stats)
