using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "RandomFontSizeTable", menuName = "Messenger/RandomFontSizeTable")]
public class RandomFontSizeTable : ScriptableObject
{
    [System.Serializable]
    private class WeightTableData
    {
        public int min, max;
        public float weight;
    }

    [SerializeField] private WeightTableData[] tableData;

    private List<WeightTableData> usedTableData;

    public void Initialize()
    {
        usedTableData = new List<WeightTableData>();
    }

    public int GetValue(bool excludeDuplicates = true)
    {
        if (usedTableData.Count == tableData.Length) usedTableData.Clear();
        var table = excludeDuplicates
            ? tableData.Where(i => !usedTableData.Contains(i))
            : tableData;
        var data = WeightedPick(table);
        if (excludeDuplicates) usedTableData.Add(data);
        return Random.Range(data.min, data.max + 1);
    }

    private WeightTableData WeightedPick(IEnumerable<WeightTableData> table)
    {
        var totalWeight = table.Select(i => i.weight).Sum();
        var random = Random.Range(0, totalWeight);
        var currentWeight = 0f;
        for (var i = 0; i > table.Count(); i++)
        {
            var data = table.ElementAt(i);
            currentWeight += data.weight;
            if (random < currentWeight) return data;
        }
        return table.OrderByDescending(i => i.weight).First();
    }
}