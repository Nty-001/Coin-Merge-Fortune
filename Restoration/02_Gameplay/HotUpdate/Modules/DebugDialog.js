// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: DebugDialog; dependency map: {"../BaseUIManager/BaseUI":"BaseUI","../BaseUIManager/Storage/GameLocalData":"GameLocalData","../BaseUIManager/Storage/PlayData":"PlayData","../BaseUIManager/Storage/TouchButton":"TouchButton","../GameScene":"GameScene","./GameManagement":"GameManagement"}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "6347ftpZZJA/bKJMiTqADeR", "DebugDialog");
var i = this && this.__decorate || function(e, t, a, i) {
var o, n = arguments.length, s = n < 3 ? t : null === i ? i = Object.getOwnPropertyDescriptor(t, a) : i;
if ("object" == typeof Reflect && "function" == typeof Reflect.decorate) s = Reflect.decorate(e, t, a, i); else for (var r = e.length - 1; r >= 0; r--) (o = e[r]) && (s = (n < 3 ? o(s) : n > 3 ? o(t, a, s) : o(t, a)) || s);
return n > 3 && s && Object.defineProperty(t, a, s), s;
};
Object.defineProperty(a, "__esModule", {
value: !0
});
const o = e("../BaseUIManager/BaseUI"), n = e("../BaseUIManager/Storage/GameLocalData"), s = e("../BaseUIManager/Storage/PlayData"), r = e("../BaseUIManager/Storage/TouchButton"), l = e("../GameScene"), c = e("./GameManagement"), {ccclass: d, property: h} = cc._decorator;
let u = class extends o.default {
constructor() {
super(...arguments);
this.countryEdit = null;
this.input_coin = null;
this.input_level = null;
this.input_prop = null;
this.input_login = null;
this.input_video = null;
this.input_daily_coin = null;
this.input_daily_success = null;
this.input_chengjiupoint = null;
this.input_score = null;
this.input_drawScore = null;
this.input_lucky = null;
this.input_wheel = null;
this.input_active_daily = null;
this.input_shoujiwu = null;
this.input_vibrator = null;
this.input_realCoin = null;
this.input_1024 = null;
this.input_merge_Coin = null;
this.confrome_btn = null;
this.close_btn = null;
this.restart_btn = null;
this.currentCountry = "US";
this.originalScoreInput = "";
this.originalDrawScoreInput = "";
}
onLoad() {
(this.close_btn.getComponent(r.default) || this.close_btn.addComponent(r.default)).registerTouchEvent(() => {
this.on_close_call();
});
(this.confrome_btn.getComponent(r.default) || this.confrome_btn.addComponent(r.default)).registerTouchEvent(() => {
this.refreshData(!1);
});
(this.restart_btn.getComponent(r.default) || this.restart_btn.addComponent(r.default)).registerTouchEvent(() => {
this.refreshData(!0);
});
}
show(e) {
super.show(e);
const t = n.default.getInstance().getData(s.default);
let a = c.default.language;
this.setEditString(this.countryEdit, a);
this.currentCountry = a;
this.setEditString(this.input_coin, t.fakeMoney);
this.setEditString(this.getScoreEditBox(), t.roundScore);
this.setEditString(this.getCoin1024EditBox(), t.coin1024Number);
this.setEditString(this.input_login, t.loginDays);
this.setEditString(this.input_video, t.watch_video_count);
this.setEditString(this.getDrawScoreEditBox(), t.gameTotalScore);
this.setEditString(this.input_wheel, t.currentLotteryCount);
this.originalScoreInput = this.getEditString(this.getScoreEditBox());
this.originalDrawScoreInput = this.getEditString(this.getDrawScoreEditBox());
}
baseOnDisable() {}
refreshData(e) {
const t = n.default.getInstance().getData(s.default);
t.fakeMoney = this.getEditNumber(this.input_coin, t.fakeMoney);
t.roundScore = this.getEditNumber(this.getScoreEditBox(), t.roundScore);
t.savedCoinsScore = t.roundScore;
t.coin1024Number = this.getEditNumber(this.getCoin1024EditBox(), t.coin1024Number);
t.loginDays = this.getEditNumber(this.input_login, t.loginDays);
t.watch_video_count = this.getEditNumber(this.input_video, t.watch_video_count);
t.today1024NumberCoin = this.getEditNumber(this.input_daily_coin, t.today1024NumberCoin);
this.applyDrawScoreDebugValue(t);
let a = this.getEditNumber(this.input_realCoin, t.UseRevenue);
t.UseRevenue = a;
t.money = a;
t.newFakeMoneyWithdraw = this.getEditNumberArray(this.input_active_daily, t.newFakeMoneyWithdraw);
this.fillNumberArray(t.watch_video_Singlecount, t.watch_video_count);
this.fillNumberArray(t.real_watch_video_Singlecount, t.watch_video_count);
let i = this.countryEdit ? this.countryEdit.string : this.currentCountry;
this.saveDebugCountry(i);
n.default.getInstance().set_local_storeage();
let o = e || this.currentCountry != i;
o && n.default.getInstance().clear_data();
this.scheduleOnce(() => {
this.refreshGame();
this.on_close_call();
if (o) {
n.default.getInstance().clear_data();
cc.game.end();
}
}, .02);
}
refreshGame() {
n.default.getInstance().saveToUserDefault();
let e = l.default.instance;
if (!e || !e.node || !e.node.isValid) return;
const t = n.default.getInstance().getData(s.default);
e.score = t.roundScore;
e.setFakeMoney();
e.updateProgressbar();
e.set1024Label();
}
setEditString(e, t) {
e && (e.string = null == t ? "" : t + "");
}
getScoreEditBox() {
return this.input_score || this.input_level;
}
getDrawScoreEditBox() {
return this.input_drawScore || this.input_lucky;
}
getCoin1024EditBox() {
return this.input_1024 || this.input_prop;
}
getEditString(e) {
return e && null != e.string ? e.string : "";
}
getEditNumber(e, t = 0) {
if (!e || null == e.string || "" === e.string) return t;
let a = Number(e.string);
return isNaN(a) ? t : a;
}
getEditNumberArray(e, t) {
if (!e || !e.string) return t;
let a = e.string.trim();
if ("" === a) return t;
if (a.indexOf(",") < 0) {
let e = Number(a);
return isNaN(e) ? t : t.map(() => e);
}
let i = a.split(",").map(e => {
let t = Number(e.trim());
return isNaN(t) ? 0 : t;
});
for (;i.length < t.length; ) i.push(t[i.length] || 0);
return i.slice(0, t.length);
}
fillNumberArray(e, t) {
if (Array.isArray(e)) for (let a = 0; a < e.length; a++) e[a] = t;
}
applyDrawScoreDebugValue(e) {
let t = this.getDrawScoreEditBox();
if (t && this.getEditString(t) != this.originalDrawScoreInput) {
let a = Math.max(0, this.getEditNumber(t, e.gameTotalScore));
e.gameTotalScore = a;
e.savedDrawScore = a;
} else {
e.gameTotalScore = Math.max(0, Number(e.gameTotalScore) || 0);
e.savedDrawScore = e.gameTotalScore;
}
}
saveDebugCountry(e) {
e && cc.sys.localStorage.setItem("coin_country", e);
}
};
i([ h(cc.EditBox) ], u.prototype, "countryEdit", void 0);
i([ h(cc.EditBox) ], u.prototype, "input_coin", void 0);
i([ h(cc.EditBox) ], u.prototype, "input_level", void 0);
i([ h(cc.EditBox) ], u.prototype, "input_prop", void 0);
i([ h(cc.EditBox) ], u.prototype, "input_login", void 0);
i([ h(cc.EditBox) ], u.prototype, "input_video", void 0);
i([ h(cc.EditBox) ], u.prototype, "input_daily_coin", void 0);
i([ h(cc.EditBox) ], u.prototype, "input_daily_success", void 0);
i([ h(cc.EditBox) ], u.prototype, "input_chengjiupoint", void 0);
i([ h(cc.EditBox) ], u.prototype, "input_score", void 0);
i([ h(cc.EditBox) ], u.prototype, "input_drawScore", void 0);
i([ h(cc.EditBox) ], u.prototype, "input_lucky", void 0);
i([ h(cc.EditBox) ], u.prototype, "input_wheel", void 0);
i([ h(cc.EditBox) ], u.prototype, "input_active_daily", void 0);
i([ h(cc.EditBox) ], u.prototype, "input_shoujiwu", void 0);
i([ h(cc.EditBox) ], u.prototype, "input_vibrator", void 0);
i([ h(cc.EditBox) ], u.prototype, "input_realCoin", void 0);
i([ h(cc.EditBox) ], u.prototype, "input_1024", void 0);
i([ h(cc.EditBox) ], u.prototype, "input_merge_Coin", void 0);
i([ h(cc.Node) ], u.prototype, "confrome_btn", void 0);
i([ h(cc.Node) ], u.prototype, "close_btn", void 0);
i([ h(cc.Node) ], u.prototype, "restart_btn", void 0);
u = i([ o.registerUIPath("GameDialog/DebugDialog"), d ], u);
a.default = u;
cc._RF.pop();
};
