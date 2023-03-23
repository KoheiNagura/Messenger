using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using InputAsRx.Triggers;
using System.Linq;

public class つController : MonoBehaviour 
{
    [SerializeField] private つ つPrefab;
    [SerializeField] private float rotationSpeed, moveSpeed;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private InGameView view;
    [SerializeField] private RandomFontSizeTable randomTable;
    private つ currentつ, lastDroppedつ;
    private float axisX = 0;
    private float cameraDistanceY = 15;

    private void Awake()
    {
        Generaつ();
        SubscribeObservables();
    }

    private void SubscribeObservables()
    {
        this.OnKeyAsObservable(KeyCode.Space)
            .Where(_ => currentつ != null)
            .Subscribe(_ => Rotaつ())
            .AddTo(gameObject);
        this.OnKeyUpAsObservable(KeyCode.Space)
            .Where(_ => currentつ != null)
            .Subscribe(_ => Drop())
            .AddTo(gameObject);
        this.OnAxisAsObservable("Horizontal")
            .Where(_ => currentつ != null)
            .Subscribe(x => axisX = x)
            .AddTo(gameObject);
        this.UpdateAsObservable()
            .Where(_ => currentつ != null && axisX != 0)
            .Subscribe(_ => Move(axisX))
            .AddTo(gameObject);
    }

    private void Generaつ()
    {
        currentつ = Instantiate(つPrefab);
        currentつ.transform.position = Vector3.up * (GetScreenTopPosition().y - 4);
        currentつ.SetPt(randomTable.GetValue());
        var sprite = GetRandomSpriつ();
        view.SetFontNameLabel(sprite.name);
        currentつ.SetSprite(sprite);
    }

    private void Rotaつ()
        => currentつ?.transform
            .Rotate(0, 0, rotationSpeed * Time.deltaTime * (Mathf.Abs(axisX) + 1));

    private void Drop()
    {
        currentつ.Drop();
        lastDroppedつ = currentつ;
        currentつ = null;
        lastDroppedつ.OnStopped
            .Subscribe(_ => OnStopped()).AddTo(lastDroppedつ);
        lastDroppedつ.OnOutOfBounds
            .Subscribe(_ => OnOutOfBounds()).AddTo(lastDroppedつ);
    }

    private void Move(float x)
    {
        var movement = x * moveSpeed *Time.deltaTime;
        var posX = currentつ.transform.position.x + movement;
        posX = Mathf.Clamp(posX, -15, 15);
        currentつ.transform.position = new Vector3(posX, currentつ.transform.position.y, 0);
    }

    private Vector3 GetScreenTopPosition()
    {
        var cameraZ = Camera.main.transform.position.z;
        var topPos = new Vector3(Screen.width / 2, Screen.height, cameraZ);
        return Camera.main.ScreenToWorldPoint(topPos);
    }

    private Vector3 GetHighestPosition()
    {
        var screenTop = GetScreenTopPosition().y + 10;
        Debug.Log(screenTop);
        var origin = new Vector2(0, screenTop);
        var size = new Vector2(30, 10);
        var hit = Physics2D.BoxCast(origin, size, 0, Vector2.down, 100f);
        if (!hit) return Vector3.zero;
        return hit.point;
    }

    private async UniTask MoveCameraIfNeeded()
    {
        var screenTop = GetScreenTopPosition().y;
        var highestPosition = GetHighestPosition().y;
        var distance = screenTop - highestPosition;
        if (distance > cameraDistanceY) return;
        var camera = Camera.main.transform;
        var endValue = camera.transform.position.y + (cameraDistanceY - distance);
        await camera.DOMoveY(endValue, .3f);
    }

    private Sprite GetRandomSpriつ()
        => sprites.OrderBy(_ => System.Guid.NewGuid()).First();

    private async void OnStopped()
    {
        if (currentつ != null) return;
        await MoveCameraIfNeeded();
        Generaつ();
    }

    private void OnOutOfBounds()
    {
        // テスト用に仮置き
        if (currentつ != null) return;
        Generaつ();
    }
}
