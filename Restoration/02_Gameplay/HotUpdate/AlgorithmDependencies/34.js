// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: 34; dependency map: {"./core":5,"./sha512":35,"./x64-core":37}
module.exports = function(e, t, a) {
this, i = function(e) {
(function() {
var t = e, a = t.x64, i = a.Word, o = a.WordArray, n = t.algo, s = n.SHA512, r = n.SHA384 = s.extend({
_doReset: function() {
this._hash = new o.init([ new i.init(3418070365, 3238371032), new i.init(1654270250, 914150663), new i.init(2438529370, 812702999), new i.init(355462360, 4144912697), new i.init(1731405415, 4290775857), new i.init(2394180231, 1750603025), new i.init(3675008525, 1694076839), new i.init(1203062813, 3204075428) ]);
},
_doFinalize: function() {
var e = s._doFinalize.call(this);
e.sigBytes -= 16;
return e;
}
});
t.SHA384 = s._createHelper(r);
t.HmacSHA384 = s._createHmacHelper(r);
})();
return e.SHA384;
}, "object" == typeof a ? t.exports = a = i(e("./core"), e("./x64-core"), e("./sha512")) : "function" == typeof define && define.amd ? define([ "./core", "./x64-core", "./sha512" ], i) : i(this.CryptoJS);
var i;
};
