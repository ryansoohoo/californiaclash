using NUnit.Framework;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class EnemyCards : MonoBehaviour
{
    public List<EnemyCardSlot> enemyCardSlots;

    public void Start() {  
        foreach (EnemyCardSlot slot in enemyCardSlots) {
            EGestures gesture = (EGestures)RandomNumberGenerator.GetInt32(0, 9);
        }
    }
}
