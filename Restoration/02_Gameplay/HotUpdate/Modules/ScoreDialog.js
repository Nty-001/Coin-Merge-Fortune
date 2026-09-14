// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: ScoreDialog; dependency map: {"../BaseUIManager/BaseUI":"BaseUI","../BaseUIManager/Storage/GameLocalData":"GameLocalData","../BaseUIManager/Storage/PlayData":"PlayData","../BaseUIManager/Storage/TouchButton":"TouchButton","../HWL/ServerConfig":"ServerConfig","../LanguageControl/Lab":"Lab"}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "b5537mriERGJIi6Jl8l6NYV", "ScoreDialog");
var i = this && this.__decorate || function(e, t, a, i) {
var o, n = arguments.length, s = n < 3 ? t : null === i ? i = Object.getOwnPropertyDescriptor(t, a) : i;
if ("object" == typeof Reflect && "function" == typeof Reflect.decorate) s = Reflect.decorate(e, t, a, i); else for (var r = e.length - 1; r >= 0; r--) (o = e[r]) && (s = (n < 3 ? o(s) : n > 3 ? o(t, a, s) : o(t, a)) || s);
return n > 3 && s && Object.defineProperty(t, a, s), s;
};
Object.defineProperty(a, "__esModule", {
value: !0
});
const o = e("../BaseUIManager/BaseUI"), n = e("../BaseUIManager/Storage/GameLocalData"), s = e("../BaseUIManager/Storage/PlayData"), r = e("../BaseUIManager/Storage/TouchButton"), l = e("../HWL/ServerConfig"), c = e("../LanguageControl/Lab"), {ccclass: d, property: h} = cc._decorator;
let u = class extends o.default {
constructor() {
super(...arguments);
this.startNode = null;
this.titleLabel = null;
this.close = null;
this.btn = null;
this.tips = null;
this.Label1 = null;
this._closeCallback = null;
this.startIndex = 0;
this.normalBtnLabelColor = new cc.Color(12, 122, 1);
}
onLoad() {
this.close.addComponent(r.default).registerTouchEvent(() => {
this.on_close_call();
this._closeCallback && this._closeCallback();
});
this.btn.addComponent(r.default).registerTouchEvent(() => {
this.setCLosePage();
});
}
setCLosePage() {
if (this.startIndex >= 3) {
l.HWLServerConfig.gotoMarket();
n.default.getInstance().getData(s.default).gameRateScore = this.startIndex;
}
this.on_close_call();
this._closeCallback && this._closeCallback();
}
onitemRefresh(e) {
for (let t = 0; t <= 4; t++) {
this.startNode.getChildByName(`start${t}`).opacity = t <= e ? 255 : 0;
}
this.startIndex = e;
n.default.getInstance().set_local_storeage();
}
show(e) {
super.show(e);
let t = e.param;
this.titleLabel.string = c.default.getlab("65");
this.tips.string = c.default.getlab("64");
this.Label1.string = c.default.getlab("92");
this._closeCallback = null;
(null == t ? void 0 : t.closeCallback) && (this._closeCallback = t.closeCallback);
for (let e = 0; e <= 4; e++) {
const t = this.startNode.getChildByName(`start${e}`);
if (t) {
const a = t.addComponent(r.default);
a && a.registerTouchEvent(() => {
this.onitemRefresh(e);
});
t.opacity = 0;
}
}
}
};
i([ h(cc.Node) ], u.prototype, "startNode", void 0);
i([ h(cc.Label) ], u.prototype, "titleLabel", void 0);
i([ h(cc.Node) ], u.prototype, "close", void 0);
i([ h(cc.Node) ], u.prototype, "btn", void 0);
i([ h(cc.Label) ], u.prototype, "tips", void 0);
i([ h(cc.Label) ], u.prototype, "Label1", void 0);
u = i([ o.registerUIPath("GameDialog/ScoreDialog"), d ], u);
a.default = u;
cc._RF.pop();
};
