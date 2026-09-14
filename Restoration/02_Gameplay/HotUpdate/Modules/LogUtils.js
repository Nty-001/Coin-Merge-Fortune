// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: LogUtils; dependency map: {}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "d39a0ZicgVJrIUwjndBpX1G", "LogUtils");
Object.defineProperty(a, "__esModule", {
value: !0
});
class i {
static isString(e) {
return !Array.isArray(e) && "object" != typeof e;
}
static print(e, ...t) {
if (t.length <= 0) return;
if (!cc.sys.isBrowser) {
console.log("[]", ...t);
return;
}
let a = "%c%s ";
if (i.isString(t[0])) {
a += t[0];
t.shift();
}
console.log.call(this, a, e, "[]", ...t);
}
static Red(...e) {
this.print("color:red;", ...e);
}
static Green(...e) {
this.print("color:green;", ...e);
}
static Orange(...e) {
this.print("color:#ee7700;", ...e);
}
static Gray(...e) {
this.print("color:gray;", ...e);
}
static Blue(...e) {
this.print("color:#3a5fcd;", ...e);
}
static Purple(...e) {
this.print("color:#b23aee;", ...e);
}
static DeepPink(...e) {
this.print("color:#ff1493;", ...e);
}
static Trace(...e) {
this.print("", ...e);
}
}
a.default = i;
i.isOpenLog = !0;
cc._RF.pop();
};
