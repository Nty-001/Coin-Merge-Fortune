// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: JBlab; dependency map: {}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "c190buIb55Iz7YnrBO178fD", "JBlab");
var i = this && this.__decorate || function(e, t, a, i) {
var o, n = arguments.length, s = n < 3 ? t : null === i ? i = Object.getOwnPropertyDescriptor(t, a) : i;
if ("object" == typeof Reflect && "function" == typeof Reflect.decorate) s = Reflect.decorate(e, t, a, i); else for (var r = e.length - 1; r >= 0; r--) (o = e[r]) && (s = (n < 3 ? o(s) : n > 3 ? o(t, a, s) : o(t, a)) || s);
return n > 3 && s && Object.defineProperty(t, a, s), s;
};
Object.defineProperty(a, "__esModule", {
value: !0
});
const {ccclass: o, property: n} = cc._decorator;
let s = class extends cc.Component {
constructor() {
super(...arguments);
this.colors = [ cc.Color.RED, cc.Color.BLUE ];
}
onLoad() {
this._cmp = this.getComponent(cc.RenderComponent);
console.log(" =========== cmp   ", this._cmp);
}
onEnable() {
cc.director.on(cc.Director.EVENT_AFTER_DRAW, this._updateColors, this);
this.node._renderFlag |= cc.RenderFlow.FLAG_COLOR;
}
onDisable() {
cc.director.off(cc.Director.EVENT_AFTER_DRAW, this._updateColors, this);
this.node._renderFlag |= cc.RenderFlow.FLAG_COLOR;
}
_updateColors() {
if (!this._cmp) return;
const e = this._cmp._assembler;
if (!(e instanceof cc.Assembler2D)) return;
const t = e._renderData.uintVDatas[0];
if (!t) return;
const a = e.floatsPerVert;
let i = 0;
for (let o = e.colorOffset; o < t.length; o += a) t[o] = (this.colors[i++] || this.colors[0])._val;
}
updateColors() {
this.node._renderFlag |= cc.RenderFlow.FLAG_COLOR;
}
};
i([ n([ cc.Color ]) ], s.prototype, "colors", void 0);
s = i([ o ], s);
a.default = s;
cc._RF.pop();
};
