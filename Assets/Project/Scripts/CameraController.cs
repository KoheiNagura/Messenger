using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class CameraController : MonoBehaviour
{
    private readonly float cameraDistanceY = 15;
    private float defaultCameraPosY;

    private void Awake()
    {
        var position = Camera.main.transform.position;
        defaultCameraPosY = position.y;
    }

    public void ResetPosition()
    {
        var position = Camera.main.transform.position;
        position.y = defaultCameraPosY;
        Camera.main.transform.position = position;
    }

    public async UniTask MoveCameraIfNeeded()
    {
        var screenTop = GetScreenTopPosition().y;
        var highestPosition = GetHighestPosition().y;
        var distance = screenTop - highestPosition;
        if (distance > cameraDistanceY) return;
        var camera = Camera.main.transform;
        var endValue = camera.transform.position.y + (cameraDistanceY - distance);
        await camera.DOMoveY(endValue, .3f);
    }

    public Vector3 GetScreenTopPosition()
    {
        var cameraZ = Camera.main.transform.position.z;
        var topPos = new Vector3(Screen.width / 2, Screen.height, cameraZ);
        return Camera.main.ScreenToWorldPoint(topPos);
    }

    public Vector3 GetHighestPosition()
    {
        var screenTop = GetScreenTopPosition().y + 10;
        var origin = new Vector2(0, screenTop);
        var size = new Vector2(30, 10);
        var hit = Physics2D.BoxCast(origin, size, 0, Vector2.down, 100f);
        if (!hit) return Vector3.zero;
        return hit.point;
    }
}