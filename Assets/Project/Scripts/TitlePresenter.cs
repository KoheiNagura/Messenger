using UnityEngine;
using System;
using UniRx;
using Cysharp.Threading.Tasks;

public class TitlePresenter : MonoBehaviour, IPresenter {
    public bool isActivate { get; private set; }
    [SerializeField] private TitleView view;
    [SerializeField] private InGamePresenter inGamePresenter;

    private async void Start()
    {
        SubscribeObservables();
        await Open();
    }

    private void SubscribeObservables()
    {
        view.OnClickBackgroud
            .Where(_ => isActivate)
            .Subscribe(_ => MoveToInGame())
            .AddTo(gameObject);
    }

    public async UniTask Open()
    {
        isActivate = true;
        view.Reset();
        view.SetEnabled(true);
        view.PlayTaeruTween();
    }

    public async UniTask Close()
    {
        isActivate = false;
        view.PauseTaeruTween();
        await UniTask.WhenAll(view.PlayFadeOutLabelTween(), view.Playつween());
        view.SetEnabled(false);
    }

    private async void MoveToInGame()
    {
        inGamePresenter.Initialize();
        await Close();
        await inGamePresenter.Open();
    }
}