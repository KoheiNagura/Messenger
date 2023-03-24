using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.Rendering;

public class ScreenRecorder
{
    public static async UniTask<Texture2D> GetTexture(Camera camera, bool hideUI = true)
    {
        var included = camera.LayerCullingIncludes("UI");
        if (hideUI)
        {
            camera.LayerCullingToggle("UI", false);
        }
        var render = GetRenderTexture(camera);
        var result = new Texture2D(render.width, render.height, TextureFormat.RGBA32, false);
        var request = await AsyncGPUReadback.Request(render);
        var buffer = request.GetData<Color32>();
        result.LoadRawTextureData(buffer);
        result.Apply();
        if (included != camera.LayerCullingIncludes("UI"))
        {
            camera.LayerCullingToggle("UI", included);
        }
        return result;
    }

    private static RenderTexture GetRenderTexture(Camera camera)
    {
        var size = new Vector2(Screen.width, Screen.height);
        var render = RenderTexture.GetTemporary((int)size.x, (int)size.y, 0, RenderTextureFormat.ARGB32);
        var beforeTarget = camera.targetTexture;
        camera.targetTexture = render;
        camera.Render();
        camera.targetTexture = beforeTarget;
        return render;
    }
}