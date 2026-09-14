// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/base/assets/assets/main/index.ba75b.js
// Module: HonorItem; dependency map: {"../ATools/LocalDataManager":"LocalDataManager","../ATools/TouchButton":"TouchButton","../ATools/UIManager":"UIManager"}
module.exports = function(e, t, o) {
"use strict";
cc._RF.push(t, "0f25en2YsBIZoq2fqgOzq/4", "HonorItem");
var a = this && this.__decorate || function(e, t, o, a) {
var s, n = arguments.length, i = n < 3 ? t : null === a ? a = Object.getOwnPropertyDescriptor(t, o) : a;
if ("object" == typeof Reflect && "function" == typeof Reflect.decorate) i = Reflect.decorate(e, t, o, a); else for (var c = e.length - 1; c >= 0; c--) (s = e[c]) && (i = (n < 3 ? s(i) : n > 3 ? s(t, o, i) : s(t, o)) || i);
return n > 3 && i && Object.defineProperty(t, o, i), i;
};
Object.defineProperty(o, "__esModule", {
value: !0
});
const s = e("../ATools/LocalDataManager"), n = e("../ATools/TouchButton"), i = e("../ATools/UIManager"), {ccclass: c, property: l} = cc._decorator;
let r = class extends cc.Component {
constructor() {
super(...arguments);
this.finish = null;
this.coin = null;
this.check = null;
this.level = 0;
}
onLoad() {
this.node.addComponent(n.default).registerTouchEvent(() => {
if (s.default.getInstance().getGameData().reduceCoins(1e3)) {
s.default.getInstance().getGameData().setAchieveStatus(this.level, 1);
this.flushItem();
} else i.default.instance.onShowToast("Not enough gold coins");
});
}
start() {}
initItem(e) {
this.level = e;
this.flushItem();
}
flushItem() {
if (s.default.getInstance().getGameData().getAchieveStatus(this.level)) {
this.finish.active = !0;
this.check.active = !0;
this.coin.active = !1;
} else {
this.finish.active = !1;
this.check.active = !1;
this.coin.active = !0;
}
}
};
a([ l(cc.Node) ], r.prototype, "finish", void 0);
a([ l(cc.Node) ], r.prototype, "coin", void 0);
a([ l(cc.Node) ], r.prototype, "check", void 0);
r = a([ c ], r);
o.default = r;
cc._RF.pop();
};
