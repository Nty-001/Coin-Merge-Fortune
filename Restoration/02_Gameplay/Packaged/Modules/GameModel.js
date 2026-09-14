// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/base/assets/assets/main/index.ba75b.js
// Module: GameModel; dependency map: {"./BaseRecord":"BaseRecord","./EventCenter":"EventCenter","./GameEventConsts":"GameEventConsts","./LocalDataManager":"LocalDataManager"}
module.exports = function(e, t, o) {
"use strict";
cc._RF.push(t, "3256aK6RsBBibgOsFHktWKM", "GameModel");
Object.defineProperty(o, "__esModule", {
value: !0
});
const a = e("./BaseRecord"), s = e("./EventCenter"), n = e("./GameEventConsts"), i = e("./LocalDataManager");
class c extends a.default {
constructor() {
super();
this.base_name = "GameModel";
this.NewUser = !0;
this.coins = 0;
this.heart = 10;
this.open_bgm = !0;
this.open_music = !0;
this.mergedMaxLv = 1;
this.mergeCount = 0;
this.passStaus = [ 1, 0, 0, 0, 0, 0 ];
this.passlevel = [ 0, 0, 0, 0, 0, 0 ];
this.achieveStaus = [ 1, 0, 0, 0, 0, 0 ];
}
parseEndCallback() {}
init() {}
setMusicStatus(e) {
this.open_bgm = e;
this.open_music = e;
i.default.getInstance().saveToUserDefault();
}
getMusicStatus() {
return this.open_bgm;
}
setSoundStatus(e) {
this.open_music = e;
i.default.getInstance().saveToUserDefault();
}
getSoundStatus() {
return this.open_music;
}
getIsNewUser() {
return this.NewUser;
}
setIsNewUser(e) {
this.NewUser = e;
i.default.getInstance().saveToUserDefault();
}
getCoins() {
return this.coins;
}
addCoins(e = 1) {
this.coins += e;
i.default.getInstance().saveToUserDefault();
s.EventCenter.getInstance().fire(n.GameEventName.UseCoins);
}
reduceCoins(e = 1) {
if (this.coins < e) return !1;
this.coins -= e;
i.default.getInstance().saveToUserDefault();
s.EventCenter.getInstance().fire(n.GameEventName.UseCoins);
return !0;
}
getHeart() {
return this.heart;
}
addHeart(e = 1) {
if (!(this.heart >= 10)) {
this.heart += e;
i.default.getInstance().saveToUserDefault();
s.EventCenter.getInstance().fire(n.GameEventName.UserHeart);
}
}
reduceHeart(e = 1) {
if (this.heart < e) return !1;
this.heart -= e;
i.default.getInstance().saveToUserDefault();
s.EventCenter.getInstance().fire(n.GameEventName.UserHeart);
return !0;
}
getPassLevel(e) {
return this.passlevel[e];
}
addPassLevel(e, t = 1) {
this.passlevel[e] += t;
i.default.getInstance().saveToUserDefault();
s.EventCenter.getInstance().fire(n.GameEventName.PassLevel);
}
getPassStatus(e) {
return this.passStaus[e];
}
setPassStatus(e, t) {
this.passStaus[e] = t;
i.default.getInstance().saveToUserDefault();
}
getAchieveStatus(e) {
return this.achieveStaus[e];
}
setAchieveStatus(e, t) {
this.achieveStaus[e] = t;
i.default.getInstance().saveToUserDefault();
}
setTotalMergeCount(e, t, o) {
if (e + 1 > this.mergedMaxLv) {
this.mergedMaxLv = e + 1;
s.EventCenter.getInstance().fire(n.GameEventName.MergedMaxLvChanged);
} else null == o || o();
this.mergeCount += t;
i.default.getInstance().saveToUserDefault();
s.EventCenter.getInstance().fire(n.GameEventName.MergeCountChanged);
}
resetAllBlockStorage(e) {
this.gameStorage = e;
i.default.getInstance().saveToUserDefault();
}
getAllBlockStorage() {
return this.gameStorage;
}
}
o.default = c;
c._name = "GameModel";
cc._RF.pop();
};
