using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UniRx;
using Cysharp.Threading.Tasks;

public class ResultPresenter : MonoBehaviour, IPresenter
{
    public bool isActivate { get; private set; }
    [SerializeField] private ResultView view;
    [SerializeField] private InGamePresenter inGamePresenter;

    public void SetupView(GameResult result)
    {
        view.SetTotalPt(result.TotalPt);
        view.SetScreenShot(result.ScreenShot);
        var stacks = result.Stacks
            .GroupBy(i => i.FontFamily)
            .Select(i => (i.Count(), i.First().Sprite, i.Key))
            .OrderByDescending(i => i.Item1)
            .ToList();
        view.SetStackedList(stacks);
        view.OnClickBackground
            .Where(_ => isActivate)
            .Subscribe(_ => BackToInGame())
            .AddTo(gameObject);
    }

    public async UniTask Open()
    {
        isActivate = true;
        await view.PlayTween();
    }

    public async UniTask Close()
    {
        isActivate = false;
        await view.PlayTween(true);
    }

    public async void BackToInGame()
    {
        inGamePresenter.Initialize();
        await Close();
        await inGamePresenter.Open();
    }
}