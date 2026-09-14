// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: UIUtils; dependency map: {"./LogUtils":"LogUtils","./Storage/ResManager":"ResManager","./Storage/Singleton":"Singleton"}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "006bfER66ZP/6NYjTlUxAiS", "UIUtils");
Object.defineProperty(a, "__esModule", {
value: !0
});
const i = e("./LogUtils"), o = e("./Storage/ResManager"), n = e("./Storage/Singleton");
a.default = class extends n.default {
randomInt(e, t) {
return Math.floor(Math.random() * (t - e + 1)) + e;
}
loadSpriteFrame(e, t, a) {
e && o.default.loadRes(e, cc.SpriteFrame, e => {
cc.isValid(a) && (null == t || t.call(a, e));
});
}
loadAndSetSpriteFrame(e, t) {
o.default.loadRes(e, cc.SpriteFrame, e => {
cc.isValid(t) && (t.spriteFrame = e);
});
}
httpDownloadImage(e, t) {
if (!e || !t) return;
const a = t.node.getContentSize();
o.default.loadRemotePng(e, e => {
if (cc.isValid(t)) {
t.spriteFrame = e;
t.node.setContentSize(a);
}
});
}
playSpine(e, t, a = !1, i = null) {
if (e && t) {
e.setAnimation(0, t, a);
e.setCompleteListener(() => {
null == i || i(e);
});
}
}
createSpine(e, t = null, a = new cc.Vec2(0, 0)) {
let i = new cc.Node().addComponent(sp.Skeleton);
i.skeletonData = LoadAllResources.getInstance().getSpineDataRes(e);
i.node.setPosition(a);
t && i.node.setParent(t);
return i;
}
getScaleRadio() {
let e = 0, t = cc.find("Canvas").getComponent(cc.Canvas);
if (1 == t.fitHeight) {
e = 1 * cc.winSize.width / t.designResolution.width;
i.default.Orange("高度适配, 比例 = " + e);
} else {
e = 1 * cc.winSize.height / t.designResolution.height;
i.default.Orange("宽度适配, 比例 = " + e);
}
return e;
}
loadScene(e, t = null) {
i.default.Orange("前往场景 ：" + e);
o.default.preloadScene(e, () => {
o.default.loadScene(e, t);
});
}
};
cc._RF.pop();
};
