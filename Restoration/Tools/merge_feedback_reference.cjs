// Execute recovered GameScene functions locally with scene/audio/SDK stand-ins.
const fs=require('fs'),path=require('path'),vm=require('vm'),assert=require('assert');
const root=path.resolve(__dirname,'..');const read=p=>JSON.parse(fs.readFileSync(p,'utf8').replace(/^\uFEFF/,''));
const ui={ui_is_loading:{},all_ui:{}},ad={adstart:false},sounds=[];
const cc={Component:class{},_RF:{push(){},pop(){}},_decorator:{ccclass:t=>t,property:(...args)=>args.length>=2?undefined:()=>{}},v3:(x,y,z)=>({x,y,z}),director:{getScene:()=>({name:'GameScene'})},Node:{EventType:{TOUCH_START:1,TOUCH_MOVE:2,TOUCH_END:3,TOUCH_CANCEL:4}}};
const context=vm.createContext({cc,sp:{Skeleton:class{}},module:{exports:{}},console,setTimeout(){},clearTimeout(){}});
vm.runInContext(fs.readFileSync(root+'/02_Gameplay/HotUpdate/Modules/GameScene.js','utf8'),context);
const exportsOriginal={};context.module.exports(name=>name.endsWith('/UIManagerNew')?{default:ui}:name.endsWith('/HWL_TStool')?{HWL:ad}:name.endsWith('/SoundManager')?{default:{playSound:n=>sounds.push(n)}}:{default:{}},{},exportsOriginal);
const Game=exportsOriginal.default,game=new Game();Game._instance=game;
const checks=[],ratings=[];const requireTrue=(value,label)=>{assert(value,label);checks.push(label);};
const node=()=>({isValid:true,active:false,scale:1,opacity:255});
game.currentMergeIcon={node:node()};game.currentMergeTimes={node:node()};game.combo=node();game.MergeIcon=['Good','Great','Amazing','Unbelievable'];game.MergeTimes=[2,3,4,5,6,7,8];
game.setMergeComboNearLastMerge=()=>{};game.playMergeComboAppearTween=n=>{n.active=true;};
for(const count of [1,2,3,4,5,6,7,8,9,15]){
 game.currentMergeIcon.node.active=false;game.showMergeComboUI(count);
 if(count<2)requireTrue(!game.currentMergeIcon.node.active,'Original single merge suppresses combo praise');
 else{
  requireTrue(game.currentMergeIcon.spriteFrame===game.MergeIcon[Math.min(count-2,3)]&&game.currentMergeTimes.spriteFrame===Math.min(count,8),'Original rating and multiplier selection for '+count+' merges');
  ratings.push({count,praise:game.currentMergeIcon.spriteFrame,multiplier:game.currentMergeTimes.spriteFrame,audio:sounds.at(-1)});
 }
}
game.guidHand=node();game.node={isValid:true,activeInHierarchy:true,on(){},off(){}};game.updatePendingDeadLineFailCoin=()=>{};game.refreshDeadLineOpacity=()=>{};
game.update(14.99);requireTrue(!game.guidHand.active,'Original hand is hidden before 15 seconds');game.update(.02);requireTrue(game.guidHand.active,'Original hand appears after 15 seconds');
for(const [label,enable,disable] of [
 ['ad',()=>ad.adstart=true,()=>ad.adstart=false],['pending reward',()=>game.pendingRewardPopup=true,()=>game.pendingRewardPopup=false],
 ['pending wheel',()=>game.pendingLuckDrawPopup=true,()=>game.pendingLuckDrawPopup=false],['open UI',()=>ui.all_ui.test={node:node()},()=>ui.all_ui={}],
 ['loading UI',()=>ui.ui_is_loading.test=true,()=>ui.ui_is_loading={}],['game over',()=>game.gameOver=true,()=>game.gameOver=false]]){
 enable();if(label==='open UI')ui.all_ui.test.node.active=true;game.update(20);
 requireTrue(!game.guidHand.active&&game.mainSceneIdleSeconds===0,'Original '+label+' resets and suppresses hand');disable();
}
game.update(15);game.onEnable();game.mainSceneIdleTouchStartHandler();requireTrue(!game.guidHand.active&&game.mainSceneIdleSeconds===0,'Original capture-phase touch-start hides hand');
const configPath=process.argv[2]||root+'/06_UnityFramework/Assets/Config/Runtime/MergeFeedback.asset';const config=fs.readFileSync(configPath,'utf8');
for(const [field,original] of [['chainDelay',.5],['starCount',game.mergeStarCount],['starDelay',game.mergeStarDelayGap],['scatterRadius',game.mergeStarScatterRadius],['flightDuration',game.mergeStarFlyDuration],['targetSpread',game.mergeStarTargetSpread],['comboGapY',game.mergeComboBaseYGap]]){
 const actual=Number(config.match(new RegExp('^  '+field+': (.+)$','m'))[1]);requireTrue(Math.abs(actual-original)<1e-5,'Unity '+field+' equals recovered original value');
}
const source=read(root+'/04_Assets/HotUpdate/SkeletalSource/c80b42c1-1e4d-4ece-a55c-7c6a5616b57c.json');
const animations=Object.keys(source._skeletonJson.animations);
requireTrue(animations.includes('idle1')&&!animations.includes(game.valueToAnimName[20]),'Original requested idel1 differs from actual idle1; prefab default remains the explicit playback source');
const report={passed:true,checks,ratings,handDelay:game.mainSceneIdleGuideDelay,handClip:read(root+'/04_Assets/HotUpdate/AnimationSource/b00460f7-cc4e-43f7-adba-350fc6c6bc97.json'),mergeSkeleton:{uuid:'c80b42c1-1e4d-4ece-a55c-7c6a5616b57c',name:source._name,animations,defaultPlayback:'idle1',requestedNames:game.valueToAnimName}};
fs.writeFileSync(root+'/07_Verification/merge_feedback_source_reference.json',JSON.stringify(report,null,2)+'\n');console.log('Original merge/idle reference checks passed: '+checks.length);
