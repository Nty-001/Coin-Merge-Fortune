import json,shutil,collections,re,hashlib
from pathlib import Path
from PIL import Image,ImageDraw,ImageFont
R=Path(__file__).resolve().parents[2];O=R/'Restoration'
def read(p):return json.loads(p.read_text(encoding='utf-8'))
def write(p,v):p.parent.mkdir(parents=True,exist_ok=True);p.write_text(json.dumps(v,ensure_ascii=False,indent=2),encoding='utf-8')
def safe(s):return re.sub(r'[<>:"\\|?*]','_',s).strip('/.')
errors=[];counts=[]
for variant in ['Packaged','HotUpdate']:
    docs={p.stem:read(p) for p in (O/'05_PrefabModel'/variant/'Decoded').glob('*.json')}
    target=O/'04_Assets'/variant;native=target/'Native';sprites=[];skeletons=[];clips=[]
    for uid,d in docs.items():
        obj=d['objects'][d['root']];typ=obj['__type__'];data=obj.get('data',{})
        if typ=='cc.SpriteFrame':
            try:
                texture=obj['_textureSetter']['$asset'];files=list((native/texture).glob('*'))
                source=next(p for p in files if p.suffix.lower() in ['.png','.jpg','.jpeg','.webp'])
                image=Image.open(source).convert('RGBA');x,y,w,h=data['rect'];rot=data.get('rotated',False)
                patch=image.crop((x,y,x+(h if rot else w),y+(w if rot else h)))
                if rot:patch=patch.transpose(Image.Transpose.ROTATE_90)
                ow,oh=map(int,data.get('originalSize',[w,h]));ox,oy=data.get('offset',[0,0])
                canvas=Image.new('RGBA',(max(1,ow),max(1,oh)));left=round((ow-w)/2+ox);top=round((oh-h)/2-oy)
                canvas.paste(patch,(left,top))
                name=next(iter(d.get('paths',[])),data.get('name',uid));relative=f'Sprites/{safe(name)}__{uid[:8]}.png'
                path=target/relative;path.parent.mkdir(parents=True,exist_ok=True);canvas.save(path)
                sprites.append({'uuid':uid,'name':name,'file':relative,'textureUuid':texture,'rect':data['rect'],'rotated':rot,'offset':[ox,oy],'originalSize':[ow,oh],'capInsets':data.get('capInsets',[0]*4),'decodedSource':d['source']})
            except Exception as e:errors.append({'variant':variant,'uuid':uid,'error':repr(e)})
        elif typ=='sp.SkeletonData':
            path=target/'SkeletalSource'/f'{uid}.json';write(path,obj);skeletons.append({'uuid':uid,'paths':d.get('paths',[]),'file':str(path.relative_to(target)),'keys':list(obj)})
        elif typ=='cc.AnimationClip':
            path=target/'AnimationSource'/f'{uid}.json';write(path,obj);clips.append({'uuid':uid,'name':obj.get('_name',data.get('name',uid)),'file':str(path.relative_to(target))})
        elif typ in ('cc.AudioClip','cc.TTFFont'):
            name=next(iter(d.get('paths',[])),obj.get('_name',data.get('name',uid)))
            for source in (native/uid).glob('*'):
                path=target/('Audio' if typ=='cc.AudioClip' else 'Fonts')/(safe(name)+source.suffix)
                path.parent.mkdir(parents=True,exist_ok=True);shutil.copy2(source,path)
    write(target/'sprites.json',sprites);write(target/'skeletal_inventory.json',skeletons);write(target/'animation_inventory.json',clips)
    # Recovered font from the application's expanded writable native cache.
    if variant=='HotUpdate':
        for p in (R/'export_20260914/snapshot/data/user/0/com.mergecoin.cotune.tuneco/app_coin').rglob('*.ttf'):
            dest=target/'Fonts'/p.name;dest.parent.mkdir(parents=True,exist_ok=True);shutil.copy2(p,dest)
    for page,start in enumerate(range(0,len(sprites),90)):
        subset=sprites[start:start+90];sheet=Image.new('RGB',(1200,((len(subset)+8)//9)*120),(235,237,242));draw=ImageDraw.Draw(sheet)
        for i,s in enumerate(subset):
            x=(i%9)*133;y=(i//9)*120;im=Image.open(target/s['file']);im.thumbnail((118,90));sheet.paste(im,(x+(125-im.width)//2,y),im)
            draw.text((x+3,y+92),s['uuid'][:8],fill='black');draw.text((x+3,y+105),s['name'].split('/')[-1][:18],fill='black')
        dest=target/f'ContactSheets/page_{page+1:02}.jpg';dest.parent.mkdir(parents=True,exist_ok=True);sheet.save(dest,quality=90)
    counts.append({'variant':variant,'sprites':len(sprites),'skeletalAssets':len(skeletons),'animationClips':len(clips)})
write(O/'07_Verification/art_export_errors.json',errors);write(O/'07_Verification/art_summary.json',counts)
print(json.dumps({'counts':counts,'errors':errors},ensure_ascii=False))
