using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Backend;

public class GameManager : MonoBehaviour
{
    void Awake()
    {
        if (Global.Game is null)
        {
            Global.Game = File.Exists(Game.SavePath) ? Game.Load() : new Game();
            Global.Game.Save();
        }
        
        Global.Altar = Global.Game.GetAltar();
        if (!(Global.Altar is null))
        {
            Global.MiniGame = Global.Altar.GetMiniGame();
        }
    }

    private void OnApplicationQuit()
    {
        Global.Game.Save();
    }

    public static void Restart()
    {
        Global.Game = new Game();
    }
}
