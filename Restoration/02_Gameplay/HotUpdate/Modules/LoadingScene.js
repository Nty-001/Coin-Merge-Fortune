// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: LoadingScene; dependency map: {"./BaseUIManager/Storage/GameLocalData":"GameLocalData","./BaseUIManager/Storage/LoadAllResources":"LoadAllResources","./BaseUIManager/UIConfig":"UIConfig","./BaseUIManager/UIManagerNew":"UIManagerNew","./HWL/ServerConfig":"ServerConfig","./LanguageControl/GameManagement":"GameManagement","./Report/NativeCall":"NativeCall"}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "665ebp+/hVHM53pKfrQzbkl", "LoadingScene");
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
const n = e("./BaseUIManager/Storage/GameLocalData"), s = e("./BaseUIManager/Storage/LoadAllResources"), r = e("./BaseUIManager/UIConfig"), l = e("./BaseUIManager/UIManagerNew"), c = e("./HWL/ServerConfig"), d = e("./LanguageControl/GameManagement"), h = e("./Report/NativeCall"), {ccclass: u, property: g} = cc._decorator;
let p = class extends cc.Component {
constructor() {
super(...arguments);
this.loadingtxt = null;
this.progress = null;
this.fakeProgress = 0;
this.loadComplete = !1;
this.preloadScenePromise = null;
this.resourcesLoadPromise = null;
this.continueLoadingStarted = !1;
this.pendingUpdateDialog = null;
this.optionalUpdateDialogShowing = !1;
}
onLoad() {
this.fakeProgress = 0;
this.loadComplete = !1;
this.preloadScenePromise = null;
this.resourcesLoadPromise = null;
this.continueLoadingStarted = !1;
this.pendingUpdateDialog = null;
this.optionalUpdateDialogShowing = !1;
this.setLoadingProgress(0);
this.schedule(this.updateFakeProgress, .1);
this.startLoading();
}
startLoading() {
return o(this, void 0, void 0, function*() {
try {
let e = yield this.promiseWithTimeout(c.HWLServerConfig.getCountry(), 3e3, "US", "getCountry timeout");
d.default.language = d.default.normalizeCountry(e || "US");
this.setLoadingProgress(.15);
yield this.loadDataWith();
} catch (e) {
console.log("LoadingScene startLoading error", e);
yield this.ensureLocalDataReady();
}
this.pendingUpdateDialog = this.checkupdate();
if (this.pendingUpdateDialog) if (this.pendingUpdateDialog.forceUpdate) this.onLoadingFinish(!0); else {
this.optionalUpdateDialogShowing = !0;
this.showUpdateDialog(this.pendingUpdateDialog, () => {
this.optionalUpdateDialogShowing = !1;
this.pendingUpdateDialog = null;
this.continueLoadingAfterUpdate();
});
} else yield this.continueLoadingAfterUpdate();
});
}
loadDataWith() {
return o(this, void 0, void 0, function*() {
console.log("GameManagerment.language   ", d.default.language);
yield this.promiseWithTimeout(h.default.getCientIdSync(), 3e3, null, "getCientIdSync timeout");
try {
h.default.getgetDevicePrams();
} catch (e) {
console.log("getgetDevicePrams error", e);
}
this.setLoadingProgress(.25);
let e = yield this.loadGameDataConfig();
d.default.all_config_data.GameData = e || d.default.globalData;
console.log("GameManagerment.all_config_data   ", d.default.all_config_data);
this.setLoadingProgress(.55);
yield this.ensureJsonDataReady();
this.setLoadingProgress(.7);
});
}
loadGameDataConfig() {
return o(this, void 0, void 0, function*() {
const e = "GameData";
let t = c.HWLServerConfig.getCachedConfig(e), a = (c.HWLServerConfig.version_name || "").trim(), i = c.HWLServerConfig.getCachedConfigAppVersion(e);
if (t && a && i !== a) {
console.log("LoadingScene app 版本变化，使用服务器配置覆盖本地缓存", e, i, a);
let t = c.HWLServerConfig.refreshConfig(e).catch(e => {
console.log("LoadingScene refresh GameData failed", e);
return null;
}), o = yield this.promiseWithTimeout(t, 9e3, null, "refresh GameData timeout");
if (o) {
c.HWLServerConfig.cacheConfigAppVersion(e, a);
console.log("LoadingScene 使用服务器配置", e);
return o;
}
let n = yield this.loadLocalBlastConfig();
n && console.log("LoadingScene 服务器配置不可用，使用本地表配置", e);
return n;
}
if (t) {
console.log("LoadingScene 使用本地缓存配置", e);
d.default.all_config_data.GameData = t;
c.HWLServerConfig.refreshConfig(e, !1).then(t => {
if (t) {
c.HWLServerConfig.cacheConfigAppVersion(e, a);
console.log("LoadingScene 后台更新服务器配置缓存", e);
}
}).catch(e => {
console.log("LoadingScene refresh GameData failed", e);
});
return t;
}
let o = c.HWLServerConfig.refreshConfig(e).catch(e => {
console.log("LoadingScene refresh GameData failed", e);
return null;
}), n = yield this.promiseWithTimeout(o, 9e3, null, "refresh GameData timeout");
if (n) {
c.HWLServerConfig.cacheConfigAppVersion(e, a);
console.log("LoadingScene 使用服务器配置", e);
return n;
}
let s = yield this.loadLocalBlastConfig();
s && console.log("LoadingScene 本地缓存和服务器配置都不可用，使用本地表配置", e);
return s;
});
}
onProgress(e) {
this.setLoadingProgress(.7 + .15 * cc.misc.clamp01(e));
}
updateFakeProgress() {
if (this.loadComplete || this.optionalUpdateDialogShowing) return;
let e = 0;
e = this.fakeProgress < .3 ? .05 : this.fakeProgress < .6 ? .03 : this.fakeProgress < .8 ? .02 : .01;
this.fakeProgress += e;
if (this.fakeProgress >= .9) {
this.fakeProgress = .9;
this.unschedule(this.updateFakeProgress);
}
this.setLoadingProgress(this.fakeProgress);
}
continueLoadingAfterUpdate() {
return o(this, void 0, void 0, function*() {
if (!(this.loadComplete || this.optionalUpdateDialogShowing || this.continueLoadingStarted)) {
this.continueLoadingStarted = !0;
yield this.loadResourcesAfterUpdate();
yield this.preloadGameScene();
this.onLoadingFinish();
}
});
}
loadResourcesAfterUpdate() {
if (this.resourcesLoadPromise) return this.resourcesLoadPromise;
this.resourcesLoadPromise = new Promise(e => {
let t = !1, a = () => {
if (!t) {
t = !0;
e();
}
};
n.default.getInstance().loadOrRequlestStorageData(() => {
s.default.getInstance().loadAllRes(e => {
this.onProgress(e);
e >= 1 && a();
});
});
});
return this.resourcesLoadPromise;
}
showUpdateDialog(e, t) {
const a = {
ui_config_path: r.default.UpdateDialog,
ui_config_name: "UpdateDialog",
param: {
forceUpdate: e.forceUpdate,
version: e.version,
continueLoadingCallback: t
}
};
l.default.show_ui(a);
}
onLoadingFinish(e = !1) {
if (this.loadComplete) return;
this.loadComplete = !0;
this.unschedule(this.updateFakeProgress);
this.setLoadingProgress(1);
cc.log("Loading complete: 100%");
const t = () => {
const e = () => {
this.showUpdateDialog(this.pendingUpdateDialog);
this.pendingUpdateDialog = null;
};
this.pendingUpdateDialog && this.pendingUpdateDialog.forceUpdate ? e() : this.pendingUpdateDialog && e();
};
e ? this.scheduleOnce(() => {
t();
}, .3) : this.scheduleOnce(() => {
cc.director.loadScene("GameScene", () => {
t();
});
}, .3);
}
setLoadingProgress(e) {
this.fakeProgress = Math.max(this.fakeProgress, cc.misc.clamp01(e));
this.progress && (this.progress.progress = this.fakeProgress);
this.loadingtxt && (this.loadingtxt.string = (100 * this.fakeProgress).toFixed(0) + "%");
}
preloadGameScene() {
if (this.preloadScenePromise) return this.preloadScenePromise;
this.preloadScenePromise = new Promise(e => {
cc.director.preloadScene("GameScene", (e, t) => {
let a = t > 0 ? e / t : 1;
this.setLoadingProgress(.85 + .14 * a);
}, t => {
t && console.log("preload GameScene error", t);
this.setLoadingProgress(.99);
e();
});
});
return this.preloadScenePromise;
}
ensureLocalDataReady() {
return o(this, void 0, void 0, function*() {
if (!d.default.all_config_data.GameData) {
let e = yield this.loadGameDataConfig();
d.default.all_config_data.GameData = e || d.default.globalData;
}
yield this.ensureJsonDataReady();
this.setLoadingProgress(.7);
});
}
ensureJsonDataReady() {
return o(this, void 0, void 0, function*() {
try {
yield d.default.initJsonData();
} catch (e) {
console.log("initJsonData error", e);
}
});
}
loadLocalBlastConfig() {
return new Promise(e => {
cc.resources.load("config/data", cc.JsonAsset, (t, a) => {
if (!t && a) e(a.json); else {
console.log("load local config/data failed", t);
e(null);
}
});
});
}
promiseWithTimeout(e, t, a, i) {
return new Promise(o => {
if (!e || !e.then) {
console.log(i);
o(a);
return;
}
let n = !1, s = setTimeout(() => {
if (!n) {
n = !0;
console.log(i);
o(a);
}
}, t);
e.then(e => {
if (!n) {
n = !0;
clearTimeout(s);
o(e);
}
}).catch(e => {
if (!n) {
n = !0;
clearTimeout(s);
console.log(i, e);
o(a);
}
});
});
}
getGlobalConfig() {
let e = d.default.globalData;
return e && e.global ? e.global : null;
}
getVersionList(e) {
if (!e) return [];
if (!Array.isArray(e)) return [ e.toString() ];
let t = [];
for (let a = 0; a < e.length; a++) null != e[a] && t.push(e[a].toString());
return t;
}
checkupdate() {
let e = d.default.globalData, t = this.getGlobalConfig();
if (!e) {
console.log("checkupdate skip, global config is empty", d.default.globalData);
return null;
}
let a = e.version || t && t.version || {}, i = this.getVersionList(a.version_code || e.version_code || t && t.not_force || e.not_force), o = this.getVersionList(a.force_version_code || e.force_version_code || t && t.force_version || e.force_version);
"" == h.default.version_name && h.default.getgetDevicePrams();
let n = (h.default.version_name || "").trim();
if (!n) {
console.log("checkupdate skip, version name is empty");
return null;
}
n = this.normalizeVersionValue(n);
console.log("this.version_name   ", n, o, i);
return o.includes(n) ? {
forceUpdate: !0,
version: n.toString()
} : i.includes(n) ? {
forceUpdate: !1,
version: n.toString()
} : null;
}
normalizeVersionValue(e) {
return e ? e.trim().replace(/^v/i, "") : "";
}
onDestroy() {
this.unschedule(this.updateFakeProgress);
}
onEnable() {}
};
i([ g(cc.Label) ], p.prototype, "loadingtxt", void 0);
i([ g(cc.ProgressBar) ], p.prototype, "progress", void 0);
p = i([ u ], p);
a.default = p;
cc._RF.pop();
};
