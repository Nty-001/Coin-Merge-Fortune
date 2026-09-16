"""Extract approved English display lettering, preserving the native text fallback."""
from pathlib import Path
from collections import deque
import numpy as np
from PIL import Image,ImageFilter
R=Path(__file__).resolve().parents[3]
D=R/'paid_ui_work/reskin_20260916/UnityVisualRepair/Assets/Resources/VisualRepair'
src=Image.open(R/'paid_ui_work/reskin_20260916/RemainingUI_Review_R1/01_coin_withdraw.png').convert('RGB')
items=[('LabelWithdraw',(283,43,665,146),'blue'),('LabelMyCoins',(457,414,672,480),'blue'),('LabelSelect',(240,562,705,641),'blue'),('LabelConditions',(144,1200,800,1284),'blue'),('LabelWithdrawDisabled',(307,1515,642,1607),'blue'),('Amount500',(155,691,366,786),'green'),('Amount800',(590,688,784,787),'gold'),('Amount1000',(135,846,385,942),'gold'),('Amount2000',(561,846,820,942),'gold'),('Amount3000',(134,1000,394,1096),'red'),('Amount5000',(561,1000,817,1096),'red')]
def solid(mask):
 h,w=mask.shape;outside=np.zeros_like(mask);q=deque()
 for x in range(w):
  for y in (0,h-1):
   if not mask[y,x]:outside[y,x]=1;q.append((y,x))
 for y in range(h):
  for x in (0,w-1):
   if not mask[y,x] and not outside[y,x]:outside[y,x]=1;q.append((y,x))
 while q:
  y,x=q.popleft()
  for yy,xx in ((y-1,x),(y+1,x),(y,x-1),(y,x+1)):
   if 0<=yy<h and 0<=xx<w and not mask[yy,xx] and not outside[yy,xx]:outside[yy,xx]=1;q.append((yy,xx))
 return ~outside
for name,box,kind in items:
 im=src.crop(box);a=np.array(im).astype(float);r,g,b=a[:,:,0],a[:,:,1],a[:,:,2]
 if kind=='blue':seed=((b-r>35)&(g<110)&(b>80))|((r>210)&(g>225)&(b>230))
 elif kind=='green':seed=(g<125)&(r<100)&(b<100)
 elif kind=='gold':seed=(r-b>30)&(g-b>5)&(r>40)
 else:seed=(r-g>70)&(r-b>65)&(r>100)
 mask=Image.fromarray((seed*255).astype('uint8')).filter(ImageFilter.MaxFilter(3)).filter(ImageFilter.MinFilter(3))
 mask=Image.fromarray((solid(np.array(mask)>127)*255).astype('uint8'))
 # Keep a narrow, smooth perimeter; no rectangular source-background matte.
 mask=mask.filter(ImageFilter.MaxFilter(3)).filter(ImageFilter.GaussianBlur(.45))
 alpha=np.array(mask);alpha[alpha<4]=0
 out=im.convert('RGBA');out.putalpha(Image.fromarray(alpha));out.save(D/(name+'.png'))
print('Extracted',len(items),'approved lettering sprites')
atlas=Image.open(R/'Restoration/10_Reskin/ArtSources/approved_counter_digits.png').convert('RGB')
edges=[8,217,392,588,783,987,1181,1380,1578,1775,1983]
for n in range(10):
 im=atlas.crop((edges[n],247,edges[n+1],528));rgb=np.array(im).astype(float)
 colored=(rgb[:,:,2]-rgb[:,:,0]>.60*rgb[:,:,2])
 white=(rgb[:,:,0]>175)&(rgb[:,:,1]>205)&(rgb[:,:,2]>220)
 binary=colored|white;seen=np.zeros_like(binary);components=[];h,w=binary.shape
 for yy,xx in zip(*np.nonzero(binary)):
  if seen[yy,xx]:continue
  points=[];q=deque([(yy,xx)]);seen[yy,xx]=1
  while q:
   y,x=q.popleft();points.append((y,x))
   for y2,x2 in ((y-1,x),(y+1,x),(y,x-1),(y,x+1)):
    if 0<=y2<h and 0<=x2<w and binary[y2,x2] and not seen[y2,x2]:seen[y2,x2]=1;q.append((y2,x2))
  components.append(points)
 clean=np.zeros_like(binary);points=max(components,key=len)
 for y,x in points:clean[y,x]=1
 clean=solid(clean)
 # Fill the pale bevel inside the silhouette, but retain transparent counters.
 neutral=((rgb.max(2)-rgb.min(2)<30)&(rgb.max(2)<205))|((rgb[:,:,0]>40)&(rgb[:,:,1]>40)&(rgb[:,:,2]<225)&(rgb[:,:,2]<2.5*rgb[:,:,0]))
 interior=np.array(Image.fromarray((clean*255).astype('uint8')).filter(ImageFilter.MinFilter(15)))>127
 clean[neutral&interior]=False
 mask=Image.fromarray((clean*255).astype('uint8')).filter(ImageFilter.MinFilter(3));valid=np.array(mask)>127
 # Dilate edge RGB underneath alpha, preventing checker/background color bleed.
 for _ in range(5):
  count=np.zeros((h,w));sums=np.zeros_like(rgb);old=valid.copy()
  for dy,dx in ((1,0),(-1,0),(0,1),(0,-1)):
   neighbors=np.roll(old,(dy,dx),(0,1));sums+=np.roll(rgb,(dy,dx),(0,1))*neighbors[:,:,None];count+=neighbors
  extend=(~old)&(count>0);rgb[extend]=sums[extend]/count[extend,None];valid[extend]=1
 edge=np.array(mask.filter(ImageFilter.GaussianBlur(1.5))).astype(float)
 aa=np.clip((edge-50)*255/155,0,255).astype('uint8');aa[aa<4]=0
 im=Image.fromarray(np.dstack([rgb.clip(0,255).astype('uint8'),aa]));bounds=im.getchannel('A').point(lambda a:255 if a>16 else 0).getbbox()
 if bounds:im=im.crop(bounds)
 im.save(D/('Digit'+str(n)+'.png'))
