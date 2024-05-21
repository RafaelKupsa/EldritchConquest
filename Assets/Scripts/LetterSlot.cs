using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Backend;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LetterSlot : MonoBehaviour
{
    public int index;

    private Dictionary<string, Sprite> _letterSprites;

    private void Awake()
    {

        _letterSprites = new List<string>
            {
                "A", "a", "B", "b", "C", "c", "D", "d", "E", "e", "F", "f", "G", "g", "H", "h",
                "I", "i", "J", "j", "K", "k", "L", "l", "M", "m", "N", "n", "O", "o", "P", "p",
                "Q", "q", "R", "r", "S", "s", "T", "t", "U", "u", "V", "v", "W", "w", "X", "x",
                "Y", "y", "Z", "z", "-", "'", ",", ".", "?", "!", "%", "0", "1", "2", "3", "4",
                "5", "6", "7", "8", "9", "AA", "AI", "AU", "BH", "CH", "DH", "EE", "FH", "GH",
                "II", "KH", "LH", "LY", "MH", "NG", "NGH", "NH", "NY", "OO", "OI", "PH", "RH",
                "SH", "TH", "UU", "UI", "ZH", "'H"
            }
            .Zip(Resources.LoadAll<Sprite>($"Sprites/letters_single").Concat(Resources.LoadAll<Sprite>("Sprites/letters_multiple")), (l, s) => new { l, s }).ToDictionary(pair => pair.l, pair => pair.s);
    }

    public void Release()
    {
        if (Global.SelectedID is null) return;

        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(0).GetComponent<Image>().sprite = _letterSprites[Global.SelectedID.ToUpper()];
        
        ((NameGame)Global.MiniGame).SetLetter(index, Global.SelectedID);
        Destroy(Global.Selected);
        Global.SelectedID = null;
        Global.Selected = null;
    }
}
