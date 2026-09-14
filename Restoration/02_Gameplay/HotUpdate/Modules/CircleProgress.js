// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: CircleProgress; dependency map: {}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "3bb33v+xVpKg6zDYoqG9tZW", "CircleProgress");
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
this.progressCircle = null;
this.progressLabel = null;
this.circleRadius = 35;
this.circleLineWidth = 15;
}
setCircleProgress(e) {
e = cc.misc.clamp01(e);
this.progressLabel.string = `${Math.floor(100 * e)}%`;
if (this.progressCircle) {
this.progressCircle.clear();
this.progressCircle.lineWidth = this.circleLineWidth;
this.progressCircle.strokeColor = cc.Color.GREEN;
this.progressCircle.lineCap = cc.Graphics.LineCap.BUTT;
const t = Math.PI / 2, a = t + 2 * e * Math.PI;
this.progressCircle.strokeColor = cc.color(203, 91, 17);
this.progressCircle.arc(0, 0, this.circleRadius, 0, 2 * Math.PI, !1);
this.progressCircle.stroke();
if (e > 0) {
this.progressCircle.strokeColor = cc.Color.GREEN;
this.progressCircle.arc(0, 0, this.circleRadius, t, a, !0);
this.progressCircle.stroke();
}
}
}
};
i([ n(cc.Graphics) ], s.prototype, "progressCircle", void 0);
i([ n(cc.Label) ], s.prototype, "progressLabel", void 0);
s = i([ o ], s);
a.default = s;
cc._RF.pop();
};
