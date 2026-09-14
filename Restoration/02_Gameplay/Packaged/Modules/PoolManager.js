// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/base/assets/assets/main/index.ba75b.js
// Module: PoolManager; dependency map: {"./Utils":"Utils"}
module.exports = function(e, t, o) {
"use strict";
cc._RF.push(t, "ac8cbJErGBNmYY8nqQpR7Dy", "PoolManager");
Object.defineProperty(o, "__esModule", {
value: !0
});
o.poolManager = void 0;
const a = e("./Utils");
class s {
constructor() {
this.allPools = new Map();
}
static get instance() {
s._instance || (s._instance = new s());
return s._instance;
}
_parseName(e) {
return e.split("/").pop();
}
getPool(e) {
this.allPools.get(e) || this.allPools.set(e, new cc.NodePool(e));
return this.allPools.get(e);
}
clearPool(e) {
const t = this.getPool(e);
t && t.clear();
}
clearAllPool() {
this.allPools.forEach(e => {
null == e || e.clear();
});
this.allPools.clear();
}
getNode(e) {
const t = this.getPool(e);
if (t.size() > 0) return t.get();
let o = "" + e;
a.default.loadRes(o, cc.Prefab).then(e => cc.instantiate(e));
}
putNode(e) {
this.getPool(e.name).put(e);
}
}
o.poolManager = s.instance;
cc._RF.pop();
};
