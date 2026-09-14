// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: RewardRealToast; dependency map: {"./BaseUI":"BaseUI","./Storage/GameLocalData":"GameLocalData","./Storage/PlayData":"PlayData"}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "e7a9bOSpDhLoq/1qzpLgsqm", "RewardRealToast");
var i = this && this.__decorate || function(e, t, a, i) {
var o, n = arguments.length, s = n < 3 ? t : null === i ? i = Object.getOwnPropertyDescriptor(t, a) : i;
if ("object" == typeof Reflect && "function" == typeof Reflect.decorate) s = Reflect.decorate(e, t, a, i); else for (var r = e.length - 1; r >= 0; r--) (o = e[r]) && (s = (n < 3 ? o(s) : n > 3 ? o(t, a, s) : o(t, a)) || s);
return n > 3 && s && Object.defineProperty(t, a, s), s;
};
Object.defineProperty(a, "__esModule", {
value: !0
});
const o = e("./BaseUI"), n = e("./Storage/GameLocalData"), s = e("./Storage/PlayData"), {ccclass: r, property: l} = cc._decorator;
let c = class extends o.default {
constructor() {
super(...arguments);
this.label_value = null;
}
show(e) {
super.show(e);
this.node.convertToNodeSpaceAR(e.param.endPos);
this.label_value.node.parent.setPosition(cc.v2(0, 0));
let t = n.default.getInstance().getData(s.default);
this.label_value.string = "+ " + t.addVodeoshowCoin.toString();
this.label_value.node.parent.opacity = 255;
this.label_value.node.parent.stopAllActions();
cc.tween(this.label_value.node.parent).to(.3, {
scale: 1
}).delay(.1).parallel(cc.tween().delay(.9).to(.1, {
opacity: 0
}), cc.tween().by(1, {
y: 70
})).by(1, {
y: 70
}).call(() => {
this.label_value.node.parent.opacity = 255;
this.on_close_call();
}).start();
}
baseOnDisable() {}
};
i([ l(cc.Label) ], c.prototype, "label_value", void 0);
c = i([ o.registerUIPath("CommonPrefab/RewardRealToast"), r ], c);
a.default = c;
cc._RF.pop();
};
