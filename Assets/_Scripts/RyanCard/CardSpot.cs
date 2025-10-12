using UnityEngine;
using UnityEngine.EventSystems;

public class CardSpot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler {
    public CardHome cardHome;
    public RectTransform rect;
    public bool isHovered;
    public CardVisualSpot visual;
    public EGestures gesture;
    public int cardIndex = -1;
    public bool isSelected;

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
        visual.OnHover();
    }
    public void OnPointerExit(PointerEventData eventData) {
        isHovered = false;
        if (cardHome) cardHome.Layout();
        visual.OffHover();
    }

    public void OnPointerDown(PointerEventData eventData) {
        isSelected = true;
        if (cardHome) cardHome.SelectCard(this);
    }
}
