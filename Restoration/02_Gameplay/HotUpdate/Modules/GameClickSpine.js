// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: GameClickSpine; dependency map: {"../BaseUIManager/UIUtils":"UIUtils","./GameClickManager":"GameClickManager"}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "28f1cmQSUJHQp6fgSDTaANH", "GameClickSpine");
var i = this && this.__decorate || function(e, t, a, i) {
var o, n = arguments.length, s = n < 3 ? t : null === i ? i = Object.getOwnPropertyDescriptor(t, a) : i;
if ("object" == typeof Reflect && "function" == typeof Reflect.decorate) s = Reflect.decorate(e, t, a, i); else for (var r = e.length - 1; r >= 0; r--) (o = e[r]) && (s = (n < 3 ? o(s) : n > 3 ? o(t, a, s) : o(t, a)) || s);
return n > 3 && s && Object.defineProperty(t, a, s), s;
};
Object.defineProperty(a, "__esModule", {
value: !0
});
const o = e("../BaseUIManager/UIUtils"), n = e("./GameClickManager"), {ccclass: s, property: r} = cc._decorator;
let l = class extends cc.Component {
constructor() {
super(...arguments);
this.spine_click = null;
}
playSpine() {
let e = this;
o.default.getInstance().playSpine(this.spine_click, "idle", !1, () => {
n.GameClickManager.getInstance().recyclePerson(e);
});
}
};
i([ r(sp.Skeleton) ], l.prototype, "spine_click", void 0);
l = i([ s ], l);
a.default = l;
cc._RF.pop();
};
