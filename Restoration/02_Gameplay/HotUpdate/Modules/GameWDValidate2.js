// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: GameWDValidate2; dependency map: {"../BaseUIManager/BaseUI":"BaseUI","../BaseUIManager/Storage/GameLocalData":"GameLocalData","../BaseUIManager/Storage/NewGamePlayData":"NewGamePlayData","../BaseUIManager/Storage/TouchButton":"TouchButton","../BaseUIManager/UIManagerNew":"UIManagerNew","../LanguageControl/GameManagement":"GameManagement","../LanguageControl/Lab":"Lab","../WithDraw/GameRealWDDialog":"GameRealWDDialog"}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "41eccpsYg1MSoKGvxX4hiqo", "GameWDValidate2");
var i = this && this.__decorate || function(e, t, a, i) {
var o, n = arguments.length, s = n < 3 ? t : null === i ? i = Object.getOwnPropertyDescriptor(t, a) : i;
if ("object" == typeof Reflect && "function" == typeof Reflect.decorate) s = Reflect.decorate(e, t, a, i); else for (var r = e.length - 1; r >= 0; r--) (o = e[r]) && (s = (n < 3 ? o(s) : n > 3 ? o(t, a, s) : o(t, a)) || s);
return n > 3 && s && Object.defineProperty(t, a, s), s;
};
Object.defineProperty(a, "__esModule", {
value: !0
});
const o = e("../LanguageControl/GameManagement"), n = e("../LanguageControl/Lab"), s = e("../BaseUIManager/Storage/GameLocalData"), r = e("../BaseUIManager/Storage/NewGamePlayData"), l = e("../BaseUIManager/Storage/TouchButton"), c = e("../BaseUIManager/BaseUI"), d = e("../BaseUIManager/UIManagerNew"), h = e("../WithDraw/GameRealWDDialog"), {ccclass: u, property: g} = cc._decorator;
let p = class extends c.default {
constructor() {
super(...arguments);
this.title = null;
this.platform = null;
this.accountLabel = null;
this.coinLabel = null;
this.tips = null;
this.btnLabel = null;
this.progress = null;
this.proLabel = null;
this.allConditionsMet = !1;
}
onLoad() {
super.onLoad();
this.btnLabel.node.parent.addComponent(l.default).registerTouchEvent(() => {
this.close_call();
let e = this.allConditionsMet ? n.default.getlab("102") : n.default.getlab("101");
d.default.show_toast({
text: e
});
});
}
close_call() {
this.on_close_call();
}
show(e) {
super.show(e);
this.param = e.param;
this.title.string = n.default.getlab("48");
const t = s.default.getInstance().getData(r.default);
o.default.loadSpriteFrame("texture/platform/" + t.selectPlatform, e => {
this.platform.spriteFrame = e;
});
this.accountLabel.string = t.accountName;
this.coinLabel.string = n.default.getlab("49") + ": " + o.default.getmonstr(this.param.withdrawAmount);
this.btnLabel.string = n.default.getlab("1");
let a = o.default.globalData.global.tixian_products[this.param.currentIndex], i = a.withdrawAmount, l = a.condition_2, c = a.condition_3, d = a.condition_4, u = a.condition_5, g = i * o.default.GetCountryDang(), p = s.default.getInstance().getData(r.default).Passlevel + 1;
this.allConditionsMet = t.red_bag >= g && t.watch_video_Singlecount[this.param.currentIndex] >= l && p >= c && t.watch_video_count >= d && t.watch_video_count >= u;
if (this.allConditionsMet) {
t.fakewithdrawRecord[this.param.currentIndex] = 1;
h.default.instance && h.default.instance.showRewardView();
s.default.getInstance().saveToUserDefault();
}
let f = !1, m = [ 0, 1, 2, 3, 4 ];
for (const e of m) if (!f) switch (e) {
case 1:
console.log(" 条件1   ", t.watch_video_Singlecount[this.param.currentIndex], l);
if (t.watch_video_Singlecount[this.param.currentIndex] < l) {
f = !0;
let e = t.watch_video_Singlecount[this.param.currentIndex] / l, a = l - t.watch_video_Singlecount[this.param.currentIndex];
this.proLabel.string = t.watch_video_Singlecount[this.param.currentIndex] + "/" + l;
this.tips.string = n.default.getlab("20", a);
this.progress.progress = e;
}
break;

case 2:
console.log(" 条件2   ", p, c);
if (p < c) {
f = !0;
let e = p / c;
this.proLabel.string = p + "/" + c;
this.progress.progress = e;
this.tips.string = n.default.getlab("21", c);
}
break;

case 3:
console.log(" 条件3   ", t.watch_video_count, d, this.allConditionsMet);
if (t.watch_video_count < d) {
f = !0;
let e = t.watch_video_count / d;
this.proLabel.string = t.watch_video_count + "/" + d;
this.progress.progress = e;
this.tips.string = n.default.getlab("22", d);
}
break;

case 4:
console.log(" 条件4   ", t.watch_video_count, u);
if (t.watch_video_count < u) {
f = !0;
let e = t.watch_video_count / u;
this.proLabel.string = t.watch_video_count + "/" + u;
this.progress.progress = e;
this.tips.string = n.default.getlab("22", u);
}
}
if (this.allConditionsMet) {
this.proLabel.string = "100%";
this.progress.progress = 1;
this.tips.string = n.default.getlab("103");
}
}
};
i([ g(cc.Label) ], p.prototype, "title", void 0);
i([ g(cc.Sprite) ], p.prototype, "platform", void 0);
i([ g(cc.Label) ], p.prototype, "accountLabel", void 0);
i([ g(cc.Label) ], p.prototype, "coinLabel", void 0);
i([ g(cc.Label) ], p.prototype, "tips", void 0);
i([ g(cc.Label) ], p.prototype, "btnLabel", void 0);
i([ g(cc.ProgressBar) ], p.prototype, "progress", void 0);
i([ g(cc.Label) ], p.prototype, "proLabel", void 0);
p = i([ c.registerUIPath("GameDialog/GameWDValidate2"), u ], p);
a.default = p;
cc._RF.pop();
};
