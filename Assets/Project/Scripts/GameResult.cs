using UnityEngine;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
public class GameResult {
    private const string ATTEMPTS_SAVEKEY = "Attempts";
    public ReadOnlyCollection<StackedつData> Stacks { get; private set; }
    public Texture2D ScreenShot { get; private set; }
    public int Attempts { get; private set; }
    public int TotalPt => Stacks.Sum(i => i.pt);

    public GameResult(IList<StackedつData> stacks, Texture2D screenShot)
    {
        Stacks = new ReadOnlyCollection<StackedつData>(stacks);
        ScreenShot = screenShot;
        Attempts = PlayerPrefs.GetInt(ATTEMPTS_SAVEKEY, 0);
        Attempts += 1;
        PlayerPrefs.SetInt(ATTEMPTS_SAVEKEY, Attempts);
    }
}