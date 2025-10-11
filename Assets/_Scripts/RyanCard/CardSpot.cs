using UnityEngine;

public class CardSpot : MonoBehaviour {
    public CardHome cardHome;
    public RectTransform rect;
    public bool isHovered;
    public CardVisualSpot visual;
    public Gestures gesture;

    void Awake() {
        if (!rect) rect = GetComponent<RectTransform>();
        if (!cardHome) cardHome = CardHome.Instance;
    }
}
