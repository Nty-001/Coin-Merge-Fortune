// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: NewGamePlayData; dependency map: {"../../HWL/ServerConfig":"ServerConfig","../../LanguageControl/GameManagement":"GameManagement","../../Report/NativeCall":"NativeCall","./GameLocalData":"GameLocalData"}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "6c168kdq49BZbjpIRA5XkHG", "NewGamePlayData");
Object.defineProperty(a, "__esModule", {
value: !0
});
const i = e("../../CardPlayGame/GameScene"), o = e("../../HWL/ServerConfig"), n = e("../../LanguageControl/GameManagement"), s = e("../../Report/NativeCall"), r = e("./GameLocalData");
class l {
constructor() {
this.base_name = "NewGamePlayData";
this.FirstOpenGame = !1;
this.UseRevenue = 0;
this.addVodeoshowCoin = 1;
this.withdrawRecords = [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ];
this.withdrawRecords2 = [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ];
this.withdrawRecords3 = [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ];
this.realSelectPlatform = o.RealCashPlatform.PayPal;
this.raccountName = "";
this.rfullName = "";
this.rdocumentId = "";
this.raccountType = "";
this.real_watch_video_Singlecount = [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ];
this.guideaddSwimRing = !1;
this.guideRefresh = !1;
this.guiderSuperturret = !1;
this.Passlevel = 0;
this.PassCollection = 0;
this.isFirstFree = !1;
this.open_bgm = !0;
this.open_music = !0;
this.open_vibrate = !0;
this.skippedAdCount = 0;
this.tixian_basePeople = 0;
this.daily_index = 0;
this.daily_tasks = [ !1, !1, !1, !1, !1, !1, !1 ];
this.today_pass_count = 0;
this.props_addSwimRing_count = 3;
this.props_refresh_count = 3;
this.props_superTurret_count = 3;
this.luacky_Value = 0;
this.remove_level_line_count = 0;
this.scenebg_type = 0;
this.pingsai_type = 0;
this.shuiwen_type = 0;
this.scenebg_obtain = [ !0, !1, !1, !1, !1, !1, !1, !1, !1, !1 ];
this.pingsai_obtain = [ !0, !1, !1, !1, !1, !1, !1, !1, !1, !1 ];
this.shuiwen_obtain = [ !0, !1, !1, !1, !1, !1, !1, !1, !1, !1 ];
this.scoreCount = 0;
this.money = 0;
this.red_bag = 0;
this.watch_video_count = 0;
this.watch_video_Singlecount = [ 0, 0, 0, 0, 0, 0, 0, 0 ];
this.share_count = 0;
this.share_item = [ !1, !1, !1 ];
this.success_count = 0;
this.accountName = "";
this.accountEmail = "";
this.accountTarget = 0;
this.selectPlatform = "";
this.atlas_edit = "";
this.fakewithdrawRecord = [ 0, 0, 0, 0, 0, 0, 0, 0 ];
this.guideEnter = !1;
this.flag_sign = Object.create(null);
this.flag_sign_daily = Object.create(null);
this.skin_Video = Object.create(null);
}
parseEndCallback() {}
parseFromUserDefault(e) {
if (e) {
for (let t in this) "function" != typeof this[t] && "todayData" != t && e.hasOwnProperty(t) && (this[t] = e[t]);
this.parseEndCallback();
}
}
init() {}
add_total_money(e) {}
resetDailyShareData() {
const e = cc.sys.localStorage.getItem("share_data_reset_date"), t = new Date().toISOString().split("T")[0];
if (!e || e !== t) {
this.share_count = 0;
this.share_item = [ !1, !1, !1 ];
cc.sys.localStorage.setItem("share_data_reset_date", t);
}
}
add_red_bag(e) {
this.red_bag = this.red_bag + e;
r.default.getInstance().set_local_storeage();
}
add_show_video() {
this.watch_video_count += 1;
for (let t = 0; t < this.watch_video_Singlecount.length; t++) {
var e = n.default.globalData.global.tixian_products[t].withdrawAmount * n.default.GetCountryDang();
this.red_bag >= e && (this.watch_video_Singlecount[t] = (this.watch_video_Singlecount[t] || 0) + 1);
}
for (let e = 0; e < this.real_watch_video_Singlecount.length; e++) if (n.default.real_products.length > e) {
var t = n.default.real_products[e].condition_coin;
this.UseRevenue >= t && (this.real_watch_video_Singlecount[e] = (this.real_watch_video_Singlecount[e] || 0) + 1);
}
r.default.getInstance().set_local_storeage();
1 == this.watch_video_count && s.default.checkClientEndingWithCoin() && i.default.getInstance().GuideManager.startStrongGuide("84", "", 8);
}
getComFlag(e, t) {
return null == this.flag_sign[e] ? t : this.flag_sign[e];
}
setComFlag(e, t) {
this.flag_sign[e] = t;
r.default.getInstance().set_local_storeage();
return t;
}
getComDaily(e, t) {
return null == this.flag_sign_daily[e] ? t : this.flag_sign_daily[e];
}
setComDaily(e, t) {
this.flag_sign_daily[e] = t;
r.default.getInstance().set_local_storeage();
return t;
}
getComSkin(e, t) {
return null == this.skin_Video[e] ? t : this.skin_Video[e];
}
setComSkin(e, t) {
this.skin_Video[e] = t;
r.default.getInstance().set_local_storeage();
return t;
}
}
a.default = l;
l._name = "NewGamePlayData";
cc._RF.pop();
};
