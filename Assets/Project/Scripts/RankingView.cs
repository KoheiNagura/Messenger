using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using UniRx;
using Cysharp.Threading.Tasks;

public class RankingView : MonoBehaviour
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

    public void SetLaycastTarget(bool isOn)
        => background.targetGraphic.raycastTarget = isOn;
}