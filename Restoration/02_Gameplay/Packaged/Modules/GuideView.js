// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/base/assets/assets/main/index.ba75b.js
// Module: GuideView; dependency map: {"../ATools/BaseUI":"BaseUI","../ATools/EventCenter":"EventCenter","../ATools/GameEventConsts":"GameEventConsts","../ATools/LocalDataManager":"LocalDataManager","../ATools/TouchButton":"TouchButton","../ATools/UIManager":"UIManager"}
module.exports = function(e, t, o) {
"use strict";
cc._RF.push(t, "a8fb6EdUJROLYRpOqrP19Dk", "GuideView");
var a = this && this.__decorate || function(e, t, o, a) {
var s, n = arguments.length, i = n < 3 ? t : null === a ? a = Object.getOwnPropertyDescriptor(t, o) : a;
if ("object" == typeof Reflect && "function" == typeof Reflect.decorate) i = Reflect.decorate(e, t, o, a); else for (var c = e.length - 1; c >= 0; c--) (s = e[c]) && (i = (n < 3 ? s(i) : n > 3 ? s(t, o, i) : s(t, o)) || i);
return n > 3 && i && Object.defineProperty(t, o, i), i;
};
Object.defineProperty(o, "__esModule", {
value: !0
});
const s = e("../ATools/BaseUI"), n = e("../ATools/EventCenter"), i = e("../ATools/GameEventConsts"), c = e("../ATools/LocalDataManager"), l = e("../ATools/TouchButton"), r = e("../ATools/UIManager"), {ccclass: h, property: u} = cc._decorator;
let d = class extends s.default {
constructor() {
super(...arguments);
this.btn_Agree = null;
this.btn_Reject = null;
this.btn_Privacy = null;
this.btn_Agreement = null;
}
onLoad() {
this.registerEvent();
}
start() {}
onShow(e) {}
registerEvent() {
this.btn_Agree.addComponent(l.default).registerTouchEvent(this.onClickAgree.bind(this));
this.btn_Reject.addComponent(l.default).registerTouchEvent(this.onClickReject.bind(this));
this.btn_Privacy.addComponent(l.default).registerTouchEvent(this.onClickPrivacy.bind(this));
this.btn_Agreement.addComponent(l.default).registerTouchEvent(this.onClickAgreement.bind(this));
}
onClickAgree() {
n.EventCenter.getInstance().fire(i.GameEventName.AgreeMent);
c.default.getInstance().getGameData().setIsNewUser(!1);
this.onDestroyView();
}
onClickReject() {
cc.sys.localStorage.clear();
cc.game.end();
}
onClickPrivacy() {
r.default.instance.onShowView(s.UIConfig.PolicyView, {
type: 1
});
}
onClickAgreement() {
r.default.instance.onShowView(s.UIConfig.PolicyView, {
type: 2
});
}
};
a([ u(cc.Node) ], d.prototype, "btn_Agree", void 0);
a([ u(cc.Node) ], d.prototype, "btn_Reject", void 0);
a([ u(cc.Node) ], d.prototype, "btn_Privacy", void 0);
a([ u(cc.Node) ], d.prototype, "btn_Agreement", void 0);
d = a([ s.registerUIPath("prefab/GuideView"), h ], d);
o.default = d;
cc._RF.pop();
};
