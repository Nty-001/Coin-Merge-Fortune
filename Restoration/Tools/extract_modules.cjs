const fs = require('fs'), path = require('path'), vm = require('vm');
const root = path.resolve(__dirname, '../..');
const targets = {
  Packaged: 'export_20260914/unpacked/base/assets/assets/main/index.ba75b.js',
  HotUpdate: 'export_20260914/unpacked/runtime_adaljkjf/assets/main/index.9a050.js'
};
const inventory = [];
for (const [variant, rel] of Object.entries(targets)) {
  const source = fs.readFileSync(path.join(root, rel), 'utf8');
  const start = source.indexOf('}({');
  if (start < 0) throw Error('Unknown bundle wrapper');
  let modules;
  // Capture function objects without executing any game or SDK module.
  vm.runInNewContext('capture({' + source.slice(start + 3), {
    capture: function (m) { modules = m; }
  }, { timeout: 10000 });
  const out = path.join(root, 'Restoration/02_Gameplay', variant);
  fs.mkdirSync(out + '/Modules', {recursive:true});
  fs.mkdirSync(out + '/AlgorithmDependencies', {recursive:true});
  fs.copyFileSync(path.join(root, rel), out + '/original_bundle.js.txt');
  const manifest = [];
  for (const [name, [fn, deps]] of Object.entries(modules)) {
    const body = fn.toString();
    const numeric = /^\d+$/.test(name);
    const relative = (numeric ? 'AlgorithmDependencies/' : 'Modules/') + name + '.js';
    fs.writeFileSync(path.join(out, relative),
      '// Recovered complete compiled JavaScript module function.\n' +
      '// Original bundle: ' + rel + '\n' +
      '// Module: ' + name + '; dependency map: ' + JSON.stringify(deps) + '\n' +
      'module.exports = ' + body + ';\n');
    const classId = body.match(/cc\._RF\.push\([^,]+,\s*"([^"]+)"/);
    const entry = {name, file:relative, dependencies:deps, classId:classId?.[1] || null,
      category:numeric?'bundled algorithm dependency':'gameplay/application module',
      source:rel, bodyBytes:Buffer.byteLength(body)};
    manifest.push(entry);
  }
  fs.writeFileSync(out + '/module_manifest.json', JSON.stringify(manifest,null,2));
  inventory.push({variant,modules:manifest.filter(m=>m.category==='gameplay/application module').length,
    algorithmModules:manifest.filter(m=>m.category!=='gameplay/application module').length});
}
fs.writeFileSync(path.join(root,'Restoration/module_summary.json'),JSON.stringify(inventory,null,2));
console.log(JSON.stringify(inventory));
