"""Static serialized-state previews only; does not run gameplay, widgets, Spine or animation."""
import json,math,re,collections
from pathlib import Path
from PIL import Image,ImageFont,ImageDraw,ImageChops
R=Path(__file__).resolve().parents[2];O=R/'Restoration'
def read(p):return json.loads(p.read_text(encoding='utf-8'))
fontpath=next((O/'04_Assets/HotUpdate/Fonts').rglob('*.ttf'))
def mul(A,B):
 a,b,c,d,e,f=A;g,h,i,j,k,l=B
 return (a*g+c*h,b*g+d*h,a*i+c*j,b*i+d*j,a*k+c*l+e,b*k+d*l+f)
def inverse(A):
 a,b,c,d,e,f=A;det=a*d-b*c
 if abs(det)<1e-10:return None
 return (d/det,-b/det,-c/det,a/det,(c*f-d*e)/det,(b*e-a*f)/det)
for variant in ['Packaged','HotUpdate']:
 assets={s['uuid']:s for s in read(O/'04_Assets'/variant/'sprites.json')}
 for p in (O/'06_UnityFramework/Assets/Resources/Recovered').glob(variant+'_*.json'):
  m=read(p);nodes={n['id']:n for n in m['nodes']};canvas=Image.new('RGBA',(750,1624),(25,33,58,255));count=0
  def drawnode(i,parent=(1,0,0,1,0,0),active=True,alpha=1):
   global count
   n=nodes[i];active=active and n['active']
   if not active:return
   x,y,z=n['position'];sx,sy,sz=n['scale'];qx,qy,qz,qw=n['rotation'];angle=2*math.atan2(qz,qw);c,s=math.cos(angle),math.sin(angle)
   M=mul(parent,(c*sx,s*sx,-s*sy,c*sy,x,y));alpha*=n['color'][3]
   w,h=n['size'];px,py=n['pivot']
   for component in n['components']:
    if not component['enabled']:continue
    im=None
    if component['type']=='cc.Sprite' and component['sprite'] in assets:
     im=Image.open(O/'04_Assets'/variant/assets[component['sprite']]['file']).convert('RGBA')
    elif component['type'] in ['cc.Label','cc.RichText'] and w>0 and h>0:
     im=Image.new('RGBA',(max(1,round(w)),max(1,round(h))));draw=ImageDraw.Draw(im);size=max(1,round(component['fontSize']));font=ImageFont.truetype(str(fontpath),size)
     text=re.sub('<[^>]+>','',component['text']);lines=[]
     for para in text.split('\n'):
      line=''
      for word in para.split(' '):
       trial=(line+' '+word).strip()
       if draw.textlength(trial,font=font)>w and line:lines.append(line);line=word
       else:line=trial
      lines.append(line)
     lh=component['lineHeight'];yy=0 if component['verticalAlign']==0 else (h-lh*len(lines))/2 if component['verticalAlign']==1 else h-lh*len(lines)
     for line in lines:
      length=draw.textlength(line,font=font);xx=0 if component['horizontalAlign']==0 else (w-length)/2 if component['horizontalAlign']==1 else w-length
      draw.text((xx,yy),line,font=font,fill='white');yy+=lh
    if im is None or w<=0 or h<=0:continue
    color=n['color'];tint=Image.new('RGBA',im.size,tuple(max(0,min(255,round(v*255))) for v in color[:3]+[alpha]));im=ImageChops.multiply(im,tint)
    # Source pixels -> node anchor coordinates -> world -> y-down preview pixels.
    P=mul((1,0,0,-1,0,1624),mul(M,(w/im.width,0,0,-h/im.height,-px*w,(1-py)*h)))
    inv=inverse(P)
    if inv:
     a,b,c,d,e,f=inv;layer=im.transform(canvas.size,Image.Transform.AFFINE,(a,c,e,b,d,f),resample=Image.Resampling.BICUBIC);canvas.alpha_composite(layer)
   for child in n['children']:
    if child in nodes:drawnode(child,M,active,alpha)
  for n in nodes.values():
   if n['parent'] not in nodes:drawnode(n['id'])
  dest=O/'05_PrefabModel'/variant/'StaticPreviews'/(m['uuid']+'.png');dest.parent.mkdir(parents=True,exist_ok=True);canvas.convert('RGB').save(dest)
print('Saved 63 static source-state previews; gameplay and animations are not executed.')
