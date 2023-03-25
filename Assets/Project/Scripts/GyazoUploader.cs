using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine.Networking;

[CreateAssetMenu(fileName = "GyazoUploader", menuName = "Messenger/GyazoUploader")]
public class GyazoUploader : ScriptableObject
{
    public bool isProcessing { get; private set; }
    [SerializeField] private string accessToken;
    private readonly string uploadUrl = "https://upload.gyazo.com/api/upload";

    [Serializable]
    private class Responce
    {
        public string image_id;
        public string permalink_url;
        public string thumb_url;
        public string url;
        public string type;
    }

    public async UniTask<string> UploadTexture(Texture2D texture)
    {
        isProcessing = true;
        var form = new WWWForm();
        form.AddField("access_token", accessToken);
        form.AddBinaryData("imagedata", texture.EncodeToPNG(), "screenshot.png", "image/png");
        using var request = UnityWebRequest.Post(uploadUrl, form);
        await request.SendWebRequest();
        var response = JsonUtility.FromJson<Responce>(request.downloadHandler.text);
        isProcessing = false;
        return response.permalink_url ?? "";
    }
}