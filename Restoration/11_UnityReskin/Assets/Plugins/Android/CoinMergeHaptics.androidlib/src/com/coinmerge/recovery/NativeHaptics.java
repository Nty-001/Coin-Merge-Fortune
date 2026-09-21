package com.coinmerge.recovery;

import android.content.Context;
import android.os.Build;
import android.os.VibrationEffect;
import android.os.Vibrator;

public final class NativeHaptics {
    private NativeHaptics() {}
    public static void pulse(Context context, int milliseconds, int amplitude) {
        if (context == null) return;
        Vibrator vibrator = (Vibrator) context.getSystemService(Context.VIBRATOR_SERVICE);
        if (vibrator == null || !vibrator.hasVibrator()) return;
        int duration = Math.max(1, milliseconds);
        if (Build.VERSION.SDK_INT >= 26) {
            vibrator.vibrate(VibrationEffect.createOneShot(duration, Math.max(1, Math.min(255, amplitude))));
        } else {
            vibrator.vibrate(duration);
        }
    }
}
