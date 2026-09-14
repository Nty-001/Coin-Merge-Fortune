// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: 13; dependency map: {"./core":5}
module.exports = function(e, t, a) {
this, i = function(e) {
(function() {
if ("function" == typeof ArrayBuffer) {
var t = e.lib.WordArray, a = t.init;
(t.init = function(e) {
e instanceof ArrayBuffer && (e = new Uint8Array(e));
(e instanceof Int8Array || "undefined" != typeof Uint8ClampedArray && e instanceof Uint8ClampedArray || e instanceof Int16Array || e instanceof Uint16Array || e instanceof Int32Array || e instanceof Uint32Array || e instanceof Float32Array || e instanceof Float64Array) && (e = new Uint8Array(e.buffer, e.byteOffset, e.byteLength));
if (e instanceof Uint8Array) {
for (var t = e.byteLength, i = [], o = 0; o < t; o++) i[o >>> 2] |= e[o] << 24 - o % 4 * 8;
a.call(this, i, t);
} else a.apply(this, arguments);
}).prototype = t;
}
})();
return e.lib.WordArray;
}, "object" == typeof a ? t.exports = a = i(e("./core")) : "function" == typeof define && define.amd ? define([ "./core" ], i) : i(this.CryptoJS);
var i;
};
