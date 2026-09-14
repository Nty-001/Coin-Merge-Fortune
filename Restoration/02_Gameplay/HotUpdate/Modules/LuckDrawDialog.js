// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: LuckDrawDialog; dependency map: {"../BaseUIManager/BaseUI":"BaseUI","../BaseUIManager/Storage/GameLocalData":"GameLocalData","../BaseUIManager/Storage/PlayData":"PlayData","../BaseUIManager/Storage/SoundManager":"SoundManager","../BaseUIManager/Storage/TouchButton":"TouchButton","../BaseUIManager/UIManagerNew":"UIManagerNew","../GameScene":"GameScene","../LanguageControl/GameManagement":"GameManagement","../LanguageControl/Lab":"Lab","../common/GameUtils":"GameUtils","./LuckyDrawRewardDialog":"LuckyDrawRewardDialog"}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "f4f6fPXsgdDJoGWz/Z75qu/", "LuckDrawDialog");
var i = this && this.__decorate || function(e, t, a, i) {
var o, n = arguments.length, s = n < 3 ? t : null === i ? i = Object.getOwnPropertyDescriptor(t, a) : i;
if ("object" == typeof Reflect && "function" == typeof Reflect.decorate) s = Reflect.decorate(e, t, a, i); else for (var r = e.length - 1; r >= 0; r--) (o = e[r]) && (s = (n < 3 ? o(s) : n > 3 ? o(t, a, s) : o(t, a)) || s);
return n > 3 && s && Object.defineProperty(t, a, s), s;
};
Object.defineProperty(a, "__esModule", {
value: !0
});
const o = e("../BaseUIManager/BaseUI"), n = e("../BaseUIManager/Storage/GameLocalData"), s = e("../BaseUIManager/Storage/PlayData"), r = e("../BaseUIManager/Storage/SoundManager"), l = e("../BaseUIManager/Storage/TouchButton"), c = e("../BaseUIManager/UIManagerNew"), d = e("../common/GameUtils"), h = e("../GameScene"), u = e("../LanguageControl/GameManagement"), g = e("../LanguageControl/Lab"), p = e("./LuckyDrawRewardDialog"), {ccclass: f, property: m} = cc._decorator;
let y = class extends o.default {
constructor() {
super(...arguments);
this.bgSpine = null;
this.isplaying = !1;
}
onLoad() {
super.onLoad();
this.btnDraw.addComponent(l.default).registerTouchEvent(() => {
if (0 == this.isplaying) {
let e = h.default.instance.getDrawResult();
if (1 == e.canLottery) this.playingDraw(); else {
let t = g.default.getlab("45").replace("%{0}", e.needScore.toString());
c.default.show_toast({
text: t
});
}
}
});
}
start() {}
playingDraw() {
this.btnDraw.active = !1;
this.drawTimesLabel2.node.active = !1;
this.isplaying = !0;
let e = this.getRandomReward(this.lotteryConfig);
this.bgSpine.setAnimation(0, "idle", !1);
this.bgSpine.setCompleteListener(() => {
this.playLotteryAnimation(e, e => {
setTimeout(() => {
if (h.default.instance && h.default.instance.isGameOverState()) {
this.isplaying = !1;
this.node && this.node.isValid && this.hide();
return;
}
const t = n.default.getInstance().getData(s.default), a = d.gameUtils.spendGameTotalScoreForLottery(), i = d.gameUtils.checkLotteryStatus(a, t.currentLotteryCount).needScore;
p.default.open_with_parm({
drawIndex: e,
needScore: i
});
this.isplaying = !1;
this.setDrawLabel();
this.on_close_call();
h.default.instance.updateProgressbar();
}, 800);
});
});
}
setDrawLabel() {
const e = n.default.getInstance().getData(s.default);
this.btnDrawLabel.string = g.default.getlab("85");
this.drawTimesLabel.string = d.gameUtils.getAvailableLotteryTimes() + g.default.getlab("46");
let t = d.gameUtils.getCurrentDrawScore(), a = d.gameUtils.getNeedScoreAfterCurrentDraw(t, e.currentLotteryCount);
this.drawTimesLabel2.string = g.default.getlab("45").replace("%{0}", a.toString());
}
show(e) {
super.show(e);
if (h.default.instance && h.default.instance.isGameOverState()) this.hide(); else {
this.setDrawLabel();
this.btnDraw.active = !0;
this.drawTimesLabel2.node.active = !0;
r.default.playSound("ChouJiangOpen");
this.lotteryConfig = u.default.globalData.lotteryConfig;
for (let e = 0; e < this.rectContent.children.length; e++) {
let t = this.rectContent.children[e];
t.getChildByName("select").active = !1;
let a = this.lotteryConfig[e].type, i = this.lotteryConfig[e].amount;
if ("money" == a) {
t.getChildByName("money").active = !0;
let e = 1;
i > 5e3 && (e = 3);
let a = "texture/coin" + e + "/";
u.default.loadSpriteFrame(a + u.default.GetCountry(), e => {
t.getChildByName("money").getComponent(cc.Sprite).spriteFrame = e;
});
} else {
t.getChildByName("coin").active = !0;
t.getChildByName("coin").getChildByName("numberTxt").getComponent(cc.Label).string = "+" + i;
}
}
}
}
getRandomReward(e) {
let t = e.reduce((e, t) => e + t.probability, 0), a = Math.random() * t, i = 0;
for (let t = 0; t < e.length; t++) if (a <= (i += e[t].probability)) return t;
return 0;
}
playLotteryAnimation(e, t) {
let a = this.rectContent.children.length, i = 2 * a + e + 1, o = 0, n = () => {
for (let e = 0; e < a; e++) this.rectContent.children[e].getChildByName("select").active = !1;
let s = o % a;
this.rectContent.children[s].getChildByName("select").active = !0;
if (++o >= i) {
r.default.playSound("ChouJiangJiangli");
t && t(e);
return;
}
r.default.playSound("ChouJiangTiaoDong");
let l = 0, c = o / i;
l = c < .4 ? 50 : c < .7 ? 120 : 150 + (c - .7) / .3 * 350;
setTimeout(n, l);
};
n();
}
};
i([ m(sp.Skeleton) ], y.prototype, "bgSpine", void 0);
i([ m(cc.Node) ], y.prototype, "btnDraw", void 0);
i([ m(cc.Label) ], y.prototype, "btnLabel", void 0);
i([ m(cc.Label) ], y.prototype, "drawTimesLabel", void 0);
i([ m(cc.Label) ], y.prototype, "drawTimesLabel2", void 0);
i([ m(cc.Node) ], y.prototype, "rectContent", void 0);
i([ m(cc.Node) ], y.prototype, "maskNode", void 0);
i([ m(cc.Label) ], y.prototype, "btnDrawLabel", void 0);
y = i([ o.registerUIPath("GameDialog/LuckDrawDialog"), f ], y);
a.default = y;
cc._RF.pop();
};
