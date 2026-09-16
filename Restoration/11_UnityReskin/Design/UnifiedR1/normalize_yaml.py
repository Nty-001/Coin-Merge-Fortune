"""Preserve baseline formatting for unchanged Unity documents; trim changed documents only."""
import re,subprocess
from pathlib import Path
root=Path(__file__).resolve().parents[4]
git=Path('C:/Users/001/.cache/codex-runtimes/codex-primary-runtime/dependencies/native/git/cmd/git.exe')
files=['Assets/Prefabs/Runtime/RecoveredMain.prefab','Assets/Prefabs/Runtime/RecoveredPackaged.prefab','Assets/Prefabs/Runtime/VersionGM.prefab','Assets/Scenes/RecoveredMain.unity','Assets/Scenes/RecoveredPackaged.unity','Assets/Scenes/RecoveredLoading.unity']
def clean(text):return '\n'.join(line.rstrip() for line in text.splitlines())+'\n'
def documents(text):return re.split(r'(?=^--- !u!)',text,flags=re.M)
for rel in files:
    rel='Restoration/11_UnityReskin/'+rel
    baseline=subprocess.check_output([str(git),'show','HEAD:'+rel],cwd=root).decode('utf8').replace('\r\n','\n')
    path=root/rel;current=path.read_text(encoding='utf8')
    old={part.splitlines()[0]:part for part in documents(baseline) if part}
    result=[]
    for part in documents(current):
        if not part:continue
        previous=old.get(part.splitlines()[0]);result.append(previous if previous is not None and clean(previous)==clean(part) else clean(part))
    path.write_bytes(''.join(result).encode('utf8'))
print('Normalized six authored Unity files; semantic contents unchanged.')
