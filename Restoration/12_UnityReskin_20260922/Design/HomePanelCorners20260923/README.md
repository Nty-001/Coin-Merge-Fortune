# Home panel corner repair — 2026-09-23

Scope: the withdrawal-success notice background and the bottom blue floor panel only.

- Preserve the original remaining-balance bubble rule: hidden at and above the first 500 threshold. No gameplay/session logic changed.
- Notice: a native sliced Image background leaf at 0.80 alpha. The original text/trophy hierarchy, ticker animation and scale remain intact. The old opaque background Image is disabled.
- Floor: protect all four slice borders; compensate for the source artwork's elliptical corners and the recovered Canvas's 32 reference pixels per unit. The visible dimensions remain 750 × 250 with the same position and floor anchor.
- Keep the original PNG files. Changes are serialized in RecoveredMain.prefab and RecoveredMain.unity and the two HomeReskin sprite importer border settings.

Unity 2022.3.62f3c1 Play Mode review used a separate project copy and a disposable save namespace. Screenshots were checked at 1080 × 1920 and 1080 × 2340, plus a below-threshold state at 941 × 1672. Runtime checks confirm sprite loading, nine-slicing, alpha, unchanged text scale and floor bounds, and both sides of the bubble visibility threshold.

Only four existing component records per runtime asset were updated and four records for the notice background leaf were added. All other existing component records were preserved, including progress text settings and withdrawal pages. Local screenshots, original backups and the review result are alongside this file and are not included in the public checkpoint.

Authoring entry point: `CoinMerge.Recovery.Editor.HomePanelCornersAuthor.Run` (Edit Mode).
No player build or emulator installation was performed.

The project copy was untracked at the start of this repair. The scoped repository checkpoint includes the authoring tool and `home-panels.patch` containing the exact serialized asset edits, rather than publishing the unrelated pending project copy. The patch is already applied in the local project; do not apply it twice.
