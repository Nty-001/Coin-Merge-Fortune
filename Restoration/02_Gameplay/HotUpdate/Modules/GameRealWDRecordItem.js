// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: GameRealWDRecordItem; dependency map: {"../BaseUIManager/Storage/TouchButton":"TouchButton","../LanguageControl/GameManagement":"GameManagement","../LanguageControl/Lab":"Lab"}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "e0aa9cHA7JEWY95OrkY+hoK", "GameRealWDRecordItem");
var i = this && this.__decorate || function(e, t, a, i) {
var o, n = arguments.length, s = n < 3 ? t : null === i ? i = Object.getOwnPropertyDescriptor(t, a) : i;
if ("object" == typeof Reflect && "function" == typeof Reflect.decorate) s = Reflect.decorate(e, t, a, i); else for (var r = e.length - 1; r >= 0; r--) (o = e[r]) && (s = (n < 3 ? o(s) : n > 3 ? o(t, a, s) : o(t, a)) || s);
return n > 3 && s && Object.defineProperty(t, a, s), s;
};
Object.defineProperty(a, "__esModule", {
value: !0
});
const o = e("../BaseUIManager/Storage/TouchButton"), n = e("../LanguageControl/GameManagement"), s = e("../LanguageControl/Lab"), {ccclass: r, property: l} = cc._decorator;
let c = class extends cc.Component {
constructor() {
super(...arguments);
this.tibg = null;
this.timeLabel = null;
this.coinLabel = null;
this.successLabel = null;
this.jiantouNode = null;
this.nodeShow = null;
this.tips1 = null;
this.tips1_1 = null;
this.tips2 = null;
this.tips2_1 = null;
this.tips3 = null;
this.tips3_1 = null;
this.tips4 = null;
this.tips4_1 = null;
this.tips5 = null;
this.tips5_1 = null;
this.tips6 = null;
this.tips6_1 = null;
this.tips7 = null;
this.tips7_1 = null;
this.tips8 = null;
this.tips8_1 = null;
this.faildNode = null;
this.faildTips = null;
this.isExpanded = !1;
}
onLoad() {
this.jiantouNode && (this.jiantouNode.angle = 0);
this.jiantouNode.addComponent(o.default).registerTouchEvent(() => {
this.onToggleClick();
});
}
initdata(e, t) {
this.timeLabel.string = e.time;
this.coinLabel.string = n.default.getRealMonstr(e.amount);
let a = "102", i = new cc.Color(168, 68, 4);
if (1 == e.withdrawStatus) {
a = "102";
i = new cc.Color(16, 191, 86);
} else if (-1 == e.withdrawStatus) {
a = "103";
i = new cc.Color(255, 0, 0);
}
this.successLabel.string = s.default.getlab(a);
this.tips8_1.string = s.default.getlab(a);
this.tips8_1.node.color = i;
this.tips1.string = s.default.getlab("94");
this.tips2.string = s.default.getlab("95");
this.tips3.string = s.default.getlab("96");
this.tips4.string = s.default.getlab("97");
this.tips5.string = s.default.getlab("98");
this.tips6.string = s.default.getlab("99");
this.tips7.string = s.default.getlab("100");
this.tips8.string = s.default.getlab("100");
this.tips1_1.string = e.time;
this.tips2_1.string = e.fullName;
this.tips3_1.string = e.accountType;
this.tips4_1.string = e.CPF;
this.tips5_1.string = e.amount;
this.tips6_1.string = e.withdrawDetails;
this.tips7_1.string = e.fee;
this.tips5.node.active = 1 == e.withdrawStatus;
this.faildNode.active = -1 == e.withdrawStatus;
this.faildTips.string = s.default.getlab("104");
this.updateUIState();
}
onToggleClick() {
this.isExpanded = !this.isExpanded;
this.updateUIState();
}
updateUIState() {
this.nodeShow && (this.nodeShow.active = this.isExpanded);
this.jiantouNode && (this.jiantouNode.angle = this.isExpanded ? 0 : 180);
this.requestParentLayoutUpdate();
}
requestParentLayoutUpdate() {
this.scheduleOnce(() => {
const e = this.node.getComponent(cc.Layout);
e && e.updateLayout();
if (this.node.parent) {
const e = this.node.parent.getComponent(cc.Layout);
e && e.updateLayout();
}
}, 0);
}
};
i([ l(cc.Node) ], c.prototype, "tibg", void 0);
i([ l(cc.Label) ], c.prototype, "timeLabel", void 0);
i([ l(cc.Label) ], c.prototype, "coinLabel", void 0);
i([ l(cc.Label) ], c.prototype, "successLabel", void 0);
i([ l(cc.Node) ], c.prototype, "jiantouNode", void 0);
i([ l(cc.Node) ], c.prototype, "nodeShow", void 0);
i([ l(cc.Label) ], c.prototype, "tips1", void 0);
i([ l(cc.Label) ], c.prototype, "tips1_1", void 0);
i([ l(cc.Label) ], c.prototype, "tips2", void 0);
i([ l(cc.Label) ], c.prototype, "tips2_1", void 0);
i([ l(cc.Label) ], c.prototype, "tips3", void 0);
i([ l(cc.Label) ], c.prototype, "tips3_1", void 0);
i([ l(cc.Label) ], c.prototype, "tips4", void 0);
i([ l(cc.Label) ], c.prototype, "tips4_1", void 0);
i([ l(cc.Label) ], c.prototype, "tips5", void 0);
i([ l(cc.Label) ], c.prototype, "tips5_1", void 0);
i([ l(cc.Label) ], c.prototype, "tips6", void 0);
i([ l(cc.Label) ], c.prototype, "tips6_1", void 0);
i([ l(cc.Label) ], c.prototype, "tips7", void 0);
i([ l(cc.Label) ], c.prototype, "tips7_1", void 0);
i([ l(cc.Label) ], c.prototype, "tips8", void 0);
i([ l(cc.Label) ], c.prototype, "tips8_1", void 0);
i([ l(cc.Node) ], c.prototype, "faildNode", void 0);
i([ l(cc.Label) ], c.prototype, "faildTips", void 0);
c = i([ r ], c);
a.default = c;
cc._RF.pop();
};
