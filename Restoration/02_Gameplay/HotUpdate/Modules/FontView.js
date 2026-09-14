// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: FontView; dependency map: {"./GameManagement":"GameManagement"}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "828a89/o+lD2o7BDeEFbGzx", "FontView");
var i = this && this.__decorate || function(e, t, a, i) {
var o, n = arguments.length, s = n < 3 ? t : null === i ? i = Object.getOwnPropertyDescriptor(t, a) : i;
if ("object" == typeof Reflect && "function" == typeof Reflect.decorate) s = Reflect.decorate(e, t, a, i); else for (var r = e.length - 1; r >= 0; r--) (o = e[r]) && (s = (n < 3 ? o(s) : n > 3 ? o(t, a, s) : o(t, a)) || s);
return n > 3 && s && Object.defineProperty(t, a, s), s;
};
Object.defineProperty(a, "__esModule", {
value: !0
});
const o = e("./GameManagement"), {ccclass: n, property: s} = cc._decorator;
let r = class extends cc.Component {
constructor() {
super(...arguments);
this.ttffont = null;
}
onEnable() {
this.setLabelFont();
}
setLabelFont() {
let e = o.default.language, t = this.node.getComponentsInChildren(cc.Label);
const a = o.default.getCountryIdByCountry(e), i = [ 2, 16, 18, 20, 17, 22, 24, 25, 19, 23 ].includes(a);
t.forEach(e => {
if (i) e.useSystemFont = !0; else {
e.useSystemFont = !1;
e.font = this.ttffont;
}
});
this.node.getComponentsInChildren(cc.RichText).forEach(e => {
if (i) e.useSystemFont = !0; else {
e.useSystemFont = !1;
e.font = this.ttffont;
}
});
}
};
i([ s(cc.TTFFont) ], r.prototype, "ttffont", void 0);
r = i([ n ], r);
a.default = r;
cc._RF.pop();
};
