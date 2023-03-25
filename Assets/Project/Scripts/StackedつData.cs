using UnityEngine;

public class StackedつData {
    public Sprite Sprite { get; private set; }
    public string FontFamily { get; private set; }
    public int pt { get; private set; }

    public StackedつData(Sprite sprite, string fontFamily, int pt)
    {
        Sprite = sprite;
        FontFamily = fontFamily;
        this.pt = pt;
    }
}