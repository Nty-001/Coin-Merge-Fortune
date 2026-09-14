// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: BaseUI; dependency map: {"./EventListener/EventManager":"EventManager","./EventListener/LinkScript":"LinkScript","./UIConfig":"UIConfig","./UIManagerNew":"UIManagerNew"}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "1d695x/0PtFTrOwiZHv/H6J", "BaseUI");
var i = this && this.__decorate || function(e, t, a, i) {
var o, n = arguments.length, s = n < 3 ? t : null === i ? i = Object.getOwnPropertyDescriptor(t, a) : i;
if ("object" == typeof Reflect && "function" == typeof Reflect.decorate) s = Reflect.decorate(e, t, a, i); else for (var r = e.length - 1; r >= 0; r--) (o = e[r]) && (s = (n < 3 ? o(s) : n > 3 ? o(t, a, s) : o(t, a)) || s);
return n > 3 && s && Object.defineProperty(t, a, s), s;
};
Object.defineProperty(a, "__esModule", {
value: !0
});
a.registerUIPath = void 0;
const o = e("./EventListener/LinkScript"), n = e("./UIConfig"), s = e("./UIManagerNew"), r = e("./EventListener/EventManager"), {ccclass: l, property: c} = cc._decorator;
a.registerUIPath = function(e) {
return function(t) {
let a = e.substring(e.lastIndexOf("/") + 1);
n.default[a] ? console.error(`prefab${a} 的路径已经存在 ${n.default[a]}`) : n.default[a] = e;
t.__UIName = a;
};
};
let d = class extends cc.Component {
constructor() {
super(...arguments);
this.contentNode = null;
this.forceRefitAfterShow = !1;
this.hideOnTouchOutBoundContainer = !1;
this.block_input_events = null;
this.ui_param_interface = null;
this.controller = null;
this.widget = null;
this.c_time = new Date().getTime();
this.events = [];
this._inTouch = !1;
}
getName() {
var e;
return null === (e = this.ui_param_interface) || void 0 === e ? void 0 : e.ui_config_name;
}
onLoad() {
this.widget = this.node.getComponent(cc.Widget);
if (this.hideOnTouchOutBoundContainer && this.contentNode) {
this.node.on(cc.Node.EventType.TOUCH_START, () => {
this._inTouch = !0;
});
this.node.on(cc.Node.EventType.TOUCH_END, e => {
if (this._inTouch) {
let t = this.contentNode, a = this.node.convertToWorldSpaceAR(t.position), i = cc.rect(a.x - t.width * t.anchorX, a.y - t.height * t.anchorY, t.width, t.height), o = cc.v2();
cc.Camera.main.getScreenToWorldPoint(e.getLocation(), o);
i.contains(o) || this.on_close_call();
}
this._inTouch = !1;
});
this.node.on(cc.Node.EventType.TOUCH_CANCEL, () => {
this._inTouch = !1;
});
}
}
android_back_callback() {
this.on_close_call();
}
show(e) {
this.ui_param_interface = e;
this.node.active = !0;
this.contentNode && this.pop(this.contentNode);
r.default.get_instance().emit({
name: o.default.game_play_event_config.ui_view_active_state_change
}, {
name: e.ui_config_name,
active_state: !0
});
}
register_event(e, t, a, i) {
const o = {
name: e,
call_back: t,
target: a,
is_swallow: i
};
this.events.push(o);
r.default.get_instance().listen(o);
}
unregister_event(e, t, a) {
r.default.get_instance().cancel_listen(e, a, t);
}
onDestroy() {
for (const e of this.events) r.default.get_instance().cancel_listen(e.name, e.target, e.call_back);
}
hide() {
if (this.node.active) {
this.node.active = !1;
r.default.get_instance().emit({
name: o.default.game_play_event_config.ui_view_active_state_change
}, {
name: this.ui_param_interface.ui_config_name,
active_state: !1
});
}
}
on_close_call(e, ...t) {
s.default.close_ui(e || this.node.name);
if (this.ui_param_interface) {
this.ui_param_interface.close_callback && this.ui_param_interface.close_callback(...t);
this.ui_param_interface.isStackView && s.default.popStackView(this.ui_param_interface);
}
}
start() {}
onAddFinished() {}
delay_add_nodes(e, t, a, i = .1) {
this.schedule(() => {
const e = cc.instantiate(t);
a(e);
}, i, e, i);
}
pop(e) {
e.scale = .5;
return new Promise(t => {
cc.tween(e).to(.35, {
scale: 1
}, {
easing: cc.easing.backOut
}).call(() => {
t(!0);
}).start();
});
}
opp(e) {
return new Promise(t => {
cc.tween(e).call(() => {
e.scale = 0;
}).to(.15, {
scale: 1.2
}).to(.1, {
scale: 1
}).call(() => {
t(!0);
}).start();
});
}
static open(e) {
let t = this.__UIName;
if (!t) {
console.warn(`界面 ${this.name} 没有配置 __UIName 属性`);
return;
}
e = e || {
param: {}
};
const a = Object.assign({
ui_config_path: n.default[t],
ui_config_name: t
}, e);
s.default.show_ui(a);
}
static open_with_parm(e) {
this.open({
param: e
});
}
static close(...e) {
let t = this.__UIName;
if (!t) {
console.warn(`界面 ${this.name} 没有配置 __UIName 属性`);
return;
}
let a = s.default.all_ui[t];
a && a.node.active && a.on_close_call(t, ...e);
}
};
i([ c(cc.Node) ], d.prototype, "contentNode", void 0);
i([ c() ], d.prototype, "forceRefitAfterShow", void 0);
i([ c() ], d.prototype, "hideOnTouchOutBoundContainer", void 0);
d = i([ l ], d);
a.default = d;
cc._RF.pop();
};
