// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: GuideDialog; dependency map: {"../BaseUIManager/BaseUI":"BaseUI","../BaseUIManager/Storage/GameLocalData":"GameLocalData","../BaseUIManager/Storage/PlayData":"PlayData","../BaseUIManager/Storage/TouchButton":"TouchButton","../GameScene":"GameScene","../LanguageControl/GameManagement":"GameManagement","../LanguageControl/Lab":"Lab","./RewardDialog":"RewardDialog"}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "7e64e2X2spIOKP/OXDwaKMQ", "GuideDialog");
var i = this && this.__decorate || function(e, t, a, i) {
var o, n = arguments.length, s = n < 3 ? t : null === i ? i = Object.getOwnPropertyDescriptor(t, a) : i;
if ("object" == typeof Reflect && "function" == typeof Reflect.decorate) s = Reflect.decorate(e, t, a, i); else for (var r = e.length - 1; r >= 0; r--) (o = e[r]) && (s = (n < 3 ? o(s) : n > 3 ? o(t, a, s) : o(t, a)) || s);
return n > 3 && s && Object.defineProperty(t, a, s), s;
};
Object.defineProperty(a, "__esModule", {
value: !0
});
const o = e("../BaseUIManager/BaseUI"), n = e("../BaseUIManager/Storage/GameLocalData"), s = e("../BaseUIManager/Storage/PlayData"), r = e("../BaseUIManager/Storage/TouchButton"), l = e("../GameScene"), c = e("../LanguageControl/GameManagement"), d = e("../LanguageControl/Lab"), h = e("./RewardDialog"), {ccclass: u, property: g} = cc._decorator;
let p = class extends o.default {
constructor() {
super(...arguments);
this.step0 = null;
this.step1 = null;
this.step3 = null;
this.step4 = null;
this.clickArea = null;
this.mask = null;
this.guideStepStates = {};
this.guideLayoutMarginX = 24;
this.guideLayoutMarginTop = 28;
this.guideLayoutMarginBottom = 28;
this.minGuideScale = .78;
this.guideDesignWidth = 750;
this.guideDesignHeight = 1624;
this.fullscreenNodeOverscan = 16;
this.fullscreenNodeMinCoverSize = 3e3;
this.step3GuideMoneyPath = [ "content", "money" ];
this.step4GuideBtnPath = [ "content", "btn1" ];
}
onLoad() {
super.onLoad();
this.saveGuideStepStates();
this.clickArea.addComponent(r.default).registerTouchEvent(() => {
const e = n.default.getInstance().getData(s.default);
if (0 == e.guideStep) {
e.guideStep = 1;
this.on_close_call();
} else if (1 == e.guideStep) {
e.guideStep = 2;
h.default.open_with_parm({
showType: 5
});
this.on_close_call();
} else if (3 == e.guideStep) {
e.guideStep = 4;
this.showGuide();
} else if (4 == e.guideStep) {
e.guideStep = 9999;
this.on_close_call();
}
});
}
onEnable() {}
onDisable() {}
onTouchStart(e) {}
onTouchMove(e) {}
onTouchEnd(e) {}
show(e) {
super.show(e);
e.param;
this.showGuide();
}
onAddFinished() {
this.scheduleAdaptGuideLayout();
}
showGuide() {
this.step0.active = this.step1.active = this.step4.active = this.step3.active = !1;
const e = n.default.getInstance().getData(s.default);
if (0 == e.guideStep) {
this.clickArea.active = !1;
this.step0.active = !0;
this.step0.getChildByName("bg").getChildByName("step1Label").getComponent(cc.Label).string = d.default.getlab("84");
} else if (1 == e.guideStep) {
this.clickArea.active = !0;
this.step1.active = !0;
this.step1.getChildByName("bg").getChildByName("step2Label1").getComponent(cc.Label).string = d.default.getlab("86");
this.step1.getChildByName("bg").getChildByName("step2Label2").getComponent(cc.Label).string = d.default.getlab("87");
} else if (3 == e.guideStep) {
this.clickArea.active = !0;
this.step3.active = !0;
const e = n.default.getInstance().getData(s.default);
this.step3.getChildByName("content").getChildByName("bg").getChildByName("step4Label1").getComponent(cc.Label).string = d.default.getlab("88");
this.step3.getChildByName("content").getChildByName("money").getChildByName("fakeMoney").getComponent(cc.Label).string = c.default.getmonstr(e.fakeMoney) + "";
this.step3.getChildByName("content").getChildByName("money").getChildByName("btn3").getChildByName("fakeMoneyLabel").getComponent(cc.Label).string = d.default.getlab("8");
} else if (4 == e.guideStep) {
this.clickArea.active = !0;
this.step4.active = !0;
this.step4.getChildByName("content").getChildByName("bg").getChildByName("step2Label1").getComponent(cc.Label).string = d.default.getlab("89");
this.step4.getChildByName("content").getChildByName("bg").getChildByName("step2Label2").getComponent(cc.Label).string = d.default.getlab("90");
} else this.on_close_call();
this.scheduleAdaptGuideLayout();
}
saveGuideStepStates() {
let e = [ this.step0, this.step1, this.step3, this.step4 ];
for (let t = 0; t < e.length; t++) {
let a = e[t];
a && (this.guideStepStates[a.name] = {
x: a.x,
y: a.y,
scaleX: a.scaleX,
scaleY: a.scaleY
});
}
}
scheduleAdaptGuideLayout() {
this.adaptGuideLayout();
this.scheduleOnce(() => {
this.adaptGuideLayout();
}, 0);
this.scheduleOnce(() => {
this.adaptGuideLayout();
}, .05);
}
adaptGuideLayout() {
if (!this.node || !this.node.parent) return;
this.adaptGuideRootToVisibleSize();
this.updateFullscreenNode(this.clickArea);
this.mask.width = 3e3;
this.mask.height = 3e3;
let e = this.getActiveStep();
if (!e) return;
this.resetGuideStep(e);
this.updateWidgetAlignmentRecursive(e);
if (e == this.step3) {
this.alignGuideNodeToTargetNode(e, this.step3GuideMoneyPath, this.getFakeWithdrawMoneyNode());
return;
}
if (e == this.step4) {
this.alignGuideNodeToTargetNode(e, this.step4GuideBtnPath, this.getWithdrawButtonNode());
return;
}
let t = this.getStepBounds(e);
if (!t) return;
let a = cc.view && cc.view.getVisibleSize ? cc.view.getVisibleSize() : cc.winSize, i = Math.min(this.node.width || a.width, a.width), o = Math.min(this.node.height || a.height, a.height), n = -i / 2 + this.guideLayoutMarginX, s = i / 2 - this.guideLayoutMarginX, r = -o / 2 + this.guideLayoutMarginBottom, l = o / 2 - this.guideLayoutMarginTop, c = t.maxX - t.minX, d = t.maxY - t.minY, h = Math.min(1, (s - n) / c, (l - r) / d);
if ((h = Math.max(this.minGuideScale, h)) < 1) {
let a = this.guideStepStates[e.name];
e.scaleX = a.scaleX * h;
e.scaleY = a.scaleY * h;
t = this.getStepBounds(e);
}
if (!t) return;
let u = 0, g = 0;
t.minX < n ? u = n - t.minX : t.maxX > s && (u = s - t.maxX);
t.minY < r ? g = r - t.minY : t.maxY > l && (g = l - t.maxY);
e.x += u;
e.y += g;
}
adaptGuideRootToVisibleSize() {
if (!this.node) return;
let e = cc.view && cc.view.getVisibleSize ? cc.view.getVisibleSize() : cc.winSize, t = Math.max(this.guideDesignWidth, e.width || 0), a = Math.max(this.guideDesignHeight, e.height || 0);
if (t <= 0 || a <= 0) return;
let i = this.node.getComponent(cc.Widget);
i && i.enabled && (i.enabled = !1);
this.node.setContentSize(t, a);
this.node.x = t * this.node.anchorX;
this.node.y = a * this.node.anchorY;
}
updateWidgetAlignment(e) {
if (!e) return;
let t = e.getComponent(cc.Widget);
t && t.enabled && t.updateAlignment();
}
updateWidgetAlignmentRecursive(e) {
if (e) {
this.updateWidgetAlignment(e);
for (let t = 0; t < e.childrenCount; t++) this.updateWidgetAlignmentRecursive(e.children[t]);
}
}
updateFullscreenNode(e) {
if (!e) return;
this.updateWidgetAlignment(e);
let t = cc.view && cc.view.getVisibleSize ? cc.view.getVisibleSize() : cc.winSize;
e.width = t.width;
e.height = t.height;
}
getActiveStep() {
let e = [ this.step0, this.step1, this.step3, this.step4 ];
for (let t = 0; t < e.length; t++) {
let a = e[t];
if (a && a.active) return a;
}
return null;
}
resetGuideStep(e) {
let t = this.guideStepStates[e.name];
if (t) {
e.x = t.x;
e.y = t.y;
e.scaleX = t.scaleX;
e.scaleY = t.scaleY;
}
}
alignGuideNodeToTargetNode(e, t, a) {
if (!e || !this.node) return;
let i = this.getNodeByPath(e, t);
if (!(a && i && a.parent && i.parent)) return;
let o = a.parent.convertToWorldSpaceAR(a.position), n = i.parent.convertToWorldSpaceAR(i.position), s = this.node.convertToNodeSpaceAR(o), r = this.node.convertToNodeSpaceAR(n);
e.x += s.x - r.x;
e.y += s.y - r.y;
}
getWithdrawButtonNode() {
let e = l.default.instance;
return e && e.btnGetMoney && e.btnGetMoney.isValid ? e.btnGetMoney : null;
}
getFakeWithdrawMoneyNode() {
let e = l.default.instance;
if (!e || !e.btnFakeMoney || !e.btnFakeMoney.isValid) return null;
let t = e.btnFakeMoney.parent;
return t && t.isValid ? t : e.btnFakeMoney;
}
getNodeByPath(e, t) {
let a = e;
for (let e = 0; e < t.length; e++) {
if (!a) return null;
a = a.getChildByName(t[e]);
}
return a;
}
getStepBounds(e) {
let t = null;
return this.collectStepBounds(e, t);
}
collectStepBounds(e, t) {
if (!e || !e.active) return t;
this.isRenderableGuideNode(e) && (t = this.includeNodeBounds(e, t));
for (let a = 0; a < e.childrenCount; a++) t = this.collectStepBounds(e.children[a], t);
return t;
}
isRenderableGuideNode(e) {
return !!(e.getComponent(cc.Sprite) || e.getComponent(cc.Label) || e.getComponent(sp.Skeleton));
}
includeNodeBounds(e, t) {
if (!e.width || !e.height) return t;
let a = e.getBoundingBoxToWorld(), i = this.node.convertToNodeSpaceAR(cc.v2(a.xMin, a.yMin)), o = this.node.convertToNodeSpaceAR(cc.v2(a.xMax, a.yMax)), n = Math.min(i.x, o.x), s = Math.max(i.x, o.x), r = Math.min(i.y, o.y), l = Math.max(i.y, o.y);
if (!t) return {
minX: n,
maxX: s,
minY: r,
maxY: l
};
t.minX = Math.min(t.minX, n);
t.maxX = Math.max(t.maxX, s);
t.minY = Math.min(t.minY, r);
t.maxY = Math.max(t.maxY, l);
return t;
}
};
i([ g(cc.Node) ], p.prototype, "step0", void 0);
i([ g(cc.Node) ], p.prototype, "step1", void 0);
i([ g(cc.Node) ], p.prototype, "step3", void 0);
i([ g(cc.Node) ], p.prototype, "step4", void 0);
i([ g(cc.Node) ], p.prototype, "clickArea", void 0);
i([ g(cc.Node) ], p.prototype, "mask", void 0);
p = i([ o.registerUIPath("GameDialog/GuideDialog"), u ], p);
a.default = p;
cc._RF.pop();
};
