# Native gameplay checkpoint — 2026-09-15

The authored entry is `06_UnityFramework/Assets/Scenes/RecoveredMain.unity`, backed by a real `RecoveredMain.prefab` and pooled `NativeCoin.prefab`. Runtime creates only configured coin prefab instances. UI layouts come from the recovered source prefabs. No third-party plugin assemblies were introduced.

## Verified in actual Unity Play Mode

`native_gameplay_validation.json` records 19 passing checks, including a merge produced by actual Physics2D contacts rather than a direct Merge call. It also exercises original guide states through standard Button events, a persisted board reload, separate GM profile persistence, failure/restart, and completed/failed revive ad outcomes through the existing facade. Test saves use their own namespace; runtime initialization is identical in Editor and device builds.

The real engine at `export_20260914/unpacked/runtime_adaljkjf/src/cocos2d-jsb.73f44.js` declares PTM_RATIO=32, VELOCITY_ITERATIONS=10, POSITION_ITERATIONS=10, and enabledAccumulator=false. `GameScene.onLoad` sets gravity to (0,-980). The native scene uses the same dimensional scale and advances Unity Physics2D through its script simulation API using frame delta. Cocos and Unity's different Box2D integrations still require trajectory, contact and frame-order comparison. The old packaged PhysicsManager setting of 2/2 is not the hot-update engine default.

Public references: [Cocos PhysicsManager](https://docs.cocos.com/creator/2.4/api/en/classes/PhysicsManager.html), [Cocos unit conversion](https://docs.cocos.com/creator/2.4/manual/en/physics/physics/physics-manager.html). Local recovered source remains the version-specific authority.

## Remaining differences — not a 100% claim

- Wheel, withdrawal, settings and rules buttons are not yet connected in the new scene; complete source bodies remain available for their ports.
- GM profile persistence exists, but the interactive country/version panel and packaged/organic gameplay selection are not implemented. The saved `rewardedVariant` flag does not yet switch scene branches. It does not patch the MuMu application.
- Maximum-coin rewards currently open without the original Spine/flight sequence. Failure tint/animation, revive removal stagger/fade, collision effects, star travel, combo effects and audio are pending.
- Guide cards preserve source structure, but full viewport-dependent adaptation and translations are pending. Buttons bind to actual visible graphics instead of the original invisible global clickArea, following the user's Button requirement.
- Cash reward calculation is ported; first-strong reward rating prompts and merge-500 rating prompts are not connected yet.
- Source disabled Image state and label wrap/shrink settings are now applied to authored runtime copies. Other original custom components, currency icon loading, broadcast notice formatting, all-country money formatting, safe area/widget/scene adaptation and Spine remain pending.
- Board save and physical merge checks pass; save compatibility with encrypted original device storage is not claimed. The Unity save carries matching gameplay field names in a versioned local envelope.
- No network requests, real ads or real withdrawals are performed by the Unity mock facade.

Re-run authoring explicitly via `CoinMerge.Recovery.Editor.NativeGameplayBuilder.Run`. It rewrites the authored runtime prefabs/scene, so commit intended manual changes before invoking it. Run Play Mode verification using `CoinMerge.Recovery.Editor.NativeGameplayValidation.Run` without `-quit`; the validator exits after its asynchronous checks.
