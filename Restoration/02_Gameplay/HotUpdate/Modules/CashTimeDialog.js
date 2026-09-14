// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: CashTimeDialog; dependency map: {"../BaseUIManager/BaseUI":"BaseUI","../BaseUIManager/FlyAnimation":"FlyAnimation","../BaseUIManager/Storage/GameLocalData":"GameLocalData","../BaseUIManager/Storage/NewGamePlayData":"NewGamePlayData","../BaseUIManager/Storage/SoundManager":"SoundManager","../BaseUIManager/Storage/TouchButton":"TouchButton","../HWL/HWL_TStool":"HWL_TStool","../LanguageControl/GameManagement":"GameManagement","../LanguageControl/Lab":"Lab"}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "bb0436lvs5O3rmvlXcbjt+S", "CashTimeDialog");
var i = this && this.__decorate || function(e, t, a, i) {
var o, n = arguments.length, s = n < 3 ? t : null === i ? i = Object.getOwnPropertyDescriptor(t, a) : i;
if ("object" == typeof Reflect && "function" == typeof Reflect.decorate) s = Reflect.decorate(e, t, a, i); else for (var r = e.length - 1; r >= 0; r--) (o = e[r]) && (s = (n < 3 ? o(s) : n > 3 ? o(t, a, s) : o(t, a)) || s);
return n > 3 && s && Object.defineProperty(t, a, s), s;
};
Object.defineProperty(a, "__esModule", {
value: !0
});
const o = e("../BaseUIManager/BaseUI"), n = e("../BaseUIManager/FlyAnimation"), s = e("../BaseUIManager/Storage/GameLocalData"), r = e("../BaseUIManager/Storage/NewGamePlayData"), l = e("../BaseUIManager/Storage/SoundManager"), c = e("../BaseUIManager/Storage/TouchButton"), d = e("../HWL/HWL_TStool"), h = e("../LanguageControl/GameManagement"), u = e("../LanguageControl/Lab"), {ccclass: g, property: p} = cc._decorator;
let f = class extends o.default {
constructor() {
super(...arguments);
this.coin = 0;
this.onlyCoin = 0;
this.onlyshow = !1;
this.timeCallback = null;
this.soundId = -1;
}
onLoad() {
super.onLoad();
this.videolabel.node.parent.addComponent(c.default).registerTouchEvent(() => {
if (-1 !== this.soundId) {
l.default.stopSound(this.soundId);
this.soundId = -1;
}
cc.Tween.stopAllByTarget(this.Pirate.node);
cc.Tween.stopAllByTarget(this.Pirate1.node);
d.HWLshowAd("CashTimeDialog", e => {
let t = h.default.cashCoin(e);
n.default.instance.ShowRealCoinEffect(t, this.node);
n.default.instance.ShowCoinEffect(this.coin, this.node);
this.on_close_call();
}, () => {});
});
this.onlyLabel.node.addComponent(c.default).registerTouchEvent(() => {
if (-1 !== this.soundId) {
l.default.stopSound(this.soundId);
this.soundId = -1;
}
cc.Tween.stopAllByTarget(this.Pirate.node);
cc.Tween.stopAllByTarget(this.Pirate1.node);
if (this.onlyshow) {
n.default.instance.ShowCoinEffect(this.onlyCoin, this.node);
this.on_close_call();
} else this.on_close_call();
});
}
show(e) {
super.show(e);
e.param;
this.unscheduleAllCallbacks();
this.soundId = l.default.playSound("CashTime_end", !0, 1);
l.default.playSound("CashTime_reward", !1, 1);
this.Pirate.node.active = !1;
this.Pirate1.node.active = !1;
this.DaBiaoTi_TX.setAnimation(0, "idle", !1);
this.DaBiaoTi_TX.setCompleteListener(() => {
this.DaBiaoTi_TX.setCompleteListener(null);
this.DaBiaoTi_TX.setAnimation(0, "idle1", !0);
});
cc.Tween.stopAllByTarget(this.Pirate.node);
cc.Tween.stopAllByTarget(this.Pirate1.node);
this.Pirate.node.stopAllActions();
this.Pirate1.node.stopAllActions();
this.Pirate.clearTracks();
this.Pirate1.clearTracks();
this.Pirate.setToSetupPose();
this.Pirate1.setToSetupPose();
cc.tween(this.Pirate.node).delay(.5).call(() => {
this.Pirate.node.active = !0;
this.Pirate.setAnimation(0, "qian", !0);
}).start();
cc.tween(this.Pirate1.node).delay(6).call(() => {
this.Pirate1.node.active = !0;
this.Pirate1.setAnimation(0, "qian", !0);
}).start();
this.coin = h.default.getnor(2);
this.onlyCoin = .1 * this.coin;
this.videolabel.string = u.default.getlab("4");
this.onlyLabel.string = u.default.getlab("67", 10);
this.coinLabel.string = `${h.default.getmonstr(this.coin)}`;
let t = s.default.getInstance().getData(r.default).red_bag, a = h.default.all_config_data.GameData.global.cashCoins * h.default.GetCountryDang();
this.onlyshow = t < a;
this.onlyLabel.string = t >= a ? u.default.getlab("68") : u.default.getlab("67");
}
};
i([ p(sp.Skeleton) ], f.prototype, "Pirate", void 0);
i([ p(sp.Skeleton) ], f.prototype, "Pirate1", void 0);
i([ p(sp.Skeleton) ], f.prototype, "DaBiaoTi_TX", void 0);
i([ p(cc.Label) ], f.prototype, "coinLabel", void 0);
i([ p(cc.Label) ], f.prototype, "videolabel", void 0);
i([ p(cc.Label) ], f.prototype, "onlyLabel", void 0);
f = i([ o.registerUIPath("GameDialog/CashTimeDialog"), g ], f);
a.default = f;
cc._RF.pop();
};
