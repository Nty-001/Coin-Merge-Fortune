// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: GameRealWDDialog; dependency map: {"../BaseUIManager/BaseUI":"BaseUI","../BaseUIManager/Storage/GameLocalData":"GameLocalData","../BaseUIManager/Storage/PlayData":"PlayData","../BaseUIManager/Storage/SoundManager":"SoundManager","../BaseUIManager/Storage/TouchButton":"TouchButton","../BaseUIManager/UIConfig":"UIConfig","../BaseUIManager/UIManagerNew":"UIManagerNew","../LanguageControl/GameManagement":"GameManagement","../LanguageControl/Lab":"Lab","../Report/NativeCall":"NativeCall","../game/GameWithdrawItem":"GameWithdrawItem"}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "f1d67UEXnlPRqBzqbYfRDiw", "GameRealWDDialog");
var i, o = this && this.__decorate || function(e, t, a, i) {
var o, n = arguments.length, s = n < 3 ? t : null === i ? i = Object.getOwnPropertyDescriptor(t, a) : i;
if ("object" == typeof Reflect && "function" == typeof Reflect.decorate) s = Reflect.decorate(e, t, a, i); else for (var r = e.length - 1; r >= 0; r--) (o = e[r]) && (s = (n < 3 ? o(s) : n > 3 ? o(t, a, s) : o(t, a)) || s);
return n > 3 && s && Object.defineProperty(t, a, s), s;
}, n = this && this.__awaiter || function(e, t, a, i) {
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
const s = e("../BaseUIManager/BaseUI"), r = e("../BaseUIManager/Storage/GameLocalData"), l = e("../BaseUIManager/Storage/PlayData"), c = e("../BaseUIManager/Storage/SoundManager"), d = e("../BaseUIManager/Storage/TouchButton"), h = e("../BaseUIManager/UIConfig"), u = e("../BaseUIManager/UIManagerNew"), g = e("../game/GameWithdrawItem"), p = e("../LanguageControl/GameManagement"), f = e("../LanguageControl/Lab"), m = e("../Report/NativeCall"), {ccclass: y, property: _} = cc._decorator;
let v = i = class extends s.default {
constructor() {
super(...arguments);
this.btnClose = null;
this.Withdrawal__title = null;
this.coinlNumberTxt1 = null;
this.coinlNumberTxt2 = null;
this.coinlNumberLabel1 = null;
this.coinlNumberLabel2 = null;
this.chooseMoneyLabel = null;
this.recordBtn = null;
this.helpBtn = null;
this.tixianBtnGray = null;
this.tixianBtn = null;
this.progressLabel = null;
this.progressBar = null;
this.currentProgressValue = 0;
this.bottomTips = null;
this.infoLabel1 = null;
this.items = null;
this.item = null;
this.platfroms = [];
this.currentIndex = 0;
this.allConditionsMet = !1;
this.playerData = null;
this.isWithdrawing = !1;
this.touchingButton = !0;
this.country = p.default.language;
this.platformSpr = [];
this.platformIndex = 0;
this.fakeMoney = 0;
}
onLoad() {
super.onLoad();
i.instance = this;
this.btnClose.addComponent(d.default).registerTouchEvent(() => {
this.on_close_call();
r.default.getInstance().saveToUserDefault();
});
this.tixianBtn.addComponent(d.default).registerTouchEvent(() => {
this.touchCoinWDAccount(!0);
});
this.tixianBtnGray.addComponent(d.default).registerTouchEvent(() => {
c.default.playSound("no_click");
});
}
touchCoinWDAccount(e) {
if (this.isWithdrawing) console.warn("提现正在进行中，请勿重复点击"); else if (e) {
this.isWithdrawing = !0;
m.default.getAdId();
this.country = p.default.normalizeCountry(p.default.language);
this.playerData = r.default.getInstance().getData(l.default);
let e = h.default.GameRealWDAccount, t = "GameRealWDAccount", a = this.hasCachedWithdrawAccount();
if (a || "BR" !== this.country) {
if (!a && this.isPhoneAccountCountry(this.country)) {
e = h.default.GameRealWDAccountID;
t = "GameRealWDAccountID";
}
} else {
e = h.default.GameRealWDAccountBR;
t = "GameRealWDAccountBR";
}
if (a) {
const e = {
ui_config_path: h.default.GameRealTXYZ,
ui_config_name: "GameRealTXYZ",
param: {
currentIndex: this.currentIndex,
account: ""
}
};
u.default.show_ui(e);
} else {
const a = {
ui_config_path: e,
ui_config_name: t,
param: {
currentIndex: this.currentIndex,
allConditionsMet: this.allConditionsMet
}
};
u.default.show_ui(a);
}
this.scheduleOnce(() => {
this.isWithdrawing = !1;
}, 3);
}
}
hasCachedWithdrawAccount() {
return !!this.playerData && !(!this.getSafeString(this.playerData.raccountName) || ("BR" === this.country ? !this.getSafeString(this.playerData.rfullName) || !this.getSafeString(this.playerData.rdocumentId) : this.isPhoneAccountCountry(this.country) && !this.getSafeString(this.playerData.rfullName)));
}
isPhoneAccountCountry(e) {
return "ID" === e || "TH" === e || "MY" === e || "VN" === e || "PH" === e;
}
getSafeString(e) {
return null == e ? "" : e.toString().trim();
}
getConfig() {
return p.default.newfake_products;
}
show(e) {
super.show(e);
e.param;
this.Withdrawal__title.string = f.default.getlab("8");
this.tixianBtn.getChildByName("btnLabel").getComponent(cc.Label).string = f.default.getlab("8");
this.tixianBtnGray.getChildByName("btnLabel").getComponent(cc.Label).string = f.default.getlab("8");
this.coinlNumberLabel1.string = f.default.getlab("9");
this.coinlNumberLabel2.string = f.default.getlab("10");
this.playerData = r.default.getInstance().getData(l.default);
this.coinlNumberTxt2.string = this.playerData.coin1024Number + "";
this.infoLabel1.string = f.default.getlab("11");
this.chooseMoneyLabel.string = f.default.getlab("105");
this.showReward();
this.fakeMoney = this.playerData.fakeMoney;
}
updateUI() {
let e = this.getConfig().new_Fake_products;
for (let t = 0; t < e.length; t++) {
let a = e[t];
const i = this.items.children[t];
if (i) {
const e = i.getComponent(g.default);
let o = this.currentIndex === t;
e.refreshSelectBGSprite(o);
e.refreshEnoughBGSprite(!o);
if (o) {
this.currentProgressValue = a.withdrawAmount;
this.setProgress(t);
}
}
}
}
onitemWithdraw(e) {
if (e !== this.currentIndex) {
this.currentIndex = e;
this.updateUI();
}
}
showReward() {
var e;
this.items.removeAllChildren();
let t = this.getConfig().new_Fake_products;
for (let a = 0; a < t.length; a++) {
let i = a;
const o = cc.instantiate(this.item);
o.parent = this.items;
const n = o.getComponent(g.default);
let s = (null === (e = t[a]) || void 0 === e ? void 0 : e.withdrawAmount) || 0;
n.initdata(s);
let r = s > this.playerData.money;
n.refreshEnoughBGSprite(r);
n.refreshSelectBGSprite(!1);
if (0 == a) {
this.currentIndex = a;
n.refreshSelectBGSprite(!0);
n.refreshEnoughBGSprite(!1);
this.currentProgressValue = s;
this.setProgress(a);
}
let l = o.getComponent(d.default);
l || (l = o.addComponent(d.default));
l && l.registerTouchEvent(() => {
this.onitemWithdraw(i);
});
}
}
setProgress(e) {
let t = this.playerData.newFakeMoneyWithdraw[e], a = this.getConfig().new_Fake_products[e];
if (0 == t) {
this.progressBar.progress = this.playerData.coin1024Number / a.condition_merge;
let e = Math.max(0, a.condition_merge - this.playerData.coin1024Number);
this.bottomTips.string = f.default.getlab("106").replace("%{0}", a.condition_merge + "").replace("%{0}", e + "");
} else if (1 == t) {
this.progressBar.progress = this.playerData.watch_video_count / a.condition_ad;
let e = Math.max(a.condition_ad - this.playerData.watch_video_count, 0);
this.bottomTips.string = f.default.getlab("107").replace("%{0}", e + "");
} else if (2 == t) {
this.progressBar.progress = this.playerData.fakeMoney / a.withdrawAmount;
let e = Math.max(a.withdrawAmount - this.playerData.fakeMoney, 0);
this.bottomTips.string = f.default.getlab("56").replace("%{0}", e + "");
} else if (3 == t) {
let e = a.condition_login_days > 0 ? this.playerData.loginDays / a.condition_login_days : 1;
this.progressBar.progress = e;
let t = Math.max(a.condition_login_days - this.playerData.loginDays, 0), i = a.condition_daily_merge;
this.bottomTips.string = f.default.getlab("60").replace("%{0}", t + "").replace("%{1}", i + "");
} else if (4 == t) {
this.progressBar.progress = this.playerData.watch_video_count / a.condition_video;
let e = Math.max(a.condition_video - this.playerData.watch_video_count, 0);
this.bottomTips.string = f.default.getlab("61").replace("%{0}", e + "");
} else if (t >= 5) {
this.progressBar.progress = 1;
this.bottomTips.string = f.default.getlab("112");
}
this.progressBar.progress = cc.misc.clamp01(this.progressBar.progress);
this.progressLabel && (this.progressLabel.string = Math.floor(100 * this.progressBar.progress) + "%");
this.allConditionsMet = this.progressBar.progress >= 1;
if (this.allConditionsMet) {
this.tixianBtn.active = !0;
this.tixianBtnGray.active = !1;
} else {
this.tixianBtn.active = !1;
this.tixianBtnGray.active = !0;
}
}
showRewardView() {
return n(this, void 0, void 0, function*() {});
}
};
o([ _(cc.Node) ], v.prototype, "btnClose", void 0);
o([ _(cc.Label) ], v.prototype, "Withdrawal__title", void 0);
o([ _(cc.Label) ], v.prototype, "coinlNumberTxt1", void 0);
o([ _(cc.Label) ], v.prototype, "coinlNumberTxt2", void 0);
o([ _(cc.Label) ], v.prototype, "coinlNumberLabel1", void 0);
o([ _(cc.Label) ], v.prototype, "coinlNumberLabel2", void 0);
o([ _(cc.Label) ], v.prototype, "chooseMoneyLabel", void 0);
o([ _(cc.Node) ], v.prototype, "recordBtn", void 0);
o([ _(cc.Node) ], v.prototype, "helpBtn", void 0);
o([ _(cc.Node) ], v.prototype, "tixianBtnGray", void 0);
o([ _(cc.Node) ], v.prototype, "tixianBtn", void 0);
o([ _(cc.Label) ], v.prototype, "progressLabel", void 0);
o([ _(cc.ProgressBar) ], v.prototype, "progressBar", void 0);
o([ _(cc.Label) ], v.prototype, "bottomTips", void 0);
o([ _(cc.Label) ], v.prototype, "infoLabel1", void 0);
o([ _(cc.Node) ], v.prototype, "items", void 0);
o([ _(cc.Prefab) ], v.prototype, "item", void 0);
o([ _([ cc.Sprite ]) ], v.prototype, "platfroms", void 0);
v = i = o([ s.registerUIPath("GameDialog/GameRealWDDialog"), y ], v);
a.default = v;
cc._RF.pop();
};
