"""Verify a YAML formatting-only edit against the committed baseline."""
import json,re,subprocess,sys
from pathlib import Path
ROOT=Path(__file__).resolve().parents[2]
sys.path.insert(0,str(ROOT/'paid_ui_work/python_deps'))
import yaml
GIT='C:/Users/001/.cache/codex-runtimes/codex-primary-runtime/dependencies/native/git/cmd/git.exe'
PROJECT=ROOT/'Restoration/06_UnityFramework'
def decode(text):
    blocks=re.split(r'(?m)^(--- !u!\d+ &\d+)\s*\n',text)
    return [(blocks[i],yaml.safe_load(blocks[i+1])) for i in range(1,len(blocks),2)]
entries=[]
for part,pattern in [('Prefabs','*.prefab'),('Scenes','*.unity')]:
    for path in sorted((PROJECT/'Assets'/part).rglob(pattern)):
        rel=path.relative_to(ROOT).as_posix()
        before=subprocess.check_output([GIT,'show','HEAD:'+rel],cwd=ROOT).decode('utf-8-sig')
        after=path.read_text(encoding='utf-8-sig')
        a,b=decode(before),decode(after)
        if a!=b:raise AssertionError('Serialized values changed: '+rel)
        entries.append({'path':rel,'serializedObjects':len(b),'allValuesUnchanged':True})
output=ROOT/'Restoration/07_Verification/yaml_format_preservation.json'
output.write_text(json.dumps(entries,indent=2),encoding='utf-8')
print(json.dumps({'files':len(entries),'allSerializedValuesUnchanged':True}))
