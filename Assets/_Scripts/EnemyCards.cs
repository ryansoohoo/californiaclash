using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using PrimeTween;

public class EnemyCards : MonoBehaviour {
    public Enemy enemy;
    public GameState gameState;
    public List<EnemyCardSlot> enemyCardSlots;
    public RectTransform joustLocation;
    public EnemyCardSlot joustingCard;
    [SerializeField] float shuffleRandomizeDuration = 1f;
    [SerializeField] int shuffleSteps = 8;
    [SerializeField] float shuffleReturnDuration = 0.2f;
    [SerializeField] float joustTweenDuration = 0.35f;
    List<Vector3> startingPositions;
    Vector3[] tmpTargets;
    int[] tmpPerm;
    Sequence shuffleSeq;
    Sequence joustSeq;
    EnemyCardSlot joustLockedSlot;
    static readonly EGestures[] FirstStageSet = { EGestures.Rock, EGestures.Paper, EGestures.FYou };

    void Awake() {
        CacheStartingPositions();
        EnsureTemps();
    }

    void OnDisable() {
        if (shuffleSeq.isAlive) shuffleSeq.Stop();
        if (joustSeq.isAlive) joustSeq.Stop();
        joustLockedSlot = null;
    }

    public void StartJoust() {
        ShuffleCardsToRPF();
    }

    public EnemyCardSlot PickRandomCard() {
        int n = enemyCardSlots != null ? enemyCardSlots.Count : 0;
        if (n == 0) return null;
        int valid = 0;
        for (int i = 0; i < n; i++) if (enemyCardSlots[i] != null) valid++;
        if (valid == 0) return null;
        int pick = RandomNumberGenerator.GetInt32(valid);
        for (int i = 0; i < n; i++) {
            var s = enemyCardSlots[i];
            if (s == null) continue;
            if (pick == 0) return s;
            pick--;
        }
        return null;
    }

    void EnsureTemps() {
        int n = enemyCardSlots != null ? enemyCardSlots.Count : 0;
        if (n <= 0) { tmpTargets = null; tmpPerm = null; return; }
        if (tmpTargets == null || tmpTargets.Length != n) tmpTargets = new Vector3[n];
        if (tmpPerm == null || tmpPerm.Length != n) tmpPerm = new int[n];
    }

    public void CacheStartingPositions() {
        int n = enemyCardSlots != null ? enemyCardSlots.Count : 0;
        if (startingPositions == null) startingPositions = new List<Vector3>(n);
        else { startingPositions.Clear(); if (startingPositions.Capacity < n) startingPositions.Capacity = n; }
        for (int i = 0; i < n; i++) {
            var slot = enemyCardSlots[i];
            var t = slot != null ? slot.transform : null;
            startingPositions.Add(t != null ? t.localPosition : Vector3.zero);
        }
    }

    public void ResetToStartingPositions() {
        ShowCards();
        int n = startingPositions != null ? startingPositions.Count : 0;
        for (int i = 0; i < n; i++) {
            var slot = enemyCardSlots[i];
            var t = slot != null ? slot.transform : null;
            if (t != null) t.localPosition = startingPositions[i];
        }
    }

    public IReadOnlyList<Vector3> GetStartingPositions() {
        return startingPositions;
    }

    public void Start() {
        EnsureTemps();
        ShuffleCardsToRPF();
        RandomizeGesture();
    }

    public void ShuffleCards() {
        EnsureTemps();
        RunShuffleAnimation(shuffleRandomizeDuration, shuffleSteps, shuffleReturnDuration, false);
    }

    void ShuffleCardsToRPF() {
        EnsureTemps();
        RunShuffleAnimation(shuffleRandomizeDuration, shuffleSteps, shuffleReturnDuration, true);
    }

    void RunShuffleAnimation(float randomizeDuration, int steps, float returnDuration, bool finalizeToRPF) {
        if (enemyCardSlots == null || enemyCardSlots.Count == 0) return;
        if (steps < 1) steps = 1;
        float stepDur = randomizeDuration / steps;
        if (shuffleSeq.isAlive) shuffleSeq.Stop();
        shuffleSeq = Sequence.Create();
        int n = enemyCardSlots.Count;
        for (int s = 0; s < steps; s++) {
            for (int i = 0; i < n; i++) tmpPerm[i] = i;
            for (int i = n - 1; i > 0; i--) {
                int j = RandomNumberGenerator.GetInt32(i + 1);
                int t = tmpPerm[i]; tmpPerm[i] = tmpPerm[j]; tmpPerm[j] = t;
            }
            for (int i = 0; i < n; i++) tmpTargets[i] = startingPositions[tmpPerm[i]];
            for (int i = 0; i < n; i++) {
                var slot = enemyCardSlots[i];
                var t = slot != null ? slot.transform : null;
                if (t == null) continue;
                shuffleSeq.Insert(s * stepDur, Tween.LocalPosition(t, tmpTargets[i], stepDur, ease: Ease.Linear));
            }
        }
        for (int i = 0; i < n; i++) {
            var slot = enemyCardSlots[i];
            var t = slot != null ? slot.transform : null;
            if (t == null) continue;
            shuffleSeq.Insert(randomizeDuration, Tween.LocalPosition(t, startingPositions[i], returnDuration, ease: Ease.InOutSine));
        }
    }

    Vector2 GetAnchoredTarget(RectTransform card, RectTransform target) {
        var parentRect = card != null ? card.parent as RectTransform : null;
        if (card == null || target == null || parentRect == null) return Vector2.zero;
        var canvas = card.GetComponentInParent<Canvas>();
        var root = canvas != null ? canvas.rootCanvas : null;
        Camera cam = null;
        if (root != null && root.renderMode != RenderMode.ScreenSpaceOverlay) cam = root.worldCamera;
        var screen = RectTransformUtility.WorldToScreenPoint(cam, target.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, screen, cam, out var local);
        return local;
    }

    public void LerpRandomEnemyCardToJoust() {
        if (joustSeq.isAlive) joustSeq.Stop();
        var pick = PickRandomCard();
        if (pick == null) return;
        LerpToJoust(pick);
    }

    public void LerpToJoust(EnemyCardSlot slot) {
        if (slot == null) return;
        joustLockedSlot = slot;
        var rt = slot.transform as RectTransform;
        if (rt == null || joustLocation == null || rt.parent == null) return;
        var targetAnchored = GetAnchoredTarget(rt, joustLocation);
        if (joustSeq.isAlive) joustSeq.Stop();
        joustSeq = Sequence.Create();
        joustSeq.Insert(0f, Tween.LocalPosition(rt, targetAnchored, joustTweenDuration, ease: Ease.InOutSine));
        joustingCard = slot;
    }

    public void HideCards() {
        foreach (EnemyCardSlot card in enemyCardSlots) card.Hide();
    }

    public void ShowCards() {
        foreach (EnemyCardSlot card in enemyCardSlots) card.UnHide();
    }

    public void EndJoust() {
        joustingCard = null;
    }

    public void RandomizeGesture() {
        int n = enemyCardSlots != null ? enemyCardSlots.Count : 0;
        if (n == 0) return;
        if (gameState != null && gameState.level <= 0) {
            for (int i = 0; i < n; i++) {
                var slot = enemyCardSlots[i];
                if (slot == null) continue;
                slot.ChangeImage(FirstStageSet[i % FirstStageSet.Length]);
            }
            return;
        }
        for (int i = 0; i < n; i++) {
            var slot = enemyCardSlots[i];
            if (slot == null) continue;
            slot.ChangeImage(enemy.GetRandomGesture(gameState != null ? gameState.level : 0));
        }
    }
}