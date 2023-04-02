using UnityEngine;
using Cysharp.Threading.Tasks;

public class RankingPresenter : MonoBehaviour, IPresenter
{
    public bool isActivate { get; private set; }

    [SerializeField] private RankingManager ranking;
    [SerializeField] private RankingView view;

    private Texture2D screenShot;

    private async void Awake()
    {
        var texture = await ScreenRecorder.GetTexture(Camera.main);
        var record = new RankingRecord() {
            userName = "こうしんしたしたあかうんと",
            score = 99999,
            stackedCount = 999,
            screenShot = texture
        };
        await ranking.Save(record);
    }

    public async UniTask Open()
    {

    }

    public async UniTask Close()
    {

    }

    public void SetScreenShot(Texture2D screenShot)
        => this.screenShot = screenShot;
}