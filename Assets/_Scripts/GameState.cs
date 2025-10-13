using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameState : MonoBehaviour {
    public EGameState gameState;

    public List<EGestures> gestures;
    public CardManager cardManager;
    public Button submitButton;
    public CardHome home;
    public int stage = 0;
    public void Start() {
        AddCard(EGestures.Rock);
        AddCard(EGestures.Paper);
        AddCard(EGestures.Scissors);
    }

    public void AddCard(EGestures gesture) {
        gestures.Add(gesture);
        cardManager.AddCard(gesture);
    }

    public void Update() {
        
    }

    public void SubmitSelectedCard() {
        if (cardManager.selectedCard == null)
            return;

    }
}


//know what hand player has played
//outcome of the round 
//know which 'level' the player is on - based on which enemy prefab is active
//access to list of gestures per 'level'
//knows the total number of hands the player has currently, is hands == 0 game over
//public gameobject of end screen ui
