// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: PlatformLoader; dependency map: {"../BaseUIManager/Storage/GameLocalData":"GameLocalData","../BaseUIManager/Storage/PlayData":"PlayData","../HWL/ServerConfig":"ServerConfig","../LanguageControl/GameManagement":"GameManagement"}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "b85799V1dJC7ZpCaCv8cxzU", "PlatformLoader");
var i = this && this.__decorate || function(e, t, a, i) {
var o, n = arguments.length, s = n < 3 ? t : null === i ? i = Object.getOwnPropertyDescriptor(t, a) : i;
if ("object" == typeof Reflect && "function" == typeof Reflect.decorate) s = Reflect.decorate(e, t, a, i); else for (var r = e.length - 1; r >= 0; r--) (o = e[r]) && (s = (n < 3 ? o(s) : n > 3 ? o(t, a, s) : o(t, a)) || s);
return n > 3 && s && Object.defineProperty(t, a, s), s;
};
Object.defineProperty(a, "__esModule", {
value: !0
});
const o = e("../BaseUIManager/Storage/GameLocalData"), n = e("../BaseUIManager/Storage/PlayData"), s = e("../HWL/ServerConfig"), r = e("../LanguageControl/GameManagement"), {ccclass: l, property: c} = cc._decorator;
let d = class extends cc.Component {
constructor() {
super(...arguments);
this.platfrom = null;
}
onEnable() {
let e = o.default.getInstance().getData(n.default), t = r.default.normalizeCountry(r.default.language), a = [ s.RealCashPlatform.PayPal ];
"BR" == t ? a = [ s.RealCashPlatform.Pagbank, s.RealCashPlatform.PIX ] : "ID" == t ? a = [ s.RealCashPlatform.DANA, s.RealCashPlatform.OVO ] : "TH" == t ? a = [ s.RealCashPlatform.Truemoney ] : "MY" == t ? a = [ s.RealCashPlatform.TNG ] : "VN" == t ? a = [ s.RealCashPlatform.ZaloPay ] : "PH" == t && (a = [ s.RealCashPlatform.GCash, s.RealCashPlatform.Graboay, s.RealCashPlatform.Paymaya ]);
if (a.indexOf(e.realSelectPlatform) < 0) {
e.realSelectPlatform = a[0];
o.default.getInstance().saveToUserDefault();
}
r.default.loadSpriteFrame("texture/CashPlat/" + e.realSelectPlatform + "_1", e => {
this.platfrom.spriteFrame = e;
});
}
};
i([ c(cc.Sprite) ], d.prototype, "platfrom", void 0);
d = i([ l ], d);
a.default = d;
cc._RF.pop();
};
