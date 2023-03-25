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
    [SerializeField] private GyazoUploader uploader;
    private GameResult result;
    private string uploadedUrl;

    private void Start()
    {
        SubscribeObservables();
    }
    
    public async void SetupView(GameResult result)
    {
        this.result = result;
        view.SetTotalPt(result.TotalPt);
        view.SetScreenShot(result.ScreenShot);
        var stacks = result.Stacks
            .GroupBy(i => i.FontFamily)
            .Select(i => (i.Count(), i.First().Sprite, i.Key))
            .OrderByDescending(i => i.Item1)
            .ToList();
        view.SetStackedList(stacks);
    }

    private void SubscribeObservables()
    {
        view.OnClickBackground
            .Where(_ => isActivate)
            .Subscribe(_ => BackToInGame())
            .AddTo(gameObject);
        view.OnClickShare
            .Where(_ => isActivate)
            .Subscribe(_ => view.ToggleOpenShare())
            .AddTo(gameObject);
        view.OnClickShare
            .Where(_ => isActivate)
            .Where(_ => !view.isAvailableShare && !uploader.isProcessing)
            .Subscribe(_ => UploadScreenShot())
            .AddTo(gameObject);
        view.OnClickTweet
            .Where(_ => isActivate && view.isAvailableShare)
            .Subscribe(_ => SharingManager.Tweet(result.TotalPt, uploadedUrl))
            .AddTo(gameObject);
        view.OnClickMisskey
            .Where(_ => isActivate && view.isAvailableShare)
            .Subscribe(_ => SharingManager.Note(result.TotalPt, uploadedUrl))
            .AddTo(gameObject);
    }

    public async UniTask Open()
    {
        isActivate = true;
        view.SetShareAvilable(false);
        await view.PlayTween();
        view.SetLaycastTarget(true);
    }

    public async UniTask Close()
    {
        isActivate = false;
        view.SetLaycastTarget(false);
        view.CloseShare();
        view.SetShareAvilable(false);
        await view.PlayTween(true);
    }

    private async void UploadScreenShot()
    {
        uploadedUrl = await uploader.UploadTexture(result.ScreenShot);
        if (uploadedUrl == "") return;
        view.SetShareAvilable(true);
    }

    private async void BackToInGame()
    {
        inGamePresenter.Initialize();
        await Close();
        await inGamePresenter.Open();
    }
}