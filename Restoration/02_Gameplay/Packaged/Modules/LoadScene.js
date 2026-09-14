// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/base/assets/assets/main/index.ba75b.js
// Module: LoadScene; dependency map: {"./ATools/BaseUI":"BaseUI","./ATools/EventCenter":"EventCenter","./ATools/GameEventConsts":"GameEventConsts","./ATools/LoadAllResources":"LoadAllResources","./ATools/LocalDataManager":"LocalDataManager","./ATools/UIManager":"UIManager","./Game/BlockUtils":"BlockUtils"}
module.exports = function(e, t, o) {
"use strict";
cc._RF.push(t, "e1b90/rohdEk4SdmmEZANaD", "LoadScene");
var a = this && this.__decorate || function(e, t, o, a) {
var s, n = arguments.length, i = n < 3 ? t : null === a ? a = Object.getOwnPropertyDescriptor(t, o) : a;
if ("object" == typeof Reflect && "function" == typeof Reflect.decorate) i = Reflect.decorate(e, t, o, a); else for (var c = e.length - 1; c >= 0; c--) (s = e[c]) && (i = (n < 3 ? s(i) : n > 3 ? s(t, o, i) : s(t, o)) || i);
return n > 3 && i && Object.defineProperty(t, o, i), i;
};
Object.defineProperty(o, "__esModule", {
value: !0
});
const s = e("./ATools/BaseUI"), n = e("./ATools/EventCenter"), i = e("./ATools/GameEventConsts"), c = e("./ATools/LoadAllResources"), l = e("./ATools/LocalDataManager"), r = e("./ATools/UIManager"), h = e("./Game/BlockUtils"), {ccclass: u, property: d} = cc._decorator;
let g = class extends cc.Component {
constructor() {
super(...arguments);
this.pgb = null;
this.loadDuration = 2;
this.progress = 0;
this.isLoadingGameScene = !1;
this.isStartLoading = !1;
}
onLoad() {
n.EventCenter.getInstance().register(i.GameEventName.AgreeMent, this.startLoading, this);
l.default.getInstance().initConfig();
l.default.getInstance().loadOrRequlestStorageData(() => {});
c.default.getInstance().loadAllRes(() => {});
h.default.init();
this.isStartLoading = !1;
this.progress = 0;
this.isLoadingGameScene = !1;
this.updateProgressBar();
this.showUserView();
}
start() {}
update(e) {
if (this.isStartLoading && !this.isLoadingGameScene) {
this.progress += e / Math.max(this.loadDuration, .01);
if (this.progress >= 1) {
this.progress = 1;
this.updateProgressBar();
this.loadGameScene();
} else this.updateProgressBar();
}
}
updateProgressBar() {
if (this.pgb) {
this.pgb.type = cc.Sprite.Type.FILLED;
this.pgb.fillType = cc.Sprite.FillType.HORIZONTAL;
this.pgb.fillStart = 0;
this.pgb.fillRange = this.progress;
}
}
loadGameScene() {
this.isLoadingGameScene = !0;
cc.director.loadScene("GameScene");
}
showUserView() {
l.default.getInstance().getGameData().getIsNewUser() ? r.default.instance.onShowView(s.UIConfig.GuideView) : this.isStartLoading = !0;
}
startLoading() {
this.isStartLoading = !0;
}
};
a([ d(cc.Sprite) ], g.prototype, "pgb", void 0);
a([ d ], g.prototype, "loadDuration", void 0);
g = a([ u ], g);
o.default = g;
cc._RF.pop();
};
