const sharp=require('C:/Users/001/.cache/codex-runtimes/codex-primary-runtime/dependencies/node/node_modules/sharp');
const fs=require('node:fs');const path=require('node:path');
const source='C:/Users/001/.codex/generated_images/01a09f59-e23d-72d2-b76d-56c17af35dab/exec-f25b10f8-0279-4e76-acd8-88e8abfcbc1a.png';
const out=path.resolve(__dirname,'../../Assets/Resources/BVisualFix');
(async()=>{fs.mkdirSync(out,{recursive:true}); const m=await sharp(source).metadata();
 for(let i=0;i<2;i++)await sharp(source).extract({left:i*887,top:0,width:887,height:887}).resize(384,384).png().toFile(path.join(out,i?'StarEmpty.png':'StarSelected.png'));
 const status='C:/Users/001/.codex/generated_images/01a09f59-e23d-72d2-b76d-56c17af35dab/exec-f4c17b12-e2c7-4f28-a0d3-9871d288d865.png';const sm=await sharp(status).metadata();
 for(let i=0;i<2;i++)await sharp(status).extract({left:i*887,top:0,width:887,height:887}).resize(384,384).png().toFile(path.join(out,i?'StatusWaiting.png':'StatusComplete.png'));
 console.log(JSON.stringify({source,width:m.width,height:m.height,alpha:m.hasAlpha,statusAlpha:sm.hasAlpha,exports:['StarSelected.png','StarEmpty.png','StatusComplete.png','StatusWaiting.png']}));})();
