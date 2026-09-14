// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: FailDialog; dependency map: {"../BaseUIManager/BaseUI":"BaseUI","../BaseUIManager/FlyAnimation":"FlyAnimation","../BaseUIManager/Storage/GameLocalData":"GameLocalData","../BaseUIManager/Storage/PlayData":"PlayData","../BaseUIManager/Storage/SoundManager":"SoundManager","../BaseUIManager/Storage/TouchButton":"TouchButton","../HWL/HWL_TStool":"HWL_TStool","../LanguageControl/Lab":"Lab","./RewardDialog":"RewardDialog"}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "7d8dfcsiFtHHq6eTEbbJqMf", "FailDialog");
var i = this && this.__decorate || function(e, t, a, i) {
var o, n = arguments.length, s = n < 3 ? t : null === i ? i = Object.getOwnPropertyDescriptor(t, a) : i;
if ("object" == typeof Reflect && "function" == typeof Reflect.decorate) s = Reflect.decorate(e, t, a, i); else for (var r = e.length - 1; r >= 0; r--) (o = e[r]) && (s = (n < 3 ? o(s) : n > 3 ? o(t, a, s) : o(t, a)) || s);
return n > 3 && s && Object.defineProperty(t, a, s), s;
};
Object.defineProperty(a, "__esModule", {
value: !0
});
const o = e("../BaseUIManager/BaseUI"), n = e("../BaseUIManager/FlyAnimation"), s = e("../BaseUIManager/Storage/GameLocalData"), r = e("../BaseUIManager/Storage/PlayData"), l = e("../BaseUIManager/Storage/SoundManager"), c = e("../BaseUIManager/Storage/TouchButton"), d = e("../HWL/HWL_TStool"), h = e("../LanguageControl/Lab"), u = e("./RewardDialog"), {ccclass: g, property: p} = cc._decorator;
let f = class extends o.default {
constructor() {
super(...arguments);
this._closeCallback = null;
this.isWatchingAd = !1;
this.reAliveTouchButton = null;
}
onLoad() {
super.onLoad();
let e = this.reAliveLabel.node.parent;
this.reAliveTouchButton = e.getComponent(c.default) || e.addComponent(c.default);
this.reAliveTouchButton.registerTouchEvent(() => {
this.onClickReAliveAd();
});
this.btnclose.addComponent(c.default).registerTouchEvent(() => {
this.setResetGame();
});
}
setResetGame() {
let e = this._closeCallback;
this.on_close_call();
e && e();
}
onClickReAliveAd() {
if (!this.isWatchingAd) if (cc.sys.isMobile) {
this.setWatchingAdState(!0);
d.HWLshowAd("3_A", e => {
this.completeReAliveReward(e);
}, () => {
this.setWatchingAdState(!1);
}) || this.setWatchingAdState(!1);
} else this.completeReAliveReward(0);
}
completeReAliveReward(e) {
if (this.node && this.node.isValid) {
this.setWatchingAdState(!1);
this.on_close_call();
u.default.open_with_parm({
showType: 1
});
}
}
setWatchingAdState(e) {
this.isWatchingAd = e;
this.reAliveTouchButton && (this.reAliveTouchButton.canTouch = !e);
let t = this.reAliveLabel && this.reAliveLabel.node ? this.reAliveLabel.node.parent : null;
t && (t.opacity = e ? 180 : 255);
}
tryShowRealCoinEffect(e) {
if (e && !(e <= 0) && n.default.instance && this.node && this.node.isValid) try {
n.default.instance.ShowRealCoinEffect(e, this.node);
} catch (e) {
console.warn("FailDialog ShowRealCoinEffect error", e);
}
}
show(e) {
super.show(e);
this.setWatchingAdState(!1);
l.default.playSound("fail");
const t = s.default.getInstance().getData(r.default);
this.titleLabel1.string = h.default.getlab("29");
this.titleLabel2.string = h.default.getlab("30");
this.reAliveLabel.string = h.default.getlab("33");
this.scoreTxt.string = t.roundScore + "";
this.maxHistoryScoreTxt.string = t.histroyMaxScore + "";
this.get1024Txt.string = "×" + t.coin1024Number;
this.maxHistoryScoreLabel.string = h.default.getlab("31") + ":";
this.get1024Label.string = h.default.getlab("32") + ":";
this._closeCallback = null;
let a = e.param;
(null == a ? void 0 : a.closeCallback) && (this._closeCallback = a.closeCallback);
if (this.aliveBtn) {
cc.Tween.stopAllByTarget(this.aliveBtn);
cc.tween(this.aliveBtn).repeatForever(cc.tween().to(1, {
scale: 1.05
}, {
easing: "sineInOut"
}).to(1, {
scale: 1
}, {
easing: "sineInOut"
})).start();
}
}
};
i([ p(cc.Node) ], f.prototype, "maskNode", void 0);
i([ p(cc.Label) ], f.prototype, "titleLabel1", void 0);
i([ p(cc.Label) ], f.prototype, "titleLabel2", void 0);
i([ p(cc.Label) ], f.prototype, "scoreTxt", void 0);
i([ p(cc.Label) ], f.prototype, "reAliveLabel", void 0);
i([ p(cc.Label) ], f.prototype, "maxHistoryScoreLabel", void 0);
i([ p(cc.Label) ], f.prototype, "maxHistoryScoreTxt", void 0);
i([ p(cc.Label) ], f.prototype, "get1024Label", void 0);
i([ p(cc.Label) ], f.prototype, "get1024Txt", void 0);
i([ p(cc.Node) ], f.prototype, "aliveBtn", void 0);
i([ p(cc.Node) ], f.prototype, "btnclose", void 0);
f = i([ o.registerUIPath("GameDialog/FailDialog"), g ], f);
a.default = f;
cc._RF.pop();
};
