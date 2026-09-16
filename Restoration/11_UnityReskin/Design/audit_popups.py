"""Audit only the authored popup scope against the last delivered home-art checkpoint."""
import hashlib,json,re,subprocess
from pathlib import Path
PROJECT=Path(__file__).resolve().parents[1]
REPO=PROJECT.parents[1]
GIT=Path('C:/Users/001/.cache/codex-runtimes/codex-primary-runtime/dependencies/native/git/cmd/git.exe')
BASE='6755bd1'
def git(*args):return subprocess.check_output([str(GIT),*args],cwd=REPO).decode('utf-8')
def norm(s):return '\n'.join(x.rstrip() for x in s.splitlines())
def docs(s):return {i:(t,norm(b)) for t,i,b in re.findall(r'--- !u!(\d+) &(\d+)\n(.*?)(?=--- !u!|\Z)',s,re.S)}
def owner(d,i):
 t,b=d[i]
 return i if t=='1' else (re.search(r'  m_GameObject: \{fileID: (\d+)\}',b).group(1) if '  m_GameObject:' in b else None)
def structures(d):
 names={i:re.search(r'  m_Name: (.*)',b).group(1) for i,(t,b) in d.items() if t=='1'}
 transforms={i:owner(d,i) for i,(t,b) in d.items() if t in ('224','4')}
 parents={owner(d,i):transforms.get(re.search(r'  m_Father: \{fileID: (\d+)\}',b).group(1)) for i,(t,b) in d.items() if t in ('224','4')}
 def hierarchy(go):
  chain=[]
  while go and go in names:chain.append(names[go]);go=parents.get(go)
  return '/'.join(reversed(chain))
 return names,hierarchy
result={'baseline':BASE,'files':{},'runtimeCodeUnchanged':True,'originalFileCount':0,'originalUnchanged':True,'errors':[]}
for relative in ['Assets/Prefabs/Runtime/RecoveredMain.prefab','Assets/Scenes/RecoveredMain.unity']:
 rel=(PROJECT/relative).relative_to(REPO).as_posix();old=docs(git('show',BASE+':'+rel));new=docs((PROJECT/relative).read_text(encoding='utf-8'))
 names,hierarchy=structures(new);oldnames,oldhierarchy=structures(old);changes=[]
 for ident in old.keys()|new.keys():
  if old.get(ident)==new.get(ident):continue
  d=new if ident in new else old;t,b=d[ident];go=owner(d,ident);chain=(hierarchy if ident in new else oldhierarchy)(go)
  changed={'id':ident,'class':t,'hierarchy':chain,'added':ident not in old,'removed':ident not in new}
  allowed=any(part in chain.split('/') for part in ['SettingDialog','mergeRuleDialog','VersionGM'])
  # Other menu pages only opt into their already-existing runtime canvas stack sorting.
  if not allowed and t=='223' and ident in old and ident in new:
   allowed=old[ident][1].replace('m_OverrideSorting: 0','m_OverrideSorting: 1')==new[ident][1]
  changed['allowed']=allowed;changes.append(changed)
  if not allowed:result['errors'].append(relative+': '+str(changed))
 result['files'][relative]=changes
changedTracked=git('diff','--name-only',BASE,'--',str(PROJECT.relative_to(REPO)/'Assets/Scripts')).strip()
result['runtimeCodeUnchanged']=not changedTracked
if changedTracked:result['errors'].append('Runtime code changed: '+changedTracked)
manifest=json.loads((PROJECT/'COPY_MANIFEST.json').read_text(encoding='utf-8'))
for rel,wanted in manifest['sourceFileHashes'].items():
 f=PROJECT.parent/'06_UnityFramework'/rel;result['originalFileCount']+=1
 if not f.exists() or hashlib.sha256(f.read_bytes()).hexdigest()!=wanted:result['originalUnchanged']=False;result['errors'].append('Original changed: '+rel)
result['passed']=not result['errors']
(PROJECT/'Design/popup_scope_audit.json').write_bytes((json.dumps(result,indent=2,ensure_ascii=False)+'\n').encode('utf-8'))
print(json.dumps({k:v for k,v in result.items() if k!='files'},indent=2,ensure_ascii=False))
