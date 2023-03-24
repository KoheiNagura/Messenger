using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System.Collections.Generic;
using System;
using System.Linq;

[RequireComponent(typeof(SpriteRenderer), typeof(PolygonCollider2D), typeof(Rigidbody2D))]
public class つ : MonoBehaviour {
    public int pt { get; private set; }
    public Sprite sprite => renderer.sprite;
    public IObservable<Collision2D> OnHitOtherつ;
    public IObservable<つ> OnOutOfBounds;
    public IObservable<つ> OnStopped;

    private PolygonCollider2D collider;
    private SpriteRenderer renderer;
    private Rigidbody2D rigidbody;
    private bool isDroping;
    private List<Vector2> velocities;
    private IDisposable velocityObservable;

    private void Awake()
    {
        collider = GetComponent<PolygonCollider2D>();
        renderer = GetComponent<SpriteRenderer>();
        rigidbody = GetComponent<Rigidbody2D>();
        velocities = new List<Vector2>();
        InitializeObservables();
        OnStopped.Subscribe(_ => velocityObservable?.Dispose()).AddTo(gameObject);
        OnHitOtherつ.FirstOrDefault().Subscribe(_ => isDroping = false).AddTo(gameObject);
        // 仮置き ObjectPoolにしてもいいかも
        OnOutOfBounds.Subscribe(_ => Destroy(gameObject)).AddTo(gameObject);
    }

    private void InitializeObservables()
    {
        OnHitOtherつ = this.OnCollisionEnter2DAsObservable()
            .Where(i => i.collider.tag == "つ");
        OnOutOfBounds = this.UpdateAsObservable()
            .Select(_ => this)
            .Where(_ => IsMoving() && IsOutOfBounds())
            .Take(1);
        OnStopped = this.UpdateAsObservable()
            .Select(_ => this)
            .Where(_ => !isDroping && IsStopping())
            .Take(1);
        velocityObservable = Observable.Interval(TimeSpan.FromSeconds(.1f))
            .Where(_ => !isDroping && rigidbody.simulated)
            .Subscribe(_ => 
            {
                velocities.Add(rigidbody.velocity);
                if (velocities.Count > 10) velocities.RemoveAt(0);
            })
            .AddTo(gameObject);
    }

    public void Drop()
    {
        isDroping = true;
        rigidbody.simulated = true;
    }

    public void SetPt(int pt)
    {
        this.pt = pt;
        var size = new Vector3(pt, pt, 0) / 100 + new Vector3(0, 0, 1);
        transform.localScale = size;
    }

    public void SetSprite(Sprite sprite) 
    {
        renderer.sprite = sprite;
        SetPhysicsShape();
    }

    private void SetPhysicsShape()
    {
        var sprite = renderer.sprite;
        var shapeCount = sprite.GetPhysicsShapeCount();
        collider.pathCount = shapeCount;
        var shape = new List<Vector2>();
        for (var i = 0; i < shapeCount; i++)
        {
            shape.Clear();
            sprite.GetPhysicsShape(i, shape);
            collider.SetPath(i, shape.ToArray());
        }
    }

    private bool IsOutOfBounds()
    {
        // 画面の下端から-1した位置とかから範囲外にしたい。
        return transform.position.y < -25;
    }

    private bool IsMoving()
        => rigidbody.velocity.magnitude > .1f;

    private bool IsStopping()
    {
        if (isDroping) return false;
        if (velocities.Count < 5) return false;
        if (velocities.Select(i => i.magnitude).Average() < 0.05f)
            return true;
        var average = velocities.Select(i => i.magnitude).Average();
        var averageDeviation = velocities
            .Select(i => Mathf.Abs(i.magnitude - average))
            .Average();
        return (averageDeviation < 0.03f);
    }
}