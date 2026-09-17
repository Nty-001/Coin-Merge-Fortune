// Technical transparent sprite extraction only. Painting: built-in image_gen.
const fs=require('fs'),path=require('path'),crypto=require('crypto');
const sharp=require('C:/Users/001/.cache/codex-runtimes/codex-primary-runtime/dependencies/node/node_modules/sharp');
const out=path.resolve(__dirname,'../../Assets/Resources/LoadingReskin');
(async()=>{
 const src=await sharp(path.join(__dirname,'GeneratedBars.png')).ensureAlpha().raw().toBuffer({resolveWithObject:true});
 const w=src.info.width,h=src.info.height,d=src.data,seen=new Uint8Array(w*h),components=[];
 for(let p=0;p<w*h;p++)if(!seen[p]&&d[p*4+3]>220){let q=[p];seen[p]=1;for(let j=0;j<q.length;j++){let k=q[j];for(let n of [k-1,k+1,k-w,k+w])if(n>=0&&n<w*h&&!seen[n]&&Math.abs(n%w-k%w)<=1&&d[n*4+3]>220){seen[n]=1;q.push(n);}}if(q.length>10000)components.push(q);}
 components.sort((a,b)=>a[0]-b[0]);if(components.length!==2)throw new Error('Expected two opaque bar components, got '+components.length);
 const manifest={method:'Original illustration copied without repainting. Two image_gen bar sprites extracted without aspect changes; detached alpha fringe removed.',files:{}};
 for(let c=0;c<2;c++){
  const mask=new Uint8Array(w*h);let l=w,t=h,r=0,b=0;for(const p of components[c]){mask[p]=255;l=Math.min(l,p%w);r=Math.max(r,p%w);t=Math.min(t,Math.floor(p/w));b=Math.max(b,Math.floor(p/w));}
  const eroded=new Uint8Array(w*h);for(let y=t;y<=b;y++)for(let x=l;x<=r;x++){let a=255;for(let dy=-1;dy<=1;dy++)for(let dx=-1;dx<=1;dx++)a=Math.min(a,mask[(y+dy)*w+x+dx]||0);eroded[y*w+x]=a;}
  const alpha=await sharp(eroded,{raw:{width:w,height:h,channels:1}}).blur(.5).toColourspace('b-w').raw().toBuffer();
  const clean=Buffer.from(d);for(let p=0;p<w*h;p++){clean[p*4+3]=Math.min(d[p*4+3],alpha[p]);if(!clean[p*4+3])clean[p*4]=clean[p*4+1]=clean[p*4+2]=0;}
  const crop={left:Math.max(0,l-3),top:Math.max(0,t-3),width:r-l+7,height:b-t+7};
  const name=c===0?'Track.png':'Fill.png';await sharp(clean,{raw:{width:w,height:h,channels:4}}).extract(crop).png().toFile(path.join(out,name));manifest.files[name]={...crop};
 }
 // Only decorative bottom clouds extend below the unchanged reference on tall phones.
 await sharp(path.join(out,'Illustration.png')).extract({left:0,top:1372,width:941,height:300}).png().toFile(path.join(out,'Clouds.png'));
 for(const name of fs.readdirSync(out).filter(n=>n.endsWith('.png'))){const file=path.join(out,name),v=await sharp(file).ensureAlpha().raw().toBuffer({resolveWithObject:true});let clear=0,edge=0;for(let i=3;i<v.data.length;i+=4){if(v.data[i]===0)clear++;else if(v.data[i]<255)edge++;}manifest.files[name]={...manifest.files[name],width:v.info.width,height:v.info.height,transparentPixels:clear,antialiasedPixels:edge,sha256:crypto.createHash('sha256').update(fs.readFileSync(file)).digest('hex')};}
 fs.writeFileSync(path.join(__dirname,'materials.json'),JSON.stringify(manifest,null,2));console.log(manifest);
})();
