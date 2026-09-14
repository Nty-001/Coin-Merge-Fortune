// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/base/assets/assets/main/index.ba75b.js
// Module: Item; dependency map: {"../ATools/ImagesPanel":"ImagesPanel","../ATools/LocalDataManager":"LocalDataManager","../ATools/TouchButton":"TouchButton","../ATools/UIManager":"UIManager","../GameScene":"GameScene"}
module.exports = function(e, t, o) {
"use strict";
cc._RF.push(t, "429bcD8CB5NX67rv+zufyNz", "Item");
var a = this && this.__decorate || function(e, t, o, a) {
var s, n = arguments.length, i = n < 3 ? t : null === a ? a = Object.getOwnPropertyDescriptor(t, o) : a;
if ("object" == typeof Reflect && "function" == typeof Reflect.decorate) i = Reflect.decorate(e, t, o, a); else for (var c = e.length - 1; c >= 0; c--) (s = e[c]) && (i = (n < 3 ? s(i) : n > 3 ? s(t, o, i) : s(t, o)) || i);
return n > 3 && i && Object.defineProperty(t, o, i), i;
};
Object.defineProperty(o, "__esModule", {
value: !0
});
const s = e("../ATools/ImagesPanel"), n = e("../ATools/LocalDataManager"), i = e("../ATools/TouchButton"), c = e("../ATools/UIManager"), l = e("../GameScene"), {ccclass: r, property: h} = cc._decorator;
let u = class extends cc.Component {
constructor() {
super(...arguments);
this.lvel = null;
this.coin = null;
this.imagesPanel = null;
this.gameModel = 0;
this.isOpen = !1;
}
onLoad() {
this.node.addComponent(i.default).registerTouchEvent(() => {
if (this.isOpen) l.default.instance.onEnterGame(this.gameModel); else if (n.default.getInstance().getGameData().reduceCoins(1e3 * this.gameModel)) {
n.default.getInstance().getGameData().setPassStatus(this.gameModel, 1);
this.initItem(this.gameModel);
} else c.default.instance.onShowToast("Not enough gold coins");
});
}
start() {}
initItem(e) {
this.gameModel = e;
let t = n.default.getInstance().getGameData().getPassStatus(e);
this.isOpen = !!t;
n.default.getInstance().getGameData().getPassLevel(e);
if (t) {
this.imagesPanel.value = 0;
this.coin.active = !1;
} else {
this.imagesPanel.value = 1;
this.coin.active = !0;
}
}
};
a([ h(cc.Label) ], u.prototype, "lvel", void 0);
a([ h(cc.Node) ], u.prototype, "coin", void 0);
a([ h(s.default) ], u.prototype, "imagesPanel", void 0);
u = a([ r ], u);
o.default = u;
cc._RF.pop();
};
