// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: DateUtils; dependency map: {}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "d786eMsEFFEG4FKRYXNS8FX", "DateUtils");
Object.defineProperty(a, "__esModule", {
value: !0
});
a.TimeFormat = void 0;
var i;
(function(e) {
e[e.YYYYMMDD = 0] = "YYYYMMDD";
e[e.YYYYMMDDHHMMSS = 1] = "YYYYMMDDHHMMSS";
e[e.YYYYMMDDHHMMSS1 = 2] = "YYYYMMDDHHMMSS1";
e[e.TimeFormatMMDD = 3] = "TimeFormatMMDD";
})(i = a.TimeFormat || (a.TimeFormat = {}));
class o {
static stringFormat(e, ...t) {
if (0 == arguments.length) return "";
let a = arguments[0];
for (let e = 1; e < arguments.length; e++) a = a.replace(new RegExp("\\{" + (e - 1) + "\\}", "g"), arguments[e]);
return a;
}
static fixZeroStart(e, t) {
return (e + "").padStart(t, "0");
}
static getCurrentUTCTimeString(e) {
const t = new Date();
return o.getTimeString(t, e);
}
static getCurrentUTCTimeString2() {
const e = new Date(), t = o.fixZeroStart(e.getFullYear(), 4), a = o.fixZeroStart(e.getMonth() + 1, 2), i = e.getDate() + "";
return o.stringFormat("{0}年{1}月{2}日", t, a, i);
}
static getYYYYMMDD(e, t) {
const a = new Date(e);
return `${o.fixZeroStart(a.getFullYear(), 4)}${t}${o.fixZeroStart(a.getMonth() + 1, 2)}${t}${a.getDate() + ""}`;
}
static getTimeString(e, t) {
const a = o.fixZeroStart(e.getFullYear(), 4), n = o.fixZeroStart(e.getMonth() + 1, 2), s = o.fixZeroStart(e.getDate(), 2), r = o.fixZeroStart(e.getHours(), 2), l = o.fixZeroStart(e.getMinutes(), 2), c = o.fixZeroStart(e.getSeconds(), 2);
let d = "";
switch (t) {
case i.YYYYMMDDHHMMSS:
d = o.stringFormat("{0}-{1}-{2} {3}:{4}:{5}", a, n, s, r, l, c);
break;

case i.YYYYMMDDHHMMSS1:
d = o.stringFormat("{0}年{1}月{2}日 {3}:{4}:{5}", a, n, s, r, l, c);
break;

case i.TimeFormatMMDD:
d = o.stringFormat("{0}-{1}", n, s);
break;

case i.YYYYMMDD:
d = o.stringFormat("{0}-{1}-{2}", a, n, s);
break;

default:
d = o.stringFormat("{0}-{1}-{2} {3}:{4}:{5}", a, n, s, r, l, c);
}
return d;
}
static formatTimestamp(e) {
const t = new Date(e);
return `${t.getFullYear()}-${String(t.getMonth() + 1).padStart(2, "0")}-${String(t.getDate()).padStart(2, "0")} ${String(t.getHours()).padStart(2, "0")}:${String(t.getMinutes()).padStart(2, "0")}:${String(t.getSeconds()).padStart(2, "0")}`;
}
}
a.default = o;
cc._RF.pop();
};
