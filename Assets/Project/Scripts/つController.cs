using UnityEngine;
using UniRx;
using System;
using UniRx.Triggers;
using Cysharp.Threading.Tasks;
using InputAsRx.Triggers;
using System.Linq;
using DG.Tweening;

public class つController : MonoBehaviour 
{
    public つ Currentつ { get; private set; }
    public IObservable<つ> OnStopped => onStoppedSubject;
    public IObservable<つ> OnOutOfBounds => onOutOfBoundsSubject;
    public ReadOnlyReactiveProperty<bool> IsInteracting
        => isInteractingProperty.ToReadOnlyReactiveProperty();

    [SerializeField] private つ つPrefab;
    [SerializeField] private float rotationSpeed, moveSpeed;
    [SerializeField] private float moveRange;

    private つ lastDroppedつ;
    private float axisX = 0;
    private Transform parent;
    private Subject<つ> onStoppedSubject, onOutOfBoundsSubject;
    private ReactiveProperty<bool> isInteractingProperty;

    private void Awake()
    {
        SubscribeObservables();
        parent = new GameObject("つParent").transform;
    }

    public void Reset()
    {
        Currentつ = null;
        lastDroppedつ = null;
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }
    }

    private void SubscribeObservables()
    {
        this.OnKeyAsObservable(KeyCode.Space)
            .Where(_ => Currentつ != null)
            .Subscribe(_ => Rotaつ())
            .AddTo(gameObject);
        this.OnKeyUpAsObservable(KeyCode.Space)
            .Where(_ => Currentつ != null)
            .Subscribe(_ => Drop())
            .AddTo(gameObject);
        this.OnAxisAsObservable("Horizontal")
            .Where(_ => Currentつ != null)
            .Subscribe(x => axisX = x)
            .AddTo(gameObject);
        this.UpdateAsObservable()
            .Where(_ => Currentつ != null && axisX != 0)
            .Subscribe(_ => Move(axisX))
            .AddTo(gameObject);

        onStoppedSubject = new Subject<つ>();
        onOutOfBoundsSubject = new Subject<つ>();
        isInteractingProperty = new ReactiveProperty<bool>();
    }

    public void Generaつ(Sprite sprite, Vector2 position, int pt)
    {
        Currentつ = Instantiate(つPrefab);
        Currentつ.transform.SetParent(parent);
        Currentつ.transform.position = position + Vector2.up * 10;
        Currentつ.transform.DOMoveY(position.y, .2f);
        Currentつ.SetPt(pt);
        Currentつ.SetSprite(sprite);
    }

    private void Rotaつ()
    {
        if (!isInteractingProperty.Value)
        {
            isInteractingProperty.Value = true;
        }
        Currentつ?.transform
            .Rotate(0, 0, rotationSpeed * Time.deltaTime * (Mathf.Abs(axisX) + 1));
    }

    private void Drop()
    {
        Currentつ.Drop();
        Currentつ.OnStopped
            .Subscribe(i => DroppedEvent(onStoppedSubject, i))
            .AddTo(Currentつ);
        Currentつ.OnOutOfBounds
            .Subscribe(i => DroppedEvent(onOutOfBoundsSubject, i))
            .AddTo(Currentつ);
        lastDroppedつ = Currentつ;
        Currentつ = null;
    }

    private void DroppedEvent(Subject<つ> subject, つ dropped)
    {
        if (lastDroppedつ == dropped)
        {
            isInteractingProperty.Value = false;
        }
        subject.OnNext(dropped);
    }

    private void Move(float x)
    {
        isInteractingProperty.Value = true;
        var movement = x * moveSpeed *Time.deltaTime;
        var posX = Currentつ.transform.position.x + movement;
        posX = Mathf.Clamp(posX, -moveRange, moveRange);
        Currentつ.transform.position = new Vector3(posX, Currentつ.transform.position.y, 0);
    }
}
