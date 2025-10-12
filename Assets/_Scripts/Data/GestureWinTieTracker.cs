using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GestureWinTieTracker : MonoBehaviour
{
    [SerializeField] private TMP_Text targetText;
    [SerializeField] private Color winColor = Color.green;
    [SerializeField] private Color tieColor = Color.yellow;

    private readonly HashSet<(EGestures, EGestures)> loggedCombinations = new HashSet<(EGestures, EGestures)>();
    private readonly List<(EGestures, EGestures, int)> outcomes = new List<(EGestures, EGestures, int)>();

    void Start()
    {
        CheckAllCombinations();
        PrintCombinations();
        UpdateText();
    }

    void CheckAllCombinations()
    {
        for (int i = 0; i < GestureOutcome.GestureCount; i++)
        {
            for (int j = 0; j < GestureOutcome.GestureCount; j++)
            {
                var a = (EGestures)i;
                var b = (EGestures)j;
                int outcome = GestureOutcome.OutcomeFromTable((EGestures)i, (EGestures)j);
                if (outcome == 1 || outcome == 0)
                {
                    if (!loggedCombinations.Contains((a, b)))
                    {
                        loggedCombinations.Add((a, b));
                        outcomes.Add((a, b, outcome));
                    }
                }
            }
        }
    }

    void PrintCombinations()
    {
        foreach (var combo in outcomes)
            Debug.Log($"{combo.Item1} vs {combo.Item2} : {(combo.Item3 == 1 ? "Win" : "Tie")}");
    }

    void UpdateText()
    {
        if (targetText == null) return;
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        foreach (var combo in outcomes)
        {
            Color color = combo.Item3 == 1 ? winColor : tieColor;
            string hex = ColorUtility.ToHtmlStringRGB(color);
            sb.Append($"<color=#{hex}>{combo.Item1} vs {combo.Item2} : {(combo.Item3 == 1 ? "Win" : "Tie")}</color>\n");
        }
        targetText.text = sb.ToString();
    }
}
