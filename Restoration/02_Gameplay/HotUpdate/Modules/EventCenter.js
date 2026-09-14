// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: EventCenter; dependency map: {"../Storage/Singleton":"Singleton","./MKNotice":"MKNotice"}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "26ee7wg7/pOAKAkdXSq/9iI", "EventCenter");
Object.defineProperty(a, "__esModule", {
value: !0
});
a.EventCenter = a.Observer = a.GameEventInterface = void 0;
const i = e("../Storage/Singleton"), o = e("./MKNotice");
class n {
constructor(e, t, a, i = !1) {
this.name = e;
this.callback = t;
this.context = a;
this.isSwallow = i;
}
}
a.GameEventInterface = n;
class s {
constructor(e) {
this.m_eventInterface = null;
this.m_eventInterface = e;
}
notify(...e) {
this.m_eventInterface.callback.call(this.m_eventInterface.context, ...e);
}
compar(e) {
return e == this.m_eventInterface.context;
}
getGameEventInterface() {
return this.m_eventInterface;
}
}
a.Observer = s;
a.EventCenter = class extends i.default {
constructor() {
super(...arguments);
this.listeners = new Map();
}
register(e, t, a, i = !1) {
const o = new n(e, t, a, i);
0 == this.listeners.has(o.name) && this.listeners.set(o.name, new Array());
this.listeners.get(o.name).push(new s(o));
}
remove(e, t) {
if (0 == this.listeners.has(e)) return;
let a = this.listeners.get(e);
for (let e = 0; e < a.length; e++) if (a[e].compar(t)) {
a.splice(e, 1);
e--;
}
0 == a.length && this.listeners.delete(e);
}
fire(e) {
let t = "";
if ("" == (t = e instanceof o.default ? e.getEventName() : e)) {
console.log("注册的事件名为空字符串");
return;
}
if (0 == this.listeners.has(t)) return;
let a = this.listeners.get(t);
for (let t = a.length - 1; t >= 0; --t) {
let i = a[t];
e instanceof o.default ? i.notify(e) : i.notify(new o.default(e));
if (i.getGameEventInterface().isSwallow) break;
}
}
};
cc._RF.pop();
};
