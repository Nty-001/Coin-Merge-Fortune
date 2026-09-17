const fs=require('fs'),path=require('path'),crypto=require('crypto');
const root=path.resolve(__dirname,'../..');
const hash=p=>crypto.createHash('sha256').update(fs.readFileSync(p)).digest('hex');
const notes=path.join(root,'Assets/Resources/Localization/Currency/1');
const text=path.join(root,'Assets/Resources/Localization/Text');
const locales=[];
for(const name of fs.readdirSync(text).filter(x=>x.endsWith('.json'))){
  const file=path.join(text,name),raw=fs.readFileSync(file,'utf8'),locale=JSON.parse(raw);
  const resource=locale.icons[0],png=path.join(root,'Assets/Resources',resource+'.png');
  if(!resource.startsWith('Localization/Currency/1/')||!fs.existsSync(png))throw Error('Missing approved banknote '+name);
  // All three display roles share the exact approved single note; pile composition is authored UI.
  fs.writeFileSync(file,raw.replace(/"Localization\/Currency\/[23]\//g,'"Localization/Currency/1/'));
  locales.push({country:locale.country,id:locale.id,currency:locale.currency,resource,sha256:hash(png)});
}
fs.mkdirSync(path.join(__dirname,'Verification'),{recursive:true});
fs.writeFileSync(path.join(__dirname,'approved-banknotes.json'),JSON.stringify({locales,notes:fs.readdirSync(notes).filter(x=>x.endsWith('.png')).map(name=>({name,sha256:hash(path.join(notes,name))}))},null,2));
console.log('Mapped '+locales.length+' countries to approved banknotes. Original PNG bytes are unchanged.');
