public static class GestureOutcome {
    public const int GestureCount = 9;

    /// <summary>
    /// One-way lookup: only consults A's table entry.
    /// Returns: 1 (A wins), 0 (tie/undefined), -1 (A loses).
    /// </summary>
    public static int OutcomeFromTable(EGestures a, EGestures b) {
        if (a == b) return 0;

        if (!GestureLookupTable.Table.TryGetValue(a, out var dataA))
            return 0; // unknown gesture -> treat as tie

        if (dataA.ties.Contains(b)) return 0;
        if (dataA.wins.Contains(b)) return 1;
        if (dataA.loses.Contains(b)) return -1;

        // If not explicitly encoded from A's perspective, treat as neutral/tie.
        return 0;
    }
}