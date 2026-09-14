// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/base/assets/assets/main/index.ba75b.js
// Module: GameMapLayer; dependency map: {"../ATools/TouchButton":"TouchButton","../GameScene":"GameScene","./LevelManager":"LevelManager","./PhysicsManager":"PhysicsManager","./Wall":"Wall"}
module.exports = function(e, t, o) {
"use strict";
cc._RF.push(t, "df800HSMZFIUIexUBkafTz+", "GameMapLayer");
var a, s = this && this.__decorate || function(e, t, o, a) {
var s, n = arguments.length, i = n < 3 ? t : null === a ? a = Object.getOwnPropertyDescriptor(t, o) : a;
if ("object" == typeof Reflect && "function" == typeof Reflect.decorate) i = Reflect.decorate(e, t, o, a); else for (var c = e.length - 1; c >= 0; c--) (s = e[c]) && (i = (n < 3 ? s(i) : n > 3 ? s(t, o, i) : s(t, o)) || i);
return n > 3 && i && Object.defineProperty(t, o, i), i;
}, n = this && this.__awaiter || function(e, t, o, a) {
return new (o || (o = Promise))(function(s, n) {
function i(e) {
try {
l(a.next(e));
} catch (e) {
n(e);
}
}
function c(e) {
try {
l(a.throw(e));
} catch (e) {
n(e);
}
}
function l(e) {
e.done ? s(e.value) : (t = e.value, t instanceof o ? t : new o(function(e) {
e(t);
})).then(i, c);
var t;
}
l((a = a.apply(e, t || [])).next());
});
};
Object.defineProperty(o, "__esModule", {
value: !0
});
const i = e("../ATools/TouchButton"), c = e("../GameScene"), l = e("./LevelManager"), r = e("./PhysicsManager"), h = e("./Wall"), {ccclass: u, property: d} = cc._decorator;
let g = a = class extends cc.Component {
constructor() {
super(...arguments);
this.layout_dynamic_zIndex = null;
this.block_show = null;
this.node_xian = null;
this.wall_left = null;
this.wall_right = null;
this.wall_top = null;
this.wall_bottom = null;
this.lbl_mode = null;
this.btn_back = null;
this.btn_restart = null;
this._isAutoDrop = !1;
}
get isAutoDrop() {
return this._isAutoDrop;
}
set isAutoDrop(e) {
this._isAutoDrop = e;
}
onLoad() {
a.instance = this;
[ this.wall_left, this.wall_right, this.wall_top, this.wall_bottom ].forEach(e => {
var t;
null === (t = e.getComponent(cc.Widget)) || void 0 === t || t.updateAlignment();
});
this.wall_left.addComponent(h.default).init(h.WallType.Left);
this.wall_right.addComponent(h.default).init(h.WallType.Right);
this.wall_top.addComponent(h.default).init(h.WallType.Top);
this.wall_bottom.addComponent(h.default).init(h.WallType.Bottom);
this.block_show.active = !1;
r.default.openPhysics();
this.scheduleOnce(() => {
l.default.getInstance().initLevelManager(this);
}, 1);
this.btn_back.addComponent(i.default).registerTouchEvent(() => {
c.default.instance.onEnterHome();
});
this.btn_restart.addComponent(i.default).registerTouchEvent(() => {
l.default.getInstance().startGame();
});
}
onEnable() {}
onDisable() {}
onDestroy() {
a.instance = null;
}
static getInstance() {
return a.instance;
}
getBlockParent() {
return this.layout_dynamic_zIndex;
}
getLineWorldPos() {
return this.node_xian.convertToWorldSpaceAR(cc.Vec2.ZERO);
}
playBomoSpine(e) {}
playBlockIdleAction(e) {
let t = null;
this.block_show.children.forEach(o => {
o.name == e.toString() && (t = o.getComponent(cc.Sprite));
});
if (!t) return;
t.node.stopActionByTag(1929);
const o = cc.tween(t.node).to(.4, {
scaleX: .85,
scaleY: 1.2,
y: 100
}).to(.4, {
scaleX: 1.2,
scaleY: .85,
y: -50
}).to(.4, {
scaleX: 1,
scaleY: 1,
y: 0
}).delay(.4);
cc.tween(t.node).tag(1929).delay(2).repeat(99999, o).start();
}
stopBlockIdleAction() {
this.block_show.children.forEach(e => {
e.stopActionByTag(1929);
e.scale = 1;
e.setPosition(cc.Vec2.ZERO);
});
}
delayTime(e) {
return n(this, void 0, void 0, function*() {
yield new Promise(t => {
this.scheduleOnce(t, e);
});
});
}
showMode(e) {
this.lbl_mode.string = [ "beginner", "simple", "normal", "hard", "limit", "hell" ][e] + "  Score0";
}
setScore(e, t) {
this.lbl_mode.string = [ "beginner", "simple", "normal", "hard", "limit", "hell" ][e] + "  Score" + t;
}
onClickBackBtn() {
l.default.getInstance().tttt();
}
};
g.instance = null;
s([ d(cc.Node) ], g.prototype, "layout_dynamic_zIndex", void 0);
s([ d(cc.Node) ], g.prototype, "block_show", void 0);
s([ d(cc.Node) ], g.prototype, "node_xian", void 0);
s([ d(cc.Node) ], g.prototype, "wall_left", void 0);
s([ d(cc.Node) ], g.prototype, "wall_right", void 0);
s([ d(cc.Node) ], g.prototype, "wall_top", void 0);
s([ d(cc.Node) ], g.prototype, "wall_bottom", void 0);
s([ d(cc.Label) ], g.prototype, "lbl_mode", void 0);
s([ d(cc.Node) ], g.prototype, "btn_back", void 0);
s([ d(cc.Node) ], g.prototype, "btn_restart", void 0);
g = a = s([ u ], g);
o.default = g;
cc._RF.pop();
};
