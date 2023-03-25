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
    }

    public async UniTask Open()
    {
        isActivate = true;
        // UIアニメーション待機
        GameStart();
    }

    public async UniTask Close()
    {
        isActivate = false;
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
        var sprite = model.GetRandomSprite();
        var pt = model.GetFontSize();
        controller.Generaつ(sprite, position, pt);
        view.SetFontNameLabel(sprite.name);
    }

    private async void OnStopped(つ stopped)
    {
        (lastScreenShot, _) = await UniTask.WhenAll(
            ScreenRecorder.GetTexture(Camera.main),
            camera.MoveCameraIfNeeded().AsAsyncUnitUniTask());
        model.AddStacked(stopped.sprite, stopped.pt);
        Generaつ();
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
        else if (controller.Currentつ == null) Generaつ();
    }
}