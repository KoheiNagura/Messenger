using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class TitleView : MonoBehaviour {
    public IObservable<Unit> OnClickBackgroud => background.OnClickAsObservable();
    [SerializeField] private Button background;
    [SerializeField] private RectTransform taeruRect, つRect;
    private Sequence taeruSequence, つSequence, fadeOutSequence;
    private Vector2 defaultTaeruPosition, defaultつPotition;
    private CanvasGroup rectGroup;
    
    private void Awake()
    {
        defaultTaeruPosition = taeruRect.anchoredPosition;
        defaultつPotition = つRect.anchoredPosition;
    }

    public void Reset()
    {
        taeruRect.anchoredPosition = defaultTaeruPosition;
        つRect.anchoredPosition = defaultつPotition;
    }

    public void SetEnabled(bool enable)
    {
        rectGroup ??= taeruRect.GetComponent<CanvasGroup>();
        rectGroup.alpha = enable ? 1 : 0;
        background.gameObject.SetActive(enable);
    }

    public void PlayTaeruTween()
    {
        taeruSequence ??= DOTween.Sequence()
            .Append(taeruRect.DOShakeAnchorPos(1f, 12f, 20, fadeOut: false))
            .SetLoops(-1)
            .SetAutoKill(false);
        taeruSequence.Restart();
    }

    public void PauseTaeruTween()
    {
        if (taeruSequence == null) return;
        taeruSequence.Pause();
    }

    public async UniTask PlayFadeOutLabelTween()
    {
        rectGroup ??= taeruRect.GetComponent<CanvasGroup>();
        fadeOutSequence ??= DOTween.Sequence()
            .Append(rectGroup.DOFade(0, .2f));
        fadeOutSequence.PlayForward();
        await UniTask.WaitUntil(() => !fadeOutSequence.IsPlaying());
    }

    public async UniTask Playつween()
    {
        つSequence ??= DOTween.Sequence()
            .Append(つRect.DOLocalRotate(new Vector3(0, 0, -90), .2f))
            .AppendInterval(.3f)
            .Append(つRect.DOAnchorPosY(-140, .5f))
            .SetAutoKill(false);
        つSequence.PlayForward();
        await UniTask.WaitUntil(() => !つSequence.IsPlaying());
    }
}