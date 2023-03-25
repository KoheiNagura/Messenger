using UnityEngine;
using UnityEngine.Networking;

public class SharingManager {
    private static readonly string
        gameUrl = "https://unityroom.com/games/tu_towerbattle";

    public static void Tweet(int totalPt, string imageUrl)
    {
        var url = "https://twitter.com/intent/tweet";
        var text = UnityWebRequest.EscapeURL($"スコア{totalPt}\n{gameUrl}");
        var hashTags = UnityWebRequest.EscapeURL($"unity1week");
        url = $"{url}/?text={text}&hashtags={hashTags}&url={UnityWebRequest.EscapeURL(imageUrl)}";
        Debug.Log(url);
    }

    public static void Note(int totalPt, string imageUrl)
    {
        var url = "https://misskey.io/share";
        var text = UnityWebRequest.EscapeURL($"スコア : {totalPt}\n{gameUrl}");
        url = $"{url}?text={text}&url={UnityWebRequest.EscapeURL(imageUrl)}";
        Debug.Log(url);
    }
}