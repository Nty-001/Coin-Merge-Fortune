// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/base/assets/assets/main/index.ba75b.js
// Module: PolicyView; dependency map: {"../ATools/BaseUI":"BaseUI","../ATools/ImagesPanel":"ImagesPanel","../ATools/TouchButton":"TouchButton"}
module.exports = function(e, t, o) {
"use strict";
cc._RF.push(t, "4d666NjoM9JP4EQnlwcjJOH", "PolicyView");
var a = this && this.__decorate || function(e, t, o, a) {
var s, n = arguments.length, i = n < 3 ? t : null === a ? a = Object.getOwnPropertyDescriptor(t, o) : a;
if ("object" == typeof Reflect && "function" == typeof Reflect.decorate) i = Reflect.decorate(e, t, o, a); else for (var c = e.length - 1; c >= 0; c--) (s = e[c]) && (i = (n < 3 ? s(i) : n > 3 ? s(t, o, i) : s(t, o)) || i);
return n > 3 && i && Object.defineProperty(t, o, i), i;
};
Object.defineProperty(o, "__esModule", {
value: !0
});
const s = e("../ATools/BaseUI"), n = e("../ATools/ImagesPanel"), i = e("../ATools/TouchButton"), {ccclass: c, property: l} = cc._decorator;
let r = class extends s.default {
constructor() {
super(...arguments);
this.btn_close = null;
this.lbl_heart = null;
this.policy = null;
}
onLoad() {
this.registerEvent();
}
start() {}
onShow(e) {
this.lbl_heart.string = 1 === e.type ? "Privacy Policy" : "User Agreement";
this.policy.value = 1 === e.type ? 0 : 1;
}
registerEvent() {
this.btn_close.addComponent(i.default).registerTouchEvent(() => {
this.onDestroyView();
});
}
};
a([ l(cc.Node) ], r.prototype, "btn_close", void 0);
a([ l(cc.Label) ], r.prototype, "lbl_heart", void 0);
a([ l(n.default) ], r.prototype, "policy", void 0);
r = a([ s.registerUIPath("prefab/PolicyView"), c ], r);
o.default = r;
cc._RF.pop();
};
