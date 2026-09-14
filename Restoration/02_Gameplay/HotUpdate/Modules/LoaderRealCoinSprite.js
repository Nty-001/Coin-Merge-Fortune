// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: LoaderRealCoinSprite; dependency map: {"./GameManagement":"GameManagement"}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "06500OwCRlGAr7owZhvXZf/", "LoaderRealCoinSprite");
var i = this && this.__decorate || function(e, t, a, i) {
var o, n = arguments.length, s = n < 3 ? t : null === i ? i = Object.getOwnPropertyDescriptor(t, a) : i;
if ("object" == typeof Reflect && "function" == typeof Reflect.decorate) s = Reflect.decorate(e, t, a, i); else for (var r = e.length - 1; r >= 0; r--) (o = e[r]) && (s = (n < 3 ? o(s) : n > 3 ? o(t, a, s) : o(t, a)) || s);
return n > 3 && s && Object.defineProperty(t, a, s), s;
};
Object.defineProperty(a, "__esModule", {
value: !0
});
const o = e("./GameManagement"), {ccclass: n, property: s} = cc._decorator;
let r = class extends cc.Component {
constructor() {
super(...arguments);
this.coin_type = 1;
this.COUNTRY_MAP = {
BR: "BR",
ID: "ID"
};
}
onLoad() {
this.node.opacity = 0;
this.loadCoinSprite();
cc.systemEvent.on("refresh_realcoin_sprite", this.loadCoinSprite, this);
}
loadCoinSprite(e = !1) {
const t = o.default.language, a = this.COUNTRY_MAP[t] || "US";
let i;
const n = `Image/common/${i = e && 1 === this.coin_type ? `coin_gray_${a}` : `coin_${a}_${this.coin_type}`}`;
o.default.loadSpriteFrame(n, e => {
this.getComponent(cc.Sprite).spriteFrame = e;
this.node.opacity = 255;
});
}
onDestroy() {
cc.systemEvent.off("refresh_realcoin_sprite", this.loadCoinSprite, this);
}
};
i([ s ], r.prototype, "coin_type", void 0);
r = i([ n ], r);
a.default = r;
cc._RF.pop();
};
