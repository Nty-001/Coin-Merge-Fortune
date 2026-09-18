using System;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace CoinMerge.Recovery.Editor
{
    [InitializeOnLoad] public static class LoadingReskinRequest
    {
        static LoadingReskinRequest(){EditorApplication.update+=Poll;}
        static void Poll()
        {
            const string path="Temp/LoadingReskin.request";
            if(!File.Exists(path)||EditorApplication.isCompiling||EditorApplication.isUpdating||SessionState.GetBool("CoinMerge.LoadingReskin.Validation",false))return;
            if(EditorApplication.isPlaying){EditorApplication.ExitPlaymode();return;}
            if(EditorApplication.isPlayingOrWillChangePlaymode)return;
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);if(EditorApplication.isCompiling||EditorApplication.isUpdating)return;
            File.Delete(path);
            try{LoadingReskinAuthor.Run();LoadingReskinValidation.Run();}
            catch(Exception e){Directory.CreateDirectory("Design/LoadingR1/Verification");File.WriteAllText("Design/LoadingR1/Verification/author_error.txt",e.ToString());Debug.LogException(e);}
        }
    }
}
