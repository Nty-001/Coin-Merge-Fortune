// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: DebugComponent; dependency map: {"../BaseUIManager/Storage/GameLocalData":"GameLocalData","../BaseUIManager/Storage/NewGamePlayData":"NewGamePlayData","../BaseUIManager/Storage/TouchButton":"TouchButton","../BaseUIManager/UIConfig":"UIConfig","../BaseUIManager/UIManagerNew":"UIManagerNew"}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "82de4T1DGpEkKEAyZS6oc5K", "DebugComponent");
var i = this && this.__decorate || function(e, t, a, i) {
var o, n = arguments.length, s = n < 3 ? t : null === i ? i = Object.getOwnPropertyDescriptor(t, a) : i;
if ("object" == typeof Reflect && "function" == typeof Reflect.decorate) s = Reflect.decorate(e, t, a, i); else for (var r = e.length - 1; r >= 0; r--) (o = e[r]) && (s = (n < 3 ? o(s) : n > 3 ? o(t, a, s) : o(t, a)) || s);
return n > 3 && s && Object.defineProperty(t, a, s), s;
};
Object.defineProperty(a, "__esModule", {
value: !0
});
const o = e("../CardPlayGame/GameScene"), n = e("../BaseUIManager/UIConfig"), s = e("../BaseUIManager/UIManagerNew"), r = e("../BaseUIManager/Storage/GameLocalData"), l = e("../BaseUIManager/Storage/NewGamePlayData"), c = e("../BaseUIManager/Storage/TouchButton"), {ccclass: d, property: h, menu: u} = cc._decorator;
let g = class extends cc.Component {
constructor() {
super(...arguments);
this.GM = null;
this.bnt_next = null;
this.bnt_top = null;
}
onLoad() {
this.GM.addComponent(c.default).registerTouchEvent(() => {
const e = {
ui_config_path: n.default.DebugDialog,
ui_config_name: "DebugDialog",
param: {
closeCallback: () => {}
}
};
s.default.show_ui(e);
});
this.bnt_next.addComponent(c.default).registerTouchEvent(() => {
this.click_NextGamePlay();
});
this.bnt_top.addComponent(c.default).registerTouchEvent(() => {
this.click_TopGamePlay();
});
}
click_TopGamePlay() {
const e = r.default.getInstance().getData(l.default);
e.Passlevel -= 1;
e.Passlevel < 0 && (e.Passlevel = 0);
r.default.getInstance().saveToUserDefault();
o.default.getInstance().generateBatteries();
}
click_NextGamePlay() {
o.default.getInstance().onLevelComplete();
}
};
i([ h(cc.Node) ], g.prototype, "GM", void 0);
i([ h(cc.Node) ], g.prototype, "bnt_next", void 0);
i([ h(cc.Node) ], g.prototype, "bnt_top", void 0);
g = i([ d, u("测试组件/DebugComponent") ], g);
a.default = g;
cc._RF.pop();
};
