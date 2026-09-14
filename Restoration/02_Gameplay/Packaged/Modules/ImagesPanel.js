// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/base/assets/assets/main/index.ba75b.js
// Module: ImagesPanel; dependency map: {}
module.exports = function(e, t, o) {
"use strict";
cc._RF.push(t, "2abafFeuJ9IFJ3ZaMFMMZS8", "ImagesPanel");
var a = this && this.__decorate || function(e, t, o, a) {
var s, n = arguments.length, i = n < 3 ? t : null === a ? a = Object.getOwnPropertyDescriptor(t, o) : a;
if ("object" == typeof Reflect && "function" == typeof Reflect.decorate) i = Reflect.decorate(e, t, o, a); else for (var c = e.length - 1; c >= 0; c--) (s = e[c]) && (i = (n < 3 ? s(i) : n > 3 ? s(t, o, i) : s(t, o)) || i);
return n > 3 && i && Object.defineProperty(t, o, i), i;
};
Object.defineProperty(o, "__esModule", {
value: !0
});
const {ccclass: s, property: n, executeInEditMode: i, menu: c} = cc._decorator;
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
a([ n(cc.SpriteFrame) ], l.prototype, "images", void 0);
a([ n(cc.Integer) ], l.prototype, "value", null);
l = a([ s, i, c("ImagesPanel") ], l);
o.default = l;
cc._RF.pop();
};
