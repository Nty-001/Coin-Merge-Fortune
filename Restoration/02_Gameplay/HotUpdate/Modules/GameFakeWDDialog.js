// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: GameFakeWDDialog; dependency map: {"../BaseUIManager/BaseUI":"BaseUI","../BaseUIManager/Storage/GameLocalData":"GameLocalData","../BaseUIManager/Storage/PlayData":"PlayData","../BaseUIManager/Storage/TouchButton":"TouchButton","../BaseUIManager/UIManagerNew":"UIManagerNew","../LanguageControl/GameManagement":"GameManagement","../LanguageControl/Lab":"Lab","../WithDraw/GameRealWDActiveTips":"GameRealWDActiveTips","./GameFakeWDItem":"GameFakeWDItem"}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "94bf9cFdKtMOpmEFi9uXZm7", "GameFakeWDDialog");
var i = this && this.__decorate || function(e, t, a, i) {
var o, n = arguments.length, s = n < 3 ? t : null === i ? i = Object.getOwnPropertyDescriptor(t, a) : i;
if ("object" == typeof Reflect && "function" == typeof Reflect.decorate) s = Reflect.decorate(e, t, a, i); else for (var r = e.length - 1; r >= 0; r--) (o = e[r]) && (s = (n < 3 ? o(s) : n > 3 ? o(t, a, s) : o(t, a)) || s);
return n > 3 && s && Object.defineProperty(t, a, s), s;
};
Object.defineProperty(a, "__esModule", {
value: !0
});
const o = e("../BaseUIManager/BaseUI"), n = e("../BaseUIManager/Storage/GameLocalData"), s = e("../BaseUIManager/Storage/PlayData"), r = e("../BaseUIManager/Storage/TouchButton"), l = e("../BaseUIManager/UIManagerNew"), c = e("../LanguageControl/GameManagement"), d = e("../LanguageControl/Lab"), h = e("../WithDraw/GameRealWDActiveTips"), u = e("./GameFakeWDItem"), {ccclass: g, property: p} = cc._decorator;
let f = class extends o.default {
constructor() {
super(...arguments);
this.btnClose = null;
this.titleLabel = null;
this.moneyLabel = null;
this.moneyTxt = null;
this.item = null;
this.scrollViewContent = null;
this.btnTixian = null;
this.adaptFrameCount = 0;
this.hasLoggedLayout = !1;
this.currentIndex = 0;
this.dataArr = [];
}
start() {}
onLoad() {
super.onLoad();
if (this.btnClose) {
this.btnClose.addComponent(r.default).registerTouchEvent(() => {
this.on_close_call();
});
this.btnTixian.addComponent(r.default).registerTouchEvent(() => {
1 == this.dataArr[1] ? h.default.open() : l.default.show_toast({
text: this.dataArr[0]
});
});
this.requestLayoutAdapt();
} else console.log("GameFakeWDDialog btnClose is null");
}
onEnable() {
this.requestLayoutAdapt();
}
show(e) {
super.show(e);
this.titleLabel && (this.titleLabel.string = d.default.getlab("8"));
this.moneyLabel && (this.moneyLabel.string = d.default.getlab("53"));
const t = n.default.getInstance().getData(s.default);
this.moneyTxt && (this.moneyTxt.string = c.default.getmonstr(t.fakeMoney));
this.updateUI();
this.requestLayoutAdapt();
}
lateUpdate() {
if (!(this.adaptFrameCount <= 0)) {
this.adaptFrameCount--;
this.adaptLayout();
}
}
updateUI() {
if (!this.scrollViewContent || !this.item) {
console.log("GameFakeWDDialog item or scrollViewContent is null");
return;
}
this.btnTixian.getChildByName("tixianBtnLabel").getComponent(cc.Label).string = d.default.getlab("8");
this.scrollViewContent.removeAllChildren();
this.currentIndex = 0;
this.dataArr = [];
let e = this.getConfig();
for (let t = 0; t < e.length; t++) {
let a = e[t];
const i = cc.instantiate(this.item);
i.parent = this.scrollViewContent;
const o = i.getComponent(u.default);
if (o) {
o.initdata(a);
0 == t ? this.dataArr = o.refreshSelectBGSprite(!0) : o.refreshSelectBGSprite(!1);
let e = i.getComponent(r.default);
e || (e = i.addComponent(r.default));
e && e.registerTouchEvent(() => {
this.onitemWithdraw(t);
});
}
}
let t = this.scrollViewContent.getComponent(cc.Layout);
t && t.updateLayout();
}
onitemWithdraw(e) {
if (e !== this.currentIndex) {
this.currentIndex = e;
this.refreshUI();
}
}
refreshUI() {
for (let e = 0; e < this.scrollViewContent.children.length; e++) {
const t = this.scrollViewContent.children[e];
if (t) {
const a = t.getComponent(u.default);
let i = this.currentIndex === e, o = a.refreshSelectBGSprite(i);
i && (this.dataArr = o);
}
}
}
requestLayoutAdapt() {
this.adaptFrameCount = 12;
this.adaptLayout();
}
adaptLayout() {
const e = this.contentNode || this.node.getChildByName("content");
if (!e) return;
const t = e.getChildByName("top"), a = e.getChildByName("bottomNode"), i = this.getScrollViewNode(e), o = i ? i.getChildByName("view") : null;
if (!(t && a && i && o)) return;
this.disableWidget(e);
this.disableWidget(t);
this.disableWidget(a);
this.disableWidget(i);
this.disableWidget(o);
const n = cc.view.getVisibleSize ? cc.view.getVisibleSize() : cc.winSize, s = this.node.width > 0 ? this.node.width : n.width || 750, r = this.node.height > 0 ? this.node.height : n.height || 1624;
e.width = s;
e.height = r;
e.x = 0;
e.y = 0;
const l = t.height || 456, c = a.height || 168, d = this.getEdgeMargin(r);
i.anchorY = 1;
o.anchorY = 1;
t.width = s;
t.x = 0;
t.y = r / 2 - l / 2 - d;
a.width = s;
a.x = 0;
a.y = -r / 2 + c / 2 + d;
const h = t.y - l / 2 - 8, u = a.y + c / 2 + 8, g = Math.max(300, h - u);
i.x = 0;
i.y = h;
i.setContentSize(s, g);
o.x = 0;
o.y = 0;
o.setContentSize(s, g);
if (this.scrollViewContent) {
this.scrollViewContent.width = s;
const e = this.scrollViewContent.getComponent(cc.Layout);
e && e.updateLayout();
}
if (!this.hasLoggedLayout) {
this.hasLoggedLayout = !0;
console.log("GameFakeWDDialog adaptLayout", {
contentWidth: s,
contentHeight: r,
topY: t.y,
bottomY: a.y,
scrollY: i.y,
scrollHeight: g
});
}
}
getScrollViewNode(e) {
return this.scrollViewContent && this.scrollViewContent.parent && this.scrollViewContent.parent.parent ? this.scrollViewContent.parent.parent : e.getChildByName("scrollView");
}
disableWidget(e) {
const t = e.getComponent(cc.Widget);
t && (t.enabled = !1);
}
getEdgeMargin(e) {
return e <= 1400 ? 6 : e <= 1624 ? 12 : 20;
}
getConfig() {
let e = null;
try {
e = c.default.fake_products;
} catch (e) {
console.log("GameFakeWDDialog getConfig error", e);
return [];
}
return e ? Array.isArray(e) ? e : e.real_products && Array.isArray(e.real_products) ? e.real_products : [] : [];
}
};
i([ p(cc.Node) ], f.prototype, "btnClose", void 0);
i([ p(cc.Label) ], f.prototype, "titleLabel", void 0);
i([ p(cc.Label) ], f.prototype, "moneyLabel", void 0);
i([ p(cc.Label) ], f.prototype, "moneyTxt", void 0);
i([ p(cc.Prefab) ], f.prototype, "item", void 0);
i([ p(cc.Node) ], f.prototype, "scrollViewContent", void 0);
i([ p(cc.Node) ], f.prototype, "btnTixian", void 0);
f = i([ o.registerUIPath("GameDialog/GameFakeWDDialog"), g ], f);
a.default = f;
cc._RF.pop();
};
