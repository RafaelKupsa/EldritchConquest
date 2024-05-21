using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Backend;
using UnityEngine;
using UnityEngine.UI;

public class FeatureSlot : MonoBehaviour
{
    public int index;

    private IconGameUI _iconGameUI;

    void Start()
    {
        _iconGameUI = FindObjectOfType<IconGameUI>();
    }

    public void Release()
    {
        if (Global.SelectedID is null) return;
        
        if (((IconGame)Global.MiniGame).CanSetFeature(index, Global.SelectedID)){
            ((IconGame)Global.MiniGame).SetFeature(index, Global.SelectedID);
        }
        Destroy(Global.Selected);
        Global.SelectedID = null;
        Global.Selected = null;

        _iconGameUI.UpdateIcon();
    }
}
