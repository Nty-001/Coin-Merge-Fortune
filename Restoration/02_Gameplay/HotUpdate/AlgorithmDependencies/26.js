// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: 26; dependency map: {"./cipher-core":4,"./core":5,"./enc-base64":6,"./evpkdf":9,"./md5":14}
module.exports = function(e, t, a) {
this, i = function(e) {
(function() {
var t = e, a = t.lib.StreamCipher, i = t.algo, o = [], n = [], s = [], r = i.RabbitLegacy = a.extend({
_doReset: function() {
var e = this._key.words, t = this.cfg.iv, a = this._X = [ e[0], e[3] << 16 | e[2] >>> 16, e[1], e[0] << 16 | e[3] >>> 16, e[2], e[1] << 16 | e[0] >>> 16, e[3], e[2] << 16 | e[1] >>> 16 ], i = this._C = [ e[2] << 16 | e[2] >>> 16, 4294901760 & e[0] | 65535 & e[1], e[3] << 16 | e[3] >>> 16, 4294901760 & e[1] | 65535 & e[2], e[0] << 16 | e[0] >>> 16, 4294901760 & e[2] | 65535 & e[3], e[1] << 16 | e[1] >>> 16, 4294901760 & e[3] | 65535 & e[0] ];
this._b = 0;
for (var o = 0; o < 4; o++) l.call(this);
for (o = 0; o < 8; o++) i[o] ^= a[o + 4 & 7];
if (t) {
var n = t.words, s = n[0], r = n[1], c = 16711935 & (s << 8 | s >>> 24) | 4278255360 & (s << 24 | s >>> 8), d = 16711935 & (r << 8 | r >>> 24) | 4278255360 & (r << 24 | r >>> 8), h = c >>> 16 | 4294901760 & d, u = d << 16 | 65535 & c;
i[0] ^= c;
i[1] ^= h;
i[2] ^= d;
i[3] ^= u;
i[4] ^= c;
i[5] ^= h;
i[6] ^= d;
i[7] ^= u;
for (o = 0; o < 4; o++) l.call(this);
}
},
_doProcessBlock: function(e, t) {
var a = this._X;
l.call(this);
o[0] = a[0] ^ a[5] >>> 16 ^ a[3] << 16;
o[1] = a[2] ^ a[7] >>> 16 ^ a[5] << 16;
o[2] = a[4] ^ a[1] >>> 16 ^ a[7] << 16;
o[3] = a[6] ^ a[3] >>> 16 ^ a[1] << 16;
for (var i = 0; i < 4; i++) {
o[i] = 16711935 & (o[i] << 8 | o[i] >>> 24) | 4278255360 & (o[i] << 24 | o[i] >>> 8);
e[t + i] ^= o[i];
}
},
blockSize: 4,
ivSize: 2
});
function l() {
for (var e = this._X, t = this._C, a = 0; a < 8; a++) n[a] = t[a];
t[0] = t[0] + 1295307597 + this._b | 0;
t[1] = t[1] + 3545052371 + (t[0] >>> 0 < n[0] >>> 0 ? 1 : 0) | 0;
t[2] = t[2] + 886263092 + (t[1] >>> 0 < n[1] >>> 0 ? 1 : 0) | 0;
t[3] = t[3] + 1295307597 + (t[2] >>> 0 < n[2] >>> 0 ? 1 : 0) | 0;
t[4] = t[4] + 3545052371 + (t[3] >>> 0 < n[3] >>> 0 ? 1 : 0) | 0;
t[5] = t[5] + 886263092 + (t[4] >>> 0 < n[4] >>> 0 ? 1 : 0) | 0;
t[6] = t[6] + 1295307597 + (t[5] >>> 0 < n[5] >>> 0 ? 1 : 0) | 0;
t[7] = t[7] + 3545052371 + (t[6] >>> 0 < n[6] >>> 0 ? 1 : 0) | 0;
this._b = t[7] >>> 0 < n[7] >>> 0 ? 1 : 0;
for (a = 0; a < 8; a++) {
var i = e[a] + t[a], o = 65535 & i, r = i >>> 16, l = ((o * o >>> 17) + o * r >>> 15) + r * r, c = ((4294901760 & i) * i | 0) + ((65535 & i) * i | 0);
s[a] = l ^ c;
}
e[0] = s[0] + (s[7] << 16 | s[7] >>> 16) + (s[6] << 16 | s[6] >>> 16) | 0;
e[1] = s[1] + (s[0] << 8 | s[0] >>> 24) + s[7] | 0;
e[2] = s[2] + (s[1] << 16 | s[1] >>> 16) + (s[0] << 16 | s[0] >>> 16) | 0;
e[3] = s[3] + (s[2] << 8 | s[2] >>> 24) + s[1] | 0;
e[4] = s[4] + (s[3] << 16 | s[3] >>> 16) + (s[2] << 16 | s[2] >>> 16) | 0;
e[5] = s[5] + (s[4] << 8 | s[4] >>> 24) + s[3] | 0;
e[6] = s[6] + (s[5] << 16 | s[5] >>> 16) + (s[4] << 16 | s[4] >>> 16) | 0;
e[7] = s[7] + (s[6] << 8 | s[6] >>> 24) + s[5] | 0;
}
t.RabbitLegacy = a._createHelper(r);
})();
return e.RabbitLegacy;
}, "object" == typeof a ? t.exports = a = i(e("./core"), e("./enc-base64"), e("./md5"), e("./evpkdf"), e("./cipher-core")) : "function" == typeof define && define.amd ? define([ "./core", "./enc-base64", "./md5", "./evpkdf", "./cipher-core" ], i) : i(this.CryptoJS);
var i;
};
