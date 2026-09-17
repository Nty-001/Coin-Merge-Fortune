import json
from html.parser import HTMLParser
from pathlib import Path
from urllib.parse import unquote

ROOT = Path(__file__).resolve().parent
class AuditParser(HTMLParser):
    def __init__(self):
        super().__init__()
        self.refs=set()
        self.ids=[]
        self.external=[]
    def handle_starttag(self, tag, attrs):
        a=dict(attrs)
        if 'id' in a:
            self.ids.append(a['id'])
        for key in ('src','href'):
            v=a.get(key,'')
            if not v: continue
            if '://' in v: self.external.append(v)
            elif not v.startswith('#'): self.refs.add(unquote(v))

p=AuditParser()
p.feed((ROOT/'index.html').read_text(encoding='utf-8'))
assert len(p.ids)==len(set(p.ids)), 'duplicate HTML id'
assert not p.external, p.external
missing=[x for x in p.refs if not (ROOT/x).is_file()]
assert not missing, missing
data=json.loads((ROOT/'audit_summary.json').read_text(encoding='utf-8'))
assert len(data['issues'])==14
assert sum(i['priority']=='P1' for i in data['issues'])==5
assert data['current_flow_passed'] and data['effects_passed']
versioned=set(x for x in p.refs if not x.startswith('Original/'))
# Include inventories associated with selected current captures, not every raw traversal image.
for file in list(versioned):
    if file.startswith('Current/') and file.endswith('.png'):
        candidate=file[:-4]+'.json'
        if (ROOT/candidate).exists(): versioned.add(candidate)
versioned.update(['index.html','audit_summary.json','README.md','build_report.py','validate_report.py','.gitignore','multi_locale_stress.json'])
(ROOT/'versioned_evidence.json').write_text(json.dumps(sorted(versioned),ensure_ascii=False,indent=2),encoding='utf-8')
result={'passed':True,'local_links':len(p.refs),'issues':14,'external_requests':0,
    'versioned_files':len(versioned),'versioned_evidence_mb':round(sum((ROOT/x).stat().st_size for x in versioned)/1048576,2)}
(ROOT/'report_validation.json').write_text(json.dumps(result,indent=2),encoding='utf-8')
print(json.dumps(result))
