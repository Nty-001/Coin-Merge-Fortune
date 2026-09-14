// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: 18; dependency map: {"./cipher-core":4,"./core":5}
module.exports = function(e, t, a) {
this, i = function(e) {
e.mode.ECB = function() {
var t = e.lib.BlockCipherMode.extend();
t.Encryptor = t.extend({
processBlock: function(e, t) {
this._cipher.encryptBlock(e, t);
}
});
t.Decryptor = t.extend({
processBlock: function(e, t) {
this._cipher.decryptBlock(e, t);
}
});
return t;
}();
return e.mode.ECB;
}, "object" == typeof a ? t.exports = a = i(e("./core"), e("./cipher-core")) : "function" == typeof define && define.amd ? define([ "./core", "./cipher-core" ], i) : i(this.CryptoJS);
var i;
};
