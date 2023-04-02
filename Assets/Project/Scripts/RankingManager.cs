using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System;
using System.Linq;
using NCMB;
using NCMB.Tasks;

[CreateAssetMenu(fileName = "RankingManager", menuName = "Messenger/RankingManager")]
public class RankingManager : ScriptableObject
{
    private const int LIMIT = 10;
    private const string CLASS_NAME = "ranking";
    private const string USERID_KEY = "userId";

    [SerializeField] private string applicationKey, clientKey;

    [NonSerialized] private bool isInitalized = false;
    [NonSerialized] private CancellationTokenSource token;

    private void OnDisable() => token?.Cancel();

    private void Initialize()
    {
        token ??= new CancellationTokenSource();
        NCMBSettings.Initialize(applicationKey, clientKey, "", "");
        isInitalized = true;
    }

    public async UniTask<List<RankingRecord>> Fetch(int limit = LIMIT)
    {
        if (!isInitalized) Initialize();
        var query = new NCMBQuery<NCMBObject>(CLASS_NAME);
        query.Limit = LIMIT;
        var result = await query.FindTaskAsync();
        return result.Select(i => ObjectToRecord(i)).ToList();
    }

    public async UniTask Save(RankingRecord record)
    {
        if (!isInitalized) Initialize();
        if (PlayerPrefs.HasKey(USERID_KEY))
            await UpdateRecord(record);
        else await SaveRecord(record);
    }

    private async UniTask SaveRecord(RankingRecord record)
    {
        var ncmbObject = RecordToObject(record);
        await ncmbObject.SaveTaskAsync();
    }

    private async UniTask UpdateRecord(RankingRecord record)
    {
        var query = new NCMBQuery<NCMBObject>(CLASS_NAME);
        query.WhereEqualTo(nameof(RankingRecord.userId), GetUserId());
        var result = await query.FindTaskAsync();
        if (result.Count < 1) 
        {
            await SaveRecord(record);
            return;
        }
        var ncmbObject = result[0];
        ncmbObject = UpdateRecord(ncmbObject, record);
        await ncmbObject.SaveTaskAsync();
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
        return UpdateRecord(obj, record);
    }

    private NCMBObject UpdateRecord(NCMBObject obj, RankingRecord record)
    {
        obj[nameof(RankingRecord.userId)] = record.userId;
        obj[nameof(RankingRecord.userName)] = record.userName;
        obj[nameof(RankingRecord.score)] = record.score;
        obj[nameof(RankingRecord.stackedCount)] = record.stackedCount;
        obj[nameof(RankingRecord.screenShot)] = TextureToBase64(record.screenShot);
        return obj;
    }

    private RankingRecord ObjectToRecord(NCMBObject obj)
    {
        var record = new RankingRecord();
        record.userId = Convert.ToString(obj[nameof(RankingRecord.userId)]);
        record.userName = Convert.ToString(obj[nameof(RankingRecord.userName)]);
        record.score = Convert.ToInt32(obj[nameof(RankingRecord.score)]);
        record.stackedCount = Convert.ToInt32(obj[nameof(RankingRecord.stackedCount)]);
        var base64 = Convert.ToString(obj[nameof(RankingRecord.screenShot)]);
        record.screenShot = Base64ToTexture(base64);
        return record;
    }

    private string GetUserId()
    {
        var userId = PlayerPrefs.GetString(USERID_KEY);
        if (!string.IsNullOrEmpty(userId)) return userId;
        userId = Guid.NewGuid().ToString("N");
        PlayerPrefs.SetString(USERID_KEY, userId);
        return userId;
    }
}

public class RankingRecord
{
    public static readonly Vector2Int TextureSize = new Vector2Int(420, 594);
    public string userId;
    public string userName;
    public int score;
    public int stackedCount;
    // 基本的にbase64で保管して必要なときだけtextureにしたい
    public Texture2D screenShot;
}