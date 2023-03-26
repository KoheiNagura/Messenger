using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine.Networking;

[CreateAssetMenu(fileName = "GyazoUploader", menuName = "Messenger/GyazoUploader")]
public class GyazoUploader : ScriptableObject, IImageUploader
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

    public async UniTask<(string result, string error)> UploadTexture(Texture2D texture)
    {
        if (string.IsNullOrEmpty(accessToken))
        {
            return ("", "");
        }
        isProcessing = true;
        var form = new WWWForm();
        form.AddField("access_token", accessToken);
        form.AddBinaryData("imagedata", texture.EncodeToPNG(), "screenshot.png", "image/png");
        using var request = UnityWebRequest.Post(uploadUrl, form);
        request.SetRequestHeader("Access-Control-Allow-Origin", "*");
        await request.SendWebRequest();
        if (request.responseCode != 200)
        {
            return ("", request.error ?? "");
        }
        var response = JsonUtility.FromJson<Responce>(request.downloadHandler.text);
        isProcessing = false;
        return (response.permalink_url ?? "", request.error ?? "");
    }
}