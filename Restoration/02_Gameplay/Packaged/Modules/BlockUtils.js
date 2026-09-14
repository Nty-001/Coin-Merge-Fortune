// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/base/assets/assets/main/index.ba75b.js
// Module: BlockUtils; dependency map: {"./Block":"Block","./PoolManager":"PoolManager","./Utils":"Utils"}
module.exports = function(e, t, o) {
"use strict";
cc._RF.push(t, "bf6cfka9uBPAZcvwmqaqzHM", "BlockUtils");
Object.defineProperty(o, "__esModule", {
value: !0
});
const a = e("./Block"), s = e("./PoolManager"), n = e("./Utils");
class i {
static init() {
const e = this;
for (let t = 0; t < i.blockIds.length; t++) {
let o = "Block/" + `block_${i.blockIds[t]}`;
n.default.loadRes(o, cc.Prefab).then(o => {
e.m_resMap.set(`block_${i.blockIds[t]}`, o);
});
}
}
static createAllInitBlocks() {
for (let e = 0; e < i.blockIds.length; e++) {
let t = this.getPrefabRes(`block_${i.blockIds[e]}`), o = cc.instantiate(t);
i.initBlocks.push(o);
}
}
static createBlock(e, t, o) {
const s = i.getPrefabNode(o);
let n = s.getComponent(a.default);
n || (n = s.addComponent(a.default));
s.setPosition(t);
s.scale = i.getOriginScale(o.type);
s.parent = e;
n.init(o);
return n;
}
static getPrefabNode(e) {
const t = `Prefab/Game/GameObject/Block/block_${e.type}`.split("/").pop();
let o = null;
if (e.type < a.BlockType.type_luckyBag && s.poolManager.getPool("Block").size() > 0) {
e.fromPool = !0;
o = s.poolManager.getNode("Block");
let t = i.blockIds.indexOf(e.type);
o.getComponent(cc.PhysicsPolygonCollider).points = i.initBlocks[t].getComponent(cc.PhysicsPolygonCollider).points;
o.getComponent(cc.PhysicsPolygonCollider).apply();
o.children[0].getComponent(cc.Sprite).spriteFrame = i.initBlocks[t].getComponent(cc.Sprite).spriteFrame;
} else {
const e = this.getPrefabRes(t);
o = cc.instantiate(e);
}
if (!o || !cc.isValid(o)) {
const e = this.getPrefabRes(t);
o = cc.instantiate(e);
}
return o;
}
static getPrefabRes(e) {
return this.m_resMap.has(e) ? this.m_resMap.get(e) : null;
}
static getOriginScale(e) {
return {
1: 1,
2: 1,
3: 1,
4: 1,
5: 1,
6: 1,
7: 1,
8: 1,
9: 1,
10: 1,
11: 1,
100: 1
}[e] || .8;
}
}
o.default = i;
i.m_resMap = new Map();
i.initBlocks = [];
i.blockIds = [ 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 ];
cc._RF.pop();
};
