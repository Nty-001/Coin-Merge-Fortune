// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: 24; dependency map: {"./cipher-core":4,"./core":5}
module.exports = function(e, t, a) {
this, i = function(e) {
e.pad.ZeroPadding = {
pad: function(e, t) {
var a = 4 * t;
e.clamp();
e.sigBytes += a - (e.sigBytes % a || a);
},
unpad: function(e) {
var t = e.words, a = e.sigBytes - 1;
for (a = e.sigBytes - 1; a >= 0; a--) if (t[a >>> 2] >>> 24 - a % 4 * 8 & 255) {
e.sigBytes = a + 1;
break;
}
}
};
return e.pad.ZeroPadding;
}, "object" == typeof a ? t.exports = a = i(e("./core"), e("./cipher-core")) : "function" == typeof define && define.amd ? define([ "./core", "./cipher-core" ], i) : i(this.CryptoJS);
var i;
};
