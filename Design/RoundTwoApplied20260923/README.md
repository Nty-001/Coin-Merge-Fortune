# Approved round-two screens and Rules A — 2026-09-23

Applied to `Restoration/12_UnityReskin_20260922`: Brazil account, phone account, drag / merge / balance / coin tutorials, and the user-selected Merge Rules A design. The native RecoveredMain prefab and scene contain editable UI controls; no fullscreen screenshot replaces interaction.

Art was extracted with built-in image_gen. See prompts.json and rules-assets-prompts.json. The three requested rule designs are saved in ../MergeRulesThreeReview20260923; only A is applied. B/C remain unselected. Image imports use full-rect alpha sprites, RGBA32, no mipmaps or texture compression. Current user-provided Gameplay/Coins sprites are reused.

Rule reveal timing and pose curves come from the recovered HeChengSM_TX animation. Only positions are authored for the selected U-shaped track. NativeRulesCoinImages follows the existing bone/slot channels using high-resolution UI sprites. Text remains localized. Close and OK use the existing menu action callbacks. The balance tutorial uses the existing country banknote binding; the coin tutorial follows the responsive HUD target.

Unity 2022.3.62f3c1 imported and rendered all seven in an isolated copy. Verification/rewarded.json reports no errors. Checks cover empty/filled forms, BR/ID/TH platform choices, Japanese tutorial cash, original guide-button progression, all 11 revealed rule coins and closing the rule popup. Screens include 1080x1920 and 1080x2340. The validation respects the original withdrawal cooldown and uses a disposable save namespace. Commercial SDK facades stay mocked; no real payout is performed.

Verification/RoundTwoApprovedReview.cs can reproduce the checks when copied to an isolated project's Assets/Editor with RoundTwoApprovedAuthor. Execute CoinMerge.Recovery.Editor.RoundTwoApprovedReview.Run. It exits Unity after finishing; do not run it in the live editor.
