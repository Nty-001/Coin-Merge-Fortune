// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: NativeCall; dependency map: {"../HWL/ServerConfig":"ServerConfig","../LanguageControl/GameManagement":"GameManagement"}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "9ef572Cb05PfoTmsMDcLDTC", "NativeCall");
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
const o = e("../HWL/ServerConfig"), n = e("../LanguageControl/GameManagement");
class s {
static gameTopShow() {
if (cc.sys.isNative && cc.sys.os == cc.sys.OS_ANDROID) {
const e = jsb.reflection.callStaticMethod("com/qcz/android/sdk/QczSDK", "qcz_isOkspinEnabled", "()Z");
console.log("qcz_isOkspinEnabled ===== ", e);
return e;
}
return !1;
}
static gameTopShows() {
if (cc.sys.isNative && cc.sys.os == cc.sys.OS_ANDROID) {
const e = jsb.reflection.callStaticMethod("com/qcz/android/sdk/QczSDK", "qcz_okspinActions", "()Ljava/lang/String;");
console.log("qcz_okspinActions ===== ", e);
return e;
}
return "";
}
static gameTopCome(e) {
console.log("gameTopCome ===== ", e);
cc.sys.isNative && cc.sys.os == cc.sys.OS_ANDROID && jsb.reflection.callStaticMethod("com/qcz/android/sdk/QczSDK", "qcz_showOkspin", "(I)V", e);
}
static getgetDevicePrams() {
this.version_name = o.HWLServerConfig.N_getDevicePrams();
return this.version_name;
}
static gotoMarket() {
o.HWLServerConfig.gotoMarket();
}
static getCientId() {
return o.HWLServerConfig.getClientId();
}
static getCientIdSync() {
return i(this, void 0, void 0, function*() {
let e = yield s.getCientId();
this.checkClientEndingWith(e);
});
}
static checkClientEndingWith(e) {
console.log("checkClientEndingWith  检测设备尾号 ", e);
if (!e || 0 === e.length) {
cc.sys.localStorage.setItem("RandomAB", "B");
return !1;
}
const t = e.charAt(e.length - 1), a = [ "5", "6", "7", "8", "9" ].includes(t);
cc.sys.localStorage.setItem("RandomAB", a ? "B" : "A");
return a;
}
static checkClientEndingWithCoin() {
let e = s.getCientId();
console.log("checkClientEndingWithCoin  检测设备尾号 ", e);
if (cc.sys.isBrowser) return !0;
if (!e || 0 === e.length) return !1;
const t = e.charAt(e.length - 1);
return n.default.globalData.global.coin_mubiaoA.includes(t);
}
static getAdId() {
return o.HWLServerConfig.getAdId();
}
static openUrl(e) {
if (cc.sys.isNative && cc.sys.os == cc.sys.OS_ANDROID) try {
jsb.reflection.callStaticMethod("com/qcz/android/sdk/QczSDK", "openWebLink", "(Ljava/lang/String;)V", e);
} catch (e) {
console.warn(" 失败:", e);
} else if (cc.sys.os == cc.sys.OS_IOS) try {
jsb.reflection.callStaticMethod("CocosBridge", "openWebLink:", e);
} catch (e) {
console.warn(" 失败:", e);
} else console.log("非原生平台");
}
}
s.version_name = "";
a.default = s;
cc._RF.pop();
};
