using UnityEngine;
using UnityEngine.EventSystems;

public class CardSpot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    public CardHome cardHome;
    public RectTransform rect;
    public bool isHovered;
    public CardVisualSpot visual;
    public Gestures gesture;

    void Awake() {
        if (!rect) rect = GetComponent<RectTransform>();
        if (!cardHome) cardHome = CardHome.Instance;
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
