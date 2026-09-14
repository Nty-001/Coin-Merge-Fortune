// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: GameRealWDTXTips; dependency map: {"../BaseUIManager/BaseUI":"BaseUI","../BaseUIManager/Storage/GameLocalData":"GameLocalData","../BaseUIManager/Storage/PlayData":"PlayData","../BaseUIManager/Storage/TouchButton":"TouchButton","../LanguageControl/GameManagement":"GameManagement","../LanguageControl/Lab":"Lab"}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "2319cvN5GdPbo9o0vIt5Dp9", "GameRealWDTXTips");
var i = this && this.__decorate || function(e, t, a, i) {
var o, n = arguments.length, s = n < 3 ? t : null === i ? i = Object.getOwnPropertyDescriptor(t, a) : i;
if ("object" == typeof Reflect && "function" == typeof Reflect.decorate) s = Reflect.decorate(e, t, a, i); else for (var r = e.length - 1; r >= 0; r--) (o = e[r]) && (s = (n < 3 ? o(s) : n > 3 ? o(t, a, s) : o(t, a)) || s);
return n > 3 && s && Object.defineProperty(t, a, s), s;
};
Object.defineProperty(a, "__esModule", {
value: !0
});
const o = e("../BaseUIManager/BaseUI"), n = e("../BaseUIManager/Storage/GameLocalData"), s = e("../BaseUIManager/Storage/PlayData"), r = e("../BaseUIManager/Storage/TouchButton"), l = e("../LanguageControl/GameManagement"), c = e("../LanguageControl/Lab"), {ccclass: d, property: h} = cc._decorator;
let u = class extends o.default {
constructor() {
super(...arguments);
this.btnClose = null;
this.title = null;
this.topTips = null;
this.coinLabel = null;
this.progressBar = null;
this.progressLabel = null;
this.tips = null;
this.btnLabel = null;
this.currentIndex = 0;
this.coinConfig = null;
}
onLoad() {
super.onLoad();
this.btnClose.addComponent(r.default).registerTouchEvent(() => {
this.on_close_call();
});
this.btnLabel.node.parent.addComponent(r.default).registerTouchEvent(() => {
this.on_close_call();
});
}
show(e) {
super.show(e);
let t = e.param;
this.currentIndex = t.currentIndex;
this.coinConfig = l.default.newfake_products.new_Fake_products[this.currentIndex];
this.coinLabel.string = l.default.getRealMonstr(this.coinConfig.withdrawAmount);
this.setProgress(this.currentIndex);
this.btnLabel.string = c.default.getlab("108");
}
getConfig() {
return l.default.newfake_products;
}
setProgress(e) {
this.playerData = n.default.getInstance().getData(s.default);
let t = this.playerData.newFakeMoneyWithdraw[e], a = this.getConfig().new_Fake_products[e];
if (0 == t) {
this.progressBar.progress = this.playerData.coin1024Number / a.condition_merge;
let e = Math.max(0, a.condition_merge - this.playerData.coin1024Number);
this.tips.string = c.default.getlab("106").replace("%{0}", a.condition_merge + "").replace("%{0}", e + "");
} else if (1 == t) {
this.progressBar.progress = this.playerData.watch_video_count / a.condition_ad;
let e = Math.max(0, a.condition_ad - this.playerData.watch_video_count);
this.tips.string = c.default.getlab("107").replace("%{0}", e + "");
} else if (2 == t) {
this.progressBar.progress = this.playerData.fakeMoney / a.withdrawAmount;
let e = Math.max(a.withdrawAmount - this.playerData.fakeMoney, 0), t = l.default.getmonstr(e);
this.tips.string = c.default.getlab("56").replace("%{0}", t + "");
} else if (3 == t) {
let e = a.condition_login_days > 0 ? this.playerData.loginDays / a.condition_login_days : 1;
this.progressBar.progress = Math.min(e, 1);
let t = Math.max(a.condition_login_days - this.playerData.loginDays, 0), i = a.condition_daily_merge;
this.tips.string = c.default.getlab("60").replace("%{0}", t + "").replace("%{1}", i + "");
} else if (4 == t) {
this.progressBar.progress = this.playerData.watch_video_count / a.condition_video;
let e = Math.max(a.condition_video - this.playerData.watch_video_count, 0);
this.tips.string = c.default.getlab("61").replace("%{0}", e + "");
}
let i = Math.min(this.progressBar.progress, 1);
this.progressLabel.string = Math.floor(100 * i) + "%";
}
};
i([ h(cc.Node) ], u.prototype, "btnClose", void 0);
i([ h(cc.Label) ], u.prototype, "title", void 0);
i([ h(cc.Label) ], u.prototype, "topTips", void 0);
i([ h(cc.Label) ], u.prototype, "coinLabel", void 0);
i([ h(cc.ProgressBar) ], u.prototype, "progressBar", void 0);
i([ h(cc.Label) ], u.prototype, "progressLabel", void 0);
i([ h(cc.Label) ], u.prototype, "tips", void 0);
i([ h(cc.Label) ], u.prototype, "btnLabel", void 0);
u = i([ o.registerUIPath("GameDialog/GameRealWDTXTips"), d ], u);
a.default = u;
cc._RF.pop();
};
