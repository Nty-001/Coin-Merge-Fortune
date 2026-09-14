// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: 19; dependency map: {"./cipher-core":4,"./core":5}
module.exports = function(e, t, a) {
this, i = function(e) {
e.mode.OFB = function() {
var t = e.lib.BlockCipherMode.extend(), a = t.Encryptor = t.extend({
processBlock: function(e, t) {
var a = this._cipher, i = a.blockSize, o = this._iv, n = this._keystream;
if (o) {
n = this._keystream = o.slice(0);
this._iv = void 0;
}
a.encryptBlock(n, 0);
for (var s = 0; s < i; s++) e[t + s] ^= n[s];
}
});
t.Decryptor = a;
return t;
}();
return e.mode.OFB;
}, "object" == typeof a ? t.exports = a = i(e("./core"), e("./cipher-core")) : "function" == typeof define && define.amd ? define([ "./core", "./cipher-core" ], i) : i(this.CryptoJS);
var i;
};
