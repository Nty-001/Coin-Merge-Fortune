// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: GameRealWDRecord; dependency map: {"../BaseUIManager/BaseUI":"BaseUI","../BaseUIManager/Storage/TouchButton":"TouchButton","../HWL/ServerConfig":"ServerConfig","../LanguageControl/Lab":"Lab","./GameRealWDRecordItem":"GameRealWDRecordItem"}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "decb6Lh461CSLj5j9V9+BVS", "GameRealWDRecord");
var i = this && this.__decorate || function(e, t, a, i) {
var o, n = arguments.length, s = n < 3 ? t : null === i ? i = Object.getOwnPropertyDescriptor(t, a) : i;
if ("object" == typeof Reflect && "function" == typeof Reflect.decorate) s = Reflect.decorate(e, t, a, i); else for (var r = e.length - 1; r >= 0; r--) (o = e[r]) && (s = (n < 3 ? o(s) : n > 3 ? o(t, a, s) : o(t, a)) || s);
return n > 3 && s && Object.defineProperty(t, a, s), s;
};
Object.defineProperty(a, "__esModule", {
value: !0
});
const o = e("../BaseUIManager/BaseUI"), n = e("../BaseUIManager/Storage/TouchButton"), s = e("../HWL/ServerConfig"), r = e("../LanguageControl/Lab"), l = e("./GameRealWDRecordItem"), {ccclass: c, property: d} = cc._decorator;
let h = class extends o.default {
constructor() {
super(...arguments);
this.btnClose = null;
this.title = null;
this.items = null;
this.item = null;
this.isSuccess = !1;
this.res = "";
this.adaptFrameCount = 0;
this.hasLoggedLayout = !1;
}
onLoad() {
super.onLoad();
this.btnClose.addComponent(n.default).registerTouchEvent(() => {
this.stopDebugTimer();
this.on_close_call();
});
this.requestLayoutAdapt();
}
onEnable() {
this.requestLayoutAdapt();
}
lateUpdate() {
if (!(this.adaptFrameCount <= 0)) {
this.adaptFrameCount--;
this.adaptLayout();
}
}
stopDebugTimer() {
this.unschedule(this.debugData);
this.unschedule(this.successData);
}
show(e) {
super.show(e);
e.param;
this.title.string = r.default.getlab("93");
this.clearItemsChildren();
this.requestLayoutAdapt();
this.onRealCashSend();
}
clearItemsChildren() {
if (!this.items) return;
const e = [ ...this.items.children ];
for (const t of e) {
t.removeFromParent();
t.destroy();
}
this.refreshItemsLayout();
}
onRealCashSend() {
cc.sys.isBrowser ? this.scheduleOnce(this.debugData, 2) : s.HWLServerConfig.sendCashRecord().then(e => {
this.onRealCashResult(!0, e);
}).catch(e => {
this.onRealCashResult(!1, e);
});
}
onRealCashResult(e, t) {
this.isSuccess = e;
this.res = t;
this.scheduleOnce(this.successData, 2);
}
successData() {
var e;
if (this.isSuccess) {
let t = JSON.parse(this.res);
if (0 == t.code) {
let a = (null === (e = t.data) || void 0 === e ? void 0 : e.recordList) || [];
for (let e = 0; e < a.length; e++) {
let t = a[e], i = cc.instantiate(this.item);
i.parent = this.items;
i.getComponent(l.default).initdata(t, e);
}
this.refreshItemsLayout();
}
}
}
debugData() {
let e = [ {
time: "2024-01-15 10:30:25",
amount: 1,
account: "123@163.com",
withdrawDetails: "PayPal",
withdrawStatus: 0,
fee: "0.338"
} ];
for (let t = 0; t < e.length; t++) {
let a = e[t], i = cc.instantiate(this.item);
i.parent = this.items;
i.getComponent(l.default).initdata(a, t);
}
this.refreshItemsLayout();
}
requestLayoutAdapt() {
this.adaptFrameCount = 12;
this.adaptLayout();
}
adaptLayout() {
const e = this.contentNode || this.node.getChildByName("content");
if (!e) return;
const t = e.getChildByName("top"), a = e.getChildByName("bg"), i = this.node.getChildByName("mask"), o = this.getScrollViewNode(e), n = o ? o.getChildByName("view") : null;
if (!t || !o || !n) return;
this.updateWidgetAlignment(this.node);
this.disableWidget(e);
this.disableWidget(t);
this.disableWidget(o);
this.disableWidget(n);
const s = cc.view.getVisibleSize ? cc.view.getVisibleSize() : cc.winSize, r = s.width || 750, l = s.height || 1624, c = Math.min(this.node.width || r, r), d = Math.min(this.node.height || l, l);
e.width = c;
e.height = d;
e.x = 0;
e.y = 0;
if (a) {
this.disableWidget(a);
a.width = c;
a.height = d;
a.x = 0;
a.y = 0;
}
if (i) {
this.disableWidget(i);
i.width = c + 2500;
i.height = d;
i.x = 0;
i.y = 0;
}
const h = this.getEdgeMargin(d), u = t.height || 150, g = h;
t.width = c;
t.x = 0;
t.y = d / 2 - u / 2 - h;
o.anchorY = 1;
n.anchorY = 1;
const p = t.y - u / 2 - 8, f = -d / 2 + g, m = Math.max(320, p - f);
o.x = 0;
o.y = p;
o.setContentSize(c, m);
n.x = 0;
n.y = 0;
n.setContentSize(c, m);
this.refreshItemsLayout(c);
const y = o.getComponent(cc.ScrollView);
y && this.items && (y.content = this.items);
if (!this.hasLoggedLayout) {
this.hasLoggedLayout = !0;
console.log("GameRealWDRecord adaptLayout", {
contentWidth: c,
contentHeight: d,
topY: t.y,
scrollY: o.y,
scrollHeight: m
});
}
}
refreshItemsLayout(e) {
if (!this.items) return;
this.items.width = e || this.node.width || this.items.width;
const t = this.items.getComponent(cc.Layout);
t && t.updateLayout();
}
getScrollViewNode(e) {
return this.items && this.items.parent && this.items.parent.parent ? this.items.parent.parent : e.getChildByName("ScrollView");
}
updateWidgetAlignment(e) {
if (!e) return;
const t = e.getComponent(cc.Widget);
t && t.enabled && t.updateAlignment();
}
disableWidget(e) {
const t = e.getComponent(cc.Widget);
t && (t.enabled = !1);
}
getEdgeMargin(e) {
return e <= 1400 ? 6 : e <= 1624 ? 12 : 20;
}
};
i([ d(cc.Node) ], h.prototype, "btnClose", void 0);
i([ d(cc.Label) ], h.prototype, "title", void 0);
i([ d(cc.Node) ], h.prototype, "items", void 0);
i([ d(cc.Prefab) ], h.prototype, "item", void 0);
h = i([ o.registerUIPath("GameDialog/GameRealWDRecord"), c ], h);
a.default = h;
cc._RF.pop();
};
