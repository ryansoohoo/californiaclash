using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// Starts with Rock–Paper–Scissors and expands to 5, 7, then 9 gestures.
public static class GestureUnlocks {
    static readonly EGestures[] Stage1 = { EGestures.Rock, EGestures.Scissors, EGestures.Paper };
    static readonly EGestures[] Stage2 = { EGestures.Rock, EGestures.FYou, EGestures.Scissors, EGestures.Gun, EGestures.Paper };
    static readonly EGestures[] Stage3 = { EGestures.Rock, EGestures.FYou, EGestures.Scissors, EGestures.OK, EGestures.Gun, EGestures.Paper, EGestures.Peace };
    static readonly EGestures[] Stage4 = { EGestures.Rock, EGestures.Boo, EGestures.FYou, EGestures.Scissors, EGestures.OK, EGestures.Gun, EGestures.Paper, EGestures.Nose, EGestures.Peace };

    public static List<EGestures> GetStage(int stage) {
        int s = Mathf.Clamp(stage, 1, 4);
        EGestures[] src = s == 1 ? Stage1 : s == 2 ? Stage2 : s == 3 ? Stage3 : Stage4;
        var list = new List<EGestures>(src.Length);
        list.AddRange(src);
        return list;
    }
}
