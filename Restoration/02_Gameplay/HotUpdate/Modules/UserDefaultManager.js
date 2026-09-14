// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: UserDefaultManager; dependency map: {}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "95cedyMfelLWZ+CwYb46W8K", "UserDefaultManager");
Object.defineProperty(a, "__esModule", {
value: !0
});
a.default = class {
static setItem(e, t) {
cc.sys.localStorage.setItem(e, t);
}
static setItem_json(e, t) {
cc.sys.localStorage.setItem(e, JSON.stringify(t));
}
static getItem(e) {
return cc.sys.localStorage.getItem(e);
}
static getItem_json(e) {
let t = cc.sys.localStorage.getItem(e);
return JSON.parse(t);
}
static removeItem(e) {
cc.sys.localStorage.removeItem(e);
}
};
cc._RF.pop();
};
