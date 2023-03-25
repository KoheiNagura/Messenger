using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StackedListCellComponent : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI usedCountLabel;
    [SerializeField] private Image fontImage;
    [SerializeField] private TextMeshProUGUI fontFamilyLabel;

    public void SetValue(int usedCount, Sprite sprite, string fontFamily)
    {
        usedCountLabel.text = usedCount > 0 ? $"{usedCount}" : "";
        fontImage.sprite = sprite;
        fontImage.enabled = sprite != null;
        fontFamilyLabel.text = $"{fontFamily}";
    }
}