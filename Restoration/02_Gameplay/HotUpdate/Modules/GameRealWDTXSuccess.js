// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: GameRealWDTXSuccess; dependency map: {"../BaseUIManager/BaseUI":"BaseUI","../BaseUIManager/Storage/TouchButton":"TouchButton","../LanguageControl/GameManagement":"GameManagement","../LanguageControl/Lab":"Lab"}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "be3f6PF+ftIyrrgEGSn/u3f", "GameRealWDTXSuccess");
var i = this && this.__decorate || function(e, t, a, i) {
var o, n = arguments.length, s = n < 3 ? t : null === i ? i = Object.getOwnPropertyDescriptor(t, a) : i;
if ("object" == typeof Reflect && "function" == typeof Reflect.decorate) s = Reflect.decorate(e, t, a, i); else for (var r = e.length - 1; r >= 0; r--) (o = e[r]) && (s = (n < 3 ? o(s) : n > 3 ? o(t, a, s) : o(t, a)) || s);
return n > 3 && s && Object.defineProperty(t, a, s), s;
};
Object.defineProperty(a, "__esModule", {
value: !0
});
const o = e("../BaseUIManager/BaseUI"), n = e("../LanguageControl/Lab"), s = e("../BaseUIManager/Storage/TouchButton"), r = e("../LanguageControl/GameManagement"), {ccclass: l, property: c} = cc._decorator;
let d = class extends o.default {
constructor() {
super(...arguments);
this.btnClose = null;
this.title = null;
this.coinLabel = null;
this.tip = null;
this.Btn = null;
this.Label = null;
}
onLoad() {
super.onLoad();
this.btnClose.addComponent(s.default).registerTouchEvent(() => {
this.on_close_call();
});
this.Btn.addComponent(s.default).registerTouchEvent(() => {
this.on_close_call();
});
}
show(e) {
super.show(e);
let t = e.param;
console.log("money", t);
this.coinLabel.string = r.default.getRealMonstr(t.money);
this.title.string = n.default.getlab("100");
this.Label.string = n.default.getlab("86");
}
};
i([ c(cc.Node) ], d.prototype, "btnClose", void 0);
i([ c(cc.Label) ], d.prototype, "title", void 0);
i([ c(cc.Label) ], d.prototype, "coinLabel", void 0);
i([ c(cc.Label) ], d.prototype, "tip", void 0);
i([ c(cc.Node) ], d.prototype, "Btn", void 0);
i([ c(cc.Label) ], d.prototype, "Label", void 0);
d = i([ o.registerUIPath("GameDialog/GameRealWDTXSuccess"), l ], d);
a.default = d;
cc._RF.pop();
};
