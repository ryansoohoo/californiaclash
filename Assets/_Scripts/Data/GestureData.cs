using System.Collections.Generic;

public readonly struct GestureData {
    public readonly Gestures gesture;
    public readonly HashSet<Gestures> wins;
    public readonly HashSet<Gestures> loses; // keep original name for compatibility
    public readonly HashSet<Gestures> ties;

    public GestureData(Gestures g, IEnumerable<Gestures> w, IEnumerable<Gestures> l, IEnumerable<Gestures> t) {
        gesture = g;
        wins = w != null ? new HashSet<Gestures>(w) : new HashSet<Gestures>();
        loses = l != null ? new HashSet<Gestures>(l) : new HashSet<Gestures>();
        ties = t != null ? new HashSet<Gestures>(t) : new HashSet<Gestures>();
    }
}