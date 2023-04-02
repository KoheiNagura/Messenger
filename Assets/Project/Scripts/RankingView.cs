using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using UniRx;
using Cysharp.Threading.Tasks;
using TMPro;

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

    public string InputUserName => userNameInput.text;

    public IObservable<Unit> OnClickBackground
        => background.OnClickAsObservable().Where(_ => isInteractable);
    public IObservable<Unit> OnClickSendRanking
        => sendButton.onClick.AsObservable().Where(_ => isInteractable);
    public IObservable<Unit> OnEndEdit
        => userNameInput.onEndEdit.AsObservable().Where(_ => isInteractable).Select(_ => Unit.Default);

    [SerializeField] private RankingCell rankingCellPrefab, higherRankingCellPrefab;
    [SerializeField] private Button background;
    [SerializeField] private RectTransform wrapper;
    [SerializeField] private ScrollRect scroll;
    [SerializeField] private Button sendButton;
    [SerializeField] private TMP_InputField userNameInput;
    [SerializeField] private TextMeshProUGUI stackedCountLabel, scoreLabel;

    private Sequence fadeInSequence;

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

    public void SetRankingCell(int rank, string name, int stackedCount, int score, bool isHighlight)
    {
        var cell = Instantiate(rankingCellPrefab);
        cell.transform.SetParent(scroll.content, false);
        cell.SetValue(rank, name, stackedCount, score);
        if (isHighlight) cell.HighlightName();
    }

    public void SetHigherRankingCell(int rank, string name, int stackedCount, int score, bool isHighlight, Texture2D thumbneil)
    {
        var cell = Instantiate(higherRankingCellPrefab);
        cell.transform.SetParent(scroll.content, false);
        cell.SetValue(rank, name, stackedCount, score);
        if (isHighlight) cell.HighlightName();
        cell.SetThumbneil(thumbneil);
    }

    public void ResetRankingCells()
    {
        foreach(Transform child in scroll.content)
        {
            Destroy(child.gameObject);
        }
    }

    public void SetAvailableSend(bool isAvailable)
        => sendButton.interactable = isAvailable;

    public void UpdateInputText(string text)
        => userNameInput.text = text;

    public void SetScrollPosition(float y)
    {
        var pos = Vector2.zero;
        pos.y = y;
        scroll.normalizedPosition = pos;
    }
}