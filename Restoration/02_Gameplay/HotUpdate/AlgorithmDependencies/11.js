// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: 11; dependency map: {"./core":5}
module.exports = function(e, t, a) {
this, i = function(e) {
a = (t = e).lib.Base, i = t.enc.Utf8, t.algo.HMAC = a.extend({
init: function(e, t) {
e = this._hasher = new e.init();
"string" == typeof t && (t = i.parse(t));
var a = e.blockSize, o = 4 * a;
t.sigBytes > o && (t = e.finalize(t));
t.clamp();
for (var n = this._oKey = t.clone(), s = this._iKey = t.clone(), r = n.words, l = s.words, c = 0; c < a; c++) {
r[c] ^= 1549556828;
l[c] ^= 909522486;
}
n.sigBytes = s.sigBytes = o;
this.reset();
},
reset: function() {
var e = this._hasher;
e.reset();
e.update(this._iKey);
},
update: function(e) {
this._hasher.update(e);
return this;
},
finalize: function(e) {
var t = this._hasher, a = t.finalize(e);
t.reset();
return t.finalize(this._oKey.clone().concat(a));
}
});
var t, a, i;
}, "object" == typeof a ? t.exports = a = i(e("./core")) : "function" == typeof define && define.amd ? define([ "./core" ], i) : i(this.CryptoJS);
var i;
};
