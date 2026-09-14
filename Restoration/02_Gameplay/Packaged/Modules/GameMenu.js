// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/base/assets/assets/main/index.ba75b.js
// Module: GameMenu; dependency map: {"../ATools/BaseUI":"BaseUI","../ATools/EventCenter":"EventCenter","../ATools/GameEventConsts":"GameEventConsts","../ATools/LocalDataManager":"LocalDataManager","../ATools/TouchButton":"TouchButton","../ATools/UIManager":"UIManager"}
module.exports = function(e, t, o) {
"use strict";
cc._RF.push(t, "2d3c6iRBbRFqLCuf/c5oAjR", "GameMenu");
var a = this && this.__decorate || function(e, t, o, a) {
var s, n = arguments.length, i = n < 3 ? t : null === a ? a = Object.getOwnPropertyDescriptor(t, o) : a;
if ("object" == typeof Reflect && "function" == typeof Reflect.decorate) i = Reflect.decorate(e, t, o, a); else for (var c = e.length - 1; c >= 0; c--) (s = e[c]) && (i = (n < 3 ? s(i) : n > 3 ? s(t, o, i) : s(t, o)) || i);
return n > 3 && i && Object.defineProperty(t, o, i), i;
};
Object.defineProperty(o, "__esModule", {
value: !0
});
const s = e("../ATools/BaseUI"), n = e("../ATools/EventCenter"), i = e("../ATools/GameEventConsts"), c = e("../ATools/LocalDataManager"), l = e("../ATools/TouchButton"), r = e("../ATools/UIManager"), {ccclass: h, property: u} = cc._decorator;
let d = class extends cc.Component {
constructor() {
super(...arguments);
this.btn_coin = null;
this.btn_heart = null;
this.lbl_coin = null;
this.lbl_heart = null;
}
onLoad() {
n.EventCenter.getInstance().register(i.GameEventName.UseCoins, this.onFlushCoin, this);
n.EventCenter.getInstance().register(i.GameEventName.UserHeart, this.onFlushCoin, this);
this.btn_coin.addComponent(l.default).registerTouchEvent(() => {
r.default.instance.onShowView(s.UIConfig.InfoView, {
type: 1
});
});
this.btn_heart.addComponent(l.default).registerTouchEvent(() => {
r.default.instance.onShowView(s.UIConfig.AddHeartView, {
type: 1
});
});
}
onDestroy() {
n.EventCenter.getInstance().remove(i.GameEventName.UseCoins, this);
n.EventCenter.getInstance().remove(i.GameEventName.UserHeart, this);
}
start() {
this.onFlushCoin();
}
onFlushCoin() {
let e = c.default.getInstance().getGameData().getCoins(), t = c.default.getInstance().getGameData().getHeart();
this.lbl_coin.string = `${e}`;
this.lbl_heart.string = `${t}`;
}
};
a([ u(cc.Node) ], d.prototype, "btn_coin", void 0);
a([ u(cc.Node) ], d.prototype, "btn_heart", void 0);
a([ u(cc.Label) ], d.prototype, "lbl_coin", void 0);
a([ u(cc.Label) ], d.prototype, "lbl_heart", void 0);
d = a([ h ], d);
o.default = d;
cc._RF.pop();
};
