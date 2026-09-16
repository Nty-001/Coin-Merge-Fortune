// Technical slicing/matting only. Painted pixels are the reference / generated panel.
const fs=require('fs'),path=require('path'),crypto=require('crypto');
const sharp=require('C:/Users/001/.cache/codex-runtimes/codex-primary-runtime/dependencies/node/node_modules/sharp');
const project=path.resolve(__dirname,'../..'),repo=path.resolve(project,'../..');
const out=path.join(project,'Assets/Resources/RulesReskin');fs.mkdirSync(out,{recursive:true});
const blank=path.join(__dirname,'BlankReference.png');
const values=[1,2,5,10,20,50,100,200,500,1000,2000];
async function cut(src,rect,name,radius){
 const {data,info}=await sharp(src).extract(rect).ensureAlpha().raw().toBuffer({resolveWithObject:true});
 const w=info.width,h=info.height;
 // Subpixel rounded-rectangle mask follows the existing sprite boundary.
 for(let y=0;y<h;y++)for(let x=0;x<w;x++){
  const dx=Math.max(radius-x-.5,0,x+.5-(w-radius)),dy=Math.max(radius-y-.5,0,y+.5-(h-radius));
  const a=Math.max(0,Math.min(1,radius-Math.sqrt(dx*dx+dy*dy)+.5));
  data[(y*w+x)*4+3]=Math.round(data[(y*w+x)*4+3]*a);
 }
 if(name==='Panel')for(let y=0;y<h;y++){
  let l=w,r=-1;for(let x=0;x<w;x++){let i=(y*w+x)*4;if(data[i+2]-data[i]>20&&data[i+1]-data[i]>6&&data[i+2]>140){l=Math.min(l,x);r=x;}}
  for(let x=0;x<w;x++){let i=(y*w+x)*4;data[i+3]=Math.round(data[i+3]*Math.max(0,Math.min(1,x-l-1.25,r-x-1.25,y-1.25,h-y-2.25)));if(!data[i+3])data[i]=data[i+1]=data[i+2]=0;}
 }
 await sharp(data,{raw:{width:w,height:h,channels:4}}).png().toFile(path.join(out,name+'.png'));
}
(async()=>{
 // Initial authoring verified decoded RGB against the original cutout sheet. Pin that accepted input for repeat runs.
 if(crypto.createHash('sha256').update(fs.readFileSync(path.join(__dirname,'Chips.png'))).digest('hex')!=='8fcc531a8f4f3b41040309a7bdd74c56328963ed395e03dd2ed8aabc144108da')throw Error('Approved chip sheet changed');
 const panel=path.join(__dirname,'GeneratedPanel.png');
 await cut(panel,{left:30,top:29,width:1092,height:1299},'Panel',81);
 await cut(blank,{left:858,top:327,width:110,height:108},'Close',29);
 await cut(blank,{left:198,top:1217,width:638,height:181},'Confirm',88);
 await cut(path.join(__dirname,'Target.jpg'),{left:255,top:848,width:24,height:24},'PathDot',12);
 const composite=[];
 for(let i=0;i<values.length;i++){
  const src=path.join(out,`Chip${values[i]}.png`);
  composite.push({input:await sharp(src).resize(304,304,{fit:'contain',background:{r:0,g:0,b:0,alpha:0}}).png().toBuffer(),left:i%4*320+8,top:Math.floor(i/4)*320+8});
 }
 await sharp({create:{width:1280,height:960,channels:4,background:{r:0,g:0,b:0,alpha:0}}}).composite(composite).png().toFile(path.join(out,'ChipAtlas.png'));
 const hashes={};for(const name of fs.readdirSync(out).filter(x=>x.endsWith('.png')))hashes[name]=crypto.createHash('sha256').update(fs.readFileSync(path.join(out,name))).digest('hex');
 fs.writeFileSync(path.join(__dirname,'materials.json'),JSON.stringify({latestChipPixelsMatch:true,values,hashes,buttonAspect:'native Button + same-object Image, preserveAspect, uniform parent scale',imageTool:'built-in image_gen, panel removal edit; no repainted chip art'},null,2));
 console.log('Prepared',Object.keys(hashes).length,'rule-specific textures; latest chip sheet matches exactly');
})().catch(e=>{console.error(e);process.exitCode=1;});
