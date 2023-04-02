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
    [SerializeField] private ResultPresenter resultPresenter;

    private GameResult gameResult;

    private void Start()
    {
        SubscribeObservables();
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
        view.SetLaycastTarget(true);
        isActivate = true;
    }

    public async UniTask Close()
    {
        isActivate = false;
        view.SetLaycastTarget(false);
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
        var dummyCount = 8 - result.Count;
        if (dummyCount < 1) return;
        for (var i = 0; i < dummyCount; i++)
        {
            view.SetRankingCell(-1, "", -1, -1, false);
        }
    }

    private async void SendRanking()
    {
        view.SetAvailableSend(false);
        var record = ranking.GetRecord(view.InputUserName, gameResult.TotalPt, gameResult.Stacks.Count, gameResult.ScreenShot);
        await ranking.Save(record);
        // viewの更新と挿入
    }

    private async void BackToResult()
    {
        await Close();
        resultPresenter.SetActivate();
    }

    public void SetResult(GameResult result)
        => this.gameResult = result;
}