# Called by an explicit native JNI signature.
-keep class com.coinmerge.recovery.NativeHaptics {
    public static void pulse(android.content.Context, int, int);
}
