using System;
using UnityEngine;

public static class GestureOutcome {
    public static int OutcomeFromTable(Gestures a, Gestures b) {
        if (a == b) return 0;

        var dataA = GetData(a);
        if (dataA.ties != null && dataA.ties.Contains(b)) return 0;
        if (dataA.wins != null && dataA.wins.Contains(b)) return 1;
        if (dataA.loses != null && dataA.loses.Contains(b)) return -1;

        var dataB = GetData(b);
        if (dataB.ties != null && dataB.ties.Contains(a)) return 0;
        if (dataB.wins != null && dataB.wins.Contains(a)) return -1;
        if (dataB.loses != null && dataB.loses.Contains(a)) return 1;

        return 0;
    }
    private static GestureData GetData(Gestures g) {
        switch (g) {
            case Gestures.FYou: return GestureLookupTable.FYou;
            case Gestures.Scissors: return GestureLookupTable.Scissors;
            case Gestures.OK: return GestureLookupTable.OK;
            case Gestures.Gun: return GestureLookupTable.Gun;
            case Gestures.Paper: return GestureLookupTable.Paper;
            case Gestures.Nose: return GestureLookupTable.Nose;
            case Gestures.Peace: return GestureLookupTable.Peace;
            case Gestures.Rock: return GestureLookupTable.Rock;
            case Gestures.Boo: return GestureLookupTable.Boo;
            default: return default;
        }
    }

    public const int GestureCount = 9;
}
