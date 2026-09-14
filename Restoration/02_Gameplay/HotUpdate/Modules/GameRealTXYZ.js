// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: GameRealTXYZ; dependency map: {"../BaseUIManager/BaseUI":"BaseUI","../BaseUIManager/Storage/GameLocalData":"GameLocalData","../BaseUIManager/Storage/NewGamePlayData":"NewGamePlayData","../BaseUIManager/Storage/PlayData":"PlayData","../BaseUIManager/Storage/TouchButton":"TouchButton","../BaseUIManager/UIConfig":"UIConfig","../BaseUIManager/UIManagerNew":"UIManagerNew","../LanguageControl/GameManagement":"GameManagement","../LanguageControl/Lab":"Lab","../game/UI_TopRewardLayer":"UI_TopRewardLayer","./GameRealWDActiveTips":"GameRealWDActiveTips","./GameRealWDDialog":"GameRealWDDialog"}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "e44dcjlfIxFwozYs342510p", "GameRealTXYZ");
var i = this && this.__decorate || function(e, t, a, i) {
var o, n = arguments.length, s = n < 3 ? t : null === i ? i = Object.getOwnPropertyDescriptor(t, a) : i;
if ("object" == typeof Reflect && "function" == typeof Reflect.decorate) s = Reflect.decorate(e, t, a, i); else for (var r = e.length - 1; r >= 0; r--) (o = e[r]) && (s = (n < 3 ? o(s) : n > 3 ? o(t, a, s) : o(t, a)) || s);
return n > 3 && s && Object.defineProperty(t, a, s), s;
}, o = this && this.__awaiter || function(e, t, a, i) {
return new (a || (a = Promise))(function(o, n) {
function s(e) {
try {
l(i.next(e));
} catch (e) {
n(e);
}
}
function r(e) {
try {
l(i.throw(e));
} catch (e) {
n(e);
}
}
function l(e) {
e.done ? o(e.value) : (t = e.value, t instanceof a ? t : new a(function(e) {
e(t);
})).then(s, r);
var t;
}
l((i = i.apply(e, t || [])).next());
});
};
Object.defineProperty(a, "__esModule", {
value: !0
});
const n = e("../BaseUIManager/BaseUI"), s = e("../BaseUIManager/Storage/GameLocalData"), r = e("../BaseUIManager/Storage/NewGamePlayData"), l = e("../BaseUIManager/Storage/PlayData"), c = e("../BaseUIManager/Storage/TouchButton"), d = e("../BaseUIManager/UIConfig"), h = e("../BaseUIManager/UIManagerNew"), u = e("../game/UI_TopRewardLayer"), g = e("../LanguageControl/GameManagement"), p = e("../LanguageControl/Lab"), f = e("./GameRealWDActiveTips"), m = e("./GameRealWDDialog"), {ccclass: y, property: _} = cc._decorator;
let v = class extends n.default {
constructor() {
super(...arguments);
this.titleLabel = null;
this.coinLabel = null;
this.realmissionLabel = null;
this.real_creditedLabel = null;
this.stageTips1 = null;
this.stageTips2 = null;
this.stageTips3 = null;
this.stageTips4 = null;
this.jxBtn = null;
this.isGaizAnimationPlayed = !1;
this.resJson = null;
this.currentIndex = 0;
this.coinConfig = null;
this.withdrawResultShown = !1;
this.addIndex = 0;
}
onLoad() {
this.jxBtn.node.addComponent(c.default).registerTouchEvent(() => {
this.on_close_call();
});
}
onDisable() {
this.unscheduleAllCallbacks();
}
show(e) {
super.show(e);
let t = e.param;
this.currentIndex = t.currentIndex;
this.coinConfig = g.default.newfake_products.new_Fake_products[this.currentIndex];
this.withdrawResultShown = !1;
this.coinLabel.string = g.default.getRealMonstr(this.coinConfig.withdrawAmount);
this.realmissionLabel.string = p.default.getlab("21") + ": 0";
this.real_creditedLabel.string = p.default.getlab("22") + ": " + this.coinLabel.string;
this.titleLabel.string = p.default.getlab("23");
this.stageTips1.string = p.default.getlab("22");
this.stageTips2.string = p.default.getlab("23");
this.stageTips3.string = p.default.getlab("24");
this.stageTips4.string = p.default.getlab("111");
this.jxBtn.string = p.default.getlab("108");
this.showyzDialog();
this.refreshData(t);
}
showContent() {
var e, t;
if (cc.sys.isBrowser) this.debugshow(); else {
s.default.getInstance().getData(r.default);
this.on_close_call();
m.default.instance && m.default.instance.showRewardView();
if (null != this.resJson) d.default.GameRealWDEmailOccupy, null === (e = this.resJson.data) || void 0 === e || e.cardNumber, 
null === (t = this.resJson.data) || void 0 === t || t.progress; else {
let e = p.default.getlab("76");
h.default.show_toast({
text: e
});
}
}
}
onRealCashResult(e, t) {
this.scheduleOnce(() => {
e && (this.resJson = JSON.parse(t));
}, 2);
}
showyzDialog() {
return o(this, void 0, void 0, function*() {
this.sp1.node.active = !1;
this.sp2.node.active = !1;
this.sp3.node.active = !1;
this.stageTips1.node.scale = 0;
this.stageTips2.node.scale = 0;
this.stageTips3.node.scale = 0;
this.stageTips4.node.scale = 0;
this.isGaizAnimationPlayed = !1;
this.jxBtn.node.parent.opacity = 0;
this.jxBtn.node.parent.scale = 0;
cc.Tween.stopAllByTarget(this.stageTips1.node);
cc.Tween.stopAllByTarget(this.stageTips2.node);
cc.Tween.stopAllByTarget(this.stageTips3.node);
this.unscheduleAllCallbacks();
this.scheduleOnce(() => {
cc.tween(this.stageTips1.node).to(.4, {
scale: 1
}, {
easing: "backOut"
}).start();
this.scheduleOnce(() => {
this.sp1.node.active = !0;
this.sp1.defaultAnimation = "1";
this.sp1.setAnimation(0, "1", !1);
this.sp1.setCompleteListener(() => {
cc.tween(this.stageTips2.node).to(.4, {
scale: 1
}, {
easing: "backOut"
}).start();
this.scheduleOnce(() => {
this.sp2.node.active = !0;
this.sp2.defaultAnimation = "1";
this.sp2.setAnimation(0, "1", !1);
this.sp2.setCompleteListener(() => {
cc.tween(this.stageTips3.node).to(.4, {
scale: 1
}, {
easing: "backOut"
}).start();
cc.tween(this.stageTips4.node).to(.4, {
scale: 1
}, {
easing: "backOut"
}).start();
this.scheduleOnce(() => {
if (!this.isGaizAnimationPlayed) {
this.sp3.node.active = !0;
this.sp3.defaultAnimation = "2";
this.sp3.setAnimation(0, "2", !1);
this.scheduleOnce(() => {
this.sp3.setAnimation(0, "3", !0);
this.jxBtn.node.parent.opacity = 255;
this.jxBtn.node.parent.scale = 1;
this.scheduleOnce(() => {
this.showNextWithdrawResult();
}, .5);
}, .38);
}
this.isGaizAnimationPlayed = !0;
}, .3);
});
}, .3);
});
}, .3);
}, 1);
});
}
refreshData(e) {
let t = this.coinConfig.withdrawAmount;
this.coinLabel.string = g.default.getRealMonstr(t);
}
showNextWithdrawResult() {
if (this.withdrawResultShown) return;
this.withdrawResultShown = !0;
const e = s.default.getInstance().getData(l.default);
this.ensureWithdrawStepData(e);
let t = Math.max(0, Number(e.newFakeMoneyWithdraw[this.currentIndex]) || 0), a = t;
t < this.getWithdrawConditionCount() && this.isWithdrawConditionMet(e, t) && (a = t + 1);
a = this.findFirstUnmetWithdrawStep(e, a);
e.newFakeMoneyWithdraw[this.currentIndex] = a;
s.default.getInstance().saveToUserDefault();
m.default.instance && m.default.instance.updateUI();
this.on_close_call();
if (a < this.getWithdrawConditionCount()) {
const e = {
ui_config_path: d.default.GameRealWDTXTips,
ui_config_name: "GameRealWDTXTips",
param: {
currentIndex: this.currentIndex
}
};
h.default.show_ui(e);
} else f.default.open();
}
ensureWithdrawStepData(e) {
e.newFakeMoneyWithdraw || (e.newFakeMoneyWithdraw = []);
null == e.newFakeMoneyWithdraw[this.currentIndex] && (e.newFakeMoneyWithdraw[this.currentIndex] = 0);
}
findFirstUnmetWithdrawStep(e, t) {
let a = this.getWithdrawConditionCount();
for (let i = Math.max(0, t); i < a; i++) if (!this.isWithdrawConditionMet(e, i)) return i;
return a;
}
isWithdrawConditionMet(e, t) {
return !!this.coinConfig && (0 == t ? this.getNumber(e.coin1024Number) >= this.getNumber(this.coinConfig.condition_merge) : 1 == t ? this.getNumber(e.watch_video_count) >= this.getNumber(this.coinConfig.condition_ad) : 2 == t ? this.getNumber(e.fakeMoney) >= this.getNumber(this.coinConfig.withdrawAmount) : 3 == t ? this.getNumber(e.loginDays) >= this.getNumber(this.coinConfig.condition_login_days) : 4 != t || this.getNumber(e.watch_video_count) >= this.getNumber(this.coinConfig.condition_video));
}
getWithdrawConditionCount() {
return 5;
}
getNumber(e) {
return Math.max(0, Number(e) || 0);
}
debugshow() {
let e = d.default.GameRealWDEmailOccupy, t = "GameRealWDEmailOccupy", a = s.default.getInstance().getData(r.default), i = 1;
if (0 == this.addIndex) {
let i = g.default.getPlayDataWithdrawRecords().withdrawRecords;
const o = i[this.currentIndex] || 0;
if (o < this.coinConfig.withdrawCount) {
i[this.currentIndex] = o + 1;
a.UseRevenue -= this.coinConfig.condition_coin;
s.default.getInstance().saveToUserDefault();
u.default.getInstance().refreshCoinData();
m.default.instance && m.default.instance.showRewardView();
}
e = d.default.GameRealWDTXSuccess;
t = "GameRealWDTXSuccess";
} else if (1 == this.addIndex) {
e = d.default.GameRealWDActiveTips;
t = "GameRealWDActiveTips";
} else if (2 == this.addIndex) {
e = d.default.GameRealWDActiveTips;
t = "GameRealWDActiveTips";
i = 2;
} else if (3 == this.addIndex) {
e = d.default.GameRealWDADIDTips;
t = "GameRealWDADIDTips";
} else if (4 == this.addIndex) {
e = d.default.GameRealWDActiveTips;
t = "GameRealWDActiveTips";
} else if (5 == this.addIndex) {
e = d.default.GameRealWDTXTips;
t = "GameRealWDTXTips";
} else if (6 == this.addIndex) {
e = d.default.GameRealWDEmailOccupy;
t = "GameRealWDEmailOccupy";
} else {
if (8 != this.addIndex) {
let e = p.default.getlab("76");
h.default.show_toast({
text: e
});
return;
}
e = d.default.GameRealWDActiveTips;
t = "GameRealWDActiveTips";
i = 3;
}
this.addIndex += 1;
this.addIndex >= 8 && (this.addIndex = 0);
if (s.default.getInstance().getData(l.default).newFakeMoneyWithdraw[this.currentIndex] >= 4) f.default.open(); else {
const e = {
ui_config_path: d.default.GameRealWDTXTips,
ui_config_name: "GameRealWDTXTips",
param: {
currentIndex: this.currentIndex
}
};
h.default.show_ui(e);
}
m.default.instance.updateUI();
this.on_close_call();
}
};
i([ _(cc.Label) ], v.prototype, "titleLabel", void 0);
i([ _(cc.Label) ], v.prototype, "coinLabel", void 0);
i([ _(cc.Label) ], v.prototype, "realmissionLabel", void 0);
i([ _(cc.Label) ], v.prototype, "real_creditedLabel", void 0);
i([ _(cc.Label) ], v.prototype, "stageTips1", void 0);
i([ _(cc.Label) ], v.prototype, "stageTips2", void 0);
i([ _(cc.Label) ], v.prototype, "stageTips3", void 0);
i([ _(cc.Label) ], v.prototype, "stageTips4", void 0);
i([ _(sp.Skeleton) ], v.prototype, "sp1", void 0);
i([ _(sp.Skeleton) ], v.prototype, "sp2", void 0);
i([ _(sp.Skeleton) ], v.prototype, "sp3", void 0);
i([ _(cc.Label) ], v.prototype, "jxBtn", void 0);
v = i([ n.registerUIPath("GameDialog/GameRealTXYZ"), y ], v);
a.default = v;
cc._RF.pop();
};
