// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: BaoXiangFly; dependency map: {"../BaseUIManager/FlyAnimation":"FlyAnimation","../BaseUIManager/Storage/GameLocalData":"GameLocalData","../BaseUIManager/Storage/NewGamePlayData":"NewGamePlayData","../BaseUIManager/Storage/TouchButton":"TouchButton","../HWL/HWL_TStool":"HWL_TStool","../LanguageControl/GameManagement":"GameManagement"}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "12f07SCnXhGUbN8cfcihv0u", "BaoXiangFly");
var i, o = this && this.__decorate || function(e, t, a, i) {
var o, n = arguments.length, s = n < 3 ? t : null === i ? i = Object.getOwnPropertyDescriptor(t, a) : i;
if ("object" == typeof Reflect && "function" == typeof Reflect.decorate) s = Reflect.decorate(e, t, a, i); else for (var r = e.length - 1; r >= 0; r--) (o = e[r]) && (s = (n < 3 ? o(s) : n > 3 ? o(t, a, s) : o(t, a)) || s);
return n > 3 && s && Object.defineProperty(t, a, s), s;
};
Object.defineProperty(a, "__esModule", {
value: !0
});
const n = e("../BaseUIManager/Storage/GameLocalData"), s = e("../BaseUIManager/Storage/NewGamePlayData"), r = e("../BaseUIManager/Storage/TouchButton"), l = e("../LanguageControl/GameManagement"), c = e("../HWL/HWL_TStool"), d = e("../BaseUIManager/FlyAnimation"), {ccclass: h, property: u} = cc._decorator;
let g = i = class extends cc.Component {
constructor() {
super(...arguments);
this.baoxiangNode = null;
}
onLoad() {
i.Instance = this;
this.baoxiangNode.zIndex = 2;
this.initBaoxiang();
}
static getInstance() {
return i.Instance;
}
initBaoxiang() {
this.baoxiangNode.active = !1;
this.refreshBaoxiang();
this.baoxiangNode.addComponent(r.default).registerTouchEvent(() => {
this.stopBubbleMovement();
this.schedule(this.refreshBaoxiang, 120);
c.HWLshowAd("4_A", e => {
let t = l.default.getnor(1), a = l.default.cashCoin(e);
d.default.instance.ShowCoinEffect(t, this.node);
d.default.instance.ShowRealCoinEffect(a, this.node);
}, () => {});
});
}
refreshBaoxiang() {
n.default.getInstance().getData(s.default).Passlevel >= 1 && (this.baoxiangNode.active = !0);
this.startBaoxiangMoveAnimation();
}
stopBubbleMovement() {
this.baoxiangNode.stopAllActions();
this.baoxiangNode.active = !1;
}
startBaoxiangMoveAnimation() {
const e = [ cc.v2(100, 300), cc.v2(900, -300), cc.v2(-500, -800) ];
this.baoxiangNode.setPosition(-cc.winSize.width / 2, 900);
this.baoxiangNode.runAction(cc.sequence(cc.bezierTo(30, e), cc.callFunc(() => {
this.stopBubbleMovement();
this.schedule(this.refreshBaoxiang, 120);
})));
}
onDisable() {
this.unschedule(this.refreshBaoxiang);
}
};
g.Instance = null;
o([ u(cc.Node) ], g.prototype, "baoxiangNode", void 0);
g = i = o([ h ], g);
a.default = g;
cc._RF.pop();
};
