// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: CollectDialog; dependency map: {"../BaseUIManager/BaseUI":"BaseUI","../BaseUIManager/FlyAnimation":"FlyAnimation","../BaseUIManager/Storage/GameLocalData":"GameLocalData","../BaseUIManager/Storage/NewGamePlayData":"NewGamePlayData","../BaseUIManager/Storage/SoundManager":"SoundManager","../BaseUIManager/Storage/TouchButton":"TouchButton","../BaseUIManager/UIConfig":"UIConfig","../BaseUIManager/UIManagerNew":"UIManagerNew","../LanguageControl/GameManagement":"GameManagement","../LanguageControl/Lab":"Lab","./CollectSuccessDialog":"CollectSuccessDialog"}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "c7617LpMHRG+piwK+TUMgc+", "CollectDialog");
var i = this && this.__decorate || function(e, t, a, i) {
var o, n = arguments.length, s = n < 3 ? t : null === i ? i = Object.getOwnPropertyDescriptor(t, a) : i;
if ("object" == typeof Reflect && "function" == typeof Reflect.decorate) s = Reflect.decorate(e, t, a, i); else for (var r = e.length - 1; r >= 0; r--) (o = e[r]) && (s = (n < 3 ? o(s) : n > 3 ? o(t, a, s) : o(t, a)) || s);
return n > 3 && s && Object.defineProperty(t, a, s), s;
};
Object.defineProperty(a, "__esModule", {
value: !0
});
const o = e("../BaseUIManager/Storage/TouchButton"), n = e("../BaseUIManager/BaseUI"), s = e("../LanguageControl/Lab"), r = e("../BaseUIManager/Storage/SoundManager"), l = e("../LanguageControl/GameManagement"), c = e("../BaseUIManager/FlyAnimation"), d = e("../BaseUIManager/Storage/GameLocalData"), h = e("../BaseUIManager/Storage/NewGamePlayData"), u = e("./CollectSuccessDialog"), g = e("../CardPlayGame/GameScene"), p = e("../BaseUIManager/UIConfig"), f = e("../BaseUIManager/UIManagerNew"), {ccclass: m, property: y} = cc._decorator;
let _ = class extends n.default {
constructor() {
super(...arguments);
this.progress = null;
this.progressLabel = null;
this.combo = null;
this.nextLabel = null;
this.tap = null;
this.skeleton = null;
this.hand = null;
this._combo = 0;
this.complete = !1;
this.isAnimationPlaying = !1;
this.animationQueue = 0;
}
onLoad() {
super.onLoad();
this.node.on(cc.Node.EventType.TOUCH_START, () => {
if (this.complete) {
r.default.playSound("collect");
this.hand.active = !1;
this._combo++;
this.skeleton.node.active = !0;
this.animationQueue++;
this.playRandomAnimation();
this.combo.string = s.default.getlab("6") + "  x" + this._combo;
this.combo.node.scale = 1;
this.combo.node.stopAllActions();
cc.tween(this.combo.node).to(.1, {
scale: 1.2
}).to(.1, {
scale: 1
}).start();
if (1 == this._combo) {
d.default.getInstance().getData(h.default).PassCollection = 0;
d.default.getInstance().saveToUserDefault();
this.scheduleOnce(() => {
this.progressLabel.string = "0%";
this.progress.progress = 0;
u.default.open();
this.unscheduleAllCallbacks();
this.on_close_call();
}, 5);
}
}
}, this);
this.nextLabel.node.parent.addComponent(o.default).registerTouchEvent(() => {
this.on_close_call();
this.showScoreDialog();
});
}
showScoreDialog() {
if (cc.sys.os == cc.sys.OS_ANDROID && cc.sys.isNative) return;
let e = d.default.getInstance().getData(h.default);
if (3 == e.Passlevel || 11 == e.Passlevel && 0 == e.scoreCount) {
const e = {
ui_config_path: p.default.ScoreDialog,
ui_config_name: "ScoreDialog",
param: {
closeCallback: () => {}
}
};
f.default.show_ui(e);
}
}
playRandomAnimation() {
if (this.isAnimationPlaying) return;
this.isAnimationPlaying = !0;
const e = [ "idle", "idle1", "idle2" ], t = e[Math.floor(Math.random() * e.length)];
this.skeleton.setAnimation(0, t, !1);
this.skeleton.setCompleteListener(() => {
this.isAnimationPlaying = !1;
if (this.animationQueue > 1) {
this.animationQueue--;
this.playRandomAnimation();
} else this.animationQueue = 0;
});
}
show(e) {
super.show(e);
let t = l.default.globalData.global.collect_progress_data, a = g.default.getInstance().blockLevelCount * t, i = d.default.getInstance().getData(h.default);
i.PassCollection += a;
d.default.getInstance().saveToUserDefault();
this.skeleton.node.active = !1;
this._combo = 0;
this.complete = i.PassCollection >= 100;
this.nextLabel.node.parent.active = !this.complete;
this.combo.string = "";
this.tap.node.active = this.complete;
this.hand.active = this.complete;
let o = Math.min(i.PassCollection / 100, 1), n = this.progress.progress;
cc.tween(this.progress).to(.5, {
progress: o
}, {
easing: "linear",
onUpdate: (e, t) => {
let a = n + (o - n) * t;
this.progressLabel.string = Math.floor(100 * a).toFixed(0) + "%";
}
}).start();
this.tap.string = s.default.getlab("7");
this.nextLabel.string = s.default.getlab("5");
e.param;
}
showSingleRewardView() {
this.on_close_call();
let e = l.default.getnor(.5);
c.default.instance.ShowCoinEffect(e, this.node);
}
};
i([ y(cc.ProgressBar) ], _.prototype, "progress", void 0);
i([ y(cc.Label) ], _.prototype, "progressLabel", void 0);
i([ y(cc.Label) ], _.prototype, "combo", void 0);
i([ y(cc.Label) ], _.prototype, "nextLabel", void 0);
i([ y(cc.Label) ], _.prototype, "tap", void 0);
i([ y(sp.Skeleton) ], _.prototype, "skeleton", void 0);
i([ y(cc.Node) ], _.prototype, "hand", void 0);
_ = i([ n.registerUIPath("GameDialog/CollectDialog"), m ], _);
a.default = _;
cc._RF.pop();
};
