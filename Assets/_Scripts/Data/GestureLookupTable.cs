using System;
using System.Collections.Generic;

public static class GestureLookupTable {
    public static readonly IReadOnlyDictionary<Gestures, GestureData> Table =
        new Dictionary<Gestures, GestureData> {
            [Gestures.FYou] = new GestureData(
                Gestures.FYou,
                new[] { Gestures.OK, Gestures.Nose }, // win
                new[] { Gestures.Scissors, Gestures.Gun, Gestures.Paper, Gestures.Peace, Gestures.Rock, Gestures.Boo }, // lose
                new[] { Gestures.FYou } // tie
            ),
            [Gestures.Scissors] = new GestureData(
                Gestures.Scissors,
                new[] { Gestures.FYou, Gestures.OK, Gestures.Paper, Gestures.Nose },
                new[] { Gestures.Gun, Gestures.Peace, Gestures.Rock, Gestures.Boo },
                new[] { Gestures.Scissors }
            ),
            [Gestures.OK] = new GestureData(
                Gestures.OK,
                new[] { Gestures.Paper, Gestures.Peace },
                new[] { Gestures.FYou, Gestures.Scissors, Gestures.Gun, Gestures.Nose, Gestures.Rock, Gestures.Boo },
                new[] { Gestures.OK }
            ),
            [Gestures.Gun] = new GestureData(
                Gestures.Gun,
                new[] { Gestures.FYou, Gestures.Scissors, Gestures.OK, Gestures.Paper, Gestures.Nose, Gestures.Rock, Gestures.Boo },
                new[] { Gestures.Gun, Gestures.Peace },
                Array.Empty<Gestures>()
            ),
            [Gestures.Paper] = new GestureData(
                Gestures.Paper,
                new[] { Gestures.Nose, Gestures.Peace, Gestures.Rock },
                new[] { Gestures.FYou, Gestures.Scissors, Gestures.OK, Gestures.Gun, Gestures.Boo },
                new[] { Gestures.Paper }
            ),
            [Gestures.Nose] = new GestureData(
                Gestures.Nose,
                new[] { Gestures.Peace, Gestures.Rock, Gestures.Boo },
                new[] { Gestures.FYou, Gestures.Scissors, Gestures.OK, Gestures.Gun, Gestures.Paper },
                new[] { Gestures.Nose }
            ),
            [Gestures.Peace] = new GestureData(
                Gestures.Peace,
                new[] { Gestures.Scissors, Gestures.Gun },
                new[] { Gestures.OK, Gestures.Paper, Gestures.Nose },
                new[] { Gestures.FYou, Gestures.Peace, Gestures.Rock, Gestures.Boo }
            ),
            [Gestures.Rock] = new GestureData(
                Gestures.Rock,
                new[] { Gestures.FYou, Gestures.Scissors, Gestures.Boo },
                new[] { Gestures.Gun, Gestures.Paper, Gestures.Nose },
                new[] { Gestures.OK, Gestures.Peace, Gestures.Rock }
            ),
            [Gestures.Boo] = new GestureData(
                Gestures.Boo,
                new[] { Gestures.FYou, Gestures.Scissors, Gestures.OK },
                new[] { Gestures.Gun, Gestures.Paper, Gestures.Nose, Gestures.Rock },
                new[] { Gestures.Peace, Gestures.Boo }
            ),
        };
}
