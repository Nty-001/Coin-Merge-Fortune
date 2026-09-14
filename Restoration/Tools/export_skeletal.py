import json,re,collections
from pathlib import Path
from PIL import Image
R=Path(__file__).resolve().parents[2];O=R/'Restoration/04_Assets/HotUpdate';out=O/'SkeletalReusable'
def write(p,v):p.parent.mkdir(parents=True,exist_ok=True);p.write_text(json.dumps(v,ensure_ascii=False,indent=2),encoding='utf-8')
def safe(s):return re.sub(r'[<>:"\\|?*]','_',s)
inventory=[];errors=[]
for p in (O/'SkeletalSource').glob('*.json'):
    obj=json.loads(p.read_text(encoding='utf-8'));name=obj['_name'];dest=out/name;dest.mkdir(parents=True,exist_ok=True)
    skeleton=obj['_skeletonJson'];skeleton=json.loads(skeleton) if isinstance(skeleton,str) else skeleton
    write(dest/'skeleton.json',skeleton);(dest/'skeleton.atlas').write_text(obj['_atlasText'],encoding='utf-8')
    textures={}
    for n,t in zip(obj['textureNames'],obj['textures']):
        file=next((O/'Native'/t['$asset']).glob('*.png'))
        image=Image.open(file).convert('RGBA');textures[n]=image;image.save(dest/n)
    regions=[];current=None;page=None
    for raw in obj['_atlasText'].splitlines()+['']:
        line=raw.strip()
        if not line:continue
        if not raw.startswith((' ','\t')) and ':' not in line:
            if line in textures:page=line;current=None
            else:
                current={'name':line,'page':page};regions.append(current)
        elif current is not None and ':' in line:
            key,val=line.split(':',1);current[key]=val.strip()
    for region in regions:
        try:
            x,y=map(int,region['xy'].split(','));w,h=map(int,region['size'].split(','));ow,oh=map(int,region.get('orig',region['size']).split(','));ox,oy=map(int,region.get('offset','0,0').split(','));rot=region.get('rotate')=='true'
            im=textures[region['page']].crop((x,y,x+(h if rot else w),y+(w if rot else h)))
            if rot:im=im.transpose(Image.Transpose.ROTATE_90)
            canvas=Image.new('RGBA',(max(1,ow),max(1,oh)));canvas.paste(im,(ox,oh-h-oy))
            path=dest/'Regions'/(safe(region['name'])+'.png');path.parent.mkdir(parents=True,exist_ok=True);canvas.save(path);region['file']=str(path.relative_to(dest)).replace('\\','/')
        except Exception as e:errors.append({'name':name,'region':region['name'],'error':repr(e)})
    write(dest/'regions.json',regions)
    types=collections.Counter();skins=skeleton.get('skins',{})
    skinlist=skins.values() if isinstance(skins,dict) else [s.get('attachments',{}) for s in skins]
    for skin in skinlist:
        for slot in skin.values():
            for attach in slot.values():types[attach.get('type','region')]+=1
    clips=[]
    for clip,body in skeleton.get('animations',{}).items():
        def times(x):
            if isinstance(x,dict):
                for k,v in x.items():
                    if k=='time' and isinstance(v,(int,float)):yield v
                    else:yield from times(v)
            elif isinstance(x,list):
                for v in x:yield from times(v)
        clips.append({'name':clip,'duration':max(times(body),default=0),'channels':list(body)})
        write(dest/'Timelines'/(safe(clip)+'.json'),body)
    item={'uuid':p.stem,'name':name,'spineVersion':skeleton.get('skeleton',{}).get('spine'),'bones':len(skeleton.get('bones',[])),'regions':len(regions),'attachmentTypes':dict(types),'ikConstraints':len(skeleton.get('ik',[])),'transformConstraints':len(skeleton.get('transform',[])),'animations':clips}
    inventory.append(item)
write(out/'inventory.json',inventory);write(R/'Restoration/07_Verification/skeletal_errors.json',errors)
print(json.dumps(inventory,ensure_ascii=False))
