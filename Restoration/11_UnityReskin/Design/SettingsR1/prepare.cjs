// Technical extraction only: preserve the supplied / generated painted pixels.
const fs=require('fs'),path=require('path'),crypto=require('crypto');
const sharp=require('C:/Users/001/.cache/codex-runtimes/codex-primary-runtime/dependencies/node/node_modules/sharp');
const project=path.resolve(__dirname,'../..'),out=path.join(project,'Assets/Resources/SettingsReskin');
fs.mkdirSync(out,{recursive:true});
async function cut(src,rect,name,radius,panel=false){
 const {data,info}=await sharp(src).extract(rect).ensureAlpha().raw().toBuffer({resolveWithObject:true});
 const w=info.width,h=info.height;
 for(let y=0;y<h;y++){
  let l=0,r=w-1;
  if(panel){l=w;r=-1;for(let x=0;x<w;x++){let i=(y*w+x)*4;if(data[i+2]-data[i]>28&&data[i+1]-data[i]>8&&data[i+2]>140){l=Math.min(l,x);r=x;}}}
  for(let x=0;x<w;x++){
   const i=(y*w+x)*4,dx=Math.max(radius-x-.5,0,x+.5-(w-radius)),dy=Math.max(radius-y-.5,0,y+.5-(h-radius));
   let a=radius===0?1:Math.max(0,Math.min(1,radius-Math.sqrt(dx*dx+dy*dy)+.5));
   if(panel)a*=Math.max(0,Math.min(1,x-l-1.25,r-x-1.25,y-1.25,h-y-2.25));
   data[i+3]=Math.round(data[i+3]*a);if(!data[i+3])data[i]=data[i+1]=data[i+2]=0;
  }
 }
 await sharp(data,{raw:{width:w,height:h,channels:4}}).png().toFile(path.join(out,name+'.png'));
}
(async()=>{
 const panel=path.join(__dirname,'GeneratedPanel.png');
 await cut(panel,{left:40,top:45,width:1287,height:1068},'Panel',0,true);
 const source=path.join(__dirname,'Target.jpg');
 await cut(source,{left:1033,top:157,width:153,height:153},'Close',76.5);
 await cut(source,{left:846,top:714,width:247,height:136},'SwitchOn',68);
 await cut(source,{left:856,top:483,width:242,height:129},'SwitchOff',64.5);
 const manifest={imageTool:'built-in image_gen, remove separate close button from blank panel; technical RGBA cutouts from exact target',files:{}};
 for(const name of fs.readdirSync(out).filter(n=>n.endsWith('.png'))){const f=path.join(out,name),{data,info}=await sharp(f).ensureAlpha().raw().toBuffer({resolveWithObject:true});let clear=0,partial=0;for(let i=3;i<data.length;i+=4){if(!data[i])clear++;else if(data[i]<255)partial++;}manifest.files[name]={width:info.width,height:info.height,transparentPixels:clear,edgePixels:partial,sha256:crypto.createHash('sha256').update(fs.readFileSync(f)).digest('hex')};}
 fs.writeFileSync(path.join(__dirname,'materials.json'),JSON.stringify(manifest,null,2));console.log(manifest);
})();
