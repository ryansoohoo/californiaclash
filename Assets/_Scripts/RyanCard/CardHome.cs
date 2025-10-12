using System.Collections.Generic;
using UnityEngine;

public class CardHome : MonoBehaviour {
    public static CardHome Instance { get; private set; }
    [SerializeField] RectTransform container;
    [SerializeField] CardSpot selectedCard;
    [SerializeField] List<RectTransform> items;
    [SerializeField] float itemWidth = 0f;
    [SerializeField] float baseGap = 12f;
    [SerializeField] float sidePadding = 0f;
    [SerializeField] float anchoredY = 0f;
    [SerializeField] float minGap = -200f;
    [SerializeField] float hoverExtraGap = 24f;
    RectTransform _rt;
    int _lastHoverIndex = int.MinValue;
    readonly Dictionary<RectTransform, int> _stableOrder = new Dictionary<RectTransform, int>(64);
    int _nextOrderId;

    sealed class GestureStableComparer : IComparer<RectTransform> {
        readonly Dictionary<RectTransform, int> order;
        public GestureStableComparer(Dictionary<RectTransform, int> orderMap) { order = orderMap; }
        public int Compare(RectTransform a, RectTransform b) {
            int ga = int.MaxValue;
            int gb = int.MaxValue;
            if (a) {
                var sa = a.GetComponent<CardSpot>();
                if (sa) ga = (int)sa.gesture;
            }
            if (b) {
                var sb = b.GetComponent<CardSpot>();
                if (sb) gb = (int)sb.gesture;
            }
            int g = ga.CompareTo(gb);
            if (g != 0) return g;
            int oa = order.TryGetValue(a, out var va) ? va : int.MaxValue;
            int ob = order.TryGetValue(b, out var vb) ? vb : int.MaxValue;
            return oa.CompareTo(ob);
        }
    }

    void Awake() {
        if (Instance == null) Instance = this;
        if (items == null) items = new List<RectTransform>(8);
    }

    void OnEnable() {
        if (!_rt) _rt = GetComponent<RectTransform>();
        if (!container) container = _rt;
        _lastHoverIndex = GetHoveredIndex();
        EnsureStableOrder();
        Layout();
    }

    void Update() {
        int idx = GetHoveredIndex();
        if (idx != _lastHoverIndex) { _lastHoverIndex = idx; Layout(); }
    }

    //from card spot
    public void SelectCard(CardSpot spot) {
        if(selectedCard)
            selectedCard.visual.selectedX.enabled = false;
        spot.visual.selectedX.enabled = true;
        selectedCard = spot;
        CardManager.Instance.selectedCard = spot;
    }
    //from card spot
    public void DeselectCard(CardSpot spot) {
        if (selectedCard)
            selectedCard.visual.selectedX.enabled = false;
        spot.visual.selectedX.enabled = false;
        selectedCard = null;
        CardManager.Instance.selectedCard = null;
    }

    public void Layout() {
        if (!_rt) _rt = GetComponent<RectTransform>();
        if (!container) container = _rt;
        for (int i = items.Count - 1; i >= 0; i--) if (!items[i]) items.RemoveAt(i);
        int n = items.Count;
        if (n == 0) return;
        EnsureStableOrder();
        items.Sort(new GestureStableComparer(_stableOrder));
        float W = container.rect.width > 0 ? container.rect.width : 750f;
        float w = ResolveItemWidth(n);
        if (n == 1) { SetIndex(items[0], 0); Place(items[0], 0f); return; }
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
            SetIndex(it, i);
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

    void SetIndex(RectTransform item, int idx) {
        var s = item.GetComponent<CardSpot>();
        if (s && s.cardIndex != idx) s.cardIndex = idx;
    }

    void UpdateIndicesFrom(int start) {
        int n = items.Count;
        if (start < 0) start = 0;
        for (int i = start; i < n; i++) {
            var it = items[i];
            if (!it) continue;
            SetIndex(it, i);
        }
    }

    public void SetItems(List<RectTransform> list) {
        items = list ?? new List<RectTransform>();
        _stableOrder.Clear();
        _nextOrderId = 0;
        for (int i = 0; i < items.Count; i++) {
            var it = items[i];
            if (!it) continue;
            if (!_stableOrder.ContainsKey(it)) _stableOrder[it] = _nextOrderId++;
        }
        UpdateIndicesFrom(0);
        Layout();
    }

    public void AddItem(RectTransform rt) {
        if (rt == null) return;
        if (items.Contains(rt)) return;
        if (!_stableOrder.ContainsKey(rt)) _stableOrder[rt] = _nextOrderId++;
        int idx = items.Count;
        items.Add(rt);
        UpdateIndicesFrom(idx);
        Layout();
    }

    public void AddItemAt(RectTransform rt, int insertIndex) {
        if (rt == null) return;
        if (items.Contains(rt)) return;
        int n = items.Count;
        if (insertIndex < 0) insertIndex = 0;
        if (insertIndex > n) insertIndex = n;
        if (!_stableOrder.ContainsKey(rt)) _stableOrder[rt] = _nextOrderId++;
        items.Insert(insertIndex, rt);
        UpdateIndicesFrom(insertIndex);
        Layout();
    }

    public void RemoveItem(RectTransform rt) {
        if (rt == null) return;
        int idx = items.IndexOf(rt);
        if (idx < 0) return;
        items.RemoveAt(idx);
        _stableOrder.Remove(rt);
        UpdateIndicesFrom(idx);
        Layout();
    }

    public void Clear() {
        items.Clear();
        _stableOrder.Clear();
        _nextOrderId = 0;
        Layout();
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

    void EnsureStableOrder() {
        for (int i = 0; i < items.Count; i++) {
            var it = items[i];
            if (!it) continue;
            if (!_stableOrder.ContainsKey(it)) _stableOrder[it] = _nextOrderId++;
        }
    }
}
