// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: 17; dependency map: {"./cipher-core":4,"./core":5}
module.exports = function(e, t, a) {
this, i = function(e) {
e.mode.CTR = function() {
var t = e.lib.BlockCipherMode.extend(), a = t.Encryptor = t.extend({
processBlock: function(e, t) {
var a = this._cipher, i = a.blockSize, o = this._iv, n = this._counter;
if (o) {
n = this._counter = o.slice(0);
this._iv = void 0;
}
var s = n.slice(0);
a.encryptBlock(s, 0);
n[i - 1] = n[i - 1] + 1 | 0;
for (var r = 0; r < i; r++) e[t + r] ^= s[r];
}
});
t.Decryptor = a;
return t;
}();
return e.mode.CTR;
}, "object" == typeof a ? t.exports = a = i(e("./core"), e("./cipher-core")) : "function" == typeof define && define.amd ? define([ "./core", "./cipher-core" ], i) : i(this.CryptoJS);
var i;
};
