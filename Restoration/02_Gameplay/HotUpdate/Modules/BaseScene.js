// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: BaseScene; dependency map: {}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "a43fbLHQ+hH1ZwtpkBSnQzb", "BaseScene");
Object.defineProperty(a, "__esModule", {
value: !0
});
a.default = class extends cc.Component {
constructor() {
super(...arguments);
this.loading_scene_time = new Date().getTime();
}
onLoad() {}
start() {
this.loading_scene_time = new Date().getTime() - this.loading_scene_time;
}
};
cc._RF.pop();
};
