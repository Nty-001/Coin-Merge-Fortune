// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: GameWithdrawItem; dependency map: {"../LanguageControl/GameManagement":"GameManagement"}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "4ec23dGqBZBAKQq6cEsBH5q", "GameWithdrawItem");
var i = this && this.__decorate || function(e, t, a, i) {
var o, n = arguments.length, s = n < 3 ? t : null === i ? i = Object.getOwnPropertyDescriptor(t, a) : i;
if ("object" == typeof Reflect && "function" == typeof Reflect.decorate) s = Reflect.decorate(e, t, a, i); else for (var r = e.length - 1; r >= 0; r--) (o = e[r]) && (s = (n < 3 ? o(s) : n > 3 ? o(t, a, s) : o(t, a)) || s);
return n > 3 && s && Object.defineProperty(t, a, s), s;
};
Object.defineProperty(a, "__esModule", {
value: !0
});
const o = e("../LanguageControl/GameManagement"), {ccclass: n, property: s} = cc._decorator;
let r = class extends cc.Component {
constructor() {
super(...arguments);
this.coinLabel1 = null;
this.coinLabel2 = null;
this.coinLabel3 = null;
this.itemSelect = null;
this.itemEnough = null;
}
refreshEnoughBGSprite(e) {}
refreshSelectBGSprite(e) {
this.itemSelect.active = e;
}
initdata(e) {
this.coinLabel1.string = o.default.getmonstr(e);
this.coinLabel2.string = o.default.getmonstr(e);
this.coinLabel3.string = o.default.getmonstr(e);
}
};
i([ s(cc.Label) ], r.prototype, "coinLabel1", void 0);
i([ s(cc.Label) ], r.prototype, "coinLabel2", void 0);
i([ s(cc.Label) ], r.prototype, "coinLabel3", void 0);
i([ s(cc.Node) ], r.prototype, "itemSelect", void 0);
i([ s(cc.Node) ], r.prototype, "itemEnough", void 0);
r = i([ n ], r);
a.default = r;
cc._RF.pop();
};
