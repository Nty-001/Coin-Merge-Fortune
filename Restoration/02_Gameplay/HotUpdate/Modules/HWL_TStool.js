// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: HWL_TStool; dependency map: {"../BaseUIManager/Storage/GameLocalData":"GameLocalData","../BaseUIManager/Storage/PlayData":"PlayData","../BaseUIManager/Storage/SoundManager":"SoundManager","../BaseUIManager/UIManagerNew":"UIManagerNew","../LanguageControl/Lab":"Lab","./ServerConfig":"ServerConfig","crypto-js":12}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "e8045w7xxRBIrOiS6W9oXf9", "HWL_TStool");
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
a.prefixZero = a.getTimestampStr = a.getRandomInt = a.shuffleArray = a.truncateToDecimal = a.roundToDecimal = a.getday = a.HWLlaunchBillingFlow = a.HWLbuy2 = a.HWLbuy1 = a.HWLchaxun = a.HWLinitzhifu = a.inittixian = a.HWLsendPostRequest = a.HWLpostWordArray = a.HWLwordArrayToBuffer = a.HWLgenerateUUID = a.AESUtil = a.HWLgetBasestr = a.HWLloadsound = a.HWLloadRes = a.HWLreader = a.HWLclean = a.HWLload = a.HWLsave = a.HWLhideBanner = a.HWLshowBanner = a.HWLshowAd = a.HWL = void 0;
const o = e("crypto-js"), n = e("../BaseUIManager/Storage/GameLocalData"), s = e("../BaseUIManager/Storage/PlayData"), r = e("../BaseUIManager/Storage/SoundManager"), l = e("../BaseUIManager/UIManagerNew"), c = e("../LanguageControl/Lab"), d = e("./ServerConfig"), h = "blafpb", u = "blafpbi";
class g {
static addadnum() {
n.default.getInstance().getData(s.default).add_show_video();
}
static netwrong() {
let e = c.default.getlab("73");
l.default.show_toast({
text: e
});
}
static pauseMusic() {
r.default.isWatchVideo = !0;
r.default.pauseBgm();
}
static resumeMusic() {
r.default.isWatchVideo = !1;
r.default.resumeBgm();
}
static initshop() {}
static cheakshop() {}
static initchaping() {
this.EVENT_SHOWtime = new Date().getTime();
cc.game.on(cc.game.EVENT_HIDE, () => {
cc.log(">>> 游戏进入后台");
}, this);
cc.game.on(cc.game.EVENT_SHOW, () => {
cc.log(">>> 游戏回到前台");
if (g.adstart) return;
let e = new Date().getTime();
-1 != this.pasttime && e - this.EVENT_SHOWtime > this.pasttime && p("7_A", () => {}, () => {});
}, this);
}
static get pasttime() {
Number(f("firstday")), L();
return -1;
}
}
a.HWL = g;
g.adscd1 = 6e4;
g.adscd2 = 4e4;
g.productArr = [];
g.clientId = "";
g.EVENT_SHOWtime = 0;
g.network = !0;
g.backcall1 = () => {};
g.backcall2 = () => {};
g.Android_Path = "com/qcz/android/sdk/QczSDK";
g.IOS_Path = "CocosBridge";
g.adname = "";
g.adstart = !1;
g.isBannerShown = !1;
function p(e, t, a, i = !1, o = 1) {
if (g.adstart) return !1;
if (d.HWLServerConfig.N_adRejected()) {
let e = c.default.getlab("74");
l.default.show_toast({
text: e
});
return !1;
}
g.backcall1 = t;
g.backcall2 = a;
g.adname = e;
d.HWLServerConfig.showVideoOpen(g.adname);
g.adstart = !0;
g.pauseMusic();
try {
if (cc.sys.isNative && cc.sys.os === cc.sys.OS_ANDROID) jsb.reflection.callStaticMethod(g.Android_Path, i ? "qcz_playAdByType" : "qcz_playAd", `(Ljava/lang/String;${i ? "I" : "Z"})V`, "ad_source", o); else if (cc.sys.isNative && cc.sys.os == cc.sys.OS_IOS) jsb.reflection.callStaticMethod(g.IOS_Path, i ? "playAdByType:" : "playAd:", o + ""); else {
g.addadnum();
g.adstart = !1;
g.resumeMusic();
g.backcall1(.002);
}
} catch (e) {
console.error("show ad error", e);
g.adstart = !1;
g.resumeMusic();
g.backcall2 && g.backcall2();
g.netwrong();
return !1;
}
return !0;
}
a.HWLshowAd = p;
a.HWLshowBanner = function() {
if (cc.sys.isNative && cc.sys.os == cc.sys.OS_ANDROID) {
jsb.reflection.callStaticMethod("com/qcz/android/sdk/QczSDK", "showBanner", "()V");
this.isBannerShown = !0;
} else if (cc.sys.isNative && cc.sys.os == cc.sys.OS_IOS) {
jsb.reflection.callStaticMethod(g.IOS_Path, "showBanner", "");
this.isBannerShown = !0;
} else this.isBannerShown = !0;
};
a.HWLhideBanner = function() {
if (0 != this.isBannerShown) {
try {
cc.sys.isNative && cc.sys.os == cc.sys.OS_ANDROID ? jsb.reflection.callStaticMethod("com/qcz/android/sdk/QczSDK", "hideBanner", "()V") : cc.sys.isNative && cc.sys.os == cc.sys.OS_IOS && jsb.reflection.callStaticMethod(g.IOS_Path, "hideBanner", "");
} catch (e) {
console.error("Failed to hide banner:", e);
}
this.isBannerShown = !1;
}
};
window.XSSdkCallback = function(e, t) {
let a = null;
try {
console.log("***************** 前端收到消息 *****************");
console.log("action: ", e);
console.log("jsonStr: ", t);
a = JSON.parse(t);
} catch (t) {
console.error("解析数据失败", e);
} finally {
console.info(a);
}
if ("ad_over" == e) {
let e = a ? Number(a.revenue) : 0;
g.EVENT_SHOWtime = new Date().getTime();
g.adstart = !1;
g.addadnum();
g.resumeMusic();
g.backcall1(e);
d.HWLServerConfig.showVideoComplete(g.adname);
} else if ("ad_play" == e) d.HWLServerConfig.showVideo(g.adname); else if ("ad_error" == e) {
let e = a ? Number(a.errorCode || "0") : 0;
g.adstart = !1;
g.resumeMusic();
g.backcall2();
g.netwrong();
d.HWLServerConfig.showVideoFail(g.adname, e);
} else if ("network_status" == e) {
g.network = "success" == t;
"success" != t && g.netwrong();
}
};
a.HWLsave = function(e, t) {
let a = h;
cc.sys.isNative && cc.sys.os == cc.sys.OS_IOS && (a = u);
cc.sys.localStorage.setItem(e + a, t);
};
function f(e) {
let t = h;
cc.sys.isNative && cc.sys.os == cc.sys.OS_IOS && (t = u);
return cc.sys.localStorage.getItem(e + t);
}
a.HWLload = f;
a.HWLclean = function(e) {
if (e) {
let t = h;
cc.sys.isNative && cc.sys.os == cc.sys.OS_IOS && (t = u);
cc.sys.localStorage.removeItem(e + t);
} else cc.sys.localStorage.clear();
};
a.HWLreader = function(e) {
return i(this, void 0, void 0, function*() {
return new Promise(t => {
m(e, cc.JsonAsset).then(e => {
t && t(e);
});
});
});
};
function m(e, t) {
return new Promise((a, i) => {
cc.resources.load(e, t, (e, t) => {
if (e) {
i && i(e);
console.error(e);
} else a && a(t);
});
});
}
a.HWLloadRes = m;
a.HWLloadsound = function(e) {
return i(this, void 0, void 0, function*() {
return this.loadRes(`sound/${e}`, cc.AudioClip);
});
};
a.HWLgetBasestr = function(e) {
return atob(e);
};
a.AESUtil = {
_secretKey: "match",
setSecretKey(e) {
this._secretKey = e;
},
encrypt(e, t) {
const a = t || this._secretKey;
if (!a) throw new Error("AESUtil: 密钥未设置");
const i = o.SHA256(a), n = o.enc.Hex.parse(i.toString(o.enc.Hex));
return o.AES.encrypt(o.enc.Utf8.parse(e), n, {
mode: o.mode.ECB,
padding: o.pad.Pkcs7
}).ciphertext;
},
decrypt(e, t) {
const a = t || this._secretKey;
if (!a) throw new Error("AESUtil: 密钥未设置");
const i = o.SHA256(a), n = o.enc.Hex.parse(i.toString(o.enc.Hex));
return o.AES.decrypt(e, n, {
mode: o.mode.ECB,
padding: o.pad.Pkcs7
}).toString(o.enc.Utf8);
},
generateKey() {
const e = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
let t = "";
for (let a = 0; a < 16; a++) t += e[Math.floor(Math.random() * e.length)];
return t;
}
};
function y() {
return "xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx".replace(/[xy]/g, e => {
const t = 16 * Math.random() | 0;
return ("x" === e ? t : 3 & t | 8).toString(16);
});
}
a.HWLgenerateUUID = y;
function _(e) {
const {words: t, sigBytes: a} = e, i = new ArrayBuffer(a), o = new DataView(i);
for (let e = 0; e < a; e++) {
const a = t[Math.floor(e / 4)];
o.setUint8(e, a >>> 8 * (3 - e % 4) & 255);
}
return i;
}
a.HWLwordArrayToBuffer = _;
function v(e, t, a = {}, i = 1e4) {
const o = _(t);
return new Promise((t, n) => {
const s = new XMLHttpRequest();
s.open("POST", e);
s.timeout = i;
s.responseType = "arraybuffer";
s.setRequestHeader("Content-Type", "application/octet-stream");
Object.keys(a).forEach(e => s.setRequestHeader(e, a[e]));
s.onload = () => s.status >= 200 && s.status < 300 ? t(s.response) : n(new Error(`Request failed: ${s.status}`));
s.onerror = () => n(new Error("Network error"));
s.ontimeout = () => n(new Error("Request timeout"));
s.send(o);
});
}
a.HWLpostWordArray = v;
a.HWLsendPostRequest = function(e, t, i) {
cc.sys.isNative && cc.sys.os == cc.sys.OS_ANDROID ? g.clientId = jsb.reflection.callStaticMethod("com/qcz/android/sdk/QczSDK", "getClientId", "()Ljava/lang/String;") : cc.sys.isNative && cc.sys.os == cc.sys.OS_IOS && (g.clientId = jsb.reflection.callStaticMethod("CocosBridge", "getClientId:", ""));
if (!g.clientId) return;
const o = `${t}`;
let n = "https://r.bfhgwv.work/open_api/" + h, s = h;
if (cc.sys.isNative && cc.sys.os == cc.sys.OS_IOS) {
n = "https://r.funiejb.top/open_api/" + u;
s = u;
}
const r = g.clientId, l = {
requestId: y(),
proid: s,
ra: e,
clientId: r,
ts: Math.floor(Date.now() / 1e3)
};
console.info(l);
l[o] = i;
v(n, a.AESUtil.encrypt(JSON.stringify(l), s)).then(e => console.log("服务器返回字节数=", e.byteLength)).catch(e => console.error("发送失败:", e));
};
function b() {
window.ggBilling || (window.ggBilling = {
queryProductDetailsCallback: null,
launchBillingFlowCallback: null,
acknowledgePurchaseCallback: null,
consumeCallback: null,
queryPurchaseCallback: null
});
}
a.inittixian = b;
a.HWLinitzhifu = function(e) {
console.info("给乐峰：", e);
S(e).then(e => {
console.log("查询列表成功", e);
g.productArr = e.product;
g.initshop();
this.chaxun();
});
};
a.HWLchaxun = function() {
D().then(e => {
console.log("查询记录成功", e);
g.productdata = e.data;
g.cheakshop();
}).catch(e => {
console.error("查询交易失败:", e);
});
};
a.HWLbuy1 = function(e) {
C(e).then(() => {
console.log("购买成功");
}).catch(e => {
console.error("购买失败:", e);
});
};
a.HWLbuy2 = function(e) {
w(e).then(() => {
console.log("购买成功");
}).catch(e => {
console.error("购买失败:", e);
});
};
function S(e) {
console.info("给乐峰：", e);
return new Promise((t, a) => {
if (cc.sys.isNative) {
b();
window.ggBilling.queryProductDetailsCallback = e => {
console.info("触发回调查询列表");
try {
const i = JSON.parse(e);
0 === i.responseCode ? t(i) : a(new Error(i.debugMessage || "Query product failed"));
} catch (e) {
a(e);
}
};
if (cc.sys.isNative && cc.sys.os == cc.sys.OS_ANDROID) if (jsb.reflection) {
console.info("请求乐峰：", e);
jsb.reflection.callStaticMethod("com/qcz/android/sdk/QczSDK", "queryProductDetails", "(Ljava/lang/String;)V", e);
} else a(new Error("Android environment not detected")); else cc.sys.os == cc.sys.OS_IOS && (jsb.reflection ? jsb.reflection.callStaticMethod(g.IOS_Path, "queryProductDetails", "(Ljava/lang/String;)V", e) : a(new Error("Android environment not detected")));
} else {
console.log("Browser environment: skip queryProductDetails");
t({
responseCode: 0,
productDetailsList: []
});
}
});
}
a.HWLlaunchBillingFlow = function(e) {
b();
return new Promise((t, a) => {
window.ggBilling.launchBillingFlowCallback = e => {
try {
const i = JSON.parse(e);
0 === i.responseCode ? t(i) : a(new Error(i.message || "Payment failed"));
} catch (e) {
a(e);
}
};
cc.sys.isNative && cc.sys.os == cc.sys.OS_ANDROID ? jsb.reflection ? jsb.reflection.callStaticMethod("com/qcz/android/sdk/QczSDK", "launchBillingFlow", "(Ljava/lang/String;)Ljava/lang/String;", e) : a(new Error("Android environment not detected")) : cc.sys.os == cc.sys.OS_IOS && (jsb.reflection ? jsb.reflection.callStaticMethod(g.IOS_Path, "launchBillingFlow", "(Ljava/lang/String;)Ljava/lang/String;", e) : a(new Error("Android environment not detected")));
});
};
function C(e) {
b();
return new Promise((t, a) => {
window.ggBilling.acknowledgePurchaseCallback = e => {
0 === JSON.parse(e).responseCode ? t() : a(new Error("Failed to acknowledge purchase"));
};
cc.sys.isNative && cc.sys.os == cc.sys.OS_ANDROID ? jsb.reflection ? jsb.reflection.callStaticMethod("com/qcz/android/sdk/QczSDK", "acknowledgePurchase", "(Ljava/lang/String;)V", e) : a(new Error("Android environment not detected")) : cc.sys.os == cc.sys.OS_IOS && (jsb.reflection ? jsb.reflection.callStaticMethod(g.IOS_Path, "acknowledgePurchase", "(Ljava/lang/String;)V", e) : a(new Error("Android environment not detected")));
});
}
function w(e) {
b();
return new Promise((t, a) => {
window.ggBilling.consumeCallback = e => {
0 === JSON.parse(e).responseCode ? t() : a(new Error("Failed to consume purchase"));
};
cc.sys.isNative && cc.sys.os == cc.sys.OS_ANDROID ? jsb.reflection ? jsb.reflection.callStaticMethod("com/qcz/android/sdk/QczSDK", "consume", "(Ljava/lang/String;)V", e) : a(new Error("Android environment not detected")) : cc.sys.os == cc.sys.OS_IOS && (jsb.reflection ? jsb.reflection.callStaticMethod(g.IOS_Path, "consume", "(Ljava/lang/String;)V", e) : a(new Error("Android environment not detected")));
});
}
function D() {
b();
console.info("尝试查询");
return new Promise((e, t) => {
window.ggBilling.queryPurchaseCallback = a => {
console.info("触发回调查询记录");
const i = JSON.parse(a);
0 === i.responseCode ? e(i) : t(new Error("Failed to query purchases"));
};
cc.sys.isNative && cc.sys.os == cc.sys.OS_ANDROID ? jsb.reflection ? jsb.reflection.callStaticMethod("com/qcz/android/sdk/QczSDK", "queryPurchase", "()V") : t(new Error("Android environment not detected")) : cc.sys.os == cc.sys.OS_IOS && (jsb.reflection ? jsb.reflection.callStaticMethod(g.IOS_Path, "queryPurchase", "()V") : t(new Error("Android environment not detected")));
});
}
function L() {
const e = new Date(), t = new Date(e.getFullYear(), 0, 1), a = e.getTime() - t.getTime();
return Math.floor(a / 864e5) + 1;
}
a.getday = L;
a.roundToDecimal = function(e, t) {
return Number((Math.floor(1e4 * e) / 1e4).toFixed(t));
};
a.truncateToDecimal = function(e, t) {
const a = Math.pow(10, t);
return Math.floor(e * a) / a;
};
a.shuffleArray = function(e) {
let t, a, i = e.length;
for (;0 !== i; ) {
a = Math.floor(Math.random() * i);
t = e[i -= 1];
e[i] = e[a];
e[a] = t;
}
return e;
};
a.getRandomInt = function(e, t) {
return Math.floor(Math.random() * (t - e + 1)) + e;
};
a.getTimestampStr = function(e) {
const t = new Date(e || new Date().getTime()), [a, i, o, n, s, r] = [ t.getFullYear(), t.getMonth() + 1, t.getDate(), t.getHours(), t.getMinutes(), t.getSeconds() ];
return `${a}-${I(i)}-${I(o)} ${I(n)}:${I(s)}:${I(r)}`;
};
function I(e, t = 2) {
return (Array(t).join("0") + Math.floor(e)).slice(-t);
}
a.prefixZero = I;
cc._RF.pop();
};
