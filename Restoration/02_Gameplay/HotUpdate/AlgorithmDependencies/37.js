// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: 37; dependency map: {"./core":5}
module.exports = function(e, t, a) {
this, i = function(e) {
a = (t = e).lib, i = a.Base, o = a.WordArray, (n = t.x64 = {}).Word = i.extend({
init: function(e, t) {
this.high = e;
this.low = t;
}
}), n.WordArray = i.extend({
init: function(e, t) {
e = this.words = e || [];
this.sigBytes = null != t ? t : 8 * e.length;
},
toX32: function() {
for (var e = this.words, t = e.length, a = [], i = 0; i < t; i++) {
var n = e[i];
a.push(n.high);
a.push(n.low);
}
return o.create(a, this.sigBytes);
},
clone: function() {
for (var e = i.clone.call(this), t = e.words = this.words.slice(0), a = t.length, o = 0; o < a; o++) t[o] = t[o].clone();
return e;
}
});
var t, a, i, o, n;
return e;
}, "object" == typeof a ? t.exports = a = i(e("./core")) : "function" == typeof define && define.amd ? define([ "./core" ], i) : i(this.CryptoJS);
var i;
};
