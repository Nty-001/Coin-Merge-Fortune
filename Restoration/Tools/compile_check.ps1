$ErrorActionPreference = 'Stop'
$taskRoot = Split-Path $PSScriptRoot -Parent
$taskProject = Join-Path $taskRoot '06_UnityFramework'
$taskEditorData = 'C:\Program Files\Unity\Hub\Editor\2022.3.62f3c1\Editor\Data'
$taskCompiler = Join-Path $taskEditorData 'MonoBleedingEdge\lib\mono\4.5\csc.exe'
$taskMono = Join-Path $taskEditorData 'MonoBleedingEdge\bin\mono.exe'
$taskRefs = Get-ChildItem (Join-Path $taskEditorData 'Managed') -Recurse -Filter '*.dll' | Where-Object { $_.Name -like 'UnityEngine*' -or $_.Name -like 'UnityEditor*' }
$taskUi = Join-Path $taskEditorData 'Resources\PackageManager\ProjectTemplates\libcache\com.unity.template.2d-7.0.4\ScriptAssemblies\UnityEngine.UI.dll'
$taskOut = Join-Path $taskRoot '07_Verification\Compiled'
New-Item -ItemType Directory -Path $taskOut -Force | Out-Null
$taskResponse = @('/nologo','/target:library','/langversion:latest',('/out:"' + (Join-Path $taskOut 'CoinMergeFramework.dll') + '"'))
$taskResponse += $taskRefs | ForEach-Object { '/reference:"' + $_.FullName + '"' }
$taskResponse += '/reference:"' + $taskUi + '"'
$taskResponse += '/reference:"' + (Join-Path $taskEditorData 'UnityReferenceAssemblies\unity-4.8-api\Facades\netstandard.dll') + '"'
$taskResponse += Get-ChildItem (Join-Path $taskProject 'Assets') -Recurse -Filter '*.cs' | ForEach-Object { '"' + $_.FullName + '"' }
$taskRsp = Join-Path $taskOut 'compile.rsp'
[System.IO.File]::WriteAllLines($taskRsp,$taskResponse,[System.Text.UTF8Encoding]::new($false))
& $taskMono $taskCompiler ('@' + $taskRsp) 2>&1 | Tee-Object -FilePath (Join-Path $taskRoot '07_Verification\csharp_compile.txt')
exit $LASTEXITCODE
