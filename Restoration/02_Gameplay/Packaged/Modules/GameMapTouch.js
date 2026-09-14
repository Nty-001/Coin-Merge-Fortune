// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/base/assets/assets/main/index.ba75b.js
// Module: GameMapTouch; dependency map: {}
module.exports = function(e, t, o) {
"use strict";
cc._RF.push(t, "5a8d2hF7GJO2apv5N2IPeY6", "GameMapTouch");
Object.defineProperty(o, "__esModule", {
value: !0
});
o.default = class extends cc.Component {
constructor() {
super(...arguments);
this.touchBeginCallback = null;
this.touchMoveCallback = null;
this.touchEndCallback = null;
}
onEnable() {
this.node.on(cc.Node.EventType.TOUCH_START, this.onTouchStart, this);
this.node.on(cc.Node.EventType.TOUCH_MOVE, this.onTouchMoved, this);
this.node.on(cc.Node.EventType.TOUCH_END, this.onTouchEnd, this);
this.node.on(cc.Node.EventType.TOUCH_CANCEL, this.onTouchCancel, this);
}
onDisable() {
this.node.off(cc.Node.EventType.TOUCH_START, this.onTouchStart, this);
this.node.off(cc.Node.EventType.TOUCH_MOVE, this.onTouchMoved, this);
this.node.off(cc.Node.EventType.TOUCH_END, this.onTouchEnd, this);
this.node.off(cc.Node.EventType.TOUCH_CANCEL, this.onTouchCancel, this);
}
onTouchStart(e) {
var t;
const o = e.getLocation();
null === (t = this.touchBeginCallback) || void 0 === t || t.call(this, o);
}
onTouchMoved(e) {
var t;
const o = e.getLocation();
null === (t = this.touchMoveCallback) || void 0 === t || t.call(this, o);
}
onTouchEnd(e) {
var t;
const o = e.getLocation();
null === (t = this.touchEndCallback) || void 0 === t || t.call(this, o);
}
onTouchCancel(e) {
this.onTouchEnd(e);
}
init(e) {
this.touchBeginCallback = e.touchBeginCallback;
this.touchMoveCallback = e.touchMoveCallback;
this.touchEndCallback = e.touchEndCallback;
}
};
cc._RF.pop();
};
