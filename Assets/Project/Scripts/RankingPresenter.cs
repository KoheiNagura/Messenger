using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System.Linq;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class RankingPresenter : MonoBehaviour, IPresenter
{
    public bool isActivate { get; private set; }

    [SerializeField] private RankingManager ranking;
    [SerializeField] private RankingView view;
    [SerializeField] private ResultPresenter resultPresenter;

    private const int DUMMY_COUNT = 10;

    private bool isSent;
    private bool isHighScore;
    private List<RankingRecord> records;
    private string vailedCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789あいうえおかきくけこさしすせそたちつてとなにぬねのはひふへほまみむめもやゆよらりるれろわをんがぎぐげござじずぜぞだぢづでどばびぶべぼぱぴぷぺぽぁぃぅぇぉっゃゅょゎー ";
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
        view.OnEndEdit
            .Where(_ => isActivate)
            .Subscribe(_ => RemoveInvalidChar())
            .AddTo(gameObject);
        this.UpdateAsObservable()
            .Where(_ => isActivate && !isSent && isHighScore)
            .Subscribe(_ => view.SetAvailableSend(view.InputUserName.Length > 0))
            .AddTo(gameObject);
    }

    public async UniTask Open()
    {
        isSent = false;
        view.SetAvailableSend(false);
        await FetchRanking();
        SetPlayerInfo();
        await view.PlayTween();
        SetRankingCells();
        view.SetLaycastTarget(true);
        isActivate = true;
    }

    public async UniTask Close()
    {
        isActivate = false;
        records = null;
        view.SetLaycastTarget(false);
        await view.PlayTween(true);
        view.ResetRankingCells();
        view.SetScrollPosition(1);
        view.UpdateInputText("");
    }

    private async UniTask FetchRanking()
    {
        records = await ranking.Fetch();
        records = records.OrderByDescending(i => i.score).ToList();
    }

    private async void SetRankingCells()
    {
        for (var i = 0; i < records.Count; i++)
        {
            var record = records[i];
            var isOwn = record.userId == ranking.GetUserId();
            if (i < 3) 
            {
                var thumbneil = await ranking.GetScreenShot(record.userId);
                view.SetHigherRankingCell(i + 1, record.userName, record.stackedCount, record.score, isOwn, thumbneil);
            }
            else view.SetRankingCell(i + 1, record.userName, record.stackedCount, record.score, isOwn);
            await UniTask.Delay(System.TimeSpan.FromSeconds(.05f));
        }
        var dummyCount = DUMMY_COUNT - records.Count;
        if (dummyCount < 1) return;
        for (var i = 0; i < dummyCount; i++)
        {
            view.SetRankingCell(-1, "", -1, -1, false);
            await UniTask.Delay(System.TimeSpan.FromSeconds(.05f));
        }
    }

    private async void SendRanking()
    {
        var length = view.InputUserName.Length;
        RemoveInvalidChar();
        if (view.InputUserName.Length < 1) return;
        if (view.InputUserName.Length != length) return;
        isSent = true;
        view.SetAvailableSend(false);
        var record = ranking.GetRecord(view.InputUserName, gameResult.TotalPt, gameResult.Stacks.Count);
        await ranking.Save(record, gameResult.ScreenShot);
        view.SetScrollPosition(1);
        view.ResetRankingCells();
        SetRankingCells();
    }

    private async void BackToResult()
    {
        await Close();
        resultPresenter.SetActivate();
    }

    private void RemoveInvalidChar()
    {
        var text = new string(view.InputUserName.Select(i => FilterVailedChar(i)).ToArray());
        view.UpdateInputText(text);
    }

    private void SetPlayerInfo()
    {
        isHighScore = true;
        var rank = GetRank(gameResult.TotalPt);
        var userId = ranking.GetUserId();
        if (records.Select(i => i.userId).Contains(userId))
        {
            var oldRecord = records.First(i => i.userId == userId);
            isHighScore = oldRecord.score <= gameResult.TotalPt;
            if (!isHighScore) rank = -1;
        }
        view.SetPlayerInfo(rank, gameResult.Stacks.Count, gameResult.TotalPt);
    }

    private int GetRank(int score)
    {
        var rank = 1;
        foreach (var s in records.Select(i => score))
        {
            if (s <= score) break;
            rank++;
        }
        return rank;
    }

    private char FilterVailedChar(char c)
        => vailedCharacters.Contains(c) ? c : '\0';

    public void SetResult(GameResult result)
        => this.gameResult = result;
}