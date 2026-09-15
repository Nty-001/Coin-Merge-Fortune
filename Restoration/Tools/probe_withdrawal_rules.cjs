// Executes complete recovered method bodies offline; all accounts are synthetic fixtures.
const fs=require('fs'),path=require('path'),vm=require('vm'),crypto=require('crypto');
const root=path.resolve(__dirname,'..'),project=process.argv[2]||path.join(root,'08_ValidationUnity');
const hashes={};
function method(file,signature,args,scope={}){
 const source=fs.readFileSync(path.join(root,'02_Gameplay/HotUpdate/Modules',file+'.js'),'utf8');
 const start=source.indexOf('\n'+signature+' {');if(start<0)throw Error(signature);const begin=source.indexOf('{',start);let depth=1,end=begin+1;
 for(;depth;end++){if(source[end]==='{')depth++;if(source[end]==='}')depth--;}
 const body=source.slice(begin+1,end-1);hashes[file+'.'+signature]=crypto.createHash('sha256').update(body).digest('hex');
 return vm.runInNewContext('(function('+args+'){'+body+'})',scope);
}
const accountSource=fs.readFileSync(path.join(root,'02_Gameplay/HotUpdate/Modules/AccountCheckManager.js'),'utf8');
const moduleBox={exports:{}};vm.runInNewContext(accountSource,{module:moduleBox,cc:{Component:class{},_RF:{push(){},pop(){}}}});
const exportsBox={};moduleBox.exports(()=>{}, {}, exportsBox);const validator=exportsBox.default;
const strings=['','x','Test Player','汉字用户','a@b.co',' Fixture@Example.Test','Fixture@Example.Test','a..b@example.test','.abc@example.test','abc.@example.test',
 'a'.repeat(65)+'@example.test','a@'+'a'.repeat(245)+'.test','a@example.test\n','a@example.test\u2028','08123456789','08-1234-56789','0012345678','01234567890',
 '84123456789','09123456789','+551234567890','00000000000','１２３４５６７８９０１','abcdefghijklmnopqrstuvwxyz1234567890',' x ','abc\n'];
const validators=[];for(const name of ['validateEmail','validateAccount','validatePhone08','validatePhone10','validatePhone10Or11','validatePhone84','validatePhone09','validatePhone55','validateCPFJ','validatePix'])for(const input of strings){const result=validator[name](input);validators.push({method:name,input,valid:result.valid,normalized:result.email||''});}
hashes.AccountCheckManager=crypto.createHash('sha256').update(accountSource).digest('hex');
const money=[],ctx={language:'US'},moneyThis={countryCashName:'$',getCountryIdByCountry:()=>1};
moneyThis.roundToDecimal=method('GameManagement','static roundToDecimal(e, t)','e,t');moneyThis.toFixed=method('GameManagement','static toFixed(e, t)','e,t');
const moneyFn=method('GameManagement','static getRealMonstr(e)','e',{c:ctx});
for(const file of fs.readdirSync(path.join(project,'Assets/Resources/Localization/Text')).filter(f=>f.endsWith('.json'))){const locale=JSON.parse(fs.readFileSync(path.join(project,'Assets/Resources/Localization/Text',file),'utf8'));moneyThis.countryCashName=locale.currency;moneyThis.getCountryIdByCountry=()=>locale.id;
 for(const amount of [0,.005,.999,1.005,234.28,499.99,500,999.999,12345.675,10000000])money.push({country:locale.country,amount,expected:moneyFn.call(moneyThis,amount)});
}
const rules=JSON.parse(fs.readFileSync(path.join(project,'Assets/Config/Runtime/RecoveredRules.json'),'utf8'));const stages=[];let player;
const scope={s:{default:{getInstance:()=>({getData:()=>player,saveToUserDefault(){}})}},l:{default:{}},m:{default:{instance:null}},d:{default:{GameRealWDTXTips:'tips'}},h:{default:{show_ui(){}}},f:{default:{open(){}}}};
const fixture={currentIndex:0,on_close_call(){},getWithdrawConditionCount:method('GameRealTXYZ','getWithdrawConditionCount()',''),getNumber:method('GameRealTXYZ','getNumber(e)','e')};
fixture.isWithdrawConditionMet=method('GameRealTXYZ','isWithdrawConditionMet(e, t)','e,t');fixture.findFirstUnmetWithdrawStep=method('GameRealTXYZ','findFirstUnmetWithdrawStep(e, t)','e,t');fixture.ensureWithdrawStepData=method('GameRealTXYZ','ensureWithdrawStepData(e)','e');
const advance=method('GameRealTXYZ','showNextWithdrawResult()','',scope);
let localeLabels={};const setProgress=method('GameRealWDDialog','setProgress(e)','e',{f:{default:{getlab:key=>localeLabels[key]||key}},cc:{misc:{clamp01:v=>Math.min(Math.max(v,0),1)}}});
// One representative per recovered currency/config group; real product thresholds are retained.
const countries=['US','MX','TH','JP','CO','ID'];
for(let groupIndex=0;groupIndex<6;groupIndex++){
 const country=countries[groupIndex],locale=JSON.parse(fs.readFileSync(path.join(project,'Assets/Resources/Localization/Text',country+'.json'),'utf8'));localeLabels=Object.fromEntries(locale.labels.map(x=>[x.key,x.value]));
 const group=rules.cashGroups[groupIndex],product=group.new_Fake_products[0];fixture.coinConfig=product;
 for(const current of [0,1,2,3,4,5])for(const coin of [product.condition_merge-1,product.condition_merge])for(const videos of [0,product.condition_ad,product.condition_video])for(const enough of [false,true]){
  player={coin1024Number:coin,watch_video_count:videos,fakeMoney:enough?product.withdrawAmount:product.withdrawAmount-.01,loginDays:enough?product.condition_login_days:0,newFakeMoneyWithdraw:[current]};
  const p={...player};fixture.withdrawResultShown=false;advance.call(fixture);
  const view={playerData:{...p,newFakeMoneyWithdraw:[current]},progressBar:{},progressLabel:{},bottomTips:{},getConfig:()=>group,tixianBtn:{},tixianBtnGray:{}};setProgress.call(view,0);
  stages.push({country,current,coin,videos,money:p.fakeMoney,days:p.loginDays,next:player.newFakeMoneyWithdraw[0],progress:Number.isNaN(view.progressBar.progress)?'NaN':String(view.progressBar.progress),ready:view.allConditionsMet,hint:view.bottomTips.string.replaceAll('</c>','</color>')});
 }
}
const output=path.join(project,'Assets/Config/Runtime/WithdrawalVectors.json');fs.writeFileSync(output,JSON.stringify({hashes,validators,money,stages},null,2));console.log(JSON.stringify({validators:validators.length,money:money.length,stages:stages.length,output}));
