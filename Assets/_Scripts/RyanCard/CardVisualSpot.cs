using UnityEngine;
using PrimeTween;
using TMPro;
using UnityEngine.UI;

public class CardVisualSpot : MonoBehaviour {
    public CardSpot mySpot;
    public RectTransform rect;
    public TextMeshProUGUI leftText;
    public TextMeshProUGUI rightText;
    public Image image;
    public RawImage cardImage;
    [SerializeField] float duration = 0.2f;
    [SerializeField] Ease ease = Ease.InOutSine;
    Vector2 lastTarget;
    Tween tweenX;
    Tween tweenY;
    Tween tweenScale;
    bool spawned;
    Vector3 originalScale = Vector3.one;
    public void Bind(CardSpot s) {
        mySpot = s;
        if (!rect) rect = GetComponent<RectTransform>();
        if (mySpot && mySpot.rect && rect) {
            lastTarget = mySpot.rect.anchoredPosition;
            rect.anchoredPosition = lastTarget;
            if (!spawned) {
                leftText.text = s.gesture.ToString();
                rightText.text = s.gesture.ToString();
                originalScale = rect.localScale;
                rect.localScale = Vector3.zero;
                if (tweenScale.isAlive) tweenScale.Stop();
                tweenScale = Tween.LocalScale(rect, originalScale, duration, ease);
                spawned = true;
            }
            if (mySpot.cardHome) mySpot.cardHome.Layout();
            int desired = mySpot.cardIndex < 0 ? 0 : mySpot.cardIndex;
            int max = rect.parent ? rect.parent.childCount - 1 : 0;
            if (desired > max) desired = max;
            if (rect.GetSiblingIndex() != desired) rect.SetSiblingIndex(desired);
        }
        cardImage.texture = CardManager.Instance.gestureSprites.Get(s.gesture).texture;
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
        int desired = mySpot.cardIndex < 0 ? 0 : mySpot.cardIndex;
        int max = rect.parent ? rect.parent.childCount - 1 : 0;
        if (desired > max) desired = max;
        if (rect.GetSiblingIndex() != desired) rect.SetSiblingIndex(desired);
    }
}
