using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UniRx;

public class ResultPresenter : MonoBehaviour
{
    [SerializeField] private ResultView view;

    private void Awake()
    {
        var stacks = new List<StackedつData>() {
            new StackedつData(null, "テスト1", 5),
            new StackedつData(null, "テスト1", 5),
            new StackedつData(null, "テスト1", 5),
            new StackedつData(null, "テスト2", 5),
            new StackedつData(null, "テスト2", 5),
            new StackedつData(null, "テスト3", 5),
        };
        var result = new GameResult(stacks, null);
        SetupView(result);
    }

    private void SetupView(GameResult result)
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
            .Subscribe(_ => view.PlayTween(true))
            .AddTo(gameObject);
    }
}