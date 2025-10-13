using PrimeTween;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameState : MonoBehaviour {
    public EGameState gameState;
    public List<EGestures> gestures;
    public CardManager cardManager;
    public EnemyCards enemyCards;
    public Enemy enemy;
    public Button submitButton;
    public CardHome home;
    public int level = 0;
    [SerializeField] float joustBounceOffset = 40f;
    [SerializeField] float joustBounceCycleDuration = 0.24f;
    [SerializeField] float joustClashDuration = 0.2f;
    [SerializeField] Transform clashAnchor;
    WaitForSeconds wfs0_3;
    WaitForSeconds wfs0_6;
    WaitForSeconds wfs0_75;
    bool isRunning;

    void Awake() {
        if (gestures == null) gestures = new List<EGestures>(8);
        wfs0_3 = new WaitForSeconds(0.3f);
        wfs0_6 = new WaitForSeconds(0.6f);
        wfs0_75 = new WaitForSeconds(0.75f);
    }

    void Start() {
        AddCard(EGestures.Rock);
        AddCard(EGestures.Paper);
        AddCard(EGestures.Scissors);
    }

    public void AddCard(EGestures gesture) {
        gestures.Add(gesture);
        if (cardManager != null) cardManager.AddCard(gesture);
    }

    public void STARTJOUST() {
        if (isRunning) return;
        StartCoroutine(JoustRoutine());
    }

    IEnumerator JoustRoutine() {
        isRunning = true;
        if (enemyCards != null) enemyCards.HideCards();
        yield return wfs0_3;
        if (enemyCards != null) enemyCards.StartJoust();
        yield return wfs0_6;
        if (enemyCards != null) enemyCards.LerpRandomEnemyCardToJoust();
        yield return wfs0_6;
        RectTransform b = cardManager != null && cardManager.joustingCard != null ? cardManager.joustingCard.GetComponent<RectTransform>() : null;
        RectTransform a = enemyCards != null && enemyCards.joustingCard != null ? enemyCards.joustingCard.GetComponent<RectTransform>() : null;
        yield return StartCoroutine(BounceThreeTimes(a, b, joustBounceOffset, joustBounceCycleDuration));
        if (enemyCards != null && enemyCards.joustingCard != null) enemyCards.joustingCard.UnHide();
        Vector3 targetWorld = clashAnchor != null ? clashAnchor.position : ((a != null && b != null) ? (a.position + b.position) * 0.5f : Vector3.zero);
        yield return wfs0_75;
        yield return StartCoroutine(ClashTogether(a, b, joustClashDuration, targetWorld));
        if (cardManager != null && cardManager.joustingCard != null) Debug.Log(cardManager.joustingCard.gesture);
        if (enemyCards != null && enemyCards.joustingCard != null) Debug.Log(enemyCards.joustingCard.gesture);
        int outcome = 0;
        if (cardManager != null && enemyCards != null && cardManager.joustingCard != null && enemyCards.joustingCard != null) outcome = GestureOutcome.OutcomeFromTable(cardManager.joustingCard.gesture, enemyCards.joustingCard.gesture);
        if (cardManager != null && enemyCards != null) {
            GestureWinTieTracker tracker = GetComponent<GestureWinTieTracker>();
            if (tracker != null && cardManager.joustingCard != null && enemyCards.joustingCard != null) {
                tracker.AddOutcome(cardManager.joustingCard.gesture, enemyCards.joustingCard.gesture);
                tracker.AddOutcome(enemyCards.joustingCard.gesture, cardManager.joustingCard.gesture);
            }
            if (outcome == 0) {
                if (cardManager.joustingCard != null) {
                    EGestures spot0 = cardManager.joustingCard.gesture;
                    cardManager.AddCard(spot0);
                    Destroy(cardManager.joustingCard);
                }
                enemyCards.ResetToStartingPositions();
                if (cardManager.cardHome != null) cardManager.cardHome.Show();
            }
            else if (outcome == 1) {
                if (cardManager.joustingCard != null) {
                    EGestures spot1 = cardManager.joustingCard.gesture;
                    if (enemyCards.joustingCard != null) enemyCards.joustingCard.gameObject.SetActive(false);
                    yield return wfs0_6;
                    Destroy(cardManager.joustingCard);
                    cardManager.AddCard(spot1);
                    if (enemyCards.joustingCard != null) enemyCards.joustingCard.gameObject.SetActive(true);
                }
                enemyCards.ResetToStartingPositions();
                if (cardManager.cardHome != null) cardManager.cardHome.Show();
                if (enemy != null) cardManager.AddCard(enemy.GetRandomGesture(level));
            }
            else if (outcome == -1) {
                if (cardManager.joustingCard != null) {
                    CardSpot spot2 = cardManager.joustingCard;
                    cardManager.RemoveCard(spot2);
                    Destroy(spot2.gameObject);
                }
                yield return wfs0_6;
                enemyCards.ResetToStartingPositions();
                if (cardManager.cardHome != null) cardManager.cardHome.Show();
            }
            level++;
            if (cardManager.cardHome != null) cardManager.cardHome.Layout();
            yield return wfs0_3;
            enemyCards.ShuffleCards();
            enemyCards.RandomizeGesture();
            if (cardManager.cardHome != null && cardManager.cardHome.transform.childCount <= 0) {
                cardManager.ClearAll();
                AddCard(EGestures.Rock);
                AddCard(EGestures.Paper);
                AddCard(EGestures.Scissors);
                level = -1;
                cardManager.joustingCard = null;
                enemyCards.joustingCard = null;
                cardManager.DeselectAllCards();
                cardManager.selectedCard = null;
            }
        }
        isRunning = false;
    }

    IEnumerator BounceThreeTimes(RectTransform a, RectTransform b, float offset, float cycleDuration) {
        if (a == null || b == null) yield break;
        Vector3 aStart = a.localPosition;
        Vector3 bStart = b.localPosition;
        Vector3 upA = aStart + Vector3.up * offset;
        Vector3 upB = bStart + Vector3.up * offset;
        Sequence seq = Sequence.Create();
        float half = cycleDuration * 0.5f;
        float halfA = half * 0.5f;
        float t = 0f;
        for (int i = 0; i < 3; i++) {
            seq.Insert(t, Tween.LocalPosition(a, upA, halfA, ease: Ease.OutSine));
            seq.Insert(t, Tween.LocalPosition(b, upB, half, ease: Ease.OutSine));
            t += half;
            seq.Insert(t, Tween.LocalPosition(a, aStart, halfA, ease: Ease.InSine));
            seq.Insert(t, Tween.LocalPosition(b, bStart, half, ease: Ease.InSine));
            t += half;
        }
        bool done = false;
        seq.OnComplete(() => done = true);
        while (!done) yield return null;
    }

    IEnumerator ClashTogether(RectTransform a, RectTransform b, float duration, Vector3 worldTarget) {
        if (a == null || b == null) yield break;
        Transform parentA = a.parent;
        Transform parentB = b.parent;
        if (parentA == null || parentB == null) yield break;
        Vector3 localA = parentA.InverseTransformPoint(worldTarget);
        Vector3 localB = parentB.InverseTransformPoint(worldTarget);
        Sequence seq = Sequence.Create();
        float durA = duration * 0.5f;
        float durB = duration;
        seq.Insert(0f, Tween.LocalPosition(a, localA, durA, ease: Ease.InOutSine));
        seq.Insert(0f, Tween.LocalPosition(b, localB, durB, ease: Ease.InOutSine));
        bool done = false;
        seq.OnComplete(() => done = true);
        while (!done) yield return null;
    }
}
