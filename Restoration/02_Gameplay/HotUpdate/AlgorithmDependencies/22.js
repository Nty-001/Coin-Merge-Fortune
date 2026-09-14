// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: 22; dependency map: {"./cipher-core":4,"./core":5}
module.exports = function(e, t, a) {
this, i = function(e) {
e.pad.Iso97971 = {
pad: function(t, a) {
t.concat(e.lib.WordArray.create([ 2147483648 ], 1));
e.pad.ZeroPadding.pad(t, a);
},
unpad: function(t) {
e.pad.ZeroPadding.unpad(t);
t.sigBytes--;
}
};
return e.pad.Iso97971;
}, "object" == typeof a ? t.exports = a = i(e("./core"), e("./cipher-core")) : "function" == typeof define && define.amd ? define([ "./core", "./cipher-core" ], i) : i(this.CryptoJS);
var i;
};
