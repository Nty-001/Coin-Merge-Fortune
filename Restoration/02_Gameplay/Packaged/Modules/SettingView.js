// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/base/assets/assets/main/index.ba75b.js
// Module: SettingView; dependency map: {"../ATools/BaseUI":"BaseUI","../ATools/LocalDataManager":"LocalDataManager","../ATools/SoundManager":"SoundManager","../ATools/TouchButton":"TouchButton","../ATools/UIManager":"UIManager"}
module.exports = function(e, t, o) {
"use strict";
cc._RF.push(t, "f0005Qb+LhLNI6PLGn4oqcb", "SettingView");
var a = this && this.__decorate || function(e, t, o, a) {
var s, n = arguments.length, i = n < 3 ? t : null === a ? a = Object.getOwnPropertyDescriptor(t, o) : a;
if ("object" == typeof Reflect && "function" == typeof Reflect.decorate) i = Reflect.decorate(e, t, o, a); else for (var c = e.length - 1; c >= 0; c--) (s = e[c]) && (i = (n < 3 ? s(i) : n > 3 ? s(t, o, i) : s(t, o)) || i);
return n > 3 && i && Object.defineProperty(t, o, i), i;
};
Object.defineProperty(o, "__esModule", {
value: !0
});
const s = e("../ATools/BaseUI"), n = e("../ATools/LocalDataManager"), i = e("../ATools/SoundManager"), c = e("../ATools/TouchButton"), l = e("../ATools/UIManager"), {ccclass: r, property: h} = cc._decorator;
let u = class extends s.default {
constructor() {
super(...arguments);
this.btn_close = null;
this.btn_privacy = null;
this.btn_agree = null;
this.btn_music = null;
this.dian = null;
}
onLoad() {
this.registerEvent();
}
start() {}
onShow(e) {
const t = n.default.getInstance().getGameData().getMusicStatus();
this.dian.x = t ? 16 : -16;
this.btn_music.color = t ? new cc.Color(255, 255, 255, 255) : new cc.Color(79, 83, 80, 255);
}
registerEvent() {
this.btn_close.addComponent(c.default).registerTouchEvent(() => {
this.onClose();
});
this.btn_agree.addComponent(c.default).registerTouchEvent(() => {
l.default.instance.onShowView(s.UIConfig.PolicyView, {
type: 2
});
});
this.btn_privacy.addComponent(c.default).registerTouchEvent(() => {
l.default.instance.onShowView(s.UIConfig.PolicyView, {
type: 1
});
});
this.btn_music.addComponent(c.default).registerTouchEvent(() => {
let e = n.default.getInstance().getGameData().getMusicStatus();
n.default.getInstance().getGameData().setMusicStatus(!e);
e = !e;
this.dian.x = e ? 16 : -16;
if (e) {
i.default.loopBgm();
this.btn_music.color = new cc.Color(255, 255, 255, 255);
} else {
i.default.stopMusic(i.default.bgMusicId);
this.btn_music.color = new cc.Color(79, 83, 80, 255);
}
});
}
};
a([ h(cc.Node) ], u.prototype, "btn_close", void 0);
a([ h(cc.Node) ], u.prototype, "btn_privacy", void 0);
a([ h(cc.Node) ], u.prototype, "btn_agree", void 0);
a([ h(cc.Node) ], u.prototype, "btn_music", void 0);
a([ h(cc.Node) ], u.prototype, "dian", void 0);
u = a([ s.registerUIPath("prefab/SettingView"), r ], u);
o.default = u;
cc._RF.pop();
};
