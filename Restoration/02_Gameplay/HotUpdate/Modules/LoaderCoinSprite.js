// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: LoaderCoinSprite; dependency map: {"./GameManagement":"GameManagement"}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "d69e3xwokpN0oBzR++PtqB1", "LoaderCoinSprite");
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
}
onLoad() {
this.node.opacity = 0;
this.loadCoinSprite();
cc.systemEvent.on("refresh_coin_sprite", this.loadCoinSprite, this);
}
loadCoinSprite() {
let e = "texture/coin" + this.coin_type + "/";
o.default.loadSpriteFrame(e + o.default.GetCountry(), e => {
if (this.node) {
this.node.getComponent(cc.Sprite).spriteFrame = e;
this.node.opacity = 255;
}
});
}
onDestroy() {
cc.systemEvent.off("refresh_coin_sprite", this.loadCoinSprite, this);
}
};
i([ s ], r.prototype, "coin_type", void 0);
r = i([ n ], r);
a.default = r;
cc._RF.pop();
};
