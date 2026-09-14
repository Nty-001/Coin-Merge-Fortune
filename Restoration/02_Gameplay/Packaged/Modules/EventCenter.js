// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/base/assets/assets/main/index.ba75b.js
// Module: EventCenter; dependency map: {}
module.exports = function(e, t, o) {
"use strict";
cc._RF.push(t, "11aa0/IGkZLs4CYvJaMzLaE", "EventCenter");
Object.defineProperty(o, "__esModule", {
value: !0
});
o.EventCenter = o.Observer = o.GameEventInterface = void 0;
class a {
constructor(e, t, o, a = !1) {
this.name = e;
this.callback = t;
this.context = o;
this.isSwallow = a;
}
}
o.GameEventInterface = a;
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
o.Observer = s;
class n {
constructor() {
this.listeners = new Map();
}
static getInstance() {
null == n.m_instance && (n.m_instance = new n());
return n.m_instance;
}
register(e, t, o, n = !1) {
const i = new a(e, t, o, n);
0 == this.listeners.has(i.name) && this.listeners.set(i.name, new Array());
this.listeners.get(i.name).push(new s(i));
}
remove(e, t) {
if (0 == this.listeners.has(e)) return;
let o = this.listeners.get(e), a = o.length;
for (let e = 0; e < a; e++) if (o[e].compar(t)) {
o.splice(e, 1);
break;
}
0 == o.length && this.listeners.delete(e);
}
fire(e, ...t) {
if (0 == this.listeners.has(e)) return;
let o = this.listeners.get(e);
for (let e = o.length - 1; e >= 0; --e) {
let a = o[e];
a.notify(...t);
if (a.getGameEventInterface().isSwallow) break;
}
}
}
o.EventCenter = n;
n.m_instance = null;
cc._RF.pop();
};
