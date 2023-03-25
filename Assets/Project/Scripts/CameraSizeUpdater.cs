using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera)), ExecuteAlways]
public class CameraSizeUpdater : MonoBehaviour
{
    [SerializeField] private float baseHeight;
    [SerializeField] private int pixelPerUnit;
    private Camera camera;
    private float currentHeight;

    private void Awake()
        => UpdateCameraSize();

    private void Update()
    {
        if (Application.isPlaying) return;
        UpdateCameraSize();
    }

    private void UpdateCameraSize()
    {
        if (Screen.height == currentHeight) return;
        if (camera == null) camera = GetComponent<Camera>();
        var orthographicSize = baseHeight / pixelPerUnit / 2 * (512 / 100);
        currentHeight = Screen.height;
        camera.orthographicSize = orthographicSize * (currentHeight / baseHeight);
    }
}
