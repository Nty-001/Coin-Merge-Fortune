// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: GameUtils; dependency map: {"../BaseUIManager/Storage/GameLocalData":"GameLocalData","../BaseUIManager/Storage/PlayData":"PlayData","../BaseUIManager/Storage/Singleton":"Singleton","../LanguageControl/GameManagement":"GameManagement"}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "3eac2BSwNxI8pireFFshRVn", "GameUtils");
Object.defineProperty(a, "__esModule", {
value: !0
});
a.gameUtils = void 0;
const i = e("../BaseUIManager/Storage/GameLocalData"), o = e("../BaseUIManager/Storage/PlayData"), n = e("../BaseUIManager/Storage/Singleton"), s = e("../LanguageControl/GameManagement");
a.gameUtils = class extends n.default {
constructor() {
super(...arguments);
this.loginDaysStorageKey = "CoinMerge_LoginDaysRecord";
this.validLoginNeedMerge1024Number = 5;
this.defaultLotteryScoreConfig = [ {
lotteryCount: 0,
requiredScore: 50
} ];
this._clientId = "";
this.videoRewardConfig = [ {
min: 0,
max: 30,
qMin: .16,
qMax: .25
}, {
min: 31,
max: 50,
qMin: .14,
qMax: .22
}, {
min: 51,
max: 100,
qMin: .12,
qMax: .18
}, {
min: 101,
max: 200,
qMin: .09,
qMax: .15
}, {
min: 201,
max: 999999,
qMin: .08,
qMax: .13
} ];
}
set clientId(e) {
this._clientId = e;
}
get clientId() {
return this._clientId;
}
getTodayDateString() {
let e = new Date(), t = e.getMonth() + 1, a = e.getDate(), i = t < 10 ? "0" + t : t.toString(), o = a < 10 ? "0" + a : a.toString();
return e.getFullYear() + "-" + i + "-" + o;
}
recordLoginDays() {
const e = i.default.getInstance().getData(o.default);
let t = this.getTodayDateString(), a = 0, n = "", s = cc.sys.localStorage.getItem(this.loginDaysStorageKey);
if (s) try {
let e = JSON.parse(s);
a = Number(e.loginDays) || 0;
n = e.lastLoginDate || "";
} catch (e) {
console.log("parse login days record error", e);
}
a > e.loginDays && (e.loginDays = a);
!e.lastLoginDate && n && (e.lastLoginDate = n);
if (e.today1024NumberCoinDate != t) {
e.today1024NumberCoinDate = t;
e.today1024NumberCoin = 0;
}
let r = Math.max(0, Number(e.today1024NumberCoin) || 0), l = e.lastLoginDate != t && r >= this.validLoginNeedMerge1024Number;
e.isNewLoginDay = l;
if (l) {
e.loginDays = Math.max(0, Number(e.loginDays) || 0) + 1;
e.lastLoginDate = t;
}
cc.sys.localStorage.setItem(this.loginDaysStorageKey, JSON.stringify({
loginDays: e.loginDays,
lastLoginDate: e.lastLoginDate
}));
i.default.getInstance().set_local_storeage();
return {
loginDays: e.loginDays,
isNewLoginDay: l,
loginDate: e.lastLoginDate
};
}
addToday1024Number(e = 1) {
const t = i.default.getInstance().getData(o.default);
let a = this.getTodayDateString();
if (t.today1024NumberCoinDate != a) {
t.today1024NumberCoinDate = a;
t.today1024NumberCoin = 0;
}
let n = Math.max(0, Number(e) || 0);
t.today1024NumberCoin = Math.max(0, (Number(t.today1024NumberCoin) || 0) + n);
return this.recordLoginDays();
}
getLoginDays() {
const e = i.default.getInstance().getData(o.default);
return Math.max(0, Number(e.loginDays) || 0);
}
getLotteryScoreConfig() {
let e = (s.default.globalData || {}).lotteryScoreConfig;
return !e || e.length <= 0 ? this.defaultLotteryScoreConfig : e;
}
getCurrentDrawScore() {
const e = i.default.getInstance().getData(o.default);
let t = Math.max(0, Number(e.gameTotalScore) || 0);
e.gameTotalScore = t;
e.savedDrawScore = t;
return t;
}
addGameTotalScore(e) {
const t = i.default.getInstance().getData(o.default);
let a = this.getCurrentDrawScore() + Math.max(0, Number(e) || 0);
t.gameTotalScore = a;
t.savedDrawScore = a;
return a;
}
spendGameTotalScoreForLottery() {
const e = i.default.getInstance().getData(o.default);
let t = Math.max(0, Math.floor(Number(e.currentLotteryCount) || 0)), a = this.getRequiredScore(t), n = this.getCurrentDrawScore(), s = Math.max(0, n - a);
e.gameTotalScore = s;
e.savedDrawScore = s;
e.currentLotteryCount = t + 1;
i.default.getInstance().set_local_storeage();
return s;
}
getDefaultRealProductsConfig() {
return {
real_products: [ {
withdrawAmount: 500
}, {
withdrawAmount: 800
}, {
withdrawAmount: 1e3
}, {
withdrawAmount: 2e3
}, {
withdrawAmount: 3e3
}, {
withdrawAmount: 5e3
} ],
ranges: [ {
le: 0,
lh: 200,
ratio: 20
}, {
le: 200,
lh: 300,
ratio: 20
}, {
le: 300,
lh: 490,
ratio: 20
}, {
le: 490,
lh: 500,
ratio: 250
}, {
le: 500,
lh: 999999999999,
ratio: 40
} ],
fluctuation: {
min: .8,
max: 1.2
},
baseMinReward: .01,
newGuideReward: 220
};
}
getRealProductsConfig() {
let e = null;
try {
e = s.default.fake_products;
} catch (e) {
console.log("getRealProductsConfig error", e);
}
e || (e = this.getDefaultRealProductsConfig());
(!e.ranges || e.ranges.length <= 0) && (e.ranges = this.getDefaultRealProductsConfig().ranges);
e.fluctuation || (e.fluctuation = this.getDefaultRealProductsConfig().fluctuation);
null == e.baseMinReward && (e.baseMinReward = this.getDefaultRealProductsConfig().baseMinReward);
null == e.newGuideReward && (e.newGuideReward = this.getDefaultRealProductsConfig().newGuideReward);
return e;
}
getWithdrawAmounts(e) {
if (e.withdrawAmounts && e.withdrawAmounts.length > 0) return e.withdrawAmounts;
let t = e.real_products || [], a = [];
for (let e = 0; e < t.length; e++) {
let i = Number(t[e].withdrawAmount) || 0;
i > 0 && a.push(i);
}
return a.length <= 0 ? [ 500, 800, 1e3, 2e3, 3e3, 5e3 ] : a;
}
isMaxLotteryLevel() {
let e = this.getCurrentDrawScore(), t = this.getLotteryScoreConfig();
return e >= t[t.length - 1].requiredScore;
}
getAvailableLotteryTimes() {
const e = i.default.getInstance().getData(o.default);
let t = 0, a = this.getCurrentDrawScore(), n = e.currentLotteryCount;
for (;;) {
let e = this.getRequiredScore(n);
if (!(a >= e)) break;
t++;
a -= e;
n++;
}
return t;
}
getRequiredScore(e) {
let t = this.getLotteryScoreConfig();
for (let a = t.length - 1; a >= 0; a--) if (e >= t[a].lotteryCount) return t[a].requiredScore;
return t[0].requiredScore;
}
getNeedScoreAfterCurrentDraw(e, t) {
let a = Math.max(0, Number(e) || 0), i = Math.max(0, Number(t) || 0), o = this.getRequiredScore(i), n = Math.max(0, a - o), s = i + 1;
return this.checkLotteryStatus(n, s).needScore;
}
getRequiredScoreForNextLottery(e) {
return this.getRequiredScore(e);
}
checkLotteryStatus(e, t) {
let a = this.getRequiredScoreForNextLottery(t), i = Number(e) || 0;
return {
canLottery: i >= a,
requiredScore: a,
needScore: Math.max(0, a - i)
};
}
calculateCashReward() {
let e = i.default.getInstance().getData(o.default).fakeMoney, t = this.getRealProductsConfig();
if (!t) return 0;
let a = this.getWithdrawAmounts(t), n = -1, s = !0;
for (let t = 0; t < a.length; t++) if (e < a[t]) {
n = a[t];
s = !1;
break;
}
if (s) return Math.round(e / 100 * 100) / 100;
let r = (n - e) / this.getRatio(e, t.ranges) * (t.fluctuation.min + Math.random() * (t.fluctuation.max - t.fluctuation.min));
r = Math.max(r, t.baseMinReward);
return Math.round(100 * r) / 100;
}
getRatio(e, t) {
if (!t || t.length <= 0) return 20;
for (let a = 0; a < t.length; a++) if (e >= t[a].le && e < t[a].lh) return t[a].ratio;
return t[t.length - 1].ratio;
}
getNewGuideReward() {
let e = this.getRealProductsConfig();
return Number(e.newGuideReward) || 0;
}
getVideoRewardQ(e) {
for (let t = 0; t < this.videoRewardConfig.length; t++) {
let a = this.videoRewardConfig[t];
if (e >= a.min && e <= a.max) {
let e = a.qMin + Math.random() * (a.qMax - a.qMin);
return Math.round(100 * e) / 100;
}
}
let t = this.videoRewardConfig[this.videoRewardConfig.length - 1], a = t.qMin + Math.random() * (t.qMax - t.qMin);
return Math.round(100 * a) / 100;
}
getVideoGold() {
const e = i.default.getInstance().getData(o.default);
let t = e.watch_video_count;
return e.ecmp * this.getVideoRewardQ(t) * 1e4;
}
}.getInstance();
cc._RF.pop();
};
