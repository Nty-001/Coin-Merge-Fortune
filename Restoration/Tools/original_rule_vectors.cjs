// Execute original JS bodies to produce expected results for the C# port.
const fs=require('fs'),path=require('path'),vm=require('vm');
const R=path.resolve(__dirname,'..'),base=R+'/02_Gameplay/HotUpdate';
const read=p=>JSON.parse(fs.readFileSync(p,'utf8').replace(/^\uFEFF/,''));
const config=read(R+'/03_Configuration/EffectiveRuntime/Cached.normalized.json');
const manifest=Object.fromEntries(read(base+'/module_manifest.json').map(x=>[x.name,x]));
let sample=0,player={},manager;const local=new Map();
class Singleton {static getInstance(){return new this();}}
const defaults={GameLocalData:{default:{getInstance:()=>({getData:()=>player,set_local_storeage(){}})}},Singleton:{default:Singleton}};
const math=Object.create(Math);math.random=()=>sample;
const context=vm.createContext({module:{exports:{}},Math:math,console:{log(){},warn(){},error(){}},sp:{Skeleton:class{}},
 cc:{Component:class{},_RF:{push(){},pop(){}},_decorator:{ccclass:x=>x,property:(...a)=>typeof a[1]==='string'?undefined:()=>{}},
 sys:{localStorage:{getItem:k=>local.get(k)||null,setItem:(k,v)=>local.set(k,v)}}}});
const loaded={};
function load(name){
 if(defaults[name])return defaults[name];if(loaded[name])return loaded[name];
 if(!manifest[name])return {default:class{}};
 const entry=manifest[name];context.module={exports:{}};vm.runInContext(fs.readFileSync(base+'/'+entry.file,'utf8'),context,{timeout:5000});
 const fn=context.module.exports,exp={};loaded[name]=exp;
 fn(dep=>{const target=entry.dependencies[dep];if(target==='BaseUI')return {default:class{},registerUIPath:()=>x=>x};return ['GameManagement','GameLocalData','Singleton','PlayData'].includes(target)?load(target):{default:class{}};},{},exp);return exp;
}
manager=load('GameManagement').default;manager.all_config_data.GameData=config;
const utils=load('GameUtils').gameUtils,scene=new (load('GameScene').default)(),lottery=new (load('LuckDrawDialog').default)();
const vectors=[];
for(const maximum of [0,1,2,4,5,9,10,20,50,100,200,500,1000,2000])for(const random of [0,.149999999,.15,.2,.3,.45,.5,.7,.99999999]){
 sample=random;scene.gameArea={childrenCount:3,children:[{isValid:true,active:true,getComponent:()=>({value:maximum,isPreview:false,isMerging:false})},{isValid:true,active:true,getComponent:()=>({value:2000,isPreview:true})},{isValid:true,active:false,getComponent:()=>({value:2000})}]};
 vectors.push({kind:'drop',maximum,sample,expected:scene.getRandomDropValue()});
}
for(const random of [0,.149999999,.15,.150000001,.3,.5,.65,.85,.99999999]){sample=random;vectors.push({kind:'lottery',sample,expected:lottery.getRandomReward(config.lotteryConfig)});}
for(const country of ['US','BR','GB','MX','ZA','TH','IN','ID','VN','JP','NG','KR','CO'])for(const balance of [0,199.99,200,299.99,300,489.99,490,499.99,500,799.99,800,999.99,1000,4999.99,5000,9000])for(const random of [0,.5,.99999999]){
 sample=random;player={fakeMoney:balance};manager.language=country;
 vectors.push({kind:'cash',country,balance,sample,expected:utils.calculateCashReward()});
}
for(const watched of [0,30,31,50,51,100,101,200,201,999999,1000000])for(const random of [0,.5,.99999999]){
 sample=random;vectors.push({kind:'video',watched,sample,expected:utils.getVideoRewardQ(watched)});
}
for(const count of [0,1,2,5,10,100])for(const score of [0,49,50,99,100,499,500,1000,9999]){
 player={gameTotalScore:score,currentLotteryCount:count};
 vectors.push({kind:'spins',count,score,expected:utils.getAvailableLotteryTimes()});
 vectors.push({kind:'score',count,score,expected:utils.getRequiredScore(count)});
}
fs.writeFileSync(R+'/07_Verification/original_rule_vectors.json',JSON.stringify({source:'Original recovered JS executed in isolated VM; all expected values are computed, not copied from C#.',vectors},null,2));
console.log('Original JS rule vectors: '+vectors.length);
