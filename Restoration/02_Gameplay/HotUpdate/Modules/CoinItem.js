// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: CoinItem; dependency map: {"./BaseUIManager/Storage/GameLocalData":"GameLocalData","./BaseUIManager/Storage/PlayData":"PlayData","./HWL/ServerConfig":"ServerConfig"}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "cdd4chPxjlBea7Xn2092bM9", "CoinItem");
var i, o = this && this.__decorate || function(e, t, a, i) {
var o, n = arguments.length, s = n < 3 ? t : null === i ? i = Object.getOwnPropertyDescriptor(t, a) : i;
if ("object" == typeof Reflect && "function" == typeof Reflect.decorate) s = Reflect.decorate(e, t, a, i); else for (var r = e.length - 1; r >= 0; r--) (o = e[r]) && (s = (n < 3 ? o(s) : n > 3 ? o(t, a, s) : o(t, a)) || s);
return n > 3 && s && Object.defineProperty(t, a, s), s;
};
Object.defineProperty(a, "__esModule", {
value: !0
});
const n = e("./BaseUIManager/Storage/GameLocalData"), s = e("./BaseUIManager/Storage/PlayData"), r = e("./HWL/ServerConfig"), {ccclass: l, property: c} = cc._decorator;
let d = i = class extends cc.Component {
constructor() {
super(...arguments);
this.label = null;
this.sprite = null;
this.coinSprites = [];
this.collisionEffect = null;
this.collisionAnimName = "idle";
this._value = 1;
this._isMerging = !1;
this._isPreview = !1;
this.rigidBody = null;
this.collider = null;
this.mergeSensorCollider = null;
this.isColliding = !1;
this.collisionCount = 0;
this.shouldRelaxDropPhysics = !1;
this.shouldSettleDropPhysics = !1;
this.settleDropContactElapsed = 0;
this.currentDropGravityScale = i.SETTLE_GRAVITY_SCALE;
this.currentDropLinearDamping = i.SETTLE_LINEAR_DAMPING;
this.upgradeMap = {
1: 2,
2: 5,
5: 10,
10: 20,
20: 50,
50: 100,
100: 200,
200: 500,
500: 1e3,
1e3: 2e3,
2e3: 2e3
};
this.radiusList = {
1: 27,
2: 32,
5: 55,
10: 57.5,
20: 71.5,
50: 85,
100: 103,
200: 121,
500: 131,
1e3: 140,
2e3: 192
};
this.offsetList = {
1: [ 0, -1.5 ],
2: [ 0, -1.5 ],
5: [ 0, -2 ],
10: [ 1, -1 ],
20: [ 0, -2.5 ],
50: [ 0, 4 ],
100: [ 0, 0 ],
200: [ 0, -2 ],
500: [ 0, -12 ],
1e3: [ 0, -20 ],
2e3: [ 0, 0 ]
};
this.score = {
2: 1,
5: 1,
10: 2,
20: 3,
50: 4,
100: 5,
200: 6,
500: 7,
1e3: 8,
2e3: 9
};
this.valueToIndex = {
1: 0,
2: 1,
5: 2,
10: 3,
20: 4,
50: 5,
100: 6,
200: 7,
500: 8,
1e3: 9,
2e3: 10
};
this.valueToScale = {
1: .9,
2: .9,
5: 1.1,
10: 1.1,
20: 1.3,
50: 1.3,
100: 2,
200: 2,
500: 2,
1e3: 2,
2e3: 2
};
this.zuni = {
1: .17,
2: .17,
5: .3,
10: .3,
20: .4,
50: .4,
100: .4,
200: .4,
500: .5,
1e3: .5,
2e3: .5
};
}
get newplayData() {
return n.default.getInstance().getData(s.default);
}
get value() {
return this._value;
}
set value(e) {
this._value = e;
this.updateDisplay();
}
get isMerging() {
return this._isMerging;
}
set isMerging(e) {
this._isMerging = e;
}
get isPreview() {
return this._isPreview;
}
set isPreview(e) {
this._isPreview = e;
}
static setMergePausedForGameOver(e) {
i.mergePausedForGameOver = e;
}
static isMergePausedForGameOver() {
return i.mergePausedForGameOver;
}
onLoad() {
this.collisionEffect && (this.collisionEffect.node.active = !1);
}
start() {
this.updateDisplay();
}
update(e) {
this.updateDropPhysicsSettling(e);
}
setupPhysics(e = !0, t = -1) {
this.rigidBody = this.node.getComponent(cc.RigidBody) || this.node.addComponent(cc.RigidBody);
this.rigidBody.enabled = !0;
this.rigidBody.type = cc.RigidBodyType.Dynamic;
let a = "number" == typeof t && !isNaN(t) && t >= 0, o = a ? Math.max(t, i.MIN_EFFECTIVE_DROP_DISTANCE) : 0, n = Math.min(1, o / 900);
this.shouldRelaxDropPhysics = e && a;
this.shouldSettleDropPhysics = !1;
this.settleDropContactElapsed = 0;
this.currentDropGravityScale = e && a ? i.DROP_GRAVITY_BASE + n * i.DROP_GRAVITY_DISTANCE_BONUS : i.SETTLE_GRAVITY_SCALE;
this.currentDropLinearDamping = e && a ? Math.max(.05, this.zuni[this._value] * i.DROP_DAMPING_MULTIPLIER) : i.SETTLE_LINEAR_DAMPING;
this.rigidBody.allowSleep = !0;
this.rigidBody.awake = !0;
this.rigidBody.awakeOnLoad = !0;
this.rigidBody.bullet = e && a;
this.rigidBody.gravityScale = this.currentDropGravityScale;
this.rigidBody.linearDamping = this.currentDropLinearDamping;
this.rigidBody.angularDamping = i.ANGULAR_DAMPING;
if (e) {
let e = a ? i.DROP_SPEED_BASE + n * i.DROP_SPEED_DISTANCE_BONUS : i.DROP_SPEED_FALLBACK;
this.rigidBody.linearVelocity = cc.v2(0, -e);
} else {
this.rigidBody.linearVelocity = cc.v2(0, 0);
this.rigidBody.angularVelocity = 0;
}
this.rigidBody.enabledContactListener = !0;
let s = this.node.getComponents(cc.PhysicsCircleCollider);
this.collider = s.length > 0 ? s[0] : this.node.addComponent(cc.PhysicsCircleCollider);
this.collider.enabled = !0;
this.collider.tag = i.SOLID_COLLIDER_TAG;
this.collider.sensor = !1;
this.collider.radius = this.getSolidColliderRadius();
this.collider.density = this.getCoinDensity(this.collider.radius);
this.collider.restitution = i.COIN_RESTITUTION;
this.collider.friction = i.COIN_FRICTION;
this.collider.apply();
this.mergeSensorCollider = s.length > 1 ? s[1] : this.node.addComponent(cc.PhysicsCircleCollider);
this.mergeSensorCollider.enabled = !0;
this.mergeSensorCollider.tag = i.MERGE_SENSOR_COLLIDER_TAG;
this.mergeSensorCollider.sensor = !0;
this.mergeSensorCollider.radius = this.collider.radius + i.MERGE_SENSOR_EXTRA_RADIUS;
this.mergeSensorCollider.density = 0;
this.mergeSensorCollider.restitution = 0;
this.mergeSensorCollider.friction = 0;
this.mergeSensorCollider.offset = cc.v2(this.collider.offset.x, this.collider.offset.y);
this.mergeSensorCollider.apply();
}
disablePhysics() {
if (!this.node || !this.node.isValid) return;
let e = this.node.getComponents(cc.PhysicsCircleCollider);
for (let t = 0; t < e.length; t++) {
let a = e[t];
if (a && a.isValid) {
a.enabled = !1;
a.destroy();
}
}
let t = this.rigidBody || this.node.getComponent(cc.RigidBody);
if (t && t.isValid) {
t.enabledContactListener = !1;
t.linearVelocity = cc.v2(0, 0);
t.angularVelocity = 0;
t.enabled = !1;
t.destroy();
}
this.collider = null;
this.mergeSensorCollider = null;
this.rigidBody = null;
this.collisionCount = 0;
this.isColliding = !1;
this.shouldRelaxDropPhysics = !1;
this.shouldSettleDropPhysics = !1;
this.settleDropContactElapsed = 0;
}
pauseMergeForGameOver() {
this.unscheduleAllCallbacks();
this._isMerging = !1;
this.isColliding = !1;
this.collisionCount = 0;
this.shouldRelaxDropPhysics = !1;
this.shouldSettleDropPhysics = !1;
this.settleDropContactElapsed = 0;
this.stopCoinTweens();
this.resetSpriteVisualScale();
if (this.collisionEffect && this.collisionEffect.node) {
this.collisionEffect.clearTracks();
cc.Tween.stopAllByTarget(this.collisionEffect.node);
this.collisionEffect.node.active = !1;
this.collisionEffect.node.opacity = 255;
}
}
resumeAfterRevive() {
if (this.node && this.node.isValid) {
this.unscheduleAllCallbacks();
this._isPreview = !1;
this._isMerging = !1;
this.isColliding = !1;
this.collisionCount = 0;
this.shouldRelaxDropPhysics = !1;
this.shouldSettleDropPhysics = !1;
this.settleDropContactElapsed = 0;
this.stopCoinTweens();
this.resetSpriteVisualScale();
this.node.active = !0;
this.node.opacity = 255;
Math.abs(this.node.scaleX) <= .01 && (this.node.scaleX = 1);
Math.abs(this.node.scaleY) <= .01 && (this.node.scaleY = 1);
if (this.collisionEffect && this.collisionEffect.node) {
this.collisionEffect.clearTracks();
cc.Tween.stopAllByTarget(this.collisionEffect.node);
this.collisionEffect.node.active = !1;
this.collisionEffect.node.opacity = 255;
}
this.setupPhysics(!1);
}
}
updateDisplay() {
this.label && (this.label.string = this._value.toString());
if (this.sprite && this.coinSprites.length > 0) {
let e = this.valueToIndex[this._value];
void 0 !== e && this.coinSprites[e] && (this.sprite.spriteFrame = this.coinSprites[e]);
}
}
onBeginContact(e, t, a) {
let o = this.isMergeSensorCollider(t), n = this.isMergeSensorCollider(a), s = o || n;
s && e && (e.disabled = !0);
if (o && n) return;
if (i.mergePausedForGameOver) {
e && (e.disabled = !0);
return;
}
if (!s) {
this.isSideWallCollider(a) || this.relaxDropPhysicsAfterContact();
this.collisionCount++;
this.isColliding || this._isPreview || this._isMerging || 1 == this.collisionCount && this.playCollisionEffect();
}
if (this._isMerging || this._isPreview) {
e && (e.disabled = !0);
return;
}
let r = a.node.getComponent(i);
if (r) if (r.isMerging || r.isPreview) e && (e.disabled = !0); else if (r.value === this._value) {
e && (e.disabled = !0);
this._isMerging = !0;
r.isMerging = !0;
this.scheduleOnce(() => {
if (i.mergePausedForGameOver) {
this._isMerging = !1;
r.node && r.node.isValid && (r.isMerging = !1);
} else this.node && this.node.isValid && r.node && r.node.isValid && this.merge(r);
}, 0);
}
}
onEndContact(e, t, a) {
if (!this.isMergeSensorCollider(t) && !this.isMergeSensorCollider(a)) {
this.collisionCount--;
this.collisionCount <= 0 && (this.collisionCount = 0);
}
}
isMergeSensorCollider(e) {
return !!e && e.tag === i.MERGE_SENSOR_COLLIDER_TAG;
}
isSideWallCollider(e) {
let t = e && e.node;
return !!t && ("WallLeft" === t.name || "WallRight" === t.name || "Wall" === t.name);
}
getSolidColliderRadius() {
let e = this.radiusList[this._value], t = this.getRadius();
return "number" != typeof e || isNaN(e) ? t : Math.min(t, e);
}
getCoinDensity(e) {
let t = Math.max(1, e || i.COIN_DENSITY_RADIUS_BASE), a = i.COIN_DENSITY * i.COIN_DENSITY_RADIUS_BASE / t;
return Math.max(i.COIN_DENSITY_MIN, Math.min(i.COIN_DENSITY_MAX, a));
}
relaxDropPhysicsAfterContact() {
if (this.shouldRelaxDropPhysics) {
this.shouldRelaxDropPhysics = !1;
this.shouldSettleDropPhysics = !0;
this.settleDropContactElapsed = 0;
if (this.rigidBody && this.rigidBody.isValid) {
this.rigidBody.gravityScale = this.currentDropGravityScale;
this.rigidBody.linearDamping = this.currentDropLinearDamping;
this.rigidBody.angularDamping = i.ANGULAR_DAMPING;
this.rigidBody.bullet = !1;
}
}
}
updateDropPhysicsSettling(e) {
if (this.shouldSettleDropPhysics) if (this.rigidBody && this.rigidBody.isValid && this.rigidBody.enabled) if (this._isPreview || this._isMerging || i.mergePausedForGameOver) this.settleDropContactElapsed = 0; else if (this.collisionCount <= 0) this.settleDropContactElapsed = 0; else {
this.settleDropContactElapsed += Math.max(0, e);
if (!(this.settleDropContactElapsed < i.DROP_CONTACT_SETTLE_DELAY)) {
this.shouldSettleDropPhysics = !1;
this.settleDropContactElapsed = 0;
this.rigidBody.gravityScale = i.SETTLE_GRAVITY_SCALE;
this.rigidBody.linearDamping = i.SETTLE_LINEAR_DAMPING;
this.rigidBody.angularDamping = i.ANGULAR_DAMPING;
}
} else {
this.shouldSettleDropPhysics = !1;
this.settleDropContactElapsed = 0;
}
}
stopCoinTweens() {
this.node && this.node.isValid && cc.Tween.stopAllByTarget(this.node);
this.sprite && this.sprite.node && this.sprite.node.isValid && cc.Tween.stopAllByTarget(this.sprite.node);
}
resetSpriteVisualScale() {
if (this.sprite && this.sprite.node && this.sprite.node.isValid) {
this.sprite.node.scale = 1;
this.sprite.node.opacity = 255;
}
}
playCollisionEffect() {
this.isColliding = !0;
if (!this.collisionEffect) return;
let e = this.valueToScale[this._value];
this.collisionEffect.node.scale = e;
this.collisionEffect.node.active = !0;
this.collisionEffect.setAnimation(0, this.collisionAnimName, !1);
this.newplayData.open_vibrate && r.HWLServerConfig.N_vibrate(1, 20, 10, 255);
}
stopCollisionEffect() {
this.isColliding = !1;
if (!this.collisionEffect) return;
this.collisionEffect.clearTracks();
let e = this.collisionEffect.node;
e.opacity = 255;
cc.tween(e).to(.2, {
opacity: 0
}).call(() => {
e.active = !1;
e.opacity = 255;
}).start();
}
merge(e) {
if (i.mergePausedForGameOver) {
this._isMerging = !1;
e && e.node && e.node.isValid && (e.isMerging = !1);
return;
}
let t = this.upgradeMap[this._value], a = this.score[t], o = this.getMergedCoinSpawnData(e), n = this.node.parent;
this.newplayData.open_vibrate && r.HWLServerConfig.N_vibrate(1, 20, 10, 255);
this.stopCollisionEffect();
e.stopCollisionEffect();
this.disablePhysics();
e.disablePhysics();
n && n.emit("create-merged-coin", {
value: t,
position: o.position,
bottomY: o.bottomY,
currentScore: a
});
if (i.mergePausedForGameOver) {
this._isMerging = !1;
e.isMerging = !1;
if (e.node && e.node.isValid) {
e.node.active = !0;
e.node.opacity = 255;
}
if (this.node && this.node.isValid) {
this.node.active = !0;
this.node.opacity = 255;
}
} else {
e.stopCoinTweens();
this.stopCoinTweens();
if (e.node && e.node.isValid) {
e.node.active = !1;
e.node.destroy();
}
if (this.node && this.node.isValid) {
this.node.active = !1;
this.node.destroy();
}
}
}
getMergedCoinSpawnData(e) {
let t = cc.v3(this.node.x, this.node.y, this.node.z), a = cc.v3(e.node.x, e.node.y, e.node.z), i = Math.abs(t.y - a.y), o = Math.min(this.getBottomY(), e.getBottomY());
return i <= 2 ? {
position: cc.v3((t.x + a.x) / 2, (t.y + a.y) / 2, t.z),
bottomY: o
} : {
position: t.y < a.y ? t : a,
bottomY: o
};
}
release(e = 0) {
this._isPreview = !1;
this.setupPhysics(!0, e);
}
getWidth() {
return this.sprite ? this.sprite.node.width : this.node.width;
}
getHeight() {
return this.sprite ? this.sprite.node.height : this.node.height;
}
getRadius() {
return this.getWidth() / 2;
}
getTopY() {
return this.node.y + this.getHeight() / 2;
}
getBottomY() {
return this.node.y - this.getHeight() / 2;
}
};
d.mergePausedForGameOver = !1;
d.SOLID_COLLIDER_TAG = 1;
d.MERGE_SENSOR_COLLIDER_TAG = 2;
d.MERGE_SENSOR_EXTRA_RADIUS = 6;
d.DROP_GRAVITY_BASE = 1.55;
d.DROP_GRAVITY_DISTANCE_BONUS = 1.2;
d.SETTLE_GRAVITY_SCALE = 1.15;
d.DROP_SPEED_BASE = 1150;
d.DROP_SPEED_DISTANCE_BONUS = 1500;
d.DROP_SPEED_FALLBACK = 3600;
d.MIN_EFFECTIVE_DROP_DISTANCE = 180;
d.DROP_DAMPING_MULTIPLIER = .25;
d.SETTLE_LINEAR_DAMPING = 2.45;
d.DROP_CONTACT_SETTLE_DELAY = .35;
d.ANGULAR_DAMPING = 12;
d.COIN_DENSITY = .55;
d.COIN_DENSITY_RADIUS_BASE = 55;
d.COIN_DENSITY_MIN = .18;
d.COIN_DENSITY_MAX = .75;
d.COIN_RESTITUTION = .25;
d.COIN_FRICTION = .4;
o([ c(cc.Label) ], d.prototype, "label", void 0);
o([ c(cc.Sprite) ], d.prototype, "sprite", void 0);
o([ c([ cc.SpriteFrame ]) ], d.prototype, "coinSprites", void 0);
o([ c(sp.Skeleton) ], d.prototype, "collisionEffect", void 0);
o([ c ], d.prototype, "collisionAnimName", void 0);
d = i = o([ l ], d);
a.default = d;
cc._RF.pop();
};
