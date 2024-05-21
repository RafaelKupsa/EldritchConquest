using System.Collections;
using System.Collections.Generic;
using Backend;
using UnityEngine;
using UnityEngine.UI;

public class PlateSlot : MonoBehaviour
{
    public SacrificeGameUI sacrificeGameUI;
    
    public void Release()
    {
        if (Global.SelectedID is null) return;

        ((SacrificeGame)Global.MiniGame).AddOrgan(Global.SelectedID);
        Destroy(Global.Selected);
        Global.SelectedID = null;
        Global.Selected = null;

        sacrificeGameUI.UpdatePlate();
    }
}
