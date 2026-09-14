// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: FlyAnimation; dependency map: {"../Report/NativeCall":"NativeCall","../game/UI_TopRewardLayer":"UI_TopRewardLayer","./EventListener/EventCenter":"EventCenter","./Storage/GameEventConsts":"GameEventConsts","./Storage/GameLocalData":"GameLocalData","./Storage/NewGamePlayData":"NewGamePlayData","./Storage/SoundManager":"SoundManager","./UIConfig":"UIConfig","./UIManagerNew":"UIManagerNew"}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "ddc501F+pROSY5JIdP0Ep4D", "FlyAnimation");
var i, o = this && this.__decorate || function(e, t, a, i) {
var o, n = arguments.length, s = n < 3 ? t : null === i ? i = Object.getOwnPropertyDescriptor(t, a) : i;
if ("object" == typeof Reflect && "function" == typeof Reflect.decorate) s = Reflect.decorate(e, t, a, i); else for (var r = e.length - 1; r >= 0; r--) (o = e[r]) && (s = (n < 3 ? o(s) : n > 3 ? o(t, a, s) : o(t, a)) || s);
return n > 3 && s && Object.defineProperty(t, a, s), s;
}, n = this && this.__awaiter || function(e, t, a, i) {
return new (a || (a = Promise))(function(o, n) {
function s(e) {
try {
l(i.next(e));
} catch (e) {
n(e);
}
}
function r(e) {
try {
l(i.throw(e));
} catch (e) {
n(e);
}
}
function l(e) {
e.done ? o(e.value) : (t = e.value, t instanceof a ? t : new a(function(e) {
e(t);
})).then(s, r);
var t;
}
l((i = i.apply(e, t || [])).next());
});
};
Object.defineProperty(a, "__esModule", {
value: !0
});
const s = e("../CardPlayGame/GameScene"), r = e("./EventListener/EventCenter"), l = e("./Storage/GameEventConsts"), c = e("./Storage/GameLocalData"), d = e("./Storage/NewGamePlayData"), h = e("./Storage/SoundManager"), u = e("../game/UI_TopRewardLayer"), g = e("./UIConfig"), p = e("./UIManagerNew"), f = e("../Report/NativeCall"), {ccclass: m, property: y} = cc._decorator;
let _ = i = class extends cc.Component {
constructor() {
super(...arguments);
this.flyicon = null;
this.flyicontip = null;
this.flyiconrevoke = null;
this.flyiconmove = null;
this.flyrealicon = null;
this.seed = new Date().getTime();
}
onLoad() {
i.instance = this;
}
ShowPropsFlyEffect(e, t, a) {
const i = s.default.getInstance();
let o = null, n = this.flyicontip;
switch (e) {
case 2:
o = i.addSwimRingBtn;
n = this.flyicontip;
break;

case 3:
o = i.refreshBtn;
n = this.flyiconrevoke;
break;

case 4:
o = i.superTurretBtn;
n = this.flyiconmove;
}
const r = a instanceof cc.Node ? a.parent.convertToWorldSpaceAR(a.position) : a, l = i.node.convertToNodeSpaceAR(r), c = o.parent.convertToWorldSpaceAR(o.position), d = i.node.convertToNodeSpaceAR(c);
this.flyPropsAction(n, l, d, e, t);
}
ShowCoinEffect(e, t, a = !0) {
const i = u.default.instance.icon.parent.convertToWorldSpaceAR(u.default.instance.icon.position), o = s.default.getInstance().node.convertToNodeSpaceAR(i);
c.default.getInstance().getData(d.default).add_red_bag(e);
this.fly_add_action(t, o, e);
a && this.ShowRealCoinEffect(c.default.getInstance().getData(d.default).addVodeoshowCoin, t);
}
ShowRealCoinEffect(e, t) {
if (!f.default.checkClientEndingWithCoin()) return;
let a = u.default.instance, i = s.default.getInstance();
if (!(a && a.node_coin && a.node_coin.parent && i && i.node)) return;
const o = a.node_coin.parent.convertToWorldSpaceAR(a.node_coin.position), n = i.node.convertToNodeSpaceAR(o);
this.flyRealCoinAction(t, n);
}
getRelativePosition(e, t) {
const a = (e.getParent() || e).convertToWorldSpaceAR(e.position);
return t.convertToNodeSpaceAR(a);
}
range(e = 0, t = 1) {
this.seed || 0 == this.seed || (this.seed = new Date().getTime());
t = t || 1;
e = e || 0;
this.seed = (9301 * this.seed + 49297) % 233280;
return e + this.seed / 233280 * (t - e);
}
rangeInt(e, t) {
var a = t - e, i = this.range(0, 1);
return e + Math.round(i * a);
}
fly_add_action(e, t, a) {
e instanceof cc.Node && (e = this.getRelativePosition(e, s.default.getInstance().node));
h.default.playSound("collect");
let i = Math.min(t.sub(e).mag() / 650, .6);
for (let o = 0; o < 3; o++) {
const n = setTimeout(() => {
let c = this.flyicon, d = u.default.instance.redLabel.node.parent, h = cc.instantiate(c);
h.parent = s.default.getInstance().node;
h.position = e;
const g = cc.callFunc(() => {
cc.tween(h).sequence(cc.tween().to(i, {
position: t,
scale: .35
}), cc.tween().call(() => {
r.EventCenter.getInstance().fire(l.GameEventName.MoneyChangedNtf);
cc.tween(d).sequence(cc.tween().to(.3, {
scale: 1.1
}), cc.tween().to(.1, {
scale: 1
})).start();
o >= 2 && this.createRewardToast(a, t);
h.destroy();
})).start();
});
h.runAction(g);
clearTimeout(n);
}, o * this.rangeInt(10, 60));
}
}
flyPropsAction(e, t, a, i, o) {
h.default.playSound("collect");
for (let i = 0; i < 3; i++) {
const o = setTimeout(() => {
let i = u.default.instance.redLabel.node.parent, n = cc.instantiate(e);
n.parent = s.default.getInstance().node;
n.position = t;
const c = cc.callFunc(() => {
cc.tween(n).sequence(cc.tween().to(.6, {
position: a,
scale: .35
}), cc.tween().call(() => {
r.EventCenter.getInstance().fire(l.GameEventName.PropsNtf);
cc.tween(i).sequence(cc.tween().to(.3, {
scale: 1.1
}), cc.tween().to(.1, {
scale: 1
})).start();
n.destroy();
})).start();
});
n.runAction(c);
clearTimeout(o);
}, i * this.rangeInt(10, 60));
}
}
flyRealCoinAction(e, t) {
e instanceof cc.Node && (e = this.getRelativePosition(e, s.default.getInstance().node));
h.default.playSound("collect");
for (let a = 0; a < 3; a++) {
const i = setTimeout(() => {
let o = u.default.instance.node_coin, n = cc.instantiate(this.flyrealicon);
n.parent = s.default.getInstance().node;
n.position = e;
const c = cc.callFunc(() => {
cc.tween(n).sequence(cc.tween().to(.6, {
position: t,
scale: .35
}), cc.tween().call(() => {
r.EventCenter.getInstance().fire(l.GameEventName.RealWDNtf);
cc.tween(o).sequence(cc.tween().to(.3, {
scale: 1.1
}), cc.tween().to(.1, {
scale: 1
})).start();
a >= 2 && this.createRewardRealToast(1, t);
n.destroy();
})).start();
});
n.runAction(c);
clearTimeout(i);
}, a * this.rangeInt(10, 60));
}
}
createRewardToast(e, t) {
return n(this, void 0, void 0, function*() {
const a = {
ui_config_path: g.default.RewardToast,
ui_config_name: "RewardToast",
param: {
value: e,
endPos: t
}
};
p.default.show_ui(a);
});
}
createRewardRealToast(e, t) {
return n(this, void 0, void 0, function*() {
const a = {
ui_config_path: g.default.RewardRealToast,
ui_config_name: "RewardRealToast",
param: {
value: e,
endPos: t
}
};
p.default.show_ui(a);
});
}
};
_.instance = null;
_.seed = new Date().getTime();
o([ y(cc.Prefab) ], _.prototype, "flyicon", void 0);
o([ y(cc.Prefab) ], _.prototype, "flyicontip", void 0);
o([ y(cc.Prefab) ], _.prototype, "flyiconrevoke", void 0);
o([ y(cc.Prefab) ], _.prototype, "flyiconmove", void 0);
o([ y(cc.Prefab) ], _.prototype, "flyrealicon", void 0);
_ = i = o([ m ], _);
a.default = _;
cc._RF.pop();
};
