// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: RewardDialog; dependency map: {"../BaseUIManager/BaseUI":"BaseUI","../BaseUIManager/Storage/GameLocalData":"GameLocalData","../BaseUIManager/Storage/PlayData":"PlayData","../BaseUIManager/Storage/SoundManager":"SoundManager","../BaseUIManager/Storage/TouchButton":"TouchButton","../GameScene":"GameScene","../LanguageControl/GameManagement":"GameManagement","../LanguageControl/Lab":"Lab","../common/GameUtils":"GameUtils","./GuideDialog":"GuideDialog","./ScoreDialog":"ScoreDialog"}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "820d7wko5lHeazUlAuIrJ/y", "RewardDialog");
var i = this && this.__decorate || function(e, t, a, i) {
var o, n = arguments.length, s = n < 3 ? t : null === i ? i = Object.getOwnPropertyDescriptor(t, a) : i;
if ("object" == typeof Reflect && "function" == typeof Reflect.decorate) s = Reflect.decorate(e, t, a, i); else for (var r = e.length - 1; r >= 0; r--) (o = e[r]) && (s = (n < 3 ? o(s) : n > 3 ? o(t, a, s) : o(t, a)) || s);
return n > 3 && s && Object.defineProperty(t, a, s), s;
};
Object.defineProperty(a, "__esModule", {
value: !0
});
const o = e("../BaseUIManager/BaseUI"), n = e("../BaseUIManager/Storage/GameLocalData"), s = e("../BaseUIManager/Storage/PlayData"), r = e("../BaseUIManager/Storage/SoundManager"), l = e("../BaseUIManager/Storage/TouchButton"), c = e("../common/GameUtils"), d = e("../GameScene"), h = e("../LanguageControl/GameManagement"), u = e("../LanguageControl/Lab"), g = e("./GuideDialog"), p = e("./ScoreDialog"), {ccclass: f, property: m} = cc._decorator;
let y = class extends o.default {
constructor() {
super(...arguments);
this._closeCallback = null;
this.showType = 0;
this.firstStrong = !1;
}
onLoad() {
super.onLoad();
this.maskNode.addComponent(l.default).registerTouchEvent(() => {
if (4 == this.showType) {
d.default.instance.resumePreviewCoin();
this.closePage();
}
if (1 == this.showType) {
d.default.instance.removeTopTwoThirdCoins();
this.closePage();
}
const e = n.default.getInstance().getData(s.default);
if (2 == e.guideStep) {
e.guideStep = 3;
this.closePage();
g.default.open();
}
});
}
start() {}
closePage() {
if (d.default.instance && d.default.instance.isGameOverState() && 1 != this.showType) this.hide(); else {
this.on_close_call();
if (4 != this.showType) {
n.default.getInstance().getData(s.default).fakeMoney += this.fakeMoney;
d.default.instance.setFakeMoney();
this.flyMoneyToGameScene(this.moneyNode.getChildByName("moneyIcon"), this.fakeMoney);
}
this.firstStrong && this.firstShowScore();
}
}
firstShowScore() {
setTimeout(() => {
p.default.open();
n.default.getInstance().getData(s.default).gameRateTimes = 1;
n.default.getInstance().set_local_storeage();
}, 1500);
}
show(e) {
super.show(e);
let t = e.param;
(null == t ? void 0 : t.closeCallback) && (this._closeCallback = t.closeCallback);
this.bgNodeGetReward.active = this.bgNodeGuide.active = this.bgNode2000.active = this.bgNodealiveAndDouble.active = !1;
this.moneyNode = this.bgNodealiveAndDouble;
this.showType = t.showType;
this.fakeMoney = 0;
if (d.default.instance && d.default.instance.isGameOverState() && 1 != this.showType) {
this.unscheduleAllCallbacks();
this.hide();
return;
}
t.firstStrong ? this.firstStrong = t.firstStrong : this.firstStrong = !1;
if (1 == this.showType || 2 == this.showType) {
r.default.playSound("fly_red_bag");
if (1 == this.showType) {
this.fakeMoney = c.gameUtils.calculateCashReward();
this.titleLabeliveAndDouble.string = u.default.getlab("34");
}
if (2 == this.showType) {
this.fakeMoney = 2 * c.gameUtils.calculateCashReward();
this.titleLabeliveAndDouble.string = u.default.getlab("35");
this.scheduleOnce(() => {
this.node && this.closePage();
}, 1.5);
}
this.moneyNode = this.bgNodealiveAndDouble;
this.bgNodealiveAndDouble.active = !0;
} else if (3 == this.showType) {
r.default.playSound("redbag_show");
this.fakeMoney = c.gameUtils.calculateCashReward();
this.titleLabelGetReward.string = u.default.getlab("110");
this.moneyNode = this.bgNodeGetReward;
this.bgNodeGetReward.active = !0;
this.scheduleOnce(() => {
this.node && this.closePage();
}, 1.5);
} else if (4 == this.showType) {
r.default.playSound("reward");
this.fakeMoney = 1;
this.titleLabel2000.string = u.default.getlab("36");
this.moneyNode = this.bgNode2000;
this.bgNode2000.active = !0;
} else if (5 == this.showType) {
r.default.playSound("guide_redbag_show");
this.fakeMoney = h.default.newfake_products.guideMoney;
this.titleLabelGuide.string = u.default.getlab("44");
this.moneyNode = this.bgNodeGuide;
this.bgNodeGuide.active = !0;
}
this.btnLabel.string = u.default.getlab("37");
this.btnGuideLabel.string = u.default.getlab("8");
let a = this.moneyNode.getChildByName("DJB_TX");
a && a.getComponent(sp.Skeleton).setAnimation(0, "animation", !1);
this.moneyNode.getChildByName("plusMoneyTxt").getComponent(cc.Label).string = "+" + h.default.getmonstr(this.fakeMoney);
let i = this.moneyNode.getChildByName("guang");
if (i) {
cc.Tween.stopAllByTarget(i);
i.angle = 0;
cc.tween(i).by(4, {
angle: -360
}).repeatForever().start();
}
}
flyMoneyToGameScene(e, t) {
const a = d.default.instance;
e = this.money;
const i = a.getFakeMoneyIconNode();
if (!i || !i.isValid || !i.parent) return;
const o = cc.director.getScene();
if (!o) return;
const n = i.parent.convertToWorldSpaceAR(i.position), s = o.convertToNodeSpaceAR(n), l = cc.view && cc.view.getVisibleSize ? cc.view.getVisibleSize() : cc.winSize, c = cc.view && cc.view.getVisibleOrigin ? cc.view.getVisibleOrigin() : cc.v2(0, 0), h = cc.v3(c.x + l.width / 2, c.y + l.height / 2, 0), u = o.convertToNodeSpaceAR(h), g = [ cc.v3(0, 14, 0), cc.v3(-26, -10, 0), cc.v3(26, -12, 0) ], p = [ {
targetOffset: cc.v3(0, 4, 0),
arcLift: 170
}, {
targetOffset: cc.v3(-12, 8, 0),
arcLift: 145
}, {
targetOffset: cc.v3(12, 6, 0),
arcLift: 150
} ];
let f = 0, m = !1;
for (let n = 0; n < 3; n++) {
const l = p[n], c = g[n], d = cc.v3(u.x + c.x, u.y + c.y, u.z + c.z), h = cc.v3(s.x + l.targetOffset.x, s.y + l.targetOffset.y, s.z + l.targetOffset.z), y = cc.v2(h.x - d.x, h.y - d.y).mag(), _ = Math.max(.48, Math.min(y / 760, .72)), v = Math.max(d.y, h.y) + l.arcLift, b = cc.v2(d.x + .18 * (h.x - d.x), v), S = cc.v2(d.x + .72 * (h.x - d.x), v - .2 * l.arcLift), C = cc.instantiate(e);
C.active = !0;
o.addChild(C, 99999);
C.position = d;
C.scale = .504;
C.opacity = 0;
cc.tween(C).to(.12, {
scale: .616,
opacity: 255
}, {
easing: "sineOut"
}).to(.1, {
scale: .7
}, {
easing: "sineOut"
}).delay(.35 + .16 * n).call(() => {
0 == n && r.default.playSound("collect");
}).parallel(cc.tween().bezierTo(_, b, S, cc.v2(h.x, h.y)), cc.tween().to(_, {
scale: .7 * .35
}, {
easing: "sineInOut"
})).call(() => {
f++;
if (!m && f >= 3) {
m = !0;
i && i.isValid && cc.tween(i).to(.16, {
scale: 1.08
}, {
easing: "sineOut"
}).to(.12, {
scale: 1
}, {
easing: "sineIn"
}).start();
a.playFlyFakePlusMoney(t);
}
C.destroy();
}).start();
}
}
};
i([ m(cc.Node) ], y.prototype, "maskNode", void 0);
i([ m(cc.Node) ], y.prototype, "bgNode2000", void 0);
i([ m(cc.Node) ], y.prototype, "bgNodealiveAndDouble", void 0);
i([ m(cc.Node) ], y.prototype, "bgNodeGetReward", void 0);
i([ m(cc.Node) ], y.prototype, "bgNodeGuide", void 0);
i([ m(cc.Node) ], y.prototype, "money", void 0);
i([ m(cc.Label) ], y.prototype, "btnLabel", void 0);
i([ m(cc.Label) ], y.prototype, "btnGuideLabel", void 0);
i([ m(cc.Label) ], y.prototype, "titleLabel2000", void 0);
i([ m(cc.Label) ], y.prototype, "titleLabeliveAndDouble", void 0);
i([ m(cc.Label) ], y.prototype, "titleLabelGetReward", void 0);
i([ m(cc.Label) ], y.prototype, "titleLabelGuide", void 0);
y = i([ o.registerUIPath("GameDialog/RewardDialog"), f ], y);
a.default = y;
cc._RF.pop();
};
