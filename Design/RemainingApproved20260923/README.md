# Seven approved remaining screens — 2026-09-23

Applied only privacy, terms, withdrawal processing, next-stage conditions, email account, daily limit and newbie reward to `Restoration/12_UnityReskin_20260922`.

The current Unity editor imported all assets and returned `APPROVED_SEVEN_AUTHORED`. Both RecoveredMain scene and prefab are saved. Native UI controls remain editable. Protocol strings and withdrawal conditions remain supplied by the recovered flow. Country-specific cash and animated reward rays remain separate layers.

Validation: Unity 2022.3.62f3c1 imported and rendered the seven screens in an isolated copy. `Verification/rewarded.json` has no errors. The flow checks exercise email empty/filled visual states, submit through the original verification callbacks into the next-stage page, protocol switching/scrolling, Japanese cash sprites, moving reward glow and a 1080×2340 layout. The standard captures are 1080×1920. Commercial SDK facades remain mocked as required; this is not a real payout test.

`Verification/ApprovedSevenReview.cs` reproduces the checks when copied to an isolated project's Assets/Editor, alongside `RemainingApprovedAuthor`. Run `CoinMerge.Recovery.Editor.ApprovedSevenReview.Run` with the Unity `-executeMethod` argument. It uses a disposable save namespace and exits Unity after completion; do not run it in the user's live editor.

Art was extracted and cleaned with built-in image_gen from the approved designs. Unity imports use full-rect alpha sprites, RGBA32, no mipmaps or texture compression. Text, status badges, input fields and buttons are separate native objects. See prompts.json for provenance.

The other seven designs are revised under `../RemainingSkinReview20260923/Round2` and are still pending approval. They have not been installed. The ordinary level-selection version remains excluded.
