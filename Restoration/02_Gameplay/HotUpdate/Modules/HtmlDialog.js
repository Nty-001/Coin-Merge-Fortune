// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: HtmlDialog; dependency map: {"../BaseUIManager/BaseUI":"BaseUI","../BaseUIManager/Storage/TouchButton":"TouchButton","../BaseUIManager/UIManagerNew":"UIManagerNew","../HWL/ServerConfig":"ServerConfig","../LanguageControl/Lab":"Lab","./HtmlItem":"HtmlItem"}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "8f0dfVMes9ChpnVds/91H8a", "HtmlDialog");
var i = this && this.__decorate || function(e, t, a, i) {
var o, n = arguments.length, s = n < 3 ? t : null === i ? i = Object.getOwnPropertyDescriptor(t, a) : i;
if ("object" == typeof Reflect && "function" == typeof Reflect.decorate) s = Reflect.decorate(e, t, a, i); else for (var r = e.length - 1; r >= 0; r--) (o = e[r]) && (s = (n < 3 ? o(s) : n > 3 ? o(t, a, s) : o(t, a)) || s);
return n > 3 && s && Object.defineProperty(t, a, s), s;
};
Object.defineProperty(a, "__esModule", {
value: !0
});
const o = e("../BaseUIManager/BaseUI"), n = e("../BaseUIManager/Storage/TouchButton"), s = e("../BaseUIManager/UIManagerNew"), r = e("../HWL/ServerConfig"), l = e("../LanguageControl/Lab"), c = e("./HtmlItem"), {ccclass: d, property: h} = cc._decorator;
let u = class extends o.default {
constructor() {
super(...arguments);
this.requestSerial = 0;
this.isClosing = !1;
}
onLoad() {
super.onLoad();
this.titleLabel.string = l.default.getlab("47");
this.tipsLabel.string = l.default.getlab("48");
(this.closeBtn.getComponent(n.default) || this.closeBtn.addComponent(n.default)).registerTouchEvent(() => {
this.closeDialog();
});
}
show(e) {
super.show(e);
this.isClosing = !1;
this.clearTasks();
let t = ++this.requestSerial;
r.HWLServerConfig.getClientId() ? r.HWLServerConfig.device_model ? r.HWLServerConfig.sendHTMLOfferLinkList().then(e => {
if (!this.isCurrentRequestValid(t)) return;
let a = null;
try {
a = JSON.parse(e);
} catch (t) {
console.error("HtmlDialog JSON解析失败", t, e);
return;
}
if (0 === Number(a.code)) {
console.log("HtmlDialog 操作成功", a.msg);
let e = a.data && Array.isArray(a.data.tasks) ? a.data.tasks : [];
if (e.length <= 0) {
console.log("HtmlDialog 没有任务信息");
return;
}
console.log("HtmlDialog 存在任务信息");
for (let a = 0; a < e.length; a++) {
if (!this.isCurrentRequestValid(t)) return;
let i = cc.instantiate(this.htmlPrefab);
i.parent = this.tasksNode;
let o = i.getComponent(c.default);
o && o.init(a, e[a]);
}
} else console.error("HtmlDialog 操作失败:", a.msg);
}).catch(e => {
this.isCurrentRequestValid(t) && console.error("HtmlDialog 请求失败", e);
}) : console.log("获取不到设备型号 ") : console.log("获取不到clientId ");
}
closeDialog() {
this.isClosing = !0;
this.requestSerial++;
this.clearTasks();
s.default.close_ui("HtmlDialog");
}
hide() {
this.isClosing = !0;
this.requestSerial++;
this.clearTasks();
super.hide();
}
clearTasks() {
if (!this.tasksNode || !cc.isValid(this.tasksNode)) return;
const e = [ ...this.tasksNode.children ];
for (const t of e) if (t && cc.isValid(t)) {
t.removeFromParent();
t.destroy();
}
}
isCurrentRequestValid(e) {
return e == this.requestSerial && !this.isClosing && !!this.node && cc.isValid(this.node) && this.node.active && !!this.tasksNode && cc.isValid(this.tasksNode);
}
};
i([ h(cc.Label) ], u.prototype, "titleLabel", void 0);
i([ h(cc.Label) ], u.prototype, "tipsLabel", void 0);
i([ h(cc.Node) ], u.prototype, "closeBtn", void 0);
i([ h(cc.Node) ], u.prototype, "tasksNode", void 0);
i([ h(cc.Prefab) ], u.prototype, "htmlPrefab", void 0);
u = i([ o.registerUIPath("GameDialog/HtmlDialog"), d ], u);
a.default = u;
cc._RF.pop();
};
