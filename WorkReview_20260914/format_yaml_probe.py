"""Re-encode Unity YAML in the disposable review copy, preserving parsed data exactly."""
import json,re,sys
from pathlib import Path
ROOT=Path(__file__).resolve().parents[1]
sys.path.insert(0,str(ROOT/'paid_ui_work/python_deps'))
import yaml

OUT=Path(__file__).resolve().parent
PROJECT=OUT/'UnityImportReview'
assert PROJECT.resolve().parent==OUT.resolve()
entries=[]
for base,extension in [('Prefabs','*.prefab'),('Scenes','*.unity')]:
    for path in sorted((PROJECT/'Assets'/base).rglob(extension)):
        text=path.read_text(encoding='utf-8-sig')
        blocks=re.split(r'(?m)^(--- !u!\d+ &\d+)\s*\n',text)
        assert len(blocks)>2
        output=['%YAML 1.1','%TAG !u! tag:unity3d.com,2011:']
        count=0
        for i in range(1,len(blocks),2):
            original=yaml.safe_load(blocks[i+1])
            replacement=yaml.safe_dump(original,allow_unicode=True,sort_keys=False,width=1000000)
            assert yaml.safe_load(replacement)==original,path
            output.extend([blocks[i],replacement.rstrip()])
            count+=1
        path.write_text('\n'.join(output)+'\n',encoding='utf-8')
        entries.append({'path':path.relative_to(PROJECT).as_posix(),'objectCount':count,'parsedValuesUnchanged':True})
(OUT/'yaml_format_probe_manifest.json').write_text(json.dumps(entries,indent=2),encoding='utf-8')
print(json.dumps({'files':len(entries),'objectValuesUnchanged':True,'target':'review copy only'}))
