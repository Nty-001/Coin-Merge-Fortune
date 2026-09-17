using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace CoinMerge.Recovery.Editor
{
    // Run on a disposable build copy so the open authoring project stays untouched.
    public static class AndroidApkBuild
    {
        public static void Run()
        {
            if (!Application.isBatchMode) throw new InvalidOperationException("Use a batch build copy.");
            string output = Environment.GetEnvironmentVariable("COINMERGE_APK_OUTPUT");
            if (string.IsNullOrEmpty(output)) throw new InvalidOperationException("COINMERGE_APK_OUTPUT is required.");
#if UNITY_ANDROID
            string toolchain = Environment.GetEnvironmentVariable("COINMERGE_ANDROID_TOOLCHAIN");
            if (!string.IsNullOrEmpty(toolchain))
            {
                UnityEditor.Android.AndroidExternalToolsSettings.sdkRootPath = Path.Combine(toolchain, "SDK");
                UnityEditor.Android.AndroidExternalToolsSettings.ndkRootPath = Path.Combine(toolchain, "NDK");
                UnityEditor.Android.AndroidExternalToolsSettings.jdkRootPath = Path.Combine(toolchain, "OpenJDK");
            }
#endif
            var balance = AssetDatabase.LoadAssetAtPath<GameBalanceConfig>("Assets/Config/Runtime/GameBalance.asset");
            if (!balance || !balance.defaultRewardedVariant || balance.defaultCohort != "B")
                throw new InvalidOperationException("Expected the current B rewarded build configuration.");

            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.coinmergefortune.reskin");
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARMv7 | AndroidArchitecture.ARM64;
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel22;
            PlayerSettings.Android.targetSdkVersion = (AndroidSdkVersions)34;
            PlayerSettings.Android.useCustomKeystore = false;
            PlayerSettings.Android.buildApkPerCpuArchitecture = false;
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
            EditorUserBuildSettings.buildAppBundle = false;
            EditorUserBuildSettings.exportAsGoogleAndroidProject = false;
            Directory.CreateDirectory(Path.GetDirectoryName(output));
            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = new[] { "Assets/Scenes/RecoveredLoading.unity", "Assets/Scenes/RecoveredMain.unity", "Assets/Scenes/RecoveredPackaged.unity" },
                locationPathName = output,
                target = BuildTarget.Android,
                options = BuildOptions.None
            });
            var summary = report.summary;
            File.WriteAllText(Path.ChangeExtension(output, ".build.txt"),
                "Result: " + summary.result + "\nBytes: " + summary.totalSize +
                "\nErrors: " + summary.totalErrors + "\nWarnings: " + summary.totalWarnings +
                "\nDuration: " + summary.totalTime + "\nUnity: " + Application.unityVersion +
                "\nPackage: com.coinmergefortune.reskin\nABI: armeabi-v7a, arm64-v8a\nDefault: B / US\nSigning: Android debug key (local test APK)\n");
            if (summary.result != BuildResult.Succeeded) throw new InvalidOperationException("Android APK build failed: " + summary.result);
            Debug.Log("ANDROID_APK_BUILD_SUCCEEDED " + output);
        }
    }
}
