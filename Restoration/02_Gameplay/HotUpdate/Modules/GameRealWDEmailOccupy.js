// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: GameRealWDEmailOccupy; dependency map: {"../BaseUIManager/BaseUI":"BaseUI","../BaseUIManager/Storage/TouchButton":"TouchButton","../LanguageControl/Lab":"Lab"}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "ffeb2BA+UxFqILsGIBcd29J", "GameRealWDEmailOccupy");
var i = this && this.__decorate || function(e, t, a, i) {
var o, n = arguments.length, s = n < 3 ? t : null === i ? i = Object.getOwnPropertyDescriptor(t, a) : i;
if ("object" == typeof Reflect && "function" == typeof Reflect.decorate) s = Reflect.decorate(e, t, a, i); else for (var r = e.length - 1; r >= 0; r--) (o = e[r]) && (s = (n < 3 ? o(s) : n > 3 ? o(t, a, s) : o(t, a)) || s);
return n > 3 && s && Object.defineProperty(t, a, s), s;
};
Object.defineProperty(a, "__esModule", {
value: !0
});
const o = e("../LanguageControl/Lab"), n = e("../BaseUIManager/Storage/TouchButton"), s = e("../BaseUIManager/BaseUI"), {ccclass: r, property: l} = cc._decorator;
let c = class extends s.default {
constructor() {
super(...arguments);
this.btnClose = null;
this.title = null;
this.tip = null;
this.Btn = null;
this.Label = null;
}
onLoad() {
super.onLoad();
this.btnClose.addComponent(n.default).registerTouchEvent(() => {
this.on_close_call();
});
this.Btn.addComponent(n.default).registerTouchEvent(() => {
this.on_close_call();
});
}
show(e) {
super.show(e);
e.param;
this.title.string = o.default.getlab("37");
this.tip.string = o.default.getlab("38");
this.Label.string = o.default.getlab("86");
}
};
i([ l(cc.Node) ], c.prototype, "btnClose", void 0);
i([ l(cc.Label) ], c.prototype, "title", void 0);
i([ l(cc.Label) ], c.prototype, "tip", void 0);
i([ l(cc.Node) ], c.prototype, "Btn", void 0);
i([ l(cc.Label) ], c.prototype, "Label", void 0);
c = i([ s.registerUIPath("GameDialog/GameRealWDEmailOccupy"), r ], c);
a.default = c;
cc._RF.pop();
};
