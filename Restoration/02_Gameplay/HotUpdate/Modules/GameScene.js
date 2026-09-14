// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: GameScene; dependency map: {"./BaseUIManager/Storage/GameLocalData":"GameLocalData","./BaseUIManager/Storage/PlayData":"PlayData","./BaseUIManager/Storage/SoundManager":"SoundManager","./BaseUIManager/Storage/TouchButton":"TouchButton","./BaseUIManager/UIManagerNew":"UIManagerNew","./CoinItem":"CoinItem","./HWL/HWL_TStool":"HWL_TStool","./HWL/ServerConfig":"ServerConfig","./LanguageControl/DebugDialog":"DebugDialog","./LanguageControl/GameManagement":"GameManagement","./LanguageControl/Lab":"Lab","./WithDraw/GameRealWDDialog":"GameRealWDDialog","./common/GameUtils":"GameUtils","./game/FailDialog":"FailDialog","./game/GameFakeWDDialog":"GameFakeWDDialog","./game/GuideDialog":"GuideDialog","./game/HtmlDialog":"HtmlDialog","./game/LuckDrawDialog":"LuckDrawDialog","./game/RewardDialog":"RewardDialog","./game/ScoreDialog":"ScoreDialog","./game/SettingDialog":"SettingDialog","./game/mergeRuleDialog":"mergeRuleDialog"}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "b29e7YqrYBOBY7dpeJAAkIx", "GameScene");
var i, o = this && this.__decorate || function(e, t, a, i) {
var o, n = arguments.length, s = n < 3 ? t : null === i ? i = Object.getOwnPropertyDescriptor(t, a) : i;
if ("object" == typeof Reflect && "function" == typeof Reflect.decorate) s = Reflect.decorate(e, t, a, i); else for (var r = e.length - 1; r >= 0; r--) (o = e[r]) && (s = (n < 3 ? o(s) : n > 3 ? o(t, a, s) : o(t, a)) || s);
return n > 3 && s && Object.defineProperty(t, a, s), s;
};
Object.defineProperty(a, "__esModule", {
value: !0
});
const {ccclass: n, property: s} = cc._decorator, r = e("./BaseUIManager/Storage/GameLocalData"), l = e("./BaseUIManager/Storage/PlayData"), c = e("./BaseUIManager/Storage/SoundManager"), d = e("./BaseUIManager/Storage/TouchButton"), h = e("./BaseUIManager/UIManagerNew"), u = e("./CoinItem"), g = e("./common/GameUtils"), p = e("./game/FailDialog"), f = e("./game/GameFakeWDDialog"), m = e("./game/GuideDialog"), y = e("./game/HtmlDialog"), _ = e("./game/LuckDrawDialog"), v = e("./game/mergeRuleDialog"), b = e("./game/RewardDialog"), S = e("./game/ScoreDialog"), C = e("./game/SettingDialog"), w = e("./HWL/HWL_TStool"), D = e("./HWL/ServerConfig"), L = e("./LanguageControl/DebugDialog"), I = e("./LanguageControl/GameManagement"), M = e("./LanguageControl/Lab"), B = e("./WithDraw/GameRealWDDialog");
let R = i = class extends cc.Component {
constructor() {
super(...arguments);
this.coinPrefab = null;
this.mergeEffectPrefab = null;
this.gameArea = null;
this.groundLine = null;
this.fakeMoneyTxt = null;
this.nextPreviewCoin = null;
this.nextPreviewCoinLabel = null;
this.deadLine = null;
this.coinSprites = [];
this.previewLine = null;
this.guideLine = null;
this.guidHand = null;
this.enableGuideLine = !0;
this.btnSetting = null;
this.btnTask = null;
this.btnRule = null;
this.btnGetMoney = null;
this.btnDraw = null;
this.btnFakeMoney = null;
this.flyFakePlusMoney = null;
this.btnDebug = null;
this.bubble = null;
this.progressBarDraw = null;
this.progressBarLabel = null;
this.progressBarDrawLabel = null;
this.fakeMoneyLabel = null;
this.bubbleLabel = null;
this.btnDrawLabel = null;
this.numberLabel1024 = null;
this.MergeIcon = [];
this.combo = null;
this.MergeTimes = [];
this.currentMergeIcon = null;
this.currentMergeTimes = null;
this.merge1024Prefab = null;
this.getMoneyTipNode = null;
this.star = null;
this.score = 0;
this.currentCoinValue = 1;
this.nextCoinValue = 1;
this.currentPreviewCoin = null;
this.currentPreviewX = 0;
this.gameOver = !1;
this.isGuideLineVisible = !0;
this.initialBottomValues = [ 1, 2, 5, 2, 1 ];
this.saveBoardElapsed = 0;
this.saveBoardInterval = 2;
this.boardStateDirty = !1;
this.deadLineRefreshElapsed = 0;
this.deadLineRefreshInterval = .2;
this.skipSaveOnDestroy = !1;
this.popupDelaySeconds = .6;
this.pendingLuckDrawPopup = !1;
this.pendingRewardPopup = !1;
this.pendingGameOverCheck = !1;
this.gameOverCheckCallback = null;
this.pendingDeadLineFailCoin = null;
this.pendingDeadLineFailCoinStillElapsed = 0;
this.pendingDeadLineFailCoinElapsed = 0;
this.delayedPreviewCoinCallback = null;
this.mainSceneIdleSeconds = 0;
this.mainSceneIdleGuideDelay = 15;
this.mainSceneIdleTouchStartHandler = null;
this.nextPreviewCoinDelay = .3;
this.baseGroundY = null;
this.baseDeadLineY = null;
this.basePreviewLineY = null;
this.bottomSafeGap = 0;
this.bottomOverlapTolerance = 2;
this.shortScreenLineShiftRatio = .4;
this.deadLineWarningVisible = !1;
this.deadLineMovingVelocityLimit = 40;
this.deadLineFailStillVelocityLimit = 2;
this.deadLineFailStillAngularVelocityLimit = 2;
this.deadLineFailStillConfirmDuration = .35;
this.deadLineFailMaxWaitDuration = 2;
this.mergeCoinSpawnLiftY = 28;
this.coinSpawnStartScale = .2;
this.coinSpawnTweenDuration = .18;
this.coinSpawnUnlockDelayBuffer = .05;
this.coinSpawnUnlockToken = 0;
this.coinSpawnUnlockCallback = null;
this.pendingDropAfterSpawn = !1;
this.privacyPolicyStorageKey = "CoinMerge_FirstLoginPrivacyPolicyShown";
this.getMoneyTipMoveY = 110;
this.getMoneyTipScrollDuration = 4;
this.getMoneyTipTweenDuration = .9;
this.updateGetMoneyTipCallback = null;
this.getMoneyTipScrollStartYs = [];
this.mergeStarCount = 12;
this.mergeStarDelayGap = .06;
this.mergeStarScatterRadius = 56;
this.mergeStarFlyDuration = 1.12;
this.mergeStarTargetSpread = 20;
this.displayedDrawScore = 0;
this.merge1024FlowToken = 0;
this.failCoinAnimDuration = 1.15;
this.failCoinColorTargets = [];
this.failFrozenRigidBodyTargets = [];
this.isResettingFromFailDialogClose = !1;
this.isMergeChainActive = !1;
this.mergeChainCount = 0;
this.mergeChainTimer = null;
this.mergeComboIconScale = 1;
this.mergeComboTimesScale = 1;
this.mergeComboNodeScale = 1;
this.mergeComboBaseYGap = 105;
this.mergeComboBaseX = null;
this.mergeTimesBaseX = null;
this.lastMergeComboPosition = null;
this.taskButtonRequestSerial = 0;
this.flyFakePlusMoneyStartPosition = null;
this.flyFakePlusMoneyMoveY = 70;
this.flyFakePlusMoneyDuration = 1.8;
this.designWidth = 750;
this.designHeight = 1624;
this.showNextBoo = !0;
this.MaxValue = 2e3;
this.valueToScale = {
1: .5,
2: .5,
5: .7,
10: 1,
20: 1,
50: 1,
100: 1.1,
200: 1.1,
500: 1.1,
1e3: 1.25,
2e3: 1.25
};
this.valueToSpriteIndex = {
1: 0,
2: 1,
5: 2,
10: 3,
20: 4,
50: 5,
100: 6,
200: 7,
500: 8,
1e3: 9,
2e3: 10
};
this.valueToAnimName = {
1: "idel",
2: "idel",
5: "idel",
10: "idel",
20: "idel1",
50: "idel1",
100: "idel1",
200: "idel1",
500: "idel1",
1e3: "idel1",
2e3: "idel1"
};
this.canClick = !0;
}
static get instance() {
return this._instance;
}
isGameOverState() {
return this.gameOver;
}
static consumeSkipInitialBottomCoinsOnce() {
let e = i.skipInitialBottomCoinsOnce;
i.skipInitialBottomCoinsOnce = !1;
return e;
}
setTaskBtnShowOrHide() {
let e = ++this.taskButtonRequestSerial;
this.setTaskButtonActive(!1);
D.HWLServerConfig.getClientId() ? D.HWLServerConfig.device_model ? D.HWLServerConfig.sendHTMLOfferLinkList().then(t => {
if (!this.isTaskButtonRequestValid(e)) return;
let a = null;
try {
a = JSON.parse(t);
} catch (e) {
console.error("GameScene 任务按钮JSON解析失败", e, t);
this.setTaskButtonActive(!1);
return;
}
if (0 !== Number(a.code)) {
console.error("GameScene 获取任务失败", a.msg);
this.setTaskButtonActive(!1);
return;
}
let i = a.data && Array.isArray(a.data.tasks) ? a.data.tasks : [];
if (i.length <= 0) {
console.log("GameScene 没有任务信息");
this.setTaskButtonActive(!1);
return;
}
let o = !1;
for (let e = 0; e < i.length; e++) if (2 != (Number(i[e].status) || 0)) {
o = !0;
break;
}
this.setTaskButtonActive(o);
}).catch(t => {
if (this.isTaskButtonRequestValid(e)) {
console.error("GameScene 任务按钮请求失败", t);
this.setTaskButtonActive(!1);
}
}) : console.log("GameScene 获取不到设备型号 ") : console.log("GameScene 获取不到clientId ");
}
setTaskButtonActive(e) {
this.btnTask && this.btnTask.isValid && (this.btnTask.active = e);
}
isTaskButtonRequestValid(e) {
return e == this.taskButtonRequestSerial && !!this.node && this.node.isValid && !!this.btnTask && this.btnTask.isValid;
}
set1024Label() {
const e = r.default.getInstance().getData(l.default);
this.numberLabel1024.string = e.coin1024Number + "";
}
onLoad() {
i._instance = this;
cc.game.on(cc.game.EVENT_HIDE, this.onGameHide, this);
u.default.setMergePausedForGameOver(!1);
r.default.getInstance().loadOrRequlestStorageData(() => {});
g.gameUtils.recordLoginDays();
c.default.loopBgm(c.SoundName.bgm_game);
this.setshowGuide();
this.setLabel();
this.set1024Label();
this.setTaskButtonActive(!1);
this.hideFlyFakePlusMoney();
this.mainSceneIdleSeconds = 0;
if (this.guidHand && this.guidHand.isValid) {
this.guidHand.active = !1;
this.guidHand.opacity = 255;
}
this.star && (this.star.active = !1);
this.adaptHomeForFullScreen();
let e = cc.director.getPhysicsManager();
e.enabled = !0;
e.gravity = cc.v2(0, -980);
cc.director.getCollisionManager().enabled = !0;
this.adaptPlayAreaForBottomBar();
this.setupGround();
this.isGuideLineVisible = this.enableGuideLine;
this.gameArea.on("create-merged-coin", this.onCreateMergedCoin, this);
let t = i.consumeSkipInitialBottomCoinsOnce();
if (this.loadGameSceneData()) this.ensureCurrentRoundStats(); else {
this.score = 0;
this.resetCurrentRoundStats();
t || this.createInitialBottomCoins();
this.setupPreviewQueue();
}
this.keepCoinsAboveGround();
this.createPreviewCoin();
this.updateNextPreviewCoin();
this.updateScore();
this.saveBoardState(!0);
this.setFakeMoney();
this.startGetMoneyTipScroll();
this.scheduleOnce(() => {
this.setTaskBtnShowOrHide();
}, 1.5);
this.refreshDeadLineOpacity();
this.scheduleOnce(() => {
this.adaptHomeForFullScreen();
this.adaptPlayAreaForBottomBar();
this.setupGround();
this.keepCoinsAboveGround();
this.drawGuideLine();
this.saveBoardState(!0);
}, 0);
this.btnRule.addComponent(d.default).registerTouchEvent(() => {
v.default.open();
});
this.btnSetting.addComponent(d.default).registerTouchEvent(() => {
C.default.open();
});
this.btnGetMoney.addComponent(d.default).registerTouchEvent(() => {
B.default.open();
});
this.btnFakeMoney.addComponent(d.default).registerTouchEvent(() => {
f.default.open();
});
this.btnDebug.addComponent(d.default).registerTouchEvent(() => {
L.default.open();
});
this.btnTask.addComponent(d.default).registerTouchEvent(() => {
y.default.open();
});
this.btnDraw.addComponent(d.default).registerTouchEvent(() => {
let e = this.getDrawResult();
if (1 == e.canLottery) ; else {
let t = M.default.getlab("45").replace("%{0}", e.needScore.toString());
h.default.show_toast({
text: t
});
}
});
}
updateProgressbar(e = !0) {
const t = r.default.getInstance().getData(l.default);
let a = e ? g.gameUtils.getCurrentDrawScore() : this.displayedDrawScore;
a = Math.max(0, Number(a) || 0);
this.displayedDrawScore = a;
let i = g.gameUtils.checkLotteryStatus(a, t.currentLotteryCount);
this.progressBarLabel.string = a + "/" + i.requiredScore;
if (1 == i.canLottery) {
this.progressBarDraw.progress = 1;
this.showLuckDrawDialogAfterDelay();
} else this.progressBarDraw.progress = a / i.requiredScore;
let o = i.needScore;
this.progressBarDrawLabel.string = M.default.getlab("45").replace("%{0}", o.toString());
}
showLuckDrawDialogAfterDelay() {
if (!(this.gameOver || this.pendingLuckDrawPopup || h.default.ui_is_show("LuckDrawDialog"))) {
this.pendingLuckDrawPopup = !0;
this.scheduleOnce(() => {
this.pendingLuckDrawPopup = !1;
this.node && this.node.isValid && (this.gameOver || this.getDrawResult().canLottery && (this.shouldDelayLuckDrawPopup() ? this.showLuckDrawDialogAfterDelay() : this.isPopupShowingOrLoading("LuckDrawDialog") || _.default.open()));
}, this.popupDelaySeconds);
}
}
shouldDelayLuckDrawPopup() {
return this.pendingRewardPopup || w.HWL.adstart || this.isPopupShowingOrLoading("RewardDialog") || this.isPopupShowingOrLoading("LuckyDrawRewardDialog") || this.isPopupShowingOrLoading("FailDialog");
}
shouldDelayRewardPopup() {
return this.isPopupShowingOrLoading("RewardDialog") || this.isPopupShowingOrLoading("LuckDrawDialog") || this.isPopupShowingOrLoading("LuckyDrawRewardDialog") || this.isPopupShowingOrLoading("FailDialog");
}
isPopupShowingOrLoading(e) {
return h.default.ui_is_show(e) || !!h.default.ui_is_loading[e];
}
isOnMainSceneWithoutPopup() {
if (!this.node || !this.node.isValid || !this.node.activeInHierarchy) return !1;
if (i._instance !== this || this.gameOver || w.HWL.adstart || this.pendingLuckDrawPopup || this.pendingRewardPopup) return !1;
let e = cc.director.getScene();
if (!e || "GameScene" !== e.name) return !1;
for (let e in h.default.ui_is_loading) if (h.default.ui_is_loading[e]) return !1;
for (let e in h.default.all_ui) {
let t = h.default.all_ui[e];
if (t && t.node && t.node.isValid && t.node.active) return !1;
}
return !0;
}
scheduleRewardPopup(e) {
if (!this.gameOver && !this.pendingRewardPopup) {
this.pendingRewardPopup = !0;
this.scheduleOnce(() => {
this.pendingRewardPopup = !1;
this.node && this.node.isValid && (this.gameOver || (this.shouldDelayRewardPopup() ? this.scheduleRewardPopup(e) : e()));
}, this.popupDelaySeconds);
}
}
getDrawResult() {
const e = r.default.getInstance().getData(l.default);
let t = g.gameUtils.getCurrentDrawScore();
return g.gameUtils.checkLotteryStatus(t, e.currentLotteryCount);
}
setshowGuide() {
9999 != r.default.getInstance().getData(l.default).guideStep && m.default.open();
}
setLabel() {
this.nextPreviewCoinLabel.string = M.default.getlab("43");
this.fakeMoneyLabel.string = M.default.getlab("8");
this.btnDrawLabel.string = M.default.getlab("91");
let e = this.bubble.y;
this.bubble.y = e;
cc.tween(this.bubble).repeatForever(cc.tween().to(1, {
y: e + 10
}, {
easing: "sineInOut"
}).to(2, {
y: e - 10
}, {
easing: "sineInOut"
}).to(1, {
y: e
}, {
easing: "sineInOut"
})).start();
}
setFakeMoney() {
const e = r.default.getInstance().getData(l.default);
this.fakeMoneyTxt.string = I.default.getmonstr(e.fakeMoney) + "";
let t = 0, a = I.default.fake_products.real_products[0];
if (e.fakeMoney >= a.withdrawAmount) this.bubble.active = !1; else {
this.bubble.active = !0;
t = a.withdrawAmount;
let i = I.default.getmonstr(t - e.fakeMoney);
this.bubbleLabel.string = M.default.getlab("56").replace("%{0}", i);
}
}
startGetMoneyTipScroll() {
let e = this.getMoneyTipNode || this.findChildByName(this.node, "getMoneyTip");
if (e && e.isValid) {
this.getMoneyTipNode = e;
this.refreshGetMoneyTipScrollStartYs(this.getMoneyTipScrollNodes());
this.refreshGetMoneyTipText();
this.updateGetMoneyTipCallback || (this.updateGetMoneyTipCallback = this.scrollGetMoneyTip.bind(this));
this.unschedule(this.updateGetMoneyTipCallback);
this.schedule(this.updateGetMoneyTipCallback, this.getMoneyTipScrollDuration);
}
}
stopGetMoneyTipScroll() {
this.updateGetMoneyTipCallback && this.unschedule(this.updateGetMoneyTipCallback);
let e = this.getMoneyTipScrollNodes();
for (let t = 0; t < e.length; t++) e[t] && e[t].isValid && cc.Tween.stopAllByTarget(e[t]);
this.getMoneyTipNode && this.getMoneyTipNode.isValid && cc.Tween.stopAllByTarget(this.getMoneyTipNode);
}
scrollGetMoneyTip() {
let e = this.getMoneyTipNode || this.findChildByName(this.node, "getMoneyTip");
if (!e || !e.isValid) return;
this.getMoneyTipNode = e;
let t = this.getMoneyTipScrollNodes();
if (t.length <= 0) return;
this.getMoneyTipScrollStartYs.length != t.length && this.refreshGetMoneyTipScrollStartYs(t);
let a = this.getMoneyTipScrollDistance();
for (let e = 0; e < t.length; e++) {
let i = t[e];
if (!i || !i.isValid) continue;
let o = this.getMoneyTipScrollStartYs[e] || 0;
cc.Tween.stopAllByTarget(i);
i.y = o;
cc.tween(i).to(this.getMoneyTipTweenDuration, {
y: o + a
}, {
easing: "sineInOut"
}).call(() => {
if (i && i.isValid) {
i.y = o - a;
0 == e && this.refreshGetMoneyTipText();
}
}).to(this.getMoneyTipTweenDuration, {
y: o
}, {
easing: "sineInOut"
}).start();
}
}
getMoneyTipScrollDistance() {
let e = this.getMoneyTipNode || this.findChildByName(this.node, "getMoneyTip"), t = e && e.isValid ? e.getChildByName("mask") : null;
return t && t.isValid && t.height > 0 ? t.height + 10 : this.getMoneyTipMoveY;
}
getMoneyTipScrollNodes() {
let e = this.getMoneyTipNode || this.findChildByName(this.node, "getMoneyTip");
if (!e || !e.isValid) return [];
let t = e.getChildByName("mask");
if (t && t.isValid) {
let e = [];
for (let a = 0; a < t.childrenCount; a++) {
let i = t.children[a];
i && i.isValid && e.push(i);
}
if (e.length > 0) return e;
}
return [ e ];
}
refreshGetMoneyTipScrollStartYs(e) {
this.getMoneyTipScrollStartYs = [];
for (let t = 0; t < e.length; t++) this.getMoneyTipScrollStartYs.push(e[t] && e[t].isValid ? e[t].y : 0);
}
refreshGetMoneyTipText() {
let e = this.findChildByName(this.getMoneyTipNode, "getMoneyTipLabel"), t = e ? e.getComponent(cc.RichText) : null;
if (!t) return;
let a = this.getRandomFakeUserName(), i = this.getRandomInt(2, 10).toString(), o = this.getRandomFakeWithdrawAmount(), n = I.default.getmonstr(o);
t.string = M.default.getlab("109").replace("%{0}", a).replace("%{1}", i).replace("%{2}", n);
}
fitGetMoneyTipLabel(e) {
e.maxWidth = 530;
for (let t = 30; t >= 24; t--) {
e.fontSize = t;
e.lineHeight = t + 5;
this.updateGetMoneyTipRichTextLayout(e);
if (e.node.height <= 2 * e.lineHeight + 1) break;
}
}
updateGetMoneyTipRichTextLayout(e) {
let t = e._updateRichText;
t && t.call(e);
}
getRandomFakeUserName() {
return this.getRandomLetter() + "**" + this.getRandomLetter() + this.getRandomLetter();
}
getRandomLetter() {
return String.fromCharCode(65 + this.getRandomInt(0, 25));
}
getRandomFakeWithdrawAmount() {
var e;
let t = (null === (e = I.default.newfake_products) || void 0 === e ? void 0 : e.new_Fake_products) || [];
if (!t || t.length <= 0) return 0;
let a = t[this.getRandomInt(0, t.length - 1)];
return Number(null == a ? void 0 : a.withdrawAmount) || 0;
}
getRandomInt(e, t) {
return Math.floor(Math.random() * (t - e + 1)) + e;
}
refreshDeadLineOpacity() {
if (!this.deadLine || !this.deadLine.isValid || !this.gameArea) return;
this.deadLine.width = this.gameArea.width;
let e = -Infinity;
for (let t = 0; t < this.gameArea.childrenCount; t++) {
let a = this.gameArea.children[t];
if (!a || !a.isValid || !a.active) continue;
let i = a.getComponent(u.default);
if (!i || i.isPreview || i.isMerging) continue;
let o = a.getComponent(cc.RigidBody);
if (o && o.enabled) {
let e = o.linearVelocity;
if (e && (Math.abs(e.x) > this.deadLineMovingVelocityLimit || Math.abs(e.y) > this.deadLineMovingVelocityLimit)) continue;
}
e = Math.max(e, i.getTopY());
}
let t = this.getGroundYInGameArea();
if (e >= t + 2 * (this.gameArea.height * (1 - this.gameArea.anchorY) - t) / 3) {
if (!this.deadLineWarningVisible || !this.deadLine.active) {
this.deadLineWarningVisible = !0;
cc.Tween.stopAllByTarget(this.deadLine);
this.deadLine.active = !0;
this.deadLine.opacity = 0;
cc.tween(this.deadLine).to(.18, {
opacity: 255
}).to(.18, {
opacity: 170
}).to(.18, {
opacity: 255
}).start();
}
} else {
this.deadLineWarningVisible = !1;
cc.Tween.stopAllByTarget(this.deadLine);
this.deadLine.opacity = 0;
this.deadLine.active = !1;
}
}
getFakeMoneyIconNode() {
if (this.btnFakeMoney && this.btnFakeMoney.isValid && this.btnFakeMoney.parent) {
const e = this.btnFakeMoney.parent.getChildByName("moneyIcon");
if (e && e.isValid) return e;
}
return this.btnFakeMoney;
}
playFlyFakePlusMoney(e) {
let t = this.getFlyFakePlusMoneyNode();
if (!t) return;
let a = Math.max(0, Number(e) || 0), i = this.findChildByName(t, "flyMoneyTxt"), o = i ? i.getComponent(cc.Label) : null;
o && (o.string = "+" + I.default.getmonstr(a));
this.flyFakePlusMoneyStartPosition || (this.flyFakePlusMoneyStartPosition = cc.v3(t.x, t.y, t.z));
let n = cc.v3(this.flyFakePlusMoneyStartPosition.x, this.flyFakePlusMoneyStartPosition.y, this.flyFakePlusMoneyStartPosition.z), s = cc.v3(n.x, n.y + this.flyFakePlusMoneyMoveY, n.z);
cc.Tween.stopAllByTarget(t);
t.active = !0;
t.opacity = 255;
t.scale = 1;
t.position = n;
cc.tween(t).to(this.flyFakePlusMoneyDuration, {
position: s,
opacity: 0
}, {
easing: "sineOut"
}).call(() => {
if (t && t.isValid) {
t.active = !1;
t.opacity = 255;
t.position = n;
}
}).start();
}
hideFlyFakePlusMoney() {
let e = this.getFlyFakePlusMoneyNode();
if (e) {
this.flyFakePlusMoneyStartPosition || (this.flyFakePlusMoneyStartPosition = cc.v3(e.x, e.y, e.z));
cc.Tween.stopAllByTarget(e);
e.active = !1;
e.opacity = 255;
e.position = cc.v3(this.flyFakePlusMoneyStartPosition.x, this.flyFakePlusMoneyStartPosition.y, this.flyFakePlusMoneyStartPosition.z);
}
}
getFlyFakePlusMoneyNode() {
if (this.flyFakePlusMoney && this.flyFakePlusMoney.isValid) return this.flyFakePlusMoney;
let e = this.findChildByName(this.node, "plusFlyMoney");
if (e && e.isValid) {
this.flyFakePlusMoney = e;
return e;
}
return null;
}
adaptHomeForFullScreen() {
if (!this.node) return;
let e = cc.view && cc.view.getVisibleSize ? cc.view.getVisibleSize() : cc.winSize, t = Math.max(this.designWidth, this.node.width || 0, e.width || 0), a = Math.max(this.designHeight, this.node.height || 0, e.height || 0);
if (t <= 0 || a <= 0) return;
let i = this.node.getComponent(cc.Widget);
i && i.enabled && (i.enabled = !1);
this.node.setContentSize(t, a);
this.node.x = t * this.node.anchorX;
this.node.y = a * this.node.anchorY;
this.refreshChildrenWidgetAlignment(this.node);
let o = this.node.getChildByName("bg") || this.findChildByName(this.node, "bg");
if (o && o.isValid) {
o.x = 0;
o.y = 0;
o.width = t + 2;
o.height = a + 2;
}
}
refreshChildrenWidgetAlignment(e) {
if (e && e.isValid) for (let t = 0; t < e.childrenCount; t++) this.refreshWidgetAlignment(e.children[t]);
}
refreshWidgetAlignment(e) {
if (!e || !e.isValid) return;
let t = e.getComponent(cc.Widget);
t && t.enabled && t.updateAlignment();
for (let t = 0; t < e.childrenCount; t++) this.refreshWidgetAlignment(e.children[t]);
}
onDestroy() {
cc.game.off(cc.game.EVENT_HIDE, this.onGameHide, this);
this.skipSaveOnDestroy || this.saveBoardState(!1);
this.resetWindowsCoinTimes(!0);
i._instance = null;
this.gameArea.off("create-merged-coin", this.onCreateMergedCoin, this);
this.pendingLuckDrawPopup = !1;
this.pendingRewardPopup = !1;
this.stopGetMoneyTipScroll();
this.cancelGameOverCheck();
this.cancelDelayedPreviewCoin();
this.restoreFailCoinTint();
this.restoreFailFrozenCoinPhysics();
u.default.setMergePausedForGameOver(!1);
null !== this.mergeChainTimer && clearTimeout(this.mergeChainTimer);
this.mainSceneIdleSeconds = 0;
if (this.guidHand && this.guidHand.isValid && this.guidHand.active) {
this.guidHand.active = !1;
this.guidHand.opacity = 255;
}
this.hideMergeComboUI();
}
setGuideLineEnabled(e) {
this.enableGuideLine = e;
this.isGuideLineVisible = e;
!e && this.guideLine && this.guideLine.clear();
}
showGuideLine() {
this.setGuideLineEnabled(!0);
}
hideGuideLine() {
this.setGuideLineEnabled(!1);
}
removeTopTwoThirdCoins() {
this.restoreFailCoinTint();
if (!this.gameArea) return;
this.guideLine && this.guideLine.clear();
let e = [];
for (let t = 0; t < this.gameArea.childrenCount; t++) {
let a = this.gameArea.children[t];
if (!a || !a.isValid) continue;
let i = a.getComponent(u.default);
a.active && i && !i.isPreview && !i.isMerging && e.push(a);
}
if (e.length <= 0) {
this.resumeGameAfterRevive();
return;
}
let t = 0;
for (let a = 0; a < e.length; a++) {
let i = e[a].getComponent(u.default);
i && i.value > t && (t = i.value);
}
e.sort((e, t) => {
let a = e.getComponent(u.default), i = t.getComponent(u.default), o = a ? a.getTopY() : e.y;
return (i ? i.getTopY() : t.y) - o;
});
let a = Math.min(e.length, Math.max(1, Math.ceil(e.length / 3))), i = [];
for (let o = 0; o < e.length && i.length < a; o++) {
let a = e[o], n = a.getComponent(u.default);
n && n.value >= t || i.push(a);
}
if (i.length <= 0) {
this.resumeGameAfterRevive();
this.saveBoardState();
return;
}
let o = 0;
for (let e = 0; e < i.length; e++) {
let t = i[e], a = t.getComponent(u.default);
if (a) {
a.isMerging = !0;
a.disablePhysics();
}
let n = t.getComponent(cc.RigidBody);
if (n) {
n.linearVelocity = cc.v2(0, 0);
n.angularVelocity = 0;
n.enabled = !1;
}
cc.Tween.stopAllByTarget(t);
cc.tween(t).delay(.04 * e).to(.12, {
scale: 0,
opacity: 0
}).call(() => {
t && t.isValid && t.destroy();
if (++o >= i.length) {
this.resumeGameAfterRevive();
this.saveBoardState();
}
}).start();
}
}
resumeGameAfterRevive() {
this.restoreFailCoinTint();
this.restoreFailFrozenCoinPhysics();
this.gameOver = !1;
u.default.setMergePausedForGameOver(!1);
this.resumeBoardCoinsAfterRevive();
this.cancelGameOverCheck();
this.showNextBoo = !0;
this.canClick = !0;
if (!this.currentPreviewCoin || !this.currentPreviewCoin.isValid) {
this.currentPreviewX = 0;
this.createPreviewCoin();
}
}
resumeBoardCoinsAfterRevive() {
if (this.gameArea) for (let e = 0; e < this.gameArea.childrenCount; e++) {
let t = this.gameArea.children[e];
if (!t || !t.isValid || !t.active) continue;
let a = t.getComponent(u.default);
a && !a.isPreview && a.resumeAfterRevive();
}
}
update(e) {
this.updatePendingDeadLineFailCoin(e);
this.deadLineRefreshElapsed += Math.max(0, e);
if (this.deadLineRefreshElapsed >= this.deadLineRefreshInterval) {
this.deadLineRefreshElapsed = 0;
this.refreshDeadLineOpacity();
}
if (this.guidHand && this.guidHand.isValid) {
let t = !!this.node && this.node.isValid && this.node.activeInHierarchy && i._instance === this && !this.gameOver && !w.HWL.adstart && !this.pendingLuckDrawPopup && !this.pendingRewardPopup, a = cc.director.getScene();
if (t = t && !!a && "GameScene" === a.name) for (let e in h.default.ui_is_loading) if (h.default.ui_is_loading[e]) {
t = !1;
break;
}
if (t) for (let e in h.default.all_ui) {
let a = h.default.all_ui[e];
if (a && a.node && a.node.isValid && a.node.active) {
t = !1;
break;
}
}
if (t) {
this.mainSceneIdleSeconds += Math.max(0, e);
if (this.mainSceneIdleSeconds >= this.mainSceneIdleGuideDelay && !this.guidHand.active) {
this.guidHand.opacity = 255;
this.guidHand.active = !0;
}
} else {
this.mainSceneIdleSeconds = 0;
if (this.guidHand.active) {
this.guidHand.active = !1;
this.guidHand.opacity = 255;
}
}
}
if (!this.gameOver) {
this.saveBoardElapsed += e;
if (this.boardStateDirty && this.saveBoardElapsed >= this.saveBoardInterval) {
this.saveBoardState(!1);
this.flushBoardStateToStorage();
}
}
}
getRandomDropValue() {
let e = 0;
for (let t = 0; t < this.gameArea.childrenCount; t++) {
let a = this.gameArea.children[t];
if (!a || !a.isValid || !a.active) continue;
let i = a.getComponent(u.default);
!i || i.isPreview || i.isMerging || (e = Math.max(e, i.value));
}
let t = Object.keys(I.default.globalData.level_list).map(Number).sort((e, t) => e - t), a = t[0];
for (let i = 0; i < t.length && !(t[i] > e); i++) a = t[i];
let i = I.default.globalData.level_list[a], o = i[0], n = i[1], s = 0;
for (let e = 0; e < n.length; e++) s += n[e];
let r = Math.random() * s, l = 0;
for (let e = 0; e < o.length; e++) if (r < (l += n[e])) return o[e];
return o[o.length - 1];
}
setupPreviewQueue() {
this.currentCoinValue = this.getRandomDropValue();
this.nextCoinValue = this.getRandomDropValue();
this.currentPreviewX = 0;
}
ensureCurrentRoundStats() {
const e = r.default.getInstance().getData(l.default);
if (!e.hasCurrentRoundStats) {
e.hasCurrentRoundStats = !0;
e.currentRoundStartHistroyMaxScore = e.histroyMaxScore || 0;
e.currentRoundCoin1024Number = 0;
r.default.getInstance().set_local_storeage();
}
}
resetCurrentRoundStats() {
const e = r.default.getInstance().getData(l.default);
e.hasCurrentRoundStats = !0;
e.currentRoundStartHistroyMaxScore = e.histroyMaxScore || 0;
e.currentRoundCoin1024Number = 0;
r.default.getInstance().set_local_storeage();
}
advancePreviewQueue() {
this.currentCoinValue = this.nextCoinValue || this.getRandomDropValue();
this.nextCoinValue = this.getRandomDropValue();
this.currentPreviewX = 0;
this.updateNextPreviewCoin();
}
createCoinNode(e, t, a, i, o = !1, n = !0) {
let s = cc.instantiate(this.coinPrefab);
s.position = a;
let r = s.getComponent(u.default);
if (r) {
r.coinSprites = this.coinSprites;
r.isPreview = i;
r.value = e;
}
t.addChild(s);
n && !o && this.playCoinSpawnTween(s);
o && r && r.setupPhysics();
return s;
}
playCoinSpawnTween(e) {
if (!e || !e.isValid) return;
let t = ++this.coinSpawnUnlockToken;
this.cancelCoinSpawnUnlockFallback();
this.canClick = !1;
cc.Tween.stopAllByTarget(e);
let a = e.scale || 1;
e.scale = a * this.coinSpawnStartScale;
cc.tween(e).to(this.coinSpawnTweenDuration, {
scale: a
}, {
easing: cc.easing.backOut
}).call(() => {
this.unlockCoinSpawn(t, e, a);
}).start();
this.coinSpawnUnlockCallback = () => {
this.unlockCoinSpawn(t, e, a);
};
this.scheduleOnce(this.coinSpawnUnlockCallback, this.coinSpawnTweenDuration + this.coinSpawnUnlockDelayBuffer);
}
unlockCoinSpawn(e, t, a) {
if (e === this.coinSpawnUnlockToken) {
this.cancelCoinSpawnUnlockFallback();
t && t.isValid && (t.scale = a);
this.canClick = !this.gameOver && !this.pendingDeadLineFailCoin;
if (this.canClick && this.pendingDropAfterSpawn) {
this.pendingDropAfterSpawn = !1;
this.dropCoin();
}
}
}
cancelCoinSpawnUnlockFallback() {
if (this.coinSpawnUnlockCallback) {
this.unschedule(this.coinSpawnUnlockCallback);
this.coinSpawnUnlockCallback = null;
}
}
cancelCoinSpawnTween(e) {
this.coinSpawnUnlockToken++;
this.cancelCoinSpawnUnlockFallback();
if (e && e.isValid) {
cc.Tween.stopAllByTarget(e);
Math.abs(e.scaleX) <= .01 && (e.scaleX = 1);
Math.abs(e.scaleY) <= .01 && (e.scaleY = 1);
}
this.canClick = !this.gameOver && !this.pendingDeadLineFailCoin;
this.pendingDropAfterSpawn = !1;
}
getGroundYInGameArea() {
if (!this.gameArea) return 0;
if (!this.groundLine) return -this.gameArea.height / 2;
let e = this.groundLine.convertToWorldSpaceAR(cc.v2(0, 0));
return this.gameArea.convertToNodeSpaceAR(e).y;
}
adaptPlayAreaForBottomBar() {
if (!this.gameArea || !this.groundLine || !this.node) return;
null === this.baseGroundY && (this.baseGroundY = this.groundLine.y);
let e = this.groundLine.getComponent(cc.Widget);
e && e.enabled && (e.enabled = !1);
let t = this.findChildByName(this.node, "bottom");
if (t) {
let e = t.getComponent(cc.Widget);
e && e.enabled && e.updateAlignment();
let a = t.height * (1 - t.anchorY), i = t.convertToWorldSpaceAR(cc.v2(0, a)), o = this.gameArea.convertToNodeSpaceAR(i).y + this.bottomSafeGap;
Math.abs(o - this.baseGroundY) > this.bottomOverlapTolerance ? this.groundLine.y = o : this.groundLine.y = this.baseGroundY;
} else this.groundLine.y = this.baseGroundY;
this.deadLine && null === this.baseDeadLineY && (this.baseDeadLineY = this.deadLine.y);
this.previewLine && null === this.basePreviewLineY && (this.basePreviewLineY = this.previewLine.y);
let a = cc.view && cc.view.getVisibleSize ? cc.view.getVisibleSize() : cc.winSize, i = a && a.height > 0 ? a.height : this.designHeight, o = cc.view && cc.view.getFrameSize ? cc.view.getFrameSize() : null;
if (o && o.width > 0 && o.height > 0) {
let e = this.designWidth * o.height / o.width;
i = Math.min(i, e);
}
let n = Math.min(0, i - this.designHeight) * this.shortScreenLineShiftRatio;
if (this.deadLine && null !== this.baseDeadLineY) {
this.deadLine.y = this.baseDeadLineY + n;
this.deadLine.width = this.gameArea.width;
}
if (this.previewLine && null !== this.basePreviewLineY) {
this.previewLine.y = this.basePreviewLineY + n;
this.previewLine.width = this.gameArea.width;
}
if (this.currentPreviewCoin && this.currentPreviewCoin.isValid && this.previewLine) {
let e = this.currentPreviewCoin.getComponent(u.default);
e && e.isPreview && (this.currentPreviewCoin.y = this.previewLine.y);
}
}
keepCoinsAboveGround() {
if (!this.gameArea) return;
let e = this.getGroundYInGameArea();
for (let t = 0; t < this.gameArea.childrenCount; t++) {
let a = this.gameArea.children[t], i = a.getComponent(u.default);
if (!i || i.isPreview || i.isMerging) continue;
let o = e + i.getRadius() + 2;
if (a.y < o) {
a.y = o;
let e = a.getComponent(cc.RigidBody);
if (e) {
e.linearVelocity = cc.v2(0, 0);
e.angularVelocity = 0;
}
}
}
}
createInitialBottomCoins() {
if (!this.gameArea || !this.coinPrefab) return;
let e = this.gameArea.width / (this.initialBottomValues.length + 1), t = -this.gameArea.width / 2 + e, a = this.getGroundYInGameArea();
for (let i = 0; i < this.initialBottomValues.length; i++) {
let o = this.initialBottomValues[i], n = this.createCoinNode(o, this.gameArea, cc.v3(t + e * i, 0, 0), !1, !1, !1), s = n.getComponent(u.default), r = s ? s.getRadius() : n.width / 2;
n.y = a + r + 10;
s && s.setupPhysics(!1);
}
}
findChildByName(e, t) {
if (!e) return null;
for (let a = 0; a < e.childrenCount; a++) {
let i = e.children[a];
if (i.name == t) return i;
let o = this.findChildByName(i, t);
if (o) return o;
}
return null;
}
getNextPreviewSprite() {
if (this.nextPreviewCoin) return this.nextPreviewCoin;
let e = this.findChildByName(this.node, "nextPreviewCoin");
e && (this.nextPreviewCoin = e.getComponent(cc.Sprite));
return this.nextPreviewCoin;
}
updateNextPreviewCoin() {
let e = this.getNextPreviewSprite();
if (!e || !this.coinSprites || this.coinSprites.length <= 0) return;
let t = this.valueToSpriteIndex[this.nextCoinValue];
void 0 !== t && this.coinSprites[t] && (e.spriteFrame = this.coinSprites[t]);
}
loadGameSceneData() {
const e = r.default.getInstance().getData(l.default);
if (!e.hasSavedGameScene) return !1;
let t = e.savedCoins || [];
this.score = Math.max(0, Number(e.roundScore) || Number(e.savedCoinsScore) || 0);
e.roundScore = this.score;
e.savedCoinsScore = this.score;
this.currentCoinValue = e.savedCurrentCoinValue || this.getRandomDropValue();
this.nextCoinValue = e.savedNextCoinValue || this.getRandomDropValue();
this.currentPreviewX = e.savedPreviewX || 0;
e.savedDrawScore = g.gameUtils.getCurrentDrawScore();
if (t.length <= 0) return !0;
let a = 0;
for (let e = 0; e < t.length; e++) {
let i = t[e];
if (!i || !i.value) continue;
let o = this.createCoinNode(i.value, this.gameArea, cc.v3(i.x || 0, i.y || 0, 0), !1, !1, !1);
o.angle = i.angle || 0;
o.scale = i.scale || 1;
let n = o.getComponent(u.default);
n && n.setupPhysics(!1);
a++;
}
return a > 0;
}
saveBoardState(e = !1) {
if (!this.gameArea) return;
const t = r.default.getInstance().getData(l.default);
let a = [];
for (let e = 0; e < this.gameArea.childrenCount; e++) {
let t = this.gameArea.children[e];
if (!t || !t.isValid) continue;
let i = t.getComponent(u.default);
!i || i.isPreview || i.isMerging || a.push({
value: i.value,
x: t.x,
y: t.y,
angle: t.angle || 0,
scale: t.scale || 1
});
}
t.hasSavedGameScene = !0;
t.savedCoins = a;
t.savedCurrentCoinValue = this.currentCoinValue;
t.savedNextCoinValue = this.nextCoinValue;
t.savedPreviewX = this.currentPreviewCoin && this.currentPreviewCoin.isValid ? this.currentPreviewCoin.x : this.currentPreviewX;
t.roundScore = this.score;
t.savedCoinsScore = this.score;
t.savedDrawScore = g.gameUtils.getCurrentDrawScore();
this.boardStateDirty = !0;
e && this.flushBoardStateToStorage();
}
savePreviewPositionState() {
r.default.getInstance().getData(l.default).savedPreviewX = this.currentPreviewX;
this.markBoardStateDirty();
}
markBoardStateDirty() {
this.boardStateDirty = !0;
}
flushBoardStateToStorage() {
if (this.boardStateDirty) {
this.boardStateDirty = !1;
this.saveBoardElapsed = 0;
r.default.getInstance().set_local_storeage();
}
}
clearBoardState() {
const e = r.default.getInstance().getData(l.default);
this.score = 0;
this.displayedDrawScore = 0;
e.hasSavedGameScene = !1;
e.savedCoins = [];
e.roundScore = 0;
e.gameTotalScore = 0;
e.savedCoinsScore = 0;
e.savedDrawScore = 0;
e.currentLotteryCount = 0;
e.savedPreviewX = 0;
e.savedCurrentCoinValue = 1;
e.savedNextCoinValue = 1;
r.default.getInstance().set_local_storeage();
}
clearAllCoinNodes() {
this.restoreFailCoinTint();
if (this.gameArea) {
for (let e = this.gameArea.childrenCount - 1; e >= 0; e--) {
let t = this.gameArea.children[e];
if (!t || !t.isValid) continue;
let a = t.getComponent(u.default);
if (a) {
try {
a.disablePhysics();
} catch (e) {
console.warn("reset clear coin physics error", e);
}
cc.Tween.stopAllByTarget(t);
t.destroy();
}
}
this.currentPreviewCoin = null;
}
}
clearRuntimeEffectNodes() {
this.merge1024FlowToken++;
this.hideMergeComboUI();
this.hideFlyFakePlusMoney();
if (this.merge1024Prefab && this.merge1024Prefab.node && this.merge1024Prefab.node.isValid) {
cc.Tween.stopAllByTarget(this.merge1024Prefab.node);
this.merge1024Prefab.node.active = !1;
}
this.destroyRuntimeEffectChildren(this.node, !1);
this.destroyRuntimeEffectChildren(this.gameArea, !0);
}
destroyRuntimeEffectChildren(e, t) {
if (e && e.isValid) for (let a = e.childrenCount - 1; a >= 0; a--) {
let i = e.children[a];
if (!i || !i.isValid) continue;
let o = "MergeStar" == i.name || "MergeGlowStar" == i.name, n = !!i.getComponent(sp.Skeleton), s = t && ("MergeEffect" == i.name || n) && !i.getComponent(u.default);
if ((o || s) && (!this.merge1024Prefab || i != this.merge1024Prefab.node)) {
cc.Tween.stopAllByTarget(i);
i.destroy();
}
}
}
resetGameAfterFailWithoutAd() {
if (this.gameOver && !this.isResettingFromFailDialogClose) return;
this.gameOver = !0;
this.showNextBoo = !0;
this.canClick = !0;
this.merge1024FlowToken++;
this.cancelGameOverCheck();
this.cancelDelayedPreviewCoin();
this.unscheduleAllCallbacks();
this.pendingLuckDrawPopup = !1;
this.pendingRewardPopup = !1;
if (null !== this.mergeChainTimer) {
clearTimeout(this.mergeChainTimer);
this.mergeChainTimer = null;
this.mergeChainCount = 0;
this.isMergeChainActive = !1;
}
this.guideLine && this.guideLine.clear();
const e = r.default.getInstance().getData(l.default);
this.failFrozenRigidBodyTargets = [];
this.clearAllCoinNodes();
this.clearRuntimeEffectNodes();
this.score = 0;
this.displayedDrawScore = g.gameUtils.getCurrentDrawScore();
e.hasSavedGameScene = !0;
e.savedCoins = [];
e.roundScore = 0;
e.savedCoinsScore = 0;
this.setupPreviewQueue();
this.adaptHomeForFullScreen();
this.adaptPlayAreaForBottomBar();
this.setupGround();
this.keepCoinsAboveGround();
this.gameOver = !1;
u.default.setMergePausedForGameOver(!1);
this.skipSaveOnDestroy = !1;
this.createPreviewCoin();
this.updateNextPreviewCoin();
this.updateScore();
this.set1024Label();
this.setFakeMoney();
this.refreshDeadLineOpacity();
this.drawGuideLine();
this.startGetMoneyTipScroll();
this.saveBoardState(!0);
h.default.clear_ui();
h.default.clear_ui_cache();
}
onCreateMergedCoin(e) {
if (this.gameOver || u.default.isMergePausedForGameOver()) return;
this.isMergeChainActive = !0;
this.mergeChainCount++;
let t;
t = this.mergeChainCount >= 2 && this.mergeChainCount <= 7 ? "sfx_merge_" + this.mergeChainCount : this.mergeChainCount >= 8 ? "sfx_merge_8" : "sfx_merge";
c.default.playSound(t);
null !== this.mergeChainTimer && clearTimeout(this.mergeChainTimer);
this.mergeChainTimer = setTimeout(() => {
let e = this.mergeChainCount;
this.showMergeComboUI(e);
this.isMergeChainActive = !1;
this.mergeChainCount = 0;
this.mergeChainTimer = null;
}, 500);
this.score += e.currentScore;
const a = r.default.getInstance().getData(l.default);
if (500 == e.value) {
let e = r.default.getInstance().getData(l.default);
if (0 == e.firstMergeIcon500) {
e.firstMergeIcon500 = !0;
e.gameRateScore <= 3 && S.default.open();
}
}
a.roundScore = this.score;
a.savedCoinsScore = this.score;
let i = g.gameUtils.addGameTotalScore(e.currentScore);
this.updateScore(!1);
this.markBoardStateDirty();
this.scheduleOnce(() => {
if (this.gameOver || u.default.isMergePausedForGameOver()) return;
let t = this.createCoinNode(e.value, this.gameArea, e.position, !1, !1, !1), a = t.getComponent(u.default), o = cc.v3(e.position.x, e.position.y, e.position.z);
if (a) {
o = this.getMergedCoinSpawnPosition(e, a);
t.position = this.getMergedCoinVisualSpawnPosition(o);
if (e.value != this.MaxValue) {
a.setupPhysics(!1);
this.markBoardStateDirty();
}
}
this.lastMergeComboPosition = cc.v3(o.x, o.y, o.z);
this.playMergeEffectAt(o, e.value);
this.playMergeStarFlyEffect(o, i) || this.applyDrawProgressAfterStarArrive(i);
1 == r.default.getInstance().getData(l.default).guideStep && this.scheduleOnce(() => {
m.default.open();
}, .2);
let n = a && a.sprite && a.sprite.node ? a.sprite.node : t, s = n.scale || 1;
n.scale = s * this.coinSpawnStartScale;
t.opacity = 255;
cc.tween(n).to(.16, {
scale: 1.1 * s
}, {
easing: cc.easing.backOut
}).to(.04, {
scale: s
}, {
easing: "sineOut"
}).call(() => {
if (this.gameOver || u.default.isMergePausedForGameOver()) {
a && a.pauseMergeForGameOver();
t && t.isValid && (t.opacity = 255);
} else t && t.isValid && a && e.value == this.MaxValue && this.playMerge1024Flow(t, e.position);
}).start();
}, 0);
}
getMergedCoinSpawnPosition(e, t) {
let a = t.getRadius(), i = cc.v3(e.position.x, e.position.y, e.position.z);
"number" != typeof e.bottomY || isNaN(e.bottomY) || (i.y = e.bottomY + a + 1);
let o = this.getGroundYInGameArea();
i.y = Math.max(i.y, o + a + 2);
if (this.gameArea) {
let e = -this.gameArea.width / 2 + a, t = this.gameArea.width / 2 - a;
i.x = Math.max(e, Math.min(t, i.x));
}
return i;
}
getMergedCoinVisualSpawnPosition(e) {
return cc.v3(e.x, e.y + this.mergeCoinSpawnLiftY, e.z);
}
playMerge1024Flow(e, t) {
if (!e || !e.isValid) return;
if (this.gameOver || u.default.isMergePausedForGameOver()) {
let t = e.getComponent(u.default);
t && t.pauseMergeForGameOver();
e.opacity = 255;
return;
}
const a = r.default.getInstance().getData(l.default);
a.coin1024Number++;
a.currentRoundCoin1024Number = Math.max(0, (Number(a.currentRoundCoin1024Number) || 0) + 1);
g.gameUtils.addToday1024Number();
console.log("合成超大金币了！！！！");
let i = e.getComponent(u.default);
if (i) {
i.isMerging = !0;
i.disablePhysics();
}
this.showNextBoo = !1;
this.cancelDelayedPreviewCoin();
let o = ++this.merge1024FlowToken, n = (this.merge1024Prefab && this.merge1024Prefab.node && this.merge1024Prefab.node, 
this.getMerge1024EffectWorldPosition(t)), s = this.getNodeLocalPositionFromWorld(e.parent, n), c = cc.v3(e.x + .35 * (s.x - e.x), Math.max(e.y, s.y) + 120, e.z);
cc.Tween.stopAllByTarget(e);
e.active = !0;
e.opacity = 255;
cc.tween(e).to(.22, {
position: c,
scale: 1.08 * e.scale
}, {
easing: "sineOut"
}).to(.28, {
position: s,
scale: Math.max(.72 * e.scale, .2),
opacity: 0
}, {
easing: "sineInOut"
}).call(() => {
if (this.gameOver || u.default.isMergePausedForGameOver()) {
if (e && e.isValid) {
e.opacity = 255;
let t = e.getComponent(u.default);
t && t.pauseMergeForGameOver();
}
} else {
e && e.isValid && e.destroy();
this.markBoardStateDirty();
this.playMerge1024PrefabAnimation(o, n);
}
}).start();
}
getMerge1024EffectWorldPosition(e) {
if (this.merge1024Prefab && this.merge1024Prefab.node && this.merge1024Prefab.node.isValid) {
let e = this.merge1024Prefab.node;
if (e.parent) return e.parent.convertToWorldSpaceAR(e.position);
}
return this.gameArea.convertToWorldSpaceAR(cc.v3(e.x, e.y, 0));
}
getNodeLocalPositionFromWorld(e, t) {
if (!e || !e.isValid) return cc.v3(t.x, t.y, t.z);
let a = e.convertToNodeSpaceAR(t);
return cc.v3(a.x, a.y, 0);
}
playMerge1024PrefabAnimation(e, t) {
if (this.gameOver || u.default.isMergePausedForGameOver()) return;
if (!this.merge1024Prefab || !this.merge1024Prefab.node || !this.merge1024Prefab.node.isValid) {
this.finishMerge1024Flow(e);
return;
}
let a = this.merge1024Prefab.node;
cc.Tween.stopAllByTarget(a);
a.active = !0;
a.opacity = 255;
a.scale = 1;
a.position = this.getNodeLocalPositionFromWorld(a.parent, t);
this.merge1024Prefab.setCompleteListener(null);
this.merge1024Prefab.setAnimation(0, "idle1", !1);
this.merge1024Prefab.setCompleteListener(() => {
if (e == this.merge1024FlowToken) {
this.merge1024Prefab.setCompleteListener(null);
this.flyMerge1024PrefabToGetMoney(e);
}
});
}
flyMerge1024PrefabToGetMoney(e) {
if (this.gameOver || u.default.isMergePausedForGameOver()) return;
if (!this.merge1024Prefab || !this.merge1024Prefab.node || !this.merge1024Prefab.node.isValid) {
this.finishMerge1024Flow(e);
return;
}
let t = this.merge1024Prefab.node, a = this.getGetMoneyButtonWorldPosition();
if (!a) {
t.active = !1;
this.finishMerge1024Flow(e);
return;
}
let o = this.getNodeLocalPositionFromWorld(t.parent, a), n = cc.v3(t.x, t.y, t.z), s = cc.v2(n.x, Math.max(n.y, o.y) + 180), r = cc.v2(o.x, Math.max(n.y, o.y) + 110), l = cc.v2(o.x, o.y);
cc.Tween.stopAllByTarget(t);
cc.tween(t).parallel(cc.tween().bezierTo(.55, s, r, l), cc.tween().to(.55, {
scale: .35,
opacity: 0
}, {
easing: "sineInOut"
})).call(() => {
if (!this.gameOver && !u.default.isMergePausedForGameOver() && t && t.isValid) {
i.instance.set1024Label();
t.active = !1;
t.opacity = 255;
t.scale = 1;
t.position = n;
this.playGetMoneyButtonPulse();
this.finishMerge1024Flow(e);
}
}).start();
}
getGetMoneyButtonWorldPosition() {
return this.btnGetMoney && this.btnGetMoney.isValid && this.btnGetMoney.parent ? this.btnGetMoney.parent.convertToWorldSpaceAR(this.btnGetMoney.position) : null;
}
playGetMoneyButtonPulse() {
if (!this.btnGetMoney || !this.btnGetMoney.isValid) return;
let e = this.btnGetMoney.scale || 1;
cc.Tween.stopAllByTarget(this.btnGetMoney);
cc.tween(this.btnGetMoney).to(.12, {
scale: 1.12 * e
}, {
easing: "sineOut"
}).to(.1, {
scale: e
}, {
easing: "sineIn"
}).to(.12, {
scale: 1.12 * e
}, {
easing: "sineOut"
}).to(.1, {
scale: e
}, {
easing: "sineIn"
}).start();
}
finishMerge1024Flow(e) {
if (!this.gameOver && !u.default.isMergePausedForGameOver() && e == this.merge1024FlowToken) {
this.showNextBoo = !0;
this.currentPreviewCoin && this.currentPreviewCoin.isValid || this.scheduleCreatePreviewCoin();
this.markBoardStateDirty();
}
}
playMergeStarFlyEffect(e, t = -1) {
if (!(this.star && this.star.isValid && this.gameArea && this.progressBarDraw && this.progressBarDraw.node && this.node && this.node.isValid)) return !1;
let a = this.node;
if (!this.progressBarDraw.node.parent) return !1;
let i = this.gameArea.convertToWorldSpaceAR(cc.v3(e.x, e.y, 0)), o = this.getDrawProgressBarTargetWorldPosition(), n = a.convertToNodeSpaceAR(i), s = a.convertToNodeSpaceAR(o), r = this.star.scale || 1, l = !1;
this.star.active = !1;
for (let e = 0; e < this.mergeStarCount; e++) {
let i = cc.instantiate(this.star);
a.addChild(i, 99999);
i.active = !0;
let o = Math.random() * Math.PI * 2, c = this.getRandomRange(10, this.mergeStarScatterRadius), d = cc.v2(n.x + Math.cos(o) * c, n.y + Math.sin(o) * c), h = cc.v2(this.getRandomRange(-this.mergeStarTargetSpread, this.mergeStarTargetSpread), this.getRandomRange(.5 * -this.mergeStarTargetSpread, .5 * this.mergeStarTargetSpread)), u = cc.v2(s.x + h.x, s.y + h.y), g = this.mergeStarFlyDuration + this.getRandomRange(-.08, .16), p = r * this.getRandomRange(.5, .68), f = p * this.getRandomRange(1.18, 1.38), m = this.setupMergeStarStyle(i), y = .38 * r;
if ("MergeGlowStar" == i.name) {
f *= 1.28;
y = .5 * r;
}
let _ = cc.v2(u.x - d.x, u.y - d.y).mag(), v = (e % 2 == 0 ? 1 : -1) * this.getRandomRange(55, 110), b = _ < 260 ? this.getRandomRange(120, 180) : this.getRandomRange(80, 145), S = Math.max(d.y, u.y) + b, C = cc.v2(d.x + .22 * (u.x - d.x) + v, S), w = cc.v2(d.x + .72 * (u.x - d.x) - .35 * v, S - .18 * b);
i.position = cc.v3(n.x, n.y, 0);
i.opacity = 0;
i.scale = p;
i.angle = this.getRandomRange(0, 360);
cc.tween(i).delay(e * this.mergeStarDelayGap).to(.14, {
position: cc.v3(d.x, d.y, 0),
opacity: m,
scale: f
}, {
easing: "sineOut"
}).parallel(cc.tween().bezierTo(g, C, w, u), cc.tween().to(g, {
angle: i.angle + this.getRandomRange(160, 300)
}, {
easing: "sineInOut"
}), cc.tween().delay(.72 * g).to(.28 * g, {
opacity: 0,
scale: y
}, {
easing: "sineIn"
})).call(() => {
if (!l) {
l = !0;
this.applyDrawProgressAfterStarArrive(t);
}
i && i.isValid && i.destroy();
}).start();
}
return !0;
}
applyDrawProgressAfterStarArrive(e) {
if (this.node && this.node.isValid) {
this.displayedDrawScore = e >= 0 ? Math.max(this.displayedDrawScore, e) : g.gameUtils.getCurrentDrawScore();
this.updateProgressbar(!1);
}
}
setupMergeStarStyle(e) {
let t = e.getComponent(cc.Sprite), a = Math.random();
e.color = cc.Color.WHITE;
e.name = "MergeStar";
if (t) {
t.srcBlendFactor = cc.macro.BlendFactor.SRC_ALPHA;
t.dstBlendFactor = cc.macro.BlendFactor.ONE_MINUS_SRC_ALPHA;
}
if (a < .3) {
e.name = "MergeGlowStar";
e.color = new cc.Color(255, 246, 174, 255);
if (t) {
t.srcBlendFactor = cc.macro.BlendFactor.SRC_ALPHA;
t.dstBlendFactor = cc.macro.BlendFactor.ONE;
}
return this.getRandomRange(170, 225);
}
return a < .7 ? this.getRandomRange(125, 190) : this.getRandomRange(205, 255);
}
getDrawProgressBarTargetWorldPosition() {
let e = this.progressBarDraw.barSprite;
if (e && e.node && e.node.isValid) {
let t = e.node, a = Math.max(0, Math.min(1, this.progressBarDraw.progress || 0)), i = this.progressBarDraw.reverse ? t.width * (1 - a) : t.width * a;
i = Math.max(0, Math.min(t.width, i));
return t.convertToWorldSpaceAR(cc.v3(i, 0, 0));
}
let t = this.progressBarDraw.node;
return t.parent.convertToWorldSpaceAR(t.position);
}
getRandomRange(e, t) {
return Math.random() * (t - e) + e;
}
playMergeEffectAt(e, t) {
if (!this.mergeEffectPrefab) return;
let a = cc.instantiate(this.mergeEffectPrefab);
a.name = "MergeEffect";
a.position = e;
this.gameArea.addChild(a);
let i = a.getComponent(sp.Skeleton);
if (i) {
let e = this.valueToScale[t] || 1;
a.scale = e;
let o = this.valueToAnimName[t];
i.setAnimation(0, o, !1);
i.setCompleteListener(() => {
a.destroy();
});
} else this.scheduleOnce(() => {
a && a.isValid && a.destroy();
}, 1);
}
getMergeChainStatus() {
return {
isActive: this.isMergeChainActive,
count: this.mergeChainCount
};
}
showMergeComboUI(e) {
if (e < 2) return;
let t = this.currentMergeIcon, a = this.currentMergeTimes, i = this.combo;
if (!(t && t.node && t.node.isValid && a && a.node && a.node.isValid && i && i.isValid && this.MergeIcon && this.MergeTimes) || this.MergeIcon.length <= 0 || this.MergeTimes.length <= 0) return;
let o = this.MergeIcon[0], n = 0, s = 0;
if (e >= 5) {
n = 3;
s = 5;
} else {
n = e - 2;
s = e;
}
o = this.MergeIcon[n];
c.default.playSound("sfx_combo_" + s);
let r = Math.max(2, Math.min(e, 8)), l = Math.min(r - 2, this.MergeTimes.length - 1), d = this.MergeTimes[l];
if (!o || !d) return;
t.spriteFrame = o;
a.spriteFrame = d;
this.setMergeComboNearLastMerge(i, a.node);
a.node.active = !1;
a.node.opacity = 255;
a.node.scale = this.mergeComboTimesScale;
let h = 0, u = () => {
++h < 2 || this.scheduleOnce(() => {
this.playMergeComboAppearTween(a.node, this.mergeComboTimesScale, () => {
this.playMergeComboHideTween(i, this.mergeComboNodeScale, () => {
this.playMergeIconFadeOutTween(t.node, this.mergeComboIconScale);
});
this.playMergeComboHideTween(a.node, this.mergeComboTimesScale);
});
}, .1);
};
this.playMergeComboAppearTween(t.node, this.mergeComboIconScale, u);
this.playMergeComboAppearTween(i, this.mergeComboNodeScale, u);
}
setMergeComboNearLastMerge(e, t) {
if (!(this.lastMergeComboPosition && this.gameArea && e && e.isValid && t && t.isValid && e.parent && t.parent)) return;
let a = this.gameArea.convertToWorldSpaceAR(cc.v3(this.lastMergeComboPosition.x, this.lastMergeComboPosition.y, 0)), i = e.parent.convertToNodeSpaceAR(a);
null === this.mergeComboBaseX && (this.mergeComboBaseX = e.x);
null === this.mergeTimesBaseX && (this.mergeTimesBaseX = t.x);
let o = this.mergeTimesBaseX - this.mergeComboBaseX, n = i.x - .5 * o, s = i.y + this.mergeComboBaseYGap, r = -e.parent.width * e.parent.anchorX + .5 * Math.max(e.width, t.width), l = e.parent.width * (1 - e.parent.anchorX) - .5 * Math.max(e.width, t.width) - o, c = -e.parent.height * e.parent.anchorY + .5 * this.mergeComboBaseYGap, d = e.parent.height * (1 - e.parent.anchorY) - .5 * this.mergeComboBaseYGap;
n = Math.max(r, Math.min(l, n));
s = Math.max(c, Math.min(d, s));
e.x = n;
e.y = s;
t.x = n + o;
t.y = s;
}
playMergeComboAppearTween(e, t, a = null) {
if (e && e.isValid) {
cc.Tween.stopAllByTarget(e);
e.active = !0;
e.opacity = 255;
e.scale = Math.max(.2 * t, .01);
cc.tween(e).to(.08, {
scale: 1.12 * t
}).to(.02, {
scale: t
}).call(() => {
if (e && e.isValid) {
e.opacity = 255;
e.scale = t;
a && a();
}
}).start();
}
}
playMergeComboHideTween(e, t, a = null) {
e && e.isValid && cc.tween(e).delay(.5).to(.2, {
scale: Math.max(.2 * t, .2),
opacity: 0
}, {
easing: "sineIn"
}).call(() => {
if (e && e.isValid) {
e.active = !1;
e.opacity = 255;
e.scale = t;
a && a();
}
}).start();
}
playMergeIconFadeOutTween(e, t) {
if (e && e.isValid) {
cc.Tween.stopAllByTarget(e);
e.active = !0;
e.opacity = 255;
e.scale = t;
cc.tween(e).to(.5, {
scale: 2 * t,
opacity: 0
}, {
easing: "sineOut"
}).call(() => {
if (e && e.isValid) {
e.active = !1;
e.opacity = 255;
e.scale = t;
}
}).start();
}
}
hideMergeComboUI() {
let e = this.currentMergeIcon;
if (e && e.node && e.node.isValid) {
cc.Tween.stopAllByTarget(e.node);
e.node.active = !1;
e.node.opacity = 255;
e.node.scale = this.mergeComboIconScale;
}
if (this.combo && this.combo.isValid) {
cc.Tween.stopAllByTarget(this.combo);
this.combo.active = !1;
this.combo.opacity = 255;
this.combo.scale = this.mergeComboNodeScale;
}
let t = this.currentMergeTimes;
if (t && t.node && t.node.isValid) {
cc.Tween.stopAllByTarget(t.node);
t.node.active = !1;
t.node.opacity = 255;
t.node.scale = this.mergeComboTimesScale;
}
}
setupGround() {
if (!this.gameArea) return;
if (this.groundLine) {
(this.groundLine.getComponent(cc.RigidBody) || this.groundLine.addComponent(cc.RigidBody)).type = cc.RigidBodyType.Static;
let e = this.groundLine.getComponent(cc.PhysicsBoxCollider) || this.groundLine.addComponent(cc.PhysicsBoxCollider);
e.size = cc.size(this.gameArea.width, 30);
e.offset = cc.v2(0, -15);
e.friction = .65;
e.restitution = 0;
e.apply();
}
this.removeLegacyWalls();
let e = this.getGroundYInGameArea() - 100, t = Math.max(this.gameArea.height * (1 - this.gameArea.anchorY), this.previewLine ? this.previewLine.y + 100 : 0, this.deadLine ? this.deadLine.y + 100 : 0), a = Math.max(this.gameArea.height, t - e), i = e + a / 2;
this.createWall("WallLeft", -this.gameArea.width / 2 - 10, i, 20, a);
this.createWall("WallRight", this.gameArea.width / 2 + 10, i, 20, a);
}
removeLegacyWalls() {
if (this.gameArea) for (let e = this.gameArea.childrenCount - 1; e >= 0; e--) {
let t = this.gameArea.children[e];
if ("Wall" !== t.name) continue;
let a = t.getComponents(cc.PhysicsBoxCollider);
for (let e = 0; e < a.length; e++) a[e].enabled = !1;
let i = t.getComponent(cc.RigidBody);
i && (i.enabled = !1);
t.destroy();
}
}
createWall(e, t, a, i, o) {
let n = this.gameArea.getChildByName(e);
n || ((n = new cc.Node(e)).parent = this.gameArea);
n.setPosition(t, a);
(n.getComponent(cc.RigidBody) || n.addComponent(cc.RigidBody)).type = cc.RigidBodyType.Static;
let s = n.getComponent(cc.PhysicsBoxCollider) || n.addComponent(cc.PhysicsBoxCollider);
s.size = cc.size(i, o);
s.friction = 0;
s.restitution = 0;
s.apply();
}
createPreviewCoin() {
if (this.gameOver || !this.showNextBoo || this.pendingDeadLineFailCoin) return;
this.currentCoinValue || (this.currentCoinValue = this.getRandomDropValue());
this.nextCoinValue || (this.nextCoinValue = this.getRandomDropValue());
if (this.currentPreviewCoin && this.currentPreviewCoin.isValid) {
this.cancelCoinSpawnTween(this.currentPreviewCoin);
this.currentPreviewCoin.destroy();
}
let e = this.previewLine ? this.previewLine.y : 0;
this.currentPreviewCoin = this.createCoinNode(this.currentCoinValue, this.gameArea, cc.v3(this.currentPreviewX, e, 0), !0, !1);
this.saveBoardState();
}
createNextPreviewCoin() {
this.updateNextPreviewCoin();
}
resumePreviewCoin() {
this.showNextBoo = !0;
this.cancelDelayedPreviewCoin();
this.pendingDeadLineFailCoin || this.createPreviewCoin();
}
scheduleCreatePreviewCoin(e = this.nextPreviewCoinDelay) {
this.cancelDelayedPreviewCoin();
if (!this.gameOver && this.showNextBoo && !this.pendingDeadLineFailCoin) {
this.delayedPreviewCoinCallback || (this.delayedPreviewCoinCallback = this.onDelayedPreviewCoin.bind(this));
this.scheduleOnce(this.delayedPreviewCoinCallback, e);
}
}
onDelayedPreviewCoin() {
this.node && this.node.isValid && !this.gameOver && this.showNextBoo && !this.pendingDeadLineFailCoin && (this.currentPreviewCoin && this.currentPreviewCoin.isValid || this.createPreviewCoin());
}
cancelDelayedPreviewCoin() {
this.delayedPreviewCoinCallback && this.unschedule(this.delayedPreviewCoinCallback);
}
onEnable() {
this.mainSceneIdleTouchStartHandler || (this.mainSceneIdleTouchStartHandler = () => {
this.mainSceneIdleSeconds = 0;
if (this.guidHand && this.guidHand.isValid && this.guidHand.active) {
this.guidHand.active = !1;
this.guidHand.opacity = 255;
}
});
this.node.on(cc.Node.EventType.TOUCH_START, this.mainSceneIdleTouchStartHandler, this, !0);
this.node.on(cc.Node.EventType.TOUCH_START, this.onTouchStart, this);
this.node.on(cc.Node.EventType.TOUCH_MOVE, this.onTouchMove, this);
this.node.on(cc.Node.EventType.TOUCH_END, this.onTouchEnd, this);
this.node.on(cc.Node.EventType.TOUCH_CANCEL, this.onTouchEnd, this);
}
onDisable() {
this.mainSceneIdleTouchStartHandler && this.node.off(cc.Node.EventType.TOUCH_START, this.mainSceneIdleTouchStartHandler, this, !0);
this.node.off(cc.Node.EventType.TOUCH_START, this.onTouchStart, this);
this.node.off(cc.Node.EventType.TOUCH_MOVE, this.onTouchMove, this);
this.node.off(cc.Node.EventType.TOUCH_END, this.onTouchEnd, this);
this.node.off(cc.Node.EventType.TOUCH_CANCEL, this.onTouchEnd, this);
this.mainSceneIdleSeconds = 0;
if (this.guidHand && this.guidHand.isValid && this.guidHand.active) {
this.guidHand.active = !1;
this.guidHand.opacity = 255;
}
}
onTouchStart(e) {
this.mainSceneIdleSeconds = 0;
if (this.guidHand && this.guidHand.isValid && this.guidHand.active) {
this.guidHand.active = !1;
this.guidHand.opacity = 255;
}
this.gameOver || this.updatePreviewPosition(e);
}
onTouchMove(e) {
this.gameOver || this.updatePreviewPosition(e);
}
onTouchEnd(e) {
if (this.gameOver) return;
this.updatePreviewPosition(e);
if (!this.currentPreviewCoin || !this.currentPreviewCoin.isValid) return;
const t = r.default.getInstance().getData(l.default);
if (0 == t.guideStep) {
t.guideStep = 1;
m.default.close();
}
1 == this.canClick ? this.dropCoin() : this.pendingDropAfterSpawn = !0;
}
updatePreviewPosition(e) {
if (!this.gameArea || !e) return;
let t = this.getPreviewXByTouchEvent(e);
this.currentPreviewX = t;
if (this.currentPreviewCoin && this.currentPreviewCoin.isValid) {
this.currentPreviewCoin.x = t;
this.drawGuideLine();
this.savePreviewPositionState();
}
}
getPreviewXByTouchEvent(e) {
let t = this.gameArea.convertToNodeSpaceAR(e.getLocation()), a = this.getCurrentPreviewRadius(), i = -this.gameArea.width / 2 + a, o = this.gameArea.width / 2 - a;
return Math.max(i, Math.min(o, t.x));
}
getCurrentPreviewRadius() {
if (!this.currentPreviewCoin || !this.currentPreviewCoin.isValid) return 50;
let e = this.currentPreviewCoin.getComponent(u.default);
return e ? e.getRadius() : 50;
}
drawGuideLine() {
if (!this.guideLine) return;
if (!this.enableGuideLine || !this.isGuideLineVisible) {
this.guideLine.clear();
return;
}
if (!this.currentPreviewCoin || !this.groundLine) {
this.guideLine.clear();
return;
}
let e = this.currentPreviewCoin.getComponent(u.default);
if (!e) {
this.guideLine.clear();
return;
}
this.guideLine.clear();
let t = this.currentPreviewCoin.x, a = e.getRadius(), i = this.currentPreviewCoin.y - a, o = this.getGroundYInGameArea();
for (let e = 0; e < this.gameArea.childrenCount; e++) {
let n = this.gameArea.children[e];
if (n === this.currentPreviewCoin) continue;
let s = n.getComponent(u.default);
if (!s) continue;
if (s.isPreview || s.isMerging) continue;
let r = a + s.getRadius(), l = Math.abs(n.x - t);
if (l > r) continue;
let c = Math.sqrt(Math.max(0, r * r - l * l)), d = n.y + c - a;
d < i && d > o && (o = d);
}
if (i <= o) return;
let n = this.gameArea.convertToWorldSpaceAR(cc.v2(t, i)), s = this.gameArea.convertToWorldSpaceAR(cc.v2(t, o)), r = this.guideLine.node.convertToNodeSpaceAR(n), l = this.guideLine.node.convertToNodeSpaceAR(s);
this.guideLine.fillColor = cc.color(255, 255, 255, 180);
r.y, l.y;
let c = r.y;
for (;c > l.y; ) {
this.guideLine.circle(r.x, c, 5);
this.guideLine.fill();
c -= 30;
}
}
dropCoin() {
if (this.gameOver || !this.canClick) return;
if (this.pendingDeadLineFailCoin) return;
if (!this.currentPreviewCoin || !this.currentPreviewCoin.isValid) return;
c.default.playSound("sfx_bom");
this.guideLine && this.guideLine.clear();
let e = this.currentPreviewCoin, t = e.getComponent(u.default), a = 0;
if (t && this.gameArea && this.groundLine) {
let i = t.getRadius(), o = e.y - i, n = this.getGroundYInGameArea();
for (let t = 0; t < this.gameArea.childrenCount; t++) {
let a = this.gameArea.children[t];
if (!a || !a.isValid || !a.active || a === e) continue;
let s = a.getComponent(u.default);
if (!s || s.isPreview || s.isMerging) continue;
let r = i + s.getRadius(), l = Math.abs(a.x - e.x);
if (l > r) continue;
let c = Math.sqrt(Math.max(0, r * r - l * l)), d = a.y + c - i;
d < o && d > n && (n = d);
}
a = Math.max(0, o - n);
}
t && t.release(a);
const i = r.default.getInstance().getData(l.default);
i.dropCointimes++;
let o = I.default.globalData.thirdjumpRewardtimes;
i.windowsCointimes > o.thirdAndlookADDouble ? i.windowsCointimes = 0 : i.windowsCointimes++;
this.checkShowReward();
this.currentPreviewCoin = null;
this.advancePreviewQueue();
this.saveBoardState();
this.scheduleCreatePreviewCoin();
this.scheduleGameOverCheck(1);
}
scheduleGameOverCheck(e = .8) {
if (!this.gameOver) {
this.gameOverCheckCallback || (this.gameOverCheckCallback = this.onScheduledGameOverCheck.bind(this));
this.unschedule(this.gameOverCheckCallback);
this.pendingGameOverCheck = !0;
this.scheduleOnce(this.gameOverCheckCallback, e);
}
}
onScheduledGameOverCheck() {
this.pendingGameOverCheck = !1;
if (this.node && this.node.isValid && !this.gameOver && this.showNextBoo) {
this.refreshDeadLineOpacity();
this.checkGameOver();
} else this.clearPendingDeadLineFailCoin();
}
cancelGameOverCheck() {
this.gameOverCheckCallback && this.unschedule(this.gameOverCheckCallback);
this.pendingGameOverCheck = !1;
this.clearPendingDeadLineFailCoin();
}
clearPendingDeadLineFailCoin() {
this.pendingDeadLineFailCoin = null;
this.pendingDeadLineFailCoinStillElapsed = 0;
this.pendingDeadLineFailCoinElapsed = 0;
}
setPendingDeadLineFailCoin(e) {
if (e && e.isValid && this.pendingDeadLineFailCoin !== e) {
this.pendingDeadLineFailCoin = e;
this.pendingDeadLineFailCoinStillElapsed = 0;
this.pendingDeadLineFailCoinElapsed = 0;
this.canClick = !1;
this.pendingDropAfterSpawn = !1;
this.cancelDelayedPreviewCoin();
}
}
resumeAfterDeadLinePendingResolved() {
if (!this.gameOver && this.showNextBoo && !this.pendingDeadLineFailCoin) {
this.canClick = !0;
this.currentPreviewCoin && this.currentPreviewCoin.isValid || this.scheduleCreatePreviewCoin();
}
}
checkShowReward() {
if (this.gameOver) return;
let e = I.default.globalData.thirdjumpRewardtimes, t = [ e.first, e.second, e.thirdAndlookADDouble ];
const a = r.default.getInstance().getData(l.default);
let i = null;
for (let e = 0; e < t.length; e++) {
let o = t[e];
if (a.windowsCointimes == o) {
i = 0 == e ? a.dropCointimes == o ? () => {
b.default.open_with_parm({
showType: 3,
firstStrong: !0
});
} : () => {
b.default.open_with_parm({
showType: 3
});
} : 1 == e ? () => {
b.default.open_with_parm({
showType: 3
});
} : () => {
cc.sys.isMobile ? w.HWLshowAd("1_A", () => {
this.getADDouble(!0);
}, () => {
this.getADDouble(!1);
}) && this.resetWindowsCoinTimes() : this.getADDouble(!0);
};
break;
}
}
i && this.scheduleRewardPopup(i);
}
getADDouble(e) {
e && !this.gameOver && b.default.open_with_parm({
showType: 2
});
this.resetWindowsCoinTimes();
}
onGameHide() {
this.saveBoardState(!1);
this.resetWindowsCoinTimes(!0);
}
resetWindowsCoinTimes(e = !0) {
r.default.getInstance().getData(l.default).windowsCointimes = 0;
this.boardStateDirty = !0;
e && this.flushBoardStateToStorage();
}
updateScore(e = !0) {
this.updateProgressbar(e);
}
getCoinScriptForDeadLineCheck(e) {
if (!e || !e.isValid || !e.active) return null;
let t = e.getComponent(u.default);
return !t || t.isPreview || t.isMerging ? null : t;
}
isCoinAboveDeadLine(e) {
return !!e && !!e.node && !!this.deadLine && e.node.y >= this.deadLine.y;
}
isCoinStillForDeadLineCheck(e) {
let t = e && e.isValid ? e.getComponent(cc.RigidBody) : null;
if (!t || !t.enabled) return !0;
if (!t.awake) return !0;
let a = t.linearVelocity || cc.v2(0, 0);
return Math.abs(a.x) <= this.deadLineFailStillVelocityLimit && Math.abs(a.y) <= this.deadLineFailStillVelocityLimit && Math.abs(t.angularVelocity || 0) <= this.deadLineFailStillAngularVelocityLimit;
}
updatePendingDeadLineFailCoin(e) {
if (!this.pendingDeadLineFailCoin) return;
if (this.gameOver || h.default.ui_is_show("FailDialog") || !this.deadLine || !this.gameArea) {
this.clearPendingDeadLineFailCoin();
return;
}
if (!this.showNextBoo || w.HWL.adstart || this.pendingLuckDrawPopup || this.pendingRewardPopup) {
this.pendingDeadLineFailCoinStillElapsed = 0;
return;
}
let t = this.getCoinScriptForDeadLineCheck(this.pendingDeadLineFailCoin);
if (t && this.isCoinAboveDeadLine(t)) {
this.pendingDeadLineFailCoinElapsed += Math.max(0, e);
if (this.pendingDeadLineFailCoinElapsed >= this.deadLineFailMaxWaitDuration) {
let e = this.pendingDeadLineFailCoin;
this.clearPendingDeadLineFailCoin();
this.triggerGameOver(e);
} else if (this.isCoinStillForDeadLineCheck(this.pendingDeadLineFailCoin)) {
this.pendingDeadLineFailCoinStillElapsed += Math.max(0, e);
if (this.pendingDeadLineFailCoinStillElapsed >= this.deadLineFailStillConfirmDuration) {
let e = this.pendingDeadLineFailCoin;
this.clearPendingDeadLineFailCoin();
this.triggerGameOver(e);
}
} else this.pendingDeadLineFailCoinStillElapsed = 0;
} else {
this.clearPendingDeadLineFailCoin();
this.checkGameOver();
this.pendingDeadLineFailCoin || this.resumeAfterDeadLinePendingResolved();
}
}
checkGameOver() {
if (this.gameOver || h.default.ui_is_show("FailDialog")) return;
if (!this.deadLine || !this.gameArea) return;
let e = this.deadLine.y;
for (let t = 0; t < this.gameArea.childrenCount; t++) {
let a = this.gameArea.children[t], i = this.getCoinScriptForDeadLineCheck(a);
if (i && a.y >= e && this.isCoinStillForDeadLineCheck(a)) {
this.clearPendingDeadLineFailCoin();
this.triggerGameOver(a);
return;
}
i && a.y >= e && !this.pendingDeadLineFailCoin && this.setPendingDeadLineFailCoin(a);
}
}
triggerGameOver(e) {
if (this.gameOver || h.default.ui_is_show("FailDialog")) return;
this.gameOver = !0;
this.showNextBoo = !1;
this.canClick = !1;
this.pauseBoardCoinMergesForGameOver();
this.cancelGameOverCheck();
this.cancelDelayedPreviewCoin();
this.cancelPendingRewardAndLuckDrawPopups();
this.applyFailCoinRedTint(e);
this.forceShowFailBoardCoins();
this.guideLine && this.guideLine.clear();
if (this.currentPreviewCoin) {
this.cancelCoinSpawnTween(this.currentPreviewCoin);
this.currentPreviewCoin.destroy();
this.currentPreviewCoin = null;
}
const t = r.default.getInstance().getData(l.default);
t.roundScore = this.score;
t.savedCoinsScore = this.score;
t.savedDrawScore = g.gameUtils.getCurrentDrawScore();
this.score > t.histroyMaxScore && (t.histroyMaxScore = this.score);
this.saveBoardState(!0);
this.playFailBoardCoinAnimation(() => {
this.cancelPendingRewardAndLuckDrawPopups();
this.openFailDialog();
});
}
openFailDialog() {
p.default.open({
param: {
closeCallback: () => {
this.isResettingFromFailDialogClose = !0;
try {
this.resetGameAfterFailWithoutAd();
} finally {
this.isResettingFromFailDialogClose = !1;
}
}
}
});
}
cancelPendingRewardAndLuckDrawPopups() {
this.pendingLuckDrawPopup = !1;
this.pendingRewardPopup = !1;
let e = [ "RewardDialog", "LuckDrawDialog", "LuckyDrawRewardDialog" ];
for (let t = 0; t < e.length; t++) {
let a = e[t], i = h.default.all_ui[a];
i && i.node && i.node.isValid && i.unscheduleAllCallbacks();
h.default.close_ui(a);
h.default.ui_is_loading[a] = !1;
}
}
pauseBoardCoinMergesForGameOver() {
u.default.setMergePausedForGameOver(!0);
this.merge1024FlowToken++;
if (null !== this.mergeChainTimer) {
clearTimeout(this.mergeChainTimer);
this.mergeChainTimer = null;
this.mergeChainCount = 0;
this.isMergeChainActive = !1;
}
this.hideMergeComboUI();
if (this.merge1024Prefab && this.merge1024Prefab.node && this.merge1024Prefab.node.isValid) {
this.merge1024Prefab.setCompleteListener(null);
cc.Tween.stopAllByTarget(this.merge1024Prefab.node);
this.merge1024Prefab.node.active = !1;
}
if (this.gameArea) for (let e = 0; e < this.gameArea.childrenCount; e++) {
let t = this.gameArea.children[e];
if (!t || !t.isValid) continue;
let a = t.getComponent(u.default);
if (a && !a.isPreview) {
a.pauseMergeForGameOver();
cc.Tween.stopAllByTarget(t);
this.freezeFailCoinPhysics(t);
t.active = !0;
t.opacity = 255;
Math.abs(t.scaleX) <= .01 && (t.scaleX = 1);
Math.abs(t.scaleY) <= .01 && (t.scaleY = 1);
}
}
}
freezeFailCoinPhysics(e) {
if (!e || !e.isValid) return;
let t = e.getComponent(cc.RigidBody);
if (!t || !t.isValid) return;
for (let e = 0; e < this.failFrozenRigidBodyTargets.length; e++) if (this.failFrozenRigidBodyTargets[e].rigidBody == t) {
t.linearVelocity = cc.v2(0, 0);
t.angularVelocity = 0;
t.enabledContactListener = !1;
t.type = cc.RigidBodyType.Static;
t.enabled = !0;
return;
}
let a = t.linearVelocity || cc.v2(0, 0);
this.failFrozenRigidBodyTargets.push({
rigidBody: t,
enabled: t.enabled,
type: t.type,
linearVelocity: cc.v2(a.x, a.y),
angularVelocity: t.angularVelocity || 0,
enabledContactListener: t.enabledContactListener
});
t.linearVelocity = cc.v2(0, 0);
t.angularVelocity = 0;
t.enabledContactListener = !1;
t.type = cc.RigidBodyType.Static;
t.enabled = !0;
}
restoreFailFrozenCoinPhysics() {
for (let e = 0; e < this.failFrozenRigidBodyTargets.length; e++) {
let t = this.failFrozenRigidBodyTargets[e];
if (t && t.rigidBody && t.rigidBody.isValid) {
t.rigidBody.type = t.type;
t.rigidBody.enabled = t.enabled;
t.rigidBody.enabledContactListener = t.enabledContactListener;
t.rigidBody.linearVelocity = cc.v2(0, 0);
t.rigidBody.angularVelocity = 0;
}
}
this.failFrozenRigidBodyTargets = [];
}
forceShowFailBoardCoins() {
if (this.gameArea) for (let e = 0; e < this.gameArea.childrenCount; e++) {
let t = this.gameArea.children[e];
if (!t || !t.isValid) continue;
let a = t.getComponent(u.default);
if (a && !a.isPreview) {
this.gameOver && this.freezeFailCoinPhysics(t);
t.active = !0;
t.opacity = 255;
Math.abs(t.scaleX) <= .01 && (t.scaleX = 1);
Math.abs(t.scaleY) <= .01 && (t.scaleY = 1);
}
}
}
applyFailCoinRedTint(e) {
this.restoreFailCoinTint();
if (!e || !e.isValid) return;
let t = new cc.Color(255, 75, 75, 255), a = e.getComponentsInChildren(cc.Sprite);
if (!a || a.length <= 0) this.addFailCoinColorTarget(e, t); else for (let e = 0; e < a.length; e++) {
let i = a[e];
i && i.node && i.node.isValid && this.addFailCoinColorTarget(i.node, t);
}
}
addFailCoinColorTarget(e, t) {
if (!e || !e.isValid) return;
for (let t = 0; t < this.failCoinColorTargets.length; t++) if (this.failCoinColorTargets[t].node == e) return;
let a = e.color || cc.Color.WHITE;
this.failCoinColorTargets.push({
node: e,
color: new cc.Color(a.r, a.g, a.b, a.a)
});
e.color = new cc.Color(t.r, t.g, t.b, t.a);
}
restoreFailCoinTint() {
for (let e = 0; e < this.failCoinColorTargets.length; e++) {
let t = this.failCoinColorTargets[e];
t && t.node && t.node.isValid && (t.node.color = t.color);
}
this.failCoinColorTargets = [];
}
playFailBoardCoinAnimation(e) {
let t = this.getBoardCoinNodes();
if (t.length <= 0) {
e && e();
return;
}
let a = 0, i = !1, o = [], n = () => {
if (!i) {
i = !0;
for (let e = 0; e < o.length; e++) o[e] && o[e]();
this.forceShowFailBoardCoins();
e && e();
}
};
for (let e = 0; e < t.length; e++) {
let s = t[e], r = Math.abs(s.scaleX) > .01 ? s.scaleX : 1, l = Math.abs(s.scaleY) > .01 ? s.scaleY : 1, c = () => {
if (s && s.isValid) {
s.active = !0;
s.opacity = 255;
s.scaleX = r;
s.scaleY = l;
}
};
o.push(c);
s.getComponent(cc.RigidBody) && this.freezeFailCoinPhysics(s);
s.active = !0;
s.opacity = 255;
cc.Tween.stopAllByTarget(s);
cc.tween(s).delay(e % 6 * .04).to(.14, {
scaleX: 1.16 * r,
scaleY: .8 * l
}, {
easing: "sineOut"
}).to(.14, {
scaleX: .9 * r,
scaleY: 1.14 * l
}, {
easing: "sineInOut"
}).to(.12, {
scaleX: 1.1 * r,
scaleY: .88 * l
}, {
easing: "sineInOut"
}).to(.12, {
scaleX: .94 * r,
scaleY: 1.08 * l
}, {
easing: "sineInOut"
}).to(.12, {
scaleX: 1.04 * r,
scaleY: .96 * l
}, {
easing: "sineInOut"
}).to(.18, {
scaleX: r,
scaleY: l
}, {
easing: cc.easing.backOut
}).call(() => {
c();
i || ++a >= t.length && n();
}).start();
}
this.scheduleOnce(() => {
n();
}, this.failCoinAnimDuration + .25);
}
getBoardCoinNodes() {
let e = [];
if (!this.gameArea) return e;
for (let t = 0; t < this.gameArea.childrenCount; t++) {
let a = this.gameArea.children[t];
if (!a || !a.isValid || !a.active) continue;
let i = a.getComponent(u.default);
i && !i.isPreview && e.push(a);
}
return e;
}
restart() {
this.skipSaveOnDestroy = !0;
this.clearBoardState();
cc.director.loadScene(cc.director.getScene().name);
}
};
R._instance = null;
R.skipInitialBottomCoinsOnce = !1;
o([ s(cc.Prefab) ], R.prototype, "coinPrefab", void 0);
o([ s(cc.Prefab) ], R.prototype, "mergeEffectPrefab", void 0);
o([ s(cc.Node) ], R.prototype, "gameArea", void 0);
o([ s(cc.Node) ], R.prototype, "groundLine", void 0);
o([ s(cc.Label) ], R.prototype, "fakeMoneyTxt", void 0);
o([ s(cc.Sprite) ], R.prototype, "nextPreviewCoin", void 0);
o([ s(cc.Label) ], R.prototype, "nextPreviewCoinLabel", void 0);
o([ s(cc.Node) ], R.prototype, "deadLine", void 0);
o([ s([ cc.SpriteFrame ]) ], R.prototype, "coinSprites", void 0);
o([ s(cc.Node) ], R.prototype, "previewLine", void 0);
o([ s(cc.Graphics) ], R.prototype, "guideLine", void 0);
o([ s(cc.Node) ], R.prototype, "guidHand", void 0);
o([ s ], R.prototype, "enableGuideLine", void 0);
o([ s(cc.Node) ], R.prototype, "btnSetting", void 0);
o([ s(cc.Node) ], R.prototype, "btnTask", void 0);
o([ s(cc.Node) ], R.prototype, "btnRule", void 0);
o([ s(cc.Node) ], R.prototype, "btnGetMoney", void 0);
o([ s(cc.Node) ], R.prototype, "btnDraw", void 0);
o([ s(cc.Node) ], R.prototype, "btnFakeMoney", void 0);
o([ s(cc.Node) ], R.prototype, "flyFakePlusMoney", void 0);
o([ s(cc.Node) ], R.prototype, "btnDebug", void 0);
o([ s(cc.Node) ], R.prototype, "bubble", void 0);
o([ s(cc.ProgressBar) ], R.prototype, "progressBarDraw", void 0);
o([ s(cc.Label) ], R.prototype, "progressBarLabel", void 0);
o([ s(cc.Label) ], R.prototype, "progressBarDrawLabel", void 0);
o([ s(cc.Label) ], R.prototype, "fakeMoneyLabel", void 0);
o([ s(cc.Label) ], R.prototype, "bubbleLabel", void 0);
o([ s(cc.Label) ], R.prototype, "btnDrawLabel", void 0);
o([ s(cc.Label) ], R.prototype, "numberLabel1024", void 0);
o([ s([ cc.SpriteFrame ]) ], R.prototype, "MergeIcon", void 0);
o([ s(cc.Node) ], R.prototype, "combo", void 0);
o([ s([ cc.SpriteFrame ]) ], R.prototype, "MergeTimes", void 0);
o([ s(cc.Sprite) ], R.prototype, "currentMergeIcon", void 0);
o([ s(cc.Sprite) ], R.prototype, "currentMergeTimes", void 0);
o([ s(sp.Skeleton) ], R.prototype, "merge1024Prefab", void 0);
o([ s(cc.Node) ], R.prototype, "getMoneyTipNode", void 0);
o([ s(cc.Node) ], R.prototype, "star", void 0);
R = i = o([ n ], R);
a.default = R;
cc._RF.pop();
};
