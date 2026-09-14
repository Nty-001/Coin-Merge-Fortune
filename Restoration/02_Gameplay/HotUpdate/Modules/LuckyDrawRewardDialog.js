// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: LuckyDrawRewardDialog; dependency map: {"../BaseUIManager/BaseUI":"BaseUI","../BaseUIManager/Storage/GameLocalData":"GameLocalData","../BaseUIManager/Storage/PlayData":"PlayData","../BaseUIManager/Storage/SoundManager":"SoundManager","../BaseUIManager/Storage/TouchButton":"TouchButton","../GameScene":"GameScene","../HWL/HWL_TStool":"HWL_TStool","../LanguageControl/GameManagement":"GameManagement","../LanguageControl/Lab":"Lab","../common/GameUtils":"GameUtils"}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "0012fFDtw1OsZHNzm3p9sC/", "LuckyDrawRewardDialog");
var i = this && this.__decorate || function(e, t, a, i) {
var o, n = arguments.length, s = n < 3 ? t : null === i ? i = Object.getOwnPropertyDescriptor(t, a) : i;
if ("object" == typeof Reflect && "function" == typeof Reflect.decorate) s = Reflect.decorate(e, t, a, i); else for (var r = e.length - 1; r >= 0; r--) (o = e[r]) && (s = (n < 3 ? o(s) : n > 3 ? o(t, a, s) : o(t, a)) || s);
return n > 3 && s && Object.defineProperty(t, a, s), s;
};
Object.defineProperty(a, "__esModule", {
value: !0
});
const o = e("../BaseUIManager/BaseUI"), n = e("../BaseUIManager/Storage/GameLocalData"), s = e("../BaseUIManager/Storage/PlayData"), r = e("../BaseUIManager/Storage/SoundManager"), l = e("../BaseUIManager/Storage/TouchButton"), c = e("../common/GameUtils"), d = e("../GameScene"), h = e("../HWL/HWL_TStool"), u = e("../LanguageControl/GameManagement"), g = e("../LanguageControl/Lab"), {ccclass: p, property: f} = cc._decorator;
let m = class extends o.default {
constructor() {
super(...arguments);
this.isshowAd = !1;
this.isWatchingAd = !1;
this.hasSettledReward = !1;
this.currentFakeMoney = 0;
this.currentCoinnumber = 0;
this.currentType = "";
}
onLoad() {
super.onLoad();
this.btnSure.addComponent(l.default).registerTouchEvent(() => {
this.onClickSure();
});
}
start() {}
onClickSure() {
this.hasSettledReward || this.isWatchingAd || (1 != this.isshowAd ? this.closeAndSettleReward() : this.showIfLookAD());
}
showIfLookAD() {
if (1 == this.isshowAd) {
this.isWatchingAd = !0;
h.HWLshowAd("2_A", () => {
this.isWatchingAd = !1;
this.closeAndSettleReward();
}, () => {
this.closeAndSettleReward();
this.isWatchingAd = !1;
}) || (this.isWatchingAd = !1);
}
}
show(e) {
super.show(e);
if (d.default.instance && d.default.instance.isGameOverState()) {
this.hide();
return;
}
let t = e.param, a = t.drawIndex, i = u.default.globalData.lotteryConfig;
this.currentType = i[a].type;
this.currentCoinnumber = i[a].amount;
this.isshowAd = !1;
this.isWatchingAd = !1;
this.hasSettledReward = !1;
let o = u.default.globalData.drawRewardStrong;
const r = n.default.getInstance().getData(s.default);
if (r.currentLotteryCount % o == 0) {
this.isshowAd = !0;
console.log("该播广告");
}
if ("money" == this.currentType) {
this.coin.node.active = !1;
this.moneyIcon.node.active = !0;
let e = 1;
this.currentCoinnumber > 5e3 && (e = 3);
let t = "texture/coin" + e + "/";
u.default.loadSpriteFrame(t + u.default.GetCountry(), e => {
this.moneyIcon.spriteFrame = e;
});
let a = c.gameUtils.calculateCashReward();
this.currentFakeMoney = 1.3 * a;
this.numberLabel.string = u.default.getmonstr(this.currentFakeMoney);
} else {
this.coin.node.active = !0;
this.moneyIcon.node.active = !1;
this.numberLabel.string = "+" + this.currentCoinnumber;
}
this.titleLabel.string = g.default.getlab("38");
this.btnLabel.string = g.default.getlab("37");
let l = Number(t.needScore);
if (isNaN(l)) {
const e = c.gameUtils.getCurrentDrawScore();
l = c.gameUtils.checkLotteryStatus(e, r.currentLotteryCount).needScore;
}
let h = g.default.getlab("45").replace("%{0}", l.toString());
this.tipLabel.string = h;
if (this.guang) {
cc.Tween.stopAllByTarget(this.guang);
this.guang.angle = 0;
cc.tween(this.guang).by(4, {
angle: -360
}).repeatForever().start();
}
}
closeAndSettleReward() {
if (this.hasSettledReward) return;
this.hasSettledReward = !0;
if (d.default.instance && d.default.instance.isGameOverState()) {
this.hide();
return;
}
let e = this.moneyIcon ? this.moneyIcon.node : null;
this.on_close_call();
this.settleReward(e);
}
settleReward(e) {
const t = n.default.getInstance().getData(s.default);
if ("money" == this.currentType) {
t.fakeMoney += this.currentFakeMoney;
if (d.default.instance) {
d.default.instance.setFakeMoney();
this.flyMoneyToGameScene(e, this.currentFakeMoney);
}
} else {
t.coin1024Number += this.currentCoinnumber;
d.default.instance && d.default.instance.set1024Label();
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
i([ f(cc.Node) ], m.prototype, "btnSure", void 0);
i([ f(cc.Label) ], m.prototype, "numberLabel", void 0);
i([ f(cc.Sprite) ], m.prototype, "moneyIcon", void 0);
i([ f(cc.Sprite) ], m.prototype, "coin", void 0);
i([ f(cc.Node) ], m.prototype, "money", void 0);
i([ f(cc.Label) ], m.prototype, "btnLabel", void 0);
i([ f(cc.Label) ], m.prototype, "tipLabel", void 0);
i([ f(cc.Label) ], m.prototype, "titleLabel", void 0);
i([ f(cc.Node) ], m.prototype, "guang", void 0);
m = i([ o.registerUIPath("GameDialog/LuckyDrawRewardDialog"), p ], m);
a.default = m;
cc._RF.pop();
};
