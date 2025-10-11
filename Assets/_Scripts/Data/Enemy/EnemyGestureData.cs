using UnityEngine;

public struct EnemyGestureData
{
    public Gestures gesture;
    public float pickRate01; // from initial pool based on stage
    public float dropRate01; // after winning, p(picking this item) 

    public EnemyGestureData(Gestures gesture, float a, float b) {
        this.gesture = gesture;
        this.dropRate01 = a;
        this.pickRate01 = b;
    }
}
