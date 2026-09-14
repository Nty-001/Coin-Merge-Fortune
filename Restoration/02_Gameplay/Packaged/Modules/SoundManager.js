// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/base/assets/assets/main/index.ba75b.js
// Module: SoundManager; dependency map: {"./AudioManager":"AudioManager","./LoadAllResources":"LoadAllResources","./LocalDataManager":"LocalDataManager"}
module.exports = function(e, t, o) {
"use strict";
cc._RF.push(t, "38a53fhmFBA+6OZ2GtnENh4", "SoundManager");
Object.defineProperty(o, "__esModule", {
value: !0
});
o.SoundName = void 0;
const a = e("./AudioManager"), s = e("./LoadAllResources"), n = e("./LocalDataManager");
var i;
(function(e) {
e.bgm_game = "BG";
e.click_sound = "button";
e.merger = "HeCheng";
e.FangZhi = "FangZhi";
e.HeChengZuiDaYinBi = "HeChengZuiDaYinBi";
e.ShiBai = "ShiBai";
})(i = o.SoundName || (o.SoundName = {}));
class c {
static playSound(e, t = !1, o = 1) {
c.isWatchVideo && (o = 0);
if (0 == n.default.getInstance().getGameData().open_music) return -1;
let i = s.default.getInstance().getAudioRes(e);
return null == i ? -1 : a.default.playSound(i, t, o);
}
static playClickSound(e = i.click_sound, t = !1, o = 1) {
c.isWatchVideo && (o = 0);
if (0 == n.default.getInstance().getGameData().open_music) return -1;
let l = s.default.getInstance().getAudioRes(e);
return null == l ? -1 : a.default.playSound(l, t, o);
}
static playMusic(e, t = !0, o = 1) {
c.isWatchVideo && (o = 0);
let n = s.default.getInstance().getAudioRes(e);
return null == n ? -1 : a.default.playMusic(n, t, o);
}
static stopMusic(e) {
a.default.stopMusic(e);
}
static stopSound(e) {
a.default.stopSound(e);
}
static pauseOne(e) {
a.default.pauseOne(e);
}
static resumeOne(e) {
a.default.resumeOne(e);
}
static pauseAll() {
a.default.pauseAll();
}
static resumeAll() {
a.default.resumeAll();
}
static getDuration(e) {
let t = s.default.getInstance().getAudioRes(e);
return null == t ? -1 : t.duration;
}
static loopBgm(e = i.bgm_game) {
if (n.default.getInstance().getGameData().getMusicStatus()) {
c.stopMusic(c.bgMusicId);
c.bgMusicId = c.playMusic(e, !0, .7);
}
}
}
o.default = c;
c.isWatchVideo = !1;
c.bgMusicId = -1;
cc._RF.pop();
};
