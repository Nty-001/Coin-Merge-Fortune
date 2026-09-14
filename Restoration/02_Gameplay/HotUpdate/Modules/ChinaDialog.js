// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: ChinaDialog; dependency map: {"../BaseUIManager/BaseUI":"BaseUI","../BaseUIManager/Storage/TouchButton":"TouchButton","../LanguageControl/Lab":"Lab"}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "7756cTKRphNKLIiavtyFJ3D", "ChinaDialog");
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
this.title = null;
this.warning = null;
this.btn = null;
this.tips = null;
}
onLoad() {
this.btn.addComponent(n.default).registerTouchEvent(() => {
this.on_close_call();
cc.game.end();
});
}
show(e) {
super.show(e);
this.title.string = s.default.getlab("111");
this.warning.string = s.default.getlab("112");
this.tips.string = s.default.getlab("113");
this.btn.string = s.default.getlab("86");
}
};
i([ l(cc.Label) ], c.prototype, "title", void 0);
i([ l(cc.Label) ], c.prototype, "warning", void 0);
i([ l(cc.Label) ], c.prototype, "btn", void 0);
i([ l(cc.Label) ], c.prototype, "tips", void 0);
c = i([ o.registerUIPath("GameDialog/ChinaDialog"), r ], c);
a.default = c;
cc._RF.pop();
};
