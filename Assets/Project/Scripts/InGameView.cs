using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameView : MonoBehaviour
{
    [SerializeField] private Text fontNameLabel;

    public void SetFontNameLabel(string name)
        => fontNameLabel.text = name;
}
