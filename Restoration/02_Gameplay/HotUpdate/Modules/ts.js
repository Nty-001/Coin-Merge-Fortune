// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: ts; dependency map: {}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "22828ZTS79Cvo2sSgaS4Myv", "ts");
var i = this && this.__decorate || function(e, t, a, i) {
var o, n = arguments.length, s = n < 3 ? t : null === i ? i = Object.getOwnPropertyDescriptor(t, a) : i;
if ("object" == typeof Reflect && "function" == typeof Reflect.decorate) s = Reflect.decorate(e, t, a, i); else for (var r = e.length - 1; r >= 0; r--) (o = e[r]) && (s = (n < 3 ? o(s) : n > 3 ? o(t, a, s) : o(t, a)) || s);
return n > 3 && s && Object.defineProperty(t, a, s), s;
};
Object.defineProperty(a, "__esModule", {
value: !0
});
const {ccclass: o, property: n, menu: s} = cc._decorator;
let r = class extends cc.Component {
constructor() {
super(...arguments);
this.scroll_view = null;
}
onLoad() {
if (this.scroll_view) {
this.node.on("scrolling", this._event_update_opacity, this);
this.scroll_view.content.on(cc.Node.EventType.CHILD_REMOVED, this._event_update_opacity, this);
this.scroll_view.content.on(cc.Node.EventType.CHILD_REORDER, this._event_update_opacity, this);
} else cc.error("不存在ScrollView组件！");
}
_get_bounding_box_to_world(e) {
let t = e._contentSize.width, a = e._contentSize.height, i = cc.rect(-e._anchorPoint.x * t, -e._anchorPoint.y * a, t, a);
e._calculWorldMatrix();
i.transformMat4(i, e._worldMatrix);
return i;
}
_check_collision(e) {
let t = this._get_bounding_box_to_world(this.scroll_view.content.parent), a = this._get_bounding_box_to_world(e);
t.width += .5 * t.width;
t.height += .5 * t.height;
t.x -= .25 * t.width;
t.y -= .25 * t.height;
return t.intersects(a);
}
_event_update_opacity() {
this.scroll_view.content.children.forEach(e => {
e.opacity = this._check_collision(e) ? 255 : 0;
});
}
};
i([ n(cc.ScrollView) ], r.prototype, "scroll_view", void 0);
r = i([ o, s("tool/list_optimize") ], r);
a.default = r;
cc._RF.pop();
};
