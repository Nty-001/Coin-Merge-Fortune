// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: GameRealWDADIDTips; dependency map: {"../BaseUIManager/BaseUI":"BaseUI","../BaseUIManager/Storage/TouchButton":"TouchButton","../HWL/HWL_TStool":"HWL_TStool","../LanguageControl/Lab":"Lab","../Report/NativeCall":"NativeCall"}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "27c699oFrZEN5WZ4MB3PLhM", "GameRealWDADIDTips");
var i = this && this.__decorate || function(e, t, a, i) {
var o, n = arguments.length, s = n < 3 ? t : null === i ? i = Object.getOwnPropertyDescriptor(t, a) : i;
if ("object" == typeof Reflect && "function" == typeof Reflect.decorate) s = Reflect.decorate(e, t, a, i); else for (var r = e.length - 1; r >= 0; r--) (o = e[r]) && (s = (n < 3 ? o(s) : n > 3 ? o(t, a, s) : o(t, a)) || s);
return n > 3 && s && Object.defineProperty(t, a, s), s;
};
Object.defineProperty(a, "__esModule", {
value: !0
});
const o = e("../BaseUIManager/BaseUI"), n = e("../BaseUIManager/Storage/TouchButton"), s = e("../HWL/HWL_TStool"), r = e("../LanguageControl/Lab"), l = e("../Report/NativeCall"), {ccclass: c, property: d} = cc._decorator;
let h = class extends o.default {
constructor() {
super(...arguments);
this.btnClose = null;
this.title = null;
this.tip = null;
this.adsBtn = null;
this.adsLabel = null;
}
onLoad() {
super.onLoad();
this.btnClose.addComponent(n.default).registerTouchEvent(() => {
this.on_close_call();
});
this.adsBtn.addComponent(n.default).registerTouchEvent(() => {
this.on_close_call();
s.HWLshowAd("adid", () => {
l.default.getAdId();
}, () => {});
});
}
show(e) {
super.show(e);
e.param;
this.title.string = r.default.getlab("37");
this.tip.string = r.default.getlab("52");
this.adsLabel.string = r.default.getlab("90");
}
};
i([ d(cc.Node) ], h.prototype, "btnClose", void 0);
i([ d(cc.Label) ], h.prototype, "title", void 0);
i([ d(cc.Label) ], h.prototype, "tip", void 0);
i([ d(cc.Node) ], h.prototype, "adsBtn", void 0);
i([ d(cc.Label) ], h.prototype, "adsLabel", void 0);
h = i([ o.registerUIPath("GameDialog/GameRealWDADIDTips"), c ], h);
a.default = h;
cc._RF.pop();
};
