// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/base/assets/assets/main/index.ba75b.js
// Module: UserDefault; dependency map: {}
module.exports = function(e, t, o) {
"use strict";
cc._RF.push(t, "c2c67woB0VP0aok5g5GCefq", "UserDefault");
Object.defineProperty(o, "__esModule", {
value: !0
});
o.default = class {
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
