// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: GameLocalData; dependency map: {"./PlayData":"PlayData","./Singleton":"Singleton","./UserDefaultManager":"UserDefaultManager"}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "799007kX09IIp0mkDMgpcVG", "GameLocalData");
Object.defineProperty(a, "__esModule", {
value: !0
});
const i = e("./PlayData"), o = e("./Singleton"), n = e("./UserDefaultManager");
class s extends o.default {
constructor() {
super();
this.m_gameCacheData = {};
this.storeKey = "COIN";
this.legacyStoreKey = "BlastFlowPixelBreak_";
}
destoryInstance() {
s.destroyInstance();
}
loadOrRequlestStorageData(e) {
let t = this.getStorageJson(this.getStoreKey()), a = !1;
t || (a = !!(t = this.getStorageJson(this.getLegacyStoreKey())));
if (t) {
this.parseStorageData(t);
a && this.removeLegacyStoreData();
e && e();
} else {
this.parseStorageData();
e && e();
}
}
getStoreKey() {
return this.storeKey;
}
getLegacyStoreKey() {
return this.legacyStoreKey;
}
saveToUserDefault(e) {
n.default.setItem_json(this.getStoreKey(), this.getPersistStorageData());
this.removeLegacyStoreData();
}
parseStorageData(e) {
this.m_gameCacheData = {};
let t = this.getPersistDataTypes();
for (let e = 0; e < t.length; e++) {
let a = t[e], i = new a(), o = a._name;
this.m_gameCacheData[o] = i;
}
for (let a = 0; a < t.length; a++) {
let i = t[a]._name, o = this.getSaveDataByClassName(e, i);
o ? this.m_gameCacheData[i].parseFromUserDefault(o) : this.m_gameCacheData[i].parseEndCallback();
}
this.saveToUserDefault();
}
removeGameLocalDataFromUserdefault() {
n.default.removeItem(this.getStoreKey());
this.removeLegacyStoreData();
}
getData(e) {
let t = e._name;
this.m_gameCacheData[t] || (this.m_gameCacheData[t] = new e());
return this.m_gameCacheData[t];
}
removeData(e) {
let t = e._name;
if (this.m_gameCacheData[t]) {
this.m_gameCacheData[t] = new e();
this.saveToUserDefault();
return !0;
}
return !1;
}
clear_data() {
this.removeGameLocalDataFromUserdefault();
this.parseStorageData();
this.saveToUserDefault();
}
set_local_storeage() {
this.saveToUserDefault();
}
getPersistDataTypes() {
return [ i.default ];
}
getPersistStorageData() {
let e = {}, t = this.getPersistDataTypes();
for (let a = 0; a < t.length; a++) {
let i = t[a]._name;
this.m_gameCacheData[i] && (e[i] = this.m_gameCacheData[i]);
}
return e;
}
getSaveDataByClassName(e, t) {
if (!e) return null;
if (e[t]) return e[t];
if (t == i.default._name && e.NewGamePlayData) {
console.log("GameLocalData migrate NewGamePlayData to PlayData");
return this.convertLegacyNewGamePlayData(e.NewGamePlayData);
}
return null;
}
convertLegacyNewGamePlayData(e) {
let t = {};
if (!e) return t;
for (let a in e) e.hasOwnProperty(a) && (t[a] = e[a]);
null == t.fakeMoney && null != e.red_bag && (t.fakeMoney = e.red_bag);
return t;
}
getStorageJson(e) {
try {
return n.default.getItem_json(e);
} catch (t) {
console.log("GameLocalData get storage json failed", e, t);
return null;
}
}
removeLegacyStoreData() {
this.getLegacyStoreKey() != this.getStoreKey() && n.default.removeItem(this.getLegacyStoreKey());
}
}
a.default = s;
s.savedTimeStamp = 0;
cc._RF.pop();
};
