// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: 4; dependency map: {"./core":5,"./evpkdf":9}
module.exports = function(e, t, a) {
this, i = function(e) {
e.lib.Cipher || function(t) {
var a = e, i = a.lib, o = i.Base, n = i.WordArray, s = i.BufferedBlockAlgorithm, r = a.enc, l = (r.Utf8, 
r.Base64), c = a.algo.EvpKDF, d = i.Cipher = s.extend({
cfg: o.extend(),
createEncryptor: function(e, t) {
return this.create(this._ENC_XFORM_MODE, e, t);
},
createDecryptor: function(e, t) {
return this.create(this._DEC_XFORM_MODE, e, t);
},
init: function(e, t, a) {
this.cfg = this.cfg.extend(a);
this._xformMode = e;
this._key = t;
this.reset();
},
reset: function() {
s.reset.call(this);
this._doReset();
},
process: function(e) {
this._append(e);
return this._process();
},
finalize: function(e) {
e && this._append(e);
return this._doFinalize();
},
keySize: 4,
ivSize: 4,
_ENC_XFORM_MODE: 1,
_DEC_XFORM_MODE: 2,
_createHelper: function() {
function e(e) {
return "string" == typeof e ? v : y;
}
return function(t) {
return {
encrypt: function(a, i, o) {
return e(i).encrypt(t, a, i, o);
},
decrypt: function(a, i, o) {
return e(i).decrypt(t, a, i, o);
}
};
};
}()
}), h = (i.StreamCipher = d.extend({
_doFinalize: function() {
return this._process(!0);
},
blockSize: 1
}), a.mode = {}), u = i.BlockCipherMode = o.extend({
createEncryptor: function(e, t) {
return this.Encryptor.create(e, t);
},
createDecryptor: function(e, t) {
return this.Decryptor.create(e, t);
},
init: function(e, t) {
this._cipher = e;
this._iv = t;
}
}), g = h.CBC = function() {
var e = u.extend();
e.Encryptor = e.extend({
processBlock: function(e, t) {
var i = this._cipher, o = i.blockSize;
a.call(this, e, t, o);
i.encryptBlock(e, t);
this._prevBlock = e.slice(t, t + o);
}
});
e.Decryptor = e.extend({
processBlock: function(e, t) {
var i = this._cipher, o = i.blockSize, n = e.slice(t, t + o);
i.decryptBlock(e, t);
a.call(this, e, t, o);
this._prevBlock = n;
}
});
function a(e, a, i) {
var o, n = this._iv;
if (n) {
o = n;
this._iv = t;
} else o = this._prevBlock;
for (var s = 0; s < i; s++) e[a + s] ^= o[s];
}
return e;
}(), p = (a.pad = {}).Pkcs7 = {
pad: function(e, t) {
for (var a = 4 * t, i = a - e.sigBytes % a, o = i << 24 | i << 16 | i << 8 | i, s = [], r = 0; r < i; r += 4) s.push(o);
var l = n.create(s, i);
e.concat(l);
},
unpad: function(e) {
var t = 255 & e.words[e.sigBytes - 1 >>> 2];
e.sigBytes -= t;
}
}, f = (i.BlockCipher = d.extend({
cfg: d.cfg.extend({
mode: g,
padding: p
}),
reset: function() {
var e;
d.reset.call(this);
var t = this.cfg, a = t.iv, i = t.mode;
if (this._xformMode == this._ENC_XFORM_MODE) e = i.createEncryptor; else {
e = i.createDecryptor;
this._minBufferSize = 1;
}
if (this._mode && this._mode.__creator == e) this._mode.init(this, a && a.words); else {
this._mode = e.call(i, this, a && a.words);
this._mode.__creator = e;
}
},
_doProcessBlock: function(e, t) {
this._mode.processBlock(e, t);
},
_doFinalize: function() {
var e, t = this.cfg.padding;
if (this._xformMode == this._ENC_XFORM_MODE) {
t.pad(this._data, this.blockSize);
e = this._process(!0);
} else {
e = this._process(!0);
t.unpad(e);
}
return e;
},
blockSize: 4
}), i.CipherParams = o.extend({
init: function(e) {
this.mixIn(e);
},
toString: function(e) {
return (e || this.formatter).stringify(this);
}
})), m = (a.format = {}).OpenSSL = {
stringify: function(e) {
var t = e.ciphertext, a = e.salt;
return (a ? n.create([ 1398893684, 1701076831 ]).concat(a).concat(t) : t).toString(l);
},
parse: function(e) {
var t, a = l.parse(e), i = a.words;
if (1398893684 == i[0] && 1701076831 == i[1]) {
t = n.create(i.slice(2, 4));
i.splice(0, 4);
a.sigBytes -= 16;
}
return f.create({
ciphertext: a,
salt: t
});
}
}, y = i.SerializableCipher = o.extend({
cfg: o.extend({
format: m
}),
encrypt: function(e, t, a, i) {
i = this.cfg.extend(i);
var o = e.createEncryptor(a, i), n = o.finalize(t), s = o.cfg;
return f.create({
ciphertext: n,
key: a,
iv: s.iv,
algorithm: e,
mode: s.mode,
padding: s.padding,
blockSize: e.blockSize,
formatter: i.format
});
},
decrypt: function(e, t, a, i) {
i = this.cfg.extend(i);
t = this._parse(t, i.format);
return e.createDecryptor(a, i).finalize(t.ciphertext);
},
_parse: function(e, t) {
return "string" == typeof e ? t.parse(e, this) : e;
}
}), _ = (a.kdf = {}).OpenSSL = {
execute: function(e, t, a, i, o) {
i || (i = n.random(8));
if (o) s = c.create({
keySize: t + a,
hasher: o
}).compute(e, i); else var s = c.create({
keySize: t + a
}).compute(e, i);
var r = n.create(s.words.slice(t), 4 * a);
s.sigBytes = 4 * t;
return f.create({
key: s,
iv: r,
salt: i
});
}
}, v = i.PasswordBasedCipher = y.extend({
cfg: y.cfg.extend({
kdf: _
}),
encrypt: function(e, t, a, i) {
var o = (i = this.cfg.extend(i)).kdf.execute(a, e.keySize, e.ivSize, i.salt, i.hasher);
i.iv = o.iv;
var n = y.encrypt.call(this, e, t, o.key, i);
n.mixIn(o);
return n;
},
decrypt: function(e, t, a, i) {
i = this.cfg.extend(i);
t = this._parse(t, i.format);
var o = i.kdf.execute(a, e.keySize, e.ivSize, t.salt, i.hasher);
i.iv = o.iv;
return y.decrypt.call(this, e, t, o.key, i);
}
});
}();
}, "object" == typeof a ? t.exports = a = i(e("./core"), e("./evpkdf")) : "function" == typeof define && define.amd ? define([ "./core", "./evpkdf" ], i) : i(this.CryptoJS);
var i;
};
