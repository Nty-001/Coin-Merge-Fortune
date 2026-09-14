// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: EventManager; dependency map: {}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "e831aDxtCRN87KUq7bv2OQh", "EventManager");
Object.defineProperty(a, "__esModule", {
value: !0
});
class i {
constructor(e, t, a) {
this.event_name = "";
this.target = null;
this.cb = null;
this.event_name = e;
this.target = t;
this.cb = a;
}
trigger(...e) {
this.cb.apply(this.target, e);
}
}
class o {
constructor() {
this._event_dict = {};
}
static get_instance() {
this._instance || (this._instance = new o());
return this._instance;
}
listen(e) {
let t = this._event_dict[e.name];
if (t) this.cancel_listen(e.name, e.target, e.call_back); else {
t = [];
this._event_dict[e.name] = t;
}
let a = new i(e.name, e.target, e.call_back);
t.push(a);
}
cancel_listen(e, t, a) {
let i = this._event_dict[e];
if (i) for (let e = 0; e < i.length; ++e) if (i[e].target === t) {
i.splice(e, 1);
break;
}
}
emit(e, ...t) {
let a = this._event_dict[e.name];
if (a) for (let i = a.length - 1; i >= 0 && !e.is_swallow; --i) {
let o = a[i], n = [ e ];
for (let e = 0; e < t.length; ++e) n.push(t[e]);
o.trigger(...n);
}
}
}
a.default = o;
cc._RF.pop();
};
