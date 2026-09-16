// Deterministic technical cutouts of the supplied art; no painted/vector substitutes.
const fs=require('fs'),path=require('path'),crypto=require('crypto');
const sharp=require('C:/Users/001/.cache/codex-runtimes/codex-primary-runtime/dependencies/node/node_modules/sharp');
const project=path.resolve(__dirname,'../..'),out=path.join(project,'Assets/Resources/CashReskin');
fs.mkdirSync(out,{recursive:true});
async function cut(src,box,name,radius,overlay){
 let input=sharp(src);if(overlay)input=input.composite(overlay);
 let buffer=await input.png().toBuffer();
 const {data,info}=await sharp(buffer).extract({left:box[0],top:box[1],width:box[2],height:box[3]}).ensureAlpha().raw().toBuffer({resolveWithObject:true});
 const w=info.width,h=info.height;
 for(let y=0;y<h;y++)for(let x=0;x<w;x++){
  const dx=Math.max(radius-x-.5,0,x+.5-(w-radius)),dy=Math.max(radius-y-.5,0,y+.5-(h-radius));
  const a=radius?Math.max(0,Math.min(1,radius-Math.hypot(dx,dy)+.5)):1,i=(y*w+x)*4;
  data[i+3]=Math.round(a*255);if(!data[i+3])data[i]=data[i+1]=data[i+2]=0;
 }
 await sharp(data,{raw:{width:w,height:h,channels:4}}).png().toFile(path.join(out,name+'.png'));
}
(async()=>{
 const blank=path.join(__dirname,'BlankReference.png'),target=path.join(__dirname,'Target.jpg');
 await cut(blank,[14,22,915,150],'Header',60);
 await cut(blank,[0,173,941,350],'HeroSky',0);
 await cut(target,[33,47,98,98],'Back',49);
 const check=await sharp(target).extract({left:818,top:542,width:88,height:85}).png().toBuffer();
 await cut(blank,[18,523,905,420],'CardSelected',62,[{input:check,left:818,top:542}]);
 await cut(blank,[18,959,905,409],'Card',64);
 // Extend only the empty card interior by 11px. Borders/corners retain their pixels,
 // and selected/normal tracks now align with the same native progress Image.
 const normal=fs.readFileSync(path.join(out,'Card.png'));
 const upper=await sharp(normal).extract({left:0,top:0,width:905,height:290}).png().toBuffer();
 const strip=await sharp(normal).extract({left:0,top:289,width:905,height:1}).resize(905,11,{fit:'fill'}).png().toBuffer();
 const lower=await sharp(normal).extract({left:0,top:290,width:905,height:119}).png().toBuffer();
 await sharp({create:{width:905,height:420,channels:4,background:'#00000000'}}).composite([{input:upper,top:0,left:0},{input:strip,top:290,left:0},{input:lower,top:301,left:0}]).png().toFile(path.join(out,'Card.png'));
 await cut(blank,[207,1454,525,169],'Withdraw',84);
 // The footer is a separate opaque glass surface; its only transparent pixels are its rounded top corners.
 await cut(path.join(__dirname,'FooterSource.png'),[0,1426,941,246],'Footer',18);
 // The AI-inpainted footer contains no button; the native control is a separate RGBA asset.
 await cut(target,[77,848,768,42],'ProgressFill',21);
 const manifest={source:'Exact user target and matching blank art, RGBA edge extraction; accepted chip sprites reused',files:{}};
 for(const name of fs.readdirSync(out).filter(n=>n.endsWith('.png'))){const f=path.join(out,name),{data,info}=await sharp(f).raw().toBuffer({resolveWithObject:true});let clear=0,partial=0;for(let i=3;i<data.length;i+=4){if(data[i]===0)clear++;else if(data[i]<255)partial++;}manifest.files[name]={width:info.width,height:info.height,transparentPixels:clear,edgePixels:partial,sha256:crypto.createHash('sha256').update(fs.readFileSync(f)).digest('hex')};}
 fs.writeFileSync(path.join(__dirname,'materials.json'),JSON.stringify(manifest,null,2)+'\n');console.log(manifest);
})();
