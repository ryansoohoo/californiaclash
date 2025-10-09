using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public struct GestureData {
    public Gestures gesture;
    public List<Gestures> wins;
    public List<Gestures> loses;
    public List<Gestures> ties;

    public GestureData(Gestures g, List<Gestures> w, List<Gestures> l, List<Gestures> t) {
        gesture = g;
        wins = w;
        loses = l;
        ties = t;
    }
}
