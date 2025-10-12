using System.Collections.Generic;
using UnityEngine;

public class CardHomeVisual : MonoBehaviour {
    public CardHome home;
    public CardVisualSpot visualPrefab;
    readonly Dictionary<CardSpot, CardVisualSpot> map = new Dictionary<CardSpot, CardVisualSpot>(128);
    static readonly List<CardSpot> tempSpots = new List<CardSpot>(256);
    static readonly List<CardSpot> tempKeys = new List<CardSpot>(256);
    void Start() {
        if (!home) home = CardManager.Instance.cardHome;
    }
    void OnEnable() {
        SyncFromHome();
    }
    public void SyncFromHome() {
        if (!home || !visualPrefab) return;
        bool changed = false;
        tempSpots.Clear();
        for (int i = 0, c = home.transform.childCount; i < c; i++) {
            var t = home.transform.GetChild(i);
            var s = t.GetComponent<CardSpot>();
            if (s && s.rect) tempSpots.Add(s);
        }
        for (int i = tempSpots.Count - 1; i >= 0; i--) {
            var s = tempSpots[i];
            if (!map.ContainsKey(s)) {
                CreateVisualFor(s);
                changed = true;
            }
        }
        tempKeys.Clear();
        using (var e = map.Keys.GetEnumerator()) {
            while (e.MoveNext()) tempKeys.Add(e.Current);
        }
        for (int i = tempKeys.Count - 1; i >= 0; i--) {
            var k = tempKeys[i];
            if (!k || !tempSpots.Contains(k)) {
                RemoveFor(k);
                changed = true;
            }
        }
        if (changed && home) home.Layout();
        ApplyHierarchyOrder();
    }
    public CardVisualSpot AddVisualFor(CardSpot spot) {
        if (!spot || map.ContainsKey(spot)) return null;
        var v = CreateVisualFor(spot);
        if (home) home.Layout();
        ApplyHierarchyOrder();
        return v;
    }
    public void RemoveFor(CardSpot spot) {
        if (!spot) return;
        if (map.TryGetValue(spot, out var vis)) {
            map.Remove(spot);
            if (spot.visual == vis) spot.visual = null;
            if (vis) Destroy(vis.gameObject);
            if (home) home.Layout();
            ApplyHierarchyOrder();
        }
    }
    void ApplyHierarchyOrder() {
        int count = transform.childCount;
        if (count == 0) return;
        foreach (var kv in map) {
            var s = kv.Key;
            var v = kv.Value;
            if (!s || !v || !v.rect) continue;
            int desired = s.cardIndex < 0 ? 0 : s.cardIndex;
            if (desired >= count) desired = count - 1;
            if (v.rect.GetSiblingIndex() != desired) v.rect.SetSiblingIndex(desired);
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
