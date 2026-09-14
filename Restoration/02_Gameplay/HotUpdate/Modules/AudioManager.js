// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: AudioManager; dependency map: {}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "e6f16jqz9FMLpibKLER5yx8", "AudioManager");
Object.defineProperty(a, "__esModule", {
value: !0
});
a.default = class {
static playSound(e, t = !1, a = 1) {
return cc.audioEngine.play(e, t, a);
}
static playMusic(e, t = !0, a = 1) {
return cc.audioEngine.play(e, t, a);
}
static stopMusic(e) {
cc.audioEngine.stop(e);
}
static pauseMusic(e) {
null == e || e < 0 || cc.audioEngine.pause(e);
}
static resumeMusic(e) {
null == e || e < 0 || cc.audioEngine.resume(e);
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
