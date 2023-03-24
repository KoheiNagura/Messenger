using UnityEngine;
using System.Linq;

public class InGameModel : MonoBehaviour
{
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private RandomFontSizeTable randomTable;

    public Sprite GetRandomSprite()
    {
        // フォントも直近5件ぐらいは重複なしにしたいかも
        return sprites.OrderBy(_ => System.Guid.NewGuid()).First();
    }

    public int GetFontSize(bool excludeDuplicates = true)
        => randomTable.GetValue(excludeDuplicates);
}