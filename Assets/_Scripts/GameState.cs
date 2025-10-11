using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public List<Gestures> gestures;
    public CardManager cardManager;

    public void Start()
    {
        AddCard(Gestures.Rock);
        AddCard(Gestures.Paper);
        AddCard(Gestures.Scissors);
    }

    public void AddCard(Gestures gesture)
    {
        gestures.Add(gesture);
        cardManager.AddCard(gesture);
    }
}


//know what hand player has played
//outcome of the round 
//know which 'level' the player is on - based on which enemy prefab is active
//access to list of gestures per 'level'
//knows the total number of hands the player has currently, is hands == 0 game over
//public gameobject of end screen ui
