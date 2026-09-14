// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: BaoXiangOkSpin; dependency map: {"../BaseUIManager/Storage/TouchButton":"TouchButton","../Report/NativeCall":"NativeCall"}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "11ec6tQOrFHlYdx80pSVTST", "BaoXiangOkSpin");
var i, o = this && this.__decorate || function(e, t, a, i) {
var o, n = arguments.length, s = n < 3 ? t : null === i ? i = Object.getOwnPropertyDescriptor(t, a) : i;
if ("object" == typeof Reflect && "function" == typeof Reflect.decorate) s = Reflect.decorate(e, t, a, i); else for (var r = e.length - 1; r >= 0; r--) (o = e[r]) && (s = (n < 3 ? o(s) : n > 3 ? o(t, a, s) : o(t, a)) || s);
return n > 3 && s && Object.defineProperty(t, a, s), s;
};
Object.defineProperty(a, "__esModule", {
value: !0
});
const n = e("../BaseUIManager/Storage/TouchButton"), s = e("../Report/NativeCall"), {ccclass: r, property: l} = cc._decorator;
let c = i = class extends cc.Component {
constructor() {
super(...arguments);
this.btn_box1 = null;
this.btn_box2 = null;
this.btn_hand = null;
this.boxSwitchInterval = 61;
this.lastSwitchTime = 0;
this.currentBoxType = 0;
}
onLoad() {
i.Instance = this;
this.schedule(() => {
this.tryAddBox();
}, 120, cc.macro.REPEAT_FOREVER, .01);
this.btn_box1.parent.addComponent(n.default).registerTouchEvent(() => {
s.default.gameTopCome(2);
});
this.btn_box2.parent.addComponent(n.default).registerTouchEvent(() => {
s.default.gameTopCome(1);
});
}
tryAddBox() {
let e = s.default.gameTopShows();
if ("string" != typeof e) {
this.btn_box1.parent.active || (this.btn_box1.active = s.default.gameTopShows());
return;
}
let t = [];
"" !== e && (t = e.split(",").map(Number));
if (0 === t.length) {
this.btn_box1.parent.active = !1;
this.btn_box2.parent.active = !1;
return;
}
const a = Date.now() / 1e3;
1 === t.length ? this.currentBoxType = t[0] : 2 === t.length && (this.currentBoxType = 1 === this.currentBoxType ? 2 : 1);
this.lastSwitchTime = a;
this.showHand();
this.btn_box1.parent.active = 2 === this.currentBoxType;
this.btn_box2.parent.active = 1 === this.currentBoxType;
}
showHand() {
this.btn_hand.active = !0;
this.scheduleOnce(() => {
this.btn_hand.active = !1;
}, 3);
}
static getInstance() {
return i.Instance;
}
};
c.Instance = null;
o([ l(cc.Node) ], c.prototype, "btn_box1", void 0);
o([ l(cc.Node) ], c.prototype, "btn_box2", void 0);
o([ l(cc.Node) ], c.prototype, "btn_hand", void 0);
c = i = o([ r ], c);
a.default = c;
cc._RF.pop();
};
