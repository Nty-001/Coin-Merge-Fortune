// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: HtmlItem; dependency map: {"../BaseUIManager/Storage/GameLocalData":"GameLocalData","../BaseUIManager/Storage/PlayData":"PlayData","../BaseUIManager/Storage/SoundManager":"SoundManager","../BaseUIManager/Storage/TouchButton":"TouchButton","../BaseUIManager/UIManagerNew":"UIManagerNew","../GameScene":"GameScene","../HWL/ServerConfig":"ServerConfig","../LanguageControl/Lab":"Lab","../Report/NativeCall":"NativeCall","../common/GameUtils":"GameUtils","./HtmlBtn":"HtmlBtn"}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "10541TTvkdNCYC011er+44E", "HtmlItem");
var i = this && this.__decorate || function(e, t, a, i) {
var o, n = arguments.length, s = n < 3 ? t : null === i ? i = Object.getOwnPropertyDescriptor(t, a) : i;
if ("object" == typeof Reflect && "function" == typeof Reflect.decorate) s = Reflect.decorate(e, t, a, i); else for (var r = e.length - 1; r >= 0; r--) (o = e[r]) && (s = (n < 3 ? o(s) : n > 3 ? o(t, a, s) : o(t, a)) || s);
return n > 3 && s && Object.defineProperty(t, a, s), s;
};
Object.defineProperty(a, "__esModule", {
value: !0
});
const o = e("../BaseUIManager/Storage/GameLocalData"), n = e("../BaseUIManager/Storage/PlayData"), s = e("../BaseUIManager/Storage/SoundManager"), r = e("../BaseUIManager/Storage/TouchButton"), l = e("../BaseUIManager/UIManagerNew"), c = e("../common/GameUtils"), d = e("../GameScene"), h = e("../HWL/ServerConfig"), u = e("../LanguageControl/Lab"), g = e("../Report/NativeCall"), p = e("./HtmlBtn"), {ccclass: f, property: m} = cc._decorator;
let y = class extends cc.Component {
constructor() {
super(...arguments);
this.linkId = "";
this.linkUrl = "";
this.status = 0;
this.isRequesting = !1;
this.btn1TouchButton = null;
this.btn2TouchButton = null;
}
onLoad() {
let e = this.btn1.node.parent, t = this.btn2.node.parent;
this.btn1TouchButton = e.getComponent(r.default) || e.addComponent(r.default);
this.btn2TouchButton = t.getComponent(r.default) || t.addComponent(r.default);
this.btn1TouchButton.registerTouchEvent(() => {
this.clickBtn();
});
this.btn2TouchButton.registerTouchEvent(() => {
this.clickBtn();
});
}
onDisable() {
this.setRequesting(!1);
}
init(e, t) {
t = t || {};
this.taskLabel.string = t.title || t.name || u.default.getlab("52");
this.btn1.string = u.default.getlab("49");
this.btn2.string = u.default.getlab("50");
this.btn3.string = u.default.getlab("51");
this.linkId = this.getTaskString(t, "linkId", "link_id", "id");
this.linkUrl = this.getTaskString(t, "linkUrl", "link_url", "url");
this.status = this.normalizeStatus(t.status);
this.setRequesting(!1);
this.initStatus(this.status);
}
initStatus(e) {
let t = this.normalizeStatus(e).toString();
for (let e = 0; e < this.btns.childrenCount; e++) {
let a = this.btns.children[e];
a.active = a.name == t;
}
if (!this.btns.getChildByName(t)) {
let e = this.btns.getChildByName("0");
e && (e.active = !0);
}
}
clickBtn() {
this.isRequesting || (h.HWLServerConfig.getClientId() ? h.HWLServerConfig.device_model ? 0 == this.status ? this.clickGoTask() : 1 == this.status ? this.claimReward() : console.log("已完成 无反应") : console.log("获取不到设备型号 ") : console.log("获取不到clientId "));
}
clickGoTask() {
if (!this.linkId) {
console.log("HtmlItem linkId为空");
return;
}
if (!this.linkUrl) {
console.log("HtmlItem linkUrl为空");
return;
}
console.log("HtmlItem 跳转");
let e = this.linkId, t = h.HWLServerConfig.generateClickId();
h.HWLServerConfig.linkId = e;
h.HWLServerConfig.clickID = t;
let a = this.linkUrl.indexOf("{click_id}") >= 0 ? this.linkUrl.split("{click_id}").join(t) : this.linkUrl;
console.log("HtmlItem this.status == 0跳转链接", a, t);
g.default.openUrl(a);
this.setRequesting(!0);
h.HWLServerConfig.sendHTMLOffLinkClick(e, t).then(e => {
if (!this.isNodeValid()) return;
this.setRequesting(!1);
let t = this.parseResponse(e);
if (t) if (0 === Number(t.code)) {
console.log("this.status == 0 HtmlItem 操作成功", t.msg);
l.default.close_ui("HtmlDialog");
} else console.error("HtmlItem 操作失败:", t.msg);
}).catch(e => {
if (this.isNodeValid()) {
this.setRequesting(!1);
console.error("HtmlItem 点击任务请求失败", e);
}
});
}
claimReward() {
if (!this.linkId) {
console.log("HtmlItem linkId为空");
return;
}
console.log("HtmlItem this.status == 1上报并得到奖励");
this.setRequesting(!0);
let e = this.linkId;
h.HWLServerConfig.linkId = e;
h.HWLServerConfig.sendHTMLOffLinkClaimReward(e).then(e => {
if (!this.isNodeValid()) return;
this.setRequesting(!1);
let t = this.parseResponse(e);
if (t) if (0 === Number(t.code)) {
console.log("HtmlItem this.status == 1 操作成功", t.msg);
this.giveTaskReward();
this.status = 2;
this.initStatus(this.status);
p.default.instance && p.default.instance.displayNode();
d.default.instance && d.default.instance.setTaskBtnShowOrHide();
l.default.close_ui("HtmlDialog");
} else console.error("HtmlItem 操作失败:", t.msg);
}).catch(e => {
if (this.isNodeValid()) {
this.setRequesting(!1);
console.error("HtmlItem 领取任务奖励请求失败", e);
}
});
}
giveTaskReward() {
let e = Math.max(0, Number(c.gameUtils.calculateCashReward()) || 0);
if (!(e <= 0)) {
o.default.getInstance().getData(n.default).fakeMoney += e;
o.default.getInstance().set_local_storeage();
if (d.default.instance && d.default.instance.node && d.default.instance.node.isValid) {
d.default.instance.setFakeMoney();
this.flyMoneyToGameScene(this.getRewardIconNode(), e);
}
}
}
flyMoneyToGameScene(e, t) {
const a = d.default.instance;
if (!(e && e.isValid && e.parent && a && a.node && a.node.isValid)) return;
const i = a.getFakeMoneyIconNode();
if (!i || !i.isValid || !i.parent) return;
const o = cc.director.getScene();
if (!o) return;
const n = e.parent.convertToWorldSpaceAR(e.position), r = i.parent.convertToWorldSpaceAR(i.position), l = o.convertToNodeSpaceAR(n), c = o.convertToNodeSpaceAR(r), h = cc.v2(c.x - l.x, c.y - l.y).mag(), u = Math.min(h / 650, .6), g = e.scale || 1;
s.default.playSound("collect");
for (let n = 0; n < 3; n++) {
const s = cc.instantiate(e);
s.active = !0;
o.addChild(s, 99999);
s.position = l;
s.scale = g;
cc.tween(s).delay(.03 * n).to(u, {
position: c,
scale: .35 * g
}).call(() => {
i && i.isValid && cc.tween(i).to(.3, {
scale: 1.1
}).to(.1, {
scale: 1
}).start();
2 == n && a.playFlyFakePlusMoney(t);
s.destroy();
}).start();
}
}
getRewardIconNode() {
return this.node.getChildByName("coinspr") || this.node;
}
normalizeStatus(e) {
let t = Number(e);
return 1 == t || 2 == t ? t : 0;
}
getTaskString(e, ...t) {
for (let a = 0; a < t.length; a++) {
let i = e[t[a]];
if (null != i && "" !== i) return i.toString();
}
return "";
}
setRequesting(e) {
if (this.isNodeValid()) {
this.isRequesting = e;
this.btn1TouchButton && (this.btn1TouchButton.canTouch = !e);
this.btn2TouchButton && (this.btn2TouchButton.canTouch = !e);
this.node.opacity = e ? 180 : 255;
}
}
parseResponse(e) {
try {
return JSON.parse(e);
} catch (t) {
console.error("HtmlItem JSON解析失败", t, e);
return null;
}
}
isNodeValid() {
return !!this.node && cc.isValid(this.node);
}
start() {}
};
i([ m(cc.Label) ], y.prototype, "btn1", void 0);
i([ m(cc.Label) ], y.prototype, "btn2", void 0);
i([ m(cc.Label) ], y.prototype, "btn3", void 0);
i([ m(cc.Node) ], y.prototype, "btns", void 0);
i([ m(cc.Label) ], y.prototype, "taskLabel", void 0);
y = i([ f ], y);
a.default = y;
cc._RF.pop();
};
