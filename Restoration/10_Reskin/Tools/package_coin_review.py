"""Package only the coin withdrawal checkpoint; other page experiments stay local."""
from pathlib import Path
import shutil,json,hashlib
R=Path(__file__).resolve().parents[3]
W=R/'paid_ui_work/reskin_20260916';P=W/'UnityVisualRepair';B=W/'UnityRemainingWorkingCopy'
D=R/'Restoration/10_Reskin/CoinWithdrawPatch';D.mkdir(parents=True,exist_ok=True)
names='Header CoinCard ChoicesPanel ChoiceGreen ChoiceBlue ConditionCard CoinTrack Green Disabled Sky Back Check Coin2000 LoadingFill LabelWithdraw LabelMyCoins LabelSelect LabelConditions LabelWithdrawDisabled Amount500 Amount800 Amount1000 Amount2000 Amount3000 Amount5000'.split()+['Digit'+str(n) for n in range(10)]
paths=['Assets/Resources/VisualRepair/'+n+'.png' for n in names]
paths+=['Assets/Resources/VisualRepair/'+n for n in ['Nunito-Black.ttf','Nunito-ExtraBold.ttf','OFL.txt']]
paths+=['Assets/Scripts/UI/'+n+'.cs' for n in ['ApprovedViewportLayout','ApprovedProgressFill','ApprovedLabelArtwork','ApprovedCounterDigits','ApprovedTextDepth','RecoveredMainMenus']]
paths+=['Assets/Prefabs/Runtime/RecoveredMain.prefab','Assets/Scenes/RecoveredMain.unity','Assets/Editor/CoinWithdrawAuthor.cs','Assets/Editor/CoinWithdrawCapture.cs']
paths+=['Assets/Resources/VisualRepair.meta']
basepaths=list(paths)
for path in basepaths:
 if (P/(path+'.meta')).is_file():paths.append(path+'.meta')
records=[]
for path in paths:
 src=P/path;dest=D/path;dest.parent.mkdir(parents=True,exist_ok=True);shutil.copy2(src,dest)
 records.append(dict(path=path,before=hashlib.sha256((B/path).read_bytes()).hexdigest() if (B/path).exists() else None,after=hashlib.sha256(src.read_bytes()).hexdigest(),bytes=src.stat().st_size))
(D/'manifest.json').write_text(json.dumps(dict(scope='coin withdrawal page only; pending user visual approval',base='UnityRemainingWorkingCopy',files=records),indent=2),encoding='utf-8')
V=R/'Restoration/10_Reskin/Verification'
for src,dest in [(W/'07_Verification/CoinWithdrawReview/checks.json',V/'coin_withdraw_review.json'),(W/'07_Verification/recovered_menus_validation.json',V/'coin_withdraw_menus.json')]:
 if src.exists():shutil.copy2(src,dest)
print('Packaged',len(records),'files')
