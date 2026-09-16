"""Slice AI-authored artwork into Unity sprites; never changes gameplay geometry.
RGB checkerboards in the AI source are excluded using a foreground contour matte.
Edges are inset, antialiased, RGB-dilated, and resized with premultiplied alpha.
"""
from pathlib import Path
import json
from collections import deque
import numpy as np
from PIL import Image, ImageFilter, ImageDraw

ROOT=Path(__file__).resolve().parents[3]
OUT=ROOT/'paid_ui_work/reskin_20260916'
PROJECT=OUT/'UnityWorkingCopy'
SRC=Path(__file__).resolve().parents[1]/'ArtSources'
DST=PROJECT/'Assets/Resources/Reskin'
DST.mkdir(parents=True,exist_ok=True)
VALUES=[1,2,5,10,20,50,100,200,500,1000,2000]
records=[]

def resize(im,size):
    return im.convert('RGBa').resize(size,Image.Resampling.LANCZOS).convert('RGBA')

def fill_holes(mask):
    h,w=mask.shape; outside=np.zeros((h,w),bool); q=deque()
    for x in range(w):
        for y in (0,h-1):
            if not mask[y,x]: outside[y,x]=1;q.append((y,x))
    for y in range(h):
        for x in (0,w-1):
            if not mask[y,x] and not outside[y,x]:outside[y,x]=1;q.append((y,x))
    while q:
        y,x=q.popleft()
        for yy,xx in ((y-1,x),(y+1,x),(y,x-1),(y,x+1)):
            if 0<=yy<h and 0<=xx<w and not mask[yy,xx] and not outside[yy,xx]:
                outside[yy,xx]=1;q.append((yy,xx))
    return ~outside

def dilate_rgb(rgb,mask,steps=5):
    rgb=rgb.copy(); valid=mask.copy()
    for _ in range(steps):
        old=valid.copy(); sums=np.zeros_like(rgb,dtype=np.float32); counts=np.zeros(mask.shape,np.float32)
        for dy,dx in ((1,0),(-1,0),(0,1),(0,-1)):
            m=np.roll(old,(dy,dx),(0,1)); c=np.roll(rgb,(dy,dx),(0,1));sums+=c*m[:,:,None];counts+=m
        new=(~old)&(counts>0);rgb[new]=(sums[new]/counts[new,None]).astype(np.uint8);valid[new]=1
    return rgb

def matte(im):
    rgb=np.array(im.convert('RGB')); spread=rgb.max(2).astype(int)-rgb.min(2).astype(int)
    colored=spread>23
    # Close only subpixel breaks in the colored outer outline, then fill interior whites.
    m=Image.fromarray((colored*255).astype('uint8')).filter(ImageFilter.MaxFilter(3)).filter(ImageFilter.MinFilter(3))
    solid=fill_holes(np.array(m)>127)
    m=Image.fromarray((solid*255).astype('uint8')).filter(ImageFilter.MinFilter(3))
    core=np.array(m)>127
    rgb=dilate_rgb(rgb,core,6)
    alpha=np.array(m.filter(ImageFilter.GaussianBlur(.45)))
    alpha[alpha<4]=0
    result=Image.fromarray(np.dstack([rgb,alpha]))
    box=result.getchannel('A').point(lambda a:255 if a>=16 else 0).getbbox()
    if not box:raise ValueError('Empty alpha')
    l,t,r,b=box;return result.crop((max(0,l-2),max(0,t-2),min(im.width,r+2),min(im.height,b+2)))

def save(im,path):
    path=Path(path);path.parent.mkdir(parents=True,exist_ok=True);im.save(path)
    a=im.getchannel('A');b={str(n):a.point(lambda v:255 if v>=n else 0).getbbox() for n in (1,4,8,16)}
    records.append({'path':str(path.relative_to(PROJECT)),'size':im.size,'alphaBounds':b})

