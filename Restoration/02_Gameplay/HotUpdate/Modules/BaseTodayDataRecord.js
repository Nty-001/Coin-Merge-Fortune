// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: BaseTodayDataRecord; dependency map: {"../../Report/DateUtils":"DateUtils","./GameLocalData":"GameLocalData","./NewGamePlayData":"NewGamePlayData"}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "120c5CxUJ9HBZ/LrM2SSwOE", "BaseTodayDataRecord");
Object.defineProperty(a, "__esModule", {
value: !0
});
a.BaseTodayDataRecord = void 0;
const i = e("../../Report/DateUtils"), o = e("./GameLocalData"), n = e("./NewGamePlayData");
class s {
parseFromUserDefault(e) {
if (e) {
for (let t in this) "function" != typeof this[t] && e.hasOwnProperty(t) && (this[t] = e[t]);
this.adaptForDate();
}
}
adaptForDate() {
let e = i.default.getCurrentUTCTimeString(i.TimeFormat.YYYYMMDD), t = new Date(o.default.savedTimeStamp - 1e3);
i.default.getTimeString(t, i.TimeFormat.YYYYMMDD) != e && this.clearTodayData();
}
static getCurrentDate() {
const e = new Date();
return `${e.getFullYear()}-${e.getMonth() + 1}-${e.getDate()}`;
}
static checkAndUpdateLoginDays() {
const e = cc.sys.localStorage.getItem(this.LOGIN_DAYS_KEY) || "", t = this.getCurrentDate();
if (e !== t) {
const e = JSON.parse(cc.sys.localStorage.getItem(this.MERGE_RECORDS_KEY) || "[]");
if (0 === e.length) {
cc.sys.localStorage.setItem(this.LOGIN_DAYS_KEY, t);
e.push({
date: t,
dateLevelCount: 0
});
cc.sys.localStorage.setItem(this.MERGE_RECORDS_KEY, JSON.stringify(e));
return 0;
}
const a = e[e.length - 1];
if (a.dateLevelCount >= this.REQUIRED_LEVEL_COUNT) {
cc.sys.localStorage.setItem(this.LOGIN_DAYS_KEY, t);
e.push({
date: t,
dateLevelCount: 0
});
cc.sys.localStorage.setItem(this.MERGE_RECORDS_KEY, JSON.stringify(e));
} else if (a.date !== t) {
e[e.length - 1] = {
date: t,
dateLevelCount: 0
};
cc.sys.localStorage.setItem(this.LOGIN_DAYS_KEY, t);
cc.sys.localStorage.setItem(this.MERGE_RECORDS_KEY, JSON.stringify(e));
}
}
const a = JSON.parse(cc.sys.localStorage.getItem(this.MERGE_RECORDS_KEY) || "[]");
let i = 0;
for (let e = a.length - 1; e >= 0 && a[e].dateLevelCount >= this.REQUIRED_LEVEL_COUNT; e--) i++;
return i;
}
static addDailyLevel() {
const e = this.getCurrentDate(), t = JSON.parse(cc.sys.localStorage.getItem(this.MERGE_RECORDS_KEY) || "[]"), a = t.find(t => t.date === e);
if (a) {
a.dateLevelCount++;
cc.sys.localStorage.setItem(this.MERGE_RECORDS_KEY, JSON.stringify(t));
}
}
static setConsecutiveLoginDays(e) {
cc.sys.localStorage.removeItem(this.MERGE_RECORDS_KEY);
cc.sys.localStorage.removeItem(this.LOGIN_DAYS_KEY);
const t = new Date(), a = [];
for (let i = e - 1; i >= 0; i--) {
const e = new Date(t);
e.setDate(e.getDate() - i);
const o = `${e.getFullYear()}-${e.getMonth() + 1}-${e.getDate()}`;
a.push({
date: o,
dateLevelCount: this.REQUIRED_LEVEL_COUNT
});
}
if (0 === e) {
const e = new Date(t), i = `${e.getFullYear()}-${e.getMonth() + 1}-${e.getDate()}`;
a.push({
date: i,
dateLevelCount: 0
});
}
cc.sys.localStorage.setItem(this.MERGE_RECORDS_KEY, JSON.stringify(a));
cc.sys.localStorage.setItem(this.LOGIN_DAYS_KEY, `${t.getFullYear()}-${t.getMonth() + 1}-${t.getDate()}`);
}
static checkLoginDays() {
const e = this.getCurrentDate(), t = JSON.parse(cc.sys.localStorage.getItem(this.MERGE_RECORDS_KEY_DAILY) || "[]");
let a = t.find(t => t.date === e), i = o.default.getInstance().getData(n.default), s = i.today_pass_count || 0;
if (a) a.dateLevelCount = s; else {
i.today_pass_count = 0;
o.default.getInstance().set_local_storeage();
t.push({
date: e,
dateLevelCount: i.today_pass_count
});
}
cc.sys.localStorage.setItem(this.MERGE_RECORDS_KEY_DAILY, JSON.stringify(t));
cc.sys.localStorage.setItem(this.LOGIN_DAYS_KEY_DAILY, e);
let r = 0;
for (let e = 0; e < t.length; e++) t[e].dateLevelCount >= this.DAILY_REQUIRED_PASS_COUNT && r++;
return r;
}
static setDebugLoginDays(e) {
cc.sys.localStorage.removeItem(this.LOGIN_DAYS_KEY_DAILY);
cc.sys.localStorage.removeItem(this.MERGE_RECORDS_KEY_DAILY);
const t = this.getCurrentDate(), a = [], i = new Date();
o.default.getInstance().getData(n.default).today_pass_count = this.DAILY_REQUIRED_PASS_COUNT;
o.default.getInstance().saveToUserDefault();
for (let t = e - 1; t >= 0; t--) {
const e = new Date(i);
e.setDate(e.getDate() - t);
const o = `${e.getFullYear()}-${e.getMonth() + 1}-${e.getDate()}`;
a.push({
date: o,
dateLevelCount: this.DAILY_REQUIRED_PASS_COUNT
});
}
if (0 === e) {
const e = new Date(i), t = `${e.getFullYear()}-${e.getMonth() + 1}-${e.getDate()}`;
a.push({
date: t,
dateLevelCount: 0
});
}
cc.sys.localStorage.setItem(this.LOGIN_DAYS_KEY_DAILY, t);
cc.sys.localStorage.setItem(this.MERGE_RECORDS_KEY_DAILY, JSON.stringify(a));
}
static checkLoginDaysPass() {
const e = this.getCurrentDate(), t = cc.sys.localStorage.getItem(this.LOGIN_DAYS_KEY_DAILY) || "";
cc.sys.localStorage.setItem(this.LOGIN_DAYS_KEY_DAILY, e);
return t === e;
}
static clearLoginRecords() {
cc.sys.localStorage.removeItem(this.MERGE_RECORDS_KEY);
cc.sys.localStorage.removeItem(this.LOGIN_DAYS_KEY);
cc.sys.localStorage.removeItem(this.LOGIN_DAYS_KEY_DAILY);
cc.sys.localStorage.removeItem(this.MERGE_RECORDS_KEY_DAILY);
}
static getLoginRecords() {
return JSON.parse(cc.sys.localStorage.getItem(this.MERGE_RECORDS_KEY) || "[]");
}
static getDailyLoginRecords() {
return JSON.parse(cc.sys.localStorage.getItem(this.MERGE_RECORDS_KEY_DAILY) || "[]");
}
}
a.BaseTodayDataRecord = s;
s.LOGIN_DAYS_KEY = "LOGIN_DAYS_KEY";
s.MERGE_RECORDS_KEY = "MERGE_RECORDS_KEY";
s.REQUIRED_LEVEL_COUNT = 20;
s.LOGIN_DAYS_KEY_DAILY = "LOGIN_DAYS_KEY_DAILY";
s.MERGE_RECORDS_KEY_DAILY = "MERGE_RECORDS_KEY_DAILY";
s.DAILY_REQUIRED_PASS_COUNT = 15;
cc._RF.pop();
};
