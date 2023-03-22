using UnityEngine;
using UniRx;
using UniRx.Triggers;
using InputAsRx.Triggers;
using System.Linq;

public class つController : MonoBehaviour 
{
    [SerializeField] private つ つPrefab;
    [SerializeField] private float rotationSpeed, moveSpeed;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private InGameView view;
    private つ currentつ, lastDroppedつ;
    private float axisX = 0;

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
        currentつ.transform.position = Vector3.up * 18;
        currentつ.SetPt(Random.Range(100, 300));
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
            .Where(_ => currentつ == null)
            .Subscribe(_ => Generaつ()).AddTo(lastDroppedつ);
        // テスト用
        lastDroppedつ.OnOutOfBounds
            .Where(_ => currentつ == null)
            .Subscribe(_ => Generaつ()).AddTo(lastDroppedつ);
    }

    private void Move(float x)
    {
        var movement = x * moveSpeed *Time.deltaTime;
        var posX = currentつ.transform.position.x + movement;
        posX = Mathf.Clamp(posX, -15, 15);
        currentつ.transform.position = new Vector3(posX, currentつ.transform.position.y, 0);
    }

    private Sprite GetRandomSpriつ()
        => sprites.OrderBy(_ => System.Guid.NewGuid()).First();
}
