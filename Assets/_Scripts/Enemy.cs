using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
    static readonly List<EGestures>[] Stages =
    {
        new List<EGestures> { EGestures.Rock, EGestures.Paper, EGestures.FYou },
        new List<EGestures> { EGestures.Rock, EGestures.Scissors, EGestures.Paper },
        new List<EGestures> { EGestures.Rock, EGestures.Scissors, EGestures.Paper, EGestures.FYou, EGestures.Gun, EGestures.Peace },
        new List<EGestures> { EGestures.Rock, EGestures.Scissors, EGestures.Paper, EGestures.FYou, EGestures.Gun, EGestures.Peace, EGestures.Nose, EGestures.Boo, EGestures.OK }
    };

    public List<EGestures> GetAvailableGestures(int stage) {
        if (stage < 0) stage = 0;
        if (stage > 3) stage = 3;
        return Stages[stage];
    }

    public EGestures GetRandomGesture(int stage) {
        var list = GetAvailableGestures(stage);
        var i = Random.Range(0, list.Count);
        return list[i];
    }
}
