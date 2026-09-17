const fs=require('fs'),path=require('path'),crypto=require('crypto');
const sharp=require('C:/Users/001/.cache/codex-runtimes/codex-primary-runtime/dependencies/node/node_modules/sharp');
const project=path.resolve(__dirname,'../..');
(async()=>{
 // Pack the imagegen-cleaned transparent cutout into the existing sprite canvas; retain its Unity GUID.
 const target=path.join(__dirname,'ApprovedKnob.png');
 await sharp(path.join(__dirname,'GeneratedKnob.png')).resize(144,145,{fit:'fill',kernel:'lanczos3'}).png().toFile(target);
 fs.copyFileSync(target,path.join(project,'Assets/Resources/WheelReskin/Knob.png'));
 const data=fs.readFileSync(target),manifestPath=path.join(__dirname,'../WheelLeverR1/assets.json'),manifest=JSON.parse(fs.readFileSync(manifestPath,'utf8'));
 manifest['Knob.png']={width:144,height:145,alpha:true,sha256:crypto.createHash('sha256').update(data).digest('hex')};
 fs.writeFileSync(manifestPath,JSON.stringify(manifest,null,2));console.log(manifest['Knob.png']);
})();
