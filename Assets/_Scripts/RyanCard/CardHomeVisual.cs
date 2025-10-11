using System.Collections.Generic;
using UnityEngine;

public class CardHomeVisual : MonoBehaviour {
    public CardHome home;
    public CardVisualSpot visualPrefab;
    readonly Dictionary<CardSpot, CardVisualSpot> map = new Dictionary<CardSpot, CardVisualSpot>(128);
    static readonly List<CardSpot> tempSpots = new List<CardSpot>(256);

    void Awake() {
        if (!home) home = CardHome.Instance;
    }

    void OnEnable() {
        SyncFromHome();
    }

    public void SyncFromHome() {
        if (!home || !visualPrefab) return;
        tempSpots.Clear();
        for (int i = 0, c = home.transform.childCount; i < c; i++) {
            var t = home.transform.GetChild(i);
            var s = t.GetComponent<CardSpot>();
            if (s && s.rect) tempSpots.Add(s);
        }
        for (int i = tempSpots.Count - 1; i >= 0; i--) {
            var s = tempSpots[i];
            if (!map.ContainsKey(s)) CreateVisualFor(s);
        }
        using (var e = new List<CardSpot>(map.Keys).GetEnumerator()) {
            while (e.MoveNext()) {
                var k = e.Current;
                if (!k || !tempSpots.Contains(k)) RemoveFor(k);
            }
        }
    }

    public CardVisualSpot AddVisualFor(CardSpot spot) {
        if (!spot || map.ContainsKey(spot)) return null;
        return CreateVisualFor(spot);
    }

    public void RemoveFor(CardSpot spot) {
        if (!spot) return;
        if (map.TryGetValue(spot, out var vis)) {
            map.Remove(spot);
            if (spot.visual == vis) spot.visual = null;
            if (vis) Destroy(vis.gameObject);
        }
    }

    CardVisualSpot CreateVisualFor(CardSpot spot) {
        var vis = Instantiate(visualPrefab, transform);
        if (!vis.rect) vis.rect = vis.GetComponent<RectTransform>();
        vis.Bind(spot);
        spot.visual = vis;
        map[spot] = vis;
        return vis;
    }
}
