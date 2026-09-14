// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/base/assets/assets/main/index.ba75b.js
// Module: AudioManager; dependency map: {}
module.exports = function(e, t, o) {
"use strict";
cc._RF.push(t, "6ef6bD61yxLu6p/WIbT++ep", "AudioManager");
Object.defineProperty(o, "__esModule", {
value: !0
});
o.default = class {
static playSound(e, t = !1, o = 1) {
return cc.audioEngine.play(e, t, o);
}
static playMusic(e, t = !0, o = 1) {
return cc.audioEngine.play(e, t, o);
}
static stopMusic(e) {
cc.audioEngine.stop(e);
}
static stopSound(e) {
cc.audioEngine.stop(e);
}
static pauseOne(e) {
cc.audioEngine.pauseEffect(e);
}
static resumeOne(e) {
cc.audioEngine.resumeEffect(e);
}
static pauseAll() {
cc.audioEngine.pauseAll();
}
static resumeAll() {
cc.audioEngine.resumeAll();
}
};
cc._RF.pop();
};
