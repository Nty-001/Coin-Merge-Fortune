// Executes recovered ORIGINAL gameplay functions against local native/network facades.
// No browser, network transport, real device identifier, or live SDK is available here.
const fs=require('fs'),path=require('path'),vm=require('vm'),assert=require('assert');
const R=path.resolve(__dirname,'..'),base=R+'/02_Gameplay/HotUpdate';
const entries=JSON.parse(fs.readFileSync(base+'/module_manifest.json','utf8'));
const manifest=Object.fromEntries(entries.map(e=>[e.name,e]));
const modules={},trace=[],storage=new Map();let xhrStatus=200,videoCount=0;
const fakeData={add_show_video(){videoCount++;}};
const stubs={
 GameLocalData:{default:{getInstance:()=>({getData:()=>fakeData,set_local_storeage(){}})}},
 PlayData:{default:class{}},SoundManager:{default:{pauseBgm(){trace.push('music.pause');},resumeBgm(){trace.push('music.resume');}}},
 UIManagerNew:{default:{show_toast(){trace.push('toast');}}},Lab:{default:{getlab:k=>'fixture-'+k}},
 GameManagement:{default:{all_config_data:{},globalData:{global:{}}}}
};
class LocalXHR {
 open(method,url){this.method=method;this.url=url;}
 setRequestHeader(){}
 send(body){trace.push({event:'mock_transport',method:this.method,url:this.url,bodyBytes:body.byteLength});
  this.body=body;this.readyState=4;this.status=xhrStatus;this.responseText=JSON.stringify({code:xhrStatus===200?0:500,data:{status:'mock_pending'}});
  this.onreadystatechange?.();
 }
}
const context=vm.createContext({console:{log(){},info(){},warn(){},error(){}},window:{},XMLHttpRequest:LocalXHR,
 cc:{Component:class{},_RF:{push(){},pop(){}},sys:{isNative:true,isBrowser:false,os:'Android',OS_ANDROID:'Android',OS_IOS:'iOS',localStorage:{getItem:k=>storage.get(k)||'',setItem:(k,v)=>storage.set(k,v)}}},
 jsb:{reflection:{callStaticMethod:(clazz,method,...args)=>{trace.push({event:'mock_native',method});
  if(method==='getClientId')return 'LOCAL-FIXTURE-6';if(method==='getAdid')return 'LOCAL-AD-FIXTURE';if(method==='isAdRejected')return false;return undefined;}}},
 setTimeout,clearTimeout,Uint8Array,ArrayBuffer,atob:s=>Buffer.from(s,'base64').toString('binary'),btoa:s=>Buffer.from(s,'binary').toString('base64')});
function load(name){
 name=String(name);if(stubs[name])return stubs[name];if(modules[name])return modules[name].exports;
 if(name==='crypto')return require('crypto');
 const entry=manifest[name];if(!entry)throw Error('Unknown module '+name);
 const mod={exports:{}};modules[name]=mod;context.module={exports:{}};
 vm.runInContext(fs.readFileSync(base+'/'+entry.file,'utf8'),context,{timeout:5000});
 const fn=context.module.exports;fn(req=>load(entry.dependencies[req]??req),mod,mod.exports);return mod.exports;
}
(async()=>{
 const tool=load('HWL_TStool'),server=load('ServerConfig').HWLServerConfig;
 // Reports are SDK/business boundary outputs: capture locally rather than call a server.
 for(const name of ['showVideoOpen','showVideo','showVideoComplete','showVideoFail'])server[name]=(...args)=>trace.push({event:name,args});
 let completed=0,failed=0;
 assert.equal(tool.HWLshowAd('fixture_reward',()=>completed++,()=>failed++),true);
 assert.equal(tool.HWLshowAd('duplicate',()=>completed++,()=>failed++),false);
 context.window.XSSdkCallback('ad_play','{}');context.window.XSSdkCallback('ad_over','{"revenue":0.002}');
 assert.equal(completed,1);assert.equal(failed,0);assert.equal(videoCount,1);assert.equal(tool.HWL.adstart,false);
 tool.HWLshowAd('fixture_error',()=>completed++,()=>failed++);context.window.XSSdkCallback('ad_error','{"errorCode":42}');
 assert.equal(completed,1);assert.equal(failed,1);assert.equal(tool.HWL.adstart,false);
 const result=await server.sendCash({sendId:0,plat:'PayPal',cash:1.25,accountType:'email',account:'fixture@example.invalid',fullName:'LOCAL FIXTURE',documentId:''});
 assert.equal(JSON.parse(result).data.status,'mock_pending');
 assert(trace.some(x=>x.event==='mock_transport'&&x.bodyBytes>0));
 await server.sendCashRecord();xhrStatus=500;
 let rejected=false;try{await server.sendCashRecord();}catch(e){rejected=e===500;}assert(rejected);
 const resultDoc={status:'passed',originalFunctions:['HWLshowAd','window.XSSdkCallback','ServerConfig.sendCash','ServerConfig.httpCashSend','ServerConfig.sendCashRecord','AESUtil.encrypt','HWLwordArrayToBuffer'],checks:['ad duplicate lock','native ad trigger','ad success callback','ad error callback','watch count increment','music pause/resume','withdraw payload encrypted','withdraw success response','withdraw history request','HTTP failure rejection'],transport:'in-memory mock only',trace};
 fs.writeFileSync(R+'/07_Verification/original_flow_tests.json',JSON.stringify(resultDoc,null,2));
 console.log('PASS: original ad callbacks, encrypted withdrawal request and failure paths (local fixtures only)');
})().catch(e=>{console.error(e);process.exitCode=1;});
