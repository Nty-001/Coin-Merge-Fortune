// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: PlayData; dependency map: {"./GameLocalData":"GameLocalData"}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "22ec2+TAoJGtpf7g6liXvKr", "PlayData");
Object.defineProperty(a, "__esModule", {
value: !0
});
const i = e("./GameLocalData");
class o {
constructor() {
this.base_name = "PlayData";
this.money = 0;
this.UseRevenue = 0;
this.addVodeoshowCoin = 1;
this.fakeMoney = 0;
this.gold = 100;
this.ecmp = 10;
this.watch_video_count = 0;
this.coin1024Number = 0;
this.today1024NumberCoin = 0;
this.today1024NumberCoinDate = "";
this.histroyMaxScore = 0;
this.gameTotalScore = 0;
this.roundScore = 0;
this.loginDays = 0;
this.lastLoginDate = "";
this.isNewLoginDay = !1;
this.hasCurrentRoundStats = !1;
this.currentRoundStartHistroyMaxScore = 0;
this.currentRoundCoin1024Number = 0;
this.hasSavedGameScene = !1;
this.savedCoins = [];
this.savedCurrentCoinValue = 1;
this.savedNextCoinValue = 1;
this.savedPreviewX = 0;
this.savedCoinsScore = 0;
this.savedDrawScore = 0;
this.raccountName = "";
this.rfullName = "";
this.rdocumentId = "";
this.raccountType = "";
this.realSelectPlatform = "PayPal";
this.playerCurrentStep = 0;
this.guideStep = 0;
this.currentLotteryCount = 0;
this.stronewardTimes = 0;
this.dropCointimes = 0;
this.windowsCointimes = 0;
this.watch_video_Singlecount = [ 0, 0, 0, 0, 0, 0, 0, 0 ];
this.real_watch_video_Singlecount = [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ];
this.open_music = !0;
this.open_vibrate = !0;
this.open_bgm = !0;
this.gameRateScore = 0;
this.gameRateTimes = 0;
this.firstMergeIcon500 = !1;
this.newFakeMoneyWithdraw = [ 0, 0, 0, 0, 0, 0 ];
}
start() {}
parseEndCallback() {
this.normalizeScoreFields(!0, !0, !0);
}
add_show_video() {
this.watch_video_count += 1;
i.default.getInstance().set_local_storeage();
}
parseFromUserDefault(e) {
if (!e) return;
let t = e.hasOwnProperty("gameTotalScore"), a = e.hasOwnProperty("roundScore"), i = e.hasOwnProperty("savedDrawScore");
for (let t in this) "function" != typeof this[t] && "todayData" != t && e.hasOwnProperty(t) && (this[t] = e[t]);
this.normalizeScoreFields(t, a, i);
this.parseEndCallback();
}
normalizeScoreFields(e, t, a) {
let i = Math.max(0, Number(this.savedCoinsScore) || 0), o = Math.max(0, a ? Number(this.savedDrawScore) || 0 : i);
this.roundScore = Math.max(0, t ? Number(this.roundScore) || 0 : i);
this.gameTotalScore = Math.max(0, e ? Number(this.gameTotalScore) || 0 : o);
this.savedCoinsScore = this.roundScore;
this.savedDrawScore = this.gameTotalScore;
}
}
a.default = o;
o._name = "PlayData";
cc._RF.pop();
};
