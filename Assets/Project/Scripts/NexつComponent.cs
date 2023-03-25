using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;
using DG.Tweening;

public class NexつComponent : MonoBehaviour {
    public UnityEvent onClick => button.onClick;
    public bool isAnimating
    {
        get
        {
            if (fadeInSequence == null) return false;
            return fadeInSequence.IsPlaying();
        }
    }

    [SerializeField] private Button button;
    [SerializeField] private Image つImage;
    [SerializeField] private Text ptLabel;
    private RectTransform rect => (RectTransform)this.transform;
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
                .Append(rect.DOAnchorPos(Vector2.zero, .5f).SetEase(Ease.OutExpo))
                .SetAutoKill(false);
        }
        fadeInSequence.timeScale = 1f;
        fadeInSequence.PlayForward();
        await UniTask.WaitUntil(() => !fadeInSequence.IsPlaying());
    }

    public void SetValue(Sprite sprite, int pt)
    {
        つImage.sprite = sprite;
        ptLabel.text = $"{pt} pt";
    }
}