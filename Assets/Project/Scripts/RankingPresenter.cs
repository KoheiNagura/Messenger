using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System.Linq;
using Cysharp.Threading.Tasks;

public class RankingPresenter : MonoBehaviour, IPresenter
{
    public bool isActivate { get; private set; }

    [SerializeField] private RankingManager ranking;
    [SerializeField] private RankingView view;

    private GameResult result;

    private void Start()
    {
        SubscribeObservables();
        Open();
    }

    private void SubscribeObservables()
    {
        view.OnClickBackground
            .Where(_ => isActivate)
            .Subscribe(_ => BackToResult())
            .AddTo(gameObject);
        view.OnClickSendRanking
            .Where(_ => isActivate)
            .Subscribe(_ => SendRanking())
            .AddTo(gameObject);
        this.UpdateAsObservable()
            .Where(_ => isActivate)
            .Subscribe(_ => view.SetAvailableSend(view.InputUserName.Length > 0))
            .AddTo(gameObject);
    }

    public async UniTask Open()
    {
        // await view.PlayLoadingAnimation();
        view.SetAvailableSend(false);
        await SetRankingCells();
        await view.PlayTween();
    }

    public async UniTask Close()
    {
        await view.PlayTween(true);
        view.ResetRankingCells();
    }

    private async UniTask SetRankingCells()
    {
        var result = await ranking.Fetch();
        result = result.OrderByDescending(i => i.score).ToList();
        for (var i = 0; i < result.Count; i++)
        {
            var record = result[i];
            var isOwn = record.userId == ranking.GetUserId(); 
            if (i < 3) view.SetHigherRankingCell(i + 1, record.userName, record.stackedCount, record.score, isOwn, record.screenShot);
            else view.SetRankingCell(i + 1, record.userName, record.stackedCount, record.score, isOwn);
        }
    }

    private async void SendRanking()
    {
        var record = ranking.GetRecord(view.InputUserName, result.TotalPt, result.Stacks.Count, result.ScreenShot);
        await ranking.Save(record);
        // viewの更新と挿入
    }

    private async void BackToResult()
    {
        await Close();
    }

    public void SetResult(GameResult result)
        => this.result = result;
}