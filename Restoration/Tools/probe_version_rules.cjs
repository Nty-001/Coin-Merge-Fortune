// Offline comparison fixtures from COMPLETE recovered JS function bodies; no device identifiers/network.
const fs=require('fs'),path=require('path'),vm=require('vm'),crypto=require('crypto');
const root=path.resolve(__dirname,'..');
const output=process.argv[2]||path.join(root,'06_UnityFramework/Assets/Config/Runtime/VersionRuleVectors.json');
function body(file,signature){const s=fs.readFileSync(path.join(root,file),'utf8');const start=s.indexOf(signature);if(start<0)throw Error(signature);const begin=s.indexOf('{',start);let depth=1,i=begin+1;for(;depth;i++){if(s[i]==='{')depth++;if(s[i]==='}')depth--;}return s.slice(begin+1,i-1);}
const nativeFile='02_Gameplay/HotUpdate/Modules/NativeCall.js';
const abBody=body(nativeFile,'static checkClientEndingWith(e)');
let label='';const context={cc:{sys:{localStorage:{setItem:(k,v)=>{label=v;}}}},console:{log:()=>{}}};
const ab=vm.runInNewContext('(function(e){'+abBody+'})',context);
const tails=['',...Array.from('0123456789abcdefABCDEF-_')].map(t=>{const id=t?'synthetic-test-'+t:'';ab(id);return {input:id,cohort:label};});
const dropBody=body('02_Gameplay/Packaged/Modules/LevelUtils.js','static genNextBlockType(e)');
let draw=0;const drop=vm.runInNewContext('(function(e){'+dropBody+'})',{s:{BlockType:{type_min:1}},i:{default:{getRandomIntInRange:(min,max)=>min+Math.floor(draw/1000000*(max-min+1))}}});
const rules=JSON.parse(fs.readFileSync(path.join(root,'03_Configuration/Packaged/Json/next_config.json'),'utf8'));const drops=[];
for(const pass of [0,1,3])for(let max=1;max<=11;max++)for(const sample of [0,1,9999,99999,199999,249999,250000,349999,399999,499999,500000,749999,750000,999998,999999]){
 const table=pass<=0?rules.firstAppearRule:rules.appearRule,value=pass<=0?max:pass;
 const keys=Object.keys(table).map(Number).sort((a,b)=>a-b);let key=keys[0];for(const k of keys)if(k<=value)key=k;
 draw=sample;drops.push({pass,max,draw:sample,type:drop(table[key])});
}
const result={scope:'Original NativeCall.checkClientEndingWith and LevelUtils.genNextBlockType bodies; synthetic identifiers only',abBodySha256:crypto.createHash('sha256').update(abBody).digest('hex'),dropBodySha256:crypto.createHash('sha256').update(dropBody).digest('hex'),tails,drops};
fs.writeFileSync(output,JSON.stringify(result,null,2));console.log(JSON.stringify({tails:tails.length,drops:drops.length,output}));
