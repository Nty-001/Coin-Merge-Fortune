# Remaining UI implementation — approved and delivered

The user approved all 14 concept boards. Their visual style is now applied to the existing player-facing menus and lifecycle views in the new independent project:

`paid_ui_work/reskin_20260916/UnityRemainingWorkingCopy`

Open with **Unity 2022.3.62f3c1**. Start from `Assets/Scenes/RecoveredLoading.unity`, or open `Assets/Scenes/RecoveredMain.unity` for the main game. The earlier independent copy was open in another editor, so it was not closed or overwritten with authored scene/prefab changes.

## Scope

- Coin withdrawal, all six amounts, enabled/disabled actions and selected checkmark.
- US, BR and ID account forms, input surfaces and dynamic confirmation button states.
- Withdrawal verification, unmet-condition view and daily-limit alert.
- User Agreement and Privacy Policy panel surfaces; full original document text remains.
- Loading artwork with the real native progress fill and percentage.
- Game-over statistics, revive, rating stars and the existing Restart action.
- Highest, newbie, ordinary, double, resurrected and wheel cash/coin reward views.
- Tutorial panels, directional arrows, hands, toast and shared state graphics.
- Existing skeletal reward title region recolored through replacement art, with original UVs and animation data retained.

Thirty-five reusable remaining-UI PNGs were imported with clean alpha, clamp, bilinear filtering, no mipmaps and uncompressed UI textures. Technical matting removes magenta source background and source grid dividers; edge colors are dilated before alpha resizing. New art was inspected over a dark backing and in actual Unity renders. Original localized payment brand assets remain; US cash illustration canvases/GUIDs and all coin physics assets are preserved.

No runtime C# or gameplay configuration was changed. The authored Restart visual reuses the existing code-bound Button and event; the old unused visual group is not made into a second gameplay path.

## Verification

Actual Unity PlayMode checks passed:

| Suite | Checks |
| --- | ---: |
| Existing menus, input, scroll and withdrawal routes | 130 |
| Native gameplay, physical merges and SDK mock branches | 31 |
| Lifecycle, rating, rewards, failure and short/tall layouts | 43 |
| Loading and recovered visuals | 25 |
| Original five-page reskin integration | 19 |
| Visible Restart control and real EventSystem click | 5 |
| **Total** | **253** |

The added Restart check confirms visible bounds receive a real pointer click, existing code binding is retained, the failure view closes, round state resets and the cash balance remains unchanged. Reports are in `Verification/`.

All **4,166 original baseline files** in `Restoration/06_UnityFramework` remain hash-identical. The cumulative patch contains **171** changed/new runtime asset files. `manifest.json` records their original and final hashes. `Tools/Apply-Reskin.ps1` was verified against the delivered copy; it rejects the original project as a target.

## Local visual review

Open `paid_ui_work/reskin_20260916/remaining_delivery.html` for the approved concepts beside actual Unity screenshots. Images are clickable at original resolution. The gallery includes clean regional forms, both reward types, all tutorial states, disabled/enabled coin withdrawal and dark-background cutout QA. Screenshots and generated editor caches remain outside public commits.

The running UI retains original localized text, typography behavior, layout constraints, balances and animation states; it is not a flattened copy of the concept boards. Commercial SDKs remain mocked. Validation is in Windows Unity PlayMode, not Android/iOS hardware.

Authoring scripts: `prepare_remaining_art.py`, `RemainingReskinAuthor.cs`. Capture/verification aid: `RemainingReskinCapture.cs`. These are Editor-side tools, not runtime construction code. For reproduction, use the cumulative verified patch rather than rerunning baseline YAML generation.
