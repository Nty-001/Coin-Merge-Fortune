// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: GameRealWDItem; dependency map: {"../BaseUIManager/Storage/GameLocalData":"GameLocalData","../BaseUIManager/Storage/NewGamePlayData":"NewGamePlayData","../LanguageControl/GameManagement":"GameManagement","../LanguageControl/Lab":"Lab"}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "5b337VhB7FDELcsMEsvurZX", "GameRealWDItem");
var i = this && this.__decorate || function(e, t, a, i) {
var o, n = arguments.length, s = n < 3 ? t : null === i ? i = Object.getOwnPropertyDescriptor(t, a) : i;
if ("object" == typeof Reflect && "function" == typeof Reflect.decorate) s = Reflect.decorate(e, t, a, i); else for (var r = e.length - 1; r >= 0; r--) (o = e[r]) && (s = (n < 3 ? o(s) : n > 3 ? o(t, a, s) : o(t, a)) || s);
return n > 3 && s && Object.defineProperty(t, a, s), s;
};
Object.defineProperty(a, "__esModule", {
value: !0
});
const o = e("../BaseUIManager/Storage/GameLocalData"), n = e("../BaseUIManager/Storage/NewGamePlayData"), s = e("../LanguageControl/GameManagement"), r = e("../LanguageControl/Lab"), {ccclass: l, property: c} = cc._decorator;
let d = class extends cc.Component {
constructor() {
super(...arguments);
this.tips = null;
this.coinLabel = null;
this.select = null;
this.grayBG = null;
this.platform = null;
}
refreshSelect(e, t = !1, a) {
this.grayBG.active = !1;
this.select.active = e;
if (t) {
this.grayBG.active = !0;
o.default.getInstance().getData(n.default);
this.select.active = !1;
}
this.tips.node.parent.active = !1;
if (t) {
this.tips.node.parent.active = !0;
this.tips.string = r.default.getlab("19");
}
}
initRedldata(e, t, a) {
this.coinLabel.string = s.default.getRealMonstr(e);
this.coinLabel.string.length > 14 && (this.coinLabel.overflow = cc.Label.Overflow.SHRINK);
}
initdata(e) {
this.coinLabel.string = s.default.getmonstr(e);
}
};
i([ c(cc.Label) ], d.prototype, "tips", void 0);
i([ c(cc.Label) ], d.prototype, "coinLabel", void 0);
i([ c(cc.Node) ], d.prototype, "select", void 0);
i([ c(cc.Node) ], d.prototype, "grayBG", void 0);
i([ c(cc.Sprite) ], d.prototype, "platform", void 0);
d = i([ l ], d);
a.default = d;
cc._RF.pop();
};
