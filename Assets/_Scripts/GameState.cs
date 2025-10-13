using PrimeTween;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    public int level = 0; // 3 levels
    [SerializeField] float joustBounceOffset = 40f;
    [SerializeField] float joustBounceCycleDuration = 0.24f;
    [SerializeField] float joustClashDuration = 0.2f;
    [SerializeField] Transform clashAnchor;

    public void Start() {
        AddCard(EGestures.Rock);
        AddCard(EGestures.Paper);
        AddCard(EGestures.Scissors);
    }

    public void AddCard(EGestures gesture) {
        gestures.Add(gesture);
        cardManager.AddCard(gesture);
    }

    public async Task STARTJOUST() {
        enemyCards.HideCards();
        await Task.Delay(300);
        enemyCards.StartJoust();
        await Task.Delay(600);
        enemyCards.LerpRandomEnemyCardToJoust();
        await Task.Delay(600);
        var b = cardManager.joustingCard != null ? cardManager.joustingCard.GetComponent<RectTransform>() : null;
        var a = enemyCards.joustingCard != null ? enemyCards.joustingCard.GetComponent<RectTransform>() : null;
        await BounceThreeTimes(a, b, joustBounceOffset, joustBounceCycleDuration);
        enemyCards.joustingCard.UnHide();
        var targetWorld = clashAnchor != null ? clashAnchor.position : ((a != null && b != null) ? (a.position + b.position) * 0.5f : Vector3.zero);
        await Task.Delay(750);
        await ClashTogether(a, b, joustClashDuration, targetWorld);
        print(cardManager.joustingCard.gesture);
        print(enemyCards.joustingCard.gesture);
        int outcome = GestureOutcome.OutcomeFromTable(cardManager.joustingCard.gesture, enemyCards.joustingCard.gesture); // Returns: 1 (A wins), 0 (tie/undefined), -1 (A loses).
        Debug.Log(outcome);
        GetComponent<GestureWinTieTracker>().AddOutcome(cardManager.joustingCard.gesture, enemyCards.joustingCard.gesture);
        GetComponent<GestureWinTieTracker>().AddOutcome(enemyCards.joustingCard.gesture, cardManager.joustingCard.gesture);
        if (outcome == 0) {            
            EGestures spot = cardManager.joustingCard.gesture;
            cardManager.AddCard(spot);
            Destroy(cardManager.joustingCard);
            enemyCards.ResetToStartingPositions();
            cardManager.cardHome.Show();
        }
        else if (outcome == 1) {
            EGestures spot = cardManager.joustingCard.gesture;
            enemyCards.joustingCard.gameObject.SetActive(false);
            await Task.Delay(600);
            Destroy(cardManager.joustingCard);
            cardManager.AddCard(spot);
            enemyCards.joustingCard.gameObject.SetActive(true);
            enemyCards.ResetToStartingPositions();
            cardManager.cardHome.Show();
            cardManager.AddCard(enemy.GetRandomGesture(level));
        }
        else if (outcome == -1) {
            CardSpot spot = cardManager.joustingCard;
            cardManager.RemoveCard(spot);
            Destroy(spot);
            await Task.Delay(600);
            enemyCards.ResetToStartingPositions();
            cardManager.cardHome.Show();
        }
        level++;

        cardManager.cardHome.Layout();
        await Task.Delay(300);
        enemyCards.ShuffleCards();
        enemyCards.RandomizeGesture();

    }

    async Task BounceThreeTimes(RectTransform a, RectTransform b, float offset, float cycleDuration) {
        if (a == null || b == null) return;
        var aStart = a.localPosition;
        var bStart = b.localPosition;
        var upA = aStart + Vector3.up * offset;
        var upB = bStart + Vector3.up * offset;
        var seq = Sequence.Create();
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
        var tcs = new TaskCompletionSource<bool>();
        seq.OnComplete(() => tcs.TrySetResult(true));
        await tcs.Task;
    }

    async Task ClashTogether(RectTransform a, RectTransform b, float duration, Vector3 worldTarget) {
        if (a == null || b == null) return;
        var parentA = a.parent as Transform;
        var parentB = b.parent as Transform;
        if (parentA == null || parentB == null) return;
        var localA = parentA.InverseTransformPoint(worldTarget);
        var localB = parentB.InverseTransformPoint(worldTarget);
        var seq = Sequence.Create();
        float durA = duration * 0.5f;
        float durB = duration;
        seq.Insert(0f, Tween.LocalPosition(a, localA, durA, ease: Ease.InOutSine));
        seq.Insert(0f, Tween.LocalPosition(b, localB, durB, ease: Ease.InOutSine));
        var tcs = new TaskCompletionSource<bool>();
        seq.OnComplete(() => tcs.TrySetResult(true));
        await tcs.Task;
    }
}
