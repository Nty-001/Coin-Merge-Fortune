// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: ResManager; dependency map: {}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "1560egr9bNGO6j+Ixp3cBFk", "ResManager");
var i = this && this.__awaiter || function(e, t, a, i) {
return new (a || (a = Promise))(function(o, n) {
function s(e) {
try {
l(i.next(e));
} catch (e) {
n(e);
}
}
function r(e) {
try {
l(i.throw(e));
} catch (e) {
n(e);
}
}
function l(e) {
e.done ? o(e.value) : (t = e.value, t instanceof a ? t : new a(function(e) {
e(t);
})).then(s, r);
var t;
}
l((i = i.apply(e, t || [])).next());
});
};
Object.defineProperty(a, "__esModule", {
value: !0
});
class o {
static loadSpriteFrameAsync(e) {
return new Promise((t, a) => {
cc.loader.loadRes(e, cc.SpriteFrame, function(i, o) {
i ? t(o) : a(new Error(`Failed to load sprite frame: ${e}`));
});
});
}
static loadPrefab(e) {
return this.loadResByPromise(e, cc.Prefab);
}
static loadJson(e) {
return this.loadResByPromise(e, cc.JsonAsset);
}
static loadRemoteJson(e, t) {
cc.assetManager.loadRemote(e, (e, a) => {
e || null == t || t(a);
});
}
static loadRemotePng(e, t) {
"null" != e && e ? cc.assetManager.loadRemote(`${e}`, {
ext: ".png"
}, (e, a) => {
e || null == t || t(new cc.SpriteFrame(a));
}) : null == t || t(null);
}
static preloadScene(e, t) {
cc.director.preloadScene(e, t);
}
static loadScene(e, t) {
cc.director.loadScene(e, t);
}
static loadRes(e, t, a, i = null) {
cc.resources.load(e, t, (t, o) => {
if (t) {
console.log(`加载资源${e}出现错误:` + t.message);
null == i || i();
} else null == a || a(o);
});
}
static loadResByPromise(e, t) {
return i(this, void 0, void 0, function*() {
return new Promise((a, i) => {
this.loadRes(e, t, a, i);
}).catch(t => {
console.log(`loadResByPromise catch; url = ${e}, reason = `, t);
return null;
});
});
}
static loadDir(e, t, a, i) {
cc.resources.loadDir(e, t, a, (t, a) => {
t ? console.log(`加载目录${e}出现错误:` + t.message) : i(a);
});
}
static loadOssImage(e, t, a = "jcdls") {
let i = `${o.ossPath}/${a}/` + e;
cc.sys.isBrowser && (i = `${o.nodeServer}/getImage?imageUrl=` + i);
o.loadRemotePng(i, t);
}
static loadOssJson(e, t, a = "jcdls") {
let i = `${o.ossPath}/${a}/` + e;
cc.sys.isBrowser && (i = `${o.nodeServer}/getJson?jsonUrl=` + i);
o.loadRemoteJson(i, t);
}
}
a.default = o;
o.ossPath = "http://shua-music.shinet.cn";
o.nodeServer = "http://localhost:6080";
cc._RF.pop();
};
