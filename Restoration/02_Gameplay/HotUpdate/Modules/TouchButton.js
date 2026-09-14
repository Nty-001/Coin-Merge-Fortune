// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: TouchButton; dependency map: {"./SoundManager":"SoundManager"}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "878c1RoNyJLx5zOQYZsegWz", "TouchButton");
Object.defineProperty(a, "__esModule", {
value: !0
});
const i = e("./SoundManager");
a.default = class extends cc.Component {
constructor() {
super(...arguments);
this.m_touchInterface = null;
this.m_actionTag = 1048578;
this.isTouch = !1;
this.m_originScale = 1;
this.canTouch = !0;
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
this.node.stopActionByTag(this.m_actionTag);
this.node.scale = this.m_originScale;
}
onTouchStart(e) {
if (!this.canTouch) return;
let t = e.getLocation(), a = this.node.getParent().convertToNodeSpaceAR(t);
if (this.node.getBoundingBox().contains(a)) {
this.isTouch = !0;
this.m_touchInterface.swallowTouch && e.stopPropagation();
cc.tween(this.node).to(this.m_touchInterface.scaleTime, {
scale: this.m_touchInterface.scale * this.m_originScale
}).tag(this.m_actionTag).start();
} else this.isTouch = !1;
}
onTouchMoved(e) {
e.getDelta().mag() > 8 && (this.isTouch = !1);
}
onTouchEnd(e) {
var t, a;
if (this.isTouch) {
this.m_touchInterface.swallowTouch && e.stopPropagation();
this.m_touchInterface.playClickSound && i.default.playClickSound();
null === (a = (t = this.m_touchInterface).touchEndCallback) || void 0 === a || a.call(t);
}
this.isTouch = !1;
cc.tween(this.node).to(this.m_touchInterface.scaleTime, {
scale: this.m_originScale
}).tag(this.m_actionTag).start();
}
onTouchCancel(e) {
this.isTouch = !1;
cc.tween(this.node).to(this.m_touchInterface.scaleTime, {
scale: this.m_originScale
}).tag(this.m_actionTag).start();
}
registerTouchEvent(e, t = 1.1, a = !0, i = !0) {
this.m_originScale = this.node.scale;
this.m_touchInterface = {
touchEndCallback: e,
scale: t,
scaleTime: .1,
swallowTouch: a,
playClickSound: i
};
}
updateOriginScale(e) {
this.m_originScale = e;
}
};
cc._RF.pop();
};