coins={}
sheet=Image.open(SRC/'coins.png')
coin_boxes=[(40,42,325,330),(394,41,694,333),(754,42,1055,334),(1118,40,1418,334),(27,372,332,683),(393,374,696,682),(744,371,1059,687),(1111,372,1417,687),(26,717,341,1039),(383,717,702,1040),(738,717,1061,1040)]
def clean_shape(im,box,kind='ellipse',radius=40):
    # Analytic subpixel silhouette: no checker-derived mask is allowed into final assets.
    l,t,r,b=box;im=im.crop((l+3,t+3,r-3,b-3)).convert('RGBA');w,h=im.size
    hi=Image.new('L',(w*4,h*4));d=ImageDraw.Draw(hi)
    if kind=='ellipse':d.ellipse((2,2,w*4-3,h*4-3),fill=255)
    else:d.rounded_rectangle((2,2,w*4-3,h*4-3),radius=radius*4,fill=255)
    a=hi.resize((w,h),Image.Resampling.LANCZOS);im.putalpha(a)
    return im
for i,value in enumerate(VALUES):
    x=i%4;y=i//4;cell=sheet.crop((round(x*sheet.width/4),round(y*sheet.height/3),round((x+1)*sheet.width/4),round((y+1)*sheet.height/3)))
    coin=clean_shape(sheet,coin_boxes[i]);coins[value]=coin
    save(coin,DST/f'Coin{value}.png')
    target=PROJECT/f'Assets/Resources/Gameplay/Coins/{value}.png'; old=Image.open(target)
    save(resize(coin,old.size),target)

ui=[];sheet=Image.open(SRC/'ui_key.png')
ui_boxes=[(12,118,310,257,68),(325,117,620,257,40),(638,114,934,259,42),(638,114,934,259,42),(49,369,272,594,50),(363,368,585,594,50),(673,369,901,600,0),(980,367,1213,601,0),(24,719,297,865,69),(333,718,609,865,70),(638,752,932,830,37),(954,752,1245,830,37),(60,960,259,1190,35),(352,957,578,1188,85),(663,954,910,1198,0),(974,953,1223,1200,0)]
for i in range(16):
    x=i%4;y=i//4;cell=sheet.crop((round(x*sheet.width/4),round(y*sheet.height/4),round((x+1)*sheet.width/4),round((y+1)*sheet.height/4)))
    rgb=np.array(cell.convert('RGB'));key=np.minimum(rgb[:,:,0],rgb[:,:,2]).astype(int)-rgb[:,:,1].astype(int)
    mask=Image.fromarray(((key<160)*255).astype('uint8')).filter(ImageFilter.MinFilter(3))
    core=np.array(mask)>127;a=np.array(mask.filter(ImageFilter.GaussianBlur(.5)));a[a<4]=0
    rgb=dilate_rgb(rgb,core,8);im=Image.fromarray(np.dstack([rgb,a]));box=im.getchannel('A').getbbox();im=im.crop(box)
    ui.append(im);save(im,DST/f'UI{i}.png')
sky=Image.open(SRC/'sky.png').convert('RGBA');save(sky,DST/'Sky.png')

# Keep the original runtime atlas UVs and exact animation data, replacing only art.
def regions(name):return json.loads((PROJECT/f'Assets/Art/HotUpdate/SkeletalReusable/{name}/regions.json').read_text('utf-8-sig'))
def atlas_patch(name,mapping):
    target=PROJECT/f'Assets/Resources/Skeletal/Atlases/{name}.png';atlas=Image.open(ROOT/'Restoration/06_UnityFramework'/target.relative_to(PROJECT)).convert('RGBA')
    for r in regions(name):
        if r['name'] not in mapping:continue
        src=mapping[r['name']];x,y=map(int,r['xy'].split(','));w,h=map(int,r['size'].split(','))
        patch=resize(src,(w,h))
        # Spine's packed true rotation is 90 degrees counter-clockwise.
        if r['rotate']=='true':patch=patch.transpose(Image.Transpose.ROTATE_90)
        atlas.paste(patch,(x,y))
    save(atlas,target)

