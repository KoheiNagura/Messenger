using UnityEngine;
using Cysharp.Threading.Tasks;

public interface IImageUploader
{
    public bool isProcessing { get; }
    public UniTask<(string result, string error)> UploadTexture(Texture2D texture);
}