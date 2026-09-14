// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/base/assets/assets/main/index.ba75b.js
// Module: Singleton; dependency map: {}
module.exports = function(e, t, o) {
"use strict";
cc._RF.push(t, "3cc2aAhDbtFAa0I6sjZYhyv", "Singleton");
Object.defineProperty(o, "__esModule", {
value: !0
});
o.default = class {
static getInstance() {
this.instance || (this.instance = new this());
return this.instance;
}
static destroyInstance() {
this.instance = null;
}
};
cc._RF.pop();
};
