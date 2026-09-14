// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: GameWithdrawDialog; dependency map: {"../BaseUIManager/BaseUI":"BaseUI","../BaseUIManager/Storage/GameLocalData":"GameLocalData","../BaseUIManager/Storage/NewGamePlayData":"NewGamePlayData","../BaseUIManager/Storage/ResManager":"ResManager","../BaseUIManager/Storage/TouchButton":"TouchButton","../BaseUIManager/UIConfig":"UIConfig","../BaseUIManager/UIManagerNew":"UIManagerNew","../LanguageControl/GameManagement":"GameManagement","../LanguageControl/Lab":"Lab","./GameWithdrawItem":"GameWithdrawItem"}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "46c6dHgLvlId6wKAcEer0kW", "GameWithdrawDialog");
var i, o = this && this.__decorate || function(e, t, a, i) {
var o, n = arguments.length, s = n < 3 ? t : null === i ? i = Object.getOwnPropertyDescriptor(t, a) : i;
if ("object" == typeof Reflect && "function" == typeof Reflect.decorate) s = Reflect.decorate(e, t, a, i); else for (var r = e.length - 1; r >= 0; r--) (o = e[r]) && (s = (n < 3 ? o(s) : n > 3 ? o(t, a, s) : o(t, a)) || s);
return n > 3 && s && Object.defineProperty(t, a, s), s;
}, n = this && this.__awaiter || function(e, t, a, i) {
return new (a || (a = Promise))(function(o, n) {
function s(e) {
try {
l(i.next(e));
} catch (e) {
n(e);
}
}
function r(e) {
try {
l(i.throw(e));
} catch (e) {
n(e);
}
}
function l(e) {
e.done ? o(e.value) : (t = e.value, t instanceof a ? t : new a(function(e) {
e(t);
})).then(s, r);
var t;
}
l((i = i.apply(e, t || [])).next());
});
};
Object.defineProperty(a, "__esModule", {
value: !0
});
const s = e("../BaseUIManager/BaseUI"), r = e("../BaseUIManager/Storage/GameLocalData"), l = e("../BaseUIManager/Storage/NewGamePlayData"), c = e("../BaseUIManager/Storage/ResManager"), d = e("../BaseUIManager/Storage/TouchButton"), h = e("../BaseUIManager/UIConfig"), u = e("../BaseUIManager/UIManagerNew"), g = e("../LanguageControl/GameManagement"), p = e("../LanguageControl/Lab"), f = e("./GameWithdrawItem"), {ccclass: m, property: y} = cc._decorator;
let _ = i = class extends s.default {
constructor() {
super(...arguments);
this.btnClose = null;
this.titleLabel = null;
this.coinLabel1 = null;
this.coinLabel2 = null;
this.platformShow = null;
this.tixianBtnLabel = null;
this.spriteFrames = [];
this.tixianBtnSpr = null;
this.txspriteFrames = [];
this.items = null;
this.scrollView = null;
this.currentIndex = 0;
this.completedConditions = [];
this.platformSpriteFrames = new Map();
this.LayoutOpenState = !1;
this.isAmountHidden = !1;
}
onLoad() {
super.onLoad();
g.default.GetadaptForTallDevices() || (this.scrollView.height = 450);
i.instance = this;
this.availableLabel.node.parent.addComponent(d.default).registerTouchEvent(() => {
this.toggleAmountDisplay(!0);
});
this.btnClose.addComponent(d.default).registerTouchEvent(() => {
this.on_close_call();
});
this.tixianBtnLabel.node.parent.addComponent(d.default).registerTouchEvent(() => {
this.onitemWithdraw();
});
this.btnShow.parent.addComponent(d.default).registerTouchEvent(() => {
this.refreshAccountDetails(!this.LayoutOpenState);
this.LayoutOpenState = !this.LayoutOpenState;
this.LayoutOpenState ? this.btnShow.getComponent(cc.Sprite).spriteFrame = this.spriteFrames[1] : this.btnShow.getComponent(cc.Sprite).spriteFrame = this.spriteFrames[0];
});
for (let e = 0; e < data.tixian_coins.length; e++) {
const t = this.items.children[e], a = t;
t.getComponent(f.default);
if (a) {
let t = a.getComponent(d.default);
t || (t = a.addComponent(d.default));
t && t.registerTouchEvent(() => {
this.currentIndex = e;
this.updateUI();
});
}
}
this.preloadPlatformIcons();
this.EditBox.node.on("editing-did-ended", () => {
r.default.getInstance().getData(l.default).accountName = this.EditBox.string;
r.default.getInstance().saveToUserDefault();
this.checkinfo();
});
this.refreshAccountDetails(!1, !0);
this.checkinfo();
}
preloadPlatformIcons() {
return n(this, void 0, void 0, function*() {
let e = p.default.GetDetailsText().payMode;
const t = Object.keys(e), a = Object.keys(t);
for (let e = 0; e < a.length && e < 4; e++) {
const i = t[a[e]];
if (!this.platformSpriteFrames.has(i)) try {
const e = yield c.default.loadSpriteFrameAsync(`texture/platform/${i}`);
this.platformSpriteFrames.set(i, e);
} catch (e) {
console.warn(`Failed to load sprite frame: texture/platform/${i}`, e);
}
}
});
}
show(e) {
super.show(e);
e.param;
this.titleLabel.string = p.default.getlab("1");
this.availableLabel.string = p.default.getlab("8");
this.accountLabel.string = p.default.getlab("9");
this.accountEntertips.string = p.default.getlab("10");
this.tixianBtnLabel.string = p.default.getlab("1");
this.EditBox.placeholder = p.default.getlab("11");
this.showRewardView();
}
updateUI() {
for (let e = 0; e < data.tixian_coins.length; e++) {
const t = this.items.children[e];
if (t) {
const a = t.getComponent(f.default);
let i = this.currentIndex === e ? 1 : 0;
a.refreshBGSprite(i);
}
}
this.updateWithdrawButtonLabel(this.currentIndex);
}
onitemWithdraw() {
let e = r.default.getInstance().getData(l.default);
if ("" == e.accountName) {
this.showAccountEnterTipsShake();
return;
}
var t = data.tixian_coins;
let a = g.default.GetCountryDang(), i = t[this.currentIndex] * a;
const o = e.fakewithdrawRecord[this.currentIndex] || !1;
if (e.red_bag < i && !o) u.default.show_toast({
text: p.default.getlab("87")
}); else {
if ("" == e.accountName) {
this.showAccountEnterTipsShake();
return;
}
const t = {
ui_config_path: h.default.GameWDValidate1,
ui_config_name: "GameWDValidate1",
param: {
withdrawAmount: i,
isFirstWithdraw: !o,
currentIndex: this.currentIndex
}
};
u.default.show_ui(t);
this.updateWithdrawButtonLabel(this.currentIndex);
}
}
updateWithdrawButtonLabel(e) {
let t = r.default.getInstance().getData(l.default).fakewithdrawRecord;
t && 0 !== t.length || (t = new Array(data.tixian_coins.length).fill(!1));
}
showRewardView() {
return n(this, void 0, void 0, function*() {
this.currentIndex = 0;
let e = r.default.getInstance().getData(l.default);
this.EditBox.placeholder = p.default.getlab("");
"" != e.accountName ? this.EditBox.string = e.accountName : this.EditBox.placeholder = p.default.getlab("11");
this.coinLabel.string = g.default.getmonstr(e.red_bag);
this.toggleAmountDisplay();
let t = g.default.GetCountryDang();
var a = data.tixian_coins;
for (let e = 0; e < data.tixian_coins.length; e++) {
const i = this.items.children[e];
if (i) {
const o = i.getComponent(f.default);
let n = a[e] * t;
o.initdata(n);
}
}
this.updateWithdrawButtonLabel(this.currentIndex);
this.updateUI();
});
}
updateAccount() {
let e = r.default.getInstance().getData(l.default);
g.default.loadSpriteFrame("texture/platform/" + e.selectPlatform, e => {
this.platformShow.spriteFrame = e;
});
r.default.getInstance().saveToUserDefault();
}
refreshAccountDetails(e = !1, t = !1) {
let a = p.default.GetDetailsText().payMode;
const i = Object.keys(a), o = Object.keys(i);
let n = r.default.getInstance().getData(l.default);
for (let a = 0; a < o.length; a++) {
const s = i[o[a]];
if (0 == a && "" == n.selectPlatform) {
n.selectPlatform = s;
g.default.loadSpriteFrame("texture/platform/" + s, e => {
this.platformShow.spriteFrame = e;
});
r.default.getInstance().saveToUserDefault();
}
const c = this.PlatformNode.children[a];
if (c) {
const a = c.getChildByName("platformBg").getComponent(cc.Sprite);
if (a && t) {
this.platformSpriteFrames.has(s) ? a.spriteFrame = this.platformSpriteFrames.get(s) : g.default.loadSpriteFrame("texture/platform/" + s, e => {
a.spriteFrame = e;
this.platformSpriteFrames.set(s, e);
});
let e = c.addComponent(d.default);
e && e.registerTouchEvent(() => {
r.default.getInstance().getData(l.default).selectPlatform = s;
r.default.getInstance().saveToUserDefault();
this.refreshAccountDetails();
this.updateAccount();
this.checkinfo();
this.LayoutOpenState = !1;
this.btnShow.getComponent(cc.Sprite).spriteFrame = this.spriteFrames[0];
});
}
c.active = e;
}
}
}
checkinfo() {
let e = r.default.getInstance().getData(l.default), t = "" != e.selectPlatform && "" != e.accountName ? 0 : 1;
this.tixianBtnSpr.spriteFrame = this.txspriteFrames[t];
this.tixianBtnLabel.getComponent(cc.LabelOutline).color = 0 == t ? new cc.Color(210, 94, 0) : new cc.Color(120, 128, 123);
}
showAccountEnterTipsShake() {
if (!this.EditBox || !this.EditBox.node) return;
this.EditBox.node.active = !0;
this.EditBox.node.opacity = 255;
this.EditBox.node.position = cc.v3(-32, 0, 0);
const e = this.EditBox.node.position, t = this.EditBox.node.color.clone(), a = new cc.Color(255, 90, 0);
this.EditBox.node.children[0].color = t;
const i = cc.sequence(cc.moveTo(.08, cc.v2(e.x - 8, e.y)), cc.moveTo(.08, cc.v2(e.x + 8, e.y)), cc.moveTo(.08, cc.v2(e.x - 8, e.y)), cc.moveTo(.08, cc.v2(e.x + 8, e.y)), cc.moveTo(.08, cc.v2(e.x, e.y))), o = cc.sequence(cc.fadeTo(.16, 200), cc.tintTo(.16, a.r, a.g, a.b), cc.fadeTo(.16, 255), cc.tintTo(0, t.r, t.g, t.b));
cc.Tween.stopAllByTarget(this.EditBox.node);
cc.Tween.stopAllByTarget(this.EditBox.node.children[0]);
this.EditBox.node.runAction(i);
this.EditBox.node.children[0].runAction(o);
}
};
o([ y(cc.Node) ], _.prototype, "btnClose", void 0);
o([ y(cc.Label) ], _.prototype, "titleLabel", void 0);
o([ y(cc.Label) ], _.prototype, "coinLabel1", void 0);
o([ y(cc.Label) ], _.prototype, "coinLabel2", void 0);
o([ y(cc.Node) ], _.prototype, "platformShow", void 0);
o([ y(cc.Label) ], _.prototype, "tixianBtnLabel", void 0);
o([ y([ cc.SpriteFrame ]) ], _.prototype, "spriteFrames", void 0);
o([ y(cc.Sprite) ], _.prototype, "tixianBtnSpr", void 0);
o([ y([ cc.SpriteFrame ]) ], _.prototype, "txspriteFrames", void 0);
o([ y(cc.Node) ], _.prototype, "items", void 0);
o([ y(cc.Node) ], _.prototype, "scrollView", void 0);
_ = i = o([ s.registerUIPath("GameDialog/GameWithdrawDialog"), m ], _);
a.default = _;
cc._RF.pop();
};
