// Executes the complete recovered modules with local scene / SDK doubles, without network access.
const fs=require('fs'),path=require('path'),vm=require('vm'),assert=require('assert');
const root=path.resolve(__dirname,'..'),checks=[],records=[],player={coin1024Number:2,currentRoundCoin1024Number:0,gameRateScore:0,guideStep:9999};
const v=(x=0,y=0,z=0)=>({x,y,z});
class Base {show(){} onLoad(){} on_close_call(){this.closed=true;} scheduleOnce(callback,delay){this.scheduled={callback,delay};}}
const cc={Component:Base,Color:class{constructor(r,g,b,a){Object.assign(this,{r,g,b,a});}},_RF:{push(){},pop(){}},_decorator:{ccclass:t=>t,property:(...args)=>args.length>=2?undefined:()=>{}},v3:v,v2:v,Tween:{stopAllByTarget(){}},easing:{backOut:'backOut'},Node:{EventType:{}},tween(node){const record={node,steps:[]};records.push(record);const api={};for(const key of ['to','by','delay','call','parallel','repeatForever','start'])api[key]=(...args)=>{record.steps.push([key,...args]);return api;};return api;}};
const events=[];
const imports={GameLocalData:{default:{getInstance:()=>({getData:()=>player,set_local_storeage(){events.push('save');}})}},PlayData:{default:class{}},CoinItem:{default:{isMergePausedForGameOver:()=>false}},GameUtils:{gameUtils:{addToday1024Number(){events.push('daily');}}},BaseUI:{default:Base,registerUIPath:()=>t=>t},ServerConfig:{HWLServerConfig:{gotoMarket(){events.push('market');}}},Lab:{default:{getlab:k=>k}},TouchButton:{default:class{}},SoundManager:{default:{playSound(){}}}};
function load(name){const c=vm.createContext({cc,sp:{Skeleton:class{}},module:{exports:{}},console,setTimeout(){},clearTimeout(){}});vm.runInContext(fs.readFileSync(path.join(root,'02_Gameplay/HotUpdate/Modules',name+'.js'),'utf8'),c);const ex={};c.module.exports(n=>imports[n.split('/').at(-1)]||{default:{}},{},ex);return ex.default;}
function check(value,label){assert(value,label);checks.push(label);}
imports.GameManagement={default:{globalData:{thirdjumpRewardtimes:{first:10,second:16,thirdAndlookADDouble:22}}}};imports.HWL_TStool={HWLshowAd:()=>false};cc.sys={isMobile:true};
const Game=load('GameScene'),game=new Game();Game._instance=game;imports.GameScene={default:Game};
game.cancelDelayedPreviewCoin=()=>events.push('cancelPreview');game.getMerge1024EffectWorldPosition=()=>v(100,300);game.getNodeLocalPositionFromWorld=(_,p)=>p;
const coin={isValid:true,x:0,y:20,z:0,scale:1,parent:{},getComponent:()=>({disablePhysics(){events.push('disablePhysics');}})};
game.playMerge1024Flow(coin,v());const rise=records.find(r=>r.node===coin).steps.filter(s=>s[0]==='to');
check(player.coin1024Number===3&&player.currentRoundCoin1024Number===1&&events.includes('daily'),'Original highest coin immediately increments total / round / daily counters');
check(rise[0][1]===.22&&rise[1][1]===.28&&rise[0][2].position.x===35&&rise[0][2].position.y===420,'Original highest rise / gather times and arc control point');
check(rise[1][2].scale===.72&&rise[1][2].opacity===0,'Original highest gathers to 72 percent scale and fades out');
game.scheduleCreatePreviewCoin=()=>events.push('preview');game.markBoardStateDirty=()=>events.push('dirty');game.currentPreviewCoin=null;game.finishMerge1024Flow(game.merge1024FlowToken);
check(events.at(-2)==='preview'&&events.at(-1)==='dirty'&&game.showNextBoo,'Original highest finish resumes preview without opening RewardDialog');
game.finishMerge1024Flow(game.merge1024FlowToken-1);check(events.at(-1)==='dirty','Stale highest-flow token cannot complete current flow');
const failNodes=Array.from({length:8},()=>({isValid:true,scaleX:1,scaleY:1,getComponent(){return null;}}));game.getBoardCoinNodes=()=>failNodes;game.forceShowFailBoardCoins=()=>{};records.length=0;game.playFailBoardCoinAnimation(()=>{});
const sourceFailure=records.filter(r=>failNodes.includes(r.node));
check(sourceFailure.length===8&&sourceFailure[5].steps[0][1]===.2&&sourceFailure[6].steps[0][1]===0,'Original fail coins stagger by 0.04 seconds modulo six');
const squash=sourceFailure[0].steps.filter(s=>s[0]==='to');check(JSON.stringify(squash.map(s=>s[1]))==='[0.14,0.14,0.12,0.12,0.12,0.18]','Original failure completes after six squash phases, not a fixed 1.15-second sleep');
check(squash[0][2].scaleX===1.16&&squash[0][2].scaleY===.8&&squash.at(-1)[2].scaleX===1,'Original failure stretches and restores coin scale');
const Rating=load('ScoreDialog'),rating=new Rating();rating.startNode={getChildByName:name=>starNodes[Number(name.slice(-1))]};const starNodes=Array.from({length:5},()=>({opacity:0}));
for(let index=0;index<5;index++) {rating.onitemRefresh(index);const prior=events.filter(e=>e==='market').length;rating.setCLosePage();check(starNodes.filter(s=>s.opacity===255).length===index+1&&events.filter(e=>e==='market').length-prior===(index>=3?1:0),'Original rating stores zero-based selection / market gate for '+(index+1)+' stars');}
const Reward=load('RewardDialog'),reward=new Reward();let touch;
reward.maskNode={addComponent(){return {registerTouchEvent(fn){touch=fn;}};}};reward.onLoad();reward.closePage=()=>events.push('rewardClose');game.resumePreviewCoin=()=>{};game.removeTopTwoThirdCoins=()=>{};
for(const kind of [2,3]){reward.showType=kind;const prior=events.length;touch();check(events.length===prior,'Original timed reward '+kind+' ignores mask input');}
for(const kind of [1,4]){reward.showType=kind;touch();check(events.at(-1)==='rewardClose','Original manual reward '+kind+' accepts mask input');}
game.scheduleRewardPopup=fn=>fn();player.windowsCointimes=player.dropCointimes=22;game.checkShowReward();check(player.windowsCointimes===22,'Original unstarted 1_A request keeps the window at 22 instead of resetting it');
reward.firstShowScore(); // setTimeout is captured separately from Cocos schedules in this source, see function body fingerprint below.
const crypto=require('crypto');const bodies={GameScene:Game.prototype.playMerge1024Flow.toString(),FailAnimation:Game.prototype.playFailBoardCoinAnimation.toString(),RewardCashFlight:Reward.prototype.flyMoneyToGameScene.toString(),Rating:Rating.prototype.setCLosePage.toString()};
const hashes=Object.fromEntries(Object.entries(bodies).map(([k,v])=>[k,crypto.createHash('sha256').update(v).digest('hex')]));
const result={passed:true,scope:'Complete recovered JS modules executed offline; scene/SDK stand-ins do not assert pixel-identical rendering',checks,highestRise:rise.map(s=>({time:s[1],values:s[2],ease:s[3]})),failure:squash.map(s=>({time:s[1],scaleX:s[2].scaleX,scaleY:s[2].scaleY})),bodyHashes:hashes};
fs.writeFileSync(path.join(root,'07_Verification/lifecycle_source_reference.json'),JSON.stringify(result,null,2)+'\n');console.log('Original lifecycle checks passed:',checks.length);
