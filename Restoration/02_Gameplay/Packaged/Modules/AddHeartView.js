// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/base/assets/assets/main/index.ba75b.js
// Module: AddHeartView; dependency map: {"../ATools/BaseUI":"BaseUI","../ATools/EventCenter":"EventCenter","../ATools/GameEventConsts":"GameEventConsts","../ATools/LocalDataManager":"LocalDataManager","../ATools/TouchButton":"TouchButton","../ATools/UIManager":"UIManager","../Game/Utils":"Utils","../GameScene":"GameScene"}
module.exports = function(e, t, o) {
"use strict";
cc._RF.push(t, "7e3a3gRQiJLrrLy9944vKcH", "AddHeartView");
var a = this && this.__decorate || function(e, t, o, a) {
var s, n = arguments.length, i = n < 3 ? t : null === a ? a = Object.getOwnPropertyDescriptor(t, o) : a;
if ("object" == typeof Reflect && "function" == typeof Reflect.decorate) i = Reflect.decorate(e, t, o, a); else for (var c = e.length - 1; c >= 0; c--) (s = e[c]) && (i = (n < 3 ? s(i) : n > 3 ? s(t, o, i) : s(t, o)) || i);
return n > 3 && i && Object.defineProperty(t, o, i), i;
};
Object.defineProperty(o, "__esModule", {
value: !0
});
const s = e("../ATools/BaseUI"), n = e("../ATools/EventCenter"), i = e("../ATools/GameEventConsts"), c = e("../ATools/LocalDataManager"), l = e("../ATools/TouchButton"), r = e("../ATools/UIManager"), h = e("../Game/Utils"), u = e("../GameScene"), {ccclass: d, property: g} = cc._decorator;
let p = class extends s.default {
constructor() {
super(...arguments);
this.btn_close = null;
this.btn_add = null;
this.lbl_heart = null;
this.lbl_time = null;
this.closeCallBack = null;
this.addCallBack = null;
}
onLoad() {
n.EventCenter.getInstance().register(i.GameEventName.HeartBeat, () => {
this.lbl_time.string = this.getTimeStr();
}, this);
this.registerEvent();
}
start() {}
onShow(e) {
this.lbl_heart.string = `Your life: ${c.default.getInstance().getGameData().getHeart()}`;
this.lbl_time.string = this.getTimeStr();
this.closeCallBack = e.closeCallBack;
this.addCallBack = e.addCallBack;
}
getTimeStr() {
let e = u.default.instance.heartDown;
return `After<color=#FF3218>${h.default.getTimeStr(1e3 * e)}</c> +1`;
}
registerEvent() {
this.btn_close.addComponent(l.default).registerTouchEvent(() => {
this.closeCallBack && this.closeCallBack();
this.onClose();
});
this.btn_add.addComponent(l.default).registerTouchEvent(() => {
if (c.default.getInstance().getGameData().reduceCoins(100)) {
c.default.getInstance().getGameData().addHeart(1);
this.onClose();
this.addCallBack && this.addCallBack();
} else r.default.instance.onShowToast("Not enough gold coins");
});
}
};
a([ g(cc.Node) ], p.prototype, "btn_close", void 0);
a([ g(cc.Node) ], p.prototype, "btn_add", void 0);
a([ g(cc.Label) ], p.prototype, "lbl_heart", void 0);
a([ g(cc.RichText) ], p.prototype, "lbl_time", void 0);
p = a([ s.registerUIPath("prefab/AddHeartView"), d ], p);
o.default = p;
cc._RF.pop();
};
