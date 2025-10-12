using System.Collections.Generic;

public readonly struct GestureData {
    public readonly EGestures gesture;
    public readonly HashSet<EGestures> wins;
    public readonly HashSet<EGestures> loses; // keep original name for compatibility
    public readonly HashSet<EGestures> ties;

    public GestureData(EGestures g, IEnumerable<EGestures> w, IEnumerable<EGestures> l, IEnumerable<EGestures> t) {
        gesture = g;
        wins = w != null ? new HashSet<EGestures>(w) : new HashSet<EGestures>();
        loses = l != null ? new HashSet<EGestures>(l) : new HashSet<EGestures>();
        ties = t != null ? new HashSet<EGestures>(t) : new HashSet<EGestures>();
    }
}