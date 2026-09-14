// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: SettingDialog; dependency map: {"../BaseUIManager/BaseUI":"BaseUI","../BaseUIManager/Storage/GameLocalData":"GameLocalData","../BaseUIManager/Storage/PlayData":"PlayData","../BaseUIManager/Storage/SoundManager":"SoundManager","../BaseUIManager/Storage/TouchButton":"TouchButton","../BaseUIManager/UIConfig":"UIConfig","../BaseUIManager/UIManagerNew":"UIManagerNew","../LanguageControl/Lab":"Lab"}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "b9246PTLYdOu6sfrLjwvVTL", "SettingDialog");
var i = this && this.__decorate || function(e, t, a, i) {
var o, n = arguments.length, s = n < 3 ? t : null === i ? i = Object.getOwnPropertyDescriptor(t, a) : i;
if ("object" == typeof Reflect && "function" == typeof Reflect.decorate) s = Reflect.decorate(e, t, a, i); else for (var r = e.length - 1; r >= 0; r--) (o = e[r]) && (s = (n < 3 ? o(s) : n > 3 ? o(t, a, s) : o(t, a)) || s);
return n > 3 && s && Object.defineProperty(t, a, s), s;
};
Object.defineProperty(a, "__esModule", {
value: !0
});
const o = e("../BaseUIManager/BaseUI"), n = e("../BaseUIManager/Storage/GameLocalData"), s = e("../BaseUIManager/Storage/PlayData"), r = e("../BaseUIManager/Storage/SoundManager"), l = e("../BaseUIManager/Storage/TouchButton"), c = e("../BaseUIManager/UIConfig"), d = e("../BaseUIManager/UIManagerNew"), h = e("../LanguageControl/Lab"), {ccclass: u, property: g} = cc._decorator;
let p = class extends o.default {
constructor() {
super(...arguments);
this.btn_close = null;
this.title = null;
this.btn_bgm_off = null;
this.btn_bgm_on = null;
this.btn_vibrate_on = null;
this.btn_vibrate_off = null;
this.btn_privacy = null;
this.btn_agreement = null;
this.musicLabel = null;
this.vibrateLabel = null;
}
get newplayData() {
return n.default.getInstance().getData(s.default);
}
onLoad() {
super.onLoad();
this.btn_close.addComponent(l.default).registerTouchEvent(() => {
this.on_close_call();
});
[ this.btn_bgm_off, this.btn_bgm_on ].forEach(e => {
e.addComponent(l.default).registerTouchEvent(() => {
this.onBgmBtn();
});
});
[ this.btn_vibrate_on, this.btn_vibrate_off ].forEach(e => {
e.addComponent(l.default).registerTouchEvent(() => {
this.onVibrateBtn();
});
});
this.btn_privacy.addComponent(l.default).registerTouchEvent(() => {
const e = {
ui_config_path: c.default.PrivacyPolicyView,
ui_config_name: "PrivacyPolicyView",
param: {
icon_idx: 0
}
};
d.default.show_ui(e);
});
this.btn_agreement.addComponent(l.default).registerTouchEvent(() => {
const e = {
ui_config_path: c.default.PrivacyPolicyView,
ui_config_name: "PrivacyPolicyView",
param: {
icon_idx: 1
}
};
d.default.show_ui(e);
});
}
show(e) {
super.show(e);
this.title.string = h.default.getlab("3");
this.btn_agreement.getChildByName("label").getComponent(cc.Label).string = h.default.getlab("6");
this.btn_privacy.getChildByName("label").getComponent(cc.Label).string = h.default.getlab("7");
this.musicLabel.string = h.default.getlab("4");
this.vibrateLabel.string = h.default.getlab("5");
e.param;
this.refreshMusicUI();
this.refreshBgmUI();
this.refreshVibrateUI();
}
onMusicBtn() {
const e = this.newplayData.open_music;
this.resetMusicStatus(!e);
}
resetMusicStatus(e) {
if (this.newplayData.open_music != e) {
this.newplayData.open_music = e;
n.default.getInstance().saveToUserDefault();
r.default.playClickSound();
this.refreshMusicUI();
}
}
refreshMusicUI() {
this.newplayData.open_music;
}
onBgmBtn() {
const e = this.newplayData.open_bgm;
this.resetBgmStatus(!e);
}
resetBgmStatus(e) {
if (this.newplayData.open_bgm != e || this.newplayData.open_music != e) {
this.newplayData.open_bgm = e;
this.newplayData.open_music = e;
n.default.getInstance().saveToUserDefault();
if (e) {
r.default.playClickSound();
r.default.loopBgm(r.SoundName.bgm_game);
} else {
r.default.playClickSound();
r.default.stopMusic(r.default.bgMusicId);
}
this.refreshBgmUI();
}
}
refreshBgmUI() {
let e = this.newplayData.open_bgm;
this.btn_bgm_off.active = !e;
this.btn_bgm_on.active = e;
}
onVibrateBtn() {
this.newplayData.open_vibrate = !this.newplayData.open_vibrate;
n.default.getInstance().saveToUserDefault();
this.refreshVibrateUI();
}
refreshVibrateUI() {
let e = this.newplayData.open_vibrate;
this.btn_vibrate_off.active = !e;
this.btn_vibrate_on.active = e;
}
};
i([ g(cc.Node) ], p.prototype, "btn_close", void 0);
i([ g(cc.Label) ], p.prototype, "title", void 0);
i([ g(cc.Node) ], p.prototype, "btn_bgm_off", void 0);
i([ g(cc.Node) ], p.prototype, "btn_bgm_on", void 0);
i([ g(cc.Node) ], p.prototype, "btn_vibrate_on", void 0);
i([ g(cc.Node) ], p.prototype, "btn_vibrate_off", void 0);
i([ g(cc.Node) ], p.prototype, "btn_privacy", void 0);
i([ g(cc.Node) ], p.prototype, "btn_agreement", void 0);
i([ g(cc.Label) ], p.prototype, "musicLabel", void 0);
i([ g(cc.Label) ], p.prototype, "vibrateLabel", void 0);
p = i([ o.registerUIPath("GameDialog/SettingDialog"), u ], p);
a.default = p;
cc._RF.pop();
};
