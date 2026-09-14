// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: PrivacyPolicyView; dependency map: {"../BaseUIManager/BaseUI":"BaseUI","../BaseUIManager/Storage/TouchButton":"TouchButton","../LanguageControl/Lab":"Lab"}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "4aae9XNfglH+IBktNP67JiA", "PrivacyPolicyView");
var i = this && this.__decorate || function(e, t, a, i) {
var o, n = arguments.length, s = n < 3 ? t : null === i ? i = Object.getOwnPropertyDescriptor(t, a) : i;
if ("object" == typeof Reflect && "function" == typeof Reflect.decorate) s = Reflect.decorate(e, t, a, i); else for (var r = e.length - 1; r >= 0; r--) (o = e[r]) && (s = (n < 3 ? o(s) : n > 3 ? o(t, a, s) : o(t, a)) || s);
return n > 3 && s && Object.defineProperty(t, a, s), s;
};
Object.defineProperty(a, "__esModule", {
value: !0
});
const o = e("../BaseUIManager/BaseUI"), n = e("../BaseUIManager/Storage/TouchButton"), s = e("../LanguageControl/Lab"), {ccclass: r, property: l} = cc._decorator;
let c = class extends o.default {
constructor() {
super(...arguments);
this.btnClose = null;
this.contentObj = null;
this.titleLabel = null;
this.scrollview1 = null;
this.scrollview2 = null;
}
onLoad() {
this.btnClose.addComponent(n.default).registerTouchEvent(() => {
this.on_close_call();
});
}
show(e) {
super.show(e);
let t = e.param, a = 1 == t.icon_idx ? s.default.getlab("6") : s.default.getlab("7");
this.scrollview1.active = !(1 == t.icon_idx);
this.scrollview2.active = 1 == t.icon_idx;
this.titleLabel.string = a;
}
};
i([ l(cc.Node) ], c.prototype, "btnClose", void 0);
i([ l(cc.Node) ], c.prototype, "contentObj", void 0);
i([ l(cc.Label) ], c.prototype, "titleLabel", void 0);
i([ l(cc.Node) ], c.prototype, "scrollview1", void 0);
i([ l(cc.Node) ], c.prototype, "scrollview2", void 0);
c = i([ o.registerUIPath("GameDialog/PrivacyPolicyView"), r ], c);
a.default = c;
cc._RF.pop();
};
