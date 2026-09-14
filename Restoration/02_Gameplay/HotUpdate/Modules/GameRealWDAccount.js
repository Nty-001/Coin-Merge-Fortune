// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: GameRealWDAccount; dependency map: {"../BaseUIManager/BaseUI":"BaseUI","../BaseUIManager/Storage/GameLocalData":"GameLocalData","../BaseUIManager/Storage/PlayData":"PlayData","../BaseUIManager/Storage/TouchButton":"TouchButton","../BaseUIManager/UIConfig":"UIConfig","../BaseUIManager/UIManagerNew":"UIManagerNew","../LanguageControl/Lab":"Lab","./AccountCheckManager":"AccountCheckManager"}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "3d2a3YZqD5OLJ+ysE4b5Kjc", "GameRealWDAccount");
var i = this && this.__decorate || function(e, t, a, i) {
var o, n = arguments.length, s = n < 3 ? t : null === i ? i = Object.getOwnPropertyDescriptor(t, a) : i;
if ("object" == typeof Reflect && "function" == typeof Reflect.decorate) s = Reflect.decorate(e, t, a, i); else for (var r = e.length - 1; r >= 0; r--) (o = e[r]) && (s = (n < 3 ? o(s) : n > 3 ? o(t, a, s) : o(t, a)) || s);
return n > 3 && s && Object.defineProperty(t, a, s), s;
};
Object.defineProperty(a, "__esModule", {
value: !0
});
const o = e("../BaseUIManager/BaseUI"), n = e("../BaseUIManager/Storage/GameLocalData"), s = e("../BaseUIManager/Storage/PlayData"), r = e("../BaseUIManager/Storage/TouchButton"), l = e("../BaseUIManager/UIConfig"), c = e("../BaseUIManager/UIManagerNew"), d = e("../LanguageControl/Lab"), h = e("./AccountCheckManager"), {ccclass: u, property: g} = cc._decorator;
let p = class extends o.default {
constructor() {
super(...arguments);
this.btnClose = null;
this.title = null;
this.emailLabel = null;
this.emailEdit = null;
this.tips = null;
this.btnLabel = null;
this.confirmBtn = null;
this.confirmBtnSpr = [];
this.currentIndex = 0;
this.param = null;
}
onLoad() {
super.onLoad();
this.btnClose.addComponent(r.default).registerTouchEvent(() => {
this.on_close_call();
});
this.emailEdit.node.on("text-changed", () => {
const e = "" !== this.emailEdit.string.trim(), t = this.confirmBtn.getComponent(cc.Sprite);
t && (t.spriteFrame = e ? this.confirmBtnSpr[0] : this.confirmBtnSpr[1]);
this.btnLabel.getComponent(cc.LabelOutline).color = e ? new cc.Color(7, 113, 50) : new cc.Color(84, 84, 84);
});
this.btnLabel.node.parent.addComponent(r.default).registerTouchEvent(() => {
const e = h.default.validateEmail(this.emailEdit.string);
let t = n.default.getInstance().getData(s.default);
if (!e.valid) {
let e = d.default.getlab("14");
c.default.show_toast({
text: e
});
return;
}
t.raccountName = e.email;
t.rfullName = "";
t.rdocumentId = "";
n.default.getInstance().saveToUserDefault();
const a = {
ui_config_path: l.default.GameRealTXYZ,
ui_config_name: "GameRealTXYZ",
param: {
currentIndex: this.currentIndex,
account: e.email
}
};
c.default.show_ui(a);
this.on_close_call();
});
this.title.string = d.default.getlab("13");
this.btnLabel.string = d.default.getlab("108");
this.emailLabel.string = d.default.getlab("96");
this.tips.string = d.default.getlab("14");
}
show(e) {
super.show(e);
this.param = e.param;
this.currentIndex = this.param.currentIndex;
let t = n.default.getInstance().getData(s.default);
this.emailEdit.string = t.raccountName;
if ("" == t.raccountName) {
this.emailEdit.placeholder = d.default.getlab("19");
this.emailEdit.string = "";
} else {
this.emailEdit.placeholder = t.raccountName;
this.emailEdit.string = t.raccountName;
}
}
};
i([ g(cc.Node) ], p.prototype, "btnClose", void 0);
i([ g(cc.Label) ], p.prototype, "title", void 0);
i([ g(cc.Label) ], p.prototype, "emailLabel", void 0);
i([ g(cc.EditBox) ], p.prototype, "emailEdit", void 0);
i([ g(cc.Label) ], p.prototype, "tips", void 0);
i([ g(cc.Label) ], p.prototype, "btnLabel", void 0);
i([ g(cc.Node) ], p.prototype, "confirmBtn", void 0);
i([ g([ cc.SpriteFrame ]) ], p.prototype, "confirmBtnSpr", void 0);
p = i([ o.registerUIPath("GameDialog/GameRealWDAccount"), u ], p);
a.default = p;
cc._RF.pop();
};
