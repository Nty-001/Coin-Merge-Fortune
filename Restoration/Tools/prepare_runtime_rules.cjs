// Build typed Unity configuration using recovered original constants and real cached values.
const fs=require('fs'),path=require('path'),vm=require('vm');
const R=path.resolve(__dirname,'..'),A=R+'/06_UnityFramework/Assets';
const read=p=>JSON.parse(fs.readFileSync(p,'utf8').replace(/^\uFEFF/,''));
function moduleExport(name){
  const context={module:{exports:{}},sp:{Skeleton:class{}},cc:{Component:class{},_RF:{push(){},pop(){}},_decorator:{ccclass:x=>x,property:(...a)=>typeof a[1]==='string'?undefined:()=>{}}}};
  vm.runInNewContext(fs.readFileSync(R+'/02_Gameplay/HotUpdate/Modules/'+name+'.js','utf8'),context,{timeout:5000});
  const exp={};context.module.exports(()=>({default:class{}}),{},exp);return exp;
}
const game=new (moduleExport('GameScene').default)();
const management=moduleExport('GameManagement');
const cached=read(R+'/03_Configuration/EffectiveRuntime/Cached.normalized.json');
const constant=read(R+'/03_Configuration/EffectiveRuntime/HotUpdate.CoinItem.constants.json');
const utils=read(R+'/03_Configuration/EffectiveRuntime/HotUpdate.GameUtils.defaults.json');
const model=read(A+'/Resources/Recovered/HotUpdate_3c55dc23-4711-4586-ae5e-7f0601524baa.json');
const component=model.nodes.flatMap(n=>n.components).find(c=>c.className==='GameScene');
const raw=JSON.parse(component.rawJson),imports=read(A+'/Resources/Recovered/sprite_import.json').sprites;
const spriteMeta=read(R+'/04_Assets/HotUpdate/sprites.json');
const sprites=Array.isArray(spriteMeta)?spriteMeta:spriteMeta.sprites;
const values=Object.keys(constant.instance.upgradeMap).map(Number).sort((a,b)=>a-b);
const C=constant.static,I=constant.instance;
const coins=values.map((value,index)=>{
  const uuid=raw.coinSprites[index].$asset;
  const art=imports.find(s=>s.variant==='HotUpdate'&&s.uuid===uuid);
  const size=sprites.find(s=>s.uuid===uuid).originalSize;
  const extension=path.extname(art.path),resourcePath='Gameplay/Coins/'+value;
  const target=A+'/Resources/'+resourcePath+extension;
  fs.mkdirSync(path.dirname(target),{recursive:true});fs.copyFileSync(R+'/06_UnityFramework/'+art.path,target);
  return {value,upgrade:I.upgradeMap[value],mergeScore:I.score[value]||0,radiusPixels:I.radiusList[value],
    visualWidthPixels:size[0],visualHeightPixels:size[1],dropDamping:I.zuni[value],spritePath:resourcePath};
});
const groups=[];
for(let i=1;i<=6;i++)groups.push({...cached['group'+i],id:'group'+i,
  guideMoney:cached['groupNew'+i].guideMoney,new_Fake_products:cached['groupNew'+i].new_Fake_products});
const rules={evidence:'CoinItem.js and GameScene.js original constructors; device-cached GameData normalized by original GameManagement; no live server substitution.',
 supportedCountries:[...new Set(Object.values(management.LANGUAGE_ID).filter(x=>typeof x==='string'&&x.length===2))],
 drops:Object.entries(cached.level_list).map(([k,v])=>({threshold:Number(k),values:v[0],weights:v[1]})).sort((a,b)=>a.threshold-b.threshold),
 lotteryScores:cached.lotteryScoreConfig,lotteryRewards:cached.lotteryConfig,cashGroups:groups,videoRewards:utils.videoRewardConfig,coins,
 physics:{gravityPixels:-980,mergeSensorExtraRadius:C.MERGE_SENSOR_EXTRA_RADIUS,dropGravityBase:C.DROP_GRAVITY_BASE,
  dropGravityDistanceBonus:C.DROP_GRAVITY_DISTANCE_BONUS,settleGravity:C.SETTLE_GRAVITY_SCALE,dropSpeedBase:C.DROP_SPEED_BASE,
  dropSpeedDistanceBonus:C.DROP_SPEED_DISTANCE_BONUS,dropSpeedFallback:C.DROP_SPEED_FALLBACK,minEffectiveDropDistance:C.MIN_EFFECTIVE_DROP_DISTANCE,
  dropDistanceRange:900,dropDampingMultiplier:C.DROP_DAMPING_MULTIPLIER,minimumDropDamping:.05,settleLinearDamping:C.SETTLE_LINEAR_DAMPING,
  contactSettleDelay:C.DROP_CONTACT_SETTLE_DELAY,angularDamping:C.ANGULAR_DAMPING,density:C.COIN_DENSITY,densityRadiusBase:C.COIN_DENSITY_RADIUS_BASE,
  densityMin:C.COIN_DENSITY_MIN,densityMax:C.COIN_DENSITY_MAX,restitution:C.COIN_RESTITUTION,friction:C.COIN_FRICTION},
 flow:{initialBottomValues:game.initialBottomValues,firstRewardDrop:cached.thirdjumpRewardtimes.first,secondRewardDrop:cached.thirdjumpRewardtimes.second,
  adRewardDrop:cached.thirdjumpRewardtimes.thirdAndlookADDouble,drawRewardStrong:cached.drawRewardStrong,validLoginMergeCount:5,
  saveInterval:game.saveBoardInterval,previewDelay:game.nextPreviewCoinDelay,spawnDuration:game.coinSpawnTweenDuration,spawnInitialScale:game.coinSpawnStartScale,
  spawnUnlockBuffer:game.coinSpawnUnlockDelayBuffer,popupDelay:game.popupDelaySeconds,idleGuideDelay:game.mainSceneIdleGuideDelay,
  failStillVelocity:game.deadLineFailStillVelocityLimit,failStillAngularVelocity:game.deadLineFailStillAngularVelocityLimit,
  failStillDuration:game.deadLineFailStillConfirmDuration,failMaxWait:game.deadLineFailMaxWaitDuration,
  failAnimationDuration:game.failCoinAnimDuration,mergeSpawnLiftPixels:game.mergeCoinSpawnLiftY}};
if(rules.supportedCountries.length<10)throw Error('Missing original language enum');
fs.mkdirSync(A+'/Config/Runtime',{recursive:true});
fs.writeFileSync(A+'/Config/Runtime/RecoveredRules.json',JSON.stringify(rules,null,2));
console.log(JSON.stringify({coins:coins.length,countries:rules.supportedCountries.length,groups:groups.length,dropRows:rules.drops.length}));
