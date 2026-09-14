"""Extract a real native function body from the full assembly by symbol substring or hex address."""
import csv,sys,re
from pathlib import Path
root=Path(__file__).resolve().parents[1]/'02_Gameplay/NativeCocosAssembly'
if len(sys.argv)!=2:raise SystemExit('Usage: python show_native_function.py SYMBOL_SUBSTRING_OR_0xADDRESS')
query=sys.argv[1]
with (root/'functions.csv').open(encoding='utf-8-sig',newline='') as f:
    matches=[row for row in csv.DictReader(f) if query in row['names'] or query.lower()==row['address'].lower()]
if len(matches)!=1:
    for row in matches[:40]:print(row['address'],row['size'],row['names'])
    raise SystemExit(f'{len(matches)} matches. Specify a unique address or symbol.')
entry=matches[0]
if not entry['assemblyFile']:raise SystemExit('Symbol has no executable body in a dumped section; check ELF classification.')
start=int(entry['address'],16);end=start+int(entry['size'])
if end<=start:raise SystemExit('Symbol has zero declared size; use full section and neighboring symbols, do not invent a function boundary.')
with (root/entry['assemblyFile']).open('rb') as f:
    f.seek(int(entry['fileByteOffset']))
    for raw in f:
        line=raw.decode('utf-8');m=re.match(r'^([0-9a-f]{16}):',line)
        if m and int(m[1],16)>=end:break
        print(line,end='')
