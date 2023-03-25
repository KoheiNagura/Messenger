using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using UniRx;
using System;

public class ResultView : MonoBehaviour
{
    public bool isInteractable 
    { 
        get 
        {
            if (fadeInSequence == null) return false;
            return !fadeInSequence.IsPlaying();
        }
    }

    public IObservable<Unit> OnClickBackground
        => background.OnClickAsObservable().Where(_ => isInteractable);

    private Sequence fadeInSequence;
    [SerializeField] private Button background;
    [SerializeField] private RectTransform wrapper;
    [SerializeField] private ShareButtonComponent shareButton;
    [SerializeField] private Text totalPtLabel;
    [SerializeField] private RawImage screenShotImage;
    [SerializeField] private StackedListComponent stackedList;

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
                .Append(wrapper.DOAnchorPos(Vector2.zero, .5f).SetEase(Ease.OutBack))
                .Join(DOTween.ToAlpha(
                    () => background.targetGraphic.color,
                    c => background.targetGraphic.color = c,
                    .45f,
                    .5f
                ))
                .SetAutoKill(false);
        }
        fadeInSequence.timeScale = 1f;
        fadeInSequence.PlayForward();
        await UniTask.WaitUntil(() => !fadeInSequence.IsPlaying());
    }

    public void SetTotalPt(int totalPt)
        => totalPtLabel.text = $"{totalPt:#,0}pt";

    public void SetScreenShot(Texture2D texture)
        => screenShotImage.texture = texture;

    public void SetStackedList(IList<(int usedCount, Sprite sprite, string fontFamily)> stacks)
        => stackedList.SetValue(stacks);
}