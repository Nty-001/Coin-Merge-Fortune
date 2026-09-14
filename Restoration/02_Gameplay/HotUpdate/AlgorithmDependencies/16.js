// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: 16; dependency map: {"./cipher-core":4,"./core":5}
module.exports = function(e, t, a) {
this, i = function(e) {
e.mode.CTRGladman = function() {
var t = e.lib.BlockCipherMode.extend();
function a(e) {
if (255 == (e >> 24 & 255)) {
var t = e >> 16 & 255, a = e >> 8 & 255, i = 255 & e;
if (255 === t) {
t = 0;
if (255 === a) {
a = 0;
255 === i ? i = 0 : ++i;
} else ++a;
} else ++t;
e = 0;
e += t << 16;
e += a << 8;
e += i;
} else e += 1 << 24;
return e;
}
function i(e) {
0 === (e[0] = a(e[0])) && (e[1] = a(e[1]));
return e;
}
var o = t.Encryptor = t.extend({
processBlock: function(e, t) {
var a = this._cipher, o = a.blockSize, n = this._iv, s = this._counter;
if (n) {
s = this._counter = n.slice(0);
this._iv = void 0;
}
i(s);
var r = s.slice(0);
a.encryptBlock(r, 0);
for (var l = 0; l < o; l++) e[t + l] ^= r[l];
}
});
t.Decryptor = o;
return t;
}();
return e.mode.CTRGladman;
}, "object" == typeof a ? t.exports = a = i(e("./core"), e("./cipher-core")) : "function" == typeof define && define.amd ? define([ "./core", "./cipher-core" ], i) : i(this.CryptoJS);
var i;
};
