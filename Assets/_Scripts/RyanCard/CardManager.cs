using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using PrimeTween;

public class CardManager : MonoBehaviour {
    public static CardManager Instance;
    public CardHome cardHome;
    [SerializeField] CardSpot cardSpotPrefab;
    [SerializeField] CardHomeVisual visualHome;
    [SerializeField] CardVisualSpot visualPrefab;
    [SerializeField] int initialCount = 0;
    public GestureSprites gestureSprites;
    readonly List<CardSpot> spots = new List<CardSpot>(128);
    public CardSpot selectedCard;
    public RectTransform joustTransform;
    Tween moveTween;
    public CardSpot joustingCard;
    void Awake() {
        if (Instance == null) Instance = this;
        if (visualHome && !visualHome.home) visualHome.home = cardHome;
        if (visualHome && !visualHome.visualPrefab) visualHome.visualPrefab = visualPrefab;
        if (visualHome) visualHome.SyncFromHome();
    }

    public void SubmitSelectedCard() {
        if (joustingCard)
            return;
        if (selectedCard == null || !joustTransform) return;
        if (moveTween.IsAlive) moveTween.Stop();
        var spot = selectedCard;
        joustingCard = selectedCard;
        var r = spot.GetComponent<RectTransform>();
        if (r && cardHome) cardHome.RemoveItem(r);
        var tr = spot.transform;
        var target = joustTransform.position;
        moveTween = Tween.Position(tr, target, 0.25f, Ease.InOutQuad);
        selectedCard.GetComponent<CardSpot>().isSelected = false;
        selectedCard.GetComponent<CardSpot>().visual.selectedX.enabled = false;
        selectedCard.GetComponent<CardSpot>().isJousting = true;
    }

    public void EndJoust() {
        AddCard(joustingCard);
        joustingCard.isJousting = false;
        joustingCard = null;
    }

    public void AddCardDebug() {
        AddCard((EGestures)RandomNumberGenerator.GetInt32(0, 9));
    }

    public CardSpot AddCard(EGestures gesture) {
        if (!cardHome || !cardSpotPrefab) return null;
        var s = Instantiate(cardSpotPrefab, cardHome.transform);
        var spot = s.GetComponent<CardSpot>();
        if (!spot) spot = s.gameObject.AddComponent<CardSpot>();
        if (!spot.rect) spot.rect = s.GetComponent<RectTransform>();
        spot.cardHome = cardHome;
        spot.gesture = gesture;
        if (spot.rect) cardHome.AddItem(spot.rect);
        if (!spots.Contains(spot)) spots.Add(spot);
        if (visualHome) visualHome.AddVisualFor(spot);
        return spot;
    }

    public CardSpot AddCard(CardSpot spot) {
        if (!spot || !cardHome) return null;
        if (!spot.rect) spot.rect = spot.GetComponent<RectTransform>();
        spot.cardHome = cardHome;
        if (spot.rect) cardHome.AddItem(spot.rect);
        if (!spots.Contains(spot)) spots.Add(spot);
        if (visualHome) visualHome.AddVisualFor(spot);
        return spot;
    }

    public bool RemoveCard() {
        if (spots.Count == 0) return false;
        return RemoveCardAt(spots.Count - 1);
    }

    public bool RemoveCardAt(int index) {
        if (index < 0 || index >= spots.Count) return false;
        var spot = spots[index];
        if (visualHome) visualHome.RemoveFor(spot);
        if (spot && spot.rect && cardHome) cardHome.RemoveItem(spot.rect);
        spots.RemoveAt(index);
        if (visualHome) visualHome.SyncFromHome();
        return true;
    }

    public bool RemoveCard(CardSpot spot) {
        if (!spot) return false;
        int idx = spots.IndexOf(spot);
        if (idx < 0) return false;
        return RemoveCardAt(idx);
    }

    public void ClearAll() {
        for (int i = spots.Count - 1; i >= 0; i--) RemoveCardAt(i);
    }

    public void DeselectAllCards() {
        foreach (CardSpot spot in spots) spot.isSelected = false;
    }
}
