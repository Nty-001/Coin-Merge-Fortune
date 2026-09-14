$ErrorActionPreference='Stop'
$taskRoot=Split-Path $PSScriptRoot -Parent
$taskMono='C:\Program Files\Unity\Hub\Editor\2022.3.62f3c1\Editor\Data\MonoBleedingEdge\bin\mono.exe'
$taskCompiler='C:\Program Files\Unity\Hub\Editor\2022.3.62f3c1\Editor\Data\MonoBleedingEdge\lib\mono\4.5\csc.exe'
$taskExe=Join-Path $taskRoot '07_Verification\Compiled\MockFlowChecks.exe'
& $taskMono $taskCompiler /nologo /target:exe ('/out:'+$taskExe) (Join-Path $PSScriptRoot 'MockFlowChecks.cs') (Join-Path $taskRoot '06_UnityFramework\Assets\Scripts\SDK\SdkFacade.cs') (Join-Path $taskRoot '06_UnityFramework\Assets\Scripts\Gameplay\MergeRules.cs')
if($LASTEXITCODE -ne 0){exit $LASTEXITCODE}
& $taskMono $taskExe | Tee-Object -FilePath (Join-Path $taskRoot '07_Verification\mock_flow_tests.txt')
exit $LASTEXITCODE
