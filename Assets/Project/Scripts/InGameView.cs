using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using UniRx;
using TMPro;

public class InGameView : MonoBehaviour
{
    public IObservable<Unit> OnClickNexつ => nexつ.onClick.AsObservable();
    [SerializeField] private TextMeshProUGUI fontNameLabel, lifeLabel, totalPtLabel;
    [SerializeField] private NexつComponent nexつ;
    private Sequence fadeInSequence, labelFadeSequence;

    public async UniTask PlayTween(bool playBackwards = false)
    {
        if (playBackwards)
        {
            if (fadeInSequence == null) return;
            fadeInSequence.timeScale = 2f;
            fadeInSequence.PlayBackwards();
            await UniTask.WaitUntil(() => !fadeInSequence.IsPlaying());
            return;
        }
        if (fadeInSequence == null)
        {
            fadeInSequence = DOTween.Sequence()
                .Append(lifeLabel.rectTransform.DOAnchorPos(Vector2.zero, .5f))
                .Append(fontNameLabel.rectTransform.DOAnchorPos(Vector2.zero, .5f))
                .SetAutoKill(false);
        }
        fadeInSequence.timeScale = 1f;
        fadeInSequence.PlayForward();
        await UniTask.WaitUntil(() => !fadeInSequence.IsPlaying());
    }

    private void PlayLabelFadeTween(TextMeshProUGUI label)
    {
        var color = label.color;
        color.a = 0.2f;
        label.color = color;
        DOTween.ToAlpha(
            () => label.color,
            c => label.color = c,
            1,
            .3f
        );
    }

    public void SetFontNameLabel(string name)
    {
        fontNameLabel.text = name;
        PlayLabelFadeTween(fontNameLabel);
    }

    public void SetNexつValue(Sprite sprite, int pt)
        => nexつ.SetValue(sprite, pt);

    public void SetLifeLabel(int life)
    {
        lifeLabel.text = $"いのち\n{life}つ";
        PlayLabelFadeTween(lifeLabel);
    }

    public void SetTotalPt(int pt)
        => totalPtLabel.text = $"{pt:#,0}pt";

    public async UniTask PlayNexつTween(bool playBackwards = false)
        => await nexつ.PlayTween(playBackwards);
}
