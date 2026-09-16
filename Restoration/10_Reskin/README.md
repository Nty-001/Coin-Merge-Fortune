# Five-page Unity reskin — 2026-09-16

The main game, draw machine, withdrawal page, settings and merge-rules page now use the supplied blue-sky / glossy UI art direction. All 11 denominations (1, 2, 5, 10, 20, 50, 100, 200, 500, 1000, 2000) use the coin family from reference 5. Assets were AI-authored from the supplied references, then cut into independent sprites and existing skeletal atlas regions.

The ready-to-open local project is `paid_ui_work/reskin_20260916/UnityWorkingCopy`. Open `Assets/Scenes/RecoveredMain.unity` using Unity **2022.3.62f3c1**. The local side-by-side review is `paid_ui_work/reskin_20260916/review_final.html`; full-resolution actual PlayMode captures are in `paid_ui_work/reskin_20260916/07_Verification/ReskinRuntime`.

## Verified behavior

Actual Unity PlayMode passed **130 menu checks, 31 native gameplay checks and 19 reskin integration checks**. The last group exercises coin sprite loading, settings and music state, the animated merge order, six withdrawal products, and the original delayed draw / animation / result flow. Reports are in `Verification/`.

The existing native world-space coin prefabs, colliders, coin canvas sizes, coin GUIDs, gameplay configuration, runtime C# and animation timing were retained. UI is implemented through existing prefab/scene objects and sprite bindings, not full-page screenshots. The original project's **4,166 baseline files are hash-identical**. Device builds were not tested; commercial services remain mocked.

## Cutout quality

Coin silhouettes use supersampled antialiased masks. UI source backgrounds are removed with an inset matte, transparent-edge RGB dilation and premultiplied-alpha resizing. Unity UI import uses transparent alpha, clamp, bilinear filtering, no mipmaps and uncompressed textures. The final sprites were checked over dark backgrounds and at actual runtime scale, including enlarged coin, panel and machine edges. This avoids the checkerboard/colored fringe and fragmented outline found in rejected intermediate assets.

Existing layout and dynamic text remain; this is a working visual reskin, not a pixel-identical reconstruction of the reference screenshots. Original font weights, text content, debug entry and rule-arrow layout are retained. Runtime screenshots use disposable test state; balances shown are test values.

## Versioned patch and reproduction

`Patch/` contains the **96 changed/new runtime asset files**, including new sprite metadata and authored prefab/scene references. `manifest.json` records before/after SHA-256 hashes. Large generated Unity caches and local runtime screenshots are excluded from commits.

To reproduce on another independent baseline copy, close Unity and run:

```powershell
./Restoration/10_Reskin/Tools/Apply-Reskin.ps1 -Destination 'D:/YourIndependentUnityCopy'
```

The script validates all baseline/patch hashes before writing and rejects the original repository project as a destination. Rollback is to restore modified files from the original baseline and remove files whose manifest `before` is null; the original project is already preserved locally.

`ArtSources/` holds the accepted AI-authored sources. `Tools/prepare_art.py` recreates cutouts for the fixed local independent-copy path (Pillow + NumPy). Editor authoring/capture tools are provided separately in `Tools/`; they are not required to run the finished game. Copy the three `Reskin*.cs` tools into an independent project's `Assets/Editor` only when reauthoring or capturing. Never rerun the baseline YAML generator over this authored result.
