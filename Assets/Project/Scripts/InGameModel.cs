using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class InGameModel : MonoBehaviour
{
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private RandomFontSizeTable randomTable;
    private List<StackedつData> stacks;

    public void Initialzie()
    {
        randomTable.Initialize();
        stacks = new List<StackedつData>();
    }

    public Sprite GetRandomSprite()
    {
        // フォントも直近5件ぐらいは重複なしにしたいかも
        return sprites.OrderBy(_ => System.Guid.NewGuid()).First();
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