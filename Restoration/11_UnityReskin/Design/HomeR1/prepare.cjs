// Technical extraction/import preparation. All painted pixels come from the
// user's exact chip sheet or reviewed/generated reference artwork.
const fs=require('fs'), path=require('path'), crypto=require('crypto');
const sharp=require('C:/Users/001/.cache/codex-runtimes/codex-primary-runtime/dependencies/node/node_modules/sharp');
const root=path.resolve(__dirname,'../../../..');
// Staging is deliberately outside Unity's Assets folder until scope is confirmed.
const project=path.join(__dirname,'Staging'), out=path.join(project,'Assets/Resources/HomeReskin');
fs.mkdirSync(out,{recursive:true});fs.mkdirSync(path.join(__dirname,'Sources'),{recursive:true});
const old=path.join(root,'Restoration/10_Reskin');
const review=path.join(root,'paid_ui_work/reskin_20260916/UnityVisualRepair/Assets/Resources/VisualRepair');
const records=[];
async function audit(file,source){const {data,info}=await sharp(file).ensureAlpha().raw().toBuffer({resolveWithObject:true});const bounds={};for(const t of [1,4,8,16]){let l=info.width,h=info.height,r=-1,b=-1;for(let y=0;y<info.height;y++)for(let x=0;x<info.width;x++)if(data[(y*info.width+x)*4+3]>=t){l=Math.min(l,x);h=Math.min(h,y);r=Math.max(r,x);b=Math.max(b,y);}bounds[t]=[l,h,r+1,b+1];}records.push({file:path.relative(project,file),source,width:info.width,height:info.height,alphaBounds:bounds,sha256:crypto.createHash('sha256').update(fs.readFileSync(file)).digest('hex')});}
async function copy(src,dest){fs.mkdirSync(path.dirname(dest),{recursive:true});fs.copyFileSync(src,dest);await audit(dest,path.relative(root,src));}
async function main(){
 const chip='C:/Users/001/AppData/Local/Temp/codex-clipboard-cc588d62-fed3-4ec3-b6ce-bd1f45f71f4f.png';
 const ref='C:/Users/001/AppData/Local/Temp/codex-clipboard-ba7beba0-dcbb-451d-aea0-92ee5dcdcc50.png';
 const a=await sharp(chip).raw().toBuffer(), b=await sharp(path.join(old,'ArtSources/coins.png')).raw().toBuffer();
 if(!a.equals(b))throw Error('Existing technical chip cutouts do not match latest user source');
 fs.copyFileSync(chip,path.join(__dirname,'Sources/Chips.png'));fs.copyFileSync(ref,path.join(__dirname,'Sources/TargetHome.png'));
 for(const n of [1,2,5,10,20,50,100,200,500,1000,2000]){
  await copy(path.join(old,`Patch/Assets/Resources/Reskin/Coin${n}.png`),path.join(out,`Coin${n}.png`));
  const target=path.join(project,`Assets/Resources/Gameplay/Coins/${n}.png`), m=await sharp(path.join(root,`Restoration/06_UnityFramework/Assets/Resources/Gameplay/Coins/${n}.png`)).metadata();fs.mkdirSync(path.dirname(target),{recursive:true});
  await sharp(path.join(out,`Coin${n}.png`)).resize(m.width,m.height).toFile(target+'.new.png');fs.renameSync(target+'.new.png',target);await audit(target,'Exact user chip sheet, same dimensions/GUID/import settings as baseline');
 }
 for(const name of ['AnNiu_TX','HeChengSM_TX','DJB_TX'])await copy(path.join(old,`Patch/Assets/Resources/Skeletal/Atlases/${name}.png`),path.join(project,`Assets/Resources/Skeletal/Atlases/${name}.png`));
 for(const n of ['HomeBalance','HomeBottom','HomeNotice','HomeSettings','HomeRules','HomeCoinTile','HomeTrack'])await copy(path.join(review,n+'.png'),path.join(out,n+'.png'));
 await copy(path.join(old,'ArtSources/approved_sky.png'),path.join(out,'Sky.png'));
 await copy(path.join(old,'Patch/Assets/Resources/Reskin/ProgressFill.png'),path.join(out,'ProgressFill.png'));
 const button='C:/Users/001/.codex/generated_images/01a09f59-e23d-72d2-b76d-56c17af35dab/exec-0f92fff7-cbd0-42ed-b4ae-677c5f052e93.png';
 fs.copyFileSync(button,path.join(__dirname,'Sources/GeneratedButton.png'));
 // AI output had baked checkerboard. Extract the continuous colored outline,
 // fill internal highlights, discard checkerboard and outer shadow completely.
 const {data,info}=await sharp(button).raw().toBuffer({resolveWithObject:true}),w=info.width,h=info.height;
 const rgba=Buffer.alloc(w*h*4);let l=w,t=h,r=0,bottom=0;
 for(let y=80;y<700;y++){
  let left=w,right=-1;for(let x=170;x<1710;x++){let i=(y*w+x)*3;const rr=data[i],g=data[i+1],bb=data[i+2];if(g-bb>55&&rr>85&&g>90){left=Math.min(left,x);right=x;}}
  if(right<=left)continue;
  l=Math.min(l,left);r=Math.max(r,right);t=Math.min(t,y);bottom=Math.max(bottom,y);
  for(let x=left;x<=right;x++){let i=(y*w+x)*3,j=(y*w+x)*4;rgba[j]=data[i];rgba[j+1]=data[i+1];rgba[j+2]=data[i+2];rgba[j+3]=(x===left||x===right)?160:255;}
 }
 await sharp(rgba,{raw:{width:w,height:h,channels:4}}).extract({left:l,top:t,width:r-l+1,height:bottom-t+1}).resize(480,216,{fit:'fill'}).png().toFile(path.join(out,'GoldButton.png'));await audit(path.join(out,'GoldButton.png'),'Sources/GeneratedButton.png; foreground color contour matte');
 fs.writeFileSync(path.join(__dirname,'asset-audit.json'),JSON.stringify({chipRawPixelsMatch:true,records},null,2));
 console.log('Prepared',records.length,'art assets; existing coin .meta preserved');
}
main().catch(e=>{console.error(e);process.exitCode=1;});
