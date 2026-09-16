param([Parameter(Mandatory=$true)][string]$Destination)
$ErrorActionPreference='Stop'
$packageRoot=Split-Path $PSScriptRoot -Parent
$repoRoot=Split-Path (Split-Path $packageRoot -Parent) -Parent
$original=[IO.Path]::GetFullPath((Join-Path $repoRoot 'Restoration/06_UnityFramework')).TrimEnd('\')
$target=[IO.Path]::GetFullPath($Destination).TrimEnd('\')
if ($target -eq $original -or $target.StartsWith($original+'\',[StringComparison]::OrdinalIgnoreCase)) { throw 'Apply only to an independent copy, never the original project.' }
if (!(Test-Path -LiteralPath (Join-Path $target 'ProjectSettings/ProjectVersion.txt'))) { throw 'Destination must be an existing independent Unity project copy.' }
$manifest=Get-Content -LiteralPath (Join-Path $packageRoot 'manifest.json') -Raw | ConvertFrom-Json
# Validate every file before performing any writes. Already applied files are allowed.
foreach($entry in $manifest.files) {
 $dest=[IO.Path]::GetFullPath((Join-Path $target $entry.path))
 if (!$dest.StartsWith($target+'\',[StringComparison]::OrdinalIgnoreCase)) { throw 'Invalid patch path.' }
 $source=Join-Path (Join-Path $packageRoot 'Patch') $entry.path
 if ((Get-FileHash -LiteralPath $source -Algorithm SHA256).Hash -ne $entry.after) { throw ('Corrupt patch: '+$entry.path) }
 if(Test-Path -LiteralPath $dest) {
  $actual=(Get-FileHash -LiteralPath $dest -Algorithm SHA256).Hash
  if ($actual -ne $entry.before -and $actual -ne $entry.after) { throw ('Destination differs from baseline: '+$entry.path) }
 } elseif ($entry.before) { throw ('Missing baseline file: '+$entry.path) }
}
foreach($entry in $manifest.files) {
 $dest=Join-Path $target $entry.path
 New-Item -ItemType Directory -Path (Split-Path $dest -Parent) -Force | Out-Null
 Copy-Item -LiteralPath (Join-Path (Join-Path $packageRoot 'Patch') $entry.path) -Destination $dest -Force
}
Write-Output ('Applied '+$manifest.files.Count+' verified visual files. Open Assets/Scenes/RecoveredMain.unity in Unity 2022.3.62f3c1.')
