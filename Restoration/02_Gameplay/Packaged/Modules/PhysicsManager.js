// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/base/assets/assets/main/index.ba75b.js
// Module: PhysicsManager; dependency map: {}
module.exports = function(e, t, o) {
"use strict";
cc._RF.push(t, "6a031eNbvdBKaI+MVYdsCKg", "PhysicsManager");
var a = this && this.__decorate || function(e, t, o, a) {
var s, n = arguments.length, i = n < 3 ? t : null === a ? a = Object.getOwnPropertyDescriptor(t, o) : a;
if ("object" == typeof Reflect && "function" == typeof Reflect.decorate) i = Reflect.decorate(e, t, o, a); else for (var c = e.length - 1; c >= 0; c--) (s = e[c]) && (i = (n < 3 ? s(i) : n > 3 ? s(t, o, i) : s(t, o)) || i);
return n > 3 && i && Object.defineProperty(t, o, i), i;
};
Object.defineProperty(o, "__esModule", {
value: !0
});
const {ccclass: s, property: n} = cc._decorator;
let i = class {
static openPhysics() {
const e = cc.director.getPhysicsManager();
e.enabled = !0;
cc.sys.isBrowser || (e.debugDrawFlags = 0);
cc.director.getPhysicsManager().enabledAccumulator = !0;
cc.PhysicsManager.FIXED_TIME_STEP = 1 / 60;
cc.PhysicsManager.VELOCITY_ITERATIONS = 2;
cc.PhysicsManager.POSITION_ITERATIONS = 2;
cc.director.getCollisionManager().enabled = !0;
}
static closePhysics() {
cc.director.getPhysicsManager().enabled = !1;
cc.director.getCollisionManager().enabled = !1;
}
};
i = a([ s ], i);
o.default = i;
cc._RF.pop();
};
