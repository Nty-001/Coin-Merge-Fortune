// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: Toast; dependency map: {"./BaseUI":"BaseUI"}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "e7d550eQPFDEZrFWauHgtlE", "Toast");
var i = this && this.__decorate || function(e, t, a, i) {
var o, n = arguments.length, s = n < 3 ? t : null === i ? i = Object.getOwnPropertyDescriptor(t, a) : i;
if ("object" == typeof Reflect && "function" == typeof Reflect.decorate) s = Reflect.decorate(e, t, a, i); else for (var r = e.length - 1; r >= 0; r--) (o = e[r]) && (s = (n < 3 ? o(s) : n > 3 ? o(t, a, s) : o(t, a)) || s);
return n > 3 && s && Object.defineProperty(t, a, s), s;
};
Object.defineProperty(a, "__esModule", {
value: !0
});
const o = e("./BaseUI"), {ccclass: n, property: s} = cc._decorator;
let r = class extends o.default {
constructor() {
super(...arguments);
this.toast_interface = null;
this.text_label = null;
this.bottom_sprite = null;
this.show_timer = null;
}
start() {}
show(e) {
super.show(e);
this.set_toast_interface(e.param);
this.flush_view();
this.node.x = cc.winSize.width / 2;
this.node.y = cc.winSize.height / 2;
const t = this.toast_interface.duration ? 1e3 * this.toast_interface.duration : 2e3;
this.show_timer && clearTimeout(this.show_timer);
this.show_timer = setTimeout(() => {
clearTimeout(this.show_timer);
this.on_close_call();
this.toast_interface.finishe_call && this.toast_interface.finishe_call();
}, t);
this.toast_interface.animation && this.move_up();
}
set_toast_interface(e) {
this.toast_interface = e;
}
flush_view() {
this.flush_text_color();
this.flush_bottom_sprite_frame();
this.text_label.string = `<outline color=#000000 width=2>${this.toast_interface.text}</outline>`;
}
flush_bottom_sprite_frame() {
this.toast_interface.bottom_sprite_frame && (this.bottom_sprite.spriteFrame = this.toast_interface.bottom_sprite_frame);
}
flush_text_color() {
this.toast_interface.text_color && (this.text_label.node.color = cc.Color.BLACK.fromHEX(this.toast_interface.text_color));
}
move_up() {
this.bottom_sprite.node.getComponent(cc.Animation).play("toast_move_up");
}
};
i([ s(cc.RichText) ], r.prototype, "text_label", void 0);
i([ s(cc.Sprite) ], r.prototype, "bottom_sprite", void 0);
r = i([ n ], r);
a.default = r;
cc._RF.pop();
};
