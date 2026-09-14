// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: GameWDValidate1; dependency map: {"../BaseUIManager/BaseUI":"BaseUI","../BaseUIManager/Storage/TouchButton":"TouchButton","../BaseUIManager/UIConfig":"UIConfig","../BaseUIManager/UIManagerNew":"UIManagerNew","../LanguageControl/Lab":"Lab"}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "c03bbXmoPNAM4u7nUMRzmH+", "GameWDValidate1");
var i = this && this.__decorate || function(e, t, a, i) {
var o, n = arguments.length, s = n < 3 ? t : null === i ? i = Object.getOwnPropertyDescriptor(t, a) : i;
if ("object" == typeof Reflect && "function" == typeof Reflect.decorate) s = Reflect.decorate(e, t, a, i); else for (var r = e.length - 1; r >= 0; r--) (o = e[r]) && (s = (n < 3 ? o(s) : n > 3 ? o(t, a, s) : o(t, a)) || s);
return n > 3 && s && Object.defineProperty(t, a, s), s;
};
Object.defineProperty(a, "__esModule", {
value: !0
});
const o = e("../LanguageControl/Lab"), n = e("../BaseUIManager/Storage/TouchButton"), s = e("../BaseUIManager/UIConfig"), r = e("../BaseUIManager/UIManagerNew"), l = e("../BaseUIManager/BaseUI"), {ccclass: c, property: d} = cc._decorator;
let h = class extends l.default {
constructor() {
super(...arguments);
this.title = null;
this.tips = null;
this.btnLabel = null;
this.closeBtn = null;
this.bg = null;
this.progress = null;
}
onLoad() {
super.onLoad();
this.closeBtn.addComponent(n.default).registerTouchEvent(() => {
this.call_back();
});
this.btnLabel.node.parent.addComponent(n.default).registerTouchEvent(() => {
this.call_back();
});
}
call_back() {
const e = {
ui_config_path: s.default.GameWDValidate2,
ui_config_name: "GameWDValidate2",
param: {
withdrawAmount: this.param.withdrawAmount,
currentIndex: this.param.currentIndex
}
};
r.default.show_ui(e);
this.on_close_call();
}
show(e) {
super.show(e);
this.param = e.param;
this.title.string = o.default.getlab("48");
this.tips.string = o.default.getlab("50");
this.btnLabel.string = o.default.getlab("1");
cc.Tween.stopAllByTarget(this.progress);
this.progress.progress = 0;
cc.tween(this.progress).to(2, {
progress: 1
}, {
easing: cc.easing.backOut
}).start();
}
};
i([ d(cc.Label) ], h.prototype, "title", void 0);
i([ d(cc.Label) ], h.prototype, "tips", void 0);
i([ d(cc.Label) ], h.prototype, "btnLabel", void 0);
i([ d(cc.Node) ], h.prototype, "closeBtn", void 0);
i([ d(cc.Node) ], h.prototype, "bg", void 0);
i([ d(cc.ProgressBar) ], h.prototype, "progress", void 0);
h = i([ l.registerUIPath("GameDialog/GameWDValidate1"), c ], h);
a.default = h;
cc._RF.pop();
};
