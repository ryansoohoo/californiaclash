using System;
using UnityEngine;

public static class GestureOutcome {
    public const int GestureCount = 9;

    /// out: 1 if a beats b, 0 if tie, -1 if a loses to b
    public static int Outcome(Gestures a, Gestures b) {
        if (a == b) return 0;
        int n = GestureCount;
        int k = (n - 1) >> 1;
        int d = ((int)b - (int)a) % n;
        if (d < 0) d += n;
        return (d >= 1 && d <= k) ? 1 : -1;
    }
}
