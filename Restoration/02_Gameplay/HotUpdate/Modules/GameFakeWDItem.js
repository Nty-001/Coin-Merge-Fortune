// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: GameFakeWDItem; dependency map: {"../BaseUIManager/Storage/GameLocalData":"GameLocalData","../BaseUIManager/Storage/PlayData":"PlayData","../LanguageControl/GameManagement":"GameManagement","../LanguageControl/Lab":"Lab"}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "828fa1heI1JZrDtwrOIJ5qr", "GameFakeWDItem");
var i = this && this.__decorate || function(e, t, a, i) {
var o, n = arguments.length, s = n < 3 ? t : null === i ? i = Object.getOwnPropertyDescriptor(t, a) : i;
if ("object" == typeof Reflect && "function" == typeof Reflect.decorate) s = Reflect.decorate(e, t, a, i); else for (var r = e.length - 1; r >= 0; r--) (o = e[r]) && (s = (n < 3 ? o(s) : n > 3 ? o(t, a, s) : o(t, a)) || s);
return n > 3 && s && Object.defineProperty(t, a, s), s;
};
Object.defineProperty(a, "__esModule", {
value: !0
});
const o = e("../BaseUIManager/Storage/GameLocalData"), n = e("../BaseUIManager/Storage/PlayData"), s = e("../LanguageControl/GameManagement"), r = e("../LanguageControl/Lab"), {ccclass: l, property: c} = cc._decorator;
let d = class extends cc.Component {
constructor() {
super(...arguments);
this.selectBg = null;
this.unselectBg = null;
this.moneyTxt = null;
this.contiditionTxt1 = null;
this.contiditionTxt2 = null;
this.progressBar = null;
}
start() {}
initdata(e) {
const t = o.default.getInstance().getData(n.default);
let a = t.fakeMoney, i = t.coin1024Number, l = t.loginDays, c = (t.today1024NumberCoin, 
t.watch_video_count);
this.moneyTxt.string = s.default.getRealMonstr(e.withdrawAmount);
let d = 0, h = 0;
if (a < e.withdrawAmount) {
d = a;
h = e.withdrawAmount;
let t = s.default.getmonstr(e.withdrawAmount);
this.contiditionTxt1.string = r.default.getlab("55").replace("%{0}", t);
let i = s.default.getmonstr(Math.max(e.withdrawAmount - a, 0));
this.contiditionTxt2.string = r.default.getlab("56").replace("%{0}", i);
} else if (i < e.condition_merge) {
d = i;
h = e.condition_merge;
this.contiditionTxt1.string = r.default.getlab("57").replace("%{0}", e.condition_merge);
this.contiditionTxt2.string = r.default.getlab("58").replace("%{0}", Math.max(e.condition_merge - i, 0) + "");
} else if (l < e.condition_login_days) {
d = l;
h = e.condition_login_days;
this.contiditionTxt1.string = r.default.getlab("59").replace("%{0}", e.condition_login_days).replace("%{1}", e.condition_daily_merge);
this.contiditionTxt2.string = r.default.getlab("60").replace("%{0}", Math.max(e.condition_login_days - l, 0) + "").replace("%{1}", e.condition_daily_merge + "");
} else {
d = c;
h = e.condition_video;
this.contiditionTxt1.string = r.default.getlab("61").replace("%{0}", e.condition_video);
this.contiditionTxt2.string = r.default.getlab("62").replace("%{0}", Math.max(e.condition_video - c, 0) + "");
}
let u = Math.min(d / h, 1);
this.progressBar.progress = u;
}
refreshSelectBGSprite(e) {
this.selectBg.active = e;
this.unselectBg.active = !e;
return [ this.contiditionTxt2.string, this.progressBar.progress ];
}
};
i([ c(cc.Node) ], d.prototype, "selectBg", void 0);
i([ c(cc.Node) ], d.prototype, "unselectBg", void 0);
i([ c(cc.Label) ], d.prototype, "moneyTxt", void 0);
i([ c(cc.Label) ], d.prototype, "contiditionTxt1", void 0);
i([ c(cc.Label) ], d.prototype, "contiditionTxt2", void 0);
i([ c(cc.ProgressBar) ], d.prototype, "progressBar", void 0);
d = i([ l ], d);
a.default = d;
cc._RF.pop();
};
