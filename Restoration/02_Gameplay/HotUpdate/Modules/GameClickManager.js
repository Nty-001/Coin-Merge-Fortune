// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: GameClickManager; dependency map: {"../BaseUIManager/Storage/ResManager":"ResManager","../BaseUIManager/Storage/Singleton":"Singleton","./GameClickSpine":"GameClickSpine"}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "67fb3/BdUtI5Il0EwYPjjXP", "GameClickManager");
var i = this && this.__awaiter || function(e, t, a, i) {
return new (a || (a = Promise))(function(o, n) {
function s(e) {
try {
l(i.next(e));
} catch (e) {
n(e);
}
}
function r(e) {
try {
l(i.throw(e));
} catch (e) {
n(e);
}
}
function l(e) {
e.done ? o(e.value) : (t = e.value, t instanceof a ? t : new a(function(e) {
e(t);
})).then(s, r);
var t;
}
l((i = i.apply(e, t || [])).next());
});
};
Object.defineProperty(a, "__esModule", {
value: !0
});
a.GameClickManager = void 0;
const o = e("../BaseUIManager/Storage/ResManager"), n = e("../BaseUIManager/Storage/Singleton"), s = e("./GameClickSpine");
a.GameClickManager = class extends n.default {
constructor() {
super(...arguments);
this.parentNode = null;
this.clickSpineNodePool = new cc.NodePool();
this.lastCreateTime = 0;
}
init(e) {
this.parentNode = e;
}
createClickSpine(e) {
return i(this, void 0, void 0, function*() {
if (!this.parentNode) return;
let t = yield this.getClickSpineNodeFromNodePool();
cc.isValid(t.parent) || t.setParent(this.parentNode);
let a = this.parentNode.convertToNodeSpaceAR(e);
t.setPosition(a);
t.getComponent(s.default).playSpine();
});
}
getClickSpineNodeFromNodePool() {
return i(this, void 0, void 0, function*() {
if (!this.parentNode) return null;
if (this.clickSpineNodePool.size() > 0) return this.clickSpineNodePool.get();
{
const e = "GameClickSpine";
try {
const t = yield o.default.loadResByPromise(e, cc.Prefab);
if (t) {
const e = cc.instantiate(t);
this.parentNode.addChild(e);
return e;
}
} catch (t) {
console.warn(`Failed to load prefab from ${e}, using createObject method`, t);
}
}
});
}
recyclePerson(e) {
this.clickSpineNodePool.put(e.node);
}
};
cc._RF.pop();
};
