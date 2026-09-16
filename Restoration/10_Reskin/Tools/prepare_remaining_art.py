"""Cut approved art components with decontaminated alpha; no gameplay data edits."""
from pathlib import Path
import json,shutil
import numpy as np
from PIL import Image,ImageFilter
ROOT=Path(__file__).resolve().parents[3]
PROJECT=ROOT/'paid_ui_work/reskin_20260916/UnityRemainingWorkingCopy'
SRC=ROOT/'Restoration/10_Reskin/ArtSources'
BASE=PROJECT/'Assets/Resources/Reskin'
DST=PROJECT/'Assets/Resources/ReskinRemaining';DST.mkdir(parents=True,exist_ok=True)
def resize(im,wh):return im.convert('RGBa').resize(wh,Image.Resampling.LANCZOS).convert('RGBA')
def save(im,name):im.save(DST/(name+'.png'))
def dilate(rgb,valid):
 for _ in range(10):
  sums=np.zeros(rgb.shape,np.float32);count=np.zeros(valid.shape,np.float32)
  for dy,dx in [(1,0),(-1,0),(0,1),(0,-1)]:
   m=np.roll(valid,(dy,dx),(0,1));sums+=np.roll(rgb,(dy,dx),(0,1))*m[:,:,None];count+=m
  new=(~valid)&(count>0);rgb[new]=(sums[new]/count[new,None]).astype('uint8');valid=valid|new
 return rgb
sheet=Image.open(SRC/'remaining_parts.png')
names=['Disabled','Secondary','Input','People','StarGold','StarEmpty','StarCrest','Hand','Cash','CashStack','Arrow','Video']
for i,name in enumerate(names):
 x,y=i%4,i//4
 # Exclude the source grid's white dividers before chroma-keying.
 cell=sheet.crop((round(x*sheet.width/4)+7,round(y*sheet.height/3)+7,round((x+1)*sheet.width/4)-7,round((y+1)*sheet.height/3)-7))
 rgb=np.array(cell.convert('RGB'));key=np.minimum(rgb[:,:,0],rgb[:,:,2]).astype(int)-rgb[:,:,1].astype(int)
 m=Image.fromarray(((key<150)*255).astype('uint8')).filter(ImageFilter.MinFilter(3));core=np.array(m)>127
 a=np.array(m.filter(ImageFilter.GaussianBlur(.45)));a[a<5]=0
 rgb=dilate(rgb,core);im=Image.fromarray(np.dstack((rgb,a)));im=im.crop(im.getchannel('A').getbbox())
 save(im,name)
def nine(im,w,h,edge=36):
 sw,sh=im.size;c=min(45,sh//3);out=Image.new('RGBA',(w,h));xs=[0,c,sw-c,sw];ys=[0,c,sh-c,sh];tx=[0,edge,w-edge,w];ty=[0,edge,h-edge,h]
 for y in range(3):
  for x in range(3):out.paste(resize(im.crop((xs[x],ys[y],xs[x+1],ys[y+1])),(tx[x+1]-tx[x],ty[y+1]-ty[y])),(tx[x],ty[y]))
 return out
glass=Image.open(BASE/'UI1.png');head=Image.open(BASE/'UI2.png')
for name,w,h in [('AccountUS',654,848),('AccountBR',654,1239),('AccountID',654,1039),('Verify',654,1084),('NextCondition',654,745),('Limit',654,922),('Policy',650,1210),('Rating',654,655),('Fail',662,930),('WheelReward',694,879)]:
 im=nine(glass,w,h);im.alpha_composite(nine(head,w-14,110,30),(7,7));save(im,name)
for name,w,h in [('CoinHeader',722,194),('CoinChoices',722,790),('CoinCondition',722,245),('GuideWide',682,220),('GuideTall',682,270),('Inset',554,130)]:save(nine(glass,w,h,28),name)
# Reward panels keep their original sizes; the original animated heading occupies its own node.
for name,h in [('RewardLarge',755),('RewardSmall',535),('RewardGuide',710)]:save(nine(glass,662,h),name)
save(nine(head,719,212,35),'RewardTitle')
choice=resize(Image.open(BASE/'UI0.png'),(331,124));choice.alpha_composite(resize(Image.open(BASE/'UI13.png'),(45,45)),(281,74));save(choice,'ChoiceSelected')
# Compose only static decoration, never dynamic text.
cash=Image.open(DST/'CashStack.png');coin=Image.open(BASE/'Coin2000.png')
fan=Image.new('RGBA',(559,266));fan.alpha_composite(resize(cash,(250,190)),(0,70));fan.alpha_composite(resize(cash,(250,190)),(309,70));fan.alpha_composite(resize(coin,(260,260)),(150,0));save(fan,'AccountCrest')
shutil.copy2(SRC/'remaining_loading.png',DST/'Loading.png')
# Replace only the reward title atlas region, preserving UVs and the original animation.
atlaspath=PROJECT/'Assets/Resources/Skeletal/Atlases/DJB_TX.png'
atlas=Image.open(atlaspath).convert('RGBA')
regions=json.loads((PROJECT/'Assets/Art/HotUpdate/SkeletalReusable/DJB_TX/regions.json').read_text(encoding='utf-8-sig'))
r=next(r for r in regions if r['name']=='JB_00');w,h=map(int,r['size'].split(','));x,y=map(int,r['xy'].split(','))
part=nine(head,w,h,25)
if r['rotate']=='true':part=part.transpose(Image.Transpose.ROTATE_90)
atlas.paste(part,(x,y));atlas.save(atlaspath)
# Keep original currency asset GUIDs, canvases and localization paths.
for kind in [1,2,3]:
 target=PROJECT/f'Assets/Resources/Localization/Currency/{kind}/6.png'
 old=Image.open(target);part=Image.open(DST/('CashStack.png' if kind==3 else 'Cash.png'));part.thumbnail(old.size,Image.Resampling.LANCZOS)
 im=Image.new('RGBA',old.size);im.alpha_composite(part,((old.width-part.width)//2,(old.height-part.height)//2));im.save(target)
# A representative dark-background cutout QA sheet, preserving each sprite's aspect.
qa=Image.new('RGB',(1200,900),'#162335')
for i,name in enumerate(names):
 im=Image.open(DST/(name+'.png'));im.thumbnail((280,275));qa.paste(im,(i%4*300+(300-im.width)//2,i//4*300+(300-im.height)//2),im)
qa.save(ROOT/'paid_ui_work/reskin_20260916/remaining_alpha_qa.png')
print('Prepared',len(list(DST.glob('*.png'))),'remaining-UI assets.')
