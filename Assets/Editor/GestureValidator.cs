#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

public static class GestureValidator {
    // Toggle to also check B's side for symmetric consistency (win/lose or tie/tie).
    // Keep FALSE if you're intentionally using one-way encoding.
    private const bool CheckSymmetry = false;

    // Menu: Tools/Gestures/Validate Lookup Table  (Cmd/Ctrl + Shift + G)
    [MenuItem("Tools/Gestures/Validate Lookup Table %#g")]
    public static void Validate() {
        int issues = 0;

        // --- 0) Basic table sanity ----------------------------------------------------------
        var allGestures = Enum.GetValues(typeof(EGestures)).Cast<EGestures>().ToArray();

        // Check that we have an entry for every gesture
        foreach (var g in allGestures) {
            if (!GestureLookupTable.Table.TryGetValue(g, out var data)) {
                Debug.LogError($"Missing table entry for gesture: {g}");
                issues++;
                continue;
            }

            // Ensure the struct’s gesture field matches the key
            if (!data.gesture.Equals(g)) {
                Debug.LogError($"Key/Value mismatch: key={g} but data.gesture={data.gesture}");
                issues++;
            }

            // Sets must be non-null (constructor already ensures this, but we validate anyway)
            if (data.wins == null || data.loses == null || data.ties == null) {
                Debug.LogError($"{g}: wins/loses/ties must not be null.");
                issues++;
            }

            // No overlaps between wins/loses/ties for a single gesture
            if (data.wins.Overlaps(data.loses)) {
                Debug.LogError($"{g}: A gesture cannot be in both WINS and LOSES.");
                issues++;
            }
            if (data.wins.Overlaps(data.ties)) {
                Debug.LogError($"{g}: A gesture cannot be in both WINS and TIES.");
                issues++;
            }
            if (data.loses.Overlaps(data.ties)) {
                Debug.LogError($"{g}: A gesture cannot be in both LOSES and TIES.");
                issues++;
            }
        }

        // --- 1) Per-pair one-way validation ------------------------------------------------
        foreach (var a in allGestures) {
            if (!GestureLookupTable.Table.TryGetValue(a, out var A)) {
                // Already counted above; skip pairs for this A
                continue;
            }

            foreach (var b in allGestures) {
                // From A's perspective, exactly one outcome for every B
                bool aWins = A.wins.Contains(b);
                bool aLoses = A.loses.Contains(b);
                bool aTies = A.ties.Contains(b);

                int aCount = (aWins ? 1 : 0) + (aLoses ? 1 : 0) + (aTies ? 1 : 0);
                if (aCount != 1) {
                    Debug.LogError($"[{a} vs {b}] A-side must have exactly one outcome, got {State(aWins, aLoses, aTies)}.");
                    issues++;
                }

                // --- 2) Optional symmetric consistency check --------------------------------
                if (CheckSymmetry && GestureLookupTable.Table.TryGetValue(b, out var B)) {
                    bool bWins = B.wins.Contains(a);
                    bool bLoses = B.loses.Contains(a);
                    bool bTies = B.ties.Contains(a);

                    // If A says win, B should say lose; etc.
                    bool consistent =
                        (aWins && bLoses) ||
                        (aLoses && bWins) ||
                        (aTies && bTies);

                    if (!consistent) {
                        Debug.LogError($"Conflict {a} vs {b} — A:{State(aWins, aLoses, aTies)}  B:{State(bWins, bLoses, bTies)}");
                        issues++;
                    }
                }
            }
        }

        // --- Final report -------------------------------------------------------------------
        if (issues == 0)
            Debug.Log("<color=green>Gesture lookup table looks consistent ✔</color>");
        else
            Debug.LogError($"Found {issues} issue(s). See logs above.");
    }

    private static string State(bool w, bool l, bool t) => w ? "win" : l ? "lose" : t ? "tie" : "(none)";
}
#endif
