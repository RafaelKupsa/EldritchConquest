using System.Collections;
using System.Collections.Generic;
using Backend;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScreen : MonoBehaviour
{
    public Image decoration;
    
    public Sprite[] decorationSprites;

    private void Awake()
    {
        decoration.sprite = decorationSprites.Choice();
        decoration.SetNativeSize();
    }

    void Start()
    {
        var hasAltar = !(Global.Altar is null);

        if (hasAltar)
        {
            Global.NextScene = Global.Altar.GetMiniGame() switch
            {
                NameGame miniGame => "NameGame",
                IconGame miniGame => "IconGame",
                SacrificeGame miniGame => "SacrificeGame",
                _ => "Altar"
            };
        }
        else
        {
            Global.NextScene = "Map";
        }
    }

    public void Continue()
    {
        SceneManager.LoadScene($"Scenes/{Global.NextScene}");
    }
}
