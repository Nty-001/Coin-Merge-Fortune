// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: MKNoticeConsts; dependency map: {"../EventListener/MKNotice":"MKNotice","./GameEventConsts":"GameEventConsts"}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "d44e8dQm8dNbLmvL3qFYUEU", "MKNoticeConsts");
Object.defineProperty(a, "__esModule", {
value: !0
});
a.MKNotice_FlyMoreGold = void 0;
const i = e("../EventListener/MKNotice"), o = e("./GameEventConsts");
a.MKNotice_FlyMoreGold = class extends i.default {
constructor(e, t = null, a = null, i = -1, n = !1, s = 1, r = !0) {
super(o.GameEventName.FlyMoreGoldNtf);
this.startWorldPos = null;
this.parentNode = null;
this.fixedRate = -1;
this.isOnlyFlyGold = !1;
this.speed = 1;
this.isPlaySound = !0;
t || (t = cc.v2(cc.winSize.width / 2, cc.winSize.height / 2));
this.yuanbaoNum = e;
this.startWorldPos = t;
this.parentNode = a;
this.fixedRate = i;
this.isOnlyFlyGold = n;
this.speed = s;
this.isPlaySound = r;
}
};
cc._RF.pop();
};
