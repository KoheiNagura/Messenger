using UnityEngine;
using System.Collections.Generic;

public class StackedListComponent : MonoBehaviour
{
    [SerializeField] private StackedListCellComponent cellPrefab;
    [SerializeField] private RectTransform contentParent;
    private const int MIN_CELL_COUNT = 6;

    public void SetValue(IList<(int usedCount, Sprite sprite, string fontFamily)> stacks)
    {
        DestroyCells();
        var cellCount = stacks.Count > MIN_CELL_COUNT
            ? stacks.Count + 2
            : MIN_CELL_COUNT + 2;
        for (var i = 0; i < cellCount; i++)
        {
            var cell = Instantiate(cellPrefab);
            cell.transform.SetParent(contentParent, false);
            if (i < stacks.Count)
            {
                var data = stacks[i];
                cell.SetValue(data.usedCount, data.sprite, data.fontFamily);
            } 
            else
            {
                cell.SetValue(0, null, "");
            }
        }
    }

    public void DestroyCells()
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }
    }
}