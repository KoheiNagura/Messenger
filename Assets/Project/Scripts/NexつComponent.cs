using UnityEngine;
using UnityEngine.UI;

public class NexつComponent : MonoBehaviour {
    [SerializeField] private Image つImage;
    [SerializeField] private Text ptLabel;

    public void SetValue(Sprite sprite, int pt)
    {
        つImage.sprite = sprite;
        ptLabel.text = $"{pt} pt";
    }
}