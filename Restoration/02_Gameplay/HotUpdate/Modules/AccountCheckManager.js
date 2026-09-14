// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: AccountCheckManager; dependency map: {}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "9d70eHlTp9MaK819yq7qs25", "AccountCheckManager");
Object.defineProperty(a, "__esModule", {
value: !0
});
a.default = class extends cc.Component {
static validateEmail(e) {
if (!e || "" === e.trim()) return {
valid: !1,
error: "邮箱不能为空"
};
if (!/^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/.test(e)) return {
valid: !1,
error: "邮箱格式不正确（应为：name@domain.com）"
};
if (e.length > 254) return {
valid: !1,
error: "邮箱长度不能超过254个字符"
};
const t = e.split("@")[0];
return t.length > 64 ? {
valid: !1,
error: "@前面的部分不能超过64个字符"
} : e.includes("..") ? {
valid: !1,
error: "邮箱不能包含连续的点号"
} : t.startsWith(".") || t.endsWith(".") ? {
valid: !1,
error: "@前面的部分不能以点号开头或结尾"
} : {
valid: !0,
email: e.toLowerCase().trim()
};
}
static validatePhone08(e) {
return /^08[0-9\s-]{9,14}$/.test(e) ? {
valid: !0
} : {
valid: !1
};
}
static validatePhone10(e) {
return /^0[0-9]{9}$/.test(e) ? {
valid: !0
} : {
valid: !1
};
}
static validatePhone10Or11(e) {
return /^[0-9]{10,11}$/.test(e) ? {
valid: !0
} : {
valid: !1
};
}
static validatePhone84(e) {
return /^84[0-9]{9}$/.test(e) ? {
valid: !0
} : {
valid: !1
};
}
static validatePhone09(e) {
return /^09[0-9]{9}$/.test(e) ? {
valid: !0
} : {
valid: !1
};
}
static validateAccount(e) {
return /^[a-zA-Z0-9\s-,.]{3,100}$/.test(e) ? {
valid: !0
} : {
valid: !1,
message: "success"
};
}
static validatePhone55(e) {
return /^\+55\d{7,12}$/.test(e) ? {
valid: !0
} : {
valid: !1,
message: "success"
};
}
static validateCPFJ(e) {
return /^\d{11}$/.test(e) ? {
valid: !0
} : {
valid: !1,
message: "success"
};
}
static validatePix(e) {
return /^[a-zA-Z0-9-]{36}$/.test(e) ? {
valid: !0
} : {
valid: !1,
message: "success"
};
}
};
cc._RF.pop();
};
