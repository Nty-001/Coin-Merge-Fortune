// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: mergeRuleDialog; dependency map: {"../BaseUIManager/BaseUI":"BaseUI","../BaseUIManager/Storage/TouchButton":"TouchButton","../LanguageControl/Lab":"Lab"}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "7a2fcJdLwFDHrK53O8wMUcC", "mergeRuleDialog");
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
this.bgSpine = null;
this.btnClose = null;
this.btnConfirm = null;
this.title = null;
this.des = null;
this.title2 = null;
this.btnLabel = null;
}
start() {}
onLoad() {
super.onLoad();
this.btnClose.addComponent(n.default).registerTouchEvent(() => {
this.on_close_call();
});
this.btnConfirm.addComponent(n.default).registerTouchEvent(() => {
this.on_close_call();
});
}
show(e) {
super.show(e);
this.bgSpine.setAnimation(0, "animation", !1);
this.title.string = s.default.getlab("1");
this.des.string = s.default.getlab("2");
this.title2.string = s.default.getlab("28");
this.btnLabel.string = s.default.getlab("92");
}
};
i([ l(sp.Skeleton) ], c.prototype, "bgSpine", void 0);
i([ l(cc.Node) ], c.prototype, "btnClose", void 0);
i([ l(cc.Node) ], c.prototype, "btnConfirm", void 0);
i([ l(cc.Label) ], c.prototype, "title", void 0);
i([ l(cc.Label) ], c.prototype, "des", void 0);
i([ l(cc.Label) ], c.prototype, "title2", void 0);
i([ l(cc.Label) ], c.prototype, "btnLabel", void 0);
c = i([ o.registerUIPath("GameDialog/mergeRuleDialog"), r ], c);
a.default = c;
cc._RF.pop();
};
