using System.Collections.Generic;
using System.Text;
using UnityEngine;
using TMPro;

public sealed class GestureWinTieTracker : MonoBehaviour
{
    [SerializeField] private TMP_Text targetText;
    [SerializeField] private Color firstGestureColor = Color.green;
    [SerializeField] private Color secondGestureColor = Color.red;
    [SerializeField] private Color tieColor = Color.yellow;

    private readonly HashSet<(EGestures, EGestures)> logged = new HashSet<(EGestures, EGestures)>();
    private readonly List<(EGestures, EGestures, int)> results = new List<(EGestures, EGestures, int)>(128);
    private static readonly Dictionary<EGestures, HashSet<EGestures>> Wins = new Dictionary<EGestures, HashSet<EGestures>>
    {
        { EGestures.FYou, new HashSet<EGestures>{ EGestures.OK, EGestures.Nose } },
        { EGestures.Scissors, new HashSet<EGestures>{ EGestures.FYou, EGestures.OK, EGestures.Paper, EGestures.Nose } },
        { EGestures.OK, new HashSet<EGestures>{ EGestures.Paper, EGestures.Peace } },
        { EGestures.Gun, new HashSet<EGestures>{ EGestures.FYou, EGestures.Scissors, EGestures.OK, EGestures.Paper, EGestures.Nose, EGestures.Rock, EGestures.Boo } },
        { EGestures.Paper, new HashSet<EGestures>{ EGestures.Nose, EGestures.Peace, EGestures.Rock } },
        { EGestures.Nose, new HashSet<EGestures>{ EGestures.Peace, EGestures.Rock, EGestures.Boo } },
        { EGestures.Peace, new HashSet<EGestures>{ EGestures.Scissors, EGestures.Gun } },
        { EGestures.Rock, new HashSet<EGestures>{ EGestures.FYou, EGestures.Scissors, EGestures.Boo } },
        { EGestures.Boo, new HashSet<EGestures>{ EGestures.FYou, EGestures.Scissors, EGestures.OK } }
    };
    private static readonly Dictionary<EGestures, HashSet<EGestures>> Ties = new Dictionary<EGestures, HashSet<EGestures>>
    {
        { EGestures.FYou, new HashSet<EGestures>{ EGestures.FYou } },
        { EGestures.Scissors, new HashSet<EGestures>{ EGestures.Scissors } },
        { EGestures.OK, new HashSet<EGestures>{ EGestures.OK } },
        { EGestures.Gun, new HashSet<EGestures>() },
        { EGestures.Paper, new HashSet<EGestures>{ EGestures.Paper } },
        { EGestures.Nose, new HashSet<EGestures>{ EGestures.Nose } },
        { EGestures.Peace, new HashSet<EGestures>{ EGestures.FYou, EGestures.Peace, EGestures.Rock, EGestures.Boo } },
        { EGestures.Rock, new HashSet<EGestures>{ EGestures.OK, EGestures.Peace, EGestures.Rock } },
        { EGestures.Boo, new HashSet<EGestures>{ EGestures.Peace, EGestures.Boo } }
    };

    private readonly StringBuilder view = new StringBuilder(1024);
    private string hexA;
    private string hexB;
    private string hexT;

    void Start()
    {
        hexA = ColorUtility.ToHtmlStringRGB(firstGestureColor);
        hexB = ColorUtility.ToHtmlStringRGB(secondGestureColor);
        hexT = ColorUtility.ToHtmlStringRGB(tieColor);
        if (targetText != null) targetText.text = string.Empty;
    }

    public bool AddOutcome(EGestures a, EGestures b)
    {
        int outcome = Outcome(a, b);
        if (outcome < 0) return false;
        if (!logged.Add((a, b))) return false;
        results.Add((a, b, outcome));
        if (targetText == null) return true;
        view.Append(FormatLine(a, b, outcome));
        targetText.text = view.ToString();
        return true;
    }

    public void ScanExtended()
    {
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                var a = (EGestures)i;
                var b = (EGestures)j;
                if ((a == EGestures.Rock || a == EGestures.Paper || a == EGestures.Scissors) && (b == EGestures.Rock || b == EGestures.Paper || b == EGestures.Scissors)) continue;
                int outcome = Outcome(a, b);
                if (outcome < 0) continue;
                if (logged.Add((a, b))) results.Add((a, b, outcome));
            }
        }
        UpdateOutputs();
    }

    int Outcome(EGestures a, EGestures b)
    {
        if (a.Equals(b)) return 0;
        if (Wins.TryGetValue(a, out var w) && w != null && w.Contains(b)) return 1;
        if (Ties.TryGetValue(a, out var t) && t != null && t.Contains(b)) return 0;
        return -1;
    }

    void UpdateOutputs()
    {
        view.Clear();
        for (int k = 0; k < results.Count; k++)
        {
            var r = results[k];
            view.Append(FormatLine(r.Item1, r.Item2, r.Item3));
        }
        if (targetText != null) targetText.text = view.ToString();
    }

    string FormatLine(EGestures a, EGestures b, int outcome)
    {
        if (outcome == 1) return $"<color=#{hexA}>{a}</color> beat <color=#{hexB}>{b}</color>\n";
        return $"<color=#{hexT}>{a}</color> tie <color=#{hexT}>{b}</color>\n";
    }
}
