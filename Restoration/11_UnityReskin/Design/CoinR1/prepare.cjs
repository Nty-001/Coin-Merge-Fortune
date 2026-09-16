// Only technical cropping and transparent masks. Source paintings are generated with image_gen.
const fs=require('fs'),path=require('path'),crypto=require('crypto');
const sharp=require('C:/Users/001/.cache/codex-runtimes/codex-primary-runtime/dependencies/node/node_modules/sharp');
const root=path.resolve(__dirname,'../..'),out=path.join(root,'Assets/Resources/CoinReskin');
const generated='C:/Users/001/.codex/generated_images/01a09f59-e23d-72d2-b76d-56c17af35dab/';
fs.mkdirSync(out,{recursive:true});
for(const [name,file] of Object.entries({Blank:'exec-ea7e11e9-4e39-4ad1-bf38-52f1f35229e4.png',Panels:'exec-37188933-dc7e-4783-ab60-3939464010ab.png',Background:'exec-79e415fb-8b96-4223-8b5c-331c92c192ca.png'}))if(!fs.existsSync(path.join(__dirname,name+'.png')))fs.copyFileSync(generated+file,path.join(__dirname,name+'.png'));
async function crop(name,source,x,y,w,h,r,extra=''){
 let shape=extra||`<rect x="1" y="1" width="${w-2}" height="${h-2}" rx="${r}" fill="white"/>`;
 const mask=Buffer.from(`<svg width="${w}" height="${h}">${shape}</svg>`);
 await sharp(path.join(__dirname,source+'.png')).extract({left:x,top:y,width:w,height:h}).ensureAlpha().composite([{input:mask,blend:'dest-in'}]).png().toFile(path.join(out,name+'.png'));
}
(async()=>{
 await crop('Balance','Panels',17,212,909,252,64);
 await crop('Amounts','Panels',17,482,909,599,64);
 await crop('Conditions','Panels',17,1101,909,299,66);
 await crop('Card','Blank',484,639,412,155,36);
 await crop('CardSelected','Blank',46,638,421,166,0,'<rect x="1" y="1" width="409" height="151" rx="34" fill="white"/><circle cx="384" cy="129" r="34" fill="white"/>');
 await crop('WithdrawDisabled','Blank',210,1433,525,169,75);
 await crop('Back','Blank',15,68,94,94,46);
 fs.copyFileSync(path.join(__dirname,'Background.png'),path.join(out,'Background.png'));
 const manifest={method:'Three image_gen edits remove lettering/UI without repainting retained UI. Fixed crops have antialiased alpha masks. Numbers, titles, selection, and progress are native live UI. Balance chip uses existing RulesReskin/Chip2000.',files:{}};
 for(const name of fs.readdirSync(out).filter(n=>n.endsWith('.png'))){const f=path.join(out,name),r=await sharp(f).ensureAlpha().raw().toBuffer({resolveWithObject:true});let clear=0,edge=0;for(let i=3;i<r.data.length;i+=4){if(!r.data[i])clear++;else if(r.data[i]<255)edge++;}manifest.files[name]={width:r.info.width,height:r.info.height,transparentPixels:clear,antialiasedPixels:edge,sha256:crypto.createHash('sha256').update(fs.readFileSync(f)).digest('hex')};}
 fs.writeFileSync(path.join(__dirname,'materials.json'),JSON.stringify(manifest,null,2));console.log(manifest);
})();
