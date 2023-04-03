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
    public const int LIMIT = 30;
    private const string CLASS_NAME_RANKING = "ranking";
    private const string CLASS_NAME_SCREENSHOT = "screenshot";
    private const string KEY_USERID = "userId";

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
        var query = new NCMBQuery<NCMBObject>(CLASS_NAME_RANKING);
        query.Limit = limit;
        var result = await query.FindTaskAsync();
        return result.Select(i => ObjectToRecord(i)).ToList();
    }

    public async UniTask Save(RankingRecord record, Texture2D texture)
    {
        if (!isInitalized) Initialize();
        if (PlayerPrefs.HasKey(KEY_USERID))
            await UpdateRecord(record);
        else await SaveRecord(record);
        await SaveScreenShot(texture);
    }

    private async UniTask SaveScreenShot(Texture2D texture)
    {
        if (!isInitalized) Initialize();
        var record = new ScreenShotRecord()
        {
            userId = GetUserId(),
            base64 = TextureToBase64(texture)
        };
        if (PlayerPrefs.HasKey(KEY_USERID))
            await UpdateScreenShotRecord(record);
        else await SaveScreenShotRecord(record);
    }

    public async UniTask<Texture2D> GetScreenShot(string userId)
    {
        if (!isInitalized) Initialize();
        var query = new NCMBQuery<NCMBObject>(CLASS_NAME_SCREENSHOT);
        query.WhereEqualTo(nameof(RankingRecord.userId), GetUserId());
        query.Limit = 1;
        var result = await query.FindTaskAsync();
        if (result.Count < 1) return null;
        var ncmbObject = result[0];
        var base64 = Convert.ToString(ncmbObject[nameof(ScreenShotRecord.base64)]);
        return Base64ToTexture(base64);
    }

    private async UniTask SaveRecord(RankingRecord record)
    {
        var ncmbObject = RecordToObject(record);
        await ncmbObject.SaveTaskAsync();
    }

    private async UniTask UpdateRecord(RankingRecord record)
    {
        var query = new NCMBQuery<NCMBObject>(CLASS_NAME_RANKING);
        query.WhereEqualTo(nameof(RankingRecord.userId), GetUserId());
        query.Limit = 1;
        var result = await query.FindTaskAsync();
        if (result.Count < 1) 
        {
            await SaveRecord(record);
            return;
        }
        var ncmbObject = result[0];
        var oldRecord = ObjectToRecord(ncmbObject);
        if (oldRecord.score > record.score) return;
        ncmbObject = UpdateRecord(ncmbObject, record);
        await ncmbObject.SaveTaskAsync();
    }

    private async UniTask SaveScreenShotRecord(ScreenShotRecord record)
    {
        var obj = new NCMBObject(CLASS_NAME_SCREENSHOT);
        obj[nameof(ScreenShotRecord.userId)] = record.userId;
        obj[nameof(ScreenShotRecord.base64)] = record.base64;
        await obj.SaveTaskAsync();
    }

    private async UniTask UpdateScreenShotRecord(ScreenShotRecord record)
    {
        var query = new NCMBQuery<NCMBObject>(CLASS_NAME_SCREENSHOT);
        query.WhereEqualTo(nameof(ScreenShotRecord.userId), GetUserId());
        var result = await query.FindTaskAsync();
        if (result.Count < 1)
        {
            await SaveScreenShotRecord(record);
            return;
        }
        var ncmbObject = result[0];
        ncmbObject[nameof(ScreenShotRecord.base64)] = record.base64;
        await ncmbObject.SaveTaskAsync();
    }

    private Texture2D Base64ToTexture(string base64)
    {
        var texture = new Texture2D(1, 1);
        texture.hideFlags = HideFlags.HideAndDontSave;
        texture.LoadImage(Convert.FromBase64String(base64));
        return texture;
    }

    private string TextureToBase64(Texture2D texture)
    {
        var encode = texture.EncodeToJPG();
        Debug.Log(Convert.ToBase64String(encode));
        return Convert.ToBase64String(encode);
    }

    private NCMBObject RecordToObject(RankingRecord record)
    {
        var obj = new NCMBObject(CLASS_NAME_RANKING);
        return UpdateRecord(obj, record);
    }

    private NCMBObject UpdateRecord(NCMBObject obj, RankingRecord record)
    {
        obj[nameof(RankingRecord.userId)] = record.userId;
        obj[nameof(RankingRecord.userName)] = record.userName;
        obj[nameof(RankingRecord.score)] = record.score;
        obj[nameof(RankingRecord.stackedCount)] = record.stackedCount;
        return obj;
    }

    private RankingRecord ObjectToRecord(NCMBObject obj)
    {
        var record = new RankingRecord();
        record.userId = Convert.ToString(obj[nameof(RankingRecord.userId)]);
        record.userName = Convert.ToString(obj[nameof(RankingRecord.userName)]);
        record.score = Convert.ToInt32(obj[nameof(RankingRecord.score)]);
        record.stackedCount = Convert.ToInt32(obj[nameof(RankingRecord.stackedCount)]);
        return record;
    }

    public string GetUserId()
    {
        var userId = PlayerPrefs.GetString(KEY_USERID);
        if (!string.IsNullOrEmpty(userId)) return userId;
        userId = Guid.NewGuid().ToString("N");
        PlayerPrefs.SetString(KEY_USERID, userId);
        return userId;
    }

    public RankingRecord GetRecord(string userName, int score, int stackedCount)
    {
        var record = new RankingRecord() {
            userId = GetUserId(),
            userName = userName,
            score = score,
            stackedCount = stackedCount,
        };
        return record;
    }
}

public class RankingRecord
{
    public string userId;
    public string userName;
    public int score;
    public int stackedCount;
}

public class ScreenShotRecord
{
    public string userId;
    public string base64;
}