// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: SuccessDialog; dependency map: {"../BaseUIManager/BaseUI":"BaseUI","../BaseUIManager/FlyAnimation":"FlyAnimation","../BaseUIManager/Storage/GameLocalData":"GameLocalData","../BaseUIManager/Storage/NewGamePlayData":"NewGamePlayData","../BaseUIManager/Storage/SoundManager":"SoundManager","../BaseUIManager/Storage/TouchButton":"TouchButton","../HWL/HWL_TStool":"HWL_TStool","../LanguageControl/GameManagement":"GameManagement","../LanguageControl/Lab":"Lab"}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "d80e5LJRVVGca2w2dCYt3ZJ", "SuccessDialog");
var i = this && this.__decorate || function(e, t, a, i) {
var o, n = arguments.length, s = n < 3 ? t : null === i ? i = Object.getOwnPropertyDescriptor(t, a) : i;
if ("object" == typeof Reflect && "function" == typeof Reflect.decorate) s = Reflect.decorate(e, t, a, i); else for (var r = e.length - 1; r >= 0; r--) (o = e[r]) && (s = (n < 3 ? o(s) : n > 3 ? o(t, a, s) : o(t, a)) || s);
return n > 3 && s && Object.defineProperty(t, a, s), s;
};
Object.defineProperty(a, "__esModule", {
value: !0
});
const o = e("../BaseUIManager/BaseUI"), n = e("../LanguageControl/GameManagement"), s = e("../BaseUIManager/Storage/GameLocalData"), r = e("../BaseUIManager/Storage/NewGamePlayData"), l = e("../BaseUIManager/Storage/SoundManager"), c = e("../BaseUIManager/Storage/TouchButton"), d = e("../BaseUIManager/FlyAnimation"), h = e("../LanguageControl/Lab"), u = e("../HWL/HWL_TStool"), {ccclass: g, property: p} = cc._decorator;
let f = class extends o.default {
constructor() {
super(...arguments);
this.titleLabel = null;
this.CaiDai_TX = null;
this.QiQiuBiaoT_TX = null;
this.coin_label = null;
this.videoLabel = null;
this.onlyLabel = null;
this.multsLabel = [];
this.jiantouNode = null;
this._closeCallback = null;
this._coin = 0;
this.move_coin = 0;
this.only_coin = 0;
}
onLoad() {
super.onLoad();
this.onlyLabel.node.addComponent(c.default).registerTouchEvent(() => {
this.showSingleRewardView();
});
this.videoLabel.node.parent.addComponent(c.default).registerTouchEvent(() => {
u.HWLshowAd("1_A", e => {
let t = n.default.cashCoin(e);
d.default.instance.ShowRealCoinEffect(t, this.node);
this.showRewardView();
}, () => {
this.showFaildRewardView();
});
});
}
show(e) {
super.show(e);
e.param;
this.unscheduleAllCallbacks();
this.QiQiuBiaoT_TX.setCompleteListener(null);
this.QiQiuBiaoT_TX.clearTracks();
this.QiQiuBiaoT_TX.setToSetupPose();
this.QiQiuBiaoT_TX.setSlotsToSetupPose();
this.scheduleOnce(function() {
this.QiQiuBiaoT_TX.setAnimation(0, "idle", !1);
}, .35);
this.titleLabel.string = h.default.getlab("3");
l.default.playSound("success");
let t = n.default.getnor(.5), a = s.default.getInstance().getData(r.default);
a.success_count += 1;
var i = n.default.globalData.global.tixian_products[0].withdrawAmount * n.default.GetCountryDang();
n.default.getCurrentRewardMultiplier(a.red_bag, i);
this._coin = t;
this.only_coin = .2 * this._coin;
this.only_coin < n.default.get_reward_base_min() && (this.only_coin = n.default.get_reward_base_min());
this.onlyLabel.node.active = !1;
this.scheduleOnce(function() {
this.onlyLabel.node.active = !0;
}, 1.5);
this.coin_label.string = `${n.default.getmonstr(this._coin)}`;
this.onlyLabel.string = `${n.default.getmonstr(this.only_coin)}`;
s.default.getInstance().saveToUserDefault();
this.playmove();
}
playmove() {
cc.Tween.stopAllByTarget(this.jiantouNode);
let e = cc.v3(-220, -240), t = cc.v3(220, -240), a = [ 2, 3, 5, 3, 2 ];
this.jiantouNode.setPosition(e);
for (let e = 0; e < this.multsLabel.length; e++) this.multsLabel[e].string = `x${a[e]}`;
this.videoLabel.string = h.default.getlab("4") + `x${a[0]}`;
let i = cc.tween(this.jiantouNode).to(2, {
position: t
}, {
easing: "linear",
onUpdate: (e, t) => {
let i = Math.floor(4 * t);
i = Math.min(i, 4);
this.videoLabel.string = h.default.getlab("4") + `x${a[i]}`;
let o = this.only_coin * a[i];
this.coin_label.string = `${n.default.getmonstr(o)}`;
this.move_coin = o;
}
}).to(2, {
position: e
}, {
easing: "linear",
onUpdate: (e, t) => {
let i = 4 - Math.floor(4 * t);
i = Math.max(0, Math.min(i, 4));
this.videoLabel.string = h.default.getlab("4") + `x${a[i]}`;
let o = this.only_coin * a[i];
this.coin_label.string = `${n.default.getmonstr(o)}`;
this.move_coin = o;
}
});
cc.tween(this.jiantouNode).repeatForever(i).start();
}
showFaildRewardView() {
this.on_close_call();
}
showSingleRewardView() {
let e = s.default.getInstance().getData(r.default);
const t = e.Passlevel;
let a = n.default.globalData.global.success_target_levels || [];
a.length > 0 && (a = n.default.getCurrentJsonData(t, a));
if (Array.isArray(a) && a.length > 1 && e.success_count > a[1]) {
e.success_count = 0;
u.HWLshowAd("1_A", e => {
let t = n.default.cashCoin(e);
d.default.instance.ShowRealCoinEffect(t, this.node);
this.showRewardView();
}, () => {
this.showFaildRewardView();
});
} else {
d.default.instance.ShowCoinEffect(this.only_coin, this.node, !1);
this.on_close_call();
}
}
showRewardView(e = !1) {
this._coin = this.move_coin;
d.default.instance.ShowCoinEffect(this._coin, this.node);
this.on_close_call();
}
};
i([ p(cc.Label) ], f.prototype, "titleLabel", void 0);
i([ p(sp.Skeleton) ], f.prototype, "CaiDai_TX", void 0);
i([ p(sp.Skeleton) ], f.prototype, "QiQiuBiaoT_TX", void 0);
i([ p(cc.Label) ], f.prototype, "coin_label", void 0);
i([ p(cc.Label) ], f.prototype, "videoLabel", void 0);
i([ p(cc.Label) ], f.prototype, "onlyLabel", void 0);
i([ p([ cc.Label ]) ], f.prototype, "multsLabel", void 0);
i([ p(cc.Node) ], f.prototype, "jiantouNode", void 0);
f = i([ o.registerUIPath("GameDialog/SuccessDialog"), g ], f);
a.default = f;
cc._RF.pop();
};
