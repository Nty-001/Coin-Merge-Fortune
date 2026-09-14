// Recovered complete compiled JavaScript module function.
// Original bundle: export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js
// Module: LinkScript; dependency map: {"../UIConfig":"UIConfig"}
module.exports = function(e, t, a) {
"use strict";
cc._RF.push(t, "afef1tYdPhMjaODgRIJTJ1m", "LinkScript");
Object.defineProperty(a, "__esModule", {
value: !0
});
a.EmitterCustomEvent = void 0;
const i = e("../UIConfig");
class o {
static register_ui_path(e, t) {
i.default[e] ? console.error(`prefab${e} 的路径已经存在 ${i.default[e]}`) : i.default[e] = t;
}
}
o.game_play_record = [];
o.game_play_event_config = {
ui_view_active_state_change: "ui_view_active_state_change"
};
(function(e) {
e.Refresh_TopData = "Refresh_TopData";
e.HomeRefresh_LevelData = "HomeRefresh_LevelData";
e.Refresh_GameBG = "Refresh_GameBG";
e.Reset_Other_Items = "Reset_Other_Items";
})(a.EmitterCustomEvent || (a.EmitterCustomEvent = {}));
a.default = o;
cc._RF.pop();
};
