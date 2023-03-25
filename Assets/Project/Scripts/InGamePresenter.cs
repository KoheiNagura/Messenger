using UnityEngine;
using System.Linq;
using System;
using UniRx;
using Cysharp.Threading.Tasks;

public class InGamePresenter : MonoBehaviour, IPresenter
{
    public bool isActivate { get; private set; }

    [SerializeField] private InGameView view;
    [SerializeField] private ResultPresenter resultPresenter;
    [SerializeField] private InGameModel model;
    [SerializeField] private CameraController camera;
    [SerializeField] private つController controller;

    private Texture2D lastScreenShot;
    private int lifeCount, maxLifeCount = 3;
    private (Sprite sprite, int pt) currentつData;

    private async void Start()
    {
        SubscribeObservables();
        Initialize();
        await Open();
    }

    public void Initialize()
    {
        lifeCount = maxLifeCount;
        model.Initialzie();
        controller.Reset();
        camera.ResetPosition();
        view.SetLifeLabel(lifeCount);
        view.SetFontNameLabel("");
        currentつData = model.GetNextつ();
        view.SetNexつValue(model.NextつData.sprite, model.NextつData.pt);
    }

    public async UniTask Open()
    {
        await UniTask.WhenAll(view.PlayTween(), view.PlayNexつTween());
        GameStart();
        isActivate = true;
    }

    public async UniTask Close()
    {
        isActivate = false;
        await UniTask.WhenAll(view.PlayTween(true), view.PlayNexつTween(true));
    }

    private void SubscribeObservables()
    {
        controller.OnStopped
            .Where(_ => isActivate)
            .Subscribe(OnStopped)
            .AddTo(gameObject);
        controller.OnOutOfBounds
            .Where(_ => isActivate)
            .Subscribe(OnOutOfBounds)
            .AddTo(gameObject);
        controller.IsInteracting
            .Where(_ => isActivate)
            .Subscribe(i => view.PlayNexつTween(i).AsAsyncUnitUniTask())
            .AddTo(gameObject);
        view.OnClickNexつ
            .Subscribe(_ => print("clk"))
            .AddTo(gameObject);
        view.OnClickNexつ
            .Where(_ => isActivate && !controller.IsInteracting.Value)
            .Subscribe(_ => Swapつ())
            .AddTo(gameObject);
    }

    private void GameStart()
    {
        Generaつ();
    }

    private async void GameOver()
    {
        var result = model.GetResult(lastScreenShot);
        resultPresenter.SetupView(result);
        await Close();
        await resultPresenter.Open();
    }

    private void Generaつ()
    {
        var screenTop = camera.GetScreenTopPosition();
        var position = Vector3.up * (screenTop.y - 4);
        controller.Generaつ(currentつData.sprite, position, currentつData.pt);
    }

    private async void Swapつ()
    {
        Destroy(controller.Currentつ.gameObject);
        currentつData = model.SwapNextつ(currentつData.sprite, currentつData.pt);
        var screenTop = camera.GetScreenTopPosition();
        var position = Vector3.up * (screenTop.y - 4);
        await view.PlayNexつTween(true);
        controller.Generaつ(currentつData.sprite, position, currentつData.pt);
        view.SetFontNameLabel(currentつData.sprite.name);
        view.SetNexつValue(model.NextつData.sprite, model.NextつData.pt);
        await view.PlayNexつTween();
    }

    private void UpdateView()
    {
        currentつData = model.GetNextつ();
        view.SetFontNameLabel(currentつData.sprite.name);
        view.SetNexつValue(model.NextつData.sprite, model.NextつData.pt);
    }

    private async void OnStopped(つ stopped)
    {
        (lastScreenShot, _) = await UniTask.WhenAll(
            ScreenRecorder.GetTexture(Camera.main),
            camera.MoveCameraIfNeeded().AsAsyncUnitUniTask());
        model.AddStacked(stopped.sprite, stopped.pt);
        if (controller.Currentつ == null)
        {
            UpdateView();
            Generaつ();
        }
    }

    private void OnOutOfBounds(つ dropped)
    {
        lifeCount--;
        view.SetLifeLabel(lifeCount);
        model.RemoveStacked(dropped.sprite, dropped.pt);
        if (lifeCount < 1)
        {
            GameOver();
        } 
        else if (controller.Currentつ == null) 
        {
            UpdateView();
            Generaつ();
        }
    }
}