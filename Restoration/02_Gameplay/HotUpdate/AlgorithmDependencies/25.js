// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: 25; dependency map: {"./core":5,"./hmac":11,"./sha256":32}
module.exports = function(e, t, a) {
this, i = function(e) {
i = (a = (t = e).lib).Base, o = a.WordArray, s = (n = t.algo).SHA256, r = n.HMAC, 
l = n.PBKDF2 = i.extend({
cfg: i.extend({
keySize: 4,
hasher: s,
iterations: 25e4
}),
init: function(e) {
this.cfg = this.cfg.extend(e);
},
compute: function(e, t) {
for (var a = this.cfg, i = r.create(a.hasher, e), n = o.create(), s = o.create([ 1 ]), l = n.words, c = s.words, d = a.keySize, h = a.iterations; l.length < d; ) {
var u = i.update(t).finalize(s);
i.reset();
for (var g = u.words, p = g.length, f = u, m = 1; m < h; m++) {
f = i.finalize(f);
i.reset();
for (var y = f.words, _ = 0; _ < p; _++) g[_] ^= y[_];
}
n.concat(u);
c[0]++;
}
n.sigBytes = 4 * d;
return n;
}
}), t.PBKDF2 = function(e, t, a) {
return l.create(a).compute(e, t);
};
var t, a, i, o, n, s, r, l;
return e.PBKDF2;
}, "object" == typeof a ? t.exports = a = i(e("./core"), e("./sha256"), e("./hmac")) : "function" == typeof define && define.amd ? define([ "./core", "./sha256", "./hmac" ], i) : i(this.CryptoJS);
var i;
};
