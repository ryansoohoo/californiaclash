using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardHome : MonoBehaviour {
    public static CardHome Instance { get; private set; }
    public enum VisibilityMode { UIBySibling, SpriteSortingOrder }

    [SerializeField] RectTransform container;
    [SerializeField] List<RectTransform> items;
    [SerializeField] float itemWidth = 0f;
    [SerializeField] float baseGap = 12f;
    [SerializeField] float sidePadding = 0f;
    [SerializeField] float anchoredY = 0f;
    [SerializeField] float minGap = -200f;
    [SerializeField] VisibilityMode visibility = VisibilityMode.UIBySibling;
    [SerializeField] string spriteSortingLayer = "";
    [SerializeField] int spriteSortingBase = 0;
    [SerializeField] float hoverExtraGap = 24f;

    RectTransform _rt;
    List<Order> _order;
    int _lastHoverIndex = int.MinValue;

    struct Order { public float x; public RectTransform rt; }

    void Awake() {
        if (Instance == null) Instance = this;
        if (items == null) items = new List<RectTransform>(8);
        if (_order == null) _order = new List<Order>(64);
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
        if (n == 1) { Place(items[0], 0f); ApplyVisibilitySingle(); ApplyGradient(); return; }

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

        ApplyVisibility(hoveredIndex);
        ApplyGradient();
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

    void ApplyVisibilitySingle() {
        if (visibility == VisibilityMode.UIBySibling) {
            items[0]?.SetAsLastSibling();
        }
        else {
            var rt = items[0];
            if (!rt) return;
            var sr = rt.GetComponent<SpriteRenderer>() ?? rt.GetComponentInChildren<SpriteRenderer>();
            if (!sr) return;
            if (!string.IsNullOrEmpty(spriteSortingLayer)) sr.sortingLayerName = spriteSortingLayer;
            sr.sortingOrder = spriteSortingBase;
        }
    }

    void ApplyVisibility(int hoveredIndex = -1) {
        _order.Clear();
        for (int i = 0; i < items.Count; i++) {
            var rt = items[i];
            if (!rt) continue;
            _order.Add(new Order { x = rt.anchoredPosition.x, rt = rt });
        }

        for (int i = 1; i < _order.Count; i++) {
            var key = _order[i];
            int j = i - 1;
            while (j >= 0 && _order[j].x > key.x) { _order[j + 1] = _order[j]; j--; }
            _order[j + 1] = key;
        }

        if (visibility == VisibilityMode.UIBySibling) {
            for (int i = _order.Count - 1; i >= 0; i--) _order[i].rt.SetAsLastSibling();
            if (hoveredIndex >= 0 && hoveredIndex < items.Count && items[hoveredIndex]) items[hoveredIndex].SetAsLastSibling();
        }
        else {
            int top = spriteSortingBase + _order.Count;
            for (int i = 0; i < _order.Count; i++) {
                var rt = _order[i].rt;
                var sr = rt.GetComponent<SpriteRenderer>() ?? rt.GetComponentInChildren<SpriteRenderer>();
                if (!sr) continue;
                if (!string.IsNullOrEmpty(spriteSortingLayer)) sr.sortingLayerName = spriteSortingLayer;
                sr.sortingOrder = top - (i + 1);
            }
            if (hoveredIndex >= 0 && hoveredIndex < items.Count) {
                var rt = items[hoveredIndex];
                if (rt) {
                    var sr = rt.GetComponent<SpriteRenderer>() ?? rt.GetComponentInChildren<SpriteRenderer>();
                    if (sr) {
                        if (!string.IsNullOrEmpty(spriteSortingLayer)) sr.sortingLayerName = spriteSortingLayer;
                        sr.sortingOrder = top + 1;
                    }
                }
            }
        }
    }

    void ApplyGradient() {
        _order.Clear();
        for (int i = 0; i < items.Count; i++) {
            var rt = items[i];
            if (!rt) continue;
            _order.Add(new Order { x = rt.anchoredPosition.x, rt = rt });
        }

        for (int i = 1; i < _order.Count; i++) {
            var key = _order[i];
            int j = i - 1;
            while (j >= 0 && _order[j].x > key.x) { _order[j + 1] = _order[j]; j--; }
            _order[j + 1] = key;
        }

        int m = _order.Count;
        if (m == 0) return;
        if (m == 1) { SetColor(_order[0].rt, 1f, 0f); return; }

        for (int i = 0; i < m; i++) {
            float t = m <= 1 ? 0f : i / (float)(m - 1);
            SetColor(_order[i].rt, 1f - t, t);
        }
    }

    static void SetColor(RectTransform rt, float r, float b) {
        var img = rt.GetComponent<Image>();
        if (img) { var c = img.color; var nc = new Color(r, 0f, b, c.a); if (c != nc) img.color = nc; return; }
        var sr = rt.GetComponent<SpriteRenderer>() ?? rt.GetComponentInChildren<SpriteRenderer>();
        if (sr) { var c = sr.color; var nc = new Color(r, 0f, b, c.a); if (c != nc) sr.color = nc; }
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
