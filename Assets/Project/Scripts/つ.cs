using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System.Collections.Generic;
using System;
using System.Linq;

[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D), typeof(Rigidbody2D))]
public class つ : MonoBehaviour {
    public int pt { get; private set; }
    public IObservable<Collision2D> OnHitOtherつ;
    public IObservable<Unit> OnOutOfBounds;
    public IObservable<Unit> OnStopped;

    private SpriteRenderer renderer;
    private Rigidbody2D rigidbody;
    private bool isDroping;
    private List<Vector2> velocities;
    private IDisposable velocityObservable;

    private void Awake()
    {
        renderer = GetComponent<SpriteRenderer>();
        rigidbody = GetComponent<Rigidbody2D>();
        velocities = new List<Vector2>();

        OnHitOtherつ = this.OnCollisionEnter2DAsObservable()
            .Where(i => i.collider.tag == "つ");
        OnOutOfBounds = this.UpdateAsObservable()
            .First(_ => IsMoving() && IsOutOfBounds());
        OnStopped = this.UpdateAsObservable()
            .First(_ => !isDroping && IsStopping());
        velocityObservable = Observable.Interval(TimeSpan.FromSeconds(.1f))
            .Where(_ => !isDroping)
            .Subscribe(_ => 
            {
                velocities.Add(rigidbody.velocity);
                if (velocities.Count > 10) velocities.RemoveAt(0);
            });

        OnStopped.Subscribe(_ => velocityObservable?.Dispose());
        OnHitOtherつ.First().Subscribe(_ => isDroping = false);
    }

    private void OnDestroy()
    {
        velocityObservable?.Dispose();
    }

    public void Drop()
    {
        isDroping = true;
        rigidbody.simulated = true;
    }

    public void SetPt(int pt)
    {
        this.pt = pt;
        // とりあえず仮置き
        var size = new Vector3(pt, pt, 0) / 100;
        transform.localScale = new Vector3(0.15f, 0.15f, 0) + size;
    }

    public void SetSprite(Sprite sprite) 
        => renderer.sprite = sprite;

    private bool IsOutOfBounds()
    {
        // 画面の下端から-1した位置とかから範囲外にしたい。
        return transform.position.y < -5;
    }

    private bool IsMoving()
        => rigidbody.velocity.magnitude > .1f;

    private bool IsStopping()
    {
        if (isDroping) return false;
        if (velocities.Count < 1) return false;
        return (velocities.Select(i => i.magnitude).Average() > 0.01f);
    }
}