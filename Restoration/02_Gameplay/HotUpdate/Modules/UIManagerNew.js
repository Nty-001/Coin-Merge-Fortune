// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: UIManagerNew; dependency map: {"./UIConfig":"UIConfig"}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "df2d0wgwvRPRq85p47v+kIp", "UIManagerNew");
Object.defineProperty(a, "__esModule", {
value: !0
});
const i = e("./UIConfig");
class o {
static pruneInvalidUI() {
const e = Object.keys(this.all_ui);
for (const t of e) {
const e = this.all_ui[t];
if (!e || !e.node || !cc.isValid(e.node)) {
delete this.all_ui[t];
delete this.ui_is_loading[t];
}
}
}
static init() {}
static show_ui(e) {
console.log("显示界面的参数 = ", e.ui_config_name);
this.pruneInvalidUI();
if (!this.ui_is_loading[e.ui_config_name]) {
this.ui_is_loading[e.ui_config_name] = !0;
if (this.all_ui[e.ui_config_name]) {
console.log("显示界面的参数 = else ", e);
const t = this.all_ui[e.ui_config_name];
if (!t || !t.node || !cc.isValid(t.node)) {
delete this.all_ui[e.ui_config_name];
this.ui_is_loading[e.ui_config_name] = !1;
this.show_ui(e);
return;
}
t.show(e);
t.controller = e.controller;
this.ui_is_loading[e.ui_config_name] = !1;
t.node.zIndex = this.max_zindex() + 1;
e.complete_callback && e.complete_callback(t);
t.onAddFinished();
} else {
console.log("显示界面的参数 = if ", e);
cc.resources.load(e.ui_config_path, cc.Prefab, (t, a) => {
var i;
if (t) {
this.ui_is_loading[e.ui_config_name] = !1;
console.error(`当前显示的UI: ${null !== (i = e.ui_config_path) && void 0 !== i ? i : e.ui_config_name} 没有加载成功`);
e.complete_callback && e.complete_callback(null);
} else {
const t = cc.director.getScene();
if (!t || !cc.isValid(t)) {
this.ui_is_loading[e.ui_config_name] = !1;
e.complete_callback && e.complete_callback(null);
return;
}
const i = cc.instantiate(a), o = i.getComponent(e.ui_config_name);
if (!o) {
this.ui_is_loading[e.ui_config_name] = !1;
console.error(`当前显示的UI: ${e.ui_config_name} 没有挂载对应脚本`);
i.destroy();
e.complete_callback && e.complete_callback(null);
return;
}
o.show(e);
o.controller = e.controller;
o.controller && (o.controller.view = o);
this.all_ui[e.ui_config_name] = o;
this.ui_is_loading[e.ui_config_name] = !1;
t.addChild(i, this.max_zindex() + 1);
e.complete_callback && e.complete_callback(o);
o.onAddFinished();
}
});
}
}
}
static showStackView() {
if (this.stackList.length > 0) {
let e = this.stackList[0];
this.show_ui(e);
}
}
static popStackView(e) {
if (this.stackList.length > 0) {
let t = !1;
for (let a = this.stackList.length - 1; a >= 0; a--) if (this.stackList[a] == e) {
this.stackList.splice(a, 1);
t = !0;
break;
}
this.showStackView();
}
}
static max_zindex() {
this.pruneInvalidUI();
const e = Object.keys(this.all_ui);
let t = 0;
for (const a of e) {
const e = this.all_ui[a];
if (!e || !e.node || !cc.isValid(e.node)) continue;
const i = e.node;
i.zIndex > t && (t = i.zIndex);
}
return t;
}
static close_ui(e) {
const t = this.all_ui[e];
t && t.node && cc.isValid(t.node) && t.hide();
}
static clear_ui() {
const e = Object.keys(this.all_ui);
for (const t of e) {
const e = this.all_ui[t].node;
cc.isValid(e) && e.destroy();
}
this.all_ui = {};
this.ui_is_loading = {};
}
static clear_ui_cache() {
this.all_ui = {};
this.ui_is_loading = {};
this.stackList = [];
}
static show_toast(e) {
e.animation = !0;
const t = {
ui_config_path: i.default.Toast,
ui_config_name: "Toast",
param: e
};
o.show_ui(t);
}
static close_all_views() {
for (let e in this.all_ui) {
const t = this.all_ui[e];
t && t.node && cc.isValid(t.node) && t.hide();
}
}
static ui_is_show(e) {
const t = o.all_ui[e];
return !!(t && t.node && cc.isValid(t.node)) && t.node.active;
}
static preloadUIComponents() {
cc.resources.load("GameDialog/WinRewardView", cc.Prefab, (e, t) => {
if (e) console.error("预加载WinRewardView失败:", e); else {
const e = cc.instantiate(t), a = e.getComponent("WinRewardView");
if (a) {
this.all_ui.WinRewardView = a;
e.active = !1;
cc.director.getScene().addChild(e);
}
}
});
cc.resources.load("GameDialog/GameWin", cc.Prefab, (e, t) => {
if (e) console.error("预加载GameWin失败:", e); else {
const e = cc.instantiate(t), a = e.getComponent("GameWin");
if (a) {
this.all_ui.GameWin = a;
e.active = !1;
cc.director.getScene().addChild(e);
}
}
});
}
}
o.all_ui = {};
o.ui_is_loading = {};
o.stackList = [];
a.default = o;
cc._RF.pop();
};
