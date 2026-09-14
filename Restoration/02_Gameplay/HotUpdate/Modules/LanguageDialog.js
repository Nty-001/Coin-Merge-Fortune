// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: LanguageDialog; dependency map: {"../BaseUIManager/BaseUI":"BaseUI","../BaseUIManager/EventListener/EventCenter":"EventCenter","../BaseUIManager/Storage/GameEventConsts":"GameEventConsts","../BaseUIManager/Storage/TouchButton":"TouchButton","../LanguageControl/GameManagement":"GameManagement","../LanguageControl/Lab":"Lab"}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "5c78d5tq0dCbIRbz4aGnsHW", "LanguageDialog");
var i = this && this.__decorate || function(e, t, a, i) {
var o, n = arguments.length, s = n < 3 ? t : null === i ? i = Object.getOwnPropertyDescriptor(t, a) : i;
if ("object" == typeof Reflect && "function" == typeof Reflect.decorate) s = Reflect.decorate(e, t, a, i); else for (var r = e.length - 1; r >= 0; r--) (o = e[r]) && (s = (n < 3 ? o(s) : n > 3 ? o(t, a, s) : o(t, a)) || s);
return n > 3 && s && Object.defineProperty(t, a, s), s;
};
Object.defineProperty(a, "__esModule", {
value: !0
});
const o = e("../LanguageControl/GameManagement"), n = e("../BaseUIManager/Storage/TouchButton"), s = e("../BaseUIManager/BaseUI"), r = e("../BaseUIManager/EventListener/EventCenter"), l = e("../BaseUIManager/Storage/GameEventConsts"), c = e("../CardPlayGame/GameScene"), d = e("../LanguageControl/Lab"), h = {
US: "English",
ID: "Bahasa Indonesia",
RU: "Русский язык",
IN: "हिन्दी",
BR: "Português",
FR: "Français",
DE: "Deutsch",
AR: "Español",
KR: "한국어",
JP: "日本語",
VN: "Tiếng Việt",
MY: "Bahasa Melayu",
TR: "Türkçe",
TH: "ภาษาไทย",
PK: "اُردُو",
BD: "বাংলা",
EG: "العَرَبِيَّة",
KZ: "Қазақ тілі"
}, {ccclass: u, property: g} = cc._decorator;
let p = class extends s.default {
constructor() {
super(...arguments);
this.btnClose = null;
this.titleLabel = null;
this.items = null;
this.item = null;
}
onLoad() {
this.btnClose.addComponent(n.default).registerTouchEvent(() => {
this.on_close_call();
});
}
show(e) {
super.show(e);
this.titleLabel.string = d.default.getlab("5");
this.items.removeAllChildren();
const t = Object.entries(h);
for (let e = 0; e < t.length; e++) {
const [a, i] = t[e], o = cc.instantiate(this.item);
this.items.addChild(o);
const s = o.getChildByName("languageLabel").getComponent(cc.Label);
s && (s.string = h[i] || i);
o.addComponent(n.default).registerTouchEvent(() => {
this.onLanguageItemSelected(a, o);
});
}
}
onLanguageItemSelected(e, t) {
console.log(`选择了语言：${e}`);
o.default.language = o.default.normalizeCountry(e);
r.EventCenter.getInstance().fire(l.GameEventName.RefreshLanguage);
c.default.getInstance().generateBatteries();
this.on_close_call();
}
};
i([ g(cc.Node) ], p.prototype, "btnClose", void 0);
i([ g(cc.Label) ], p.prototype, "titleLabel", void 0);
i([ g(cc.Node) ], p.prototype, "items", void 0);
i([ g(cc.Prefab) ], p.prototype, "item", void 0);
p = i([ s.registerUIPath("GameDialog/LanguageDialog"), u ], p);
a.default = p;
cc._RF.pop();
};
