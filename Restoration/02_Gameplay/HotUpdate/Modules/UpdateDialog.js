// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: UpdateDialog; dependency map: {"../BaseUIManager/BaseUI":"BaseUI","../BaseUIManager/Storage/TouchButton":"TouchButton","../LanguageControl/Lab":"Lab","../Report/NativeCall":"NativeCall"}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "93169cRaBBO2Jmn/2NmnozC", "UpdateDialog");
var i = this && this.__decorate || function(e, t, a, i) {
var o, n = arguments.length, s = n < 3 ? t : null === i ? i = Object.getOwnPropertyDescriptor(t, a) : i;
if ("object" == typeof Reflect && "function" == typeof Reflect.decorate) s = Reflect.decorate(e, t, a, i); else for (var r = e.length - 1; r >= 0; r--) (o = e[r]) && (s = (n < 3 ? o(s) : n > 3 ? o(t, a, s) : o(t, a)) || s);
return n > 3 && s && Object.defineProperty(t, a, s), s;
};
Object.defineProperty(a, "__esModule", {
value: !0
});
const o = e("../BaseUIManager/BaseUI"), n = e("../BaseUIManager/Storage/TouchButton"), s = e("../LanguageControl/Lab"), r = e("../Report/NativeCall"), {ccclass: l, property: c} = cc._decorator;
let d = class extends o.default {
constructor() {
super(...arguments);
this.title = null;
this.closeBrn = null;
this.btn = null;
this.tips1 = null;
this.continueLoadingCallback = null;
}
onLoad() {
this.closeBrn.addComponent(n.default).registerTouchEvent(() => {
let e = this.continueLoadingCallback;
this.continueLoadingCallback = null;
this.on_close_call();
e && e();
});
this.btn.addComponent(n.default).registerTouchEvent(() => {
r.default.gotoMarket();
});
}
show(e) {
super.show(e);
let t = e.param || {};
this.continueLoadingCallback = t.forceUpdate ? null : t.continueLoadingCallback;
this.title.string = s.default.getlab("39");
this.btn.string = s.default.getlab("40");
this.closeBrn.active = !t.forceUpdate;
1 == t.forceUpdate ? this.tips1.string = s.default.getlab("41") : this.tips1.string = s.default.getlab("42");
}
};
i([ c(cc.Label) ], d.prototype, "title", void 0);
i([ c(cc.Node) ], d.prototype, "closeBrn", void 0);
i([ c(cc.Label) ], d.prototype, "btn", void 0);
i([ c(cc.Label) ], d.prototype, "tips1", void 0);
d = i([ o.registerUIPath("GameDialog/UpdateDialog"), l ], d);
a.default = d;
cc._RF.pop();
};
