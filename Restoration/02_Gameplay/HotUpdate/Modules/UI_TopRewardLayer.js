// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: UI_TopRewardLayer; dependency map: {"../BaseUIManager/EventListener/EventCenter":"EventCenter","../BaseUIManager/Storage/GameEventConsts":"GameEventConsts","../BaseUIManager/Storage/GameLocalData":"GameLocalData","../BaseUIManager/Storage/NewGamePlayData":"NewGamePlayData","../BaseUIManager/Storage/SoundManager":"SoundManager","../BaseUIManager/Storage/TouchButton":"TouchButton","../BaseUIManager/UIConfig":"UIConfig","../BaseUIManager/UIManagerNew":"UIManagerNew","../LanguageControl/GameManagement":"GameManagement","../LanguageControl/Lab":"Lab","../Report/NativeCall":"NativeCall","./SettingDialog":"SettingDialog"}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "d84d5A/JnZAHrbpYzfXJpGb", "UI_TopRewardLayer");
var i, o = this && this.__decorate || function(e, t, a, i) {
var o, n = arguments.length, s = n < 3 ? t : null === i ? i = Object.getOwnPropertyDescriptor(t, a) : i;
if ("object" == typeof Reflect && "function" == typeof Reflect.decorate) s = Reflect.decorate(e, t, a, i); else for (var r = e.length - 1; r >= 0; r--) (o = e[r]) && (s = (n < 3 ? o(s) : n > 3 ? o(t, a, s) : o(t, a)) || s);
return n > 3 && s && Object.defineProperty(t, a, s), s;
};
Object.defineProperty(a, "__esModule", {
value: !0
});
const n = e("../BaseUIManager/EventListener/EventCenter"), s = e("../BaseUIManager/Storage/GameEventConsts"), r = e("../BaseUIManager/Storage/GameLocalData"), l = e("../BaseUIManager/Storage/NewGamePlayData"), c = e("../BaseUIManager/Storage/SoundManager"), d = e("../BaseUIManager/Storage/TouchButton"), h = e("../BaseUIManager/UIConfig"), u = e("../BaseUIManager/UIManagerNew"), g = e("../CardPlayGame/GameScene"), p = e("../LanguageControl/GameManagement"), f = e("../LanguageControl/Lab"), m = e("../Report/NativeCall"), y = e("./SettingDialog"), {ccclass: _, property: v} = cc._decorator;
let b = i = class extends cc.Component {
constructor() {
super(...arguments);
this.reRealtoast = null;
this.real_label_value = null;
this.node_coin = null;
this.label_usecoinLabel = null;
this.usecoin_red = null;
this.handGuide = null;
this.icon = null;
this.tixianNode = null;
this.tixianBtnLabel = null;
this.redLabel = null;
this.CurrentLevel = null;
this.CurrentLevelCount = null;
this.settingBtn = null;
this.tipCountLable = null;
this.revokeCountLable = null;
this.addmoveCountLable = null;
this.cashTimeNode = null;
this.progressbar = null;
this.progressbarLabel = null;
this.cashtime = null;
this.BianYuanG_TX = null;
this.levelbg = null;
this.levelLabel = null;
}
static getInstance() {
return i.instance;
}
refreshLanguage() {
this.tixianBtnLabel && (this.tixianBtnLabel.string = f.default.getlab("1"));
this.refreshLevel();
}
onLoad() {
i.instance = this;
m.default.checkClientEndingWithCoin() || (i.getInstance().node_coin.active = !1);
const e = r.default.getInstance().getData(l.default);
if (!e.FirstOpenGame) {
e.UseRevenue = p.default.realConfig.real_init_coin;
e.FirstOpenGame = !0;
r.default.getInstance().saveToUserDefault();
}
this.cashTimeNode.active = e.Passlevel > 0;
c.default.loopBgm(c.SoundName.bgm_game);
n.EventCenter.getInstance().register(s.GameEventName.MoneyChangedNtf, this.flush_red_bag, this);
n.EventCenter.getInstance().register(s.GameEventName.PropsNtf, this.flushprops, this);
n.EventCenter.getInstance().register(s.GameEventName.RealWDNtf, this.refreshCoinData, this);
n.EventCenter.getInstance().register(s.GameEventName.RefreshLevel, this.playLevelTips, this);
n.EventCenter.getInstance().register(s.GameEventName.RefreshLanguage, this.refreshLanguage, this);
this.tixianBtnLabel.node.parent.addComponent(d.default).registerTouchEvent(() => {
const e = {
ui_config_path: h.default.GameRealWDDialog,
ui_config_name: "GameRealWDDialog",
param: {
EnterTouch: !0
}
};
u.default.show_ui(e);
g.default.getInstance().GameOver = !0;
});
this.refreshLanguage();
this.flush_red_bag();
this.flushprops();
this.refreshCoinData();
this.refreshLevel();
this.settingBtn.addComponent(d.default).registerTouchEvent(() => {
y.default.open();
});
this.node_coin.addComponent(d.default).registerTouchEvent(() => {
g.default.getInstance().GameOver = !0;
const e = {
ui_config_path: h.default.GameRealWDDialog,
ui_config_name: "GameRealWDDialog",
param: {
EnterTouch: !1
}
};
u.default.show_ui(e);
});
i.getInstance().refreshProgressData(0, p.default.getCashTimeConfig()[2]);
}
flush_red_bag() {
this.redLabel.node.stopAllActions();
this.redLabel.node.runAction(cc.sequence([ cc.scaleTo(.2, 1.1), cc.scaleTo(.1, 1) ]));
let e = r.default.getInstance().getData(l.default).red_bag;
this.redLabel && (this.redLabel.string = `${p.default.getmonstr(e)}`);
}
flushprops() {
const e = r.default.getInstance().getData(l.default);
let t = e.props_addSwimRing_count > 0 ? e.props_addSwimRing_count : "AD", a = e.props_refresh_count > 0 ? e.props_refresh_count : "AD", i = e.props_superTurret_count > 0 ? e.props_superTurret_count : "AD";
if (this.tipCountLable && this.revokeCountLable && this.addmoveCountLable) {
this.tipCountLable.string = `${t}`;
this.revokeCountLable.string = `${a}`;
this.addmoveCountLable.string = `${i}`;
}
this.tipCountLable.node.parent.active = e.guideaddSwimRing;
this.revokeCountLable.node.parent.active = e.guideRefresh;
this.addmoveCountLable.node.parent.active = e.guiderSuperturret;
}
refreshLevel() {
const e = r.default.getInstance().getData(l.default);
if (this.CurrentLevel && this.CurrentLevelCount && this.levelLabel) {
this.CurrentLevel.string = f.default.getlab("2");
this.CurrentLevelCount.string = `${e.Passlevel + 1}`;
this.levelLabel.string = `${f.default.getlab("2")}  ${e.Passlevel + 1}`;
}
}
refreshCoinData() {
this.label_usecoinLabel.node.stopAllActions();
this.label_usecoinLabel.node.runAction(cc.sequence([ cc.scaleTo(.2, 1.1), cc.scaleTo(.1, 1) ]));
const e = r.default.getInstance().getData(l.default);
this.label_usecoinLabel && (this.label_usecoinLabel.string = p.default.getPlayerUseCoin(e.UseRevenue).toString());
let t = !1, a = e.Passlevel + 1, i = p.default.real_products;
for (let o = 0; o < i.length; o++) {
let n = e.real_watch_video_Singlecount[o], s = a, r = e.watch_video_count, l = e.watch_video_count;
if (t = e.UseRevenue >= i[o].condition_coin && s >= i[o].condition_2 && n >= i[o].condition_3 && r >= i[o].condition_4 && l >= i[o].condition_5 && !p.default.getPlayDataWithdrawRecords().withdrawRecords[o]) break;
}
this.usecoin_red.active = t;
}
playRealCoinAnimation() {
this.reRealtoast.active = !0;
this.reRealtoast.scale = 0;
this.reRealtoast.opacity = 255;
this.reRealtoast.position = cc.v3(0, 0, 0);
let e = r.default.getInstance().getData(l.default);
this.real_label_value && (this.real_label_value.string = " + " + e.addVodeoshowCoin.toString());
cc.tween(this.reRealtoast).to(.3, {
scale: 1
}).delay(.1).parallel(cc.tween().delay(.9).to(.1, {
opacity: 0
}), cc.tween().by(1, {
y: 70
})).by(1, {
y: 70
}).call(() => {
this.reRealtoast.active = !1;
}).start();
this.refreshCoinData();
}
refreshTopUI(e = !0) {
this.tixianBtnLabel.node.parent.active = e;
this.node_coin.active = e;
g.default.getInstance().refreshPropsBtn(e);
}
refreshProgressData(e, t) {
const a = e / t;
this.progressbar.progress = a;
this.progressbarLabel && (this.progressbarLabel.string = e + "/" + t);
this.progressbar.node.scale = 1;
0 == e && (this.BianYuanG_TX.node.active = !1);
a >= .9 && this.startScaleAnimation();
}
startScaleAnimation() {
if (this.progressbar) {
const e = 1;
let t = cc.tween(this.progressbar.node).to(.2, {
scale: 1.1 * e
}, {
easing: "quadOut"
}).to(.2, {
scale: e
}, {
easing: "quadIn"
});
cc.tween(this.progressbar.node).repeat(3, t).start();
}
}
checkCompleteProgress(e) {
this.playCashTime();
this.BianYuanG_TX.node.active = !0;
this.BianYuanG_TX.setAnimation(0, "idle1", !0);
}
refreshStartCashTimeProgress(e, t) {
if (1 == t) {
const t = e[2];
cc.tween(this.progressbar).to(e[1], {
progress: 0
}, {
easing: "quadOut",
onUpdate: (e, a) => {
const i = Math.ceil(t * (1 - a));
this.progressbarLabel && (this.progressbarLabel.string = i + "/" + t);
}
}).start();
}
}
refreshCashTimeProgress(e, t) {
cc.Tween.stopAllByTarget(this.progressbar);
this.progressbarLabel && (this.progressbarLabel.string = t + "/" + e[2]);
this.progressbar.progress = 0;
0 == t && (this.BianYuanG_TX.node.active = !1);
}
playCashTime() {
this.cashtime.parent.active = !0;
this.cashtime.scale = 1;
this.cashtime.setPosition(cc.v3(0, 1e3, 0));
let e = cc.v3(this.cashtime.x, 200);
cc.tween(this.cashtime).to(1, {
position: e
}, {
easing: "quadOut"
}).delay(1).to(.5, {
scale: 0
}, {
easing: "quadOut"
}).call(() => {
this.cashtime.parent.active = !1;
}).start();
}
playLevelTips() {
console.log("generateBatteries GameScene.getInstance().GuideManager.currentStepIndex ==    ", g.default.getInstance().GuideManager.currentStepIndex);
if (g.default.getInstance().GuideManager && 8 == g.default.getInstance().GuideManager.currentStepIndex) return;
const e = r.default.getInstance().getData(l.default);
this.CurrentLevel && (this.CurrentLevel.string = f.default.getlab("2"));
this.cashTimeNode.active = e.Passlevel > 0;
this.CurrentLevelCount && (this.CurrentLevelCount.string = `${e.Passlevel + 1}`);
this.levelLabel && (this.levelLabel.string = `${f.default.getlab("2")}  ${e.Passlevel + 1}`);
this.levelbg.parent.active = !0;
this.levelbg.setPosition(cc.v3(0, 150, 0));
let t = cc.v3(this.levelbg.x, this.levelbg.y + 200);
this.canTouchshow();
cc.tween(this.levelbg).to(1, {
position: t,
scale: 1
}, {
easing: "quadOut"
}).delay(.5).to(.2, {
scale: 0
}, {
easing: "quadOut"
}).call(() => {
this.levelbg.parent.active = !1;
if (e.Passlevel + 1 != 3 || e.guideaddSwimRing) if (e.Passlevel + 1 != 5 || e.guideRefresh) {
if (e.Passlevel + 1 == 7 && !e.guiderSuperturret) {
g.default.getInstance().locksuperTurret.active = !1;
g.default.getInstance().superTurretBtnBG.active = !0;
g.default.getInstance().GuideManager.GuidePropsDialog("80", "81");
e.guiderSuperturret = !0;
e.props_superTurret_count += 1;
}
} else {
g.default.getInstance().lockrefresh.active = !1;
g.default.getInstance().refreshBtnBG.active = !0;
g.default.getInstance().GuideManager.GuidePropsDialog("78", "79");
e.guideRefresh = !0;
e.props_refresh_count += 1;
} else {
g.default.getInstance().lockAddSwimRing.active = !1;
g.default.getInstance().addSwimRingBtnBG.active = !0;
g.default.getInstance().GuideManager.GuidePropsDialog("82", "83");
e.guideaddSwimRing = !0;
e.props_addSwimRing_count += 1;
}
r.default.getInstance().saveToUserDefault();
this.flushprops();
}).start();
}
Guideshow(e = !1, t) {
this.tixianBtnLabel.node.parent.getComponent(d.default).canTouch = 3 == t || 0 == t;
this.node_coin.getComponent(d.default).canTouch = e || 8 == t;
this.settingBtn.getComponent(d.default).canTouch = e || 8 == t;
g.default.getInstance().addSwimRingBtn.getComponent(d.default).canTouch = 5 == t || 0 == t || 8 == t;
g.default.getInstance().refreshBtn.getComponent(d.default).canTouch = 6 == t || 0 == t || 8 == t;
g.default.getInstance().superTurretBtn.getComponent(d.default).canTouch = 7 == t || 0 == t || 8 == t;
}
canTouchshow() {
this.tixianBtnLabel.node.parent.getComponent(d.default).canTouch = !0;
this.node_coin.getComponent(d.default).canTouch = !0;
this.settingBtn.getComponent(d.default).canTouch = !0;
g.default.getInstance().addSwimRingBtn.getComponent(d.default).canTouch = !0;
g.default.getInstance().refreshBtn.getComponent(d.default).canTouch = !0;
g.default.getInstance().superTurretBtn.getComponent(d.default).canTouch = !0;
}
};
b.instance = null;
b.touchRestartCount = 0;
o([ v(cc.Node) ], b.prototype, "reRealtoast", void 0);
o([ v(cc.Label) ], b.prototype, "real_label_value", void 0);
o([ v(cc.Node) ], b.prototype, "node_coin", void 0);
o([ v(cc.Label) ], b.prototype, "label_usecoinLabel", void 0);
o([ v(cc.Node) ], b.prototype, "usecoin_red", void 0);
o([ v(cc.Node) ], b.prototype, "handGuide", void 0);
o([ v(cc.Node) ], b.prototype, "icon", void 0);
o([ v(cc.Node) ], b.prototype, "tixianNode", void 0);
o([ v(cc.Label) ], b.prototype, "tixianBtnLabel", void 0);
o([ v(cc.Label) ], b.prototype, "redLabel", void 0);
o([ v(cc.Label) ], b.prototype, "CurrentLevel", void 0);
o([ v(cc.Label) ], b.prototype, "CurrentLevelCount", void 0);
o([ v(cc.Node) ], b.prototype, "settingBtn", void 0);
o([ v(cc.Label) ], b.prototype, "tipCountLable", void 0);
o([ v(cc.Label) ], b.prototype, "revokeCountLable", void 0);
o([ v(cc.Label) ], b.prototype, "addmoveCountLable", void 0);
o([ v(cc.Node) ], b.prototype, "cashTimeNode", void 0);
o([ v(cc.ProgressBar) ], b.prototype, "progressbar", void 0);
o([ v(cc.Label) ], b.prototype, "progressbarLabel", void 0);
o([ v(cc.Node) ], b.prototype, "cashtime", void 0);
o([ v(sp.Skeleton) ], b.prototype, "BianYuanG_TX", void 0);
o([ v(cc.Node) ], b.prototype, "levelbg", void 0);
o([ v(cc.Label) ], b.prototype, "levelLabel", void 0);
b = i = o([ _ ], b);
a.default = b;
cc._RF.pop();
};
