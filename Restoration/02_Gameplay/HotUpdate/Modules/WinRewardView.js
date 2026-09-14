// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: WinRewardView; dependency map: {"../BaseUIManager/BaseUI":"BaseUI","../BaseUIManager/FlyAnimation":"FlyAnimation","../BaseUIManager/Storage/GameLocalData":"GameLocalData","../BaseUIManager/Storage/NewGamePlayData":"NewGamePlayData","../BaseUIManager/Storage/SoundManager":"SoundManager","../BaseUIManager/Storage/TouchButton":"TouchButton","../LanguageControl/GameManagement":"GameManagement","../LanguageControl/Lab":"Lab"}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "26625Y3ZHFMGpirsxPrKa3H", "WinRewardView");
var i = this && this.__decorate || function(e, t, a, i) {
var o, n = arguments.length, s = n < 3 ? t : null === i ? i = Object.getOwnPropertyDescriptor(t, a) : i;
if ("object" == typeof Reflect && "function" == typeof Reflect.decorate) s = Reflect.decorate(e, t, a, i); else for (var r = e.length - 1; r >= 0; r--) (o = e[r]) && (s = (n < 3 ? o(s) : n > 3 ? o(t, a, s) : o(t, a)) || s);
return n > 3 && s && Object.defineProperty(t, a, s), s;
};
Object.defineProperty(a, "__esModule", {
value: !0
});
const o = e("../LanguageControl/GameManagement"), n = e("../BaseUIManager/Storage/GameLocalData"), s = e("../BaseUIManager/Storage/NewGamePlayData"), r = e("../BaseUIManager/Storage/SoundManager"), l = e("../BaseUIManager/Storage/TouchButton"), c = e("../BaseUIManager/FlyAnimation"), d = e("../BaseUIManager/BaseUI"), h = e("../LanguageControl/Lab"), {ccclass: u, property: g} = cc._decorator;
let p = class extends d.default {
constructor() {
super(...arguments);
this.btnClose = null;
this.wincashTitle = null;
this.coinLabel = null;
this.onlyLabel = null;
this.btnNode = null;
this.btnNodeLabel = null;
this.player = null;
this.coin = 0;
this.onlyCoin = 0;
this._closeCallback = null;
this.win_closeCallback = null;
this.rewardValue = void 0;
}
onLoad() {
this.btnClose.addComponent(l.default).registerTouchEvent(() => {
this.closeCall();
});
this.btnNode.addComponent(l.default).registerTouchEvent(() => {
this.on_close_call();
});
this.onlyLabel.node.addComponent(l.default).registerTouchEvent(() => {
this.closeCall();
});
}
show(e) {
super.show(e);
let t = e.param;
this.player = n.default.getInstance().getData(s.default);
this.player.winShow_count += 1;
this.coin = o.default.getnor(1);
this.onlyCoin = .1 * this.coin;
this._closeCallback = null;
(null == t ? void 0 : t.closeCallback) && (this._closeCallback = t.closeCallback);
this.wincashTitle.string = h.default.getlab("19");
o.default.GetCountryDang();
this.btnClose.active = !1;
this.scheduleOnce(function() {
this.btnClose.active = !0;
}, 1.5);
this.coinLabel.string = `+${o.default.getmonstr(this.coin)}`;
this.btnNodeLabel.string = h.default.getlab("21");
r.default.playSound("win_audio");
}
onWithdraw() {
n.default.getInstance().getData(s.default).skippedAdCount = 0;
n.default.getInstance().saveToUserDefault();
c.default.instance.ShowCoinEffect(this.coin, this.node);
this.on_close_call();
}
closeCall() {
if (this.player.winShow_count > 2) {
this.on_close_call();
this.player.winShow_count = 0;
n.default.getInstance().saveToUserDefault();
adManager.adname = "9_A";
adManager.showVideo(this.onWithdraw.bind(this), () => {}, !1);
} else this.on_close_call();
}
};
i([ g(cc.Node) ], p.prototype, "btnClose", void 0);
i([ g(cc.Label) ], p.prototype, "wincashTitle", void 0);
i([ g(cc.Label) ], p.prototype, "coinLabel", void 0);
i([ g(cc.Label) ], p.prototype, "onlyLabel", void 0);
i([ g(cc.Node) ], p.prototype, "btnNode", void 0);
i([ g(cc.Label) ], p.prototype, "btnNodeLabel", void 0);
p = i([ d.registerUIPath("GameDialog/WinRewardView"), u ], p);
a.default = p;
cc._RF.pop();
};
