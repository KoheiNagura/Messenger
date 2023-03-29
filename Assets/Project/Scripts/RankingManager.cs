using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System;
using System.Linq;
using NCMB;

[CreateAssetMenu(fileName = "RankingManager", menuName = "Messenger/RankingController")]
public class RankingManager : ScriptableObject
{
    private const int LIMIT = 30;
    private const string CLASS_NAME = "ranking";
    [SerializeField] private string applicationKey, clientKey;

    private CancellationTokenSource token;

    private void OnEnable() 
    {
        token ??= new CancellationTokenSource();
        NCMBSettings.Initialize(applicationKey, clientKey, "", "");
    }

    private void OnDisable() => token?.Cancel();

    public async UniTask<List<RankingRecord>> Fetch(int limit = LIMIT)
    {
        var query = new NCMBQuery<NCMBObject>(CLASS_NAME);
        query.Limit = LIMIT;
        var result = default(List<NCMBObject>);
        query.FindAsync((list, e) =>
        {
            if (e != null)
            {
                Debug.LogWarning($"{e.ErrorCode} : {e.ErrorMessage}");
                return;
            }
            result = list;
        });
        await UniTask.WaitUntil(() => result != null, cancellationToken: token.Token);
        return result.Select(i => ObjectToRecord(i)).ToList();
    }

    public async void Save(RankingRecord record)
    {
        var ncmbObject = RecordToObject(record);
        var result = default(NCMBException);
        ncmbObject.SaveAsync(e => result = e);
        await UniTask.WaitUntil(() => result != null, cancellationToken: token.Token);
    }

    private Texture2D Base64ToTexture(string base64)
    {
        var size = RankingRecord.TextureSize;
        var texture = new Texture2D(size.x, size.y, TextureFormat.RGBA32, false);
        texture.LoadImage(Convert.FromBase64String(base64));
        return texture;
    }

    private string TextureToBase64(Texture2D texture)
    {
        var size = RankingRecord.TextureSize;
        if (texture.width != size.x || texture.height != size.y)
        {
            texture = ResizeTexture(texture, RankingRecord.TextureSize);
        }
        var encode = texture.EncodeToJPG();
        return Convert.ToBase64String(encode);
    }

    private Texture2D ResizeTexture(Texture2D source, Vector2Int size)
    {
        var result = new Texture2D(size.x, size.y, TextureFormat.RGBA32, false);
        Graphics.ConvertTexture(source, result);
        return result;
    }

    private NCMBObject RecordToObject(RankingRecord record)
    {
        var obj = new NCMBObject(CLASS_NAME);
        obj[nameof(record.userName)] = record.userName;
        obj[nameof(record.score)] = record.score;
        obj[nameof(record.stackedCount)] = record.stackedCount;
        obj[nameof(record.screenShot)] = TextureToBase64(record.screenShot);
        return obj;
    }

    private RankingRecord ObjectToRecord(NCMBObject obj)
    {
        var record = new RankingRecord();
        record.userName = Convert.ToString(obj[nameof(record.userName)]);
        record.score = Convert.ToInt32(obj[nameof(record.score)]);
        record.stackedCount = Convert.ToInt32(obj[nameof(record.stackedCount)]);
        var base64 = Convert.ToString(obj[nameof(record.screenShot)]);
        record.screenShot = Base64ToTexture(base64);
        return record;
    }
}

public class RankingRecord
{
    public static readonly Vector2Int TextureSize = new Vector2Int(420, 594);
    public string userName;
    public int score;
    public int stackedCount;
    // 基本的にbase64で保管して必要なときだけtextureにしたい
    public Texture2D screenShot;
}