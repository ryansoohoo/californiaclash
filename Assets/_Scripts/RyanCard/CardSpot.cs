using UnityEngine;
using UnityEngine.EventSystems;

public class CardSpot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    public CardHome cardHome;
    public RectTransform rect;
    public bool isHovered;
    public CardVisualSpot visual;
    public Gestures gesture;
    public int cardIndex = -1;
    void Awake() {
        if (!rect) rect = GetComponent<RectTransform>();
        if (!cardHome) cardHome = CardHome.Instance;
    }
    void OnEnable() {
        if (!rect) rect = GetComponent<RectTransform>();
        if (!cardHome) cardHome = CardHome.Instance;
        if (cardHome && rect) cardHome.AddItem(rect);
    }
    void OnDisable() {
        if (cardHome && rect) cardHome.RemoveItem(rect);
    }
    public void OnPointerEnter(PointerEventData eventData) {
        isHovered = true;
        if (cardHome) cardHome.Layout();
    }
    public void OnPointerExit(PointerEventData eventData) {
        isHovered = false;
        if (cardHome) cardHome.Layout();
    }
}
