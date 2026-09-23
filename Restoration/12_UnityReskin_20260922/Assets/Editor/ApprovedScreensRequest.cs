using System;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace CoinMerge.Recovery.Editor
{
    // Narrow local authoring request for the already-open Editor. Never builds a player.
    [InitializeOnLoad] public static class ApprovedScreensRequest
    {
        const string Request="Temp/ApprovedScreens.request",Result="Temp/ApprovedScreensApplied.txt";
        static ApprovedScreensRequest(){EditorApplication.update+=Poll;}
        static void Poll()
        {
            if(!File.Exists(Request)||EditorApplication.isCompiling||EditorApplication.isUpdating)return;
            if(EditorApplication.isPlaying){EditorApplication.ExitPlaymode();return;}
            if(EditorApplication.isPlayingOrWillChangePlaymode)return;
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            if(EditorApplication.isCompiling||EditorApplication.isUpdating)return;
            File.Delete(Request);
            try{ApprovedScreensAuthor.Run();File.WriteAllText(Result,"OK: six approved screens authored in scene and prefab. "+DateTime.UtcNow.ToString("O"));}
            catch(Exception error){File.WriteAllText(Result,error.ToString());Debug.LogException(error);}
        }
    }
}
