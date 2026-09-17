// Packaging and alpha cleanup of imagegen output, matching the established reskin asset pipeline.
const fs=require('fs'),path=require('path'),crypto=require('crypto');
const sharp=require('C:/Users/001/.cache/codex-runtimes/codex-primary-runtime/dependencies/node/node_modules/sharp');
const project=path.resolve(__dirname,'../..'),out=path.join(project,'Assets/Resources/WheelReskin');
fs.mkdirSync(out,{recursive:true});
async function clean(file,dest,rect){
 let input=sharp(file);if(rect)input=input.extract(rect);
 const {data,info}=await input.ensureAlpha().raw().toBuffer({resolveWithObject:true}),w=info.width,h=info.height;
 const seen=new Uint8Array(w*h);let largest=[];
 for(let p=0;p<w*h;p++)if(!seen[p]&&data[p*4+3]>200){const q=[p];seen[p]=1;for(let j=0;j<q.length;j++)for(const n of [q[j]-1,q[j]+1,q[j]-w,q[j]+w])if(n>=0&&n<w*h&&!seen[n]&&Math.abs(n%w-q[j]%w)<=1&&data[n*4+3]>200){seen[n]=1;q.push(n);}if(q.length>largest.length)largest=q;}
 const mask=new Uint8Array(w*h);for(const p of largest)mask[p]=255;
 const eroded=new Uint8Array(w*h);for(let y=1;y<h-1;y++)for(let x=1;x<w-1;x++){let a=255;for(let dy=-1;dy<=1;dy++)for(let dx=-1;dx<=1;dx++)a=Math.min(a,mask[(y+dy)*w+x+dx]);eroded[y*w+x]=a;}
 const soft=await sharp(eroded,{raw:{width:w,height:h,channels:1}}).blur(.6).toColourspace('b-w').raw().toBuffer();
 for(let p=0;p<w*h;p++){data[p*4+3]=Math.min(data[p*4+3],soft[p]);if(!data[p*4+3])data[p*4]=data[p*4+1]=data[p*4+2]=0;}
 let image=sharp(data,{raw:{width:w,height:h,channels:4}});
 if(rect){let l=w,t=h,r=0,b=0;for(let y=0;y<h;y++)for(let x=0;x<w;x++)if(data[(y*w+x)*4+3]>0){l=Math.min(l,x);t=Math.min(t,y);r=Math.max(r,x);b=Math.max(b,y);}image=image.extract({left:l-2,top:t-2,width:r-l+5,height:b-t+5});}
 await image.png().toFile(path.join(out,dest));
}
(async()=>{
 await clean(path.join(__dirname,'GeneratedBody.png'),'Body.png');
 const parts=path.join(__dirname,'GeneratedParts.png'),m=await sharp(parts).metadata();
 await clean(parts,'Shaft.png',{left:Math.floor(m.width/2),top:0,width:m.width-Math.floor(m.width/2),height:m.height});
 // R2 removes the silver stump carried by the original crop. Do not reintroduce that overlapping shaft.
 fs.copyFileSync(path.join(__dirname,'../WheelJointR2/ApprovedKnob.png'),path.join(out,'Knob.png'));
 const manifest={};for(const file of fs.readdirSync(out).filter(f=>f.endsWith('.png'))){const b=fs.readFileSync(path.join(out,file)),m=await sharp(b).metadata();manifest[file]={width:m.width,height:m.height,alpha:m.hasAlpha,sha256:crypto.createHash('sha256').update(b).digest('hex')};}
 fs.writeFileSync(path.join(__dirname,'assets.json'),JSON.stringify(manifest,null,2));console.log(manifest);
})();
