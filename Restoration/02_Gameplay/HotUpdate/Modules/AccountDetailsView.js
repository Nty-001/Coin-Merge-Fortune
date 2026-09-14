// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: AccountDetailsView; dependency map: {"../BaseUIManager/BaseUI":"BaseUI","../BaseUIManager/Storage/GameLocalData":"GameLocalData","../BaseUIManager/Storage/NewGamePlayData":"NewGamePlayData","../BaseUIManager/Storage/TouchButton":"TouchButton","../BaseUIManager/UIManagerNew":"UIManagerNew","../LanguageControl/GameManagement":"GameManagement","../LanguageControl/Lab":"Lab"}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "4da49ae6f1KfoMUK2WeXyG2", "AccountDetailsView");
var i = this && this.__decorate || function(e, t, a, i) {
var o, n = arguments.length, s = n < 3 ? t : null === i ? i = Object.getOwnPropertyDescriptor(t, a) : i;
if ("object" == typeof Reflect && "function" == typeof Reflect.decorate) s = Reflect.decorate(e, t, a, i); else for (var r = e.length - 1; r >= 0; r--) (o = e[r]) && (s = (n < 3 ? o(s) : n > 3 ? o(t, a, s) : o(t, a)) || s);
return n > 3 && s && Object.defineProperty(t, a, s), s;
};
Object.defineProperty(a, "__esModule", {
value: !0
});
const o = e("../BaseUIManager/BaseUI"), n = e("../BaseUIManager/Storage/GameLocalData"), s = e("../BaseUIManager/Storage/NewGamePlayData"), r = e("../BaseUIManager/Storage/TouchButton"), l = e("../BaseUIManager/UIManagerNew"), c = e("../LanguageControl/GameManagement"), d = e("../LanguageControl/Lab"), {ccclass: h, property: u} = cc._decorator;
let g = class extends o.default {
constructor() {
super(...arguments);
this.title = null;
this.platformLabel = null;
this.btnClose = null;
this.itemNode2 = null;
this.itemNode = [];
this.nameLabel = [];
this.editBox = [];
this.detailsTipsLabel = null;
this.confirmBtn = null;
this.confirmBtnLabel = null;
this.confirmBtnSpr = [];
this.payModeConfig = null;
this._targetIndex = 0;
this._platformData = "";
}
onLoad() {
this.btnClose.addComponent(r.default).registerTouchEvent(() => {
this.on_close_call();
});
this.editBox[0].node.on("text-changed", () => {
this.updateConfirmButtonState();
});
this.editBox[1].node.on("text-changed", () => {
this.updateConfirmButtonState();
});
this.confirmBtn.addComponent(r.default).registerTouchEvent(() => {
let e = n.default.getInstance().getData(s.default);
e.accountTarget = this._targetIndex;
e.selectPlatform = this._platformData;
if ("" == this.editBox[0].string) {
let e = this.editBox[0].placeholder;
l.default.show_toast({
text: e
});
} else if ("" == this.editBox[1].string && this.editBox[1].node.active) {
let e = this.editBox[1].placeholder;
l.default.show_toast({
text: e
});
} else {
e.accountName = this.editBox[0].string;
e.accountEmail = this.editBox[1].string;
this.on_close_call();
}
n.default.getInstance().set_local_storeage();
});
}
updateConfirmButtonState() {
const e = "" !== this.editBox[0].string.trim(), t = !this.editBox[1].node.active || "" !== this.editBox[1].string.trim(), a = this.confirmBtn.getComponent(cc.Sprite);
a && (a.spriteFrame = e && t ? this.confirmBtnSpr[0] : this.confirmBtnSpr[1]);
this.confirmBtnLabel.getComponent(cc.LabelOutline).color = e && t ? new cc.Color(7, 113, 50) : new cc.Color(84, 84, 84);
}
onItemClicked(e, t, a) {
this._targetIndex = a;
for (let a = 0; a < e.length; a++) {
let i = this.payModeConfig[t][e[a]];
this.editBox[a].placeholder = i[1];
this.nameLabel[a].string = i[0];
}
this._platformData = t;
this.showPayNode(e, a);
this.updateConfirmButtonState();
}
show(e) {
super.show(e);
e.param;
this.title.string = d.default.getlab("41");
this.platformLabel.string = d.default.getlab("42");
this.detailsTipsLabel.string = d.default.getlab("14");
this.confirmBtnLabel.string = d.default.getlab("86");
this.payModeConfig = d.default.GetDetailsText().payMode;
let t = n.default.getInstance().getData(s.default), a = 0;
if (this.payModeConfig && "object" == typeof this.payModeConfig) {
const e = Object.keys(this.payModeConfig), i = Object.keys(e);
this._platformData = e[i[0]];
for (let o = 0; o < i.length && o < this.itemNode.length; o++) {
const n = e[i[o]], s = this.itemNode[a];
if (s) {
const e = s.getChildByName("paySpr");
if (e) {
const t = e.getComponent(cc.Sprite);
if (t) {
c.default.loadSpriteFrame("texture/platform/" + n, e => {
t.spriteFrame = e;
});
s.active = !0;
}
}
}
a++;
const l = Object.keys(this.payModeConfig[n]), d = this.itemNode[o];
if (d) {
let e = d.getComponent(r.default);
e || (e = d.addComponent(r.default)).registerTouchEvent(() => {
this.onItemClicked(l, n, o);
});
}
let h = t.accountTarget >= 0 ? t.accountTarget : 0;
const u = Object.keys(this.payModeConfig[e[h]]), g = e[i[h]];
let p = this.payModeConfig[g][u[0]];
if (1 == u.length) {
this.editBox[0].placeholder = p[1];
this.editBox[0].string = t.accountName;
this.nameLabel[0].string = p[0];
} else if (2 == u.length) {
let e = this.payModeConfig[g][u[u.length - 1]];
this.editBox[0].placeholder = p[1];
this.editBox[0].string = t.accountName;
this.editBox[1].placeholder = e[1];
this.editBox[1].string = t.accountEmail;
this.nameLabel[0].string = p[0];
this.nameLabel[1].string = e[0];
}
this.editBox[1].node.active = u.length > 1;
this.nameLabel[1].node.active = u.length > 1;
this.showPayNode(u, t.accountTarget);
}
}
for (let e = a; e < this.itemNode.length; e++) this.itemNode[e].active = !1;
this.itemNode2.active = a > 2;
this.updateConfirmButtonState();
}
showPayNode(e, t) {
for (let e = 0; e < this.itemNode.length; e++) this.itemNode[e].getChildByName("select").active = e == t;
this.editBox[1].node.active = e.length > 1;
this.nameLabel[1].node.active = e.length > 1;
}
};
i([ u(cc.Label) ], g.prototype, "title", void 0);
i([ u(cc.Label) ], g.prototype, "platformLabel", void 0);
i([ u(cc.Node) ], g.prototype, "btnClose", void 0);
i([ u(cc.Node) ], g.prototype, "itemNode2", void 0);
i([ u([ cc.Node ]) ], g.prototype, "itemNode", void 0);
i([ u([ cc.Label ]) ], g.prototype, "nameLabel", void 0);
i([ u([ cc.EditBox ]) ], g.prototype, "editBox", void 0);
i([ u(cc.Label) ], g.prototype, "detailsTipsLabel", void 0);
i([ u(cc.Node) ], g.prototype, "confirmBtn", void 0);
i([ u(cc.Label) ], g.prototype, "confirmBtnLabel", void 0);
i([ u([ cc.SpriteFrame ]) ], g.prototype, "confirmBtnSpr", void 0);
g = i([ o.registerUIPath("GameDialog/AccountDetailsView"), h ], g);
a.default = g;
cc._RF.pop();
};
