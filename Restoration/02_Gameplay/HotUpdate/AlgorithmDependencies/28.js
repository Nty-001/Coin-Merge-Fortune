// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: 28; dependency map: {"./cipher-core":4,"./core":5,"./enc-base64":6,"./evpkdf":9,"./md5":14}
module.exports = function(e, t, a) {
this, i = function(e) {
(function() {
var t = e, a = t.lib.StreamCipher, i = t.algo, o = i.RC4 = a.extend({
_doReset: function() {
for (var e = this._key, t = e.words, a = e.sigBytes, i = this._S = [], o = 0; o < 256; o++) i[o] = o;
o = 0;
for (var n = 0; o < 256; o++) {
var s = o % a, r = t[s >>> 2] >>> 24 - s % 4 * 8 & 255;
n = (n + i[o] + r) % 256;
var l = i[o];
i[o] = i[n];
i[n] = l;
}
this._i = this._j = 0;
},
_doProcessBlock: function(e, t) {
e[t] ^= n.call(this);
},
keySize: 8,
ivSize: 0
});
function n() {
for (var e = this._S, t = this._i, a = this._j, i = 0, o = 0; o < 4; o++) {
a = (a + e[t = (t + 1) % 256]) % 256;
var n = e[t];
e[t] = e[a];
e[a] = n;
i |= e[(e[t] + e[a]) % 256] << 24 - 8 * o;
}
this._i = t;
this._j = a;
return i;
}
t.RC4 = a._createHelper(o);
var s = i.RC4Drop = o.extend({
cfg: o.cfg.extend({
drop: 192
}),
_doReset: function() {
o._doReset.call(this);
for (var e = this.cfg.drop; e > 0; e--) n.call(this);
}
});
t.RC4Drop = a._createHelper(s);
})();
return e.RC4;
}, "object" == typeof a ? t.exports = a = i(e("./core"), e("./enc-base64"), e("./md5"), e("./evpkdf"), e("./cipher-core")) : "function" == typeof define && define.amd ? define([ "./core", "./enc-base64", "./md5", "./evpkdf", "./cipher-core" ], i) : i(this.CryptoJS);
var i;
};
