# Coin withdrawal — single-page review checkpoint

The user's latest scope is to review this page before any other page continues. This checkpoint is **pending visual approval**, not a claim of completed pixel-identical restoration.

Independent project: `paid_ui_work/reskin_20260916/UnityVisualRepair`, Unity 2022.3.62f3c1. Original and currently open delivered projects were not overwritten. The main prefab and scene were restored from the last delivered copy before applying only this page. Other page experiments remain local and are not part of this patch.

The coin page now uses measured 941 × 1672 reference rectangles, a responsive authored layout surface, native interactive amount choices, and separate static panel artwork. English display lettering follows the live label value and falls back to native localized text when it differs. The displayed counter is driven by the real player value through ten pre-authored digit Images. No screenshot replaces the interactive page. The existing withdrawal conditions, amount selection and code-bound Buttons are retained.

The ratio shown in the first withdrawal step now reads current merges / required merges. A serialized progress presentation component sizes the rounded fill without stretching its end caps. The debug GM overlay is hidden in this review copy.

Local screenshots under `07_Verification/CoinWithdrawReview` include a matching zero-count state at 941 × 1672 and the user's 503-count state at both 941 × 1672 and 407 × 881. These are Windows Unity PlayMode renders, not Android hardware captures. Both resolution checks use the same runtime components.

Verification: 29 focused interaction/state assertions and the existing 130-assertion menu/withdrawal regression suite. Reports accompany the checkpoint. The broader approved styles are intentionally awaiting this single-page review.

`CoinWithdrawPatch` is an incremental patch over the previous cumulative reskin delivery. Its manifest records each previous and new file hash. Do not apply it to the protected original recovery project.

Art provenance: approved reference panel art was cleaned using the built-in image generation tool; typography labels were technically extracted from the approved reference. Dynamic counter digits were generated with the built-in tool using the reference's large white/blue numeral as style input, then cut, matted, and checked against a contrasting background. Font files use the included OFL license. No external image API or model fallback was used.

Digit prompt: transparent atlas of 0–9, one numeral per cell, matching the reference's rounded white face, icy blue bevel, royal blue outline and darker blue extrusion; equal baseline and size; no other objects or text. The generated source contained a rendered checkerboard, so the final sprites remove it and isolated components, smooth the alpha silhouette, and dilate edge RGB under transparency. The source atlas is not used as a runtime texture.
