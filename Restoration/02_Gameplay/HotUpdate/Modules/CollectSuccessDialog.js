// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: CollectSuccessDialog; dependency map: {"../BaseUIManager/BaseUI":"BaseUI","../BaseUIManager/FlyAnimation":"FlyAnimation","../BaseUIManager/Storage/GameLocalData":"GameLocalData","../BaseUIManager/Storage/NewGamePlayData":"NewGamePlayData","../BaseUIManager/Storage/TouchButton":"TouchButton","../BaseUIManager/UIConfig":"UIConfig","../BaseUIManager/UIManagerNew":"UIManagerNew","../HWL/HWL_TStool":"HWL_TStool","../LanguageControl/GameManagement":"GameManagement","../LanguageControl/Lab":"Lab"}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "fa0ca5xAbxBy5jn39XS4EOG", "CollectSuccessDialog");
var i = this && this.__decorate || function(e, t, a, i) {
var o, n = arguments.length, s = n < 3 ? t : null === i ? i = Object.getOwnPropertyDescriptor(t, a) : i;
if ("object" == typeof Reflect && "function" == typeof Reflect.decorate) s = Reflect.decorate(e, t, a, i); else for (var r = e.length - 1; r >= 0; r--) (o = e[r]) && (s = (n < 3 ? o(s) : n > 3 ? o(t, a, s) : o(t, a)) || s);
return n > 3 && s && Object.defineProperty(t, a, s), s;
};
Object.defineProperty(a, "__esModule", {
value: !0
});
const o = e("../BaseUIManager/BaseUI"), n = e("../BaseUIManager/FlyAnimation"), s = e("../BaseUIManager/Storage/GameLocalData"), r = e("../BaseUIManager/Storage/NewGamePlayData"), l = e("../BaseUIManager/Storage/TouchButton"), c = e("../BaseUIManager/UIConfig"), d = e("../BaseUIManager/UIManagerNew"), h = e("../HWL/HWL_TStool"), u = e("../LanguageControl/GameManagement"), g = e("../LanguageControl/Lab"), {ccclass: p, property: f} = cc._decorator;
let m = class extends o.default {
constructor() {
super(...arguments);
this.titleLabel = null;
this.CaiDai_TX = null;
this.QiQiuBiaoT_TX = null;
this.coin_label = null;
this.videoLabel = null;
this.restartLabel = null;
this._coin = 0;
}
onLoad() {
super.onLoad();
this.videoLabel.node.parent.addComponent(l.default).registerTouchEvent(() => {
h.HWLshowAd("1_A", e => {
let t = u.default.cashCoin(e);
n.default.instance.ShowRealCoinEffect(t, this.node);
n.default.instance.ShowCoinEffect(this._coin, this.node);
this.showRewardView();
}, () => {});
});
this.restartLabel.node.addComponent(l.default).registerTouchEvent(() => {
this.showRewardView();
});
}
show(e) {
super.show(e);
e.param;
this.unscheduleAllCallbacks();
this.titleLabel.string = g.default.getlab("8");
this.videoLabel.string = g.default.getlab("4");
this.restartLabel.string = g.default.getlab("5");
let t = u.default.getnor(3);
this._coin = t;
this.coin_label.string = `${u.default.getmonstr(this._coin)}`;
}
showRewardView(e = !1) {
this.on_close_call();
this.showScoreDialog();
}
showScoreDialog() {
if (cc.sys.os == cc.sys.OS_ANDROID && cc.sys.isNative) return;
let e = s.default.getInstance().getData(r.default);
if (3 == e.Passlevel || 11 == e.Passlevel && 0 == e.scoreCount) {
const e = {
ui_config_path: c.default.ScoreDialog,
ui_config_name: "ScoreDialog",
param: {
closeCallback: () => {}
}
};
d.default.show_ui(e);
}
}
};
i([ f(cc.Label) ], m.prototype, "titleLabel", void 0);
i([ f(sp.Skeleton) ], m.prototype, "CaiDai_TX", void 0);
i([ f(sp.Skeleton) ], m.prototype, "QiQiuBiaoT_TX", void 0);
i([ f(cc.Label) ], m.prototype, "coin_label", void 0);
i([ f(cc.Label) ], m.prototype, "videoLabel", void 0);
i([ f(cc.Label) ], m.prototype, "restartLabel", void 0);
m = i([ o.registerUIPath("GameDialog/CollectSuccessDialog"), p ], m);
a.default = m;
cc._RF.pop();
};
