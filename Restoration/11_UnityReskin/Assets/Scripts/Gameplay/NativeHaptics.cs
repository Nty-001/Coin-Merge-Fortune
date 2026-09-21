using System;
using UnityEngine;
namespace CoinMerge.Recovery
{
    // Explicit JNI method binding, not System.Reflection/Java reflection.
    public static class NativeHaptics
    {
        static IntPtr bridge,method,unityPlayer,activityField;
        static readonly jvalue[] arguments=new jvalue[3];
        public static void Pulse(int milliseconds,int amplitude)
        {
            if(Application.platform==RuntimePlatform.Android)
            {
                if(bridge==IntPtr.Zero)
                {
                    var local=AndroidJNI.FindClass("com/coinmerge/recovery/NativeHaptics");
                    bridge=AndroidJNI.NewGlobalRef(local);AndroidJNI.DeleteLocalRef(local);
                    method=AndroidJNI.GetStaticMethodID(bridge,"pulse","(Landroid/content/Context;II)V");
                    local=AndroidJNI.FindClass("com/unity3d/player/UnityPlayer");
                    unityPlayer=AndroidJNI.NewGlobalRef(local);AndroidJNI.DeleteLocalRef(local);
                    activityField=AndroidJNI.GetStaticFieldID(unityPlayer,"currentActivity","Landroid/app/Activity;");
                }
                var activity=AndroidJNI.GetStaticObjectField(unityPlayer,activityField);
                arguments[0].l=activity;arguments[1].i=milliseconds;arguments[2].i=amplitude;
                try{AndroidJNI.CallStaticVoidMethod(bridge,method,arguments);}finally{AndroidJNI.DeleteLocalRef(activity);}
            }
            else if(Application.platform==RuntimePlatform.IPhonePlayer)Handheld.Vibrate();
        }
    }
}
