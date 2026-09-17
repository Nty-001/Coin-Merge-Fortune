const fs=require('fs'),path=require('path'),cp=require('child_process'),crypto=require('crypto');
const project=path.resolve(__dirname,'../..'),repo=path.resolve(project,'../..');
const git='C:/Users/001/.cache/codex-runtimes/codex-primary-runtime/dependencies/native/git/cmd/git.exe';
const sha=p=>crypto.createHash('sha256').update(fs.readFileSync(p)).digest('hex');
const approved=JSON.parse(fs.readFileSync(path.join(__dirname,'approved-banknotes.json')));
for(const note of approved.notes)if(sha(path.join(project,'Assets/Resources/Localization/Currency/1',note.name))!==note.sha256)throw Error('Approved PNG changed: '+note.name);
for(const locale of approved.locales){const data=JSON.parse(fs.readFileSync(path.join(project,'Assets/Resources/Localization/Text',locale.country+'.json')));if(data.icons.some(p=>p!==locale.resource))throw Error('Old country resource '+locale.country);}
const blocks=raw=>new Map([...raw.matchAll(/^--- !u!(\d+) &([^\r\n]+)\r?\n([\s\S]*?)(?=^--- !u!|$(?![\s\S]))/gm)].map(m=>[m[2],{type:m[1],text:m[3].replace(/\r/g,'').replace(/[ \t]+$/gm,'')} ]));
const files=[];
for(const relative of ['Assets/Prefabs/Runtime/RecoveredMain.prefab','Assets/Scenes/RecoveredMain.unity','Assets/Prefabs/Runtime/CashFlight.prefab']){
 const repositoryPath='Restoration/11_UnityReskin/'+relative;
 const old=blocks(cp.execFileSync(git,['show','HEAD:'+repositoryPath],{cwd:repo,encoding:'utf8',maxBuffer:20*1024*1024})),now=blocks(fs.readFileSync(path.join(project,relative),'utf8'));
 const modified=[];let removed=0,added=0;
 for(const[id,obj]of old){if(!now.has(id)){removed++;continue;}if(obj.text!==now.get(id).text){const a=obj.text.split('\n'),b=now.get(id).text.split('\n');modified.push({id,type:obj.type,name:(obj.text.match(/m_Name: ([^\n]*)/)||[])[1]||'',removedLines:a.filter(x=>!b.includes(x)),addedLines:b.filter(x=>!a.includes(x))});}}
 for(const id of now.keys())if(!old.has(id))added++;
 files.push({relative,modified,removed,added});
}
fs.writeFileSync(path.join(__dirname,'Verification/source-audit.json'),JSON.stringify({passed:true,unchangedApprovedPngs:approved.notes.length,countries:approved.locales.length,files},null,2));
console.log(JSON.stringify(files.map(f=>({file:f.relative,modified:f.modified.length,types:f.modified.reduce((a,x)=>(a[x.type]=(a[x.type]||0)+1,a),{}),removed:f.removed,added:f.added})),null,2));
