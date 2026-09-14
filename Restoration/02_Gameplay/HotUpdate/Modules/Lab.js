// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: Lab; dependency map: {"./GameManagement":"GameManagement"}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "2958djucVhM1oVSYkoABSUD", "Lab");
var i = this && this.__decorate || function(e, t, a, i) {
var o, n = arguments.length, s = n < 3 ? t : null === i ? i = Object.getOwnPropertyDescriptor(t, a) : i;
if ("object" == typeof Reflect && "function" == typeof Reflect.decorate) s = Reflect.decorate(e, t, a, i); else for (var r = e.length - 1; r >= 0; r--) (o = e[r]) && (s = (n < 3 ? o(s) : n > 3 ? o(t, a, s) : o(t, a)) || s);
return n > 3 && s && Object.defineProperty(t, a, s), s;
};
Object.defineProperty(a, "__esModule", {
value: !0
});
a.replacePlaceholders = void 0;
const o = e("./GameManagement"), {ccclass: n, property: s} = cc._decorator;
function r(e, t) {
return e.replace(/\%\{(\w+)\}/g, (e, a) => t[a] || 0);
}
a.replacePlaceholders = r;
let l = class extends cc.Component {
static getlab(e, t, a, i, n) {
return o.default.languageJson[o.LANGUAGE_ID[o.default.GetCountryIndewx()]][e] ? o.default.languageJson[o.LANGUAGE_ID[o.default.GetCountryIndewx()]][e] : e;
}
static getstorelab(e, t, a, i, n) {
return o.default.languageJson[o.LANGUAGE_ID[o.default.GetCountryIndewx()]][e] ? r(o.default.languageJson[o.LANGUAGE_ID[o.default.GetCountryIndewx()]][e], {
num1: t,
num2: a,
num3: i,
num4: n
}) : e;
}
static GetLanguageText(e) {
return o.default.languageJson[o.LANGUAGE_ID[o.default.GetCountryIndewx()]][e];
}
static GetDetailsText() {
let e = o.default.normalizeCountry(o.default.language);
const t = o.default.detailsData[e];
return null == t ? o.default.detailsData.US : t;
}
};
l = i([ n ], l);
a.default = l;
cc._RF.pop();
};
