// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: LoadAllResources; dependency map: {"./ResManager":"ResManager","./Singleton":"Singleton"}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "c6776SSNdROIboBy6ZhS1nO", "LoadAllResources");
Object.defineProperty(a, "__esModule", {
value: !0
});
const i = e("./ResManager"), o = e("./Singleton");
class n {
constructor(e, t, a) {
this.hasStartLoad = !1;
this.completedCount = 0;
this.totalCount = 0;
this.m_resMap = new Map();
this.resPath = e;
this.resType = t;
if (a) this.startLoad(a); else {
this.completedCount = -1;
this.totalCount = -1;
}
}
startLoad(e) {
i.default.loadDir(this.resPath, this.resType, (t, a) => {
if (0 == this.hasStartLoad) {
this.hasStartLoad = !0;
e.startLoadCount++;
}
this.completedCount = t;
this.totalCount = a;
this.totalCount != this.completedCount && e.updateProgress();
}, t => {
if (0 == this.hasStartLoad) {
this.hasStartLoad = !0;
e.startLoadCount++;
}
for (let e = 0; e < t.length; e++) this.m_resMap.set(t[e].name, t[e]);
0 == this.m_resMap.size && (this.hasStartLoad = !0);
e.finishLoadCount++;
e.updateProgress();
});
}
getRes(e) {
return this.m_resMap.has(e) ? this.m_resMap.get(e) : null;
}
setRes(e, t) {
this.m_resMap.set(e, t);
}
removeRes(e) {
this.m_resMap.has(e) && this.m_resMap.delete(e);
}
}
a.default = class extends o.default {
constructor() {
super();
this.allBaseLoader = [];
this.startLoadCount = 0;
this.finishLoadCount = 0;
this.m_progressCallback = null;
this.m_initFinished = !1;
this.has = 0;
this.total = 0;
this.extraHas = 0;
this.extraTotal = 0;
}
loadAllRes(e) {
this.m_progressCallback = e;
const t = new n("Audio", cc.AudioClip, this);
this.allBaseLoader.push(t);
this.m_initFinished = !0;
this.updateProgress();
}
updateProgress() {
var e;
if (!this.m_initFinished) return;
if (this.startLoadCount < this.allBaseLoader.length) return;
for (let e = 0; e < this.allBaseLoader.length; e++) if (0 == this.allBaseLoader[e].totalCount || 0 == this.allBaseLoader[e].completedCount) return;
this.has = 0;
this.total = 0;
this.allBaseLoader.forEach(e => {
this.has += e.completedCount;
this.total += e.totalCount;
});
let t = (this.has + this.extraHas) / (this.total + this.extraTotal);
null === (e = this.m_progressCallback) || void 0 === e || e.call(this, t);
t >= 1 && this.allBaseLoader.forEach(e => {
console.log(e.resPath + ": " + e.completedCount + "   " + e.totalCount);
});
}
getBaseLoader(e) {
let t = null;
for (let a = 0; a < this.allBaseLoader.length; a++) this.allBaseLoader[a].resType == e && (t = this.allBaseLoader[a]);
return t;
}
getAudioRes(e) {
var t;
return null === (t = this.getBaseLoader(cc.AudioClip)) || void 0 === t ? void 0 : t.getRes(e);
}
getPrefabRes(e) {
return this.getBaseLoader(cc.Prefab).getRes(e);
}
getSpriteFrameRes(e) {
var t;
return null === (t = this.getBaseLoader(cc.SpriteFrame)) || void 0 === t ? void 0 : t.getRes(e);
}
getSpineDataRes(e) {
var t;
return null === (t = this.getBaseLoader(sp.SkeletonData)) || void 0 === t ? void 0 : t.getRes(e);
}
};
cc._RF.pop();
};
