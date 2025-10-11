using UnityEngine;

[CreateAssetMenu(fileName = "GestureSprites", menuName = "Gestures/Sprite Set")]
public sealed class GestureSprites : ScriptableObject {
    [SerializeField] private Sprite[] sprites = new Sprite[9];

    public Sprite Get(Gestures gesture) {
        int i = (int)gesture;
        if (i < 0 || i >= sprites.Length) return null;
        return sprites[i];
    }

    public void Set(Gestures gesture, Sprite sprite) {
        int i = (int)gesture;
        if (i < 0) return;
        if (sprites == null || sprites.Length != 9) sprites = new Sprite[9];
        sprites[i] = sprite;
    }
}