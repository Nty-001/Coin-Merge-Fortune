// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: GuidePropsDialog; dependency map: {"../BaseUIManager/BaseUI":"BaseUI","../BaseUIManager/Storage/TouchButton":"TouchButton","../LanguageControl/Lab":"Lab"}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "e8965QjwrVKdZTDiUYGuv0+", "GuidePropsDialog");
var i = this && this.__decorate || function(e, t, a, i) {
var o, n = arguments.length, s = n < 3 ? t : null === i ? i = Object.getOwnPropertyDescriptor(t, a) : i;
if ("object" == typeof Reflect && "function" == typeof Reflect.decorate) s = Reflect.decorate(e, t, a, i); else for (var r = e.length - 1; r >= 0; r--) (o = e[r]) && (s = (n < 3 ? o(s) : n > 3 ? o(t, a, s) : o(t, a)) || s);
return n > 3 && s && Object.defineProperty(t, a, s), s;
};
Object.defineProperty(a, "__esModule", {
value: !0
});
const o = e("../BaseUIManager/BaseUI"), n = e("../BaseUIManager/Storage/TouchButton"), s = e("../CardPlayGame/GameScene"), r = e("../LanguageControl/Lab"), {ccclass: l, property: c} = cc._decorator;
let d = class extends o.default {
constructor() {
super(...arguments);
this.props = [];
this.param = null;
}
onLoad() {
this.btn.node.parent.addComponent(n.default).registerTouchEvent(() => {
this.on_close_call();
"82" == this.param.title ? s.default.getInstance().GuideManager.startStrongGuide("", "", 5) : "78" == this.param.title ? s.default.getInstance().GuideManager.startStrongGuide("", "", 6) : "80" == this.param.title && s.default.getInstance().GuideManager.startStrongGuide("", "", 7);
});
}
show(e) {
super.show(e);
this.param = e.param;
this.titleLabel.string = r.default.getlab(this.param.title);
this.tips.string = r.default.getlab(this.param.tips);
this.btn.string = r.default.getlab("4");
this.props.forEach(e => {
e.active = !1;
});
"82" == this.param.title ? this.props[0].active = !0 : "78" == this.param.title ? this.props[1].active = !0 : "80" == this.param.title && (this.props[2].active = !0);
}
};
i([ c(cc.Label) ], d.prototype, "titleLabel", void 0);
i([ c(cc.Label) ], d.prototype, "btn", void 0);
i([ c(cc.Label) ], d.prototype, "tips", void 0);
i([ c([ cc.Node ]) ], d.prototype, "props", void 0);
d = i([ o.registerUIPath("GameDialog/GuidePropsDialog"), l ], d);
a.default = d;
cc._RF.pop();
};
