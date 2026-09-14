// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/base/assets/assets/main/index.ba75b.js
// Module: HomeView; dependency map: {"../ATools/BaseUI":"BaseUI","../ATools/TouchButton":"TouchButton","../ATools/UIManager":"UIManager","./Item":"Item"}
module.exports = function(e, t, o) {
"use strict";
cc._RF.push(t, "4f92d79EGJA8YYHtb2CA/I4", "HomeView");
var a = this && this.__decorate || function(e, t, o, a) {
var s, n = arguments.length, i = n < 3 ? t : null === a ? a = Object.getOwnPropertyDescriptor(t, o) : a;
if ("object" == typeof Reflect && "function" == typeof Reflect.decorate) i = Reflect.decorate(e, t, o, a); else for (var c = e.length - 1; c >= 0; c--) (s = e[c]) && (i = (n < 3 ? s(i) : n > 3 ? s(t, o, i) : s(t, o)) || i);
return n > 3 && i && Object.defineProperty(t, o, i), i;
};
Object.defineProperty(o, "__esModule", {
value: !0
});
const s = e("../ATools/BaseUI"), n = e("../ATools/TouchButton"), i = e("../ATools/UIManager"), c = e("./Item"), {ccclass: l, property: r} = cc._decorator;
let h = class extends cc.Component {
constructor() {
super(...arguments);
this.items = [];
this.btn_set = null;
this.btn_honor = null;
}
onLoad() {
this.btn_set.addComponent(n.default).registerTouchEvent(() => {
i.default.instance.onShowView(s.UIConfig.SettingView, {
type: 1
});
});
this.btn_honor.addComponent(n.default).registerTouchEvent(() => {
i.default.instance.onShowView(s.UIConfig.HonorView, {
type: 1
});
});
}
start() {
this.initView();
}
initView() {
this.items.forEach((e, t) => {
e.initItem(t);
});
}
};
a([ r(c.default) ], h.prototype, "items", void 0);
a([ r(cc.Node) ], h.prototype, "btn_set", void 0);
a([ r(cc.Node) ], h.prototype, "btn_honor", void 0);
h = a([ l ], h);
o.default = h;
cc._RF.pop();
};
