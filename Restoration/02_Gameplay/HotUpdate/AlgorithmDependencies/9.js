// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: 9; dependency map: {"./core":5,"./hmac":11,"./sha1":30}
module.exports = function(e, t, a) {
this, i = function(e) {
i = (a = (t = e).lib).Base, o = a.WordArray, s = (n = t.algo).MD5, r = n.EvpKDF = i.extend({
cfg: i.extend({
keySize: 4,
hasher: s,
iterations: 1
}),
init: function(e) {
this.cfg = this.cfg.extend(e);
},
compute: function(e, t) {
for (var a, i = this.cfg, n = i.hasher.create(), s = o.create(), r = s.words, l = i.keySize, c = i.iterations; r.length < l; ) {
a && n.update(a);
a = n.update(e).finalize(t);
n.reset();
for (var d = 1; d < c; d++) {
a = n.finalize(a);
n.reset();
}
s.concat(a);
}
s.sigBytes = 4 * l;
return s;
}
}), t.EvpKDF = function(e, t, a) {
return r.create(a).compute(e, t);
};
var t, a, i, o, n, s, r;
return e.EvpKDF;
}, "object" == typeof a ? t.exports = a = i(e("./core"), e("./sha1"), e("./hmac")) : "function" == typeof define && define.amd ? define([ "./core", "./sha1", "./hmac" ], i) : i(this.CryptoJS);
var i;
};
