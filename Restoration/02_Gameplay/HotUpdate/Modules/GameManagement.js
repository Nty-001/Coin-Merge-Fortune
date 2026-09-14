// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: GameManagement; dependency map: {"../BaseUIManager/FlyAnimation":"FlyAnimation","../BaseUIManager/Storage/GameLocalData":"GameLocalData","../BaseUIManager/Storage/NewGamePlayData":"NewGamePlayData","../BaseUIManager/Storage/PlayData":"PlayData","../HWL/ServerConfig":"ServerConfig"}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "bda10BvToFIobdRuKG5cJ4w", "GameManagement");
var i = this && this.__awaiter || function(e, t, a, i) {
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
a.LANGUAGE_ID = void 0;
const o = e("../BaseUIManager/FlyAnimation"), n = e("../BaseUIManager/Storage/GameLocalData"), s = e("../BaseUIManager/Storage/NewGamePlayData"), r = e("../BaseUIManager/Storage/PlayData"), l = e("../HWL/ServerConfig");
class c extends cc.Component {
static normalizeCountry(e) {
if (!e) return "US";
let t = e.toString().trim().toUpperCase();
return "CN" == t ? "US" : t && Object.keys(a.LANGUAGE_ID).some(e => a.LANGUAGE_ID[e] == t) ? t : "US";
}
static get globalData() {
let e = c.all_config_data.GameData;
if (!e) {
e = {};
c.all_config_data.GameData = e;
}
return c.normalizeBlastConfig(e);
}
static getDefaultGlobalConfig() {
return {
guide_coins: 600,
show_CN: !1,
not_force: [],
force_version: [],
coin_mubiaoA: [ "0", "1", "2", "3", "4" ],
coin_mubiaoB: [ "5", "6", "7", "8", "9" ],
cashTime: [ [ 0, 15, 15 ], [ 6, 10, 20 ], [ 11, 8, 25 ], [ 31, 6, 25 ], [ 51, 5, 30 ] ],
cashCoins: 990,
tixian_products: [ {
id: 1,
withdrawAmount: 1e3,
withdrawCount: 1,
condition_2: 25,
condition_3: 200,
condition_4: 600,
condition_5: 25e4
}, {
id: 2,
withdrawAmount: 1500,
withdrawCount: 1,
condition_2: 25,
condition_3: 200,
condition_4: 600,
condition_5: 25e4
}, {
id: 3,
withdrawAmount: 2e3,
withdrawCount: 1,
condition_2: 25,
condition_3: 200,
condition_4: 600,
condition_5: 25e4
}, {
id: 4,
withdrawAmount: 3e3,
withdrawCount: 1,
condition_2: 25,
condition_3: 200,
condition_4: 600,
condition_5: 25e4
}, {
id: 5,
withdrawAmount: 4e3,
withdrawCount: 1,
condition_2: 25,
condition_3: 200,
condition_4: 600,
condition_5: 25e4
}, {
id: 6,
withdrawAmount: 5e3,
withdrawCount: 1,
condition_2: 25,
condition_3: 200,
condition_4: 600,
condition_5: 25e4
}, {
id: 7,
withdrawAmount: 6e3,
withdrawCount: 1,
condition_2: 25,
condition_3: 200,
condition_4: 600,
condition_5: 25e4
}, {
id: 8,
withdrawAmount: 8e3,
withdrawCount: 1,
condition_2: 25,
condition_3: 200,
condition_4: 600,
condition_5: 25e4
} ],
success_target_levels: [ [ 0, 2 ], [ 10, 2 ], [ 31, 2 ] ],
reward_coins: [ [ 600, 50 ], [ 800, 60 ], [ 990, 70 ], [ 1e3, 250 ], [ 99999999, 100 ] ],
reward_multiplier: [ [ 0, 5 ], [ .4, 5 ], [ .5, 5 ], [ .8, 5 ], [ 1, 5 ], [ 1, 5 ] ],
collect_progress_data: .2,
ui_delay_data: 45,
real_init_coin: 50,
real_addvideo: [ [ 30, [ .35, .4 ] ], [ 31, [ .28, .38 ] ], [ 51, [ .25, .3 ] ], [ 101, [ .18, .3 ] ], [ 201, [ .15, .25 ] ] ]
};
}
static normalizeBlastConfig(e) {
let t = c.getDefaultGlobalConfig();
e.global || (e.global = {});
for (let a in t) null == e.global[a] && (e.global[a] = t[a]);
let a = {
type1: .01,
type2: .01,
type3: .1,
type4: 1,
type5: 1,
type6: 20
};
for (let t in a) {
e[t] || (e[t] = {});
null == e[t].reward_base_min && (e[t].reward_base_min = a[t]);
}
return e;
}
static GetadaptForTallDevices() {
const e = cc.view.getFrameSize().height / cc.view.getFrameSize().width;
console.log("屏幕宽高比:", e, cc.view.getFrameSize());
return e > 1.9;
}
static loadSpriteFrame(e, t) {
cc.loader.loadRes(e, cc.SpriteFrame, function(e, a) {
e ? console.log("UIUtils::loadSpriteFrame error " + e, a) : t(a);
});
}
static loadRandomPlatformSpriteFrame(e) {
cc.loader.loadResDir("texture/platform", cc.SpriteFrame, function(t, a) {
if (t) console.log("UIUtils::loadRandomPlatformSpriteFrame error " + t); else if (a && a.length > 0) {
const t = Math.floor(Math.random() * a.length);
e(a[t]);
} else console.log("UIUtils::loadRandomPlatformSpriteFrame no sprite frames found");
});
}
static getCountryIdByCountry(e) {
e = c.normalizeCountry(e);
let t = Object.entries(a.LANGUAGE_ID);
for (const [a, i] of t) if (i == e) return Number(a);
return 6;
}
static initJsonData() {
return i(this, void 0, void 0, function*() {
const e = [];
null == this.languageJson && e.push(new Promise((e, t) => {
cc.resources.load("config/language", (a, i) => {
if (a) {
cc.error("JSON 加载失败:", a);
t(a);
} else {
this.languageJson = i.json;
e();
}
});
}));
null == this.detailsData && e.push(new Promise((e, t) => {
cc.resources.load("config/detailsData", (a, i) => {
if (a) {
cc.error("JSON 加载失败:", a);
t(a);
} else {
this.detailsData = i.json;
e();
}
});
}));
null == this.data && e.push(new Promise((e, t) => {
cc.resources.load("config/data", (a, i) => {
if (a) {
cc.error("JSON 加载失败:", a);
t(a);
} else {
this.data = i.json;
e();
}
});
}));
yield Promise.all(e);
});
}
static GetCountryDang() {
let e = 1;
switch (this.GetJsonDataIndex()) {
case 1:
e = 4;
break;

case 2:
e = 10;
break;

case 3:
e = 100;
break;

case 4:
e = 400;
break;

case 5:
e = 2e4;
break;

default:
e = 1;
}
return e;
}
static getnor(e = 1, t = !1) {
this.birate = this.GetCountryDang();
this.lan = this.GetCountryIndewx();
let a = n.default.getInstance().getData(s.default);
this.mo = a.red_bag;
this.mons = this.globalData.global.tixian_products;
this.dangs = this.globalData.global.reward_coins;
let i = 0, o = .4 * Math.random() + .8;
console.log("获取标准奖励值 0       随机  ", this.mons, o, "最近档位的值 ", this.getlastTXmon(), "当前金额 ", this.mo, " 差值 ", this.getlastTXmon() - this.mo, " 奖励系数 ", this.getXS(), "倍数 ", e);
i = 0 != this.getlastTXmon() ? (this.getlastTXmon() - this.mo) / this.getXS() * o : this.mo / 100;
i *= e;
console.log("获取标准奖励值 1     ", i, t);
i < this.get_reward_base_min() && (i = this.get_reward_base_min());
e >= 1 && (i *= e);
let r = this.roundToDecimal(i, 2);
console.log("获取标准奖励值 end     ", r);
return r;
}
static getRandomInRange(e, t) {
const a = Math.random() * (t - e) + e;
return this.roundToDecimal(a, 2);
}
static getlastTXmon() {
let e = 0;
for (let t = 0; t < this.mons.length; t++) if (this.mons[t].withdrawAmount * this.birate > this.mo) {
e = this.mons[t].withdrawAmount * this.birate;
break;
}
return this.roundToDecimal(e, 2);
}
static getXS() {
let e = this.dangs.length - 1;
for (let t = 0; t < this.dangs.length; t++) if (this.dangs[t][0] * this.birate > this.mo) {
e = t;
break;
}
return this.dangs[e][1];
}
static GetCountryIndewx() {
let e = c.language;
return this.getCountryIdByCountry(e);
}
static GetCountry() {
let e = c.language;
return this.getCountryIdByCountry(e);
}
static GetJsonDataIndex() {
switch (c.normalizeCountry(c.language)) {
case "US":
case "GB":
case "CA":
case "DE":
case "FR":
case "AU":
case "BR":
case "MY":
case "TR":
return 0;

case "MX":
case "ZA":
case "EG":
return 1;

case "RU":
case "IN":
case "PH":
case "TH":
case "PK":
case "BD":
return 2;

case "NG":
case "JP":
case "AR":
case "KZ":
return 3;

case "CO":
case "KR":
return 4;

case "ID":
case "VN":
return 5;

default:
return 0;
}
}
static getNextDataNumber(e, t) {
for (let a = 0; a < t.length; a++) if (e < t[a] * this.GetCountryDang()) return 0 === a ? t[0] * this.GetCountryDang() : t[a] * this.GetCountryDang();
return t[t.length - 1] * this.GetCountryDang();
}
static getCurrentJsonData(e, t) {
if (!t || 0 === t.length) return 0;
for (let a = t.length - 1; a >= 0; a--) if (e >= t[a][0]) return t[a];
return t[0];
}
static getCurrentRewardMultiplier(e, t) {
let a = this.globalData.global.reward_multiplier;
if (e < a[1][0] * t) return a[0][1];
if (e > t) return a[a.length - 1][1];
for (let i = a.length - 1; i >= 1; i--) if (e >= a[i][0] * t) return a[i][1];
return a[a.length - 1][1];
}
static getCurrentFirstJsonData(e, t) {
let a = t[0];
for (let i = 0; i < t.length; i++) if (e <= t[i][0]) {
a = t[i];
break;
}
return a;
}
static getmonstr(e) {
let t = "", i = c.language, o = "", n = "", s = this.languageJson[a.LANGUAGE_ID[this.getCountryIdByCountry(i)]].country;
switch (this.getCountryIdByCountry(i)) {
case 1:
case 12:
t = s + (t = this.roundToDecimal(e, 0).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
break;

case 13:
t = s + (t = this.toFixed(e, 2).toString());
break;

case 15:
t = this.roundToDecimal(e, 2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
t += s;
break;

case 16:
t = this.roundToDecimal(e, 0).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
t += s;
break;

case 17:
t = (n = (o = this.roundToDecimal(e, 0).toString()).replace(/\./g, ",")).replace(/\B(?=(\d{3})+(?!\d))/g, " ");
t += s;
break;

case 19:
case 20:
t = s + (t = this.roundToDecimal(e, 0).toString().replace(/\B(?=(\d{3})+(?!\d))/g, "."));
break;

case 22:
t = (n = (o = this.roundToDecimal(e, 2).toString()).replace(/\./g, ",")).replace(/\B(?=(\d{3})+(?!\d))/g, " ");
t += s;
break;

case 23:
t = s + (t = this.roundToDecimal(e, 2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, "."));
break;

case 24:
t = (o = this.roundToDecimal(e, 0).toString()).replace(/\B(?=(\d{3})+(?!\d))/g, ",");
t += s;
break;

case 25:
t = s + (t = (o = this.roundToDecimal(e, 0).toString()).replace(/\B(?=(\d{3})+(?!\d))/g, ","));
break;

case 28:
t = s + (t = (n = (o = this.roundToDecimal(e, 2).toString()).replace(/\./g, ",")).replace(/\B(?=(\d{3})+(?!\d))/g, "."));
break;

case 30:
case 31:
case 32:
default:
t = s + (t = this.roundToDecimal(e, 2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
}
return t;
}
static getRealMonstr(e) {
let t = "", a = c.language, i = "", o = "", n = this.countryCashName;
switch (this.getCountryIdByCountry(a)) {
case 1:
t = n + (t = this.roundToDecimal(e, 0).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
break;

case 2:
t = n + (t = this.toFixed(e, 2).toString());
break;

case 4:
t = n + (t = this.roundToDecimal(e, 2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
break;

case 5:
t = this.roundToDecimal(e, 2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
t += n;
break;

case 16:
t = this.roundToDecimal(e, 0).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
t += n;
break;

case 3:
t = (o = (i = this.roundToDecimal(e, 2).toString()).replace(/\./g, ",")).replace(/\B(?=(\d{3})+(?!\d))/g, " ");
t += n;
break;

default:
t = n + (t = this.roundToDecimal(e, 2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
}
return t;
}
static getPlayerCoin(e) {
let t = 0, a = c.language;
switch (this.getCountryIdByCountry(a)) {
case 1:
case 12:
case 20:
case 21:
case 14:
case 25:
case 16:
case 17:
case 24:
case 19:
t = 0;
break;

case 2:
case 3:
case 27:
case 28:
case 4:
case 5:
case 6:
case 10:
case 11:
case 15:
case 18:
case 26:
case 29:
case 30:
case 7:
case 8:
case 32:
case 33:
case 22:
case 23:
case 31:
case 13:
default:
t = 2;
}
return t;
}
static formatStr(e) {
let t = String(e), [a, i] = t.split(".");
if (a.length <= 3) return a;
let o = a.substring(0, a.length - 3), n = a.substring(a.length - 3, a.length), s = o.replace(/\B(?=(\d{2})+(?!\d))/g, ",");
return i ? `${s},${n}.${i}` : `${s},${n}`;
}
static toFixed(e, t) {
return parseFloat(e.toFixed(t + 1).slice(0, -1));
}
static roundToDecimal(e, t) {
return Number((Math.floor(100 * e) / 100).toFixed(t));
}
static getMaxNumber(e, t) {
return Math.max(e, t);
}
static getPlayerUseCoin(e) {
if (e < 1e4) return e.toString();
if (e < 1e6) {
const t = e / 1e3;
return Number.isInteger(t) ? `${t}k` : `${t.toFixed(1)}k`;
}
if (e < 1e9) {
const t = e / 1e6;
return Number.isInteger(t) ? `${t}M` : `${t.toFixed(1)}M`;
}
{
const t = e / 1e9;
return Number.isInteger(t) ? `${t}B` : `${t.toFixed(1)}B`;
}
}
static get countryCashName() {
const e = c.normalizeCountry(c.language);
return "BR" === e ? "R$" : "ID" === e ? "Rp" : "TH" === e ? "฿" : "MY" === e ? "RM " : "VN" === e ? "₫" : "PH" === e ? "₱" : "$";
}
static get realConfig() {
const e = c.normalizeCountry(c.language), t = this.globalData, a = t.global || {};
let i = t.US;
"BR" == e ? i = t.BR : "ID" == e ? i = t.ID : "TH" == e ? i = t.TH : "MY" == e ? i = t.MY : "VN" == e ? i = t.VN : "PH" == e && (i = t.PH);
const o = Object.assign({}, a, i || {});
null == o.real_init_coin && (o.real_init_coin = a.real_init_coin);
Array.isArray(o.real_addvideo) && 0 !== o.real_addvideo.length || (o.real_addvideo = a.real_addvideo);
return o;
}
static get real_products() {
const e = c.normalizeCountry(c.language);
return "VN" == e ? [ c.data.VN, c.data.paypal ] : "TH" == e ? [ c.data.TH, c.data.paypal ] : "ID" == e ? [ c.data.ID ] : "BR" == e ? [ c.data.BR ] : [ c.data.paypal ];
}
static get newfake_products() {
const e = c.normalizeCountry(c.language);
let t = this.globalData.groupNew1;
"US" == e || "GB" == e || "CA" == e || "DE" == e || "FR" == e || "AU" == e || "BR" == e || "MY" == e || "TR" == e ? t = this.globalData.groupNew1 : "MX" == e || "ZA" == e || "EG" == e ? t = this.globalData.groupNew2 : "RU" == e || "IN" == e || "PH" == e || "TH" == e || "PK" == e || "BD" == e ? t = this.globalData.groupNew3 : "NG" == e || "JP" == e || "AR" == e || "KZ" == e ? t = this.globalData.groupNew4 : "CO" == e || "KR" == e ? t = this.globalData.groupNew5 : "VN" != e && "ID" != e || (t = this.globalData.groupNew6);
return t;
}
static get fake_products() {
const e = c.normalizeCountry(c.language);
let t = this.globalData.group1;
"US" == e || "GB" == e || "CA" == e || "DE" == e || "FR" == e || "AU" == e || "BR" == e || "MY" == e || "TR" == e ? t = this.globalData.group1 : "MX" == e || "ZA" == e || "EG" == e ? t = this.globalData.group2 : "RU" == e || "IN" == e || "PH" == e || "TH" == e || "PK" == e || "BD" == e ? t = this.globalData.group3 : "NG" == e || "JP" == e || "AR" == e || "KZ" == e ? t = this.globalData.group4 : "CO" == e || "KR" == e ? t = this.globalData.group5 : "VN" != e && "ID" != e || (t = this.globalData.group6);
return t;
}
static getPlatformSpr() {
let e = c.normalizeCountry(c.language), t = [ l.RealCashPlatform.PayPal ];
if ("BR" == e) {
let e = c.globalData.BR.withdrawal_platform;
t = null == e || e && 0 === e.length ? [ l.RealCashPlatform.Pagbank ] : e.map(e => "Pagbank" === e ? l.RealCashPlatform.Pagbank : "PIX" === e ? l.RealCashPlatform.PIX : null).filter(Boolean);
} else if ("ID" == e) {
let e = c.globalData.ID.withdrawal_platform;
t = null == e || e && 0 === e.length ? [ l.RealCashPlatform.DANA ] : e.map(e => "DANA" === e ? l.RealCashPlatform.DANA : "OVO" === e ? l.RealCashPlatform.OVO : null).filter(Boolean);
} else if ("TH" == e) {
let e = c.globalData.TH.withdrawal_platform;
t = null == e || e && 0 === e.length ? [ l.RealCashPlatform.PayPal ] : e.map(e => "Truemoney" === e ? l.RealCashPlatform.Truemoney : null).filter(Boolean);
} else if ("MY" == e) {
let e = c.globalData.MY.withdrawal_platform;
t = null == e || e && 0 === e.length ? [ l.RealCashPlatform.PayPal ] : e.map(e => "TNG" === e ? l.RealCashPlatform.TNG : null).filter(Boolean);
} else if ("VN" == e) {
let e = c.globalData.VN.withdrawal_platform;
t = null == e || e && 0 === e.length ? [ l.RealCashPlatform.PayPal ] : e.map(e => "ZaloPay" === e ? l.RealCashPlatform.ZaloPay : null).filter(Boolean);
} else if ("PH" == e) {
let e = c.globalData.PH.withdrawal_platform;
t = null == e || e && 0 === e.length ? [ l.RealCashPlatform.PayPal ] : e.map(e => "GCash" === e ? l.RealCashPlatform.GCash : "Graboay" === e ? l.RealCashPlatform.Graboay : "Paymaya" === e ? l.RealCashPlatform.Paymaya : null).filter(Boolean);
}
console.log("真实可用提现平台  ", t);
return t;
}
static getPlayDataWithdrawRecords() {
let e = n.default.getInstance().getData(s.default), t = e.withdrawRecords, a = this.cashIndex1;
if (e.realSelectPlatform == l.RealCashPlatform.PIX) {
t = e.withdrawRecords2;
a = this.cashIndex2;
}
if (e.realSelectPlatform == l.RealCashPlatform.OVO) {
t = e.withdrawRecords2;
a = this.cashIndex2;
}
if (e.realSelectPlatform == l.RealCashPlatform.Graboay) {
t = e.withdrawRecords2;
a = this.cashIndex2;
}
if (e.realSelectPlatform == l.RealCashPlatform.Graboay) {
t = e.withdrawRecords2;
a = this.cashIndex2;
}
if (e.realSelectPlatform == l.RealCashPlatform.Paymaya) {
t = e.withdrawRecords3;
a = this.cashIndex3;
}
return {
withdrawRecords: t,
cashIndex: a
};
}
static get_reward_base_min() {
let e = this.globalData;
if (!e) {
console.log("get_reward_base_min GameData config is empty");
return 0;
}
let t = this.getDataType(), a = t.replace("type", "group"), i = e[t] || e[a];
if (i && null != i.reward_base_min) return Number(i.reward_base_min) || 0;
let o = e.global;
if (o && null != o.reward_base_min) {
let e = o.reward_base_min;
if ("object" != typeof e) return Number(e) || 0;
if (null != e[t]) return Number(e[t]) || 0;
if (null != e[a]) return Number(e[a]) || 0;
}
console.log("get_reward_base_min missing reward_base_min", t, e);
return 0;
}
static getDataType() {
let e = c.normalizeCountry(c.language);
return [ "US", "GB", "CA", "DE", "FR", "AU", "BR", "MY", "TR" ].includes(e) ? "type1" : [ "MX", "ZA", "EG" ].includes(e) ? "type2" : [ "RU", "IN", "PH", "TH", "PK", "BD" ].includes(e) ? "type3" : [ "NG", "JP", "AR", "KZ" ].includes(e) ? "type4" : [ "CO", "KR" ].includes(e) ? "type5" : [ "ID", "VN" ].includes(e) ? "type6" : "type1";
}
static cashCoin(e) {
let t = n.default.getInstance().getData(r.default), a = Math.max(0, Number(t.watch_video_count) || 0);
const i = c.realConfig || {}, s = Array.isArray(i.real_addvideo) && i.real_addvideo.length > 0 ? i.real_addvideo : c.getDefaultGlobalConfig().real_addvideo;
let l = c.getCurrentJsonData(a, s);
console.log("add_over_video q 需要取配置，根据用户累计视频数来取配置随机值   ", l, s);
let d = c.normalizeCountry(c.language), h = 0;
if (l && Array.isArray(l) && 2 === l.length && Array.isArray(l[1])) {
const i = l[1][0], o = l[1][1], n = Math.random() * (o - i) + i;
h = Math.max(1, Math.floor(e * n * 1e4));
1 == a && "BR" == d && (h = 10);
t.UseRevenue = Math.max(Number(t.UseRevenue) || 0, Number(t.money) || 0);
t.UseRevenue += h;
t.money = t.UseRevenue;
t.addVodeoshowCoin = h;
console.log(`add_over_video 随机比例：${n}, 安卓返回看视频的值ecpm ${e} 每次看视频给的金币 ${h}`);
}
n.default.getInstance().saveToUserDefault();
if (1 == a && "BR" == d && o.default.instance) try {
o.default.instance.ShowRealCoinEffect(0, new cc.Vec3(0, 0, 0));
} catch (e) {
console.warn("ShowRealCoinEffect error", e);
}
return h;
}
static getCurrencyConfig(e) {
let t = 1, a = 2;
switch (e) {
case "ID":
t = .5;
a = 0;
break;

case "BR":
t = 2e3;
a = 2;
break;

case "TH":
t = 400;
a = 0;
break;

case "MY":
t = 2500;
a = 0;
break;

case "VN":
t = .5;
a = 0;
break;

case "PH":
t = 200;
a = 0;
break;

default:
a = 2;
t = 1e4;
}
return {
mult: t,
flot: a,
coin: 1e4
};
}
static getCashTimeConfig() {
const e = n.default.getInstance().getData(s.default).Passlevel;
let t = c.globalData.global.cashTime || [];
t.length > 0 && (t = c.getCurrentJsonData(e, t));
return t;
}
}
a.default = c;
c.languageJson = null;
c.all_config_data = {};
c.language = null;
c.detailsData = null;
c.data = null;
c.maxLevel = 1418;
c.mo = 0;
c.lan = 0;
c.birate = 1;
c.mons = [];
c.dangs = [ 50, 90, 95, 100 ];
c.cashIndex1 = 1e3;
c.cashIndex2 = 1e4;
c.cashIndex3 = 1e5;
a.LANGUAGE_ID = {
1: "ID",
2: "BR",
3: "PH",
4: "TH",
5: "MY",
6: "US",
10: "GB",
11: "ZA",
12: "NG",
13: "IN",
15: "EG",
16: "VN",
17: "KZ",
18: "MX",
19: "AR",
20: "CO",
22: "RU",
23: "TR",
24: "JP",
25: "KR",
26: "CA",
27: "DE",
28: "FR",
29: "AU",
31: "PK",
32: "BD"
};
cc._RF.pop();
};
