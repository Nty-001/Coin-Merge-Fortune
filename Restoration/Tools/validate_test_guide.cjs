// Offline source checks only. This does not launch or automate a browser.
const fs=require('fs'),path=require('path'),vm=require('vm'),assert=require('assert');
const root=path.resolve(__dirname,'..'),html=fs.readFileSync(path.join(root,'09_TestGuide/index.html'),'utf8');
const checks=[];function check(value,label){assert(value,label);checks.push(label);}
const data=JSON.parse(html.match(/<script id="guide-data" type="application\/json">([\s\S]*?)<\/script>/)[1]);
const source=html.match(/<\/script><script>([\s\S]*?)<\/script>/)[1];
new vm.Script(source);checks.push('All inline JavaScript parses without syntax errors');
check(data.report.passed,'Embedded Unity workflow result is passed');
check(JSON.stringify(data.report)===JSON.stringify(JSON.parse(fs.readFileSync(path.join(root,'07_Verification/gm_workflow_validation.json'),'utf8').replace(/^\uFEFF/,''))),'Embedded result matches the latest verification JSON');
const ids=[...html.matchAll(/\bid="([^"]+)"/g)].map(m=>m[1]);
check(ids.length===new Set(ids).size,'Static and generated HTML IDs are unique');
const getIds=[...source.matchAll(/\$\('([^']+)'\)/g)].map(m=>m[1]);
check(getIds.every(id=>ids.includes(id)),'All literal element references have authored targets');
check(!/<(?:script|link)\b[^>]*(?:src|href)\s*=\s*["']https?:/i.test(html),'No remote JavaScript or stylesheet dependencies');
check(Object.keys(data.images).length>=2&&Object.values(data.images).every(v=>v.startsWith('data:image/png;base64,iVBOR')),'Unity screenshots are embedded PNG data');
const context=vm.createContext({group:null,route:'coin',productIndex:0});
vm.runInContext(source.slice(source.indexOf('function taskData()'),source.indexOf('function renderTasks()')),context);
let combinations=0;
for(const group of data.rules.cashGroups)for(let i=0;i<group.new_Fake_products.length;i++)for(const route of ['coin','cash']){
 context.group=group;context.productIndex=i;context.route=route;
 const actual=vm.runInContext('taskData().map(t=>t[1])',context);
 const p=(route==='coin'?group.new_Fake_products:group.real_products)[i];
 const expected=route==='coin'?[p.condition_merge,p.condition_ad,p.withdrawAmount,p.condition_login_days,p.condition_video]:[p.withdrawAmount,p.condition_merge,p.condition_login_days,p.condition_video];
 assert.deepStrictEqual(Array.from(actual),expected);combinations++;
}
checks.push('Task graph values match recovered configuration for '+combinations+' region-group/product/route combinations');
check(data.rules.supportedCountries.every(c=>data.rules.cashGroups.some(g=>g.countries.includes(c))),'Every offered country maps to a recovered cash group');
const nodes={};const simulator=vm.createContext({$:id=>nodes[id]||(nodes[id]={style:{}})});
vm.runInContext(source.slice(source.indexOf('let sim;'),source.indexOf('const C=[')),simulator);
vm.runInContext('resetSim();advanceSim(12);advanceSim(12)',simulator);
check(vm.runInContext('sim.days===14&&sim.day===2&&sim.merges===0',simulator),'Calendar simulator does not award a day merely for +24h');
vm.runInContext('for(let i=0;i<4;i++)mergeSim()',simulator);
check(vm.runInContext('sim.days===14',simulator),'Calendar simulator keeps the threshold blocked after four merges');
vm.runInContext('mergeSim();mergeSim()',simulator);
check(vm.runInContext('sim.days===15',simulator),'Fifth merge completes the day and extra same-day merges do not double count');
vm.runInContext('advanceSim(24);for(let i=0;i<5;i++)mergeSim()',simulator);
check(vm.runInContext('sim.days===16',simulator),'A later date with five merges adds exactly one day');
const lists=vm.createContext({});
vm.runInContext(source.slice(source.indexOf('const C=['),source.indexOf('let saved=')),lists);
const cases=vm.runInContext('C',lists);
check(cases.length===42&&new Set(cases.map(c=>c[0])).size===42,'Checklist contains 42 distinct manual cases');
vm.runInContext(source.slice(source.indexOf('const S=['),source.indexOf("$('sources').innerHTML")),lists);
check(vm.runInContext('S',lists).every(s=>fs.existsSync(path.join(root,'02_Gameplay/HotUpdate/Modules',s[0]))),'Every original-module source link resolves');
const result={passed:true,scope:'Offline HTML structure, JavaScript syntax, embedded-data freshness and pure task/simulator logic; no browser rendering verification',checks,taskCombinations:combinations,manualCases:cases.length};
fs.writeFileSync(path.join(root,'07_Verification/html_guide_validation.json'),JSON.stringify(result,null,2)+'\n');
console.log(JSON.stringify(result,null,2));
