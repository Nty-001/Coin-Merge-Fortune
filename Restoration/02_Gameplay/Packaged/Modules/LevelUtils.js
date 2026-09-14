// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/base/assets/assets/main/index.ba75b.js
// Module: LevelUtils; dependency map: {"../ATools/LocalDataManager":"LocalDataManager","./Block":"Block","./LevelManager":"LevelManager","./Utils":"Utils"}
module.exports = function(e, t, o) {
"use strict";
cc._RF.push(t, "9023fVBmiVE1ppEak4XsY9y", "LevelUtils");
Object.defineProperty(o, "__esModule", {
value: !0
});
const a = e("../ATools/LocalDataManager"), s = e("./Block"), n = e("./LevelManager"), i = e("./Utils");
o.default = class {
static saveBlockToStorage() {
const e = [];
n.default.getInstance().getAllBlock().forEach(t => {
const o = t.node.convertToWorldSpaceAR(cc.Vec2.ZERO);
e.push({
type: t.getBlockType(),
worldX: o.x,
worldY: o.y,
angle: t.node.angle
});
});
a.default.getInstance().getGameData().resetAllBlockStorage(e);
}
static genNextBlockType(e) {
let t = 0;
e = e.filter(e => {
t += e.weigh;
return e.weigh > 0;
});
let o = s.BlockType.type_min, a = i.default.getRandomIntInRange(1, t);
for (let t = 0; t < e.length; t++) {
if (a <= e[t].weigh) {
o = e[t].id;
break;
}
a -= e[t].weigh;
}
return o;
}
};
cc._RF.pop();
};
