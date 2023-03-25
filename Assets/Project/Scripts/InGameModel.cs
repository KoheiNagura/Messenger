using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class InGameModel : MonoBehaviour
{
    public (Sprite sprite, int pt) NextつData { get; private set; }
    public int TotalPt => stacks.Sum(i => i.pt);
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private RandomFontSizeTable randomTable;
    private List<StackedつData> stacks;

    public void Initialzie()
    {
        randomTable.Initialize();
        stacks = new List<StackedつData>();
        NextつData = (GetRandomSprite(), GetFontSize());
    }

    public Sprite GetRandomSprite()
    {
        // フォントも直近5件ぐらいは重複なしにしたいかも
        return sprites.OrderBy(_ => System.Guid.NewGuid()).First();
    }

    public (Sprite sprite, int pt) GetNextつ()
    {
        var data = NextつData;
        NextつData = (GetRandomSprite(), GetFontSize());
        return data;
    }

    public (Sprite sprite, int pt) SwapNextつ(Sprite sprite, int pt)
    {
        var data = NextつData;
        NextつData = (sprite, pt);
        return data;
    }

    public int GetFontSize(bool excludeDuplicates = true)
        => randomTable.GetValue(excludeDuplicates);

    public void AddStacked(Sprite sprite, int pt)
    {
        var data = new StackedつData(sprite, sprite.name, pt);
        stacks.Add(data);
    }

    public void RemoveStacked(Sprite sprite, int pt)
    {
        var matched = stacks.Where(i => i.Sprite == sprite && i.pt == pt);
        if (matched.Count() < 1) return;
        stacks.Remove(matched.First());
    }

    public GameResult GetResult(Texture2D screenShot)
        => new GameResult(stacks, screenShot);
}