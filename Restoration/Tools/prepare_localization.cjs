// Export actual source locale tables/icons and execute original money formatting for comparison vectors.
const fs=require('fs'),path=require('path'),vm=require('vm');
const root=path.resolve(__dirname,'..'),assets=root+'/06_UnityFramework/Assets';
const read=p=>JSON.parse(fs.readFileSync(p,'utf8').replace(/^\uFEFF/,''));
const sandbox={module:{exports:{}},cc:{Component:class{},_RF:{push(){},pop(){}},_decorator:{ccclass:x=>x,property:()=>()=>{}}}};
vm.runInNewContext(fs.readFileSync(root+'/02_Gameplay/HotUpdate/Modules/GameManagement.js','utf8'),sandbox,{timeout:5000});
const exported={};sandbox.module.exports(()=>({default:class{}}),{},exported);
const management=exported.default,languages=read(root+'/03_Configuration/HotUpdate/config/language.json');
management.languageJson=languages;
const imports=read(assets+'/Resources/Recovered/sprite_import.json').sprites;
const vectors=[],manifest=[];
for(const [key,country] of Object.entries(exported.LANGUAGE_ID)){
  const id=Number(key),language=languages[country];if(!language)throw Error('Missing actual locale '+country);
  const locale={country,id,currency:language.country,labels:Object.entries(language).filter(([k])=>k!=='country').map(([key,value])=>({key,value})),icons:[]};
  for(const type of [1,2,3]){
    const part='/texture/coin'+type+'/'+id+'__';
    const found=imports.find(x=>x.variant==='HotUpdate'&&x.path.replace(/\\/g,'/').includes(part));
    if(!found)throw Error('Missing source currency sprite '+country+' '+part);
    const target='Localization/Currency/'+type+'/'+id;
    fs.mkdirSync(path.dirname(assets+'/Resources/'+target+'.png'),{recursive:true});
    fs.copyFileSync(root+'/06_UnityFramework/'+found.path,assets+'/Resources/'+target+'.png');locale.icons.push(target);
  }
  fs.mkdirSync(assets+'/Resources/Localization/Text',{recursive:true});
  fs.writeFileSync(assets+'/Resources/Localization/Text/'+country+'.json',JSON.stringify(locale,null,2));
  management.language=country;
  for(const amount of [0,.005,.145,.999,1,1.005,1.995,9.999,100.5,234.28,999.999,1234.567,10000.995,500000])
    vectors.push({country,amount,expected:management.getmonstr(amount)});
  manifest.push({country,id,labelCount:locale.labels.length,currency:locale.currency});
}
fs.writeFileSync(root+'/07_Verification/original_money_format_vectors.json',JSON.stringify({vectors},null,2));
fs.writeFileSync(root+'/07_Verification/localization_exports.json',JSON.stringify({source:'HotUpdate/config/language.json; GameManagement.LANGUAGE_ID; original sprite UUID mapping',locales:manifest,currencyIcons:manifest.length*3},null,2));
console.log(JSON.stringify({locales:manifest.length,currencyIcons:manifest.length*3,originalFormatVectors:vectors.length}));
