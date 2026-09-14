// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/base/assets/assets/main/index.ba75b.js
// Module: UIManager; dependency map: {"./BaseUI":"BaseUI"}
module.exports = function(e, t, o) {
"use strict";
cc._RF.push(t, "9e0af7F7JNO6INe6cvPEm7f", "UIManager");
Object.defineProperty(o, "__esModule", {
value: !0
});
const a = e("./BaseUI");
class s {
constructor() {
this.all_ui = {};
this.ui_is_loading = {};
}
static get instance() {
s._instance || (s._instance = new s());
return s._instance;
}
show_ui(e) {
return new Promise(t => {
if (!this.ui_is_loading[e.ui_name]) {
this.ui_is_loading[e.ui_name] = !0;
if (this.all_ui[e.ui_name]) {
this.all_ui[e.ui_name].onShowUI(e);
this.ui_is_loading[e.ui_name] = !1;
this.all_ui[e.ui_name].node.zIndex = this.max_zindex() + 1;
t();
} else cc.resources.load(e.ui_path, cc.Prefab, (o, a) => {
if (o) {
this.ui_is_loading[e.ui_name] = !1;
t();
} else {
const o = cc.instantiate(a), s = o.getComponent(e.ui_name);
s.onShowUI(e);
this.all_ui[e.ui_name] = s;
this.ui_is_loading[e.ui_name] = !1;
cc.director.getScene().addChild(o, this.max_zindex() + 1);
t();
}
});
}
});
}
max_zindex() {
const e = Object.keys(this.all_ui);
let t = 0;
for (const o of e) {
const e = this.all_ui[o].node;
e.zIndex > t && (t = e.zIndex);
}
return t;
}
close_ui(e) {
this.all_ui[e] && this.all_ui[e].onClose();
}
close_all_views() {
for (let e in this.all_ui) this.all_ui[e].onClose();
}
clear_ui() {
const e = Object.keys(this.all_ui);
for (const t of e) {
const e = this.all_ui[t].node;
cc.isValid(e) && e.destroy();
}
this.all_ui = {};
this.ui_is_loading = {};
}
ui_is_show(e) {
return !!this.all_ui[e] && this.all_ui[e].node.active;
}
onShowToast(e) {
const t = {
ui_path: a.UIConfig.ToastView,
ui_name: "ToastView",
param: e
};
this.show_ui(t);
}
onShowView(e, t) {
let o = e.split("/").pop();
const a = {
ui_path: e,
ui_name: o,
param: t
};
this.show_ui(a);
}
}
o.default = s;
s._instance = null;
cc._RF.pop();
};
