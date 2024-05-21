using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class NameBar : MonoBehaviour
{
    public GameObject letterPrefab;

    private Dictionary<string, Sprite> _letterSprites;

    void Awake()
    {
        _letterSprites = new List<string>
        {
            "a", "aa", "ai", "au", "b", "bh", "c", "ch",
            "d", "dh", "e", "ee", "f", "fh", "g", "gh", 
            "h", "i", "ii", "k", "kh", "l", "lh", "ly", 
            "m", "mh", "n", "ng", "ngh", "nh", "ny", 
            "o", "oi", "oo", "p", "ph", "r", "rh", "s", 
            "sh", "t", "th", "u", "ui", "uu", "v", "w", 
            "x", "y", "z", "zh", "'", "'h", "-"
        }.ToDictionary(x => x, x => Resources.Load<Sprite>($"Sprites/ng_letter_{x}"));
    }
    
    public void Populate()
    {
        foreach (var l in Global.Altar.GetNameAsLetterList())
        {
            var letter = Instantiate(letterPrefab, transform, false);
            letter.GetComponent<Image>().sprite = _letterSprites[l];
        }
    }
}
