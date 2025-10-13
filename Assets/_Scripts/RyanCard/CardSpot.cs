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
    public bool isJousting;

    void Awake() { if (!rect) rect = GetComponent<RectTransform>(); }
    public void Start() { if (!cardHome) cardHome = CardManager.Instance.cardHome; }
    void OnEnable() { if (!rect) rect = GetComponent<RectTransform>(); if (cardHome && rect) cardHome.AddItem(rect); }
    void OnDisable() { if (cardHome && rect) cardHome.RemoveItem(rect); }
    void OnDestroy() { if (visual) Destroy(visual.gameObject); }
    public void OnPointerEnter(PointerEventData eventData) { if (isJousting) return; isHovered = true; if (cardHome) cardHome.Layout(); if (visual) visual.OnHover(); }
    public void OnPointerExit(PointerEventData eventData) { if (isJousting) return; isHovered = false; if (cardHome) cardHome.Layout(); if (visual) visual.OffHover(); }
    public void OnPointerDown(PointerEventData eventData) {
        if (isJousting) return;
        if (!isSelected) CardManager.Instance.DeselectAllCards();
        if (visual) visual.PlayPointerDownShake();
        if (isSelected) { isSelected = false; if (cardHome) cardHome.DeselectCard(this); }
        else { isSelected = true; if (cardHome) cardHome.SelectCard(this); }
    }
}
