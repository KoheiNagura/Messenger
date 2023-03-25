using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;

public class InGameView : MonoBehaviour
{
    [SerializeField] private Text fontNameLabel, lifeLabel;
    [SerializeField] private NexつComponent nexつ;
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
                .Append(lifeLabel.rectTransform.DOAnchorPos(Vector2.zero, .5f))
                .Append(fontNameLabel.rectTransform.DOAnchorPos(Vector2.zero, .5f))
                .SetAutoKill(false);
        }
        fadeInSequence.timeScale = 1f;
        fadeInSequence.PlayForward();
        await UniTask.WaitUntil(() => !fadeInSequence.IsPlaying());
    }

    public void SetFontNameLabel(string name)
        => fontNameLabel.text = name;

    public void SetNexつValue(Sprite sprite, int pt)
        => nexつ.SetValue(sprite, pt);

    public void SetLifeLabel(int life)
        => lifeLabel.text = $"いのち\n{life}つ";

    public async UniTask PlayNexつTween(bool playBackwards = false)
        => await nexつ.PlayTween(playBackwards);
}
