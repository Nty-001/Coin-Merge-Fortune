const fs=require('fs'),path=require('path'),crypto=require('crypto');
const sharp=require('C:/Users/001/.cache/codex-runtimes/codex-primary-runtime/dependencies/node/node_modules/sharp');
const dir=path.join(__dirname,'PureArt_R2'),art=path.join(dir,'Assets/Resources/HomeReskin'),coin=path.join(__dirname,'Staging/Assets/Resources/HomeReskin');
(async()=>{
 const layers=[],values=[1,2,5,10,20,50,100,200,500,1000,2000];
 for(let i=0;i<11;i++)layers.push({input:await sharp(path.join(coin,`Coin${values[i]}.png`)).resize(112,112).png().toBuffer(),left:28+(i%6)*158,top:640+Math.floor(i/6)*140});
 for(const [name,x,y,w,h] of [['Balance',28,75,348,101],['GreenButton',410,75,198,102],['Settings',650,75,100,100],['Rules',790,75,100,100],['Notice',28,245,727,113],['NextTile',790,245,100,100],['Bottom',28,400,900,240],['Track',60,463,540,57],['Fill',69,474,332,39]])layers.push({input:await sharp(path.join(art,name+'.png')).resize(w,h).png().toBuffer(),left:x,top:y});
 const text=Buffer.from('<svg width="960" height="950" xmlns="http://www.w3.org/2000/svg"><g font-family="Arial" fill="#123e64"><text x="28" y="40" font-size="25">HOME ART ONLY - R2</text><text x="28" y="218" font-size="17">Blank surfaces - existing game text and controls stay native</text><text x="28" y="918" font-size="17">11 chip levels | No layout / font / gameplay changes | No aiming dots</text></g></svg>');
 layers.push({input:text,left:0,top:0});
 await sharp({create:{width:960,height:950,channels:4,background:'#7acdf5'}}).composite(layers).png().toFile(path.join(dir,'MATERIALS_R2.png'));
 const files=[];function walk(p){for(const e of fs.readdirSync(p,{withFileTypes:true})){const f=path.join(p,e.name);if(e.isDirectory())walk(f);else files.push({path:path.relative(dir,f).replaceAll('\\','/'),sha256:crypto.createHash('sha256').update(fs.readFileSync(f)).digest('hex')});}}walk(path.join(dir,'Assets'));
 fs.writeFileSync(path.join(dir,'CANDIDATE.json'),JSON.stringify({version:'HOME_ART_ONLY_R2',scope:'PURE_ART',status:'Awaiting explicit generated-material confirmation required by automatic approval review',layoutChanges:false,fontsChanged:false,runtimeCodeChanged:false,aimingDots:false,files},null,2));
 console.log(files.length+' candidate files fingerprinted');
})();
