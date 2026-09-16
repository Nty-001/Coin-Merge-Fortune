"""Ensure the cash reskin is confined to presentation and the copy."""
import hashlib,json,re,subprocess
from pathlib import Path
PROJECT=Path(__file__).resolve().parents[2]
REPO=PROJECT.parents[1]
GIT=Path('C:/Users/001/.cache/codex-runtimes/codex-primary-runtime/dependencies/native/git/cmd/git.exe')
BASE='1116403'
def git(*args):return subprocess.check_output([str(GIT),*args],cwd=REPO).decode('utf-8')
def norm(s):return '\n'.join(x.rstrip() for x in s.splitlines())
def docs(s):return {i:(t,norm(b)) for t,i,b in re.findall(r'--- !u!(\d+) &(\d+)\n(.*?)(?=--- !u!|\Z)',s,re.S)}
def owner(d,i):
 t,b=d[i]
 return i if t=='1' else (re.search(r'  m_GameObject: \{fileID: (\d+)\}',b).group(1) if '  m_GameObject:' in b else None)
def hierarchy(d):
 names={i:re.search(r'  m_Name: (.*)',b).group(1) for i,(t,b) in d.items() if t=='1'}
 transforms={i:owner(d,i) for i,(t,b) in d.items() if t in ('224','4')}
 parents={owner(d,i):transforms.get(re.search(r'  m_Father: \{fileID: (\d+)\}',b).group(1)) for i,(t,b) in d.items() if t in ('224','4')}
 def chain(go):
  parts=[]
  while go and go in names:parts.append(names[go]);go=parents.get(go)
  return '/'.join(reversed(parts))
 return chain
result={'baseline':BASE,'files':{},'originalFiles':0,'originalUnchanged':True,'errors':[]}
for relative in ['Assets/Prefabs/Runtime/RecoveredMain.prefab','Assets/Scenes/RecoveredMain.unity']:
 rel=(PROJECT/relative).relative_to(REPO).as_posix();old=docs(git('show',BASE+':'+rel));new=docs((PROJECT/relative).read_text(encoding='utf-8'))
 paths=hierarchy(new);oldpaths=hierarchy(old);changes=[]
 for ident in sorted(old.keys()|new.keys(),key=int):
  if old.get(ident)==new.get(ident):continue
  d=new if ident in new else old;t,b=d[ident];chain=(paths if ident in new else oldpaths)(owner(d,ident))
  allowed='GameFakeWDDialog' in chain.split('/')
  changes.append({'id':ident,'class':t,'hierarchy':chain,'allowed':allowed})
  if not allowed:result['errors'].append(relative+': '+chain+' #'+ident)
 result['files'][relative]=changes
allowed={'Assets/Scripts/UI/RecoveredCashPageLayout.cs','Assets/Scripts/UI/RecoveredTextVerticalGradient.cs'}
changed=git('diff','--name-only',BASE,'--',str(PROJECT.relative_to(REPO)/'Assets/Scripts')).splitlines()
for rel in changed:
 local=(REPO/rel).relative_to(PROJECT).as_posix()
 if (local[:-5] if local.endswith('.meta') else local) not in allowed:result['errors'].append('Unexpected runtime change '+local)
result['runtimePresentationChanges']=sorted(allowed)
manifest=json.loads((PROJECT/'COPY_MANIFEST.json').read_text(encoding='utf-8'))
for rel,wanted in manifest['sourceFileHashes'].items():
 f=PROJECT.parent/'06_UnityFramework'/rel;result['originalFiles']+=1
 if not f.exists() or hashlib.sha256(f.read_bytes()).hexdigest()!=wanted:result['originalUnchanged']=False;result['errors'].append('Original changed: '+rel)
result['passed']=not result['errors']
Path(__file__).with_name('scope_audit.json').write_bytes((json.dumps(result,indent=2,ensure_ascii=False)+'\n').encode('utf-8'))
print(json.dumps({k:v for k,v in result.items() if k!='files'},indent=2,ensure_ascii=False))
