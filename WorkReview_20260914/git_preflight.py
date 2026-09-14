"""Check Git's current non-ignored paths before the first public backup."""
import hashlib,json,subprocess
from pathlib import Path
ROOT=Path(__file__).resolve().parents[1]
OUT=Path(__file__).resolve().parent
GIT='C:/Users/001/.cache/codex-runtimes/codex-primary-runtime/dependencies/native/git/cmd/git.exe'
paths=subprocess.check_output([GIT,'ls-files','--others','--exclude-standard','-z'],cwd=ROOT).decode().split('\0')
paths=[p for p in paths if p]
secret_file=ROOT/'Restoration/03_Configuration/CapturedDevice/coin_clientId.json'
client_id=json.loads(secret_file.read_text(encoding='utf-8-sig'))
assert isinstance(client_id,str) and len(client_id)>20
needles=[client_id.encode('utf-8')]
hits=[];large=[];total=0
for relative in paths:
    p=ROOT/relative
    if not p.is_file():continue
    size=p.stat().st_size;total+=size
    if size>=100*1024*1024:large.append({'path':relative,'bytes':size})
    if size<40*1024*1024:
        data=p.read_bytes()
        if any(n in data for n in needles):hits.append(relative)
report={'candidateFiles':len(paths),'candidateBytes':total,'deviceClientIdentifierFiles':hits,
        'filesOver100MiB':large,'scope':'Known device client identifier plus excluded acquisition directories; not a general proof of absence of all secrets.'}
(OUT/'git_preflight_result.json').write_text(json.dumps(report,indent=2),encoding='utf-8')
print(json.dumps(report,indent=2))
if hits or large:raise SystemExit(2)
