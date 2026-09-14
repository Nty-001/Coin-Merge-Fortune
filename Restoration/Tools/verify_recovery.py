import json,re,hashlib,collections,sys
from pathlib import Path
R=Path(__file__).resolve().parents[2];O=R/'Restoration';U=O/'06_UnityFramework'
sys.path.insert(0,str(R/'paid_ui_work/python_deps'))
import yaml
def read(p):return json.loads(p.read_text(encoding='utf-8-sig'))
errors=[];missing=[];stats=[]
def walk(x):
    if isinstance(x,dict):
        yield x
        for v in x.values():yield from walk(v)
    elif isinstance(x,list):
        for v in x:yield from walk(v)
for variant in ['Packaged','HotUpdate']:
    files=list((O/'05_PrefabModel'/variant/'Decoded').glob('*.json'));known={p.stem for p in files};refs=0
    for p in files:
        d=read(p)
        for v in walk(d['objects']):
            if '$ref' in v and not 0<=v['$ref']<len(d['objects']):errors.append({'file':str(p),'badObjectReference':v['$ref']})
            if '$assetPending' in v:errors.append({'file':str(p),'unresolvedPendingAsset':v})
            if '$asset' in v:
                refs+=1
                if v['$asset'] not in known:missing.append({'variant':variant,'source':p.stem,'target':v['$asset']})
    stats.append({'variant':variant,'decodedAssets':len(files),'assetReferences':refs})
guids={}
for p in (U/'Assets').rglob('*.meta'):
    m=re.search(r'^guid: (\w+)',p.read_text(encoding='utf-8'),re.M)
    if m:
        if m[1] in guids:errors.append({'duplicateGuid':m[1]})
        guids[m[1]]=str(p)
for p in Path('C:/Program Files/Unity/Hub/Editor/2022.3.62f3c1/Editor/Data/Resources/PackageManager/BuiltInPackages').rglob('*.cs.meta'):
    m=re.search(r'^guid: (\w+)',p.read_text(encoding='utf-8'),re.M)
    if m:guids[m[1]]=str(p)
counts={}
for p in list((U/'Assets/Prefabs').rglob('*.prefab'))+list((U/'Assets/Scenes').glob('*.unity')):
    text=p.read_text(encoding='utf-8');ids=set(map(int,re.findall(r'^--- !u!\d+ &(\d+)',text,re.M)))
    raw=re.sub(r'^%.*\n','',text,flags=re.M);raw=re.sub(r'^--- !u!\d+ &\d+','---',raw,flags=re.M)
    docs=list(yaml.safe_load_all(raw));roots=0
    for doc in docs:
        if 'RectTransform' in doc and doc['RectTransform']['m_Father']['fileID']==0:roots+=1
        for v in walk(doc):
            if 'fileID' in v and v['fileID']:
                if 'guid' in v:
                    if v['guid'] not in guids:errors.append({'file':str(p),'missingGuid':v['guid']})
                elif v['fileID'] not in ids:errors.append({'file':str(p),'missingLocalFileId':v['fileID']})
    if p.suffix=='.prefab' and roots!=1:errors.append({'file':str(p),'prefabRootCount':roots})
    counts[str(p.relative_to(U)).replace('\\','/')]=len(docs)
result={'assetGraphs':stats,'missingAssetReferences':missing,'errors':errors,'nativeYamlFiles':len(counts),'nativeYamlObjects':sum(counts.values()),'nativeYamlParseAndReferenceCheck':not errors,'editorImported':False,'editorBlocker':'No valid Unity Editor license'}
(O/'07_Verification/structural_validation.json').write_text(json.dumps(result,ensure_ascii=False,indent=2),encoding='utf-8')
print({k:v for k,v in result.items() if k not in ['missingAssetReferences','errors']});print('errors',len(errors),'missingAssetReferences',len(missing))
if errors:print(errors[:5]);sys.exit(1)
