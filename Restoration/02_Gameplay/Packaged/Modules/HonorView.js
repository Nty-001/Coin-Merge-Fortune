// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/base/assets/assets/main/index.ba75b.js
// Module: HonorView; dependency map: {"../ATools/BaseUI":"BaseUI","../ATools/TouchButton":"TouchButton","./HonorItem":"HonorItem"}
module.exports = function(e, t, o) {
"use strict";
cc._RF.push(t, "dd561EEDP1GwqDmL4q+ZZB0", "HonorView");
var a = this && this.__decorate || function(e, t, o, a) {
var s, n = arguments.length, i = n < 3 ? t : null === a ? a = Object.getOwnPropertyDescriptor(t, o) : a;
if ("object" == typeof Reflect && "function" == typeof Reflect.decorate) i = Reflect.decorate(e, t, o, a); else for (var c = e.length - 1; c >= 0; c--) (s = e[c]) && (i = (n < 3 ? s(i) : n > 3 ? s(t, o, i) : s(t, o)) || i);
return n > 3 && i && Object.defineProperty(t, o, i), i;
};
Object.defineProperty(o, "__esModule", {
value: !0
});
const s = e("../ATools/BaseUI"), n = e("../ATools/TouchButton"), i = e("./HonorItem"), {ccclass: c, property: l} = cc._decorator;
let r = class extends s.default {
constructor() {
super(...arguments);
this.items = [];
this.btn_close = null;
}
onLoad() {
this.registerEvent();
}
start() {}
onShow(e) {
this.initItem();
}
registerEvent() {
this.btn_close.addComponent(n.default).registerTouchEvent(() => {
this.onClose();
});
}
initItem() {
this.items.forEach((e, t) => {
e.initItem(t);
});
}
};
a([ l(i.default) ], r.prototype, "items", void 0);
a([ l(cc.Node) ], r.prototype, "btn_close", void 0);
r = a([ s.registerUIPath("prefab/HonorView"), c ], r);
o.default = r;
cc._RF.pop();
};
