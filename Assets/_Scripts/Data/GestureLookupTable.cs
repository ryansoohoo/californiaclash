using System;
using System.Collections.Generic;

public static class GestureLookupTable {
    public static readonly IReadOnlyDictionary<EGestures, GestureData> Table =
        new Dictionary<EGestures, GestureData> {
            [EGestures.FYou] = new GestureData(
                EGestures.FYou,
                new[] { EGestures.OK, EGestures.Nose }, // win
                new[] { EGestures.Scissors, EGestures.Gun, EGestures.Paper, EGestures.Peace, EGestures.Rock, EGestures.Boo }, // lose
                new[] { EGestures.FYou } // tie
            ),
            [EGestures.Scissors] = new GestureData(
                EGestures.Scissors,
                new[] { EGestures.FYou, EGestures.OK, EGestures.Paper, EGestures.Nose },
                new[] { EGestures.Gun, EGestures.Peace, EGestures.Rock, EGestures.Boo },
                new[] { EGestures.Scissors }
            ),
            [EGestures.OK] = new GestureData(
                EGestures.OK,
                new[] { EGestures.Paper, EGestures.Peace },
                new[] { EGestures.FYou, EGestures.Scissors, EGestures.Gun, EGestures.Nose, EGestures.Rock, EGestures.Boo },
                new[] { EGestures.OK }
            ),
            [EGestures.Gun] = new GestureData(
                EGestures.Gun,
                new[] { EGestures.FYou, EGestures.Scissors, EGestures.OK, EGestures.Paper, EGestures.Nose, EGestures.Rock, EGestures.Boo },
                new[] { EGestures.Gun, EGestures.Peace },
                Array.Empty<EGestures>()
            ),
            [EGestures.Paper] = new GestureData(
                EGestures.Paper,
                new[] { EGestures.Nose, EGestures.Peace, EGestures.Rock },
                new[] { EGestures.FYou, EGestures.Scissors, EGestures.OK, EGestures.Gun, EGestures.Boo },
                new[] { EGestures.Paper }
            ),
            [EGestures.Nose] = new GestureData(
                EGestures.Nose,
                new[] { EGestures.Peace, EGestures.Rock, EGestures.Boo },
                new[] { EGestures.FYou, EGestures.Scissors, EGestures.OK, EGestures.Gun, EGestures.Paper },
                new[] { EGestures.Nose }
            ),
            [EGestures.Peace] = new GestureData(
                EGestures.Peace,
                new[] { EGestures.Scissors, EGestures.Gun },
                new[] { EGestures.OK, EGestures.Paper, EGestures.Nose },
                new[] { EGestures.FYou, EGestures.Peace, EGestures.Rock, EGestures.Boo }
            ),
            [EGestures.Rock] = new GestureData(
                EGestures.Rock,
                new[] { EGestures.FYou, EGestures.Scissors, EGestures.Boo },
                new[] { EGestures.Gun, EGestures.Paper, EGestures.Nose },
                new[] { EGestures.OK, EGestures.Peace, EGestures.Rock }
            ),
            [EGestures.Boo] = new GestureData(
                EGestures.Boo,
                new[] { EGestures.FYou, EGestures.Scissors, EGestures.OK },
                new[] { EGestures.Gun, EGestures.Paper, EGestures.Nose, EGestures.Rock },
                new[] { EGestures.Peace, EGestures.Boo }
            ),
        };
}
