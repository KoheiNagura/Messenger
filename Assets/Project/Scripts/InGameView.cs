using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameView : MonoBehaviour
{
    [SerializeField] private Text fontNameLabel, lifeLabel;
    [SerializeField] private NexつComponent nexつ;

    public void SetFontNameLabel(string name)
        => fontNameLabel.text = name;

    public void SetNexつValue(Sprite sprite, int pt)
        => nexつ.SetValue(sprite, pt);

    public void SetLifeLabel(int life)
        => lifeLabel.text = $"いのち\n{life}つ";
}
