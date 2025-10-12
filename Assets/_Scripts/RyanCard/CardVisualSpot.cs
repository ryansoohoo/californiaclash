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
    public Image selectedX;
    [SerializeField] float duration = 0.2f;
    [SerializeField] Ease ease = Ease.InOutSine;
    [SerializeField] float hoverLift = 12f;
    [SerializeField] float hoverScaleMultiplier = 1.06f;
    Vector2 lastTarget;
    Vector2 hoverOffset;
    Tween tweenX;
    Tween tweenY;
    Tween tweenScale;
    bool spawned;
    bool isHovered;
    Vector3 originalScale = Vector3.one;

    void Awake() {
        if (!rect) rect = GetComponent<RectTransform>();
        hoverOffset = new Vector2(0f, hoverLift);
        if (rect) originalScale = rect.localScale;
    }

    void OnValidate() {
        hoverOffset = new Vector2(0f, hoverLift);
    }

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
        var target = ComputeTarget();
        if (target != lastTarget) AnimateTo(target);
        int desired = mySpot.cardIndex < 0 ? 0 : mySpot.cardIndex;
        int max = rect.parent ? rect.parent.childCount - 1 : 0;
        if (desired > max) desired = max;
        if (isHovered && rect.parent) desired = rect.parent.childCount - 1;
        if (rect.GetSiblingIndex() != desired) rect.SetSiblingIndex(desired);
    }

    Vector2 ComputeTarget() {
        var t = mySpot.rect.anchoredPosition;
        if (isHovered) t += hoverOffset;
        return t;
    }

    void AnimateTo(Vector2 target) {
        if (tweenX.isAlive) tweenX.Stop();
        if (tweenY.isAlive) tweenY.Stop();
        tweenX = Tween.UIAnchoredPositionX(rect, target.x, duration, ease);
        tweenY = Tween.UIAnchoredPositionY(rect, target.y, duration, ease);
        lastTarget = target;
    }

    public void OnHover() {
        if (!rect) return;
        isHovered = true;
        if (rect.parent) rect.SetSiblingIndex(rect.parent.childCount - 1);
        if (tweenScale.isAlive) tweenScale.Stop();
        tweenScale = Tween.LocalScale(rect, originalScale * hoverScaleMultiplier, duration, ease);
        AnimateTo(ComputeTarget());
    }

    public void OffHover() {
        if (!rect) return;
        isHovered = false;
        if (tweenScale.isAlive) tweenScale.Stop();
        tweenScale = Tween.LocalScale(rect, originalScale, duration, ease);
        AnimateTo(ComputeTarget());
    }
}
