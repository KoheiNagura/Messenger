using UnityEngine;
using System.Linq;
using System;
using UniRx;
using Cysharp.Threading.Tasks;

public class InGamePresenter : MonoBehaviour
{
    [SerializeField] private InGameView view;
    [SerializeField] private InGameModel model;
    [SerializeField] private CameraController camera;
    [SerializeField] private つController controller;

    private Texture2D lastScreenShot;
    private int lifeCount = 3;

    private async void Start()
    {
        SubscribeObservables();
        // UIアニメーション待機
        GameStart();
    }

    private void SubscribeObservables()
    {
        controller.OnStopped
            .Subscribe(OnStopped)
            .AddTo(gameObject);
        controller.OnOutOfBounds
            .Subscribe(OnOutOfBounds)
            .AddTo(gameObject);
    }

    private async UniTask OpenTweenAnimation()
    {

    }

    private void GameStart()
    {
        Generaつ();
    }

    private void GameOver()
    {

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
        Generaつ();
    }

    private void OnOutOfBounds(つ dropped)
    {
        lifeCount--;
        view.SetLifeLabel(lifeCount);
        if (lifeCount < 1)
        {
            GameOver();
        }
        // テスト用
        if (controller.Currentつ != null) return;
        Generaつ();
    }
}