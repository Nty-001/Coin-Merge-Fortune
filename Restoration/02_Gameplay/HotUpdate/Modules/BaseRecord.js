// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: BaseRecord; dependency map: {}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "bb262Cqo3xCvJ+9mTWDWLR1", "BaseRecord");
Object.defineProperty(a, "__esModule", {
value: !0
});
a.default = class {
parseEndCallback() {}
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
