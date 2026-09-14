// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: Singleton; dependency map: {}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "38f56XeLxdE8ZUbItEE2//d", "Singleton");
Object.defineProperty(a, "__esModule", {
value: !0
});
a.default = class {
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
