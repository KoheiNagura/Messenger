using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "CustomInputValidator", menuName = "Messenger/CustomInputValidator")]
public class CustomInputValidator : TMP_InputValidator
{
    [SerializeField] private TMP_FontAsset fontAsset;
    private HashSet<char> vailedChars;

    public override char Validate(ref string text, ref int pos, char ch)
    {
        vailedChars ??= fontAsset.characterTable
            .Select(i => (char)i.unicode).ToHashSet();
        if (vailedChars.Contains(ch)) return ch;
        return (char)0;
    }
}