// Pure-art import preparation: copy baseline texture dimensions, alpha silhouette
// and importer settings. Never edits RectTransforms, text, Buttons, or game logic.
const fs=require('fs'),path=require('path'),crypto=require('crypto');
const sharp=require('C:/Users/001/.cache/codex-runtimes/codex-primary-runtime/dependencies/node/node_modules/sharp');
const project=path.resolve(__dirname,'../..'),root=path.resolve(project,'../..');
const stage=path.join(__dirname,'Staging/Assets/Resources/HomeReskin'),candidate=path.join(__dirname,'PureArt_R2'),out=path.join(candidate,'Assets/Resources/HomeReskin');
const sources=path.join(__dirname,'Sources'),old=path.join(root,'Restoration/06_UnityFramework/Assets/Art/HotUpdate/Sprites');
const generated='C:/Users/001/.codex/generated_images/01a09f59-e23d-72d2-b76d-56c17af35dab/';
const rows=[
 ['Sky','bg__eb47e646',path.join(stage,'Sky.png')],
 ['Balance','编组 4备份 5__284583c4',generated+'exec-4c5c085b-0282-4ed9-9813-d3812ae3fb0b.png',{left:0,top:87,width:2170,height:537}],
 ['Notice','bg1__d7add077',generated+'exec-f567d3fb-5a1f-4855-893d-3bc11db60c5a.png',{left:3,top:202,width:2164,height:314}],
 ['Bottom','1__3ff9510d',generated+'exec-e87efc43-c547-4bb9-8ca1-cb8ca9459724.png',{left:2,top:50,width:2168,height:614}],
 ['GreenButton','btn3__f6455ee9',path.join(stage,'GoldButton.png')],
 ['Settings','btn4__cf37e82c',path.join(stage,'HomeSettings.png')],
 ['Rules','btn5__096204b8',path.join(stage,'HomeRules.png')],
 ['NextTile','btn_bg__b8612d37',path.join(stage,'HomeCoinTile.png')],
 ['Track','progressBg__ff5418d6',path.join(stage,'HomeTrack.png')],
 ['Fill','progressBar__632014c3',path.join(stage,'GoldButton.png')]
];
function bounds(data,w,h,threshold){let l=w,t=h,r=-1,b=-1;for(let y=0;y<h;y++)for(let x=0;x<w;x++)if(data[(y*w+x)*4+3]>=threshold){l=Math.min(l,x);t=Math.min(t,y);r=Math.max(r,x);b=Math.max(b,y);}return [l,t,r+1,b+1];}
(async()=>{
 fs.mkdirSync(out,{recursive:true});const audit=[];
 for(const [name,original,src,crop] of rows){
  const baseline=path.join(old,original+'.png');const {data:base,info}=await sharp(baseline).ensureAlpha().raw().toBuffer({resolveWithObject:true});
  let paint=sharp(src);if(crop)paint=paint.extract(crop);if(name==='Fill')paint=paint.extract({left:235,top:32,width:10,height:150});
  // Bleed painted edge RGB into transparent/checkerboard margins before the
  // original alpha is restored, preventing gray/black fringes at different radii.
  const pixels=await paint.ensureAlpha().raw().toBuffer({resolveWithObject:true});
  const pd=pixels.data,pw=pixels.info.width,ph=pixels.info.height;
  if(crop)for(let y=0;y<ph;y++){
   let left=pw,right=-1;for(let x=0;x<pw;x++){const i=(y*pw+x)*4,r=pd[i],g=pd[i+1],b=pd[i+2];
    if(name==='Bottom'?(Math.min(r,g,b)>222||(b-r>45&&g-r>20)):(b-r>55&&g-r>25)){left=Math.min(left,x);right=x;}}
   for(let x=0;x<pw;x++)pd[(y*pw+x)*4+3]=(x>=left&&x<=right)?255:0;
  }
  const alpha=Buffer.alloc(pw*ph);for(let p=0;p<pw*ph;p++)alpha[p]=pd[p*4+3];
  const seen=new Uint8Array(pw*ph),queue=new Int32Array(pw*ph);let head=0,tail=0;
  for(let p=0;p<pw*ph;p++)if(pd[p*4+3]>240){seen[p]=1;queue[tail++]=p;}
  while(head<tail){const p=queue[head++],x=p%pw;for(const q of [x>0?p-1:-1,x<pw-1?p+1:-1,p>=pw?p-pw:-1,p<(ph-1)*pw?p+pw:-1]){
   if(q<0||seen[q])continue;seen[q]=1;queue[tail++]=q;for(let c=0;c<3;c++)pd[q*4+c]=pd[p*4+c];}}
  for(let p=0;p<pw*ph;p++)pd[p*4+3]=255;
  const data=await sharp(pd,{raw:{width:pw,height:ph,channels:4}}).resize(info.width,info.height,{fit:'fill'}).removeAlpha().raw().toBuffer();
  const finalAlpha=await sharp(alpha,{raw:{width:pw,height:ph,channels:1}}).resize(info.width,info.height,{fit:'fill'}).extractChannel(0).raw().toBuffer();
  if(finalAlpha.length!==info.width*info.height)throw Error('Unexpected matte channel count');
  const rgba=Buffer.alloc(info.width*info.height*4);
  for(let p=0;p<info.width*info.height;p++){rgba[p*4]=data[p*3];rgba[p*4+1]=data[p*3+1];rgba[p*4+2]=data[p*3+2];rgba[p*4+3]=(name==='Sky'||name==='Fill')?base[p*4+3]:finalAlpha[p];}
  const target=path.join(out,name+'.png');await sharp(rgba,{raw:{width:info.width,height:info.height,channels:4}}).png().toFile(target);
  const guid=crypto.createHash('md5').update('CoinMerge.Reskin.PureArt.Home.R1.'+name).digest('hex');
  const meta=fs.readFileSync(baseline+'.meta','utf8').replace(/^guid: .+$/m,'guid: '+guid);fs.writeFileSync(target+'.meta',meta);
  if(crop)fs.copyFileSync(src,path.join(sources,'Blank'+name+'.png'));
  const alphaBounds={};for(const t of [1,4,8,16])alphaBounds[t]=bounds(rgba,info.width,info.height,t);
  audit.push({name,guid,baseline:path.relative(root,baseline),output:path.relative(project,target),width:info.width,height:info.height,alphaBounds,originalAlphaPreserved:name==='Sky'||name==='Fill',originalCanvasDimensionsPreserved:true,importerSettings:'identical to source except new GUID',source:crop?'Sources/Blank'+name+'.png':path.relative(__dirname,src)});
 }
 const staged=path.join(__dirname,'Staging/Assets/Resources');
 for(const n of [1,2,5,10,20,50,100,200,500,1000,2000])fs.copyFileSync(path.join(staged,`Gameplay/Coins/${n}.png`),path.join(candidate,`Assets/Resources/Gameplay/Coins/${n}.png`));
 for(const n of ['AnNiu_TX','HeChengSM_TX','DJB_TX'])fs.copyFileSync(path.join(staged,`Skeletal/Atlases/${n}.png`),path.join(candidate,`Assets/Resources/Skeletal/Atlases/${n}.png`));
 fs.writeFileSync(path.join(__dirname,'pure-art-manifest.json'),JSON.stringify({scope:'PURE_ART',approval:'User: 只换美术',layoutChanged:false,runtimeCodeChanged:false,aimDotsAdded:false,art:audit},null,2));
 console.log('Prepared',audit.length,'scoped sprites. Original canvas dimensions and importer settings retained.');
})().catch(e=>{console.error(e);process.exitCode=1});
