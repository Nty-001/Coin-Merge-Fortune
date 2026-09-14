// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: HtmlBtn; dependency map: {"../BaseUIManager/Storage/TouchButton":"TouchButton","../BaseUIManager/UIConfig":"UIConfig","../BaseUIManager/UIManagerNew":"UIManagerNew","../HWL/ServerConfig":"ServerConfig"}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "108fc2cdhJJNZGMIttjUfh+", "HtmlBtn");
var i, o = this && this.__decorate || function(e, t, a, i) {
var o, n = arguments.length, s = n < 3 ? t : null === i ? i = Object.getOwnPropertyDescriptor(t, a) : i;
if ("object" == typeof Reflect && "function" == typeof Reflect.decorate) s = Reflect.decorate(e, t, a, i); else for (var r = e.length - 1; r >= 0; r--) (o = e[r]) && (s = (n < 3 ? o(s) : n > 3 ? o(t, a, s) : o(t, a)) || s);
return n > 3 && s && Object.defineProperty(t, a, s), s;
};
Object.defineProperty(a, "__esModule", {
value: !0
});
const n = e("../BaseUIManager/Storage/TouchButton"), s = e("../BaseUIManager/UIConfig"), r = e("../BaseUIManager/UIManagerNew"), l = e("../HWL/ServerConfig"), {ccclass: c, property: d} = cc._decorator;
let h = i = class extends cc.Component {
constructor() {
super(...arguments);
this.displayRequestSerial = 0;
}
displayNode() {
let e = ++this.displayRequestSerial;
this.resetTaskEntryState();
l.HWLServerConfig.getClientId() ? l.HWLServerConfig.device_model ? l.HWLServerConfig.sendHTMLOfferLinkList().then(t => {
if (e != this.displayRequestSerial || !this.node || !this.node.isValid) return;
let a = null;
try {
a = JSON.parse(t);
} catch (e) {
console.error("HtmlBtn JSON解析失败", e, t);
return;
}
if (0 === Number(a.code)) {
console.log("HtmlBtn 操作成功", a.msg);
let e = a.data && Array.isArray(a.data.tasks) ? a.data.tasks : [];
if (e.length <= 0) {
console.log("HtmlBtn 没有任务信息");
this.resetTaskEntryState();
return;
}
console.log("HtmlBtn 存在任务信息");
let t = !1, i = !1;
for (let a = 0; a < e.length; a++) {
let o = Number(e[a].status) || 0;
2 != o && (t = !0);
1 == o && (i = !0);
}
console.log("HtmlBtn 是否还有未领取任务" + t);
this.resetTaskEntryState(t, i);
t && this.startHandTween();
} else {
console.error("操作失败:", a.msg);
this.resetTaskEntryState();
}
}).catch(e => {
console.error("HtmlBtn 请求失败", e);
this.resetTaskEntryState();
}) : console.log("获取不到设备型号 ") : console.log("获取不到clientId ");
}
resetTaskEntryState(e = !1, t = !1) {
this.stopHandTween();
this.rwBtn && (this.rwBtn.active = e);
this.red && (this.red.active = t);
this.hand && (this.hand.active = !1);
}
startHandTween() {
if (this.hand) {
this.stopHandTween();
cc.tween(this.hand).delay(30).call(() => {
this.hand.active = !0;
}).delay(5).call(() => {
this.hand.active = !1;
}).union().repeatForever().start();
}
}
stopHandTween() {
this.hand && cc.Tween.stopAllByTarget(this.hand);
}
start() {
i.instance = this;
this.resetTaskEntryState();
(this.rwBtn.getComponent(n.default) || this.rwBtn.addComponent(n.default)).registerTouchEvent(() => {
const e = {
ui_config_path: s.default.HtmlDialog,
ui_config_name: "HtmlDialog",
param: {}
};
r.default.show_ui(e);
});
this.scheduleOnce(() => {
this.displayNode();
}, 1.5);
}
onDestroy() {
this.stopHandTween();
i.instance == this && (i.instance = null);
}
};
h.instance = null;
o([ d(cc.Node) ], h.prototype, "rwBtn", void 0);
o([ d(cc.Node) ], h.prototype, "hand", void 0);
o([ d(cc.Node) ], h.prototype, "red", void 0);
h = i = o([ c ], h);
a.default = h;
cc._RF.pop();
};
