"""Recover Cocos Creator compiled format v1 using original engine module 254.
Every unsupported value fails loudly; original payloads remain in the evidence export.
"""
import json,base64,copy,collections,hashlib,re,shutil,csv
from pathlib import Path
R=Path(__file__).resolve().parents[2]; OUT=R/'Restoration'
def read(p):return json.loads(p.read_text(encoding='utf-8-sig'))
def write(p,v):p.parent.mkdir(parents=True,exist_ok=True);p.write_text(json.dumps(v,ensure_ascii=False,indent=2),encoding='utf-8')
def uuid(s):
    if len(s) not in (22,23):return s
    n=2 if len(s)==22 else 5
    chars='ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/'
    h=s[:n]
    for i in range(n,len(s),2):
        a,b=chars.index(s[i]),chars.index(s[i+1]);h+=f'{a>>2:x}{((a&3)<<2)|(b>>4):x}{b&15:x}'
    return '-'.join([h[:8],h[8:12],h[12:16],h[16:20],h[20:]])
def safe(s):return re.sub(r'[<>:"\\|?*]','_',s).strip('/.')
class Decoder:
    def __init__(self,data):
        self.d=copy.deepcopy(data);self.objects=[];self.owners={}
    def new(self,typ,body=None,idx=None):
        obj={'__type__':typ}
        if body is not None:obj['data']=body
        if idx is None:idx=len(self.objects);self.objects.append(obj)
        else:self.objects[idx]=obj
        return obj,{'$ref':idx}
    def setref(self,o,k,n):
        if n>=0:o[k]={'$ref':n}
        else:self.owners[~n]=o
    def typed(self,typ,o,k,n):
        if typ==0:o[k]=n
        elif typ==1:self.setref(o,k,n)
        elif typ in (2,3,9):
            o[k]=list(n)
            for i,v in enumerate(n):self.typed({2:1,3:6,9:4}[typ],o[k],i,v)
        elif typ==4:o[k]=self.instance(n)
        elif typ in (5,8):
            names=['cc.Vec2','cc.Vec3','cc.Vec4','cc.Quat','cc.Color','cc.Size','cc.Rect','cc.Mat4']
            o[k]={'__type__':names[n[0]],'values':n[1:]}
        elif typ==6:
            o[k]={'$assetPending':n};self.assetowners[n]=o
        elif typ==7:o[k]=n
        elif typ==10:
            obj,ref=self.new(self.d[3][n[0]],n[1]);o[k]=ref
        elif typ==11:
            o[k]=n[0]
            for i in range(1,len(n),3):self.typed(n[i+1],o[k],n[i],n[i+2])
        elif typ==12:
            o[k]=n[0]
            for i,v in enumerate(list(n[0])):
                if n[i+1]:self.typed(n[i+1],o[k],i,v)
        else:raise ValueError(f'Unknown type {typ}')
    def instance(self,encoded,idx=None):
        mask=self.d[4][encoded[0]];cls=self.d[3][mask[0]]
        o,ref=self.new(cls[0],idx=idx);props=cls[1];offset=cls[2];cutoff=mask[-1]
        for i in range(1,len(encoded)):
            p=mask[i];k=props[p]
            if i<cutoff:o[k]=encoded[i]
            else:self.typed(cls[p+offset],o,k,encoded[i])
        return ref
    def decode(self):
        d=self.d;data=d[5];types=d[6] or [];root=0;last=data[-1] if data else None
        count=len(data)
        native=isinstance(last,int)
        if native:root=~last if last<0 else last;count-=1
        normal=count-len(types);self.objects=[None]*count
        self.assetowners={}
        for i in range(normal):self.instance(data[i],i)
        for j,t in enumerate(types):
            i=normal+j
            if t>=0:self.new(d[3][t],data[i],i)
            else:self.typed(~t,self.objects,i,data[i])
        refs=d[7]
        if refs:
            for j in range(0,len(refs)-1,3):
                a,k,b=refs[j:j+3]
                o=self.owners[j//3] if j<3*refs[-1] else self.objects[a]
                key=d[2][k] if k>=0 else ~k
                o[key]={'$ref':b}
        for j,(owner,k,u) in enumerate(zip(d[8],d[9],d[10])):
            o=self.assetowners[j] if j in self.assetowners else self.objects[owner]
            key=(d[2][k] if k>=0 else ~k) if isinstance(k,int) else k
            asset=d[1][u] if isinstance(u,int) else u
            o[key]={'$asset':uuid(asset)}
        return {'root':root,'nativeDependency':bool(native and last<0),'objects':self.objects}

summary=[]; assetrows=[]; errors=[]
for variant,base in [('Packaged',R/'export_20260914/unpacked/base/assets'),('HotUpdate',R/'export_20260914/unpacked/runtime_adaljkjf')]:
    decoded={};aliases={};native={};meta={}
    for folder in sorted((base/'assets').iterdir()):
        configs=list(folder.glob('config.*.json'))
        if not configs:continue
        config=read(configs[0]);uids=[uuid(s) for s in config['uuids']]
        for idx,v in config.get('paths',{}).items():
            aliases.setdefault(uids[int(idx)],[]).append(v[0]);meta[uids[int(idx)]]={'type':config['types'][v[1]],'bundle':folder.name}
        for name,idx in config.get('scenes',{}).items():
            aliases.setdefault(uids[idx],[]).append(name.replace('db://assets/',''));meta[uids[idx]]={'type':'cc.SceneAsset','bundle':folder.name}
        packs=config.get('packs',{})
        for p in sorted(folder.glob('import/*/*.json')):
            data=read(p);stem=p.name.split('.')[0]
            try:
                if stem in packs:
                    ids=packs[stem]
                    if isinstance(data,dict) and data.get('type')=='cc.Texture2D':
                        payloads=data['data'].split('|')
                        if len(payloads)!=len(ids):raise ValueError('Texture pack section mismatch')
                        parts=[[1,[],[],['cc.Texture2D'],0,[s,-1],[0],0,[],[],[]] for s in payloads]
                    else:
                        if len(data)!=6 or len(ids)!=len(data[5]):raise ValueError('Pack section mismatch')
                        parts=[data[:5]+part for part in data[5]]
                    keys=[uids[i] if isinstance(i,int) else uuid(i) for i in ids]
                else:parts=[data];keys=[stem]
                for key,part in zip(keys,parts):
                    doc=Decoder(part).decode();doc['uuid']=key;doc['source']=str(p.relative_to(R)).replace('\\','/')
                    if key in decoded and decoded[key]['objects']!=doc['objects']:errors.append({'variant':variant,'uuid':key,'error':'Different duplicate payload'})
                    decoded[key]=doc
            except Exception as e:errors.append({'file':str(p.relative_to(R)),'error':repr(e)})
        for p in folder.glob('native/**/*'):
            if p.is_file():
                key=p.relative_to(folder/'native').parts[1].split('.')[0]
                native.setdefault(key,[]).append(p)
    # Expanded runtime fonts/audio are additional exact original bytes.
    types=collections.Counter();prefabs=[]
    for key,doc in decoded.items():
        doc['paths']=aliases.get(key,[])
        write(OUT/'05_PrefabModel'/variant/'Decoded'/f'{key}.json',doc)
        obj=doc['objects'][doc['root']];typ=obj.get('__type__','unknown');types[typ]+=1
        name=next(iter(doc['paths']),obj.get('_name') or obj.get('data',{}).get('name',key) if isinstance(obj.get('data',{}),dict) else key)
        if typ=='cc.JsonAsset':
            body=obj.get('json',obj.get('data'))
            if isinstance(body,dict) and 'json' in body:body=body['json']
            write(OUT/'03_Configuration'/variant/(safe(name)+'.json'),body)
        if typ in ('cc.Prefab','cc.SceneAsset'):
            prefabs.append({'uuid':key,'name':name,'type':typ,'objects':len(doc['objects']),'source':doc['source']})
        for p in native.get(key,[]):
            target=OUT/'04_Assets'/variant/'Native'/key/p.name
            target.parent.mkdir(parents=True,exist_ok=True);shutil.copy2(p,target)
            assetrows.append({'variant':variant,'uuid':key,'type':typ,'path':';'.join(doc['paths']),'file':str(target.relative_to(OUT)).replace('\\','/'),'sha256':hashlib.sha256(p.read_bytes()).hexdigest()})
    for key,files in native.items():
        if key in decoded:continue
        for p in files:
            target=OUT/'04_Assets'/variant/'Native'/key/p.name;target.parent.mkdir(parents=True,exist_ok=True);shutil.copy2(p,target)
            assetrows.append({'variant':variant,'uuid':key,'type':'native without import root','path':';'.join(aliases.get(key,[])),'file':str(target.relative_to(OUT)).replace('\\','/'),'sha256':hashlib.sha256(p.read_bytes()).hexdigest()})
    write(OUT/'05_PrefabModel'/variant/'prefab_inventory.json',prefabs)
    write(OUT/'04_Assets'/variant/'aliases.json',aliases)
    summary.append({'variant':variant,'decodedAssets':len(decoded),'types':dict(types),'prefabsAndScenes':len(prefabs),'nativeFiles':sum(map(len,native.values()))})
write(OUT/'07_Verification/asset_summary.json',summary);write(OUT/'07_Verification/decode_errors.json',errors)
with (OUT/'04_Assets/asset_inventory.csv').open('w',newline='',encoding='utf-8-sig') as f:
    w=csv.DictWriter(f,fieldnames=assetrows[0].keys());w.writeheader();w.writerows(assetrows)
print(json.dumps({'summary':summary,'errors':len(errors)},ensure_ascii=False))
