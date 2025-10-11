using System;
using System.Collections.Generic;
using UnityEngine;

public static class GestureLookupTable {
    public static readonly GestureData FYou = new GestureData(
        Gestures.FYou,
        new List<Gestures> { Gestures.OK, Gestures.Nose }, // win
        new List<Gestures> { Gestures.Scissors, Gestures.Gun, Gestures.Paper, Gestures.Peace, Gestures.Rock, Gestures.Boo }, // lose
        new List<Gestures> { Gestures.FYou } //tie
    );

    public static readonly GestureData Scissors = new GestureData(
        Gestures.Scissors,
        new List<Gestures> { Gestures.FYou, Gestures.OK, Gestures.Paper, Gestures.Nose },
        new List<Gestures> { Gestures.Gun, Gestures.Peace, Gestures.Rock, Gestures.Boo },
        new List<Gestures> { Gestures.Scissors }
    );

    public static readonly GestureData OK = new GestureData(
        Gestures.OK,
        new List<Gestures> { Gestures.Paper, Gestures.Peace},
        new List<Gestures> { Gestures.FYou, Gestures.Scissors, Gestures.Gun, Gestures.Nose,Gestures.Rock,Gestures.Boo },
        new List<Gestures> { Gestures.OK }
    );

    public static readonly GestureData Gun = new GestureData(
        Gestures.Gun,
        new List<Gestures> { Gestures.FYou, Gestures.Scissors, Gestures.OK, Gestures.Paper, Gestures.Nose, Gestures.Rock, Gestures.Boo },
        new List<Gestures> { Gestures.Gun, Gestures.Peace },
        new List<Gestures> { }
    );

    public static readonly GestureData Paper = new GestureData(
        Gestures.Paper,
        new List<Gestures> { Gestures.Nose, Gestures.Peace, Gestures.Rock },
        new List<Gestures> { Gestures.FYou, Gestures.Scissors, Gestures.OK, Gestures.Gun, Gestures.Boo},
        new List<Gestures> { Gestures.Paper }
    );

    public static readonly GestureData Nose = new GestureData(
        Gestures.Nose,
        new List<Gestures> { Gestures.Peace, Gestures.Rock, Gestures.Boo},
        new List<Gestures> { Gestures.FYou, Gestures.Scissors, Gestures.OK, Gestures.Gun, Gestures.Paper },
        new List<Gestures> { Gestures.Nose }
    );

    public static readonly GestureData Peace = new GestureData(
        Gestures.Peace,
        new List<Gestures> { Gestures.Scissors, Gestures.Gun },
        new List<Gestures> { Gestures.OK, Gestures.Paper, Gestures.Nose },
        new List<Gestures> { Gestures.FYou, Gestures.Peace, Gestures.Rock, Gestures.Boo }
    );

    public static readonly GestureData Rock = new GestureData(
        Gestures.Rock,
        new List<Gestures> { Gestures.FYou, Gestures.Scissors, Gestures.Boo },
        new List<Gestures> { Gestures.Gun, Gestures.Paper, Gestures.Nose },
        new List<Gestures> { Gestures.OK, Gestures.Peace, Gestures.Rock }
    );

    public static readonly GestureData Boo = new GestureData(
        Gestures.Boo,
        new List<Gestures> { Gestures.FYou, Gestures.Scissors, Gestures.OK },
        new List<Gestures> { Gestures.Peace, Gestures.Boo },
        new List<Gestures> { Gestures.Gun, Gestures.Paper, Gestures.Nose, Gestures.Rock }
    );
}