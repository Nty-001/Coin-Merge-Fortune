// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/base/assets/assets/main/index.ba75b.js
// Module: ResManager; dependency map: {}
module.exports = function(e, t, o) {
"use strict";
cc._RF.push(t, "901b4y1sCZPBoSaqHlRpfGK", "ResManager");
var a = this && this.__awaiter || function(e, t, o, a) {
return new (o || (o = Promise))(function(s, n) {
function i(e) {
try {
l(a.next(e));
} catch (e) {
n(e);
}
}
function c(e) {
try {
l(a.throw(e));
} catch (e) {
n(e);
}
}
function l(e) {
e.done ? s(e.value) : (t = e.value, t instanceof o ? t : new o(function(e) {
e(t);
})).then(i, c);
var t;
}
l((a = a.apply(e, t || [])).next());
});
};
Object.defineProperty(o, "__esModule", {
value: !0
});
o.default = class {
static loadSpriteFrame(e, t) {
cc.loader.loadRes(e, cc.SpriteFrame, function(e, o) {
e ? console.log("UIUtils::loadSpriteFrame error " + e, o) : t(o);
});
}
static loadPrefab(e) {
return this.loadResByPromise(e, cc.Prefab);
}
static loadJson(e) {
return this.loadResByPromise(e, cc.JsonAsset);
}
static preloadScene(e, t) {
cc.director.preloadScene(e, t);
}
static loadScene(e, t) {
cc.director.loadScene(e, t);
}
static loadRes(e, t, o, a = null) {
cc.resources.load(e, t, (t, s) => {
if (t) {
console.log(`resource${e}err:` + t.message);
null == a || a();
} else null == o || o(s);
});
}
static loadResByPromise(e, t) {
return a(this, void 0, void 0, function*() {
return new Promise((o, a) => {
this.loadRes(e, t, o, a);
}).catch(t => {
console.log(`loadResByPromise catch; url = ${e}, reason = `, t);
return null;
});
});
}
static loadDir(e, t, o, a) {
cc.resources.loadDir(e, t, o, (t, o) => {
t ? console.log(`resource${e}err:` + t.message) : a(o);
});
}
};
cc._RF.pop();
};
