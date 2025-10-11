using System.Collections.Generic;
using UnityEngine;

public class CardHome : MonoBehaviour {
    public static CardHome Instance { get; private set; }
    [SerializeField] RectTransform container;
    [SerializeField] List<RectTransform> items;
    [SerializeField] float itemWidth = 0f;
    [SerializeField] float baseGap = 12f;
    [SerializeField] float sidePadding = 0f;
    [SerializeField] float anchoredY = 0f;
    [SerializeField] float minGap = -200f;
    [SerializeField] float hoverExtraGap = 24f;
    RectTransform _rt;
    int _lastHoverIndex = int.MinValue;

    void Awake() {
        if (Instance == null) Instance = this;
        if (items == null) items = new List<RectTransform>(8);
    }

    void OnEnable() {
        if (!_rt) _rt = GetComponent<RectTransform>();
        if (!container) container = _rt;
        _lastHoverIndex = GetHoveredIndex();
        Layout();
    }

    void Update() {
        int idx = GetHoveredIndex();
        if (idx != _lastHoverIndex) { _lastHoverIndex = idx; Layout(); }
    }

    public void Layout() {
        if (!_rt) _rt = GetComponent<RectTransform>();
        if (!container) container = _rt;
        int n = items.Count;
        if (n == 0) return;
        float W = container.rect.width > 0 ? container.rect.width : 750f;
        float w = ResolveItemWidth(n);
        if (n == 1) { Place(items[0], 0f); return; }
        float desiredStep = w + baseGap;
        float desiredSpan = desiredStep * (n - 1);
        float usableW = Mathf.Max(0f, W - 2f * sidePadding);
        float maxSpan = Mathf.Max(0f, usableW - w);
        float gap = desiredSpan <= maxSpan ? baseGap : Mathf.Max(minGap, (maxSpan / (n - 1)) - w);
        float step = w + gap;
        int hoveredIndex = GetHoveredIndex();
        float extra = hoveredIndex >= 0 ? Mathf.Max(0f, hoverExtraGap) : 0f;
        float baseSpan = step * (n - 1);
        float startX = -0.5f * baseSpan;
        for (int i = 0; i < n; i++) {
            var it = items[i];
            if (!it) continue;
            if (it.anchorMin.x != 0.5f || it.anchorMax.x != 0.5f || it.pivot.x != 0.5f) {
                var am = it.anchorMin; am.x = 0.5f; it.anchorMin = am;
                var ax = it.anchorMax; ax.x = 0.5f; it.anchorMax = ax;
                var pv = it.pivot; pv.x = 0.5f; it.pivot = pv;
            }
            float x = startX + i * step;
            if (extra > 0f) {
                if (i <= hoveredIndex - 1) x -= extra;
                else if (i >= hoveredIndex + 1) x += extra;
            }
            Place(it, x);
        }
    }

    float ResolveItemWidth(int n) {
        if (itemWidth > 0f) return itemWidth;
        for (int i = 0; i < n; i++) {
            var r = items[i];
            if (r) {
                float c = r.rect.width;
                if (c > 0f) return c;
            }
        }
        return 100f;
    }

    void Place(RectTransform item, float x) {
        var p = item.anchoredPosition;
        if (p.x != x || p.y != anchoredY) item.anchoredPosition = new Vector2(x, anchoredY);
    }

    int GetHoveredIndex() {
        int n = items.Count;
        for (int i = 0; i < n; i++) {
            var rt = items[i];
            if (!rt) continue;
            var spot = rt.GetComponent<CardSpot>();
            if (spot != null && spot.isHovered) return i;
        }
        return -1;
    }

    public void SetItems(List<RectTransform> list) { items = list ?? new List<RectTransform>(); Layout(); }
    public void AddItem(RectTransform rt) { if (rt != null && !items.Contains(rt)) { items.Add(rt); Layout(); } }
    public void RemoveItem(RectTransform rt) { if (items.Remove(rt)) Layout(); }
    public void Clear() { items.Clear(); Layout(); }
}
