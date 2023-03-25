using UnityEngine;
using UnityEngine.UI;

public class StackedListCellComponent : MonoBehaviour
{
    [SerializeField] private Text usedCountLabel;
    [SerializeField] private Image fontImage;
    [SerializeField] private Text fontFamilyLabel;

    public void SetValue(int usedCount, Sprite sprite, string fontFamily)
    {
        usedCountLabel.text = usedCount > 0 ? $"{usedCount}" : "";
        fontImage.sprite = sprite;
        fontFamilyLabel.text = $"{fontFamily}";
    }
}