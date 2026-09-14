// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/base/assets/assets/main/index.ba75b.js
// Module: Wall; dependency map: {}
module.exports = function(e, t, o) {
"use strict";
cc._RF.push(t, "8b11fQHu7tNqqyV5i1XPhwy", "Wall");
var a = this && this.__decorate || function(e, t, o, a) {
var s, n = arguments.length, i = n < 3 ? t : null === a ? a = Object.getOwnPropertyDescriptor(t, o) : a;
if ("object" == typeof Reflect && "function" == typeof Reflect.decorate) i = Reflect.decorate(e, t, o, a); else for (var c = e.length - 1; c >= 0; c--) (s = e[c]) && (i = (n < 3 ? s(i) : n > 3 ? s(t, o, i) : s(t, o)) || i);
return n > 3 && i && Object.defineProperty(t, o, i), i;
};
Object.defineProperty(o, "__esModule", {
value: !0
});
o.WallType = void 0;
(function(e) {
e.Left = "Left";
e.Right = "Right";
e.Top = "Top";
e.Bottom = "Bottom";
})(o.WallType || (o.WallType = {}));
const {ccclass: s, property: n} = cc._decorator;
let i = class extends cc.Component {
constructor() {
super(...arguments);
this.rigidBody = null;
this.physicsCollider = null;
this.wallType = null;
}
onLoad() {
this.rigidBody = this.node.getComponent(cc.RigidBody);
this.physicsCollider = this.node.getComponent(cc.PhysicsCollider);
this.rigidBody.type = cc.RigidBodyType.Static;
this.rigidBody.gravityScale = 0;
this.physicsCollider.friction = 1;
this.physicsCollider.restitution = .1;
}
init(e) {
this.wallType = e;
}
getWallType() {
return this.wallType;
}
};
i = a([ s ], i);
o.default = i;
cc._RF.pop();
};
