// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/base/assets/assets/main/index.ba75b.js
// Module: Utils; dependency map: {"../ATools/SoundManager":"SoundManager"}
module.exports = function(e, t, o) {
"use strict";
cc._RF.push(t, "7213787GeJADbSUL5kZh4mQ", "Utils");
Object.defineProperty(o, "__esModule", {
value: !0
});
const a = e("../ATools/SoundManager");
o.default = class {
static getRandomIntInRange(e, t) {
return Math.floor(Math.random() * (t - e + 1) + e);
}
static playSpine(e, t, o = !1, a = null) {
if (e && t) {
e.setAnimation(0, t, o);
e.setCompleteListener(() => {
null == a || a(e);
});
}
}
static addClick(e, t, o) {
e instanceof cc.Node && !e.getComponent(cc.Button) && e.addComponent(cc.Button);
const s = e instanceof cc.Button ? e : e.getComponent(cc.Button);
s.transition = cc.Button.Transition.SCALE;
s.node.on("click", () => {
a.default.playClickSound();
t.call(o || this, s);
}, this);
}
static loadRes(e, t) {
return new Promise((o, a) => {
cc.loader.loadRes(e, t, (e, t) => {
e ? a && a(e) : o && o(t);
});
});
}
static getTimeStr(e, t = !1) {
let o = Math.round(e / 1e3);
const a = this.prefixZero(o % 60);
if ((o = Math.floor(o / 60)) <= 60 && t) return `${this.prefixZero(o)}:${a}`;
{
const e = this.prefixZero(o % 60);
o = Math.floor(o / 60);
this.prefixZero(o);
return `${e}:${a}`;
}
}
static prefixZero(e, t = 2) {
return (Array(t).join("0") + Math.floor(e)).slice(-t);
}
};
cc._RF.pop();
};
