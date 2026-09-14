// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/base/assets/assets/main/index.ba75b.js
// Module: GameScene; dependency map: {"./ATools/BaseUI":"BaseUI","./ATools/EventCenter":"EventCenter","./ATools/GameEventConsts":"GameEventConsts","./ATools/LocalDataManager":"LocalDataManager","./ATools/SoundManager":"SoundManager","./ATools/UIManager":"UIManager","./Game/LevelManager":"LevelManager"}
module.exports = function(e, t, o) {
"use strict";
cc._RF.push(t, "1e715AvPr9LvLFJE2giP6tP", "GameScene");
var a, s = this && this.__decorate || function(e, t, o, a) {
var s, n = arguments.length, i = n < 3 ? t : null === a ? a = Object.getOwnPropertyDescriptor(t, o) : a;
if ("object" == typeof Reflect && "function" == typeof Reflect.decorate) i = Reflect.decorate(e, t, o, a); else for (var c = e.length - 1; c >= 0; c--) (s = e[c]) && (i = (n < 3 ? s(i) : n > 3 ? s(t, o, i) : s(t, o)) || i);
return n > 3 && i && Object.defineProperty(t, o, i), i;
};
Object.defineProperty(o, "__esModule", {
value: !0
});
const n = e("./ATools/BaseUI"), i = e("./ATools/EventCenter"), c = e("./ATools/GameEventConsts"), l = e("./ATools/LocalDataManager"), r = e("./ATools/SoundManager"), h = e("./ATools/UIManager"), u = e("./Game/LevelManager"), {ccclass: d, property: g} = cc._decorator;
let p = a = class extends cc.Component {
constructor() {
super(...arguments);
this.home = null;
this.heartDown = 1200;
}
onLoad() {
a.instance = this;
this.schedule(() => {
i.EventCenter.getInstance().fire(c.GameEventName.HeartBeat);
if (this.heartDown > 0) {
this.heartDown--;
this.addHeart();
}
}, 1, cc.macro.REPEAT_FOREVER, 0);
}
start() {
r.default.loopBgm();
}
addHeart() {
if (l.default.getInstance().getGameData().getHeart() < 10 && this.heartDown <= 0) {
this.heartDown = 1200;
l.default.getInstance().getGameData().addHeart(1);
}
}
onEnterGame(e) {
if (l.default.getInstance().getGameData().reduceHeart(1)) {
this.home.active = !1;
u.default.getInstance().startGame(e);
} else h.default.instance.onShowView(n.UIConfig.AddHeartView, {
type: 1
});
}
onEnterHome() {
this.home.active = !0;
}
};
p.instance = null;
s([ g(cc.Node) ], p.prototype, "home", void 0);
p = a = s([ d ], p);
o.default = p;
cc._RF.pop();
};
