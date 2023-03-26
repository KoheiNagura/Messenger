using UnityEngine;
using UnityEngine.Networking;
using System.Runtime.InteropServices;

public class SharingManager {
    private static readonly string
        gameUrl = "https://unityroom.com/games/tu_towerbattle";

    private static string GetText(int つcount, int totalPt)
        => $"『つ』をつみあげて {つcount}つ の『つ』がたえましたことを伝えます。\nすこあ : {totalPt:#,0}\n\n";

    public static void Tweet(int つcount,int totalPt, string imageUrl)
    {
        var shareUrl = "https://twitter.com/intent/tweet";
        var text = UnityWebRequest.EscapeURL(GetText(つcount, totalPt));
        var hashTags = UnityWebRequest.EscapeURL($"つ,unity1week,unityroom");
        var url = string.IsNullOrEmpty(imageUrl) ? gameUrl : imageUrl;
        text += !string.IsNullOrEmpty(imageUrl) ? UnityWebRequest.EscapeURL(gameUrl) : "";
        shareUrl = $"{shareUrl}/?text={text}&hashtags={hashTags}&url={url+"\n"}";
        Debug.Log(shareUrl);
        OpenNewTab(shareUrl);
    }

    public static void Note(int つcount, int totalPt, string imageUrl)
    {
        var shareUrl = "https://misskey.io/share";
        var text = UnityWebRequest.EscapeURL(GetText(つcount, totalPt));
        text += UnityWebRequest.EscapeURL("#つ #unity1week #unityroom");
        var url = string.IsNullOrEmpty(imageUrl) ? gameUrl : imageUrl;
        text += !string.IsNullOrEmpty(imageUrl) ? UnityWebRequest.EscapeURL(gameUrl) : "";
        shareUrl = $"{shareUrl}?text={text}&url={url+"\n"}";
        Debug.Log(shareUrl);
        OpenNewTab(shareUrl);
    }

#if !UNITY_EDITOR && UNITY_WEBGL
    [DllImport("__Internal")]
    private static extern void OpenNewTab(string url);
#else
    private static void OpenNewTab(string url) => Application.OpenURL(url);
#endif
}