// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: SoundManager; dependency map: {"./AudioManager":"AudioManager","./GameLocalData":"GameLocalData","./LoadAllResources":"LoadAllResources","./PlayData":"PlayData"}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "592fb/d/ZFAqZQD5HqYZiEm", "SoundManager");
Object.defineProperty(a, "__esModule", {
value: !0
});
a.SoundName = void 0;
const i = e("./AudioManager"), o = e("./GameLocalData"), n = e("./LoadAllResources"), s = e("./PlayData");
var r;
(function(e) {
e.bgm_game = "bgm_game";
e["点击按钮"] = "click";
})(r = a.SoundName || (a.SoundName = {}));
class l {
static get playData() {
return o.default.getInstance().getData(s.default);
}
static isSoundEffectOpen() {
return !1 !== l.playData.open_music;
}
static isBgmOpen() {
return !1 !== l.playData.open_bgm;
}
static getAudioClip(e) {
return n.default.getInstance().getAudioRes(e) || l.audioClipCache[e] || null;
}
static loadAudioClip(e, t) {
let a = l.getAudioClip(e);
if (a) t(a); else if (l.audioLoadingCallbacks[e]) l.audioLoadingCallbacks[e].push(t); else {
l.audioLoadingCallbacks[e] = [ t ];
cc.resources.load("Audio/" + e, cc.AudioClip, (t, a) => {
let i = l.audioLoadingCallbacks[e] || [];
delete l.audioLoadingCallbacks[e];
if (!t && a) {
l.audioClipCache[e] = a;
for (let e = 0; e < i.length; e++) i[e](a);
} else cc.warn("load audio failed:", e, t);
});
}
}
static playSound(e, t = !1, a = 1) {
l.isWatchVideo && (a = 0);
if (!l.isSoundEffectOpen()) return -1;
let o = l.getAudioClip(e);
if (null == o) {
l.loadAudioClip(e, e => {
l.isSoundEffectOpen() && i.default.playSound(e, t, l.isWatchVideo ? 0 : a);
});
return -1;
}
return i.default.playSound(o, t, a);
}
static playClickSound(e = r.点击按钮, t = !1, a = .5) {
l.isWatchVideo && (a = 0);
if (!l.isSoundEffectOpen()) return -1;
let o = l.getAudioClip(e);
if (null == o) {
l.loadAudioClip(e, e => {
l.isSoundEffectOpen() && i.default.playSound(e, t, l.isWatchVideo ? 0 : a);
});
return -1;
}
return i.default.playSound(o, t, a);
}
static playMusic(e, t = !0, a = 1) {
if (l.isWatchVideo) return -1;
if (!l.isBgmOpen()) return -1;
let o = ++l.musicLoadToken, n = l.getAudioClip(e);
if (null == n) {
l.loadAudioClip(e, e => {
if (o == l.musicLoadToken && l.isBgmOpen() && !l.isWatchVideo) {
l.stopMusic(l.bgMusicId);
l.bgMusicId = i.default.playMusic(e, t, a);
}
});
return -1;
}
return i.default.playMusic(n, t, a);
}
static stopMusic(e) {
l.musicLoadToken++;
i.default.stopMusic(e);
}
static pauseBgm() {
i.default.pauseMusic(l.bgMusicId);
}
static resumeBgm(e = r.bgm_game) {
l.isBgmOpen() && (null == l.bgMusicId || l.bgMusicId < 0 ? l.loopBgm(e) : i.default.resumeMusic(l.bgMusicId));
}
static stopSound(e) {
i.default.stopSound(e);
}
static pauseOne(e) {
i.default.pauseOne(e);
}
static resumeOne(e) {
i.default.resumeOne(e);
}
static pauseAll() {
i.default.pauseAll();
}
static resumeAll() {
i.default.resumeAll();
}
static getDuration(e) {
let t = l.getAudioClip(e);
return null == t ? -1 : t.duration;
}
static loopBgm(e = r.bgm_game) {
if (l.isBgmOpen()) {
l.stopMusic(l.bgMusicId);
l.bgMusicId = l.playMusic(e, !0, .2);
} else l.stopMusic(l.bgMusicId);
}
}
a.default = l;
l.isWatchVideo = !1;
l.bgMusicId = -1;
l.audioClipCache = {};
l.audioLoadingCallbacks = {};
l.musicLoadToken = 0;
cc._RF.pop();
};
