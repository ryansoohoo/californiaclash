using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyCardSlot : MonoBehaviour
{
    public RawImage image;
    public RawImage hiddenImage;
    public EGestures gesture;
    public TextMeshProUGUI leftText;
    public TextMeshProUGUI rightText;
    public void ChangeImage(EGestures gesture) {
        image.texture = CardManager.Instance.gestureSprites.Get(gesture).texture;
        leftText.text = gesture.ToString();
        rightText.text = gesture.ToString();
    }

    public void Hide() {
        image.enabled=false;
        hiddenImage.enabled = true;
        leftText.enabled = false;
        rightText.enabled = false;
    }

    public void UnHide() {
        image.enabled = true;
        hiddenImage.enabled = false;
        leftText.enabled = true;
        rightText.enabled = true;
    }
}
