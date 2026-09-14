// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/base/assets/assets/main/index.ba75b.js
// Module: BaseUI; dependency map: {"./UIManager":"UIManager"}
module.exports = function(e, t, o) {
"use strict";
cc._RF.push(t, "35d03Mv0U9FQJTwc/zvyIV4", "BaseUI");
var a = this && this.__decorate || function(e, t, o, a) {
var s, n = arguments.length, i = n < 3 ? t : null === a ? a = Object.getOwnPropertyDescriptor(t, o) : a;
if ("object" == typeof Reflect && "function" == typeof Reflect.decorate) i = Reflect.decorate(e, t, o, a); else for (var c = e.length - 1; c >= 0; c--) (s = e[c]) && (i = (n < 3 ? s(i) : n > 3 ? s(t, o, i) : s(t, o)) || i);
return n > 3 && i && Object.defineProperty(t, o, i), i;
};
Object.defineProperty(o, "__esModule", {
value: !0
});
o.registerUIPath = o.UIConfig = void 0;
const s = e("./UIManager");
o.UIConfig = {};
o.registerUIPath = function(e) {
return function(t) {
let a = e.substring(e.lastIndexOf("/") + 1);
o.UIConfig[a] ? console.error(`prefab${a} == ${o.UIConfig[a]}`) : o.UIConfig[a] = e;
t.__UIName = a;
};
};
const {ccclass: n, property: i} = cc._decorator;
let c = class extends cc.Component {
constructor() {
super(...arguments);
this.contentNode = null;
this.params = null;
}
onLoad() {}
onShowUI(e) {
this.params = e.param;
this.node.active = !0;
this.onShow(this.params);
this.contentNode && this.pop(this.contentNode);
}
onClose() {
this.node.active && (this.node.active = !1);
}
onDestroyView() {
delete s.default.instance.all_ui[this.node.name];
this.node.destroy();
}
pop(e) {
return new Promise(t => {
cc.tween(e).call(() => {
e.scale = 0;
}).to(.35, {
scale: 1
}, {
easing: cc.easing.backOut
}).call(() => {
t(!0);
}).start();
});
}
opp(e) {
return new Promise(t => {
cc.tween(e).call(() => {
e.scale = 0;
}).to(.15, {
scale: 1.2
}).to(.1, {
scale: 1
}).call(() => {
t(!0);
}).start();
});
}
};
a([ i(cc.Node) ], c.prototype, "contentNode", void 0);
c = a([ n ], c);
o.default = c;
cc._RF.pop();
};
