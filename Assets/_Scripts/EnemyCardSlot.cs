using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyCardSlot : MonoBehaviour
{
    public RawImage image;
    public EGestures gesture;
    public TextMeshProUGUI leftText;
    public TextMeshProUGUI rightText;
    public void ChangeImage(EGestures gesture) {
        image.texture = CardManager.Instance.gestureSprites.Get(gesture).texture;
        leftText.text = gesture.ToString();
        rightText.text = gesture.ToString();
    }
}
