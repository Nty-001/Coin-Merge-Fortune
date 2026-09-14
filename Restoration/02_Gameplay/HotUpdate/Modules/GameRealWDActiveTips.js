// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: GameRealWDActiveTips; dependency map: {"../BaseUIManager/BaseUI":"BaseUI","../BaseUIManager/Storage/TouchButton":"TouchButton","../LanguageControl/Lab":"Lab"}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "ef5f7YVU0FAZYsPHrQuLxN0", "GameRealWDActiveTips");
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
this.title = null;
this.tip = null;
this.btn = null;
this.btnLabel = null;
this.bottomtips = null;
this.progress = 15;
}
onLoad() {
super.onLoad();
this.btnClose.addComponent(n.default).registerTouchEvent(() => {
this.on_close_call();
});
this.btn.addComponent(n.default).registerTouchEvent(() => {
this.on_close_call();
});
}
show(e) {
super.show(e);
this.title.string = s.default.getlab("15");
this.btnLabel.string = s.default.getlab("92");
this.bottomtips.string = s.default.getlab("27");
}
updateTime() {
this.progress -= 1;
this.progress <= 0 && this.unschedule(this.updateTime);
let e = s.default.getlab("110", this.progress);
this.tip.string = e;
}
};
i([ l(cc.Node) ], c.prototype, "btnClose", void 0);
i([ l(cc.Label) ], c.prototype, "title", void 0);
i([ l(cc.Label) ], c.prototype, "tip", void 0);
i([ l(cc.Node) ], c.prototype, "btn", void 0);
i([ l(cc.Label) ], c.prototype, "btnLabel", void 0);
i([ l(cc.Label) ], c.prototype, "bottomtips", void 0);
c = i([ o.registerUIPath("GameDialog/GameRealWDActiveTips"), r ], c);
a.default = c;
cc._RF.pop();
};
