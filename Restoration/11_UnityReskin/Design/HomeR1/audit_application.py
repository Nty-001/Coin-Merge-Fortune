"""Verify art-only scope against the byte-verified source copy. Standard library only."""
import hashlib
import json
import re
from pathlib import Path

project = Path(__file__).resolve().parents[2]
source = project.parent / '06_UnityFramework'
manifest = json.loads((project / 'COPY_MANIFEST.json').read_text(encoding='utf-8-sig'))
digest = lambda p: hashlib.sha256(p.read_bytes()).hexdigest()
source_errors = [rel for rel, sha in manifest['sourceFileHashes'].items() if digest(source / rel) != sha]
assert not source_errors, source_errors

candidate = project / 'Design/HomeR1/PureArt_R2'
approved = json.loads((candidate / 'CANDIDATE.json').read_text(encoding='utf-8-sig'))
for record in approved['files']:
    assert digest(candidate / record['path']) == record['sha256'], record['path']
    assert digest(project / record['path']) == record['sha256'], record['path']

art_files = {entry['path'] for entry in approved['files']}
scene_files = {'Assets/Scenes/RecoveredMain.unity', 'Assets/Prefabs/Runtime/RecoveredMain.prefab'}
unexpected = []
for rel, sha in manifest['sourceFileHashes'].items():
    if digest(project / rel) != sha and rel not in art_files | scene_files | {'ProjectSettings/ProjectSettings.asset'}:
        unexpected.append(rel)
assert not unexpected, unexpected

bindings = {}
for rel in sorted(scene_files):
    original = (source / rel).read_text(encoding='utf-8-sig').splitlines(keepends=True)
    changed = (project / rel).read_text(encoding='utf-8-sig').splitlines(keepends=True)
    assert len(original) == len(changed), rel
    changes = []
    clean = []
    for line_number, (old, new) in enumerate(zip(original, changed), 1):
        if old.rstrip() == new.rstrip():
            clean.append(old)
        else:
            assert old.strip().startswith('m_Sprite:') and new.strip().startswith('m_Sprite:'), (rel, line_number, old, new)
            changes.append({'line': line_number, 'old': old.strip(), 'new': new.strip()})
            clean.append(new)
    assert len(changes) == 13, (rel, len(changes))
    # Drop Unity's unrelated trailing-whitespace normalization for reviewable diffs.
    (project / rel).write_text(''.join(clean), encoding='utf-8', newline='\n')
    bindings[rel] = changes

report = {
    'passed': True,
    'originalFilesUnchanged': len(manifest['sourceFileHashes']),
    'approvedCandidateFilesMatch': len(approved['files']),
    'changedImageBindingsPerRoot': 13,
    'runtimeScriptsChanged': False,
    'layoutFontsPhysicsButtonsChanged': False,
    'existingImporterMetadataChanged': False,
    'unexpectedChanges': unexpected,
    'imageBindings': bindings,
}
output = project / 'Design/HomeR1/Verification/static_audit.json'
output.parent.mkdir(parents=True, exist_ok=True)
output.write_text(json.dumps(report, indent=2, ensure_ascii=False), encoding='utf-8')
print(json.dumps({k: v for k, v in report.items() if k != 'imageBindings'}, ensure_ascii=False))
