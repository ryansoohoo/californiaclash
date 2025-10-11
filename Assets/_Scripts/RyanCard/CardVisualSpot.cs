using UnityEngine;
using PrimeTween;
using UnityEngine.EventSystems;
using TMPro;
public class CardVisualSpot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    public CardSpot mySpot;
    public RectTransform rect;
    [SerializeField] float duration = 0.2f;
    [SerializeField] Ease ease = Ease.InOutSine;
    Vector2 lastTarget;
    Tween tweenX;
    Tween tweenY;
    Tween tweenScale;
    bool spawned;
    Vector3 originalScale = Vector3.one;
    public TextMeshProUGUI text;

    public void Bind(CardSpot s) {
        mySpot = s;
        if (!rect) rect = GetComponent<RectTransform>();
        if (mySpot && mySpot.rect && rect) {
            lastTarget = mySpot.rect.anchoredPosition;
            rect.anchoredPosition = lastTarget;
            if (!spawned) {
                originalScale = rect.localScale;
                rect.localScale = Vector3.zero;
                if (tweenScale.isAlive) tweenScale.Stop();
                tweenScale = Tween.LocalScale(rect, originalScale, duration, ease);
                spawned = true;
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (!mySpot) return;
        mySpot.isHovered = true;
        var ch = mySpot.cardHome;
        if (ch) ch.Layout();
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (!mySpot) return;
        mySpot.isHovered = false;
        var ch = mySpot.cardHome;
        if (ch) ch.Layout();
    }

    void LateUpdate() {
        if (!mySpot || !mySpot.rect || !rect) return;
        var target = mySpot.rect.anchoredPosition;
        if (target != lastTarget) {
            if (tweenX.isAlive) tweenX.Stop();
            if (tweenY.isAlive) tweenY.Stop();
            tweenX = Tween.UIAnchoredPositionX(rect, target.x, duration, ease);
            tweenY = Tween.UIAnchoredPositionY(rect, target.y, duration, ease);
            lastTarget = target;
        }
    }
}
