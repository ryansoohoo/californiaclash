using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

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
    void Awake() {
        if (Instance == null)
            Instance = this;
        if (visualHome && !visualHome.home) visualHome.home = cardHome;
        if (visualHome && !visualHome.visualPrefab) visualHome.visualPrefab = visualPrefab;
        if (visualHome) visualHome.SyncFromHome();
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
        spots.Add(spot);
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
        if (spot) Destroy(spot.gameObject);
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
        foreach (CardSpot spot in spots) {
            spot.isSelected = false;
        }

    }
}
