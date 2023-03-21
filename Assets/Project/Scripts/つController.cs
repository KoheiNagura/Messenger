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
    private つ currentつ, lastDroppedつ;

    private void Awake()
    {
        Generaつ();
        this.OnKeyAsObservable(KeyCode.Space)
            .Where(_ => currentつ != null)
            .Subscribe(_ => currentつ?.transform.Rotate(0, 0, rotationSpeed * Time.deltaTime));
        this.OnKeyDownAsObservable(KeyCode.Return)
            .Where(_ => currentつ != null)
            .Subscribe(_ => Drop());
        this.OnAxisAsObservable("Horizontal")
            .Where(_ => currentつ != null)
            .Subscribe(x => Move(x));
    }

    private void Generaつ()
    {
        currentつ = Instantiate(つPrefab);
        currentつ.transform.position = Vector3.up * 4;
        currentつ.SetPt(Random.Range(10, 50));
        currentつ.SetSprite(GetRandomSpriつ());
    }

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
        posX = Mathf.Clamp(posX, -4, 4);
        currentつ.transform.position = new Vector3(posX, currentつ.transform.position.y, 0);
    }

    private Sprite GetRandomSpriつ()
        => sprites.OrderBy(_ => System.Guid.NewGuid()).First();
}
