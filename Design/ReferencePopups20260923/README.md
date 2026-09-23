# Settings and reward popup reference pass

Applies only to Settings, normal cash reward (kind 3), and double cash reward (kind 2).
The user's three supplied references are the visual targets. Built-in `image_gen` extracted
four transparent panel/control assets; the prompts are recorded in `prompts.json`.
Native Text components render localized headings and live amounts with a gradient,
outline, highlight and short extrusion. Font glyphs are live Unity text, not pixels
copied from the reference illustrations.

## Source geometry and behavior

- `SettingDialog`: background 654 x 562, anchored at (-0.34, 30.539), from the recovered
  Cocos node. Music/vibration, persistence, agreement, privacy and close use existing Buttons.
- `RewardDialog`: both original normal/double groups are 800 x 800 at (-2.135, -2.693).
  Original normal ribbon: width 719, top 381.6; 662 x 535 body ends at -232.24.
  The reference is fitted to that combined 719 x 613.84 visual envelope.
  Original double crown's `DJB_TX` central bone/mesh reaches approximately 454.8;
  its 662 x 535 body ends at -300.576. Its complete reference envelope is 719 x 755.376.
- Double has its own serialized visual group so kind 1 (revive) keeps the existing visuals/title.
  Highest-coin and guide rewards are unchanged. The normal legacy banknote container is
  inactive because currency localization can re-enable its individual Image components.
- Five double-header coins use existing lossless 1254px `Gameplay/Coins/500`, `1000`,
  and `2000` sprites, preserving circular aspect. No reward values are baked into art.
- Runtime reward calculations, mocked ad branches, auto-dismiss timing and credit events
  remain in their original code paths.

## Assets and authoring

- Art: `Restoration/12_UnityReskin_20260922/Assets/Resources/ReferencePopups/`.
- Native assets: `Assets/Prefabs/Runtime/RecoveredMain.prefab` and
  `Assets/Scenes/RecoveredMain.unity` in that project.
- Editor author: `ReferencePopupsAuthor.cs`; rerunning it is safe for the live labels.
- `RecoveredPopupLettering.cs` is a native UGUI mesh effect, rebuilding when labels change.
- Four PNGs are uncompressed, mipmaps off, max size 2048 and bilinear filtering.

`reference-popups.patch` records changes to existing local project files that were not yet
tracked in this repository. It is already applied; do not reapply it. Unrelated scene
reserialization is excluded, and Unity stripped nested-prefab documents are preserved.
Screenshots and disposable review saves remain local. No APK/build or emulator install
was produced.

## Verification

Unity 2022.3.62f3c1 imported the authored assets and ran the original startup route in an
independent copy of the project. The final applied prefab was reloaded without running
the authoring tool. 47 checks passed, covering both settings switches and persistence,
policy links, the orange close Button, native dynamic amounts, lossless coin textures,
normal/double dismissal and single credit, and the untouched revive presentation.
GPU captures at 1080 x 1920 and 1080 x 2340 were inspected. Existing ad integrations
remain mocked; this is Unity Play Mode validation, not device-install validation.
