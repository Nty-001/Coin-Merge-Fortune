// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: 15; dependency map: {"./cipher-core":4,"./core":5}
module.exports = function(e, t, a) {
this, i = function(e) {
e.mode.CFB = function() {
var t = e.lib.BlockCipherMode.extend();
t.Encryptor = t.extend({
processBlock: function(e, t) {
var i = this._cipher, o = i.blockSize;
a.call(this, e, t, o, i);
this._prevBlock = e.slice(t, t + o);
}
});
t.Decryptor = t.extend({
processBlock: function(e, t) {
var i = this._cipher, o = i.blockSize, n = e.slice(t, t + o);
a.call(this, e, t, o, i);
this._prevBlock = n;
}
});
function a(e, t, a, i) {
var o, n = this._iv;
if (n) {
o = n.slice(0);
this._iv = void 0;
} else o = this._prevBlock;
i.encryptBlock(o, 0);
for (var s = 0; s < a; s++) e[t + s] ^= o[s];
}
return t;
}();
return e.mode.CFB;
}, "object" == typeof a ? t.exports = a = i(e("./core"), e("./cipher-core")) : "function" == typeof define && define.amd ? define([ "./core", "./cipher-core" ], i) : i(this.CryptoJS);
var i;
};
