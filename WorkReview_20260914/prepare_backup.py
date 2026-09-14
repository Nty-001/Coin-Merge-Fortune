"""Prepare non-destructive native assembly backup and check project hashes.

No Git invocation or network request is performed.
"""
import gzip,hashlib,json,shutil
from pathlib import Path

ROOT=Path(__file__).resolve().parents[1]
OUT=Path(__file__).resolve().parent
PROJECT=ROOT/'Restoration/06_UnityFramework'
NATIVE=ROOT/'Restoration/02_Gameplay/NativeCocosAssembly'
original=NATIVE/'text.asm'
archive=NATIVE/'text.asm.gz'
if not archive.exists():
    with original.open('rb') as inp,archive.open('wb') as raw:
        with gzip.GzipFile(filename='text.asm',fileobj=raw,mode='wb',compresslevel=6,mtime=0) as zipped:
            shutil.copyfileobj(inp,zipped)

def digest(stream):
    sha=hashlib.sha256()
    while data:=stream.read(1024*1024):sha.update(data)
    return sha.hexdigest()

with original.open('rb') as f:original_sha=digest(f)
with gzip.open(archive,'rb') as f:restored_sha=digest(f)
assert original_sha==restored_sha
baseline=json.loads((OUT/'original_project_manifest.json').read_text(encoding='utf-8'))
changed=[]
for entry in baseline:
    path=PROJECT/entry['path']
    if not path.exists():changed.append(entry['path']);continue
    with path.open('rb') as f:
        if digest(f)!=entry['sha256']:changed.append(entry['path'])
report=dict(archive=archive.relative_to(ROOT).as_posix(),originalBytes=original.stat().st_size,
    compressedBytes=archive.stat().st_size,originalSha256=original_sha,decompressedSha256=restored_sha,
    originalProjectFilesChecked=len(baseline),originalProjectFilesChanged=changed,
    gitCommandsRun=False,gitCommitCreated=False,gitPushed=False)
(OUT/'backup_preparation.json').write_text(json.dumps(report,indent=2),encoding='utf-8')
print(json.dumps(report,indent=2))
