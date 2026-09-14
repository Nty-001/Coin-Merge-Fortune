// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/base/assets/assets/main/index.ba75b.js
// Module: Block; dependency map: {"../ATools/SoundManager":"SoundManager","./BlockUtils":"BlockUtils","./LevelManager":"LevelManager","./PoolManager":"PoolManager","./Wall":"Wall"}
module.exports = function(e, t, o) {
"use strict";
cc._RF.push(t, "4bdd29QO2lBM5i94uj0TE4V", "Block");
var a, s = this && this.__decorate || function(e, t, o, a) {
var s, n = arguments.length, i = n < 3 ? t : null === a ? a = Object.getOwnPropertyDescriptor(t, o) : a;
if ("object" == typeof Reflect && "function" == typeof Reflect.decorate) i = Reflect.decorate(e, t, o, a); else for (var c = e.length - 1; c >= 0; c--) (s = e[c]) && (i = (n < 3 ? s(i) : n > 3 ? s(t, o, i) : s(t, o)) || i);
return n > 3 && i && Object.defineProperty(t, o, i), i;
}, n = this && this.__awaiter || function(e, t, o, a) {
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
o.BlockType = void 0;
const i = e("./BlockUtils"), c = e("./PoolManager"), l = e("./LevelManager"), r = e("./Wall"), h = e("../ATools/SoundManager");
var u;
(function(e) {
e[e.type_min = 1] = "type_min";
e[e.type_2 = 2] = "type_2";
e[e.type_3 = 3] = "type_3";
e[e.type_4 = 4] = "type_4";
e[e.type_5 = 5] = "type_5";
e[e.type_6 = 6] = "type_6";
e[e.type_7 = 7] = "type_7";
e[e.type_8 = 8] = "type_8";
e[e.type_9 = 9] = "type_9";
e[e.type_10 = 10] = "type_10";
e[e.type_11 = 11] = "type_11";
e[e.type_max = 12] = "type_max";
e[e.type_luckyBag = 100] = "type_luckyBag";
})(u = o.BlockType || (o.BlockType = {}));
const {ccclass: d, property: g} = cc._decorator;
let p = a = class extends cc.Component {
constructor() {
super(...arguments);
this.sprite_block = null;
this.rigidBody = null;
this.physicsCollider = null;
this.canMerge = !0;
this.overFrameCount = 0;
this.isCanPlayDropMusic = !1;
this.totalTriggerContactNum = 0;
this.animatingState = !1;
this.isHavePlayHitEffect = !0;
this.viewData = null;
}
onLoad() {
this.node.getComponent(cc.Sprite) && (this.node.getComponent(cc.Sprite).enabled = !1);
}
initRigidBodyAndCollider() {
this.rigidBody = this.node.getComponent(cc.RigidBody);
this.sprite_block = this.node.children[0].getComponent(cc.Sprite);
this.physicsCollider = this.node.getComponent(cc.PhysicsCollider);
this.rigidBody.bullet = !1;
this.rigidBody.type = cc.RigidBodyType.Dynamic;
this.rigidBody.gravityScale = 80;
this.rigidBody.linearDamping = 0;
this.physicsCollider.friction = 1;
this.physicsCollider.restitution = .2;
}
onEnable() {}
onDisable() {}
tryFailed(e, t) {
if (!e) {
this.overFrameCount = 0;
return !1;
}
if (l.default.getInstance().isGameOver) {
this.overFrameCount = 0;
return !1;
}
const o = this.node.convertToWorldSpaceAR(cc.Vec2.ZERO).y + this.node.height * i.default.getOriginScale(this.viewData.type) / 2;
this.node.height, i.default.getOriginScale(this.viewData.type);
if (o < e.y) {
this.overFrameCount = 0;
return !1;
}
this.overFrameCount += t;
if (this.overFrameCount > 300) {
this.rigidBody.type = cc.RigidBodyType.Static;
this.destorySelf();
l.default.getInstance().gameOver();
}
return !0;
}
isCanMerge(e) {
return this.viewData.type == e.viewData.type && this.canMerge && e.canMerge;
}
onBeginContact(e, t, o) {
if (l.default.getInstance().isGameOver) return;
if (this.animatingState) return;
const a = this.getOtherData(o);
if ("Block" != a.type || !a.block.animatingState) {
this.totalTriggerContactNum++;
if (this.totalTriggerContactNum > 3) {
this.physicsCollider.restitution = 0;
this.rigidBody.linearDamping = .5;
this.rigidBody.gravityScale = 4;
}
switch (a.type) {
case "Block":
if (this.isCanMerge(a.block)) l.default.getInstance().mergeTwoBlock(this, a.block); else {
this.isCanPlayDropMusic && h.default.playSound("floor");
this.isCanPlayDropMusic = !1;
if (!this.isHavePlayHitEffect) {
l.default.getInstance().createHitEffect(e.getWorldManifold().points[0]);
this.isHavePlayHitEffect = !0;
}
}
break;

case "Wall":
if (a.wall.getWallType() == r.WallType.Bottom) {
this.isCanPlayDropMusic && h.default.playSound("floor");
this.isCanPlayDropMusic = !1;
this.isHavePlayHitEffect = !0;
}
}
}
}
getOtherData(e) {
const t = e.node.getComponent(a);
if (t) return {
type: "Block",
block: t
};
const o = e.node.getComponent(r.default);
return o ? {
type: "Wall",
wall: o
} : {
type: "Unknow"
};
}
destorySelf() {
l.default.getInstance().removeBlockFromArr(this);
this.recycleItem();
}
recycleItem() {
if (cc.sys.isNative && cc.sys.os == cc.sys.OS_IOS) this.node.destroy(); else {
this.sprite_block.node.active = !0;
this.node.stopAllActions();
this.node.scale = i.default.getOriginScale(this.viewData.type);
this.node.angle = 0;
this.canMerge = !0;
this.overFrameCount = 0;
this.isCanPlayDropMusic = !0;
this.totalTriggerContactNum = 0;
this.animatingState = !1;
this.isHavePlayHitEffect = !1;
this.rigidBody.active = !0;
this.viewData = null;
"Block" == this.node.name ? c.poolManager.putNode(this.node) : this.node.destroy();
}
}
init(e) {
this.viewData = e;
this.canMerge = !0;
this.overFrameCount = 0;
this.node.name = "Block";
switch (this.viewData.type) {
case u.type_luckyBag:
this.node.name = "LuckyBag";
this.canMerge = !1;
}
this.initRigidBodyAndCollider();
this.addBlockEffect();
cc.sys.isBrowser && e.fromPool;
}
addBlockEffect() {}
playMergeAni() {
this.sprite_block.node.active = !1;
}
playBornAni() {
this.sprite_block.node.active = !0;
}
getBlockType() {
return this.viewData.type;
}
playScaleAction() {
this.node.scale = 0 * i.default.getOriginScale(this.viewData.type);
this.animatingState = !0;
cc.tween(this.node).to(.1, {
scale: i.default.getOriginScale(this.viewData.type)
}).call(() => {
this.animatingState = !1;
}).to(.05, {
scale: 1.2 * i.default.getOriginScale(this.viewData.type)
}).to(.05, {
scale: i.default.getOriginScale(this.viewData.type)
}).start();
}
playBomb(e) {
cc.tween(this.node).delay(e).call(() => {
const e = this.node.convertToWorldSpaceAR(cc.Vec2.ZERO);
l.default.getInstance().createBomb(this.viewData.type, e);
h.default.playSound("disappear");
this.destorySelf();
}).start();
}
getWorldPoint() {
return this.physicsCollider.points.map(e => this.node.convertToWorldSpaceAR(e));
}
getRadius() {
return this.physicsCollider.radius;
}
mergeAndMoveToTargetBlock(e, t) {
return n(this, void 0, void 0, function*() {
yield new Promise(e => {
this.scheduleOnce(e, .01);
});
e.animatingState = !0;
this.playMergeAni();
e.playMergeAni();
e.rigidBody.type = cc.RigidBodyType.Static;
this.rigidBody.active = !1;
cc.Tween.stopAllByTarget(this.node);
t && (yield new Promise(t => {
cc.tween(this.node).to(.1, {
position: e.node.position
}).call(() => {
t();
}).start();
}));
this.destorySelf();
e.destorySelf();
});
}
calculateMinDistance(e, t) {
let o = Number.MAX_SAFE_INTEGER;
for (let a = 0; a < e.length; a++) for (let a = 0; a < t.length; a++) {
const s = cc.Vec2.distance(e[a], t[a]);
s < o && (o = s);
}
return o;
}
getMinDistance(e) {
return e && this.isCanMerge(e) ? this.calculateMinDistance(this.getWorldPoint(), e.getWorldPoint()) : Number.MAX_SAFE_INTEGER;
}
isCanChangeXDrop(e) {
if (e.getBlockType() == u.type_luckyBag) return !1;
if (this == e) return !1;
if (this.getBlockType() != e.getBlockType()) return !1;
const t = this.getWorldPoint(), o = e.node.convertToWorldSpaceAR(cc.Vec2.ZERO).y;
let a = 0;
t.forEach(e => {
a / t.length > .02 + .1 || cc.director.getPhysicsManager().rayCast(e, cc.v2(e.x, o), cc.RayCastType.Any).length <= 0 && a++;
});
return a / t.length >= .1;
}
};
p = a = s([ d ], p);
o.default = p;
cc._RF.pop();
};
