"""Read-only project/source audit. Writes reports only in WorkReview_20260914."""
import collections
import hashlib
import json
import re
from pathlib import Path

ROOT=Path(__file__).resolve().parents[1]
OUT=Path(__file__).resolve().parent
PROJECT=ROOT/'Restoration/06_UnityFramework'

def read(path):
    return json.loads(path.read_text(encoding='utf-8-sig'))

def write(name,value):
    (OUT/name).write_text(json.dumps(value,ensure_ascii=False,indent=2),encoding='utf-8')

def sha(path):
    h=hashlib.sha256()
    with path.open('rb') as f:
        while data:=f.read(1024*1024):h.update(data)
    return h.hexdigest()

files=[]
for part in ('Assets','Packages','ProjectSettings'):
    for path in sorted((PROJECT/part).rglob('*')):
        if path.is_file():
            files.append(dict(path=path.relative_to(PROJECT).as_posix(),bytes=path.stat().st_size,sha256=sha(path)))
write('original_project_manifest.json',files)

log=(OUT/'unity_import_review.log').read_text(encoding='utf-8-sig',errors='replace')
parse=collections.defaultdict(list)
for path,line,msg in re.findall(r'Unable to parse file (.+?): \[Parser Failure at line (\d+): ([^\r\n]+)',log):
    parse[path].append(dict(line=int(line),message=msg))
report=read(OUT/'unity_scene_audit.json')
source_prefabs=list((PROJECT/'Assets/Prefabs').rglob('*.prefab'))
source_scenes=list((PROJECT/'Assets/Scenes').glob('*.unity'))
write('import_findings.json',dict(
    sourcePrefabCount=len(source_prefabs),sourceSceneCount=len(source_scenes),
    assetDatabaseDiscoveredPrefabCount=len(report['prefabs']),
    filesWithParserFailures=len(parse),parserFailureCount=sum(map(len,parse.values())),
    prefabFilesWithParserFailures=sum(x.endswith('.prefab') for x in parse),
    sceneFilesWithParserFailures=sum(x.endswith('.unity') for x in parse),
    nativeComponentAutoRepairLogCount=log.count('does not reference component'),
    transformChildLoadErrorCount=log.count("Transform child can't be loaded"),
    errorsByFile=parse,
    note='Editor process exit 0 does not mean asset import or gameplay parity passed.'
))

inventory=[]
for variant in ('Packaged','HotUpdate'):
    base=ROOT/'Restoration/02_Gameplay'/variant
    manifest=read(base/'module_manifest.json')
    for module in manifest:
        if not module['file'].startswith('Modules/'):continue
        path=base/module['file']
        source=path.read_text(encoding='utf-8')
        # Navigational method headings only; original complete function bodies remain primary evidence.
        methods=[]
        for number,line in enumerate(source.splitlines(),1):
            if re.match(r'^(?:(?:static|async|get|set) )*(?:[\w$]+)\([^;]*\) \{$',line) and not re.match(r'^(if|for|while|switch|catch|function)\b',line):
                methods.append(dict(line=number,declaration=line))
        inventory.append(dict(variant=variant,name=module['name'],file=path.relative_to(ROOT).as_posix(),
            classId=module.get('classId'),bodyBytes=module['bodyBytes'],sha256=sha(path),
            methods=methods,status='Original full JS retained; no equivalence inferred from method names.'))
write('source_module_review_index.json',inventory)

constants=read(ROOT/'Restoration/03_Configuration/EffectiveRuntime/Cached.normalized.json')
write('configuration_availability.json',dict(
    evidence='Local captured GameData normalized using original code; this audit made no request to game servers.',
    topLevelKeys=sorted(constants),
    regionalOverrides={country:country in constants for country in ('US','BR','ID','TH','VN','MY','PH')},
    globalDefaultsPresent='global' in constants,
    note='Missing per-country override is not proof the original server has none. Runtime defaults must retain original precedence.'
))

runtime_cs=list((PROJECT/'Assets/Scripts').rglob('*.cs'))
write('review_summary.json',dict(
    exactReplica=False,sourceProjectChanged=False,gitCheckpointCreated=False,gitPushed=False,
    remote='https://github.com/Nty-001/Coin-Merge-Fortune.git',remotePublic=True,
    runtimeCSharpFiles=len(runtime_cs),originalProjectFiles=len(files),originalProjectBytes=sum(x['bytes'] for x in files),
    unityLicenseProbe='passed_exit_0',importInspection='completed_with_asset_errors',
    allSourceModuleBodiesPreserved=True,
    mainGameplayPort='missing',visualParity='failed_structural_gate',countryABGM='missing',
    skill='C:/Users/001/.codex/skills/unity-native-prefab-development/SKILL.md'
))
print(json.dumps(dict(projectFiles=len(files),projectBytes=sum(x['bytes'] for x in files),
    parserFiles=len(parse),parserMessages=sum(map(len,parse.values())),prefabsDiscovered=len(report['prefabs']),
    sourceModules=len(inventory),runtimeCSharpFiles=len(runtime_cs)),indent=2))
