using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

public sealed class GestureWinTieTester : EditorWindow
{
    Vector2 scroll;
    readonly HashSet<int> seen = new HashSet<int>(256);
    readonly StringBuilder sb = new StringBuilder(4096);
    string output = "";

    [MenuItem("Tools/Gesture Win/Tie Tester")]
    static void Open() { GetWindow<GestureWinTieTester>("Gesture Tester").Show(); }

    void OnGUI()
    {
        if (GUILayout.Button("Scan Wins/Ties")) Scan();
        EditorGUILayout.Space();
        using (var view = new EditorGUILayout.ScrollViewScope(scroll))
        {
            scroll = view.scrollPosition;
            EditorGUILayout.TextArea(output, GUILayout.ExpandHeight(true));
        }
    }

    void Scan()
    {
        seen.Clear();
        if (sb.Length > 0) sb.Length = 0;
        int n = GestureOutcome.GestureCount;
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                var a = (EGestures)i;
                var b = (EGestures)j;
                int outcome = GestureOutcome.OutcomeFromTable(a, b);
                if (outcome < 0) continue;
                int key = i * n + j;
                if (!seen.Add(key)) continue;
                sb.Append(a).Append(" vs ").Append(b).Append(": ").Append(outcome == 1 ? "win" : "tie").Append('\n');
            }
        }
        output = sb.ToString();
        if (!string.IsNullOrEmpty(output)) Debug.Log(output);
    }
}