atlas_patch('HeChengSM_TX',{f'11_{i:02}':coins[v] for i,v in enumerate(reversed(VALUES))})
atlas_patch('AnNiu_TX',{'an_00':coins[2000]})
atlas_patch('DJB_TX',{'JB_01':coins[2000],'JB_02':coins[1000],'JB_03':coins[1000],'JB_04':coins[500],'JB_05':coins[500],'JB_06':coins[200],'JB_07':coins[200]})

# Re-register the AI-painted machine against the existing packed UV silhouette.
target=PROJECT/'Assets/Resources/Skeletal/Atlases/LaoHuJ_TX.png';old=Image.open(ROOT/'Restoration/06_UnityFramework'/target.relative_to(PROJECT)).convert('RGBA')
paint=Image.open(SRC/'slot.png').convert('RGB').resize(old.size,Image.Resampling.LANCZOS)
rgb=np.array(paint);spread=rgb.max(2).astype(int)-rgb.min(2).astype(int)
valid=fill_holes(spread>35)
valid=np.array(Image.fromarray((valid*255).astype('uint8')).filter(ImageFilter.MinFilter(7)))>127
rgb=dilate_rgb(rgb,valid,25)
save(Image.fromarray(np.dstack([rgb,np.array(old)[:,:,3]])),target)

# Coin decorations have no dynamic text: compose only from the approved coin family.
def fan(name,size,values,boxes):
    im=Image.new('RGBA',size)
    for v,(x,y,w,h) in zip(values,boxes):im.alpha_composite(resize(coins[v],(w,h)),(x,y))
    save(im,DST/(name+'.png'))
fan('CashCoins',(716,392),[1000,500,2000],[(0,112,250,260),(490,150,220,235),(192,0,380,390)])
fan('RuleCoins',(1204,400),[5,10,100,20,500,2000],[(0,230,160,160),(990,220,185,180),(800,130,265,265),(130,175,225,225),(310,90,300,310),(530,0,385,400)])

# Composite blank panel surfaces from generated parts. These do not contain text.
def panel(name,w,h,head):
    im=nine(ui[1],w,h,40);im.alpha_composite(nine(ui[2],w-16,head,36),(8,8));save(im,DST/(name+'.png'))
def nine(im,w,h,edge):
    sw,sh=im.size;cut=min(45,sh//3);out=Image.new('RGBA',(w,h))
    xs=[0,cut,sw-cut,sw];ys=[0,cut,sh-cut,sh];tx=[0,edge,w-edge,w];ty=[0,edge,h-edge,h]
    for iy in range(3):
        for ix in range(3):out.paste(resize(im.crop((xs[ix],ys[iy],xs[ix+1],ys[iy+1])),(tx[ix+1]-tx[ix],ty[iy+1]-ty[iy])),(tx[ix],ty[iy]))
    return out
panel('SettingsPanel',654,562,122)
panel('RulesPanel',654,892,125)
save(nine(ui[1],722,319,35),DST/'CashCard.png')
card=nine(ui[1],722,319,35);hsv=np.array(card.convert('RGB').convert('HSV'));hsv[:,:,0]=85
green=Image.fromarray(hsv,'HSV').convert('RGBA');green.putalpha(card.getchannel('A'));save(green,DST/'CashCardSelected.png')
save(nine(ui[2],750,220,35),DST/'Header.png')
save(nine(ui[3],606,94,25),DST/'Notice.png')
save(nine(ui[10],588,62,25),DST/'ProgressTrack.png')
save(nine(ui[11],568,42,20),DST/'ProgressFill.png')
close=nine(ui[1],128,128,22);glyph=Image.open(PROJECT/'Assets/Resources/MenuArt/e5f13ccb28f96dadc8e58230d467bbf2.png').convert('RGBA');close.alpha_composite(resize(glyph,(80,80)),(24,24));save(close,DST/'RuleClose.png')

(OUT/'prepared_art_manifest.json').write_text(json.dumps(records,ensure_ascii=False,indent=2),'utf-8')
print('Prepared',len(records),'RGBA assets; gameplay configuration unchanged.')
