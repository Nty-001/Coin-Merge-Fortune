// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/base/assets/assets/main/index.ba75b.js
// Module: ToastView; dependency map: {"../ATools/BaseUI":"BaseUI"}
module.exports = function(e, t, o) {
"use strict";
cc._RF.push(t, "5f656hmSV5AHYoVlEH1lyrI", "ToastView");
var a = this && this.__decorate || function(e, t, o, a) {
var s, n = arguments.length, i = n < 3 ? t : null === a ? a = Object.getOwnPropertyDescriptor(t, o) : a;
if ("object" == typeof Reflect && "function" == typeof Reflect.decorate) i = Reflect.decorate(e, t, o, a); else for (var c = e.length - 1; c >= 0; c--) (s = e[c]) && (i = (n < 3 ? s(i) : n > 3 ? s(t, o, i) : s(t, o)) || i);
return n > 3 && i && Object.defineProperty(t, o, i), i;
};
Object.defineProperty(o, "__esModule", {
value: !0
});
const s = e("../ATools/BaseUI"), {ccclass: n, property: i} = cc._decorator;
let c = class extends s.default {
constructor() {
super(...arguments);
this.bg = null;
this.lbl_txt = null;
}
onLoad() {
this.registerEvent();
}
start() {}
onShow(e) {
this.node.active = !0;
this.showTxt(e);
}
registerEvent() {}
showTxt(e) {
this.lbl_txt.string = e;
this.bg.stopAllActions();
cc.tween(this.bg).set({
y: 0
}).delay(.8).by(.5, {
y: 100
}).call(() => {
this.onClose();
}).start();
}
};
a([ i(cc.Node) ], c.prototype, "bg", void 0);
a([ i(cc.Label) ], c.prototype, "lbl_txt", void 0);
c = a([ s.registerUIPath("prefab/ToastView"), n ], c);
o.default = c;
cc._RF.pop();
};
