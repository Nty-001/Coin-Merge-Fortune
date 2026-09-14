// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/base/assets/assets/main/index.ba75b.js
// Module: LevelManager; dependency map: {"../ATools/BaseUI":"BaseUI","../ATools/EventCenter":"EventCenter","../ATools/GameEventConsts":"GameEventConsts","../ATools/LocalDataManager":"LocalDataManager","../ATools/SoundManager":"SoundManager","../ATools/UIManager":"UIManager","./Block":"Block","./BlockUtils":"BlockUtils","./GameMapLayer":"GameMapLayer","./GameMapTouch":"GameMapTouch","./LevelUtils":"LevelUtils"}
module.exports = function(e, t, o) {
"use strict";
cc._RF.push(t, "10598zefMRKSZPB+CzpSGY7", "LevelManager");
var a = this && this.__awaiter || function(e, t, o, a) {
return new (o || (o = Promise))(function(s, n) {
function i(e) {
try {
l(a.next(e));
} catch (e) {
n(e);
}
}
function c(e) {
try {
l(a.throw(e));
} catch (e) {
n(e);
}
}
function l(e) {
e.done ? s(e.value) : (t = e.value, t instanceof o ? t : new o(function(e) {
e(t);
})).then(i, c);
var t;
}
l((a = a.apply(e, t || [])).next());
});
};
Object.defineProperty(o, "__esModule", {
value: !0
});
const s = e("../ATools/BaseUI"), n = e("../ATools/EventCenter"), i = e("../ATools/GameEventConsts"), c = e("../ATools/LocalDataManager"), l = e("../ATools/SoundManager"), r = e("../ATools/UIManager"), h = e("./Block"), u = e("./BlockUtils"), d = e("./GameMapLayer"), g = e("./GameMapTouch"), p = e("./LevelUtils");
class f {
constructor() {
this.mapLayer = null;
this.isNewer = !0;
this.toBigKind = [];
this.toSmallKind = [];
this.curBlockType = h.BlockType.type_min;
this.allBlock = [];
this.stepCount = 0;
this.luckyBagHas = 0;
this.isGameOver = !1;
this.continueEleminateCount = 0;
this.isUsingRemoveBlockProp = !1;
this.frameCount = 0;
this.mergeFlag = !1;
this.eleminateScheduleId = null;
this.lineWorldPos = null;
this.curMode = 1;
this.targetScore = 0;
this.curScore = 0;
cc.director.getScheduler().enableForTarget(this);
n.EventCenter.getInstance().register(i.GameEventName.PassLvChanged, this.onPassLvChangedNtf, this);
n.EventCenter.getInstance().register(i.GameEventName.MergedMaxLvChanged, this.onMergedMaxLvChangedNtf, this);
}
static getInstance() {
f._instance || (f._instance = new f());
return f._instance;
}
onDestroy() {
cc.director.getScheduler().unschedule(this.frameSchedule, this);
cc.director.getScheduler().unschedule(this.autoDropSchedule, this);
n.EventCenter.getInstance().remove(i.GameEventName.PassLvChanged, this);
n.EventCenter.getInstance().remove(i.GameEventName.MergedMaxLvChanged, this);
}
onPassLvChangedNtf() {
this.reInitKind();
}
onMergedMaxLvChangedNtf() {
return a(this, void 0, void 0, function*() {
this.reInitKind();
});
}
autoDropSchedule(e) {
this.tryAutoDrop();
}
tryFailed(e) {
if (this.isGameOver) return;
this.mapLayer && (this.lineWorldPos = this.mapLayer.getLineWorldPos());
let t = 0;
for (let o = this.allBlock.length - 1; o >= 0; o--) {
const a = this.allBlock[o].tryFailed(this.lineWorldPos, e);
a ? this.mapLayer.node_xian.active = a : (t += 1) % 3 == 0 && (this.mapLayer.node_xian.active = a);
}
}
frameSchedule(e) {
this.frameCount++;
this.frameCount % 20 == 0 && this.tryFailed(20);
this.frameCount % 180 == 0 && this.tryAutoMerge();
this.frameCount >= 999 && (this.frameCount = 0);
}
tryAutoDrop() {
d.default.getInstance().isAutoDrop && this.onTouchEnd(cc.v2(cc.winSize.width / 2, cc.winSize.height / 2));
}
tryAutoMerge() {
const e = new Map();
this.allBlock.forEach(t => {
let o = e.get(t.getBlockType());
if (!o) {
o = [];
e.set(t.getBlockType(), o);
}
o.push(t);
});
const t = [ h.BlockType.type_min, h.BlockType.type_2, h.BlockType.type_3, h.BlockType.type_4, h.BlockType.type_5, h.BlockType.type_6, h.BlockType.type_7, h.BlockType.type_8, h.BlockType.type_9, h.BlockType.type_10, h.BlockType.type_11 ];
for (let o = 0; o < t.length; o++) {
const a = e.get(t[o]), s = this.getCanAutoMergeBlock(a);
if (s.length >= 2) {
this.mergeTwoBlock(s[0], s[1]);
return;
}
}
}
getCanAutoMergeBlock(e) {
if (!e) return [];
if (e.length < 2) return [];
const t = {
1: 20,
2: 20,
3: 20,
4: 20,
5: 30,
6: 30,
7: 30,
8: 30,
9: 20,
10: 20,
11: 20
};
for (let o = e.length - 1; o >= 1; o--) {
const a = e[o], s = e[o - 1];
if (a.getMinDistance(s) <= t[a.getBlockType()]) return [ a, s ];
e.splice(o, 1);
}
return e;
}
reInitKind() {
this.toBigKind = [];
this.toSmallKind = [];
const e = c.default.getInstance().getNextBlockItemConfig();
for (let t = 0; t < e.length; t++) {
this.toBigKind.push(Object.assign({}, e[t]));
this.toSmallKind.push(Object.assign({}, e[t]));
}
}
restartEleminateSchedule() {
clearTimeout(this.eleminateScheduleId);
this.eleminateScheduleId = setTimeout(() => {
this.continueEleminateSchedule(0);
}, 800);
}
continueEleminateSchedule(e) {
d.default.getInstance().playBomoSpine(this.continueEleminateCount);
this.continueEleminateCount = 0;
}
isCanTouch() {
return !(this.isGameOver || !this.mapLayer.block_show.active || this.mapLayer.block_show.getNumberOfRunningActions() > 0);
}
tryConvertWorldPos(e) {
if (this.isNewer && this.curBlockType == h.BlockType.type_11) {
e.x = Math.max(e.x, 250);
e.x = Math.min(e.x, 500);
}
return e;
}
onTouchBegin(e) {
d.default.getInstance().stopBlockIdleAction();
if (this.isCanTouch()) {
e = this.tryConvertWorldPos(e);
this.changeBlockShowX(e);
this.changeCirclePointActive(!0);
this.changeCirclePointX(e);
}
}
onTouchMove(e) {
if (this.isCanTouch()) {
e = this.tryConvertWorldPos(e);
this.changeBlockShowX(e);
this.changeCirclePointActive(!0);
this.changeCirclePointX(e);
}
}
onTouchEnd(e) {
if (!this.isCanTouch()) return;
this.stepCount++;
e = this.tryConvertWorldPos(e);
this.mapLayer.block_show.active = !1;
this.changeCirclePointActive(!1);
l.default.playClickSound();
const t = this.mapLayer.block_show.convertToWorldSpaceAR(cc.Vec2.ZERO);
let o = this.createBlock({
type: this.curBlockType,
worldPos: t,
isScale: !1,
fromStorage: !1
});
o.isHavePlayHitEffect = !1;
o.isCanPlayDropMusic = !0;
const a = c.default.getInstance().getGMConfig();
this.resetAllBlocksRigidType();
this.curBlockType = this.genNextType();
d.default.getInstance().delayTime(a.dropGaps).then(() => {
this.showTipsBlock();
});
}
tryAutoChangeX(e) {
if (!d.default.getInstance().isAutoDrop) return;
if (!c.default.getInstance().getGMConfig().smartDrop) return;
const t = this.getAllBlock();
for (let o = t.length - 1; o >= 0; o--) if (t[o].isCanChangeXDrop(e)) {
const a = t[o].node.x;
if (a <= -cc.winSize.width / 2 || a >= cc.winSize.height / 2) break;
e.node.x = a;
break;
}
}
resetAllBlocksRigidType() {
for (let e = 0; e < this.allBlock.length; e++) {
this.allBlock[e].totalTriggerContactNum = 0;
this.allBlock[e].rigidBody.type = cc.RigidBodyType.Dynamic;
}
}
createCirclePointAndChangeY() {}
changeBlockShowX(e) {
this.mapLayer.block_show.x = this.mapLayer.block_show.getParent().convertToNodeSpaceAR(e).x;
}
changeCirclePointActive(e) {}
changeCirclePointX(e) {}
genNextType() {
let e = h.BlockType.type_min;
e = p.default.genNextBlockType(this.toBigKind);
e = Math.max(e, h.BlockType.type_min);
return Math.min(e, h.BlockType.type_max);
}
showTipsBlock() {
this.mapLayer.block_show.active = !0;
this.mapLayer.block_show.children.forEach(e => {
const t = e.name == this.curBlockType.toString();
e.active = t;
});
this.mapLayer.block_show.scale = 0;
cc.tween(this.mapLayer.block_show).to(.15, {
scale: u.default.getOriginScale(this.curBlockType)
}).start();
d.default.getInstance().playBlockIdleAction(this.curBlockType);
}
loadFromStorage() {
d.default.getInstance().delayTime(.01).then(() => {
const e = c.default.getInstance().getGameData().getAllBlockStorage();
for (let t = 0; t < e.length; t++) {
const o = e[t];
o.worldX <= 0 || o.worldX >= cc.winSize.width || o.worldY <= 0 || o.worldY >= cc.winSize.height || (this.createBlock({
type: o.type,
worldPos: cc.v2(o.worldX, o.worldY),
isScale: !1,
fromStorage: !0
}).node.angle = o.angle);
}
});
}
cleanAllBlocks() {
for (let e = this.allBlock.length - 1; e >= 0; e--) this.allBlock[e].destorySelf();
this.allBlock = [];
}
createBlock(e) {
console.log("createBlock: ", e.type, e.worldPos, e.isScale, e.fromStorage);
e.worldPos || (e.worldPos = cc.v2(cc.winSize.width / 2, cc.winSize.height / 2));
(isNaN(e.worldPos.x) || isNaN(e.worldPos.y)) && (e.worldPos = cc.v2(cc.winSize.width / 2, cc.winSize.height / 2));
(e.worldPos.x < 0 || e.worldPos.x > cc.winSize.width || e.worldPos.y < 0 || e.worldPos.y > cc.winSize.height) && (e.worldPos = cc.v2(cc.winSize.width / 2, cc.winSize.height / 2));
const t = this.mapLayer.getBlockParent(), o = t.convertToNodeSpaceAR(e.worldPos), a = u.default.createBlock(t, o, {
type: e.type,
fromStorage: e.fromStorage,
fromPool: !1
});
this.allBlock.push(a);
e.isScale && a.playScaleAction();
return a;
}
createBomb(e, t) {}
createHitEffect(e) {}
initLevelManager(e) {
this.mapLayer = e;
u.default.createAllInitBlocks();
cc.director.getScheduler().unschedule(this.frameSchedule, this);
cc.director.getScheduler().schedule(this.frameSchedule, this, .016);
cc.director.getScheduler().unschedule(this.autoDropSchedule, this);
cc.director.getScheduler().schedule(this.autoDropSchedule, this, .65);
this.mapLayer.node.addComponent(g.default).init({
touchBeginCallback: this.onTouchBegin.bind(this),
touchMoveCallback: this.onTouchMove.bind(this),
touchEndCallback: this.onTouchEnd.bind(this)
});
this.reInitKind();
}
startGame(e = this.curMode, t = !1) {
return a(this, void 0, void 0, function*() {
this.curMode = e;
this.mapLayer.showMode(e);
let o = c.default.getInstance().getGameData().getPassLevel(0);
2 == this.curMode && (o = c.default.getInstance().getGameData().getPassLevel(1));
this.targetScore = 400 * o;
this.curScore = 0;
this.mapLayer.setScore(this.curMode, this.curScore);
this.stepCount = 0;
this.isGameOver = !1;
this.mapLayer.block_show.active = !1;
this.mapLayer.node_xian.active = !1;
this.curBlockType = this.genNextType();
yield d.default.getInstance().delayTime(.01);
this.showTipsBlock();
t || this.cleanAllBlocks();
});
}
createAllObject(e) {
return a(this, void 0, void 0, function*() {
e || this.loadFromStorage();
yield d.default.getInstance().delayTime(.01);
this.createCirclePointAndChangeY();
});
}
mergeTwoBlock(e, t) {
return a(this, void 0, void 0, function*() {
if (!cc.isValid(e) || !cc.isValid(t)) return;
e.canMerge = !1;
t.canMerge = !1;
this.mergeFlag = !0;
this.continueEleminateCount++;
this.restartEleminateSchedule();
const o = e.getBlockType();
l.default.playSound(l.SoundName.merger);
const a = e.node.convertToWorldSpaceAR(cc.Vec2.ZERO), s = t.node.convertToWorldSpaceAR(cc.Vec2.ZERO);
let n = t, i = e, c = a;
if (a.y > s.y) {
n = e;
i = t;
c = s;
}
if (o == h.BlockType.type_11) ; else {
yield n.mergeAndMoveToTargetBlock(i, !0);
this.createBlock({
type: o + 1,
worldPos: c,
isScale: !0,
fromStorage: !1
});
this.addEachMergeReward(o, c);
yield this.onMergeCountLocalReport(o);
}
if (this.mergeFlag) {
this.resetAllBlocksRigidType();
this.mergeFlag = !1;
}
});
}
onMergeCountLocalReport(e) {
return a(this, void 0, void 0, function*() {
e = Math.min(e, h.BlockType.type_max - 1);
yield new Promise(t => {
c.default.getInstance().getGameData().setTotalMergeCount(e, 1, t);
});
});
}
onMergeTwoMax() {
this.isGameOver = !0;
1 == this.curMode ? c.default.getInstance().getGameData().addPassLevel(0) : c.default.getInstance().getGameData().addPassLevel(1);
d.default.getInstance().delayTime(.7).then(() => {
let e = 0;
for (let t = this.allBlock.length - 1; t >= 0; t--) {
this.allBlock[t].getBlockType();
const o = .1 + .025 * t;
this.allBlock[t].playBomb(o);
e < o && (e = o);
}
console.log("SUCCESS");
r.default.instance.onShowView(s.UIConfig.VictoryView);
});
}
addEachMergeReward(e, t) {
this.curScore += 10;
this.mapLayer.setScore(this.curMode, this.curScore);
c.default.getInstance().getGameData().addCoins(10);
}
getCurMaxBlockType() {
let e = h.BlockType.type_min;
for (let t = 0; t < this.allBlock.length; t++) {
let o = Number(this.allBlock[t].getBlockType());
e < o && o != h.BlockType.type_luckyBag && (e = o);
}
return e;
}
removeBlockFromArr(e) {
const t = this.allBlock.indexOf(e);
-1 != t && this.allBlock.splice(t, 1);
}
getAllBlock() {
return this.allBlock;
}
gameOver() {
if (this.isGameOver) return;
this.isGameOver = !0;
this.mapLayer.node_xian.active = !1;
this.mapLayer.block_show.active = !1;
let e = 0;
for (let t = this.allBlock.length - 1; t >= 0; t--) {
this.allBlock[t].getBlockType();
const o = .1 + .025 * t;
this.allBlock[t].playBomb(o);
e < o && (e = o);
}
d.default.getInstance().delayTime(e + .1).then(() => {
r.default.instance.onShowView(s.UIConfig.FailureView);
});
}
findType(e) {
for (let t = 0; t < this.allBlock.length; t++) if (this.allBlock[t].getBlockType() == e) return !0;
return !1;
}
cleanThreeLowLevelBlocks() {
this.curBlockType <= 3 && this.reGenTopBlock();
this.allBlock.forEach((e, t) => {
if (e.getBlockType() <= 3) {
const o = .1 + .025 * t;
e.playBomb(o);
}
});
}
reGenTopBlock() {
this.curBlockType = this.genNextType();
this.showTipsBlock();
}
cleanAllBlocksExcept3() {
this.allBlock.sort((e, t) => e.getBlockType() - t.getBlockType());
let e = this.allBlock.length, t = e > 3 ? e - 3 : 0;
for (let o = e - 1; o > e - 1 - t; o--) this.allBlock[o].playBomb(.1 + .025 * (e - o));
this.reGenTopBlock();
}
getBlock(e) {
let t = [];
this.allBlock.forEach(o => {
o.getBlockType() == e && t.push(o);
});
return t;
}
tttt() {
this.createBlock({
type: 2,
worldPos: cc.v2(0, 0),
isScale: !0,
fromStorage: !1
});
}
}
o.default = f;
cc._RF.pop();
};
