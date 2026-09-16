using System;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace CoinMerge.Recovery.Editor
{
    // A narrow local asset-authoring request for an already-open Editor; no network or arbitrary execution.
    [InitializeOnLoad] public static class RulesReskinEditorRequest
    {
        static RulesReskinEditorRequest(){EditorApplication.update+=Poll;}
        static void Poll()
        {
            bool settings=!File.Exists("Temp/RulesReskin.request")&&File.Exists("Temp/SettingsReskin.request");
            string request=settings?"Temp/SettingsReskin.request":"Temp/RulesReskin.request";
            if(EditorApplication.isCompiling||EditorApplication.isUpdating||!File.Exists(request))return;
            if(SessionState.GetBool("CoinMerge.RulesReskin.Validation",false)||SessionState.GetBool("CoinMerge.SettingsReskin.Validation",false))return;
            if(EditorApplication.isPlaying){EditorApplication.ExitPlaymode();return;}
            if(EditorApplication.isPlayingOrWillChangePlaymode)return;
            File.Delete(request);
            try{if(settings){SettingsReskinAuthor.Run();SettingsReskinValidation.Run();}else{RulesReskinAuthor.Run();RulesReskinValidation.Run();}}
            catch(Exception e){string dir=settings?"Design/SettingsR1/Verification":"Design/RulesR1/Verification";Directory.CreateDirectory(dir);File.WriteAllText(dir+"/author_error.txt",e.ToString());Debug.LogException(e);}
        }
    }
}
