// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/base/assets/assets/main/index.ba75b.js
// Module: FailureView; dependency map: {"../ATools/BaseUI":"BaseUI","../ATools/LocalDataManager":"LocalDataManager","../ATools/SoundManager":"SoundManager","../ATools/TouchButton":"TouchButton","../ATools/UIManager":"UIManager","../Game/LevelManager":"LevelManager","../GameScene":"GameScene"}
module.exports = function(e, t, o) {
"use strict";
cc._RF.push(t, "11c9fWNcqhH4JiE3t6Zxg9C", "FailureView");
var a = this && this.__decorate || function(e, t, o, a) {
var s, n = arguments.length, i = n < 3 ? t : null === a ? a = Object.getOwnPropertyDescriptor(t, o) : a;
if ("object" == typeof Reflect && "function" == typeof Reflect.decorate) i = Reflect.decorate(e, t, o, a); else for (var c = e.length - 1; c >= 0; c--) (s = e[c]) && (i = (n < 3 ? s(i) : n > 3 ? s(t, o, i) : s(t, o)) || i);
return n > 3 && i && Object.defineProperty(t, o, i), i;
};
Object.defineProperty(o, "__esModule", {
value: !0
});
const s = e("../ATools/BaseUI"), n = e("../ATools/LocalDataManager"), i = e("../ATools/SoundManager"), c = e("../ATools/TouchButton"), l = e("../ATools/UIManager"), r = e("../Game/LevelManager"), h = e("../GameScene"), {ccclass: u, property: d} = cc._decorator;
let g = class extends s.default {
constructor() {
super(...arguments);
this.btn_close = null;
this.btn_retrt = null;
this.btn_back = null;
}
onLoad() {
this.registerEvent();
}
start() {}
onShow(e) {
i.default.playSound(i.SoundName.ShiBai);
}
registerEvent() {
this.btn_close.addComponent(c.default).registerTouchEvent(() => {
this.onClose();
h.default.instance.onEnterHome();
});
this.btn_retrt.addComponent(c.default).registerTouchEvent(() => {
this.onClose();
this.restartGame();
});
this.btn_back.addComponent(c.default).registerTouchEvent(() => {
this.onClose();
h.default.instance.onEnterHome();
});
}
restartGame() {
n.default.getInstance().getGameData().reduceHeart(1) ? r.default.getInstance().startGame() : l.default.instance.onShowView(s.UIConfig.AddHeartView, {
addCallBack: () => {
n.default.getInstance().getGameData().reduceCoins(100) ? r.default.getInstance().startGame() : l.default.instance.onShowToast("Not enough gold coins");
},
closeCallBack: () => {
h.default.instance.onEnterHome();
}
});
}
};
a([ d(cc.Node) ], g.prototype, "btn_close", void 0);
a([ d(cc.Node) ], g.prototype, "btn_retrt", void 0);
a([ d(cc.Node) ], g.prototype, "btn_back", void 0);
g = a([ s.registerUIPath("prefab/FailureView"), u ], g);
o.default = g;
cc._RF.pop();
};
