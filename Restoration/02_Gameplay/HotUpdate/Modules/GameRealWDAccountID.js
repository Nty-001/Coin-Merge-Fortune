// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: GameRealWDAccountID; dependency map: {"../BaseUIManager/BaseUI":"BaseUI","../BaseUIManager/Storage/GameLocalData":"GameLocalData","../BaseUIManager/Storage/PlayData":"PlayData","../BaseUIManager/Storage/TouchButton":"TouchButton","../BaseUIManager/UIConfig":"UIConfig","../BaseUIManager/UIManagerNew":"UIManagerNew","../HWL/ServerConfig":"ServerConfig","../LanguageControl/GameManagement":"GameManagement","../LanguageControl/Lab":"Lab","./AccountCheckManager":"AccountCheckManager","./GameRealWDDialog":"GameRealWDDialog"}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "5747emnmN9BiKAry+SBYm0X", "GameRealWDAccountID");
var i = this && this.__decorate || function(e, t, a, i) {
var o, n = arguments.length, s = n < 3 ? t : null === i ? i = Object.getOwnPropertyDescriptor(t, a) : i;
if ("object" == typeof Reflect && "function" == typeof Reflect.decorate) s = Reflect.decorate(e, t, a, i); else for (var r = e.length - 1; r >= 0; r--) (o = e[r]) && (s = (n < 3 ? o(s) : n > 3 ? o(t, a, s) : o(t, a)) || s);
return n > 3 && s && Object.defineProperty(t, a, s), s;
};
Object.defineProperty(a, "__esModule", {
value: !0
});
const o = e("../BaseUIManager/BaseUI"), n = e("../BaseUIManager/Storage/GameLocalData"), s = e("../BaseUIManager/Storage/PlayData"), r = e("../BaseUIManager/Storage/TouchButton"), l = e("../BaseUIManager/UIConfig"), c = e("../BaseUIManager/UIManagerNew"), d = e("../HWL/ServerConfig"), h = e("../LanguageControl/GameManagement"), u = e("../LanguageControl/Lab"), g = e("./AccountCheckManager"), p = e("./GameRealWDDialog"), {ccclass: f, property: m} = cc._decorator;
let y = class extends o.default {
constructor() {
super(...arguments);
this.btnClose = null;
this.title = null;
this.platfroms = [];
this.accountLabel = null;
this.accountEdit = null;
this.nameLabel = null;
this.nameEdit = null;
this.tips = null;
this.btnLabel = null;
this.confirmBtn = null;
this.confirmBtnSpr = [];
this.currentIndex = 0;
this.param = null;
this.player = null;
this._currentPlatform = "";
this._tempPlatform = "";
this.platformSpr = [];
}
onLoad() {
super.onLoad();
this.btnClose.addComponent(r.default).registerTouchEvent(() => {
this.on_close_call();
});
this.accountEdit.node.on("text-changed", () => {
this.checkBtn();
});
this.nameEdit.node.on("text-changed", () => {
this.checkBtn();
});
this.btnLabel.node.parent.addComponent(r.default).registerTouchEvent(() => {
let e = n.default.getInstance().getData(s.default), t = this.validatePhone(this.accountEdit.string).valid, a = g.default.validateAccount(this.nameEdit.string).valid;
if (!t || !a) {
let e = u.default.getlab("14");
c.default.show_toast({
text: e
});
return;
}
e.raccountName = this.accountEdit.string;
e.rfullName = this.nameEdit.string;
e.rdocumentId = "";
n.default.getInstance().saveToUserDefault();
if (this._tempPlatform != this._currentPlatform) {
this.on_close_call();
p.default.instance && p.default.instance.showRewardView();
return;
}
if (!this.param.allConditionsMet) {
this.on_close_call();
return;
}
const i = {
ui_config_path: l.default.GameRealTXYZ,
ui_config_name: "GameRealTXYZ",
param: {
currentIndex: this.currentIndex,
account: ""
}
};
c.default.show_ui(i);
this.on_close_call();
});
this.platfroms[0].node.addComponent(r.default).registerTouchEvent(() => {
for (let e = 0; e < this.platfroms.length; e++) this.platfroms[e].node.getChildByName("select").active = 0 == e;
this._currentPlatform = this.platformSpr[0];
this.player.realSelectPlatform = this._currentPlatform;
n.default.getInstance().saveToUserDefault();
});
this.platfroms[1].node.addComponent(r.default).registerTouchEvent(() => {
for (let e = 0; e < this.platfroms.length; e++) this.platfroms[e].node.getChildByName("select").active = 1 == e;
if (this.platformSpr.length >= 2) {
this._currentPlatform = this.platformSpr[1];
this.player.realSelectPlatform = this._currentPlatform;
n.default.getInstance().saveToUserDefault();
}
});
this.platfroms[2].node.addComponent(r.default).registerTouchEvent(() => {
for (let e = 0; e < this.platfroms.length; e++) this.platfroms[e].node.getChildByName("select").active = 2 == e;
if (this.platformSpr.length >= 3) {
this._currentPlatform = this.platformSpr[2];
this.player.realSelectPlatform = this._currentPlatform;
n.default.getInstance().saveToUserDefault();
}
});
this.refreshPlatform();
}
show(e) {
super.show(e);
this.title.string = u.default.getlab("13");
this.btnLabel.string = u.default.getlab("108");
this.tips.string = u.default.getlab("14");
this.param = e.param;
this.currentIndex = this.param.currentIndex;
this.player = n.default.getInstance().getData(s.default);
this.accountEdit.string = this.player.raccountName;
this._tempPlatform = this.player.realSelectPlatform;
this._currentPlatform = this.player.realSelectPlatform;
this.accountLabel.string = u.default.getlab("96");
this.nameLabel.string = u.default.getlab("95");
if ("" == this.player.raccountName) {
this.accountEdit.placeholder = u.default.getlab("19");
this.accountEdit.string = "";
} else {
this.accountEdit.placeholder = this.player.raccountName;
this.accountEdit.string = this.player.raccountName;
}
if ("" == this.player.rfullName) {
this.nameEdit.placeholder = u.default.getlab("18");
this.nameEdit.string = "";
} else {
this.nameEdit.placeholder = this.player.rfullName;
this.nameEdit.string = this.player.rfullName;
}
this.refreshPlatform();
}
refreshPlatform() {
if (this.player) {
this.platformSpr = h.default.getPlatformSpr();
if (this.platformSpr.length > 0 && this.platformSpr.indexOf(this.player.realSelectPlatform) < 0) {
this.player.realSelectPlatform = this.platformSpr[0];
this._tempPlatform = this.player.realSelectPlatform;
this._currentPlatform = this.player.realSelectPlatform;
}
this.platfroms[1].node.active = this.platformSpr.length >= 2;
this.platfroms[2].node.active = this.platformSpr.length >= 3;
for (let e = 0; e < this.platformSpr.length; e++) {
h.default.loadSpriteFrame("texture/CashPlat/" + this.platformSpr[e] + "_1", t => {
this.platfroms[e].spriteFrame = t;
});
this.platfroms[e].node.getChildByName("select").active = this.platformSpr[e] == this.player.realSelectPlatform;
n.default.getInstance().saveToUserDefault();
}
}
}
checkBtn() {
const e = this.accountEdit.string.length > 0 && this.nameEdit.string.length > 0, t = this.confirmBtn.getComponent(cc.Sprite);
t && (t.spriteFrame = e ? this.confirmBtnSpr[0] : this.confirmBtnSpr[1]);
this.btnLabel.getComponent(cc.LabelOutline).color = e ? new cc.Color(7, 113, 50) : new cc.Color(84, 84, 84);
}
validatePhone(e) {
switch (this.player.realSelectPlatform) {
case d.RealCashPlatform.DANA:
case d.RealCashPlatform.OVO:
return g.default.validatePhone08(e);

case d.RealCashPlatform.Truemoney:
return g.default.validatePhone10(e);

case d.RealCashPlatform.TNG:
return g.default.validatePhone10Or11(e);

case d.RealCashPlatform.ZaloPay:
return g.default.validatePhone84(e);

case d.RealCashPlatform.GCash:
case d.RealCashPlatform.Graboay:
case d.RealCashPlatform.Paymaya:
return g.default.validatePhone09(e);
}
}
};
i([ m(cc.Node) ], y.prototype, "btnClose", void 0);
i([ m(cc.Label) ], y.prototype, "title", void 0);
i([ m([ cc.Sprite ]) ], y.prototype, "platfroms", void 0);
i([ m(cc.Label) ], y.prototype, "accountLabel", void 0);
i([ m(cc.EditBox) ], y.prototype, "accountEdit", void 0);
i([ m(cc.Label) ], y.prototype, "nameLabel", void 0);
i([ m(cc.EditBox) ], y.prototype, "nameEdit", void 0);
i([ m(cc.Label) ], y.prototype, "tips", void 0);
i([ m(cc.Label) ], y.prototype, "btnLabel", void 0);
i([ m(cc.Node) ], y.prototype, "confirmBtn", void 0);
i([ m([ cc.SpriteFrame ]) ], y.prototype, "confirmBtnSpr", void 0);
y = i([ o.registerUIPath("GameDialog/GameRealWDAccountID"), f ], y);
a.default = y;
cc._RF.pop();
};
