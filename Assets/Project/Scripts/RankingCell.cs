using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;
using System;

public class RankingCell : MonoBehaviour
{
    public IObservable<Unit> OnClickThumbneil
        => thumbneilButton?.onClick.AsObservable(); 

    [SerializeField] private Button thumbneilButton;
    [SerializeField] private RawImage thumbneil;
    [SerializeField] private TextMeshProUGUI
        rankLabel,
        nameLabel,
        stackedCountLabel,
        scoreLabel;

    private static Color hilightColor = new Color(0.8156863f, 0.1176471f, 0.4156863f); // D01E6A

    public void SetValue(int rank, string name, int stackedCount, int score)
    {
        rankLabel.text = $"{rank}";
        nameLabel.text = $"{name}";
        stackedCountLabel.text = $"{stackedCount}つ";
        scoreLabel.text = $"{score:#,0}pt";
    }

    public void SetThumbneil(Texture2D texture)
    {
        if (thumbneil == null) return;
        thumbneil.texture = texture;
    }

    public void HighlightName()
        => nameLabel.color = hilightColor;

}