using UnityEngine;
using UnityEngine.UI;

public class EnemyCardSlot : MonoBehaviour
{
    public RawImage image;
    public EGestures gesture;

    public void ChangeImage(EGestures gesture) {
        image.texture = CardManager.Instance.gestureSprites.Get(gesture).texture;
    }
}
