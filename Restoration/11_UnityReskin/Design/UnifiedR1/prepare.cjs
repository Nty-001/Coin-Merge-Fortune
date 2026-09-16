// Technical alpha cleanup / slicing only; artwork is authored with image_gen.
const fs=require('fs'),path=require('path'),crypto=require('crypto');
const sharp=require('C:/Users/001/.cache/codex-runtimes/codex-primary-runtime/dependencies/node/node_modules/sharp');
const root=path.resolve(__dirname,'../..'),out=path.join(root,'Assets/Resources/UnifiedReskin');
fs.mkdirSync(out,{recursive:true});
(async()=>{
 const {data,info}=await sharp(path.join(__dirname,'GeneratedPanel.png')).extract({left:40,top:40,width:1076,height:1280}).ensureAlpha().raw().toBuffer({resolveWithObject:true});
 for(let y=0;y<info.height;y++){
  let l=info.width,r=-1;for(let x=0;x<info.width;x++){const i=(y*info.width+x)*4;if(data[i+2]-data[i]>35&&data[i+1]-data[i]>10&&data[i+2]>140){l=Math.min(l,x);r=x;}}
  for(let x=0;x<info.width;x++){const i=(y*info.width+x)*4;data[i+3]=Math.round(255*Math.max(0,Math.min(1,x-l-1.1,r-x-1.1,y-1.1,info.height-y-2.1)));if(!data[i+3])data[i]=data[i+1]=data[i+2]=0;}
 }
 await sharp(data,{raw:{width:info.width,height:info.height,channels:4}}).png().toFile(path.join(out,'Panel.png'));
 const body=await sharp(path.join(out,'Panel.png')).extract({left:20,top:266,width:1036,height:990}).raw().toBuffer();
 for(let y=0;y<990;y++)for(let x=0;x<1036;x++){const i=(y*1036+x)*4,dx=Math.max(96-x-.5,0,x+.5-940),dy=Math.max(96-y-.5,0,y+.5-894);const a=Math.max(0,Math.min(1,96-Math.hypot(dx,dy)));body[i+3]=Math.round(a*255);if(!body[i+3])body[i]=body[i+1]=body[i+2]=0;}
 await sharp(body,{raw:{width:1036,height:990,channels:4}}).png().toFile(path.join(out,'Body.png'));
 const machine=await sharp(path.join(__dirname,'GeneratedMachine.png')).ensureAlpha().raw().toBuffer({resolveWithObject:true});
 const w=machine.info.width,h=machine.info.height,src=machine.data,seen=new Uint8Array(w*h);let largest=[];
 for(let p=0;p<w*h;p++)if(!seen[p]&&src[p*4+3]>200){let q=[p];seen[p]=1;for(let j=0;j<q.length;j++){let k=q[j];for(let n of [k-1,k+1,k-w,k+w])if(n>=0&&n<w*h&&!seen[n]&&Math.abs(n%w-k%w)<=1&&src[n*4+3]>200){seen[n]=1;q.push(n);}}if(q.length>largest.length)largest=q;}
 const mask=new Uint8Array(w*h);for(let p of largest)mask[p]=255;
 // One-pixel erosion followed by subpixel antialias removes detached flecks and contaminated boundary pixels.
 const eroded=new Uint8Array(w*h);for(let y=1;y<h-1;y++)for(let x=1;x<w-1;x++){let a=255;for(let dy=-1;dy<=1;dy++)for(let dx=-1;dx<=1;dx++)a=Math.min(a,mask[(y+dy)*w+x+dx]);eroded[y*w+x]=a;}
 const soft=await sharp(eroded,{raw:{width:w,height:h,channels:1}}).blur(.6).toColourspace('b-w').raw().toBuffer();
 for(let p=0;p<w*h;p++){src[p*4+3]=Math.min(src[p*4+3],soft[p]);if(!src[p*4+3])src[p*4]=src[p*4+1]=src[p*4+2]=0;}
 await sharp(src,{raw:{width:w,height:h,channels:4}}).png().toFile(path.join(out,'Machine.png'));
 fs.copyFileSync(path.join(root,'Assets/Resources/CashReskin/Withdraw.png'),path.join(out,'Button.png'));
 const manifest={method:'AI-authored blank sky glass panel and machine. Original pixels retained; external checkerboard removed and machine alpha cleaned. Blank button reuses accepted cash artwork.',files:{}};
 for(const name of fs.readdirSync(out).filter(n=>n.endsWith('.png'))){const f=path.join(out,name),v=await sharp(f).raw().toBuffer({resolveWithObject:true});let clear=0,edge=0;for(let i=3;i<v.data.length;i+=4){if(v.data[i]===0)clear++;else if(v.data[i]<255)edge++;}manifest.files[name]={width:v.info.width,height:v.info.height,transparentPixels:clear,antialiasedPixels:edge,sha256:crypto.createHash('sha256').update(fs.readFileSync(f)).digest('hex')};}
 fs.writeFileSync(path.join(__dirname,'materials.json'),JSON.stringify(manifest,null,2));console.log(manifest);
})();
