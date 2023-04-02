using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System;
using System.Linq;
using NCMB;

[CreateAssetMenu(fileName = "RankingManager", menuName = "Messenger/RankingManager")]
public class RankingManager : ScriptableObject
{
    private const int LIMIT = 10;
    private const string CLASS_NAME = "ranking";
    private const string USERID_KEY = "userId";
    private const string OBJECTID_KEY = "objectId";

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
        var isFetching = true;
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
            isFetching = false;
        });
        await UniTask.WaitUntil(() => isFetching, cancellationToken: token.Token);
        return result.Select(i => ObjectToRecord(i)).ToList();
    }

    public async void Save(RankingRecord record)
    {
        if (!isInitalized) Initialize();
        var error = default(NCMBException);
        if (PlayerPrefs.HasKey(OBJECTID_KEY))
            error = await UpdateRecord(record);
        else error = await SaveRecord(record);
        if (error != null) Debug.LogError($"{error.ErrorCode} : {error.ErrorMessage}");
    }

    private async UniTask<NCMBException> SaveRecord(RankingRecord record)
    {
        var ncmbObject = RecordToObject(record);
        var error = await SaveRecord(ncmbObject);
        if (error == null) 
        {
            Debug.Log(ncmbObject.ObjectId);
            PlayerPrefs.SetString(OBJECTID_KEY, ncmbObject.ObjectId);
        }
        return error;
    }

    private async UniTask<NCMBException> SaveRecord(NCMBObject ncmbObject)
    {
        var isSending = true;
        var error = default(NCMBException);
        ncmbObject.SaveAsync(e =>
        {
            error = e;
            isSending = false;
        });
        Debug.Log(ncmbObject.ObjectId);
        await UniTask.WaitUntil(() => isSending, cancellationToken: token.Token);
        return error;
    }

    private async UniTask<NCMBException> UpdateRecord(RankingRecord record)
    {
        var isFetching = true;
        var objectId = PlayerPrefs.GetString(OBJECTID_KEY);  
        var ncmbObject = new NCMBObject(CLASS_NAME);
        var error = default(NCMBException);
        ncmbObject.ObjectId = objectId;
        ncmbObject.FetchAsync(e =>
        {
            error = e;
            isFetching = false;
        });
        if (error != null) return error;
        await UniTask.WaitUntil(() => isFetching, cancellationToken: token.Token);
        ncmbObject = UpdateRecord(ncmbObject, record);
        return await SaveRecord(ncmbObject);
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