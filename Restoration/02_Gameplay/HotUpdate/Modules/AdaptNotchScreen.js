// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: AdaptNotchScreen; dependency map: {}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "b263fmlKzNLN7Xsg2QNBAEG", "AdaptNotchScreen");
var i = this && this.__decorate || function(e, t, a, i) {
var o, n = arguments.length, s = n < 3 ? t : null === i ? i = Object.getOwnPropertyDescriptor(t, a) : i;
if ("object" == typeof Reflect && "function" == typeof Reflect.decorate) s = Reflect.decorate(e, t, a, i); else for (var r = e.length - 1; r >= 0; r--) (o = e[r]) && (s = (n < 3 ? o(s) : n > 3 ? o(t, a, s) : o(t, a)) || s);
return n > 3 && s && Object.defineProperty(t, a, s), s;
};
Object.defineProperty(a, "__esModule", {
value: !0
});
const {ccclass: o, property: n} = cc._decorator;
let s = class extends cc.Component {
constructor() {
super(...arguments);
this.widgerTop = 0;
this.safeareaOffset = 0;
}
start() {
(this.node.getComponent(cc.Widget) || this.node.addComponent(cc.Widget)).top = this.widgerTop;
this.adaptNotchScreen();
}
adaptNotchScreen() {
if (cc.sys.getSafeAreaRect) try {
const e = cc.sys.getSafeAreaRect();
if (!e) return;
const t = this.node.getComponent(cc.Widget);
if (!t) return;
const a = cc.view.getVisibleSize(), i = Math.max(0, a.height - e.yMax), o = Math.max(0, e.yMin);
t.top = this.widgerTop + i;
t.bottom = o;
t.updateAlignment();
} catch (e) {
cc.warn("获取安全区域失败:", e);
}
}
adaptNotchScreen1() {
if (cc.sys.getSafeAreaRect) try {
const e = cc.sys.getSafeAreaRect();
if (!e) return;
const t = this.node.getComponent(cc.Widget);
if (!t) return;
const a = cc.view.getVisibleSize(), i = Math.max(0, a.height - e.yMax), o = Math.max(0, e.yMin);
t.top = this.widgerTop + i;
t.bottom = o;
t.updateAlignment();
} catch (e) {
cc.warn("获取安全区域失败:", e);
}
}
};
i([ n ], s.prototype, "widgerTop", void 0);
s = i([ o ], s);
a.default = s;
cc._RF.pop();
};
