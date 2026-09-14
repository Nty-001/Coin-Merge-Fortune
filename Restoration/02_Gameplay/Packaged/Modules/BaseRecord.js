// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/base/assets/assets/main/index.ba75b.js
// Module: BaseRecord; dependency map: {}
module.exports = function(e, t, o) {
"use strict";
cc._RF.push(t, "bdca0Q0Dt5Hq4wKs//14qrm", "BaseRecord");
Object.defineProperty(o, "__esModule", {
value: !0
});
o.default = class {
parseFromUserDefault(e) {
if (e) {
for (let t in this) "function" != typeof this[t] && "todayData" != t && e.hasOwnProperty(t) && (this[t] = e[t]);
for (let e in this) this[e];
this.parseEndCallback();
}
}
};
cc._RF.pop();
};
