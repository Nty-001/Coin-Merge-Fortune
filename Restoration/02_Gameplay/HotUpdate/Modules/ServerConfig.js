// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: ServerConfig; dependency map: {"../LanguageControl/GameManagement":"GameManagement","./HWL_TStool":"HWL_TStool"}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "9f95eg5l6tASIGjwYy3lkbf", "ServerConfig");
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
a.HWLServerConfig = a.RealCashPlatform = void 0;
const o = e("../LanguageControl/GameManagement"), n = e("./HWL_TStool");
var s;
(function(e) {
e.PayPal = "PayPal";
e.DANA = "DANA";
e.OVO = "OVO";
e.PIX = "PIX";
e.Pagbank = "Pagbank";
e.Truemoney = "Truemoney";
e.TNG = "TNG";
e.ZaloPay = "ZaloPay";
e.GCash = "GCash";
e.Graboay = "Graboay";
e.Paymaya = "Paymaya";
})(s = a.RealCashPlatform || (a.RealCashPlatform = {}));
class r {
constructor() {
this.productId = "";
this.url_config = "";
this.url_country = "";
this.url_cash = "";
this.url_report = "";
this.url_android = "https://coinmergefortune.top/coin?conf=";
this.url_ios = "https://funiejb.top/blafpbi?conf=";
this.header_android = "coin";
this.header_ios = "blafpbi";
this.configIdArr = [ "GameData" ];
this.url_country_android = "https://coinmergefortune.top/ ";
this.url_country_ios = "https://funiejb.top";
this.url_cash_android = "https://b.coinmergefortune.top/api/";
this.url_cash_ios = "https://b.funiejb.top/api/";
this.reportUrl_android = "https://r.coinmergefortune.top/open_api/";
this.reportUrl_ios = "https://r.funiejb.top/open_api/";
this.linkId = "";
this.version_name = "";
this.device_model = "";
this.clickID = "";
this.productId = this.header_android;
this.url_config = this.url_android;
this.url_country = this.url_country_android;
this.url_cash = this.url_cash_android;
this.url_report = this.reportUrl_android;
if (cc.sys.isNative && cc.sys.os == cc.sys.OS_IOS) {
this.productId = this.header_ios;
this.url_config = this.url_ios;
this.url_country = this.url_country_ios;
this.url_cash = this.url_cash_ios;
this.url_report = this.reportUrl_ios;
}
}
static get instance() {
r._instance || (r._instance = new r());
return r._instance;
}
loadAllConfig() {
this.configIdArr.forEach(e => {
this.loadGroupConfig(e);
});
}
getConfig(e) {
return i(this, void 0, void 0, function*() {
let t = this.getCachedConfig(e);
if (t) {
console.log("server数据配置 -- 使用缓存配置", e);
o.default.all_config_data[e] = t;
this.refreshConfig(e).catch(t => {
console.log("server数据配置 -- 后台刷新失败", e, t);
});
return t;
}
return new Promise(t => {
this.refreshConfig(e).then(e => {
console.log("server数据配置 -- 成功 loadServerConfig");
t(e);
}).catch(() => {
console.log("server数据配置 -- 失败 loadServerConfig");
this.loadLocalConfig(e).then(t);
});
});
});
}
getCachedConfig(e) {
return this.getItem_json(this.productId + e);
}
getCachedConfigAppVersion(e) {
return this.getItem_str(this.productId + e + "_app_version") || "";
}
cacheConfigAppVersion(e, t) {
t && this.setItem_str(this.productId + e + "_app_version", t);
}
refreshConfig(e, t = !0) {
return this.loadGroupConfig(e, t);
}
cacheConfig(e, t) {
t && this.setItem_json(this.productId + e, t);
}
loadGroupConfig(e, t = !0) {
return new Promise((a, i) => {
{
let s = this.url_config, r = this.productId, l = this.getClientId();
s += e;
console.log("server数据配置 -- loadGroupConfig url = ", s, e);
var n = new XMLHttpRequest();
n.open("GET", s, !0);
n.timeout = 8e3;
n.onreadystatechange = () => {
console.log("server数据配置=======   ", n.readyState, n.status);
if (4 === n.readyState) {
if (200 === n.status) try {
let i = JSON.parse(atob(n.responseText));
if (i) {
this.setItem_json(this.productId + e, i);
t && (o.default.all_config_data[e] = i);
console.log("server数据配置 -- 成功 loadServerConfig", i);
a(i);
return;
}
} catch (e) {
console.log("server数据配置 -- parse error loadServerConfig", e);
}
i();
}
};
n.onerror = () => {
console.log("server数据配置 -- onerror loadServerConfig");
i();
};
n.ontimeout = () => {
console.log("server数据配置 -- timeout loadServerConfig");
i();
};
console.log("server数据配置 -- loadGroupConfig clientId = ", l);
n.setRequestHeader(r, l);
n.send();
}
});
}
loadLocalConfig(e) {
return new Promise(t => {
cc.loader.loadRes("config/data", cc.JsonAsset, (a, i) => {
if (!a && i) {
this.setItem_json(this.productId + e, i.json);
o.default.all_config_data[e] = i.json;
t(i.json);
} else {
console.log("server数据配置 -- 本地配置加载失败", a);
t(null);
}
});
});
}
setItem_json(e, t) {
cc.sys.localStorage.setItem(e, JSON.stringify(t));
}
getItem_json(e) {
let t = cc.sys.localStorage.getItem(e);
if (!t) return null;
try {
return JSON.parse(t);
} catch (t) {
console.log("getItem_json parse failed", e, t);
return null;
}
}
setItem_str(e, t) {
cc.sys.localStorage.setItem(e, t);
}
getItem_str(e) {
return cc.sys.localStorage.getItem(e);
}
getCountry() {
return new Promise(e => {
let t = this.getItem_str(this.productId + "_country");
if ("string" == typeof t && "" != t) {
console.log("getCountry localStorage.  country = ", t);
e(t);
} else this.reqCountryConfig().then(t => {
this.setItem_str(this.productId + "_country", t);
console.log("getCountry XMLHttp.  country = ", t);
e(t);
});
});
}
SwitchCountry(e) {
cc.sys.localStorage.clear();
this.setItem_str(this.productId + "_country", e);
cc.game.end();
}
reqCountryConfig() {
return new Promise(e => {
if (cc.sys.isBrowser) {
e("US");
return;
}
let t = this.url_country, a = new XMLHttpRequest();
a.open("GET", t);
a.timeout = 3e3;
a.ontimeout = () => {
e("US");
};
a.onreadystatechange = () => {
if (4 === a.readyState && 200 === a.status) {
var t = a.responseText;
console.log("XMLHttpRequest.  country = ", t);
let i = JSON.parse(t).country;
e(null != i ? i : "US");
} else e("US");
};
a.onerror = () => {
e("US");
};
a.send();
});
}
sendCash(e) {
return this.httpCashSend("withdraw", e);
}
sendCashRecord() {
return this.httpCashSend("withdraw_list");
}
sendHTMLOfferLinkList() {
return this.httpCashSend("offer_link_list");
}
sendHTMLOffLinkClick(e = this.linkId, t = this.clickID) {
return this.httpCashSend("offer_link_click", null, {
linkId: e,
clickId: t
});
}
sendHTMLOffLinkClaimReward(e = this.linkId) {
return this.httpCashSend("offer_link_claim_reward", null, {
linkId: e
});
}
getSendPlatType(e) {
let t = {
[s.PayPal]: "13",
[s.DANA]: "3",
[s.OVO]: "2",
[s.PIX]: "4",
[s.Pagbank]: "5",
[s.TNG]: "6",
[s.GCash]: "7",
[s.Graboay]: "8",
[s.Paymaya]: "10",
[s.Truemoney]: "11",
[s.ZaloPay]: "12"
}[e];
if (t) return t;
console.error("getSendPlatType error. plat = ", e);
return "1";
}
httpCashSend(e, t, i) {
return new Promise((o, s) => {
let r = this.url_cash, l = this.productId;
r += l;
const c = this.getClientId(), d = this.getAdId();
let h = null;
"withdraw" == e && (h = {
action: "withdraw",
clientId: c,
adid: d,
time: n.getTimestampStr(),
wid: 0 == t.sendId ? 0 : t.sendId,
type: this.getSendPlatType(t.plat),
money: t.cash.toFixed(2),
accountType: t.accountType,
account: t.account,
fullName: t.fullName,
documentId: t.documentId
});
"withdraw_list" == e && (h = {
action: "withdraw_list",
clientId: c,
adid: d
});
"offer_link_list" == e && (h = {
clientId: c,
deviceModel: a.HWLServerConfig.device_model,
action: "offer_link_list"
});
"offer_link_click" == e && (h = {
action: "offer_link_click",
clientId: c,
clickId: i && null != i.clickId ? i.clickId : this.clickID,
linkId: i && null != i.linkId ? i.linkId : this.linkId,
deviceModel: a.HWLServerConfig.device_model
});
"offer_link_claim_reward" == e && (h = {
action: "offer_link_claim_reward",
clientId: c,
linkId: i && null != i.linkId ? i.linkId : this.linkId
});
if (!h) {
s("requestData is null");
return;
}
console.log("request url ======>>", r);
console.log("requestData ======>>", JSON.stringify(h));
let u = JSON.stringify(h), g = n.AESUtil.encrypt(u, l);
const p = n.HWLwordArrayToBuffer(g);
console.log("request buffer ======>>", p);
let f = new XMLHttpRequest();
f.open("POST", r);
f.timeout = 15e3;
f.ontimeout = () => {
s("ontimeout");
};
f.onreadystatechange = () => {
if (4 === f.readyState) if (200 === f.status) {
console.log("请求成功:", f.responseText);
o(f.responseText);
} else {
console.error("请求失败:", f.status, f.statusText);
s(f.status);
}
};
f.onerror = () => {
console.error("请求失败:", f.status, f.statusText);
s(f.status);
};
f.send(p);
});
}
showVideoOpen(e) {
this.reportADEvent(e, 0, 0);
}
showVideoComplete(e) {
this.reportADEvent(e, 1, 0);
}
showVideo(e) {
this.reportADEvent(e, 3, 0);
}
showVideoFail(e, t = 0) {
this.reportADEvent(e, 2, t);
}
reportADEvent(e, t, a = 0) {
this.reportADSend(e, t, a).then(e => {
console.log("reportADEvent success:", e);
}).catch(e => {
console.error("reportADEvent error:", e);
});
}
reportADSend(e, t, a = 0) {
return new Promise((i, o) => {
let s = n.HWLgenerateUUID(), r = this.productId, l = this.url_report;
l += r;
let c = {
requestId: `${s}`,
clientId: this.getClientId(),
adid: this.getAdId(),
productId: r,
ts: Math.floor(Date.now() / 1e3),
ra: "ad",
appVersion: this.version_name,
adInfo: {
ad: e,
repType: t,
errorCode: a
}
};
console.log("report request url ======>>", l);
console.log("report requestData ======>>", JSON.stringify(c));
let d = n.AESUtil.encrypt(JSON.stringify(c), r);
const h = n.HWLwordArrayToBuffer(d);
let u = new XMLHttpRequest();
u.open("POST", l);
u.setRequestHeader("Content-Type", "application/octet-stream");
u.timeout = 3e3;
u.ontimeout = () => {
o("ontimeout");
};
u.onreadystatechange = () => {
if (4 === u.readyState) if (200 === u.status) {
console.log("请求成功:", u.responseText);
i(u.responseText);
} else {
console.error("请求失败:", u.status, u.statusText);
o(u.status);
}
};
u.onerror = () => {
console.error("请求失败:", u.status, u.statusText);
o(u.status);
};
u.send(h);
});
}
reportGameEvent(e, t) {
this.reportGameSend(e, t).then(e => {
console.log("reportGameEvent Game success:", e);
}).catch(e => {
console.error("reportGameEvent Game error:", e);
});
}
reportGameSend(e, t) {
return new Promise((a, i) => {
let o = n.HWLgenerateUUID(), s = this.productId, r = this.url_report;
r += s;
let l = {
requestId: `${o}`,
clientId: this.getClientId(),
adid: this.getAdId(),
productId: s,
ts: Math.floor(Date.now() / 1e3),
ra: "game",
gameInfo: {
gameType: 0,
repType: t,
level: e + "",
value: ""
}
};
console.log("report Game request url ======>>", r);
console.log("report Game requestData ======>>", JSON.stringify(l));
let c = n.AESUtil.encrypt(JSON.stringify(l), s);
const d = n.HWLwordArrayToBuffer(c);
let h = new XMLHttpRequest();
h.open("POST", r);
h.timeout = 3e3;
h.ontimeout = () => {
i("ontimeout");
};
h.onreadystatechange = () => {
if (4 === h.readyState) if (200 === h.status) {
console.log("请求成功:", h.responseText);
a(h.responseText);
} else {
console.error("请求失败:", h.status, h.statusText);
i(h.statusText);
}
};
h.onerror = () => {
console.error("请求失败:", h.status, h.statusText);
i(h.statusText);
};
h.send(d);
});
}
getClientId() {
let e = this.getItem_str(this.productId + "_clientId");
if (!e || "" == e) {
e = this.N_getClientId();
this.setItem_str(this.productId + "_clientId", e);
}
return e;
}
getAdId() {
let e = this.getItem_str(this.productId + "_adId");
if (!e || "" == e) {
e = this.N_getAdId();
this.setItem_str(this.productId + "_adId", e);
}
return e;
}
N_getClientId() {
return cc.sys.isNative && cc.sys.os == cc.sys.OS_IOS ? jsb.reflection.callStaticMethod("CocosBridge", "getClientId:", "") : cc.sys.isNative && cc.sys.os == cc.sys.OS_ANDROID ? jsb.reflection.callStaticMethod("com/qcz/android/sdk/QczSDK", "getClientId", "()Ljava/lang/String;") : "";
}
N_getAdId() {
return cc.sys.isNative && cc.sys.os == cc.sys.OS_IOS ? jsb.reflection.callStaticMethod("CocosBridge", "getAdid:", "") : cc.sys.isNative && cc.sys.os == cc.sys.OS_ANDROID ? jsb.reflection.callStaticMethod("com/qcz/android/sdk/QczSDK", "getAdid", "()Ljava/lang/String;") : "";
}
N_vibrate(e = 1, t = 26, a = 0, i = 255) {
if (!cc.sys.isBrowser) {
cc.sys.isNative && cc.sys.os == cc.sys.OS_ANDROID && jsb.reflection.callStaticMethod("com/qcz/android/sdk/QczSDK", "qcz_onVibrator", "(ILjava/lang/String;Ljava/lang/String;I)V", e, t.toString(), a.toString(), i);
cc.sys.isNative && cc.sys.os == cc.sys.OS_IOS && jsb.reflection.callStaticMethod("CocosBridge", "onVibrator:", "");
}
}
N_callPaste(e) {
if (!cc.sys.isBrowser) {
cc.sys.isNative && cc.sys.os == cc.sys.OS_ANDROID && jsb.reflection.callStaticMethod("com/qcz/android/sdk/QczSDK", "qcz_clip", "(Ljava/lang/String;)V", e);
!cc.sys.isNative || (cc.sys.os, cc.sys.OS_IOS);
}
}
N_gameScore() {
cc.sys.isNative && cc.sys.os == cc.sys.OS_IOS ? jsb.reflection.callStaticMethod("CocosBridge", "onScore:", "") : console.log("非IOS平台");
}
N_gameBoxShow() {
if (cc.sys.isNative && cc.sys.os == cc.sys.OS_ANDROID) {
const e = jsb.reflection.callStaticMethod("com/qcz/android/sdk/QczSDK", "qcz_isOkspinEnabled", "()Z");
console.log("qcz_isOkspinEnabled ===== ", e);
return e;
}
return !1;
}
N_gameBoxCome() {
cc.sys.isNative && cc.sys.os == cc.sys.OS_ANDROID && jsb.reflection.callStaticMethod("com/qcz/android/sdk/QczSDK", "qcz_showOkspin", "()V");
}
N_adRejected() {
let e = !1;
cc.sys.isNative && cc.sys.os == cc.sys.OS_ANDROID && (e = jsb.reflection.callStaticMethod("com/qcz/android/sdk/QczSDK", "isAdRejected", "()Z"));
cc.sys.isNative && cc.sys.os == cc.sys.OS_IOS && (e = jsb.reflection.callStaticMethod("CocosBridge", "isAdRejected:", ""));
return e;
}
N_getDevicePrams() {
if (cc.sys.isNative && cc.sys.os == cc.sys.OS_ANDROID) {
const e = jsb.reflection.callStaticMethod("com/qcz/android/sdk/QczSDK", "getDevicePrams", "()Ljava/lang/String;");
console.log("qcz_okspinActions ===== ", e);
let t = null;
try {
console.log("***************** getgetDevicePrams  *****************");
console.log("getgetDevicePrams jsonStr: ", e);
t = JSON.parse(e);
} catch (e) {} finally {
console.info(t);
}
if (!t || "object" != typeof t) {
this.version_name = "";
this.device_model = "";
return "";
}
this.version_name = t.version_name || "";
this.device_model = t.device_model || "";
return this.version_name;
}
if (cc.sys.isNative && cc.sys.os == cc.sys.OS_IOS) {
const e = jsb.reflection.callStaticMethod("CocosBridge", "getDevicePrams:", "");
console.log("IOS qcz_okspinActions ===== ", e);
let t = null;
try {
console.log("***************** getgetDevicePrams  *****************");
console.log("getgetDevicePrams jsonStr: ", e);
t = JSON.parse(e);
} catch (e) {} finally {
console.info(t);
}
if (!t || "object" != typeof t) {
this.version_name = "";
this.device_model = "";
return "";
}
this.version_name = t.version_name || "";
this.device_model = t.device_model || "";
return this.version_name;
}
return "";
}
gotoMarket() {
cc.sys.isNative && cc.sys.os == cc.sys.OS_ANDROID && jsb.reflection.callStaticMethod("com/qcz/android/sdk/QczSDK", "gotoMarket", "()V");
cc.sys.isNative && cc.sys.os == cc.sys.OS_IOS && jsb.reflection.callStaticMethod("CocosBridge", "gotoMarket:", "");
}
generateClickId() {
const e = new Date(), t = `${e.getFullYear()}${(e.getMonth() + 1).toString().padStart(2, "0")}${e.getDate().toString().padStart(2, "0")}${e.getHours().toString().padStart(2, "0")}${e.getMinutes().toString().padStart(2, "0")}${e.getSeconds().toString().padStart(2, "0")}`, a = "0123456789abcdefghijklmnopqrstuvwxyz";
let i = "";
for (let e = 0; e < 6; e++) i += a.charAt(Math.floor(Math.random() * a.length));
return t + i;
}
}
a.HWLServerConfig = r.instance;
cc._RF.pop();
};
