using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Events;
using DG.Tweening;

public class ShareButtonComponent : MonoBehaviour
{
    public bool isOpen { get; private set; }
    public bool isAvailable { get; private set; }
    public UnityEvent onClickShare => shareButton.onClick;
    public UnityEvent onClickTweet => tweetButton.onClick;
    public UnityEvent onClickMisskey => misskeyButton.onClick;

    [SerializeField] private Button shareButton, tweetButton, misskeyButton;
    [SerializeField] private CanvasGroup buttonsGroup;
    private Sequence fadeInSequence;

    public void ToggleOpen()
    {
        if (!isOpen) Open();
        else Close();
    }

    public void Open()
    {
        if (isOpen) return;
        isOpen = true;
        PlayTween();
    }

    public void Close()
    {
        if (!isOpen) return;
        isOpen = false;
        PlayTween(true);
    }

    public void SetAvailable(bool value)
    {
        isAvailable = value;
        if (isAvailable)
        {
            PlayAvailableAnimation();
            return;
        }
        buttonsGroup.alpha = 0.4f;
    }

    private void PlayAvailableAnimation()
    {
        buttonsGroup.DOFade(1, .2f);
    }

    private void PlayTween(bool playBackwards = false)
    {
        if (playBackwards)
        {
            if (fadeInSequence == null) return;
            fadeInSequence.timeScale = 2f;
            fadeInSequence.PlayBackwards();
            return;
        }
        if (fadeInSequence == null)
        {
            var rect = (RectTransform)buttonsGroup.transform;
            fadeInSequence = DOTween.Sequence()
                .Append(rect.DOAnchorPos(Vector2.zero, .3f).SetEase(Ease.OutBack))
                .SetAutoKill(false);
        }
        fadeInSequence.timeScale = 1f;
        fadeInSequence.PlayForward();
    }
}