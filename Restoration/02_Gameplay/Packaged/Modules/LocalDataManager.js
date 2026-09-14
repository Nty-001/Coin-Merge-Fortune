// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/base/assets/assets/main/index.ba75b.js
// Module: LocalDataManager; dependency map: {"../Game/Utils":"Utils","./GameModel":"GameModel","./Singleton":"Singleton","./UserDefault":"UserDefault"}
module.exports = function(e, t, o) {
"use strict";
cc._RF.push(t, "61871KIZ6hGiLMw60ceFHdv", "LocalDataManager");
Object.defineProperty(o, "__esModule", {
value: !0
});
const a = e("../Game/Utils"), s = e("./GameModel"), n = e("./Singleton"), i = e("./UserDefault");
class c extends n.default {
constructor() {
super();
this.m_gameCacheData = {};
this.next_config = {};
}
destoryInstance() {
c.destroyInstance();
}
loadOrRequlestStorageData(e) {
let t = i.default.getItem_json(this.getStoreKey());
if (t) {
this.parseStorageData(t);
e && e();
} else {
this.parseStorageData();
e && e();
}
}
getStoreKey() {
return "Coin_7826";
}
saveToUserDefault(e) {
i.default.setItem_json(this.getStoreKey(), this.m_gameCacheData);
}
parseStorageData(e) {
this.m_gameCacheData = {};
let t = [ s.default ];
for (let e = 0; e < t.length; e++) {
let o = t[e], a = new o(), s = o._name;
this.m_gameCacheData[s] = a;
}
for (let o = 0; o < t.length; o++) {
let a = t[o]._name;
e && e[a] ? this.m_gameCacheData[a].parseFromUserDefault(e[a]) : this.m_gameCacheData[a].parseEndCallback();
}
this.saveToUserDefault();
}
removeGameLocalDataFromUserdefault() {
i.default.removeItem(this.getStoreKey());
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
getGameData() {
return this.getData(s.default);
}
initConfig() {
return new Promise(e => {
a.default.loadRes("./Json/next_config", cc.JsonAsset).then(t => {
this.next_config = t.json;
e && e({});
}).catch(() => {
e && e({});
});
});
}
getNextBlockItemConfig() {
const e = this.next_config, t = c.getInstance().getGameData().getPassLevel(0);
if (t <= 0) {
const t = c.getInstance().getGameData().mergedMaxLv;
return this.getValueByKey(t, e.firstAppearRule);
}
return this.getValueByKey(t, e.appearRule);
}
getValueByKey(e, t) {
try {
if (!t) return 10;
const o = Object.keys(t);
o.sort((e, t) => parseInt(e) - parseInt(t));
let a = -1;
for (let s = 0; s < o.length; s++) {
-1 == a && (a = t[o[s] + ""]);
e >= parseInt(o[s]) && (a = t[o[s] + ""]);
}
return a;
} catch (e) {
return 1;
}
}
getGMConfig() {
return {
smartDrop: !0,
gravityScale: 80,
linearDamping: .7,
scale: {
1: .7,
2: .75,
3: .7,
4: .65,
5: .8,
6: .82,
7: .82,
8: .85,
9: .9,
10: .88,
11: .88,
100: .8
},
dropGaps: .35,
dropGaps2: .35,
failedFrame: 300
};
}
}
o.default = c;
c.savedTimeStamp = 0;
cc._RF.pop();
};
