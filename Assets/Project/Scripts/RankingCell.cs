using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;
using System;
using DG.Tweening;

public class RankingCell : MonoBehaviour
{
    public bool isInteractable
    {
        get
        {
            if (fadeInTween == null) return false;
            return fadeInTween.IsPlaying();
        }
    }

    public IObservable<Unit> OnClickThumbneil
        => thumbneilButton?.onClick.AsObservable(); 

    [SerializeField] private RectTransform wrapper;
    [SerializeField] private Button thumbneilButton;
    [SerializeField] private RawImage thumbneil;
    [SerializeField] private TextMeshProUGUI
        rankLabel,
        nameLabel,
        stackedCountLabel,
        scoreLabel;

    private static Color hilightColor = new Color(0.8156863f, 0.1176471f, 0.4156863f); // D01E6A
    private Tween fadeInTween;

    public void PlayTween()
        => fadeInTween ??= wrapper.DOAnchorPos(Vector2.zero, .25f).SetAutoKill(false);

    public void SetValue(int rank, string name, int stackedCount, int score)
    {
        rankLabel.text = rank > -1 ? $"{rank}" : "";
        nameLabel.text = $"{name}";
        stackedCountLabel.text = stackedCount > -1 ? $"{stackedCount}つ" : "";
        scoreLabel.text = score > -1 ? $"{score:#,0}pt" : "";
    }

    public void SetThumbneil(Texture2D texture)
    {
        if (thumbneil == null) return;
        thumbneil.texture = texture;
    }

    public void HighlightName()
        => nameLabel.color = hilightColor;

}