using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
public class ResultView : MonoBehaviour
{
    [SerializeField] private ShareButtonComponent shareButton;
    [SerializeField] private Text totalPtLabel;
    [SerializeField] private RawImage screenShotImage;
    [SerializeField] private StackedListComponent stackedList;

    public void SetTotalPt(int totalPt)
        => totalPtLabel.text = $"{totalPt:#,0}pt";

    public void SetScreenShot(Texture2D texture)
        => screenShotImage.texture = texture;

    public void SetStackedList(IList<(int usedCount, Sprite sprite, string fontFamily)> stacks)
        => stackedList.SetValue(stacks);
}