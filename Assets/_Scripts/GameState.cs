using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public List<Gestures> gestures;
    public CardManager cardManager;

    public void Start() {
        AddCard(Gestures.Rock);
        AddCard(Gestures.Paper);
        AddCard(Gestures.Scissors);
    }

    public void AddCard(Gestures gesture) {
        gestures.Add(gesture);
        cardManager.AddCard(gesture);
    }
}
