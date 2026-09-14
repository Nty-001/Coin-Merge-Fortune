// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: ImagesPanel; dependency map: {}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "658043FPBdJWpjjTasnP92n", "ImagesPanel");
var i = this && this.__decorate || function(e, t, a, i) {
var o, n = arguments.length, s = n < 3 ? t : null === i ? i = Object.getOwnPropertyDescriptor(t, a) : i;
if ("object" == typeof Reflect && "function" == typeof Reflect.decorate) s = Reflect.decorate(e, t, a, i); else for (var r = e.length - 1; r >= 0; r--) (o = e[r]) && (s = (n < 3 ? o(s) : n > 3 ? o(t, a, s) : o(t, a)) || s);
return n > 3 && s && Object.defineProperty(t, a, s), s;
};
Object.defineProperty(a, "__esModule", {
value: !0
});
const {ccclass: o, property: n, executeInEditMode: s, menu: r} = cc._decorator;
let l = class extends cc.Component {
constructor() {
super(...arguments);
this.images = [];
this._value = 0;
}
get value() {
return this._value;
}
set value(e) {
if (this._value !== e) {
this._value = e;
this._updateValue();
}
}
onLoad() {
this._loadPanel();
this._updateValue();
}
_loadPanel() {
this.node.getComponent(cc.Sprite) || this.node.addComponent(cc.Sprite);
}
_updateValue() {
if (this._value >= 0 && this._value < this.images.length) {
const e = this.images[this._value], t = this.node.getComponent(cc.Sprite);
t && (t.spriteFrame = e);
}
}
};
i([ n({
type: cc.SpriteFrame,
tooltip: "根据序号显示图片",
displayName: "显示图片"
}) ], l.prototype, "images", void 0);
i([ n ], l.prototype, "_value", void 0);
i([ n(cc.Integer) ], l.prototype, "value", null);
l = i([ o, s, r("通用组件/ImagesPanel") ], l);
a.default = l;
cc._RF.pop();
};
