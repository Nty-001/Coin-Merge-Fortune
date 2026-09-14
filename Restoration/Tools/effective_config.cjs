const fs=require('fs'),path=require('path'),vm=require('vm');
const root=path.resolve(__dirname,'..');
const code=fs.readFileSync(root+'/02_Gameplay/HotUpdate/Modules/GameManagement.js','utf8');
const context={module:{exports:{}},cc:{Component:class{},_RF:{push(){},pop(){}}}};
vm.runInNewContext(code,context,{timeout:5000});
const exp={};context.module.exports(()=>({default:{}}),{},exp);
const manager=exp.default;
const target=root+'/03_Configuration/EffectiveRuntime';fs.mkdirSync(target,{recursive:true});
fs.writeFileSync(target+'/CodeDefaults.json',JSON.stringify(manager.getDefaultGlobalConfig(),null,2));
for(const [name,rel] of Object.entries({Cached:'CapturedDevice/coinGameData.json',PackagedDefault:'HotUpdate/config/data.json'})){
 const raw=JSON.parse(fs.readFileSync(root+'/03_Configuration/'+rel,'utf8'));
 const result=manager.normalizeBlastConfig(raw);
 fs.writeFileSync(target+'/'+name+'.normalized.json',JSON.stringify(result,null,2));
}
console.log('Executed original normalization function offline; saved defaults and effective values.');
function moduleExports(variant,name){
 const sandbox={module:{exports:{}},sp:{Skeleton:class{}},cc:{Component:class{},_RF:{push(){},pop(){}},_decorator:{ccclass:x=>x,property:(...args)=>typeof args[1]==='string'?undefined:()=>{}}}};
 vm.runInNewContext(fs.readFileSync(root+'/02_Gameplay/'+variant+'/Modules/'+name+'.js','utf8'),sandbox,{timeout:5000});
 const exports={};sandbox.module.exports(()=>({default:class{static getInstance(){return new this();}}}),{},exports);return exports;
}
const local=moduleExports('Packaged','LocalDataManager').default;
fs.writeFileSync(target+'/Packaged.GMConfig.json',JSON.stringify(local.prototype.getGMConfig(),null,2));
const coin=moduleExports('HotUpdate','CoinItem').default;
const instance=new coin();
fs.writeFileSync(target+'/HotUpdate.CoinItem.constants.json',JSON.stringify({instance:Object.fromEntries(Object.entries(instance).filter(([k,v])=>v!==null&&typeof v!=='function')),static:Object.fromEntries(Object.entries(coin).filter(([k,v])=>typeof v!=='function'))},null,2));
const utils=moduleExports('HotUpdate','GameUtils').gameUtils;
const util=typeof utils==='function'?new utils():utils;
fs.writeFileSync(target+'/HotUpdate.GameUtils.defaults.json',JSON.stringify({videoRewardConfig:util.videoRewardConfig,defaultLotteryScoreConfig:util.defaultLotteryScoreConfig,realProducts:util.getDefaultRealProductsConfig()},null,2));
