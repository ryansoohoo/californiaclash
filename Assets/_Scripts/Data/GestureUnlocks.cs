using System.Collections.Generic;
using UnityEngine;
/// Starts with Rock–Paper–Scissors and expands to 5, 7, then 9 gestures.
public static class GestureUnlocks {
    static readonly Gestures[] Stage1 = { Gestures.Rock, Gestures.Scissors, Gestures.Paper };
    static readonly Gestures[] Stage2 = { Gestures.Rock, Gestures.FYou, Gestures.Scissors, Gestures.Gun, Gestures.Paper };
    static readonly Gestures[] Stage3 = { Gestures.Rock, Gestures.FYou, Gestures.Scissors, Gestures.OK, Gestures.Gun, Gestures.Paper, Gestures.Peace };
    static readonly Gestures[] Stage4 = { Gestures.Rock, Gestures.Boo, Gestures.FYou, Gestures.Scissors, Gestures.OK, Gestures.Gun, Gestures.Paper, Gestures.Nose, Gestures.Peace };

    public static List<Gestures> GetStage(int stage) {
        int s = Mathf.Clamp(stage, 1, 4);
        Gestures[] src = s == 1 ? Stage1 : s == 2 ? Stage2 : s == 3 ? Stage3 : Stage4;
        var list = new List<Gestures>(src.Length);
        list.AddRange(src);
        return list;
    }
}
